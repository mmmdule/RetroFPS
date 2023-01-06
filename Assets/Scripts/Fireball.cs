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
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        player = GameObject.FindGameObjectWithTag("Player");
        target = player.transform;

        // rb.AddForce(target.position * (Speed), ForceMode.Impulse);
        transform.LookAt(target);
        rb.AddRelativeForce(Vector3.forward * Speed, ForceMode.Impulse);
    }

    // Update is called once per frame
    void Update()
    {
        SpriteChild.transform.LookAt(player.transform);
        rb.AddRelativeForce(Vector3.forward * Speed, ForceMode.Impulse);
        //rb.AddForce(target.position * Speed, ForceMode.Impulse);
        
        
        //transform.position = Vector3.MoveTowards(transform.position, target.position, Speed * Time.deltaTime);
        //rb.MovePosition(player.transform.position * Time.deltaTime);
    }

    void OnTriggerEnter(Collider collided){
        if(collided.gameObject.tag == "Wall" || collided.gameObject.tag == "Floor")
        {
            gameObject.SetActive(false);
        }
    }
}
