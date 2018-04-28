using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SourcePoint : MonoBehaviour
{
    private MeshRenderer meshRenderer;
    private PlayerResourcesManager plyResourcesMng;
    private GameManager gameMng;
    private int addJumps, addHealth, addArrow, addDash,addSource;
    public Material[] surfaceColors;
    private float immuneTime;
    public Vector3 destination;
    private Vector3 rotationValue;

    // Use this for initialization
    void Start()
    {
        rotationValue = new Vector3(Random.Range(-1, 2), Random.Range(-1, 2), Random.Range(-1, 2));
        transform.localScale = new Vector3(Random.Range(0.1f, 0.7f), Random.Range(0.1f, 0.7f), Random.Range(0.1f, 0.7f));
        plyResourcesMng = GameObject.Find("GameManager").GetComponent<PlayerResourcesManager>();
        gameMng = GameObject.Find("GameManager").GetComponent<GameManager>();
        meshRenderer = GetComponent<MeshRenderer>();
    }

    private void Update()
    {
        transform.Rotate(rotationValue * Random.Range(1.1f, 3));
        if (destination != Vector3.zero)
        {
            transform.position = Vector3.MoveTowards(transform.position, destination, Time.deltaTime);
        }

        if (gameObject.layer == 8)
        {

            wearOffImmunity();
        }
    }

    private void wearOffImmunity()
    {
        immuneTime += Time.deltaTime * 20f;
        if (immuneTime >= 200f)
        {

            gameObject.layer = 10;
        }
    }

    private void OnTriggerEnter(Collider other)
    {   
        if (other.gameObject.CompareTag("Player"))
        {
           
            meshRenderer.enabled = false;
            PickedUp();
        }
    }

    //private void OnCollisionEnter(Collision collision)
    //{
    //    if (collision.gameObject.CompareTag("Player"))
    //    {
    //        meshRenderer.enabled = false;
    //        StartCoroutine(PickedUp(collision.gameObject));
    //    }
    //}

    public void Init(int rightBound, Vector3 destination, int leftBound = 0)
    {
        //this.layerIndex = layerIndex;
        if (rightBound > surfaceColors.Length)
        {
            rightBound = surfaceColors.Length;
        }
        int colorIndex = Random.Range(leftBound, rightBound);
        this.destination = destination;
        meshRenderer = GetComponent<MeshRenderer>();
        meshRenderer.GetComponent<MeshRenderer>().material = surfaceColors[colorIndex];
        switch (colorIndex)
        {
            case 0:
                addArrow = 5;
                break;
            case 1:
                addHealth = 5;
                break;
            case 2:
                addDash = 1;
                break;
            case 3:
                addJumps = 1;
                break;
            case 5:
                addHealth -= 5;
                break;
            case 4:
                addSource = 5;
                break;
        }
    }

    private void PickedUp()
    {
        plyResourcesMng.Arrows += addArrow;
        plyResourcesMng.JumpsLeft += addJumps;
        plyResourcesMng.Health += addHealth;
        plyResourcesMng.Dashes += addDash;
        plyResourcesMng.SourceReserve += addSource;
        Destroy(gameObject);

    }
}
