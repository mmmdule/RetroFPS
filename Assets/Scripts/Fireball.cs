using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fireball : MonoBehaviour
{
    Rigidbody rb;
    GameObject player;
    Transform target;
    public bool isPlayerProjectile = false;
    public float Speed;
    public GameObject SpriteChild;
    public GameObject ownerImp;
    public AudioSource audioSource;
    // Start is called before the first frame update


    [SerializeField]
    HitMarker hitMarker;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        player = GameObject.FindGameObjectWithTag("Player");
        hitMarker = isPlayerProjectile ? player.GetComponentInChildren<HitMarker>() : null;
        target = player.transform;
        healthManager = player.GetComponent<HealthManager>();
        // rb.AddForce(target.position * (Speed), ForceMode.Impulse);
        transform.LookAt(target);
        rb.AddRelativeForce(Vector3.forward * Speed, ForceMode.Impulse);
        audioSource.Play();
    }
    public bool notStatic = true;

    // Update is called once per frame
    void Update()
    {
        if(PlayerMovement.paused)
            return;
        
        SpriteChild.transform.LookAt(player.transform);

        if(isPlayerProjectile){
            //Speed = 2.55f;
            audioSource = null;
            rb.AddRelativeForce(Vector3.back * Speed, ForceMode.Impulse);
            return;
        }
            
        if(notStatic && !isPlayerProjectile){
            rb.AddRelativeForce(Vector3.forward * Speed, ForceMode.Impulse);
        }
    }
    public HealthManager healthManager;
    public int Damage = 10;
    void OnTriggerEnter(Collider collided){
        switch(collided.gameObject.tag){
            case "Wall":
            case "Floor":
                gameObject.SetActive(false);
                break;
            case "Player":
                if(isPlayerProjectile)
                    break;
                Debug.LogWarning("Fireball Hit Player");
                healthManager.TakeDamage(Damage);
                gameObject.SetActive(false);
                break;
            case "Imp":
                if(isPlayerProjectile) {//if Imp not the one who launched it
                    hitMarker.Show();
                    collided.gameObject.GetComponent<Imp>().TakeDamage(60);
                    gameObject.SetActive(false);
                }
                break;
            case "Tri-Imp":
                if(isPlayerProjectile) { //if Imp not the one who launched it
                    hitMarker.Show();
                    collided.gameObject.GetComponent<TriImp>().TakeDamage(60);
                    gameObject.SetActive(false);
                }
                break;
        }
    }
}
