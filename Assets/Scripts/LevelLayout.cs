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
    
    public GameObject ExitPrefab;
    
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

    [HeaderAttribute("Ceilling Light")]
    //[SerializeField]
    /*public*/private Color LightColor;

    [HeaderAttribute("Keys")]
    public GameObject KeyPrefab;
    public Color KeyColor;
    public bool spawnedKey = false;

    public NavMeshSurface surface;

    // Start is called before the first frame update
    
    private string[] mapNames, lightColors;
    private string levelName;
    public string nextLevel;
    private string LightColorString;
    void Awake()//Start()
    {
        levelName = PlayerPrefs.GetString("LevelToLoad", "level1.png");
        if(levelName.Equals("END")){
            LoadEnd();
            return;
        }

        

        //Read NextLevel (for ExitDoor)
        string levelNameTmp = PlayerPrefs.GetString("LevelToLoad", "level1.png");
        int i;
        using (StreamReader sr = new StreamReader(Application.streamingAssetsPath + "/MapNames.txt")){
            mapNames = sr.ReadToEnd().Split("\n");
            for(i = 0; i < mapNames.Length - 1; i++) //-1 jer je 1 "END.png"
                if(mapNames[i].Equals(levelNameTmp))
                    break;
            nextLevel = mapNames[i+1];
            sr.Close();
        }

        //Read LightColors (for SpotLights)
        using (StreamReader sr = new StreamReader(Application.streamingAssetsPath + "/LightColors.txt")){
            lightColors = sr.ReadToEnd().Split("\n");
            LightColorString = lightColors[i];
            Debug.Log(LightColorString);
            sr.Close();
        }

        //ReadColor();//PlayerPrefs.GetString("LevelToLoad", "level1"));
        AudioLoad(PlayerPrefs.GetString("LevelToLoad", "level1.png"));
        StartCoroutine(GetMapFileUWR());



    }

    private void LoadEnd(){
        SceneManager.LoadScene("End");
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
    private enum ColorCode{
        Red, Blue, White, Yellow, Magenta, Green
    }
    private void IntToLightColor(string ColorName){
        ColorCode ColorInt = (ColorCode)int.Parse(ColorName);
        //Debug.Log("Light Color String passed to method is: " + ColorName);
        float[] rgb = {1.00f, 1.00f, 1.00f};
        switch(ColorInt){
            case ColorCode.Red:
                rgb[0] = 1.00f;
                rgb[1] = 0.00f;
                rgb[2] = 0.00f;
                //the default standard
                break;
            case ColorCode.Blue:
                rgb[0] = 0.00f;
                rgb[1] = 0.00f;
                rgb[2] = 1.00f;
                break;
            case ColorCode.White:
                rgb[0] = 1.00f;
                rgb[1] = 1.00f;
                rgb[2] = 1.00f;
                break;
            case ColorCode.Yellow:
                rgb[0] = 1.00f;
                rgb[1] = 0.92f;
                rgb[2] = 0.016f;
                break;
            case ColorCode.Magenta:
                rgb[0] = 1.00f;
                rgb[1] = 0.00f;
                rgb[2] = 1.00f;
                break;
            case ColorCode.Green:
                rgb[0] = 0.00f;
                rgb[1] = 1.00f;
                rgb[2] = 0.00f;
                break;
            default:
                break;
        }
        Debug.LogWarning("RGB array: " + rgb[0] + " " + rgb[1] + " " + rgb[2]);
        LightColor[0] = rgb[0];
        LightColor[1] = rgb[1];
        LightColor[2] = rgb[2];
        LightColor[3] =  1.00f;
        //Debug.LogWarning("Light Color at end of to method is: " + LightColor);
    }
    private void ReadColor(){
        Color32 currentColor = new Color();

        IntToLightColor(LightColorString);
        Debug.LogError(LightColor);


        for(int i = 0; i < 64; i++){
            for(int j = 0; j < 64; j++){
                currentColor = LevelMap.GetPixel(i, j);
                
                if(currentColor == Color.green){
                    GameObject tmpObject = Instantiate(PlayerPrefab, new Vector3(i, 1.00f/*1.8f*/, j), Quaternion.identity);
                    GameObject.Find("GameManager").GetComponent<MessageManager>().player = tmpObject;
                }
                else if (currentColor == Color.blue)
                    Instantiate(WallPrefab, new Vector3(i, 1.5f/*1.2f*/, j), Quaternion.identity);
                else if (currentColor.r == 255 && currentColor.g == 0 && currentColor.b == 255)
                    Instantiate(ExitPrefab, new Vector3(i, 1.5f/*1.2f*/, j), Quaternion.Euler(0, 270, 0)).GetComponent<ExitDoor>().nextLevel = nextLevel;
                else if (currentColor.r == 15 && currentColor.g == 10 && currentColor.b == 120f)
                    Instantiate(CobwebWallPrefab, new Vector3(i, 1.5f/*1.2f*/, j), Quaternion.identity);
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
                else if (currentColor.r == 0 && currentColor.g == 217 && currentColor.b == 234){
                    Instantiate(Column, new Vector3(i, 1.5f/*1.2f*/, j), Quaternion.identity);
                }
                else if (currentColor.r == 155 && currentColor.g == 17 && currentColor.b == 124){
                    Instantiate(ColumnTwo, new Vector3(i, 1.5f/*1.2f*/, j), Quaternion.identity);
                }
                // else if (currentColor.r == 0.2f && currentColor.g == 0.00f && currentColor.b == 0.2f){
                //     Instantiate(EmptyBlockPrefab, new Vector3(i, 1.5f/*1.2f*/, j), Quaternion.identity);
                // }
                //Debug.Log(currentColor);
            }
        }
        surface.BuildNavMesh();
    }

    private bool devMode = false;
    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Home)){
            devMode = !devMode;
            GetComponent<MessageManager>().MessageUpdate("Dev mode: " + devMode.ToString(), 1.2f);
        }
        if(devMode){
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
}
