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

            string levelJson =  System.IO.File.ReadAllText(Application.streamingAssetsPath + "/Maps/" + levelToLoad);
            MapJson map = JsonUtility.FromJson<MapJson>(levelJson);

            if(map.PlayerGameObject.Type == null && !map.StoryTextSegment)
                throw new Exception("PlayerGameObject is null!");

            SceneManager.LoadScene(map.StoryTextSegment ? "End" : "Level1");
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
