using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class WallTexture : MonoBehaviour
{
    public Material[] materialArray;
    public MeshRenderer meshRenderer;
    // Start is called before the first frame update
    void Start()
    {
        string[] mapNames, wallMaterials;
        int i;
        string currentMap = PlayerPrefs.GetString("LevelToLoad", "level1.png");

        using (StreamReader sr = new StreamReader(Application.streamingAssetsPath + "/MapNames.txt")){
            mapNames = sr.ReadToEnd().Split("\n");
            for(i = 0; i < mapNames.Length; i++)
                mapNames[i] = mapNames[i].Substring(0, mapNames[i].Length - 1);
            for(i = 0; i < mapNames.Length; i++)
                if(mapNames[i].Equals(currentMap))
                    break;
            sr.Close();
        }

        using (StreamReader sr = new StreamReader(Application.streamingAssetsPath + "/WallTextures.txt")){
            wallMaterials = sr.ReadToEnd().Split("\n");
            meshRenderer.material = materialArray[int.Parse(wallMaterials[i])];
            sr.Close();
        }
    }
}
