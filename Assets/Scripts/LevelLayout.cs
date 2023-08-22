using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using System.IO;
using UnityEngine.Networking;

public class LevelLayout : MonoBehaviour
{
    [HeaderAttribute("Player")]
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
    public GameObject Archway;
    public GameObject ArchwaySmall;
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

    private MapJson mapJson;

    void Awake()//Start()
    {
        levelName = PlayerPrefs.GetString("LevelToLoad", "level1.lem");
        if(levelName.Equals("END")){
            LoadEnd();
            return;
        }

        AudioListener.pause = false;
        

        //TODO: ReadNextLevel(because of the exit door, so we know what to load next)


        AudioLoad(9); //Loads random track from Resources/Music 
        ReadMap();
        AddObjectsToScene();



    }

    private void LoadEnd(){
        SceneManager.LoadScene("End");
    }

    private void AudioLoad(int SongCount){
        int randomInt = UnityEngine.Random.Range(1, SongCount + 1);
        AudioClip musicTrack = Resources.Load("Music/" + randomInt) as AudioClip;
        GetComponent<AudioSource>().clip = musicTrack;
        GetComponent<AudioSource>().Play();
    }

    private GameObject Door;
    private GameObject Key;

    private void ReadMap(){
        string levelToLoad = PlayerPrefs.GetString("LevelToLoad");
        string path = Application.streamingAssetsPath + "/Maps/" + levelToLoad;
        string levelJsonData = System.IO.File.ReadAllText(path);
        mapJson = JsonUtility.FromJson<MapJson>(levelJsonData);
    }

    private void AddObjectsToScene(){
        //add all members of the mapJson to the scene
        //to see which prefab to instantiate, check the string value "type" of the object
        AddMapObjects();
        AddPlayer();
        AddPickups();
        AddNpcs();
        surface.BuildNavMesh();
    }

    private void AddPlayer(){
        //add player to the scene
        GameObject playerObject = Instantiate(PlayerPrefab, new Vector3(mapJson.PlayerGameObject.X, 1.00f, mapJson.PlayerGameObject.Y), Quaternion.identity);
        GameObject.Find("GameManager").GetComponent<MessageManager>().player = playerObject;
        //TODO: add player properties
    }

    private void AddPickups(){
        //add pickups to the scene
        foreach(PickupJson pickup in mapJson.Pickups){
            switch(pickup.Type){
                case "Health":
                    Instantiate(HealthPickup, new Vector3(pickup.X, 1.5f, pickup.Y), Quaternion.identity);
                    break;
                case "ShotgunAmmo":
                    Instantiate(ShotgunAmmoPickup, new Vector3(pickup.X, 1.5f, pickup.Y), Quaternion.identity);
                    break;
                case "RevolverAmmo":
                    Instantiate(RevolverAmmoPickup, new Vector3(pickup.X, 1.5f, pickup.Y), Quaternion.identity);
                    break;
            }
            //TODO: set pickup properties
        }
    }

    private void AddNpcs(){
        //add npc objects to the scene
        foreach(MapNpcObjectJson npc in mapJson.MapNpcObjects){
            GameObject tmp;
            switch(npc.Type){
                case "Tri_horn":
                    tmp = Instantiate(TriImpPrefab, new Vector3(npc.X, 1.79f, npc.Y), Quaternion.identity);
                    break;
                case "Imp":
                    tmp = Instantiate(ImpPrefab, new Vector3(npc.X, 1.79f, npc.Y), Quaternion.identity);
                    break;
            }
            //TODO: set npc properties
        }
    }

    private void AddMapObjects(){
        foreach(MapObjectJson obj in mapJson.MapObjects){
            switch(obj.Type){
                case "wallBrick":
                    Instantiate(WallPrefab, new Vector3(obj.X, 1.5f, obj.Y), Quaternion.identity);
                    break;
                case "wallStone":
                    Instantiate(WallPrefab, new Vector3(obj.X, 1.5f, obj.Y), Quaternion.identity);
                    //change wall texture
                    break;
                case "wallMoss":
                    Instantiate(WallPrefab, new Vector3(obj.X, 1.5f, obj.Y), Quaternion.identity);
                    //change wall texture
                    break;
                case "tileWall":
                    Instantiate(WallPrefab, new Vector3(obj.X, 1.5f, obj.Y), Quaternion.identity);
                    //change wall texture
                    break;
                case "Cobweb_Wall":
                    Instantiate(CobwebWallPrefab, new Vector3(obj.X, 1.5f, obj.Y), Quaternion.identity);
                    break;
                case "Torch":
                    Instantiate(TorchPrefab, new Vector3(obj.X, 1.5f, obj.Y), Quaternion.identity);
                    break;
                case "ArmorBlink":
                    Instantiate(KnightPrefab, new Vector3(obj.X, 1.5f, obj.Y), Quaternion.identity);
                    break;
                case "Stone":
                    Instantiate(PebblePrefab, new Vector3(obj.X, 1.5f, obj.Y), Quaternion.identity);
                    break;
                case "ExitDoor":
                    Instantiate(ExitPrefab, new Vector3(obj.X, 1.5f, obj.Y), Quaternion.identity);
                    break;
                case "EnergyBall":
                    Instantiate(EnergySphereTrapPrefab, new Vector3(obj.X, 0.5f, obj.Y), Quaternion.identity);
                    break;
                case "ArchwaySingle":
                    Instantiate(Archway, new Vector3(obj.X, 1.5f/*1.2f*/, obj.Y), Quaternion.identity);
                    break;
                case "ArchwaySmall":
                    Instantiate(ArchwaySmall, new Vector3(obj.X, 1.5f/*1.2f*/, obj.Y), Quaternion.identity);
                    break;
                case "Key":
                    Key = Instantiate(KeyPrefab, new Vector3(obj.X, 1.5f, obj.Y), Quaternion.identity);
                    Key.GetComponentInChildren<SpriteRenderer>().color = KeyColor;
                    spawnedKey = true;
                    if(spawnedDoor)
                        Key.GetComponent<Key>().Door = Door;
                    break;
                case "DoorGate":
                    Door = Instantiate(DoorPrefab, new Vector3(obj.X, 1.5f, obj.Y), Quaternion.identity);
                    if(spawnedKey)
                        Key.GetComponent<Key>().Door = Door;
                    break;
            }
        }
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
