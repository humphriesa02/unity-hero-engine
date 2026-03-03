using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{    
    [Header("Combat")]
    [SerializeField] float attackCD = 3f;
    [SerializeField] float attackRange = 1f;
    [SerializeField] float aggroRange = 4f;
    
    GameObject player;
    NavMeshAgent agent;
    Animator anim;
    float timePassed;
    float newDestinationCD = 0.5f;

    void Start()
    {
        player = GameObject.FindWithTag("Player");
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();    
    }

    void Update()
    {
        anim.SetFloat("speed", agent.velocity.magnitude / agent.speed);

        if (timePassed >= attackCD)
        {
            if (Vector3.Distance(player.transform.position, transform.position) <= attackRange)
            {
                anim.SetTrigger("attack");
                timePassed = 0;
            }
        }
        timePassed+=Time.deltaTime;

        if (newDestinationCD <= 0 && Vector3.Distance(player.transform.position, transform.position) <= aggroRange)
        {
            newDestinationCD = 0.5f;
            agent.SetDestination(player.transform.position);
            transform.LookAt(player.transform);
        }
        newDestinationCD -= Time.deltaTime;
    }

    public void StartDealDamage()
    {
        GetComponentInChildren<Hurtbox>().StartDealDamage();
    }

    public void EndDealDamage()
    {
        GetComponentInChildren<Hurtbox>().EndDealDamage();
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, aggroRange);
    }
}
