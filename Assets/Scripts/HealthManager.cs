using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthManager : MonoBehaviour
{
    public Text HealthText;

    public int health = 100;
    public AudioSource audioSource;
    public MessageManager messageManager;
    // Start is called before the first frame update
    void Start()
    {
        messageManager = GameObject.Find("GameManager").GetComponent<MessageManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void TakeDamage(int damage)
    {
        Debug.LogError("TAKEN " + damage + " DAMAGE!");
        audioSource.Play(); //play Pain/Death sound
        health -= damage;

        ChangeHealthText(health);
        if (health <= 0) {
            ChangeHealthText(0);
            Invoke(nameof(ShowGameOver), audioSource.clip.length - 0.065f);
        }
    }

    public void ShowGameOver(){
        HealthText.text = "GAME OVER";
        transform.position = new Vector3(1000, 1000, 1000);
        gameObject.GetComponentInChildren<Image>().enabled = false;
        (gameObject.GetComponentsInParent<Shoot>())[0].enabled = false;
        (gameObject.GetComponentsInParent<PlayerMovement>())[0].enabled = false;
    }

    public void ChangeHealthText(int value){
        HealthText.text = "+ " + value.ToString();
    }
}
