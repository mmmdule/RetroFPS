using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fireball : MonoBehaviour
{
    Rigidbody rb;
    GameObject player;
    Transform target;
    public float Speed;
    public GameObject SpriteChild;
    public GameObject ownerImp;
    public AudioSource audioSource;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        player = GameObject.FindGameObjectWithTag("Player");
        target = player.transform;
        healthManager = player.GetComponent<HealthManager>();
        // rb.AddForce(target.position * (Speed), ForceMode.Impulse);
        transform.LookAt(target);
        rb.AddRelativeForce(Vector3.forward * Speed, ForceMode.Impulse);
        audioSource.Play();
    }


    // Update is called once per frame
    void Update()
    {
        SpriteChild.transform.LookAt(player.transform);
        rb.AddRelativeForce(Vector3.forward * Speed, ForceMode.Impulse);
        
        //transform.Translate(Vector3.forward * Time.deltaTime * Speed);
        
        //rb.AddForce(target.position * Speed, ForceMode.Impulse);
        
        
        //transform.position = Vector3.MoveTowards(transform.position, target.position, Speed * Time.deltaTime);
        //rb.MovePosition(player.transform.position * Time.deltaTime);
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
                Debug.LogWarning("Fireball Hit Player");
                healthManager.TakeDamage(Damage);
                gameObject.SetActive(false);
                break;
            /*
            case "Imp":
                if(!GameObject.ReferenceEquals(collided.gameObject, ownerImp)) //if Imp not the one who launched it
                    collided.gameObject.GetComponent<Imp>().TakeDamage((int)Imp.damage);
                gameObject.SetActive(false);
                break;
            */
        }
    }
}
