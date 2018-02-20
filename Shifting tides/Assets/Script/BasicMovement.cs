
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BasicMovement : MonoBehaviour
{
  
    private Rigidbody rbPlayer;
    private Vector3 moveHorizontal, moveVertical, moveUpWard;
    private Camera cameraMain;
    private bool onGround, ClimblingWall;
    public GameObject bow, UI;
    private GameObject arrow;
    public GameObject[] dashesImages;
    //0: bow , 1: The Source
    private bool[] skillObtained = new bool[10];
    public Ui ui;
    public float speed, speedLimit, maxSpeed, moveSpeed, DashSpeed, jumpVel,gravity;
    private float arrowSpeed, cameraView;
    public float ArrowSpeed { get { return arrowSpeed; } set { arrowSpeed = value; if (arrowSpeed > 70) { arrowSpeed = 70; } } }  
    public float CameraView { get { return cameraView; } set { cameraView = value; if (cameraView <= 40) { cameraView = 40; } if (cameraView > 60) { cameraView = 60; } } }

    #region Player Resources
    private int jumpsLeft, health, arrows, dashes;
    private int maxDash = 0, maxJump = 1, maxHealth = 100;
    private float dashCharge;
    public int JumpsLeft { get { return jumpsLeft; } set { jumpsLeft = value; ui.sliders[0].value = JumpsLeft; if (jumpsLeft < 0) { jumpsLeft = 0; } if (jumpsLeft > maxJump) { jumpsLeft = maxJump; } } }
    public int Health { get { return health; } set { health = value; ui.sliders[1].value = Health; if (health < 0) { health = 0; } if (health > 100) { health = 100; } } }
    private float stamina;
    public float Stamina { get { return stamina; } set { stamina = value; if (stamina < 0) { stamina = 0; } if (stamina > 100) { stamina = 100; } } }
    public int Arrows { get { return arrows; } set { arrows = value; ui.texts[0].text = Arrows.ToString(); if (arrows < 0) { arrows = 0; } if (arrows > 20) { arrows = 20; } } }
    public int Dashes { get { return dashes; } set { dashes = value; if (dashes < 0) { dashes = 0; } if (dashes > maxDash) { dashes = maxDash; } } }
    #endregion

    void Start()
    {   
        arrowSpeed = 30;
        JumpsLeft = 1;
        Arrows = 10;
        cameraView = 60;  
        dashes = 1  ;
        Health = 20;
        rbPlayer = gameObject.GetComponent<Rigidbody>();
        cameraMain = Camera.main;       
        arrow = Resources.Load("Prefabs/Arrow") as GameObject;
    
    }

    void Update()
    {  
        
       // Debug.Log(Input.inputString);
        //Debug.Log(Input.GetAxis("Horizontal"));
        moveHorizontal = Input.GetAxisRaw("Horizontal") * speed * transform.right;
        moveVertical = Input.GetAxisRaw("Vertical") * speed * transform.forward;
        if (Input.GetKeyDown(KeyCode.F)&& Dashes > 0 && skillObtained[1])
        {
            StartCoroutine(Dash());
        }
        if (Dashes < maxDash)
        {
            chargeUpDash();
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Jumping();
        }
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            speed = maxSpeed;
        }
        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            speed= moveSpeed;
        }
        if (Input.GetMouseButton(1) && Arrows > 0 && skillObtained[0])
        {
            ChargeUpArrow();
        }
        if (!Input.GetMouseButton(1))
        {
            gainBackVision();
        }
        if (Input.GetMouseButtonUp(1) && Arrows > 0 && skillObtained[0])
        {
            ShotArrow();        
        }
        
    }
    private void Jumping()
    {
      
        if (JumpsLeft > 0)
        {
            JumpsLeft -= 1;
             if (onGround)
            {
                 rbPlayer.AddForce((Vector3.up * jumpVel), ForceMode.Impulse);
               
            }
            if (onGround == false )
            {
                  rbPlayer.velocity = moveHorizontal + moveVertical + new Vector3(0f, 1f, 0f);
                rbPlayer.AddForce(new Vector3(0f, jumpVel / 3, 0f), ForceMode.Impulse);
            }
        }
    }

    private IEnumerator Dash()
    {
        ui.dashCharges[Dashes-1].enabled = false;
        Dashes -= 1;
        speed= DashSpeed;
        yield return new WaitForSeconds(0.5f);
        speed= moveSpeed;
        yield break;
    }

    private void chargeUpDash()
    {

        dashCharge += Time.deltaTime*20f;
        if (dashCharge >= 60f) {

           ui.dashCharges[Dashes].enabled = true;
           Dashes += 1;
           dashCharge = 0;
        }
       
    }

    private void ShotArrow()
    {
        Arrows -= 1;

        GameObject Arrow = Instantiate(arrow, bow.transform.position, bow.transform.rotation);
        Arrow.GetComponent<Rigidbody>().velocity = Arrow.transform.forward * ArrowSpeed;
        Debug.Log(Arrow.transform.position);
        ArrowSpeed = 30;
   
    }

    private void gainBackVision()
    {
        CameraView += Time.deltaTime * 50f; 
    }

    private void ChargeUpArrow()    
    {       
        ArrowSpeed += Time.deltaTime * 30f;
        CameraView -= Time.deltaTime * 30f;
   }

    void FixedUpdate()
    {
       
         gravity = rbPlayer.velocity.y >= 0 ? 0 : -8f;
         rbPlayer.AddForce(moveHorizontal + moveVertical + Vector3.up*gravity, ForceMode.Force);
        Vector2 horizontalVelocity = new Vector2(rbPlayer.velocity.x,rbPlayer.velocity.z);
        if ( horizontalVelocity.magnitude > speedLimit)
        {
            horizontalVelocity = horizontalVelocity.normalized * speedLimit;
        }
        rbPlayer.velocity = new Vector3(horizontalVelocity.x, rbPlayer.velocity.y, horizontalVelocity.y);
    }

    
   


void LateUpdate()
    {
        transform.localRotation = Quaternion.Euler(transform.localEulerAngles.x, cameraMain.transform.localEulerAngles.y, transform.localEulerAngles.z);
    }

    void OnCollisionEnter(Collision other)
    {
        switch (other.gameObject.tag)
        {
            case "Ground":
                onGround = true;
                JumpsLeft = 2;
               // rbPlayer.drag = 1;
                break;
            case "Wall":
                ClimblingWall = true;
                break;
            case "Enemy":
                Health -= 10;
                break;
        }    

    }

    private void OnCollisionExit(Collision collision)
    {
        switch (collision.gameObject.tag)
        {
            case "Ground":
                onGround = false;
               // rbPlayer.drag = 0.8f;
                break;
            case "Wall":
                ClimblingWall = false;
                break;
            case "Bow":
                GameObject.Destroy(collision.gameObject);
                skillObtained[0] = true;
                bow.SetActive(true);
                UI.SetActive(true);
                break;
            case "TheSource":
                GameObject.Destroy(collision.gameObject);
                skillObtained[1] = true;
                dashesImages[0].SetActive(true);
                maxDash = 1;
                maxJump = 2;
                break;
            
        }
    }
  
}
