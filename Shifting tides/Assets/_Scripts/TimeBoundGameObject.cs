using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TimeBoundGameObject : MonoBehaviour
{
    protected GameManager gameMng;
    protected Rigidbody rbObject;
    private Vector3 startVelocity;
    private bool affectedByGravity;
    private bool isRunningPhsicsUpdate;
    private float gravity;


    public virtual void Start()
    {
        isRunningPhsicsUpdate = false;
        gameMng = GameObject.Find("GameManager").GetComponent<GameManager>();
        rbObject = this.gameObject.GetComponent<Rigidbody>();

    }
    public virtual void Initialize(bool isFloating = false)
    {
        isFloating = affectedByGravity;
        gravity = isFloating ? 0 : -10.81f;
    }

    private void FixedUpdate()
    {
        localPhysicsUpdate();
    }

    public virtual void localPhysicsUpdate()
    {
        if (gameMng.isTimeStoped && startVelocity == Vector3.zero)
        {
            restrictVelocity();
        }
        else if (!gameMng.isTimeStoped) {

            if (startVelocity != Vector3.zero)
            {
                resumeVelocity();
            }               
                rbObject.AddForce(new Vector3(0, gravity, 0));                  
        }                   
    }

    private void restrictVelocity() {      
        startVelocity = rbObject.velocity;   
        rbObject.drag = 15f;
    }

    private void resumeVelocity() {
        rbObject.velocity = startVelocity;
        startVelocity = Vector3.zero;
        rbObject.drag = 0.1f;
    }

}

