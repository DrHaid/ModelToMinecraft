using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BlockDrawer : MonoBehaviour
{
  public InputField addBlockField;
  public Dropdown blockSelection;
  public Image colorPreview;
  
  [HideInInspector]public Color selectedColor;

  void Start()
  {
    BlockMaterialMapping.AddBlock("minecraft:quartz");
    RefreshDropDown();
  }

  void Update()
  {
    if (Input.GetMouseButtonDown(0))
    {
      RaycastHit hit;
      Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
      Physics.Raycast(ray, out hit);
      if (hit.collider == null)
      {
        return;  
      }
      if(hit.collider.gameObject.tag != "Voxel")
      {
        return;
      }
      hit.collider.gameObject.GetComponent<MeshRenderer>().material.color = selectedColor;
    }  
  }

  public void OnAddBlockButton()
  {
    if (string.IsNullOrEmpty(addBlockField.text))
    {
      return;
    }
    BlockMaterialMapping.AddBlock(addBlockField.text);
    addBlockField.text = "";
    RefreshDropDown();
    blockSelection.value = 0;
  }

  private void RefreshDropDown()
  {
    blockSelection.ClearOptions();
    List<string> blockList = new List<string>(BlockMaterialMapping.blockMaterialMap.Keys);
    blockList.Reverse();
    blockSelection.AddOptions(blockList);
    RefreshSelectedColor();
  }

  public void RefreshSelectedColor()
  {
    selectedColor = BlockMaterialMapping.blockMaterialMap[blockSelection.options[blockSelection.value].text];
    colorPreview.color = selectedColor;
  }
}

public class BlockMaterialMapping
{
  public static Dictionary<string, Color> blockMaterialMap = new Dictionary<string, Color>();

  public static void AddBlock(string blockName)
  {
    Color color = new Color(
      Random.Range(0f, 1f),
       Random.Range(0f, 1f),
       Random.Range(0f, 1f)
    );
    blockMaterialMap.Add(blockName, color);
  }
}
