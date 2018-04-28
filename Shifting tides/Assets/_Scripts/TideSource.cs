using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TideSource : MonoBehaviour
{
    // Use this for initialization
    void Start()
    {

        GameObject tide = Instantiate(Resources.Load("Tide") as GameObject, transform.position, Quaternion.identity);
        tide.GetComponent<ShiftingTide>().Init(20f, 300f, 4);
    }

}
