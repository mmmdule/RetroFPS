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
    void Awake()
    {
        ReadEndingText();
        array = endingText.ToCharArray();
        InvokeRepeating(nameof(LetterByLetter), delay, repeatRate);
    }
    string endingText;
    [SerializeField]
    TextMeshProUGUI textMeshPro;
    [SerializeField]
    AudioSource audioSource;

    int index = 0;
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
            SceneManager.LoadScene("MainMenu");
        }
    }

    void ReadEndingText(){
        using (StreamReader sr = new StreamReader(Application.streamingAssetsPath + "/EndText.txt")){
            endingText = sr.ReadToEnd();
            sr.Close();
        }
    }
}
