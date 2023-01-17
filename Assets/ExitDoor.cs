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
            PlayerPrefs.SetString("LevelToLoad", nextLevel);
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}
