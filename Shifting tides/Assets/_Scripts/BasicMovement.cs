using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BasicMovement : MonoBehaviour
{
    private Rigidbody rbPlayer;
    public Animator aniPlayer;
    private Vector3 currentMotion;
    private float slopeAngle;
    private Camera cameraMain;
    [HideInInspector]
    public bool onGround, ClimblingWall;
    public GameObject bow, bowMesh, UI, gameManagerObject;
    public GameObject[] dashesImages;
    private GameObject arrow;
    //0: bow , 1: The Source
    private bool[] skillObtained = new bool[10];
    public Ui ui;
    public PlayerCamera plyCamera;
    public PlayerResourcesManager plyResourceMng;
    private GameManager gameMng;
    public float defaultMoveForce, moveForce, speedLimit, runLimit, moveLimit, dashForce, DashLimit, jumpVel, gravity, maxInput, startArrowSpeed, maxArrowSpeed;
    private float h, v, inputSpeed,arrowSpeed;
    public bool isAiming;
    private Vector3 lastDirection;

    void Start()
    {   
        rbPlayer = gameObject.GetComponent<Rigidbody>();
        gameMng = gameManagerObject.GetComponent<GameManager>();
        cameraMain = Camera.main;
        arrow = Resources.Load("Prefabs/Arrow") as GameObject;
        resetArrowSpeed();
    }

    void Update()
    {
        //moveHorizontal = Input.GetAxisRaw("Horizontal") * speed * cameraMain.transform.right;
        //moveVertical = Input.GetAxisRaw("Vertical") * speed * cameraMain.transform.forward;
        currentMotion = Input.GetAxisRaw("Vertical") * moveForce * gameManagerObject.transform.forward + Input.GetAxisRaw("Horizontal") * moveForce * gameManagerObject.transform.right;
        h = Input.GetAxis("Horizontal");
        v = Input.GetAxis("Vertical");
        inputSpeed = Vector2.ClampMagnitude(new Vector2(h, v), maxInput).magnitude;
        aniPlayer.SetFloat("Speed", inputSpeed);
    
        if (!isAiming)
        {
            rotate(h, v);
        }

        if (Input.GetKeyDown(KeyCode.F) && plyResourceMng.Dashes > 0 && skillObtained[1])
        {
            StartCoroutine(Dash());
        }
        if (Input.GetKeyDown(KeyCode.R)) {
            gameMng.enableTimeStop();
        }

        if (Input.GetKeyDown(KeyCode.Space) && plyResourceMng.JumpsLeft > 0)
        {
            Jumping();
        }

        if (Input.GetKey(KeyCode.LeftShift))
        {
            changeMoveSpeedLimit(true,moveLimit);
        }

        if (!Input.GetKey(KeyCode.LeftShift))
        {
            changeMoveSpeedLimit(false,runLimit);
        }

        if (Input.GetMouseButton(1) && plyResourceMng.Arrows > 0 && skillObtained[0])
        {
            ChargeUpArrow();
        }
        if (Input.GetMouseButtonUp(1) && isAiming)
        {
            ShootArrow();
        }

    }

    void FixedUpdate()
    {
        handleRigidBodyMovement();
    }

    private void handleRigidBodyMovement() {
        gravity = rbPlayer.velocity.y >= -1 ? 0 : gravity -= 1.2f;
        rbPlayer.AddForce(currentMotion + Vector3.up * (gravity + slopeAngle), ForceMode.Acceleration);
        Vector2 horizontalVelocity = new Vector2(rbPlayer.velocity.x, rbPlayer.velocity.z);
        horizontalVelocity = Vector2.ClampMagnitude(horizontalVelocity, speedLimit);
        rbPlayer.velocity = new Vector3(horizontalVelocity.x, rbPlayer.velocity.y, horizontalVelocity.y);
    }

    private void resetArrowSpeed() {
        arrowSpeed = startArrowSpeed;
    }

    private void aimRotate()
    {
        Vector3 forward = cameraMain.transform.TransformDirection(Vector3.forward);
        // Player is moving on ground, Y component of camera facing is not relevant.
        forward.y = 0.0f;
        forward = forward.normalized;

        // Always rotates the player according to the camera horizontal rotation in aim mode.
        Quaternion targetRotation = Quaternion.Euler(0, plyCamera.angleH, 0);

        float minSpeed = Quaternion.Angle(transform.rotation, targetRotation) * 0.02f;

        // Rotate entire player to face camera.
        lastDirection = forward;
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, minSpeed * Time.deltaTime);
    }

    private void rotate(float horizontal, float vertical)
    {

        Vector3 desiredDirection;
        Vector3 cameraForward = cameraMain.transform.TransformDirection(Vector3.forward);

        cameraForward.y = 0f;
        cameraForward = cameraForward.normalized;

        Vector3 right = new Vector3(cameraForward.z, 0, -cameraForward.x);
        desiredDirection = cameraForward * vertical + right * horizontal;
        if (isMoving() && desiredDirection != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(desiredDirection);
            Quaternion newRotation = Quaternion.Slerp(rbPlayer.rotation, targetRotation, 0.05f);
            rbPlayer.MoveRotation(newRotation);
            LastDirection = desiredDirection;
        }
        if (!isMoving())
        {
            Repositioning();
        }
    }

    private void Repositioning()
    {
        if (lastDirection != Vector3.zero)
        {
            LastDirection = new Vector3(LastDirection.x, 0, LastDirection.z);
            Quaternion targetRotation = Quaternion.LookRotation(lastDirection);
            Quaternion newRotation = Quaternion.Slerp(rbPlayer.rotation, targetRotation, 0.05f);
            rbPlayer.MoveRotation(newRotation);
        }
    }

    private bool isMoving()
    {
        return (h != 0) || (v != 0);
    }

    private void Jumping()
    {
        rbPlayer.AddForce((Vector3.up * jumpVel), ForceMode.Impulse);
        plyResourceMng.JumpsLeft -= 1;
    }

    private void changeMoveSpeedLimit(bool isWalking,float targetLimit)
    {
        speedLimit = Mathf.Lerp(speedLimit, targetLimit, Time.deltaTime);
        maxInput = isWalking ? maxInput = Mathf.Lerp(maxInput, 0.2f, Time.deltaTime) : maxInput = Mathf.Lerp(maxInput, 0.5f, Time.deltaTime);
    }

   private IEnumerator Dash()
    {
        Debug.Log("Dashing");
        ui.dashCharges[plyResourceMng.Dashes - 1].enabled = false;
        plyResourceMng.Dashes -= 1;
        moveForce = dashForce;
        speedLimit = DashLimit;
        maxInput = 1f;
        yield return new WaitForSeconds(0.3f);
        moveForce = defaultMoveForce;
        while (speedLimit > runLimit+0.2f)
        {
            speedLimit = Mathf.Lerp(speedLimit, runLimit, Time.deltaTime*5f);
            maxInput = Mathf.Lerp(maxInput, 0.5f, Time.deltaTime * 5f);
            yield return new WaitForSeconds(0.03f);
        }
        Invoke("chargeUpDash", 0.7f);
        yield break;
    }

    private void chargeUpDash()
    {
        ui.dashCharges[plyResourceMng.Dashes].enabled = true;
        plyResourceMng.Dashes += 1;
    }
        
    private void ShootArrow()
    {
        plyResourceMng.Arrows -= 1;
        GameObject Arrow = Instantiate(arrow, bow.transform.position, bow.transform.rotation);
        Arrow.GetComponent<Rigidbody>().AddForce(Arrow.transform.forward * arrowSpeed, ForceMode.Impulse);
        Arrow.GetComponent<ArrowBehaviour>().Initialize(false);
        if (isAiming) isAiming = !isAiming;
        resetArrowSpeed();
    }

    private void ChargeUpArrow()
    {
        aimRotate();
        arrowSpeed = Mathf.Lerp(arrowSpeed, maxArrowSpeed,Time.deltaTime);
        if (!isAiming) isAiming = !isAiming;
    }

    private void OnCollisionStay(Collision collision)
    {
        foreach (ContactPoint contact in collision.contacts)
        {
            if (Vector3.Angle(contact.normal, Vector3.up) < 45 && collision.gameObject.tag == "Slope")
            {
                slopeAngle = 5f;
            }
        }
    }

    void OnCollisionEnter(Collision other)
    {
        foreach (ContactPoint contact in other.contacts)
        {
            if (Vector3.Angle(contact.normal, Vector3.up) < 45)
            {
                onGround = true;
                plyResourceMng.JumpsLeft = 1;
            }
        }
        switch (other.gameObject.tag)
        {
            case "Wall":
                ClimblingWall = true;
                break;
            case "Enemy":
                plyResourceMng.Health -= 10;
                break;
            case "Bow":
                GameObject.Destroy(other.gameObject);
                skillObtained[0] = true;
                bow.SetActive(true);
                bowMesh.SetActive(true);
                break;
            case "TheSource":
                GameObject.Destroy(other.gameObject);
                skillObtained[1] = true;
                UI.SetActive(true);
                dashesImages[0].SetActive(true);
                plyResourceMng.MaxDash = 1;
                plyResourceMng.MaxDash = 2;
                break;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        switch (collision.gameObject.tag)
        {
            case "Ground":
                onGround = false;
                break;

            case "Slope":
                onGround = false;
                slopeAngle = 0;
                break;
            case "Wall":
                ClimblingWall = false;
                break;

        }
    }

    public Vector3 LastDirection
    {
        get { return lastDirection; }
        set { lastDirection = value; }
    }

}
