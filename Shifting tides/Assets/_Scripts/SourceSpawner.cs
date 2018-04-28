using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SourceSpawner : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Arrow")
        {
            SpawnSourcePoint(collision.contacts[0].point, collision.relativeVelocity);
        }
    }

    private void SpawnSourcePoint(Vector3 contactPoint, Vector3 hitSpeed)
    {
        Vector3 spawnPosition = contactPoint - Vector3.one;
        Vector3 endPosition = contactPoint - hitSpeed / 5;

        GameObject sourcePoint = Instantiate(Resources.Load("Prefabs/Source") as GameObject, spawnPosition, Quaternion.identity);
        sourcePoint.GetComponent<SourcePoint>().Init(5, endPosition,4);
    }
}
