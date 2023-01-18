using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class LevelButton : MonoBehaviour
{
    TextMeshProUGUI text;
    public void ChangeLevel(){
        text = GetComponentInChildren<TextMeshProUGUI>();
        PlayerPrefs.SetString("LevelToLoad", text.text + ".png");
        SceneManager.LoadScene("Level1");
    }
}
