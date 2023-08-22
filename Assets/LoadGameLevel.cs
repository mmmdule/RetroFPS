using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System;

public class LoadGameLevel : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI text;

    private bool error = false;

    // Start is called before the first frame update
    void Start()
    {
        //check if map can be parsed without error.
        //catch error and display error message if not.
        try {
            string levelToLoad = PlayerPrefs.GetString("LevelToLoad");
            if(levelToLoad == null || levelToLoad == "")
                throw new Exception("LevelToLoad is null or empty!");

            Debug.LogWarning(Application.streamingAssetsPath + "/Maps/" + levelToLoad);

            string levelJson =  System.IO.File.ReadAllText(Application.streamingAssetsPath + "/Maps/" + levelToLoad);
            MapJson map = JsonUtility.FromJson<MapJson>(levelJson);
            //why is the player game object alway null?
            Debug.Log(map);
            

            if(map.PlayerGameObject == null)
                throw new Exception("PlayerGameObject is null!");
            SceneManager.LoadScene("Level1");
        }
        catch (Exception ex){
            if(ex.Message != null)
                text.text = ex.Message;   
            else
                text.text = "Error loading level!\n\nPress any key to return to menu...";   
            error = true;
        }
    }

    void Update(){
        if(error && Input.anyKeyDown){
            SceneManager.LoadScene("MainMenu");
        }
    }
}
