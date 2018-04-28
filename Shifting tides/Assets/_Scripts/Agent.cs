using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public abstract class Agent : MonoBehaviour
{
    protected NavMeshAgent agent;
    protected Vector3 currentTarget, destination;
    protected float stamina;
    public float currentHealth, maxHealth;
    protected IEnumerator MovingCor;
    protected bool alerted = false;
    protected MeshRenderer meshRenderer;
    protected Material originMaterial;

    protected IEnumerator Moving(Transform[] wayPoints)
    {
        Debug.Log(gameObject.name + "Started Moving");

        DetermineTargetPosition(wayPoints);
        MoveTowardsTarget(agent, currentTarget);

        yield return new WaitUntil(() => Arrived(transform.position, destination, 5));

        yield return new WaitForSeconds(5f);
        StartCoroutine(Moving(wayPoints));
    }

    protected void MoveTowardsTarget(NavMeshAgent agent, Vector3 currentTarget)
    {
        agent.SetDestination(currentTarget);
    }

    void DetermineTargetPosition(Transform[] wayPoints)
    {
        currentTarget = wayPoints[Random.Range(0, wayPoints.Length)].position;
        destination = currentTarget;
    }

    void RethinkingDestination(Transform[] wayPoints)
    {

        int random = Random.Range(0, 10);
        if (random == 0)
        {
            destination = wayPoints[Random.Range(0, wayPoints.Length)].position;
            MoveTowardsTarget(agent, destination);
        }
        else
        {
            MoveTowardsTarget(agent, destination);
        }
    }

    protected bool Arrived(Vector3 gameObject, Vector3 target, float range)
    {
        bool Arrived = false;
        float distance = Vector3.Distance(gameObject, target);
        if (distance < range)
        {
            Arrived = true;
        }
        return Arrived;
    }

    protected void Chase(GameObject hunted)
    {
        //Debug.Log("Chase");
        currentTarget = hunted.transform.position;
        MoveTowardsTarget(agent, currentTarget);
    }

    protected void Flee(GameObject hunter)
    {
        //Debug.Log("Flee");
        currentTarget = hunter.transform.position + new Vector3(Random.Range(-50, 50), 0, Random.Range(-50, 50));
        MoveTowardsTarget(agent, currentTarget);
    }

    public virtual void GotHit()
    {
    }

    void ChangeToOriginMaterial()
    {

        meshRenderer.material = originMaterial;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Arrow"))
        {
            Destroy(collision.gameObject);
            GotHit();
            Invoke("ChangeToOriginMaterial", 0.5f);
        }
    }
}
