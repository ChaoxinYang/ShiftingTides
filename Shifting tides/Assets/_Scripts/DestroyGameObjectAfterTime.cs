using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyGameObjectAfterTime : MonoBehaviour
{
    public float destroyObjectAfterSeconds;

    // Update is called once per frame
    void Update()
    {
        Destroy(gameObject, destroyObjectAfterSeconds);
    }
}
