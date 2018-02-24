using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SourcePoint : MonoBehaviour
{
    private MeshRenderer meshRenderer;
    private BasicMovement basicMovement;
    private int addJumps, addHealth, addArrow, addDash;
    public Material[] surfaceColors;
    private float immuneTime;
    public Vector3 destination;
    private Vector3 rotationValue;

    // Use this for initialization
    void Start()
    {
        rotationValue = new Vector3(Random.Range(0, 2), Random.Range(0, 2), Random.Range(0, 2));
        basicMovement = GameObject.Find("Player").GetComponent<BasicMovement>();
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

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            meshRenderer.enabled = false;
            StartCoroutine(PickedUp(collision.gameObject));
        }
    }

    public void Init(int rightBound, Vector3 destination, int leftBound = 0)
    {
        //this.layerIndex = layerIndex;
        if (rightBound > surfaceColors.Length)
        {

            rightBound = surfaceColors.Length;
        }
        int colorIndex = Random.Range(leftBound, rightBound);
        this.destination = destination;
        // Debug.Log(destination);

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
                addJumps = 2;
                break;
            case 4:
                addHealth -= 5;
                break;
        }
        addJumps += 1;
    }

    private IEnumerator PickedUp(GameObject player)
    {
        Time.timeScale = 0.7F;
        basicMovement.Arrows += addArrow;
        basicMovement.JumpsLeft += addJumps;
        basicMovement.Health += addHealth;
        basicMovement.Dashes += addDash;
        player.GetComponent<Rigidbody>().useGravity = false;
        yield return new WaitForSecondsRealtime(1f);
        Time.timeScale = 1F;
        player.GetComponent<Rigidbody>().useGravity = true;
        Destroy(gameObject);
    }
}
