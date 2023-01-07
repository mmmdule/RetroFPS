using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleBillboard : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }
    private Transform player;
    public SpriteRenderer thisSprite;
    // Update is called once per frame
    void Update()
    {
        if(GameObject.FindGameObjectWithTag("Player") == null)
            return;
        
        if(player == null)
            player = GameObject.FindGameObjectWithTag("Player").transform;

        thisSprite.gameObject.transform.LookAt(player);
    }
}
