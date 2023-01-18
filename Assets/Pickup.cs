using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickup : MonoBehaviour
{
    // Start is called before the first frame update
    private GameObject player;
    private bool foundPlayer = false;
    public SpriteRenderer spriteRenderer;
    public BoxCollider boxCollider;
    MessageManager msg;
    void Awake(){
        msg = GameObject.Find("GameManager").GetComponent<MessageManager>();
    }
    void Start(){
        //spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        //boxCollider = GetComponent<BoxCollider>();
    }

    void Update()
    {
        if(!foundPlayer){
            player = GameObject.FindGameObjectWithTag("Player");
            if(player == null)
                return;
            else{
                foundPlayer = true;
                healthManager = player.GetComponent<HealthManager>();
                ammoManager = player.GetComponent<AmmoManager>();
            }
        }
    }
    public AudioSource audioSource;
    public bool health;
    public bool shotgunAmmo;
    public bool revolverAmmo;

    //Managers
    public HealthManager healthManager;
    public AmmoManager ammoManager;
    //Managers

    //Limits and values
    public int HealthPickupValue = 10;
    public int ShotgunAmmoPickupValue = 7;
    public int RevolverAmmoPickupValue = 15;
    public int RevolverLimit = 100, ShotgunLimit = 50;
    //Limits and values

    void OnTriggerEnter(Collider collision){
        bool delete = false;
        Debug.LogWarning("Collided with " + collision.gameObject.tag);
        if(collision.gameObject.tag == "Player"){
            if(health){
                if(healthManager.health < 100){
                    healthManager.HealthPickup(HealthPickupValue);
                    msg.MessageUpdate("+" + HealthPickupValue + " health", 0.9f);
                    delete = true;
                }
            }
            else if(shotgunAmmo){
                if(ammoManager.shotgunAmmo < ShotgunLimit){
                    ammoManager.IncreaseAmmo(ShotgunAmmoPickupValue, 3);
                    msg.MessageUpdate("+" + ShotgunAmmoPickupValue + " shells", 0.9f);
                    delete = true;
                }
            }
            else if(revolverAmmo){
                if(ammoManager.pistolAmmo < RevolverLimit){
                    msg.MessageUpdate("+" + RevolverAmmoPickupValue + " bullets", 0.9f);
                    ammoManager.IncreaseAmmo(RevolverAmmoPickupValue, 2);
                    delete = true;
                }
            }
            if(delete){
                audioSource.Play();
                spriteRenderer.enabled = false;
                boxCollider.enabled = false;
                Invoke("Remove", audioSource.clip.length);
            }
        }
        
    }
    void Remove(){
        gameObject.SetActive(false);
        Destroy(gameObject);
    }
}
