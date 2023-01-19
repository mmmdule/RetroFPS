using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MainMenu : MonoBehaviour
{

    private string heading, episode;
    private string[] mapNames;
    [SerializeField]
    TextMeshProUGUI Heading;
    [SerializeField]
    TextMeshProUGUI EpisodeTitle;
    // Start is called before the first frame update
    void Awake()
    {
        
        AudioListener.pause = false;

        PlayerPrefs.SetString("LevelToLoad", "level1.png"); //sets the default just in case

        ReadHeadingFile();
        ReadLevelNames();
        
        Heading.text = heading;
        EpisodeTitle.text = episode;
        
        CreateList();
        
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;
    }
    [SerializeField]
    GameObject buttonPrefab;
    [SerializeField]
    GameObject content;
    List<GameObject> buttonList;
    void CreateList(){
        float y = 100;
        RectTransform DefaultButtonRect = buttonPrefab.GetComponent<RectTransform>();
        buttonList = new List<GameObject>();
        for(int i = 0; i < mapNames.Length - 1; i++){
            GameObject tmp = Instantiate(buttonPrefab);
            tmp.transform.SetParent(content.transform);
            tmp.GetComponent<RectTransform>().anchoredPosition3D = new Vector3(DefaultButtonRect.anchoredPosition3D.x, (y - (i * 100.00f)), DefaultButtonRect.anchoredPosition3D.z);
            tmp.GetComponent<RectTransform>().sizeDelta = DefaultButtonRect.sizeDelta;
            tmp.GetComponent<RectTransform>().localScale = DefaultButtonRect.localScale;
            tmp.GetComponentInChildren<TextMeshProUGUI>().text = mapNames[i];
            buttonList.Add(tmp);
            //i+1 * 100
        }
        buttonPrefab.SetActive(false);
        for(int i = 0; i < mapNames.Length - 1; i++){
            RectTransform tmpRect = buttonList[i].GetComponent<RectTransform>();
            tmpRect.anchoredPosition3D = new Vector3(tmpRect.anchoredPosition3D.x, (y - (i * 100.00f)), tmpRect.anchoredPosition3D.z);
        }
    }
    
    void ReadHeadingFile(){
        using (StreamReader sr = new StreamReader(Application.streamingAssetsPath + "/GameTitle.txt")){
            heading = sr.ReadLine();
            episode = sr.ReadLine();
            sr.Close();
        }
    }
    void ReadLevelNames(){
        using (StreamReader sr = new StreamReader(Application.streamingAssetsPath + "/MapNames.txt")){
            mapNames = sr.ReadToEnd().Split("\n");
            sr.Close();
        }
        for(int i = 0; i < mapNames.Length - 1; i++){
            mapNames[i] = mapNames[i].Split(".png")[0];
        }
    }
}
