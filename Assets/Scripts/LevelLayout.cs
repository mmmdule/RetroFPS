using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

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

    public GameObject WallPrefab;
    
    public GameObject PlayerPrefab;
    
    public GameObject DoorPrefab;
    public GameObject ImpPrefab;
    public GameObject TriImpPrefab;
    public GameObject EmptyBlockPrefab;
    public GameObject TorchPrefab;
    public GameObject KnightPrefab;
    public GameObject PebblePrefab;
    public GameObject EnergySphereTrapPrefab;
    public GameObject HealthPickup, ShotgunAmmoPickup, RevolverAmmoPickup;
    public bool spawnedDoor = false;

    public GameObject LightPrefab;
    public Color LightColor;

    public GameObject KeyPrefab;
    public Color KeyColor;
    public bool spawnedKey = false;

    public NavMeshSurface surface;

    // Start is called before the first frame update
    
    [Header("Health Settings")]
    public int resX = 1280;
    public int resY = 720;
    void Start()
    {
        ReadColor("Maps/" + PlayerPrefs.GetString("LevelToLoad", "level1 19"));
        AudioLoad(PlayerPrefs.GetString("LevelToLoad", "level1 19"));
        Application.targetFrameRate = 60;
        Screen.SetResolution(resX, resY, true);//Screen.SetResolution(1280, 720, true);
    }

    private void AudioLoad(string levelName){
        AudioClip musicTrack = Resources.Load("Music/" + levelName) as AudioClip;
        GetComponent<AudioSource>().clip = musicTrack;
        GetComponent<AudioSource>().Play();
    }

    private GameObject Door;
    private GameObject Key;

    private void ReadColor(string levelName){
        Color32 currentColor = new Color();

        //konvertuje sliku u teksturu za citanje
        LevelMap = Resources.Load(levelName) as Texture2D;

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

    public string[] mapNames;
    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.F1)){
            PlayerPrefs.SetString("LevelToLoad", mapNames[0]);
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
        else if (Input.GetKeyDown(KeyCode.F2)){
            PlayerPrefs.SetString("LevelToLoad", mapNames[1]);
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}
