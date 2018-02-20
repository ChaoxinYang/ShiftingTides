using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Ui : MonoBehaviour {

    private BasicMovement basicMovement;
    public Enemy currentTarget;
    public Slider[] sliders;
    public Text[] texts;
    public Image[] dashCharges;
   
	// Use this for initialization
	void Start () {
        basicMovement = GameObject.Find("Player").GetComponent<BasicMovement>();
      	}

}
