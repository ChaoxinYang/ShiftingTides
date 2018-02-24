using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowBehaviour : MonoBehaviour
{
    private Vector3 arrowPlaceholderPosition, arrowPlaceholderRotation;
    public float penetrationStrength;
    private GameObject arrowPlaceholder;

    // Use this for initialization
    void Start()
    {
        arrowPlaceholder = Resources.Load("Prefabs/ArrowPlaceholder") as GameObject;
    }

    private void OnCollisionEnter(Collision other)
    {
        switch (other.gameObject.tag)
        {
         
            default:
                Debug.Log("Hitting: " + other.gameObject.tag);
                SetupArrowPlaceholder(other.contacts[0].point, other.relativeVelocity);
                break;
        }
    }

    // This method calls CopyPositionAndRotationForArrowPlaceholder.
    // Then it instantiates the ArrowPlaceholder with the values from CopyPositionAndRotationForArrowPlaceholder and destroys the current arrow.
    private void SetupArrowPlaceholder(Vector3 contactPoint, Vector3 hitSpeed)
    {
      
        Vector3 spawnPosition = contactPoint - hitSpeed*penetrationStrength;    
        arrowPlaceholderPosition = spawnPosition;
        arrowPlaceholderRotation = transform.eulerAngles;
        Instantiate(arrowPlaceholder, arrowPlaceholderPosition, arrowPlaceholder.transform.rotation = Quaternion.Euler(arrowPlaceholderRotation));
        Destroy(gameObject);
    }
}
