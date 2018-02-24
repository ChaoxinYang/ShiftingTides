
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

public class PlayerCamera : MonoBehaviour
{
    public float mouseSensitivity;

    [HideInInspector]
    public float mouseX, mouseY, cameraMouseY;
    private const float MAX_Y = 90.0f;
    private const float MIN_Y = -40.0f;
    private const float cameraMouseMIN_Y = -20.0f;
    [HideInInspector]
    public Camera cameraMain;
    public float smoothSpeed = 0.125f;
    public Vector3 offset;
    public Image cursor;
    public Sprite lockOnCursor, lockOffCursor;
    public GameObject shootTarget;
    public BasicMovement basicMovement;
    private float targetDistance;
    public Vector3 targetOffeset;
    public bool lockedOn;
    private GameObject nearestTarget;
    private RaycastHit hits;

    private void Start()
    {
        lockedOn = false;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        cursor.sprite = lockOffCursor;
        cameraMain = Camera.main;
        basicMovement = GetComponent<BasicMovement>();      
    }
    void Update()
    {
        mouseX += Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        mouseY -= Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;
        cameraMouseY += -Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;
        if (Input.GetMouseButtonDown(2))
        {
            if (lockedOn)
            {
                cursor.GetComponent<Image>().sprite = lockOffCursor;
                lockedOn = false;
                nearestTarget = null;
            }
            else {
                lockOnTarget();                           
            }  
                             
        }
        if (lockedOn)
        {
            Invoke("CheckIfTargetIsInVision", 1);

        }
    
    }

    private RaycastHit[] lookForTarget() {

        int layerMask = 1 <<13;
        RaycastHit[] hits;
        Vector3 fwd = transform.TransformDirection(Vector3.forward);
        hits = Physics.BoxCastAll(gameObject.transform.position, new Vector3(6, 5, 1), fwd, Quaternion.Euler(cameraMouseY, mouseX, 0.0f), 30f, layerMask);
        return hits;

    }

    private void CheckIfTargetIsInVision()
    {


        GameObject lastTarget = nearestTarget;
        RaycastHit[] hits;
        hits = lookForTarget();
        bool TargetInSight = false; 
        foreach (RaycastHit rh in hits)
        {
            if (rh.collider.gameObject == lastTarget)
            {
                TargetInSight = true;
                return;
            }
        }
        if (TargetInSight == false)
        {
            lockedOn = false;
            cursor.GetComponent<Image>().sprite = lockOffCursor;
        }
    }



    private void lockOnTarget()
    {
       
     
        RaycastHit[] hits;
        hits = lookForTarget();
        float closestDistanceSqr = Mathf.Infinity;
        if (hits.Length == 0) {
            cursor.GetComponent<Image>().sprite = lockOffCursor;
            lockedOn = false;
            return;

        }
        
        foreach (RaycastHit rh in hits)
        {
            Rigidbody ridg = rh.collider.gameObject.GetComponent<Rigidbody>();
            if (ridg != null)
            {
                Vector3 directionToTarget = rh.collider.gameObject.transform.position - gameObject.transform.position;
                float dSqrToTarget = directionToTarget.sqrMagnitude;
                
                if (dSqrToTarget < closestDistanceSqr)
                {

                    closestDistanceSqr = dSqrToTarget;
                    nearestTarget = rh.collider.gameObject;
                  
                }
            }
        }

        lockedOn = true;
        cursor.GetComponent<Image>().sprite = lockOnCursor;
    }

    void LateUpdate()
    {

        cameraMouseY = Mathf.Clamp(mouseY, cameraMouseMIN_Y, MAX_Y);
        mouseY = Mathf.Clamp(mouseY, MIN_Y, MAX_Y);
     
        Vector3 direction = new Vector3((1f * transform.localScale.z), 0.0f, (-5f * transform.localScale.x));
        Quaternion rotation = Quaternion.Euler(cameraMouseY, mouseX, 0.0f);

        Vector3 desiredPosition = transform.position + offset;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        cameraMain.fieldOfView = basicMovement.CameraView;
                
        basicMovement.bow.transform.LookAt(shootTarget.transform);
        cameraMain.transform.position = smoothedPosition + rotation * direction;
        cameraMain.transform.rotation = rotation;
        if (lockedOn) {
            shootTarget.transform.position = nearestTarget.transform.position;
        }
        if (!lockedOn)
        {
            shootTarget.transform.position = cameraMain.transform.position + cameraMain.transform.forward * 30f + cameraMain.transform.right * targetOffeset.x
            + cameraMain.transform.up * targetOffeset.y;
        }

        cursor.transform.position = Camera.main.WorldToScreenPoint(shootTarget.transform.position);
    }
}
