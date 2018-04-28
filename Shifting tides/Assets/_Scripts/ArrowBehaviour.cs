using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowBehaviour : TimeBoundGameObject
{
    private Vector3 arrowPlaceholderRotation;
    public float penetrationStrength;
    private GameObject arrowPlaceholder;
    private Quaternion localRotation;
    // Use this for initialization
    public override void Start()
    {
        base.Start();
        arrowPlaceholder = Resources.Load("Prefabs/ArrowPlaceholder") as GameObject;
    }
    public override void localPhysicsUpdate()
    {   //Rotate arrow towards ground 
        base.localPhysicsUpdate();

        float angle = Mathf.LerpAngle(transform.eulerAngles.x, 90f, Time.fixedDeltaTime - (rbObject.velocity.y / 150));
        transform.eulerAngles = new Vector3(angle, transform.eulerAngles.y, transform.eulerAngles.z);
    }
    private void OnCollisionEnter(Collision other)
    {
        switch (other.gameObject.tag)
        {
            default:
                SetupArrowPlaceholder(other.contacts[0].point, rbObject.velocity * penetrationStrength);
                break;
        }
    }
    // This method calls CopyPositionAndRotationForArrowPlaceholder.
    // Then it instantiates the ArrowPlaceholder with the values from CopyPositionAndRotationForArrowPlaceholder and destroys the current arrow.
    private void SetupArrowPlaceholder(Vector3 contactPoint, Vector3 hitSpeed)
    {
        //Vector3 nomalizedHitspeed = Vector3.Normalize(hitSpeed);
        Vector3 spawnPosition = contactPoint - hitSpeed;
        arrowPlaceholderRotation = transform.eulerAngles;
        Instantiate(arrowPlaceholder, spawnPosition, arrowPlaceholder.transform.rotation = Quaternion.Euler(arrowPlaceholderRotation));
        Destroy(gameObject);
    }
}
