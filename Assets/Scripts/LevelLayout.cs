using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using System.IO;
using UnityEngine.Networking;

public class LevelLayout : MonoBehaviour
{
    private Dictionary<int, int> enemy = new Dictionary<int, int>();
	private Dictionary<int, int> player = new Dictionary<int, int>();
	private Dictionary<int, int> wall = new Dictionary<int, int>();
	private Dictionary<int, int> door = new Dictionary<int, int>();
	private Dictionary<int, int> ground = new Dictionary<int, int>();
	private Dictionary<int, int> win = new Dictionary<int, int>();
	private Dictionary<int, int> key = new Dictionary<int, int>();

    public Texture2D LevelMap;
    
    public GameObject PlayerPrefab;
    

    [HeaderAttribute("Walls and Doors")]
    public GameObject WallPrefab;
    public GameObject CobwebWallPrefab;
    
    public GameObject DoorPrefab;
    
    [HeaderAttribute("Enemies")]
    public GameObject ImpPrefab;
    public GameObject TriImpPrefab;
    public GameObject EmptyBlockPrefab;
    [HeaderAttribute("Traps")]
    public GameObject EnergySphereTrapPrefab;
    [HeaderAttribute("Decorations")]
    public GameObject Column;
    public GameObject ColumnTwo;
    public GameObject TorchPrefab;
    public GameObject KnightPrefab;
    public GameObject PebblePrefab;
    [HeaderAttribute("Pickups")]
    public GameObject HealthPickup; 
    public GameObject ShotgunAmmoPickup;
    public GameObject RevolverAmmoPickup;
    public bool spawnedDoor = false;

    [HeaderAttribute("Lights")]
    public GameObject LightPrefab;
    public Color LightColor;

    [HeaderAttribute("Keys")]
    public GameObject KeyPrefab;
    public Color KeyColor;
    public bool spawnedKey = false;

    public NavMeshSurface surface;

    // Start is called before the first frame update
    
    private string[] mapNames;
    private string levelName;
    void Start()
    {
        //string dir = Application.streamingAssetsPath.Replace(@"/", @"\") + @"\Maps\";
        //string levelName = PlayerPrefs.GetString("LevelToLoad", "level1.png"); //+ ".png";
        //Debug.Log(dir + levelName);
        
        //Debug.Log(File.Exists((dir + levelName)) ? "Map file exists." : "Map file doesn't exist.");
        


        using (StreamReader sr = new StreamReader(Application.streamingAssetsPath + "/MapNames.txt")){
            mapNames = sr.ReadToEnd().Split("\n");
            foreach(string s in mapNames)
                //Debug.Log(s);
            sr.Close();
        }
        
        levelName = PlayerPrefs.GetString("LevelToLoad", "level1.png");
        //ReadColor();//PlayerPrefs.GetString("LevelToLoad", "level1"));
        AudioLoad(PlayerPrefs.GetString("LevelToLoad", "level1.png"));
        StartCoroutine(GetMapFileUWR());
        
    }

    private void AudioLoad(string levelName){
        levelName = levelName.Split(".png")[0];
        AudioClip musicTrack = Resources.Load("Music/" + levelName) as AudioClip;
        GetComponent<AudioSource>().clip = musicTrack;
        GetComponent<AudioSource>().Play();
    }

    private GameObject Door;
    private GameObject Key;

    private IEnumerator GetMapFileUWR(){
        string relativePath = "/Maps/" + levelName;
        string fullPath = "file:////" + Application.streamingAssetsPath + relativePath;//Path.Combine(Application.streamingAssetsPath, relativePath);
        Debug.LogWarning(fullPath);
        LevelMap = new Texture2D(2, 2, TextureFormat.RGB24, false);

        using (UnityWebRequest uwr = UnityWebRequestTexture.GetTexture(fullPath)){
            yield return uwr.SendWebRequest(); 
            if (uwr.result == UnityWebRequest.Result.ConnectionError) { 
                Debug.Log(uwr.error); 
            }
            else { 
               LevelMap = DownloadHandlerTexture.GetContent(uwr); 
               ReadColor();
            }
        }
    }
    
    private void ReadColor(){
        Color32 currentColor = new Color();

        //konvertuje sliku u teksturu za citanje
        //LevelMap = Resources.Load(levelName) as Texture2D;
        
        // string relativePath = "Maps//" + levelName;
        // string fullPath = "file://" + Path.Combine(Application.streamingAssetsPath, relativePath);
        

        //LevelMap = new Texture2D(2, 2, TextureFormat.RGB24, false);
        

        //byte[] imageBytes = File.ReadAllBytes(@fullPath);

        // LevelMap = new Texture2D(2, 2, TextureFormat.RGB24, false);
        //LevelMap.LoadImage(imageBytes);

        

        for(int i = 0; i < 64; i++){
            for(int j = 0; j < 64; j++){
                currentColor = LevelMap.GetPixel(i, j);
                
                if(currentColor == Color.green){
                    GameObject tmpObject = Instantiate(PlayerPrefab, new Vector3(i, 1.00f/*1.8f*/, j), Quaternion.identity);
                    GameObject.Find("GameManager").GetComponent<MessageManager>().player = tmpObject;
                }
                else if (currentColor == Color.blue)
                    Instantiate(WallPrefab, new Vector3(i, 1.5f/*1.2f*/, j), Quaternion.identity);
                else if (currentColor == Color.black && !spawnedDoor){
                    Door = Instantiate(DoorPrefab, new Vector3(i, 1.5f/*1.2f*/, j), Quaternion.identity);
                    spawnedDoor = true;
                    if(spawnedKey)
                        Key.GetComponent<Key>().Door = Door;
                }
                else if (currentColor == Color.white){
                    //spawns light with X rotation of 90 deg (Euler)
                    Instantiate(LightPrefab, new Vector3(i, 4.00f, j), Quaternion.Euler(new Vector3(90, 0, 0))).GetComponent<Light>().color = LightColor;
                }
                /*Yellow*/ else if (currentColor.r == 255 && currentColor.g == 255 && currentColor.b == 0.00f && !spawnedKey){
                    Key = Instantiate(KeyPrefab, new Vector3(i,  1.5f/*1.2f*/, j), Quaternion.identity);
                    Key.GetComponentInChildren<SpriteRenderer>().color = KeyColor;
                    spawnedKey = true;
                    if(spawnedDoor)
                        Key.GetComponent<Key>().Door = Door;
                }
                else if (currentColor == Color.red)
                    Instantiate(ImpPrefab, new Vector3(i, 1.79f, j), Quaternion.identity);
                /*Dark Red*/else if (currentColor.r == 185 && currentColor.g == 0 && currentColor.b == 30)
                    Instantiate(TriImpPrefab, new Vector3(i, 1.79f, j), Quaternion.identity);
                else if (currentColor.r == 192 && currentColor.g == 192 && currentColor.b == 192){
                    Instantiate(TorchPrefab, new Vector3(i, 1.5f/*1.2f*/, j), Quaternion.identity);
                }
                else if (currentColor.r == 255 && currentColor.g == 153 && currentColor.b == 255){
                    Instantiate(KnightPrefab, new Vector3(i, 1.5f/*1.2f*/, j), Quaternion.identity);
                }
                else if (currentColor.r == 255 && currentColor.g == 128 && currentColor.b == 64){
                    Instantiate(PebblePrefab, new Vector3(i, 1.5f/*1.2f*/, j), Quaternion.identity);
                }
                else if (currentColor.r == 0 && currentColor.g == 162 && currentColor.b == 232){
                    Instantiate(EnergySphereTrapPrefab, new Vector3(i, 0.5f/*1.2f*/, j), Quaternion.identity);
                }
                else if (currentColor.r == 0 && currentColor.g == 64 && currentColor.b == 0){
                    Instantiate(HealthPickup, new Vector3(i, 1.5f/*1.2f*/, j), Quaternion.identity);
                }
                else if (currentColor.r == 128 && currentColor.g == 64 && currentColor.b == 64){
                    Instantiate(ShotgunAmmoPickup, new Vector3(i, 1.5f/*1.2f*/, j), Quaternion.identity);
                }
                else if (currentColor.r == 128 && currentColor.g == 0 && currentColor.b == 64){
                    Instantiate(RevolverAmmoPickup, new Vector3(i, 1.5f/*1.2f*/, j), Quaternion.identity);
                }
                // else if (currentColor.r == 0.2f && currentColor.g == 0.00f && currentColor.b == 0.2f){
                //     Instantiate(EmptyBlockPrefab, new Vector3(i, 1.5f/*1.2f*/, j), Quaternion.identity);
                // }
                //Debug.Log(currentColor);
            }
        }
        surface.BuildNavMesh();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.F1) && mapNames.Length > (KeyCode.F1 - KeyCode.F1)){
            PlayerPrefs.SetString("LevelToLoad", mapNames[0]);
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
        else if (Input.GetKeyDown(KeyCode.F2) && mapNames.Length > (KeyCode.F2 - KeyCode.F1)){
            PlayerPrefs.SetString("LevelToLoad", mapNames[1]);
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
        else if (Input.GetKeyDown(KeyCode.F3) && mapNames.Length > (KeyCode.F3 - KeyCode.F1)){
            PlayerPrefs.SetString("LevelToLoad", mapNames[2]);
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
        else if (Input.GetKeyDown(KeyCode.F4) && mapNames.Length > (KeyCode.F4 - KeyCode.F1)){
            PlayerPrefs.SetString("LevelToLoad", mapNames[3]);
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
        else if (Input.GetKeyDown(KeyCode.F5) && mapNames.Length > (KeyCode.F5 - KeyCode.F1)){
            PlayerPrefs.SetString("LevelToLoad", mapNames[4]);
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
        else if (Input.GetKeyDown(KeyCode.F6) && mapNames.Length > (KeyCode.F6 - KeyCode.F1)){
            PlayerPrefs.SetString("LevelToLoad", mapNames[5]);
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
        else if (Input.GetKeyDown(KeyCode.F7) && mapNames.Length > (KeyCode.F7 - KeyCode.F1)){
            PlayerPrefs.SetString("LevelToLoad", mapNames[6]);
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
        else if (Input.GetKeyDown(KeyCode.F8) && mapNames.Length > (KeyCode.F8 - KeyCode.F1)){
            PlayerPrefs.SetString("LevelToLoad", mapNames[7]);
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
        else if (Input.GetKeyDown(KeyCode.F9) && mapNames.Length > (KeyCode.F9 - KeyCode.F1)){
            PlayerPrefs.SetString("LevelToLoad", mapNames[8]);
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
        else if (Input.GetKeyDown(KeyCode.F10) && mapNames.Length > (KeyCode.F10 - KeyCode.F1)){
            PlayerPrefs.SetString("LevelToLoad", mapNames[9]);
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
        else if (Input.GetKeyDown(KeyCode.F11) && mapNames.Length > (KeyCode.F11 - KeyCode.F1)){
            PlayerPrefs.SetString("LevelToLoad", mapNames[10]);
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
        else if (Input.GetKeyDown(KeyCode.F12) && mapNames.Length > (KeyCode.F12 - KeyCode.F1)){
            PlayerPrefs.SetString("LevelToLoad", mapNames[11]);
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}
