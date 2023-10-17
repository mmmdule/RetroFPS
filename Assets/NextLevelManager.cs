using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

public class NextLevelManager //: MonoBehaviour
{
    public static int NextSegment(){
        string[] lepFiles = Directory.GetFiles(Application.streamingAssetsPath, "*.lep");
        ProjectJson project = JsonUtility.FromJson<ProjectJson>(File.ReadAllText(lepFiles[0]));
        int index = Array.FindIndex(project.MapNameList, x => x == PlayerPrefs.GetString("LevelToLoad").Replace(".lem", ""));

        if(index + 1 >= project.MapNameList.Length){
            //PlayerPrefs.SetString("LevelToLoad", null);
            return 0; //ending
        }
        else{
            string nextMapName = project.MapNameList[index + 1];
            PlayerPrefs.SetString("LevelToLoad", nextMapName + ".lem");

            if(MainMenu.ReadMap(nextMapName).StoryTextSegment)
                return 1; //story segment
            else
                return 2; //level segment
        }
        
        return -1; //something went wrong
    }

    public static void SetGameOverPref(){
        PlayerPrefs.SetString("GameOverLevel", PlayerPrefs.GetString("LevelToLoad"));
    }
}
