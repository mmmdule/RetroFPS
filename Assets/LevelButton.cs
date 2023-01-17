using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class LevelButton : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI text;
    public void ChangeLevel(){
        text = GetComponentInChildren<TextMeshProUGUI>();
        PlayerPrefs.GetString("LevelToLoad", text.text + ".png");
        SceneManager.LoadScene("Level1");
    }
}
