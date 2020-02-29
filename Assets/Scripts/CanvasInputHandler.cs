using SFB;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Handles all button presses on the Canvas
/// </summary>
public class CanvasInputHandler : MonoBehaviour
{
  public ModelToMinecraftManager modelToMinecraftManager;
  public Text objLabel;
  public InputField sizeInput;

  public void OnOpenObjButton()
  {
    var paths = StandaloneFileBrowser.OpenFilePanel("Open OBJ file", "", "obj", false);
    if (paths.Length == 0)
    {
      Debug.LogWarning("File not found");
      return;
    }
    modelToMinecraftManager.LoadObj(paths[0]);
    objLabel.text = paths[0];
  }

  public void OnGenerateButton()
  {
    if (modelToMinecraftManager.srcMesh == null)
    {
      Debug.LogWarning("No OBJ loaded");
      return;
    }
    modelToMinecraftManager.GenerateBlocks(int.Parse(sizeInput.text));
  }

  public void OnSavePackButton()
  {
    var path = StandaloneFileBrowser.SaveFilePanel("Save DataPack", "", "Model2Minecraft.zip", "zip");
    if (string.IsNullOrEmpty(path))
    {
      Debug.LogWarning("Path invalid");
      return;
    }
    if(Path.GetExtension(path) != ".zip")
    {
      Debug.LogWarning("Target file not ZIP");
      return;
    }
    if (!modelToMinecraftManager.voxelObject.activeSelf)
    {
      Debug.LogWarning("No blocks generated");
      return;
    }
    modelToMinecraftManager.SaveDataPack(path);
  }
}
