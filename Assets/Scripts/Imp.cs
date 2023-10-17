using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.AI;

public class Imp : MonoBehaviour
{
    public Rigidbody rb;
    public SpriteRenderer spriteRenderer;
    public NavMeshAgent agent;

    public Transform player;

    public LayerMask whatIsGround, whatIsPlayer;

    public float health;
    public static float damage = 10;

    //Patroling
    public Vector3 walkPoint;
    bool walkPointSet;
    public float walkRange;

    //Attacking
    public float timeBetweenAttacks;
    protected bool alreadyAttacked, disableAttack = false;
    public GameObject projectile;

    //States
    public float sightRange, attackRange;
    public bool playerInSightRange, playerInAttackRange;

    protected void Awake()
    {
        //      player = GameObject.FindGameObjectWithTag("Player").transform;
        agent = GetComponent<NavMeshAgent>();
        //animator.enabled = false;
    }
    protected bool cameraIsSet = false;
    Camera mainCamera;
    protected void Update()
    {
        if (PlayerMovement.paused)
            return;

        if (!cameraIsSet)
        {
            if (GameObject.FindGameObjectWithTag("Player") == null)
                return;


            player = GameObject.FindGameObjectWithTag("Player").transform;
            mainCamera = Camera.main;
            cameraIsSet = true;
        }
        if (agent.velocity.magnitude < 0.15f && agent.enabled)
            SearchWalkPoint();

        //transform.rotation = Quaternion.Euler(new Vector3(transform.rotation.x, 0.00f, transform.rotation.y));
        gameObject.transform.LookAt(player);

        //Check for sight and attack range
        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);

        if (playerInAttackRange /* && playerInSightRange */) AttackPlayer();

        if(!agent.enabled)
            return;

        if (!playerInSightRange && !playerInAttackRange) Patroling();
        if (playerInSightRange /* && !playerInAttackRange */) ChasePlayer();
    }

    protected void Patroling()
    {
        if (!walkPointSet) SearchWalkPoint();

        if (walkPointSet)
            agent.SetDestination(walkPoint);

        Vector3 distanceToWalkPoint = transform.position - walkPoint;

        //Walkpoint reached
        if (distanceToWalkPoint.magnitude < 1f)
            walkPointSet = false;
    }
    public float randomZ;
    public float randomX;
    protected void SearchWalkPoint()
    {
        //Calculate random point in range
        randomZ = Random.Range(-walkRange, walkRange);
        randomX = Random.Range(-walkRange, walkRange);

        walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);

        if (Physics.Raycast(walkPoint, -transform.up, 2f, whatIsGround))
            walkPointSet = true;
    }

    protected void ChasePlayer()
    {
        agent.SetDestination(player.position);
    }
    protected bool targetFound = false;
    protected bool CastRay()
    { //checks to see if player is open for shot
        RaycastHit hit;
        Quaternion originalRotation = transform.rotation;
        transform.LookAt(player);
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, 64.00f))
        {
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * hit.distance, Color.red);

            targetFound = (hit.transform.gameObject.tag == "Player");
            //Debug.Log("Imp acquired target.");
        }
        transform.rotation = originalRotation;
        return targetFound;
    }
    protected virtual void AttackPlayer()
    {
        //Make sure enemy doesn't move
        //agent.SetDestination(transform.position);


        if (!alreadyAttacked && !disableAttack)
        {
            if (!CastRay()) //prevents shooting at the player through the walls and other obstacles
                return;

            //SPRITE RENDERER SET TO ATTACK SPRITE
            spriteRenderer.sprite = attackSprite;

            ///Attack code here
            GameObject Fireball = Instantiate(projectile, new Vector3(transform.position.x, 1.55f, transform.position.z - 0.3f), Quaternion.identity);
            Fireball.GetComponent<Fireball>().ownerImp = gameObject;
            //Quaternion.Euler(new Vector3(0, 0, 180)));

            //changeSprite
            ///End of attack code

            alreadyAttacked = true;
            Invoke(nameof(ResetAttack), timeBetweenAttacks); //poziva nakon timeBetweenAttacks(float) sekundi
            Invoke(nameof(ResetSprite), 0.5f);
        }
    }

    public Sprite attackSprite;
    public Sprite idleSprite;
    protected void ResetSprite()
    {
        //SET SPRITE TO IDLE SPRITE
        spriteRenderer.sprite = idleSprite;
    }

    protected void ResetAttack()
    {
        alreadyAttacked = false;
    }
    public AudioSource audioSource;
    [SerializeField]
    protected AudioClip DeathScream;
    public virtual void TakeDamage(int damage)
    {
        health -= damage;

        if (health <= 0)
        {
            audioSource.PlayOneShot(DeathScream);    //play Death sound
            disableAttack = true;
            spriteRenderer.sprite = null;

            animator.enabled = true;
            animator.Play("ImpDeath");
            
            //Invoke(nameof(DestroyEnemy), audioSource.clip.length - 0.065f);
        }
        else
            audioSource.Play();  //play Pain sound
    }
    public Animator animator;
    protected void DestroyEnemy()
    {
        gameObject.SetActive(false);
        Destroy(gameObject);
    }

    protected void FreezeConstraints(){
        rb.constraints = RigidbodyConstraints.FreezePosition | RigidbodyConstraints.FreezeRotation;
    }

    // protected void OnDrawGizmosSelected()
    // {
    //     Gizmos.color = Color.red;
    //     Gizmos.DrawWireSphere(transform.position, attackRange);
    //     Gizmos.color = Color.yellow;
    //     Gizmos.DrawWireSphere(transform.position, sightRange);
    // }

    void OnCollisionEnter2D(Collision2D collision)
    {
        SearchWalkPoint();
    }


}