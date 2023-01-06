using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShotDetectionPlayer : MonoBehaviour
{
    BoxCollider boxCollider;
    // Start is called before the first frame update
    void Start()
    {
        boxCollider = GetComponent<BoxCollider>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void DisableTrigger(){
        boxCollider.enabled = false;
    }
    
    public void EnableTrigger(){
        boxCollider.enabled = true;
    }
    void OnTriggerEnter(Collider collided){
        if(collided.gameObject.tag == "Imp")
        {
            collided.gameObject.SetActive(false);
            Debug.Log("Hit imp");
        }
    }
}
