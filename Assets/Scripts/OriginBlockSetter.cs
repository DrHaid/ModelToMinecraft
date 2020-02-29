using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OriginBlockSetter : MonoBehaviour
{
  public GameObject originBlock;
  public InputField xInput;
  public InputField yInput;
  public InputField zInput;

  public void RefreshOriginBlock()
  {
    originBlock.transform.position = new Vector3(int.Parse(xInput.text), int.Parse(yInput.text), int.Parse(zInput.text));
  }
}
