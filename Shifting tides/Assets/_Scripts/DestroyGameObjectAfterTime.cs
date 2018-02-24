using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyGameObjectAfterTime : MonoBehaviour
{
    public float destroyObjectAfterSeconds;

    private void Start()
    {
        Destroy(gameObject, destroyObjectAfterSeconds);
    }

}
