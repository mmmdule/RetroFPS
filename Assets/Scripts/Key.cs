using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Key : MonoBehaviour
{
    Camera mainCamera;
    public GameObject KeySprite;
    public GameObject Door;

    void Start(){
    }

    public bool cameraIsSet = false;
    void LateUpdate()
    {
        if(!cameraIsSet){
            if(GameObject.FindGameObjectWithTag("Player") == null)
                return;
            
            mainCamera = Camera.main;
            cameraIsSet = true;
        }

        KeySprite.transform.LookAt(mainCamera.transform);
        KeySprite.transform.Rotate(0, 180, 0);
    }

    void OnTriggerEnter(Collider collided){
        if(collided.gameObject.tag == "Player" && KeySprite.activeSelf)
        {
            KeySprite.SetActive(false);
            //Play Sound
            gameObject.GetComponent<AudioSource>().Play();
            Door.SetActive(false); //Removes Door ("Unlocks")
            GameObject.Find("GameManager").GetComponent<MessageManager>().MessageUpdate("Door Unlocked.", 4.5f);
            //Show UI Message
        }
    }
}
