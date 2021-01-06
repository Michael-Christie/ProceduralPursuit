using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Monster : MonoBehaviour
{
    bool dead = false;
    NavMeshAgent agent;
    MonsterSpawner MS;
    Animator anim;
    Transform player;
    float minDistToPlayer = 25f;
    float minHitDist = 5f;
    bool isAttacking = false;
    const float resetTime = 5f;
    float timeTillReset = 0f;

    public bool IsDead()
    {
        return dead;
    }

    public void Initialize(MonsterSpawner m)
    {
        MS = m;
        dead = false;
        gameObject.layer = LayerMask.NameToLayer("Monsters");
        anim = GetComponent<Animator>();
        agent = gameObject.AddComponent<NavMeshAgent>();
        agent.speed = 2.5f;
        
    }

    public void TargetPlayer()
    {
        agent.destination = player.position;
        anim.SetBool("Walking", true);
    }

    private void Update()
    {
        if (!dead)
        {

            player = GameObject.Find("Player").transform;

            float distToPlayer = Mathf.Abs((player.position - transform.position).magnitude);

            if (distToPlayer < minDistToPlayer)
            {
                TargetPlayer();
            }

            if (agent.remainingDistance < .5f)
                anim.SetBool("Walking", false);

            if (distToPlayer < minHitDist && !isAttacking)
            {
                isAttacking = true;
                anim.SetTrigger("IsAttacking");
                StartCoroutine(IsStillAttacking());
                timeTillReset = resetTime;
            }

            if (timeTillReset > 0f)
                timeTillReset -= Time.deltaTime;
            if (timeTillReset <= 0 && isAttacking)
                isAttacking = false;

        }
    }

    IEnumerator IsStillAttacking()
    {
        yield return new WaitForSeconds(1f);

        float distToPlayer = Mathf.Abs((player.position - transform.position).magnitude);

        Debug.Log(distToPlayer < minHitDist);

        if (distToPlayer < minHitDist)
            player.GetComponent<PlayerInteraction>().TakeDamage(10f);
    }

    public void Hit()
    {
        if (!dead)
        {
            MS.MonsterKilled();
            dead = true;
            anim.SetBool("IsDead", true);
            GetComponent<CapsuleCollider>().enabled = false;
            agent.isStopped = true;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        //if its a weapon && dead is = false
        if (collision.transform.tag == "Weapon" && !dead)
        {
            //remove it from the monsterSpawner;
            MS.MonsterKilled();
            dead = true;
            anim.SetBool("IsDead", true);
            agent.isStopped = true;
            //play dead animation;
        }
    }
}
