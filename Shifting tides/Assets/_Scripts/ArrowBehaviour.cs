using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowBehaviour : MonoBehaviour
{
    private Vector3 arrowPlaceholderPosition, arrowPlaceholderRotation;
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
            case "Ground":
                Debug.Log("Hitting: " + other.gameObject.tag);
                SetupArrowPlaceholder();
                break;
            case "Wall":
                Debug.Log("Hitting: " + other.gameObject.tag);
                SetupArrowPlaceholder();
                break;
        }
    }

    // This method gets the current postion and rotation of the arrow and hold them in
    // two separate variables.
    private void CopyPositionAndRotationForArrowPlaceholder()
    {
        arrowPlaceholderPosition = transform.position;
        arrowPlaceholderRotation = transform.eulerAngles;
    }

    // This method calls CopyPositionAndRotationForArrowPlaceholder.
    // Then it instantiates the ArrowPlaceholder with the values from CopyPositionAndRotationForArrowPlaceholder and destroys the current arrow.
    private void SetupArrowPlaceholder()
    {
        CopyPositionAndRotationForArrowPlaceholder();
        Instantiate(arrowPlaceholder, arrowPlaceholderPosition, arrowPlaceholder.transform.rotation = Quaternion.Euler(arrowPlaceholderRotation));
        Destroy(gameObject);
    }
}
