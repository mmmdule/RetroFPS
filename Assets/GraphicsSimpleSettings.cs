using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class GraphicsSimpleSettings : MonoBehaviour
{
    
    [HeaderAttribute("Resolution Settings")]
    public int resX = 1280;
    public int resY = 720;
    
    [HeaderAttribute("Target Framerate")]
    public int framerate = 60;

    private string ConfigFilePath = "settings.ini";
    
    [HeaderAttribute("Post-processing Settings")]
    public Assets.Scripts.Cam.Effects.Dither dither;
    public CRTPostProcess crtEffect;
    void Start()
    {
        setResolutionAndFps();
        setPostProcessing();
        setFov();
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
        resX = OptionsScript.GetResolutionX();
        resY = OptionsScript.GetResolutionY();
        Screen.SetResolution(resX, resY, true);
    }

    void setFov(){
        Camera.main.fieldOfView = OptionsScript.GetFov();
    }
}
