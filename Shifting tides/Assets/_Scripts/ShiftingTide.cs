using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShiftingTide : MonoBehaviour {

    private int  rightBound;
    private float growSpeed ,maxSize;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        transform.localScale += Vector3.one *growSpeed * Time.deltaTime;
        if (transform.localScale.magnitude >= maxSize) {
            Destroy(gameObject);
        }
    }

    public void Init( float growSpeed, float maxSize, int changeRangeRightBound) {    
        rightBound = changeRangeRightBound;
        this.maxSize = maxSize;       
        this.growSpeed = growSpeed;
    }
    private void OnCollisionEnter(Collision collision)
    {
        switch (collision.gameObject.tag)
        {   
            case "SourcePoint":              
                Debug.Log("hit");
                collision.gameObject.GetComponent<SourcePoint>().Init(rightBound,collision.gameObject.transform.position);
                collision.gameObject.layer = 8;              
                break;
        }
    }
}
