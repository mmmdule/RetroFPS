using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Key : MonoBehaviour
{
    Camera mainCamera;
    public GameObject KeySprite;
    public GameObject Door;
    
    [SerializeField]
    private float fadeSpeed = 0.015f;

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
            RemoveDoorCollider();
            FadeDoorOut();
            //Door.SetActive(false); //Removes Door
            GameObject.Find("GameManager").GetComponent<MessageManager>().MessageUpdate("Door Unlocked.", 4.5f);
            //Show UI Message
        }
    }

    void FadeDoorOut(){
        //find sprite renderers in Door children and fade them out slowly in a coroutine
        foreach(SpriteRenderer sr in Door.GetComponentsInChildren<SpriteRenderer>()){
            StartCoroutine(FadeOut(sr));
        }
    }

    IEnumerator FadeOut(SpriteRenderer sr){
        for(float f = 1f; f >= 0; f -= 0.01f){
            Color c = sr.color;
            c.a = f;
            sr.color = c;
            yield return new WaitForSeconds(fadeSpeed);
        }
    }

    void RemoveDoorCollider(){
        //find colliders in Door children and disable them
        foreach(Collider c in Door.GetComponentsInChildren<Collider>()){
            c.enabled = false;
        }
    }
}
