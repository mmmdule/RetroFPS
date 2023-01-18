using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.AI;

public class TriImp : MonoBehaviour
{
    public NavMeshAgent agent;

    public Transform player;

    public LayerMask whatIsGround, whatIsPlayer;

    public float health;
    public static float damage = 10;

    //Patroling
    public Vector3 walkPoint;
    bool walkPointSet;
    public float walkPointRange;

    //Attacking
    public float timeBetweenAttacks;
    bool alreadyAttacked, disableAttack = false;
    public GameObject projectile;

    //States
    public float sightRange, attackRange;
    public bool playerInSightRange, playerInAttackRange;

    private void Awake()
    {
//      player = GameObject.FindGameObjectWithTag("Player").transform;
        agent = GetComponent<NavMeshAgent>();
    }
    private bool cameraIsSet = false;
    Camera mainCamera;
    private void Update()
    {
        if(PlayerMovement.paused)
            return;
            
        if(!cameraIsSet){
            if(GameObject.FindGameObjectWithTag("Player") == null)
                return;
            
            
            player = GameObject.FindGameObjectWithTag("Player").transform;
            mainCamera = Camera.main;
            cameraIsSet = true;
        }

        //transform.rotation = Quaternion.Euler(new Vector3(transform.rotation.x, 0.00f, transform.rotation.y));
        thisSprite.gameObject.transform.LookAt(player);
        attackSprite.gameObject.transform.LookAt(player);

        //Check for sight and attack range
        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);
        
        if (!playerInSightRange && !playerInAttackRange) Patroling();
        if (playerInSightRange && !playerInAttackRange) ChasePlayer();
        if (playerInAttackRange && playerInSightRange) AttackPlayer();
    }

    private void Patroling()
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
    private void SearchWalkPoint()
    {
        //Calculate random point in range
        randomZ = Random.Range(-walkPointRange, walkPointRange);
        randomX = Random.Range(-walkPointRange, walkPointRange);

        walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);

        if (Physics.Raycast(walkPoint, -transform.up, 2f, whatIsGround))
            walkPointSet = true;
    }

    private void ChasePlayer()
    {
        agent.SetDestination(player.position);
    }
    private bool targetFound = false;
    private bool CastRay(){ //checks to see if player is open for shot
        RaycastHit hit;
        Quaternion originalRotation = transform.rotation;
        transform.LookAt(player);
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, 64.00f)) {
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * hit.distance, Color.red);
            
            targetFound = (hit.transform.gameObject.tag == "Player");
            //Debug.Log("Imp acquired target.");
        }
        transform.rotation = originalRotation;
        return targetFound;
    }

    public SpriteRenderer thisSprite;
    private void AttackPlayer()
    {
        //Make sure enemy doesn't move
        //agent.SetDestination(transform.position);


        if (!alreadyAttacked && !disableAttack)
        {
            if(!CastRay()) //prevents shooting at the player through the walls and other obstacles
                return;

            attackSprite.enabled = true;
            thisSprite.enabled = false;
            ///Attack code here
            GameObject Fireball = Instantiate(projectile, new Vector3(transform.position.x, 1.55f, transform.position.z - 0.3f),  Quaternion.identity);
            
            // Fireball[] array = Fireball.GetComponentsInChildren<Fireball>();
            // for(int i = 0; i < array.Length; i++)
            //     array[i].ownerImp = gameObject;
            
            //Quaternion.Euler(new Vector3(0, 0, 180)));

            //changeSprite
            ///End of attack code

            alreadyAttacked = true;
            Invoke(nameof(ResetAttack), timeBetweenAttacks); //poziva nakon timeBetweenAttacks(float) sekundi
            Invoke(nameof(ResetSprite), 0.5f);
        }
    }

    public SpriteRenderer attackSprite;
    private void ResetSprite(){
        attackSprite.enabled = false;
        thisSprite.enabled = true;
    }

    private void ResetAttack()
    {
        alreadyAttacked = false;
    }
    public AudioSource audioSource;
    public void TakeDamage(int damage)
    {
        audioSource.Play(); //play Pain/Death sound
        health -= damage;

        if (health <= 0) {
            disableAttack = true;
            attackSprite.sprite = null;
            thisSprite.sprite = null;
            attackSprite.enabled = false;
            thisSprite.enabled = false;
            animator.Play("ImpDeath");
            Invoke(nameof(DestroyEnemy), audioSource.clip.length - 0.065f);
        }
    }
    public Animator animator;
    private void DestroyEnemy()
    {
        gameObject.SetActive(false);
        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sightRange);
    }

    void OnCollisionEnter2D(Collision2D collision){
        if(collision.gameObject.tag == "Wall")
            SearchWalkPoint();
    }


}


//my code (CTRL K + U to uncomment)
// {
//     Camera mainCamera;
//     public GameObject ImpSprite;
//     bool cameraIsSet = false;
//     public Animator animator;

    
//     public NavMeshAgent agent;

//     public Transform player;

//     public LayerMask whatIsGround, whatIsPlayer;

//     public float health;

//     //Patroling
//     public Vector3 walkPoint;
//     bool walkPointSet;
//     public float walkPointRange;

//     //Attacking
//     public float timeBetweenAttacks;
//     bool alreadyAttacked;
//     public GameObject projectile;

//     //States
//     public float sightRange, attackRange;
//     public bool playerInSightRange, playerInAttackRange;

//     private void Awake()
//     {
//         //player = GameObject.Find("Player").transform;
//         agent = GetComponent<NavMeshAgent>();
//     }



//     // Start is called before the first frame update
//     void Start()
//     {
//         animator.SetFloat("OldX", transform.position.x);
//         animator.SetFloat("OldZ", transform.position.z);
//     }

//     void LateUpdate()
//     {
//         //Billboarding
//         if(!cameraIsSet){
//             if(GameObject.FindGameObjectWithTag("Player") == null)
//                 return;
            
            
//             player = GameObject.FindGameObjectWithTag("Player").transform;
//             mainCamera = Camera.main;
//             cameraIsSet = true;
//         }

//         //ImpSprite.transform.LookAt(mainCamera.transform);
//         //ImpSprite.transform.Rotate(0, 180, 0);
//         transform.LookAt(player);

//         //Billboarding
        

//         //Check for sight and attack range
//         playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);
//         playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);

//         if (!playerInSightRange && !playerInAttackRange) Patroling();
//         if (playerInSightRange && !playerInAttackRange) ChasePlayer();
//         if (playerInAttackRange && playerInSightRange) AttackPlayer();
//     }

//     public void Patroling()
//     {
//         animator.SetBool("IsAlert", false);
//         if (!walkPointSet) SearchWalkPoint();

//         if (walkPointSet)
//             agent.SetDestination(walkPoint);

//         Vector3 distanceToWalkPoint = transform.position - walkPoint;

//         OnAnimatorMove ();
//         animator.SetBool("Walking", true);
//         //Walkpoint reached
//         if (distanceToWalkPoint.magnitude < 1f)
//             walkPointSet = false;
        
//     }
//     [ExecuteInEditMode]
//     void OnAnimatorMove ()
//     {
//         // Update position to agent position
//         //transform.position = agent.nextPosition;
//         if (walkPointSet)
//             agent.SetDestination(walkPoint);
//     }

//     public float randomZ;
//     public float randomX;
//     public void SearchWalkPoint()
//     {
//         //Calculate random point in range
//         randomZ = Random.Range(-walkPointRange, walkPointRange);
//         randomX = Random.Range(-walkPointRange, walkPointRange);

//         walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);
//         animator.SetFloat("NewX", randomX);
//         animator.SetFloat("NewZ", randomZ);

//         if (Physics.Raycast(walkPoint, -transform.up, 2f, whatIsGround))
//             walkPointSet = true;
//     }

//     private void ChasePlayer()
//     {
//         agent.SetDestination(player.position);
//         animator.SetBool("IsAlert", true);
//     }

//     private void AttackPlayer()
//     {
//         animator.SetFloat("OldX", transform.position.x);
//         animator.SetFloat("OldZ", transform.position.z);

//         animator.SetBool("Walking", false);
//         animator.SetBool("IsAlert", false);
//         animator.SetBool("TimeToShoot", true);
//         //Make sure enemy doesn't move
//         agent.SetDestination(transform.position);

//         //transform.LookAt(player);

//         if (!alreadyAttacked)
//         {
//             ///Attack code here
//             Rigidbody rb = Instantiate(projectile, transform.position, Quaternion.identity).GetComponent<Rigidbody>();
//             //rb.AddForce(player.transform.position * 5f, ForceMode.Impulse);
            
//             //rb.AddForce(transform.up * 8f, ForceMode.Impulse);
            
//             ///End of attack code

//             alreadyAttacked = true;
//             Invoke(nameof(ResetAttack), timeBetweenAttacks);
//         }
//     }
//     private void ResetAttack()
//     {
//         alreadyAttacked = false;
//         animator.SetBool("TimeToShoot", false);
//     }

//     public void TakeDamage(int damage)
//     {
//         health -= damage;

//         if (health <= 0) Invoke(nameof(DestroyEnemy), 0.5f);
//     }
//     private void DestroyEnemy()
//     {
//         Destroy(gameObject);
//     }

//     private void OnDrawGizmosSelected()
//     {
//         Gizmos.color = Color.red;
//         Gizmos.DrawWireSphere(transform.position, attackRange);
//         Gizmos.color = Color.yellow;
//         Gizmos.DrawWireSphere(transform.position, sightRange);
//     }
// }
