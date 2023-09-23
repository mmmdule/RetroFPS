using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class MainMenu : MonoBehaviour
{
    private List<string> mapNames;

    [SerializeField]
    TextMeshProUGUI Heading;
    [SerializeField]
    TextMeshProUGUI EpisodeTitle;
    
    bool canExit = false;

    private ProjectJson project;

    // Start is called before the first frame update
    void Awake()
    {
        #if UNITY_STANDALONE && !UNITY_EDITOR
            canExit = true;
        #endif
        
        AudioListener.pause = PlayerPrefs.GetInt("Sound", 1) == 0;

        PlayerPrefs.SetString("LevelToLoad", "primera.lem"); //sets the default just in case

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;

        project = ReadProjectFile();
        if(project == null)
            return;

        Heading.text = project.GameTitle;
        EpisodeTitle.text = project.GameSubtitle;
        ReadLevels();
        CreateList();
    }
    
    void Update(){
        if(Input.GetKey(KeyCode.Escape) && canExit)
            Application.Quit();
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
        for(int i = 0; i < mapNames.Count; i++){
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
        for(int i = 0; i < mapNames.Count - 1; i++){
            RectTransform tmpRect = buttonList[i].GetComponent<RectTransform>();
            tmpRect.anchoredPosition3D = new Vector3(tmpRect.anchoredPosition3D.x, (y - (i * 100.00f)), tmpRect.anchoredPosition3D.z);
        }
    }
    
    private ProjectJson ReadProjectFile(){
        //locate all .lem files in the streaming assets folder
        string[] lepFiles = Directory.GetFiles(Application.streamingAssetsPath, "*.lep");
        
        if (lepFiles.Length != 1){
            //TODO: Show error message (depending on .lem count) on prefab button and disable it's script
            Heading.text = "ERROR";
            EpisodeTitle.text = lepFiles.Length == 0 ? "No .lep files found" : "Multiple .lep files found";
            buttonPrefab.GetComponentInChildren<TextMeshProUGUI>().text = "Please have only one .lep file at a time";
            LevelButton lvlButtonScript = buttonPrefab.GetComponent<LevelButton>();
            Destroy(lvlButtonScript); //disables the button's script
            return null;
        }
       
        //json parse the 1 .lem file and return it as ProjectJson
        Debug.Log(lepFiles[0]);
        return JsonUtility.FromJson<ProjectJson>(File.ReadAllText(lepFiles[0]));
        
    }
    void ReadLevels(){
        mapNames = new List<string>();
        foreach(string mapName in project.MapNameList){
            MapJson map = ReadMap(mapName);
            Debug.Log(mapName + "; is story text segment: " + map.StoryTextSegment);
            if(!map.StoryTextSegment)
                mapNames.Add(mapName);
        }
    }

    public static MapJson ReadMap(string mapName)
    {
        try
        {
            //json parse map from streaming assets/maps/<MapName>.lem
            MapJson map = JsonUtility.FromJson<MapJson>(File.ReadAllText(Application.streamingAssetsPath + "\\maps\\" + mapName + ".lem"));

            return map;
        }
        catch
        {
            return null;
        }
    }
}
