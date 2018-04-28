
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
    private const float MAX_Y = 60;
    private const float MIN_Y = -60.0f;
    private const float cameraMouseMIN_Y = -20.0f;
    [HideInInspector]
    public Camera cameraMain;
    public float smoothSpeed = 10f;

    public Image cursor;
    public Sprite lockOnCursor, lockOffCursor;
    public Transform player;
    public BasicMovement basicMovement;

    public GameObject shootTarget;
    private float targetDistance;
    public Vector3 targetOffeset;
    public bool lockedOn;
    private GameObject nearestTarget;
    private RaycastHit hits;

    [HideInInspector]
    public float angleH;
    private float angleV;
    public Vector3 pivotOffset;
    public Vector3 camOffset;
    private Vector3 smoothPivotOffset;
    private Vector3 smoothCamOffset;
    private Vector3 targetPivotOffset;
    private Vector3 targetCamOffset;
    [HideInInspector]
    public float targetFOV;
    private float relCameraPosMag;
    private Vector3 relCameraPos;

    private void Start()
    {
        lockedOn = false;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        cursor.sprite = lockOffCursor;
        cameraMain = Camera.main;
        relCameraPos = transform.position - player.position;
        relCameraPosMag = relCameraPos.magnitude - 0.5f;
        transform.position = player.position + Quaternion.identity * pivotOffset + Quaternion.identity * camOffset;
        smoothPivotOffset = pivotOffset;
        smoothCamOffset = camOffset;
        angleH = player.eulerAngles.y;
        targetFOV = 60f;
        ResetTargetOffsets();
    }

    void Update()
    {       
        angleH += Mathf.Clamp(Input.GetAxis("Mouse X"), -1, 1) * mouseSensitivity;
        angleV += Mathf.Clamp(Input.GetAxis("Mouse Y"), -1, 1) * mouseSensitivity;
        angleV = Mathf.Clamp(angleV, MIN_Y, MAX_Y);

        Quaternion aimRotation = Quaternion.Euler(-angleV, angleH, 0);
        Quaternion camYRotation = Quaternion.Euler(0, angleH, 0);
        transform.rotation = aimRotation;

        Vector3 baseTempPosition = player.position + camYRotation * targetPivotOffset;
        Vector3 noCollisionOffset = targetCamOffset;
       
        for (float zOffset = targetCamOffset.z; zOffset <= 0; zOffset += 0.5f)
        {
            noCollisionOffset.z = zOffset;
            if (DoubleViewingPosCheck(baseTempPosition + aimRotation * noCollisionOffset, Mathf.Abs(zOffset)) || zOffset == 0)
            {
                
                break;
            }
        }
        smoothPivotOffset = Vector3.Lerp(smoothPivotOffset, targetPivotOffset, smoothSpeed * Time.deltaTime);
        smoothCamOffset = Vector3.Lerp(smoothCamOffset, noCollisionOffset, smoothSpeed * Time.deltaTime);
        transform.position = player.position + camYRotation * smoothPivotOffset + aimRotation * smoothCamOffset;
        transform.GetComponent<Camera>().fieldOfView = Mathf.Lerp(transform.GetComponent<Camera>().fieldOfView, determineCurrentFOV(), Time.deltaTime);


        if (Input.GetMouseButtonDown(2))
        {
            if (lockedOn)
            {
                cursor.GetComponent<Image>().sprite = lockOffCursor;
                lockedOn = false;
                nearestTarget = null;
            }
            else
            {
                lockOnTarget();
            }

        }

        if (lockedOn)
        {
            Invoke("CheckIfTargetIsInVision", 1);
        }
    }
    public float determineCurrentFOV() {
        float targetFOV = 60f;
        if (!basicMovement.isAiming) {
            if (basicMovement.maxInput >= 0f && basicMovement.maxInput <= 0.4f)
            {
                return targetFOV;
            }
            else if (basicMovement.maxInput > 0.4f && basicMovement.maxInput <= 0.7f)
            {
                return targetFOV = 65f;
            }
            else if (basicMovement.maxInput > 0.7f) {
                return targetFOV = 130f;
            }
        }
        else{
            targetFOV = 40;
        }
        return targetFOV;
    }
    public void ResetTargetOffsets()
    {
        targetPivotOffset = pivotOffset;
        targetCamOffset = camOffset;
    }
    bool DoubleViewingPosCheck(Vector3 checkPos, float offset)
    {
        float playerFocusHeight = player.GetComponent<BoxCollider>().center.y;
        return ViewingPosCheck(checkPos, playerFocusHeight) && ReverseViewingPosCheck(checkPos, playerFocusHeight, offset);
    }
    bool ViewingPosCheck(Vector3 checkPos, float deltaPlayerHeight)
    {
        RaycastHit hit;

        // If a raycast from the check position to the player hits something...
        if (Physics.Raycast(checkPos, player.position + (Vector3.up * deltaPlayerHeight) - checkPos, out hit, relCameraPosMag))
        {
            // ... if it is not the player...
            if (hit.transform != player && !hit.transform.GetComponent<Collider>().isTrigger)
            {
                // This position isn't appropriate.
                return false;
            }
        }
        // If we haven't hit anything or we've hit the player, this is an appropriate position.
        return true;
    }

    // Check for collision from player to camera.
    bool ReverseViewingPosCheck(Vector3 checkPos, float deltaPlayerHeight, float maxDistance)
    {
        RaycastHit hit;

        if (Physics.Raycast(player.position + (Vector3.up * deltaPlayerHeight), checkPos - player.position, out hit, maxDistance))
        {
            if (hit.transform != player && hit.transform != transform && !hit.transform.GetComponent<Collider>().isTrigger)
            {
                return false;
            }
        }
        return true;
    }

    private RaycastHit[] lookForTarget()
    {
        int layerMask = 1 << 13;
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

        if (!TargetInSight)
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
        if (hits.Length == 0)
        {
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
       basicMovement.bow.transform.LookAt(shootTarget.transform);     
        if (lockedOn)
        {
            shootTarget.transform.position = nearestTarget.transform.position;
        }
        else
        {
            shootTarget.transform.position = cameraMain.transform.position + cameraMain.transform.forward * 30f + cameraMain.transform.right * targetOffeset.x
            + cameraMain.transform.up * targetOffeset.y;
        }

        cursor.transform.position = Camera.main.WorldToScreenPoint(shootTarget.transform.position);
    }
}
