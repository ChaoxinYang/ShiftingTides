using System.Collections;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class Enemy : Agent
{
    //private Vector3 homePoint;
    public Transform[] wayPoints;
    protected float attackRange, visionRange, sightRange, chaseRange, standByspeed, combatSpeed, distanceToPlayer;
    protected bool finishedAttacking;

    //add when being attacked. Jump instant to flee or hunt mode.
    public virtual void Start()
    {
        Debug.Log(0);
        MovingCor = Moving(wayPoints);
        agent = GetComponent<NavMeshAgent>();
        agent.speed = standByspeed;
        meshRenderer = this.gameObject.GetComponent<MeshRenderer>();
        originMaterial = meshRenderer.material;
        StartCoroutine(Moving(wayPoints));
        StartCoroutine(Searching());
    }

    /// <summary>
    /// Initializing the values for the enemy.
    /// </summary>
    /// <param name="MaxHealth"> Maximum health of the enemy.</param>
    /// <param name="AttackRange">Maximum attack range of the enemy.</param>
    /// <param name="VisionRange">Maximum attack range of the enemy</param>
    /// <param name="SightRange">Maximum attack range of the enemy</param>
    /// <param name="ChaseRange">Maximum attack range of the enemy</param>
    /// <param name="StandByspeed">Maximum attack range of the enemy</param>
    /// <param name="CombatSpeed">Maximum attack range of the enemy</param>
    public void Init(int MaxHealth, float AttackRange, float VisionRange, float SightRange, float ChaseRange, float StandByspeed, float CombatSpeed)
    {
        maxHealth = MaxHealth;
        currentHealth = maxHealth;
        attackRange = AttackRange;
        visionRange = VisionRange;
        sightRange = SightRange;
        chaseRange = ChaseRange;
        standByspeed = StandByspeed;
        combatSpeed = CombatSpeed;
    }

    IEnumerator Searching()
    {
        Debug.Log("Search");
        while (this.enabled == true)
        {
            yield return new WaitForSeconds(0.2f);

            if (alerted == true)
            {
                StartEnemyBehavior();
                yield break;
            }

            RaycastHit hit;
            Vector3 fwd = transform.TransformDirection(Vector3.forward);

            if (Physics.SphereCast(transform.position, visionRange, fwd, out hit, sightRange))
            {

                if (hit.collider.gameObject.tag == "Player")
                {
                    updatePlayerDistance(hit.collider.gameObject);
                    StartEnemyBehavior();
                    yield break;
                }
            }
        }
    }

    public IEnumerator Hunt(GameObject hunted)
    {
        while (distanceToPlayer < chaseRange)
        {
            updatePlayerDistance(hunted);
            Chase(hunted);
            yield return new WaitForSeconds(0.5f);

            if (distanceToPlayer < attackRange)
            {
                Attack();
                //yield return new WaitUntil(() => finishedAttacking);
                yield return new WaitForSeconds(0.5f);
                updatePlayerDistance(hunted);
                finishedAttacking = false;
            }
        }
        alerted = false;
        agent.speed = standByspeed;
        StartCoroutine(MovingCor);
        StartCoroutine(Searching());
    }

    private void updatePlayerDistance(GameObject hunted)
    {
        distanceToPlayer = Vector3.Distance(gameObject.transform.position, hunted.transform.position);
    }

    public virtual void Attack()
    {
        //string creatureName = Regex.Replace(gameObject.name, @"[^a-zA-Z]+", "");
        //Vector3 fwd = gameObject.transform.TransformDirection(Vector3.forward);
        //GameObject SlashZone = Instantiate(Resources.Load(creatureName + "AttackHitBox") as GameObject, gameObject.transform.position + (fwd * 1.5f), transform.rotation);
        //SlashZone.GetComponent<Rigidbody>().velocity = gameObject.GetComponent<Rigidbody>().velocity;
        //Destroy(SlashZone, 0.1f);
    }

    public virtual void StartEnemyBehavior()
    {
        agent.speed = combatSpeed;
    }

    public override void GotHit()
    {
        meshRenderer.material = Resources.Load("Hurt") as Material;
        this.alerted = true;
        this.currentHealth -= 1;
        if (currentHealth == 0)
        {
            Destroy(gameObject);
        }
    }
}
