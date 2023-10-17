using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PebbleSpawn : MonoBehaviour
{
    public Sprite[] spriteArray;
    public SpriteRenderer spriteRenderer;
    // Start is called before the first frame update
    void Start()
    {
        int rng = Random.Range(0, spriteArray.Length);
        spriteRenderer.sprite = spriteArray[rng];
    }
}
