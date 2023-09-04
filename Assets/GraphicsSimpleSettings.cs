using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class GraphicsSimpleSettings : MonoBehaviour
{
    
    [HeaderAttribute("Resolution Settings")]
    public int resX = 1280;
    public int resY = 720;
    
    [HeaderAttribute("Targer Framerate")]
    public int framerate = 60;
    private string ConfigFilePath = "settings.ini";
    
    [HeaderAttribute("Post-processing Settings")]
    public Assets.Scripts.Cam.Effects.Dither dither;
    public CRTPostProcess crtEffect;
    void Start()
    {
        setResolutionAndFps();
        setPostProcessing();
    }

    void setPostProcessing(){
        crtEffect.enabled = OptionsScript.GetCRT() == 1 ? true : false;
        dither.enabled = OptionsScript.GetDither() == 1 ? true : false;
    }

    void setResolutionAndFps(){
        string settingsFilePath = Application.streamingAssetsPath + "/" + ConfigFilePath;
        if(File.Exists(settingsFilePath)){
            StreamReader sr = new StreamReader(settingsFilePath);
            resX = int.Parse(sr.ReadLine());
            resY = int.Parse(sr.ReadLine());
            Application.targetFrameRate = int.Parse(sr.ReadLine());
            sr.Close();
        }
        Screen.SetResolution(resX, resY, true);//Screen.SetResolution(1280, 720, true);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
