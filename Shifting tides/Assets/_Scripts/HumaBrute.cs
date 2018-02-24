using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public class HumaBrute : Enemy
{
    GameObject player;

    public override void Start()
    {
        Init(5, 5, 2, 30, 30, 5, 7);
        player = GameObject.FindGameObjectWithTag("Player");
        base.Start();
    }

    public override void Attack()
    {
        Debug.Log(1);
        StartCoroutine(attackfase());
    }

    public IEnumerator attackfase()
    {
        if (distanceToPlayer < 20 && distanceToPlayer > 10)
        {
            DashSlash(player);
            agent.speed = combatSpeed;
        }

        yield return new WaitForSeconds(1f);
        ForwardSlash();

        finishedAttacking = true;
        yield break;
    }

    private void ForwardSlash()
    {
        string enemyName = Regex.Replace(gameObject.name, @"[^a-zA-Z]+", "");
        // transform.position = Vector3.MoveTowards(transform.position, transform.position + Vector3.forward * 3f , 5f);
        Vector3 fwd = gameObject.transform.TransformDirection(Vector3.forward);
        GameObject SlashZone = Instantiate(Resources.Load(enemyName + "FowardAttack") as GameObject, gameObject.transform.position + (fwd * 3f), transform.rotation);
        SlashZone.GetComponent<Rigidbody>().velocity = gameObject.GetComponent<Rigidbody>().velocity;
        Destroy(SlashZone, 0.1f);
    }

    private void DashSlash(GameObject player)
    {
        Debug.Log(1);

        string enemyName = Regex.Replace(gameObject.name, @"[^a-zA-Z]+", "");
        // transform.position = Vector3.MoveTowards(transform.position, transform.position + Vector3.forward * 3f , 5f);
        agent.speed = 50f;
        Vector3 fwd = gameObject.transform.TransformDirection(Vector3.forward);
        GameObject SlashZone = Instantiate(Resources.Load(enemyName + "FowardAttack") as GameObject, gameObject.transform.position + (fwd * 10f), transform.rotation);
        SlashZone.GetComponent<Rigidbody>().velocity = gameObject.GetComponent<Rigidbody>().velocity;

        Destroy(SlashZone, 0.1f);
    }

    public override void StartEnemyBehavior()
    {
        base.StartEnemyBehavior();

        StartCoroutine(Hunt(player));
    }
}
