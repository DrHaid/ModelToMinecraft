using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectRotater : MonoBehaviour
{
  public GameObject voxelObject;
  
  public void RotateObject(bool clockwise)
  {
    voxelObject.transform.rotation *= Quaternion.Euler(0, clockwise ? 90 : -90, 0);  
  }
}
