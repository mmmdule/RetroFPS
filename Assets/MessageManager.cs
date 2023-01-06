using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MessageManager : MonoBehaviour
{
    private Text MessageText;
    public GameObject player;
    // Start is called before the first frame update
    void Start()
    {
        
    }
    public void MessageUpdate(string Text, float delay){
        StartCoroutine(ShowMessage(Text, delay));
    }


    private IEnumerator ShowMessage(string Text, float delay){
        MessageText = player.GetComponentsInChildren<Text>()[1]; //[0] je Ammo Text
        MessageText.text = Text;
        yield return new WaitForSeconds(delay);
        MessageText.text = "";
    }
}
