using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ExitDoor : MonoBehaviour
{
    public string nextLevel;

    // Update is called once per frame
    void OnTriggerEnter(Collider collision)
    {
        if(collision.gameObject.tag == "Player"){
            string mapName = PlayerPrefs.GetString("LevelToLoad");
            SceneManager.LoadScene(mapName == null || mapName == "" ? "MainMenu" : "LoadingScene");
        }
    }
}
