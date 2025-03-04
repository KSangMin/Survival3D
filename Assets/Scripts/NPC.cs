using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public enum AIState
{
    Idle,
    Wandering,
    Attacking
}

public class NPC : MonoBehaviour, IDamageable
{
    [Header("Stats")]
    public int health;
    public float walkSpeed;
    public float runSpeed;
    public ItemData[] dropOnDeathPrefab;

    [Header("AI")]
    private NavMeshAgent agent;
    public float detectDistance;
    private AIState aiState;

    [Header("Wandering")]
    public float minWanderDistance;
    public float maxWanderDistance;
    public float minWanderWaitTime;
    public float maxWanderWaitTime;

    [Header("Combat")]
    public int damage;
    public float attackRate;
    private float lastAttackTime;
    public float attackDistance;

    private float playerDistance;

    public float fov = 120f;

    Animator animator;
    SkinnedMeshRenderer[] meshRenderers;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        meshRenderers = GetComponentsInChildren<SkinnedMeshRenderer>();
    }

    private void Start()
    {
        SetState(AIState.Wandering);
    }

    private void Update()
    {
        playerDistance = Vector3.Distance(transform.position, CharacterManager.Instance.Player.transform.position);

        animator.SetBool("isMoving", aiState != AIState.Idle);

        switch (aiState)
        {
            case AIState.Idle:
            case AIState.Wandering:
                PassiveUpdate();
                break;
            case AIState.Attacking:
                AttackingUpdate();
                break;
        }
    }

    void SetState(AIState state)
    {
        aiState = state;

        switch (aiState)
        {
            case AIState.Idle:
                agent.speed = walkSpeed;
                agent.isStopped = true;
                break;
            case AIState.Wandering:
                agent.speed = walkSpeed;
                agent.isStopped = false;
                break;
            case AIState.Attacking:
                agent.speed = runSpeed;
                agent.isStopped = false;
                break;
        }

        animator.speed = agent.speed / walkSpeed;
    }

    void PassiveUpdate()
    {
        if (aiState == AIState.Wandering && agent.remainingDistance <= 0.1f)
        {
            SetState(AIState.Idle);
            Invoke("WanderToNewLocation", Random.Range(minWanderWaitTime, maxWanderWaitTime));
        }

        if(playerDistance < detectDistance)
        {
            SetState(AIState.Attacking);
        }
    }

    void WanderToNewLocation()
    {
        if (aiState != AIState.Idle) return;

        SetState(AIState.Wandering);
        agent.SetDestination(GetWanderLocation());
    }

    Vector3 GetWanderLocation()
    {
        NavMesh.SamplePosition(transform.position + Random.onUnitSphere * Random.Range(minWanderDistance, maxWanderDistance), out NavMeshHit hit, maxWanderDistance, NavMesh.AllAreas);

        int i = 0;
        while(Vector3.Distance(transform.position, hit.position) < detectDistance)
        {
            NavMesh.SamplePosition(transform.position + Random.onUnitSphere * Random.Range(minWanderDistance, maxWanderDistance), out hit, maxWanderDistance, NavMesh.AllAreas);
            i++;
            if (i >= 30) break;
        }

        return hit.position;
    }

    void AttackingUpdate()
    {
        if (playerDistance < attackDistance && isPlayerInFov())
        {
            agent.isStopped = true;
            if (Time.time - lastAttackTime > attackRate)
            {
                lastAttackTime = Time.time;
                CharacterManager.Instance.Player.controller.GetComponent<IDamageable>().TakePhysicalDamage(damage);
                animator.speed = 1;
                animator.SetTrigger("Attack");
            }
        }
        else
        {
            if (playerDistance < detectDistance)
            {
                agent.isStopped = false;
                NavMeshPath path = new NavMeshPath();
                if (agent.CalculatePath(CharacterManager.Instance.Player.transform.position, path))
                {
                    agent.SetDestination(CharacterManager.Instance.Player.transform.position);
                }
                else
                {
                    agent.SetDestination(transform.position);
                    agent.isStopped = true;
                    SetState(AIState.Wandering);
                }
            }
            else
            {
                agent.SetDestination(transform.position);
                agent.isStopped = true;
                SetState(AIState.Wandering);
            }
        }
    }

    bool isPlayerInFov()
    {
        Vector3 dirToPlayer = CharacterManager.Instance.Player.transform.position - transform.position;
        float angle = Vector3.Angle(transform.forward, dirToPlayer);
        return angle <= fov * 0.5f;
    }

    public void TakePhysicalDamage(int damage)
    {
        health -= damage;
        StartCoroutine(DamageFlash());

        if (health < 0) Dead();
    }

    void Dead()
    {
        for(int i =  0; i < dropOnDeathPrefab.Length; i++)
        {
            Instantiate(dropOnDeathPrefab[i].prefab, transform.position + Vector3.up * 2, Quaternion.identity);
        }

        Destroy(gameObject);
    }

    IEnumerator DamageFlash()
    {
        for(int i = 0; i < meshRenderers.Length; i++)
        {
            meshRenderers[i].material.color = new Color(1f, 0.6f, 0.6f);
        }

        yield return new WaitForSeconds(0.1f);

        for (int i = 0; i < meshRenderers.Length; i++)
        {
            meshRenderers[i].material.color = Color.white;
        }
    }
}
