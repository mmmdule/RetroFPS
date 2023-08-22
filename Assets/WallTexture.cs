using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class WallTexture : MonoBehaviour
{
    public Material[] materialArray;
    public MeshRenderer meshRenderer;

    [SerializeField]
    public int materialIndex = 0;

    // Start is called before the first frame update
    void Start()
    {   
        meshRenderer.material = materialArray[materialIndex];
    }
}
