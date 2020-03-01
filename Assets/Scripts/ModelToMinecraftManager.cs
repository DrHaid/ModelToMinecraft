using MeshVoxelizerProject;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using UnityEngine;

public class ModelToMinecraftManager : MonoBehaviour
{
  public GameObject sourceObject;
  public GameObject voxelObject;
  public GameObject originBlock;
  [HideInInspector]public Mesh srcMesh;

  private MeshVoxelizer meshVoxelizer;

  /// <summary>
  /// Load OBJ file from <paramref name="path"/> and assign mesh to the source object.
  /// </summary>
  /// <param name="path">Absolute path to the OBJ file</param>
  public void LoadObj(string path) {
    sourceObject.SetActive(true);
    srcMesh = FastObjImporter.Instance.ImportFile(path);
    sourceObject.GetComponent<MeshFilter>().mesh = srcMesh;
    voxelObject.SetActive(false);
  }

  /// <summary>
  /// Uses <see cref="MeshVoxelizer"/> to build voxel object collection from the source mesh.
  /// </summary>
  /// <param name="size">xyz size of the voxel object collection</param>
  public void GenerateBlocks(int size)
  {
    Color defaultColor = gameObject.GetComponent<BlockDrawer>().selectedColor;
    voxelObject.SetActive(true);
    var bounds = srcMesh.bounds;
    var max = bounds.extents.x;
    if (max < bounds.extents.y)
      max = bounds.extents.y;
    if (max < bounds.extents.z)
      max = bounds.extents.z;
    var center = srcMesh.bounds.center;
    Box3 box = new Box3(center - new Vector3(max, max, max), center + new Vector3(max, max, max));
    meshVoxelizer = new MeshVoxelizer(size, size, size);
    meshVoxelizer.Voxelize(srcMesh.vertices, srcMesh.triangles, box);

    for (int i = 0; i < meshVoxelizer.Voxels.GetLength(0); i++)
    {
      for (int j = 0; j < meshVoxelizer.Voxels.GetLength(1); j++)
      {
        for (int k = 0; k < meshVoxelizer.Voxels.GetLength(2); k++)
        {
          if (meshVoxelizer.Voxels[i, j, k] == 1)
          {
            GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.transform.parent = voxelObject.transform;
            cube.transform.localPosition = new Vector3(i, j, k);
            cube.tag = "Voxel";
            cube.GetComponent<MeshRenderer>().material.color = defaultColor;
          }
        }
      }
    }
    sourceObject.SetActive(false);
  }

  /// <summary>
  /// Converts the voxel object collection into setblock commands.
  /// </summary>
  /// <returns>Line by line content of the mcfunction file</returns>
  private List<string> GetMcFuncContent()
  {
    var lines = new List<string>();
    foreach (Transform child in voxelObject.transform)
    {
      Color color = child.GetComponent<MeshRenderer>().material.color;
      string blockName = BlockMaterialMapping.blockMaterialMap.FirstOrDefault(x => x.Value == color).Key;
      Vector3 minecraftBlockPosition = 
        new Vector3(
          child.position.x - originBlock.transform.position.x,
          child.position.y - originBlock.transform.position.y,
          -(child.position.z - originBlock.transform.position.z)
        );
      string line = $"setblock ~{minecraftBlockPosition.x} ~{minecraftBlockPosition.y} ~{minecraftBlockPosition.z} {blockName}";
      lines.Add(line);
    }
    return lines;
  }

  /// <summary>
  /// Create final DataPack ZIP archive in memory and save it to file.
  /// </summary>
  /// <param name="path">Output ZIP file path</param>
  public void SaveDataPack(string path)
  {
    using (var memoryStream = new MemoryStream())
    {
      using (var archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
      {
        var packFile = archive.CreateEntry("pack.mcmeta");

        using (var entryStream = packFile.Open())
        using (var streamWriter = new StreamWriter(entryStream))
        {
          StringBuilder sb = new StringBuilder();

          sb.AppendLine("{");
          sb.AppendLine("    \"pack\": {");
          sb.AppendLine("        \"pack_format\": 4,");
          sb.AppendLine("        \"description\": \"Model2Minecraft by DrHaid\"");
          sb.AppendLine("    }");
          sb.AppendLine("}");

          streamWriter.Write(sb.ToString());
        }

        var mcfuncFile = archive.CreateEntry("data/meshtominecraft/functions/buildmodel.mcfunction");
        using (var entryStream = mcfuncFile.Open())
        using (var streamWriter = new StreamWriter(entryStream))
        {
          foreach(string line in GetMcFuncContent())
          {
            streamWriter.WriteLine(line);
          }
        }
      }

      using (var fileStream = new FileStream(path, FileMode.Create))
      {
        memoryStream.Seek(0, SeekOrigin.Begin);
        memoryStream.CopyTo(fileStream);
      }
    }
  }
}

