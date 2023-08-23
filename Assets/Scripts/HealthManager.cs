using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HealthManager : MonoBehaviour
{
    public Text HealthText;
    public Image reticle;

    public int health = 100;
    public AudioSource audioSource;
    private MessageManager messageManager;

    void Awake(){
        AudioListener.pause = false;
    }

    float oldVolume;
    // Start is called before the first frame update
    void Start()
    {
        messageManager = GameObject.Find("GameManager").GetComponent<MessageManager>();
        oldVolume = audioSource.volume;
    }

    // Update is called once per frame
    void Update()
    {
        if(health <= 0 && (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.Space))){
            PlayerPrefs.SetString("LevelToLoad", PlayerPrefs.GetString("GameOverLevel"));
            SceneManager.LoadScene("LoadingScene");
        }
            
    }
    [SerializeField]
    AudioClip DeathScream;
    public void TakeDamage(int damage)
    {
        Debug.LogError("TAKEN " + damage + " DAMAGE!");
        
        health -= damage;
        
        audioSource.volume = 1;
        if(health <= 0){
            audioSource.volume = 0.9f;
            audioSource.PlayOneShot(DeathScream);
        }
        else
            audioSource.Play(); //play Pain/Death sound
        
        Invoke(nameof(ResetVolume), audioSource.clip.length);
        ChangeHealthText(health);
        if (health <= 0) {
            ChangeHealthText(0);
            
            (gameObject.GetComponentsInParent<PlayerMovement>())[0].canMove = false;
            (gameObject.GetComponentsInParent<Shoot>())[0].Die();
            (gameObject.GetComponentsInParent<Shoot>())[0].enabled = false;
            Invoke(nameof(ShowGameOver), audioSource.clip.length - 0.035f);
        }
    }

    void ResetVolume(){
        audioSource.volume = oldVolume;
    }

    [SerializeField]
    AmmoManager ammoManager;
    
    public void ShowGameOver(){
        AudioListener.pause = true;

        HealthText.text = "GAME OVER";
        transform.position = new Vector3(1000, 1000, 1000);
        gameObject.GetComponentInChildren<Image>().enabled = false;
        reticle.enabled = false;
        (gameObject.GetComponentsInParent<Shoot>())[0].enabled = false;
        (gameObject.GetComponentsInParent<PlayerMovement>())[0].enabled = false;
        ammoManager.AmmoText.text = "";
    }

    public void ChangeHealthText(int value){
        HealthText.text = "+ " + value.ToString();
    }

    public void HealthPickup(int value){
        health = health + value > 100 ? 100 : health + value;
        ChangeHealthText(health);
    }
}
