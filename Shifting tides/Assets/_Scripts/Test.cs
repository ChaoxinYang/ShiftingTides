using UnityEngine;
using System.Collections;

public class Test : MonoBehaviour
{
    public Vector3 point = new Vector3(0.5f, 0.5f, 0);

    public void Update()
    {
        Ray ray = Camera.main.ViewportPointToRay(point);
        Color col = Color.red;
        if (Physics.Raycast(ray)) col = Color.green;

        Debug.DrawRay(ray.origin, ray.direction * 100, col);
    }
}