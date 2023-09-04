using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class OptionsScript : MonoBehaviour
{
    //add textMeshPro properies
    [SerializeField]
    TMP_InputField resolutionInputX;
    [SerializeField]
    TMP_InputField resolutionInputY;
    

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

    public static int GetResolutionX(){
        return PlayerPrefs.GetInt("ResolutionX", Screen.currentResolution.width);
    }

    public static int GetResolutionY(){
        return PlayerPrefs.GetInt("ResolutionY", Screen.currentResolution.height);
    }

    void ReadPrefs(){
        GameObject.Find("CRT").GetComponent<UnityEngine.UI.Toggle>().isOn = (PlayerPrefs.GetInt("CRT") == 1);
        GameObject.Find("Dither").GetComponent<UnityEngine.UI.Toggle>().isOn = (PlayerPrefs.GetInt("Dither") == 1);
        resolutionInputX.text = OptionsScript.GetResolutionX().ToString();
        resolutionInputY.text = OptionsScript.GetResolutionY().ToString();
    }


    public void ApplySettings(){
        bool crt = GameObject.Find("CRT").GetComponent<UnityEngine.UI.Toggle>().isOn;
        bool dither = GameObject.Find("Dither").GetComponent<UnityEngine.UI.Toggle>().isOn;
        PlayerPrefs.SetInt("CRT", crt ? 1 : 0);
        PlayerPrefs.SetInt("Dither", dither ? 1 : 0);

        int x;
        int.TryParse(resolutionInputX.text, out x);
        int y;
        int.TryParse(resolutionInputY.text, out y);
        PlayerPrefs.SetInt("ResolutionX", x);
        PlayerPrefs.SetInt("ResolutionY", y);

        PlayerPrefs.Save();
    }
}
