using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class Ending : MonoBehaviour
{
    // Start is called before the first frame update
    char[] array;
    
    [SerializeField]
    float delay;

    [SerializeField]
    float repeatRate;

    string storyText;

    [SerializeField]
    TextMeshProUGUI textMeshPro;

    [SerializeField]
    AudioSource audioSource;

    int index = 0;
    
    int nextLevelInt = -1;

    ProjectJson project;
    MapJson map;

    void Awake()
    {
        storyText = ReadStoryText();
        array = storyText.ToCharArray();
        nextLevelInt = NextLevelManager.NextSegment();
        InvokeRepeating(nameof(LetterByLetter), delay, repeatRate);
    }
    void LetterByLetter(){
        if(index < array.Length){
            textMeshPro.text += array[index++];
            audioSource.enabled = index < array.Length;
        }
    }
    // Update is called once per frame
    void Update()
    {
        if(index >= array.Length && (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.Space))){
            switch(nextLevelInt){
                case 0:
                    SceneManager.LoadScene("MainMenu");
                    break;
                case 1:
                    SceneManager.LoadScene("End");
                    break;
                case 2:
                    SceneManager.LoadScene("LoadingScene");
                    break;
                default:
                    break;
            }
        }
    }

    private string ReadStoryText(){
        string levelToLoad = PlayerPrefs.GetString("LevelToLoad");
        string levelJson =  System.IO.File.ReadAllText(Application.streamingAssetsPath + "/Maps/" + levelToLoad);
        map = JsonUtility.FromJson<MapJson>(levelJson);
        return map.StoryText;
    }
}
