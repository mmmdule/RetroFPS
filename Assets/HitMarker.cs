using UnityEngine;
using UnityEngine.UI;

public class HitMarker : MonoBehaviour
{
    [SerializeField]
    Image marker;
    [SerializeField]
    float showTime;

    // Start is called before the first frame update
    public void Show(){
        marker.enabled = true;
        Invoke("Hide", showTime);
    }

    private void Hide(){
        marker.enabled = false;
    }
}
