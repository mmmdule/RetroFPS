using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptionsScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        //get values and set toggles and other inputs
        ReadPrefs();
    }

    public static int GetCRT(){
        return PlayerPrefs.GetInt("CRT", 0);
    }

    public static int GetDither(){
        return PlayerPrefs.GetInt("Dither", 0);
    }

    void ReadPrefs(){
        GameObject.Find("CRT").GetComponent<UnityEngine.UI.Toggle>().isOn = (PlayerPrefs.GetInt("CRT") == 1);
        GameObject.Find("Dither").GetComponent<UnityEngine.UI.Toggle>().isOn = (PlayerPrefs.GetInt("Dither") == 1);
    }


    public void ApplySettings(){
        bool crt = GameObject.Find("CRT").GetComponent<UnityEngine.UI.Toggle>().isOn;
        bool dither = GameObject.Find("Dither").GetComponent<UnityEngine.UI.Toggle>().isOn;
        PlayerPrefs.SetInt("CRT", crt ? 1 : 0);
        PlayerPrefs.SetInt("Dither", dither ? 1 : 0);
        PlayerPrefs.Save();
    }
}
