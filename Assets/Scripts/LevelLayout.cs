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
    

    [HeaderAttribute("Lights")]
    public GameObject LightPrefab;

    [HeaderAttribute("Ceilling Light")]
    //[SerializeField]
    /*public*/private Color LightColor;

    [HeaderAttribute("Keys")]
    public GameObject KeyPrefab;
    public Color KeyColor;
    

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

        AudioListener.pause = false;
        
        //TODO: ReadNextLevel(because of the exit door, so we know what to load next)

        //has to be done before adding objects to the scene

        AudioLoad(9); //Loads random track from Resources/Music 
        ReadMap();
        string levelBeforeChange = PlayerPrefs.GetString("LevelToLoad");
        NextLevelManager.SetGameOverPref();
        NextLevelManager.NextSegment(); //sets the next level to load
        nextLevel = PlayerPrefs.GetString("LevelToLoad");
        AddObjectsToScene(mapJson);

        if(levelBeforeChange == nextLevel)
            PlayerPrefs.SetString("LevelToLoad", null); //this way exit door will load main menu
    }

    private void AudioLoad(int SongCount){
        int randomInt = UnityEngine.Random.Range(1, SongCount + 1);
        AudioClip musicTrack = Resources.Load("Music/" + randomInt) as AudioClip;
        GetComponent<AudioSource>().clip = musicTrack;
        GetComponent<AudioSource>().Play();
    }

    public bool spawnedDoor = false;
    public bool spawnedKey = false;
    private GameObject Door;
    private GameObject Key;

    private void ReadMap(){
        string levelToLoad = PlayerPrefs.GetString("LevelToLoad");
        string path = Application.streamingAssetsPath + "/Maps/" + levelToLoad;
        string levelJsonData = System.IO.File.ReadAllText(path);
        mapJson = JsonUtility.FromJson<MapJson>(levelJsonData);
    }

    private void AddObjectsToScene(MapJson map){
        //add all members of the mapJson to the scene
        //to see which prefab to instantiate, check the string value "type" of the object
        AddMapObjects();
        AddPlayer(map.PlayerGameObject);
        AddPickups();
        AddNpcs();
        surface.BuildNavMesh();
    }

    private void AddPlayer(PlayerObjectJson playerObjectJson){
        //add player to the scene
        GameObject playerObject = Instantiate(PlayerPrefab, new Vector3(playerObjectJson.X, 1.00f, playerObjectJson.Y), Quaternion.identity);
        GameObject.Find("GameManager").GetComponent<MessageManager>().player = playerObject;
        //TODO: add player properties
        HealthManager healthManager = playerObject.GetComponent<HealthManager>();
        healthManager.health = playerObjectJson.Health;
        healthManager.ChangeHealthText(playerObjectJson.Health);

        AmmoManager ammoManager = playerObject.GetComponent<AmmoManager>();
        ammoManager.shotgunAmmo = playerObjectJson.ShotgunAmmo;
        ammoManager.pistolAmmo = playerObjectJson.RevolverAmmo;
        
        Shoot shoot = playerObject.GetComponent<Shoot>();
        shoot.HasRevolver = playerObjectJson.HasRevolver;
        shoot.HasShotgun = playerObjectJson.HasShotgun;
        if(playerObjectJson.HasShotgun) {
            shoot.currentWeapon = 3;
            shoot.ChangeSprite(3);
            ammoManager.ChangeAmmoText(ammoManager.shotgunAmmo, 2);
        }
        else if(playerObjectJson.HasRevolver) {
            shoot.currentWeapon = 2;
            shoot.ChangeSprite(2);
            ammoManager.ChangeAmmoText(ammoManager.shotgunAmmo, 1);
        }
        else {
            shoot.currentWeapon = 1;
            shoot.NoWeaponSprite();
            ammoManager.ClearText();
        }
    }

    private void AddPickups(){
        //add pickups to the scene
        foreach(PickupJson pickup in mapJson.Pickups){
            GameObject tmp;
            switch(pickup.Type){
                case "SmallMedkit":
                    tmp = Instantiate(HealthPickup, new Vector3(pickup.X, 1.5f, pickup.Y), Quaternion.identity);
                    tmp.GetComponent<Pickup>().HealthPickupValue = pickup.Value;
                    break;
                case "ShotgunAmmo":
                    tmp = Instantiate(ShotgunAmmoPickup, new Vector3(pickup.X, 1.5f, pickup.Y), Quaternion.identity);
                    tmp.GetComponent<Pickup>().ShotgunAmmoPickupValue = pickup.Value;
                    break;
                case "RevolverAmmo":
                    tmp = Instantiate(RevolverAmmoPickup, new Vector3(pickup.X, 1.5f, pickup.Y), Quaternion.identity);
                    tmp.GetComponent<Pickup>().RevolverAmmoPickupValue = pickup.Value;
                    break;
            }
            //TODO: set pickup properties
        }
    }

    private void AddNpcs(){
        //maybe default should handle properties of the npc, if every npc ends up extending the same class
        //probably the 'Imp' class
        //but leave this for now

        //add npc objects to the scene
        foreach(MapNpcObjectJson npc in mapJson.MapNpcObjects){
            GameObject tmp;
            switch(npc.Type){
                case "Tri_horn":
                    tmp = Instantiate(TriImpPrefab, new Vector3(npc.X, 1.79f, npc.Y), Quaternion.identity);
                    TriImp tri = tmp.GetComponent<TriImp>();
                    tri.health = npc.Health;
                    tri.walkRange = npc.PatrolRange;
                    tri.sightRange = npc.ChaseRange;
                    tri.attackRange = npc.AttackRange;
                    tri.timeBetweenAttacks = npc.FiringRate;
                    Fireball[] fireballs = tri.projectile.GetComponentsInChildren<Fireball>();
                    foreach(Fireball fireball in fireballs) //evenly distribute damage between fireballs
                        fireball.Damage = npc.ProjectileDamage / fireballs.Length;
                    if(!npc.CanMove){
                        tri.agent.enabled = false;
                        //add rigidbody constraints for every axis position and rotation
                        Rigidbody rb = tmp.GetComponent<Rigidbody>();
                        rb.constraints = RigidbodyConstraints.FreezeAll;
                    }
                    break;
                case "Imp":
                    tmp = Instantiate(ImpPrefab, new Vector3(npc.X, 1.79f, npc.Y), Quaternion.identity);
                    Imp imp = tmp.GetComponent<Imp>();
                    imp.health = npc.Health;
                    imp.walkRange = npc.PatrolRange;
                    imp.sightRange = npc.ChaseRange;
                    imp.attackRange = npc.AttackRange;
                    imp.timeBetweenAttacks = npc.FiringRate;
                    imp.projectile.GetComponent<Fireball>().Damage = npc.ProjectileDamage;
                    if(!npc.CanMove){
                        imp.agent.enabled = false;
                        //add rigidbody constraints for position
                        Rigidbody rb = tmp.GetComponent<Rigidbody>();
                        rb.constraints = RigidbodyConstraints.FreezeAll;
                    }
                    break;
            }
            //TODO: set npc properties
        }
    }

    private void AddMapObjects(){
        GameObject tmp;
        foreach(MapObjectJson obj in mapJson.MapObjects){
            switch(obj.Type){
                case "wallBrick":
                    Instantiate(WallPrefab, new Vector3(obj.X, 1.5f, obj.Y), Quaternion.identity);
                    break;
                case "wallStone":
                    tmp = Instantiate(WallPrefab, new Vector3(obj.X, 1.5f, obj.Y), Quaternion.identity);
                    tmp.GetComponentInChildren<WallTexture>().materialIndex = 1;
                    //change wall texture
                    break;
                case "wallMoss":
                    tmp = Instantiate(WallPrefab, new Vector3(obj.X, 1.5f, obj.Y), Quaternion.identity);
                    tmp.GetComponentInChildren<WallTexture>().materialIndex = 2;
                    //change wall texture
                    break;
                case "tileWall":
                    tmp = Instantiate(WallPrefab, new Vector3(obj.X, 1.5f, obj.Y), Quaternion.identity);
                    tmp.GetComponentInChildren<WallTexture>().materialIndex = 3;
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
                    spawnedDoor = true;
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
