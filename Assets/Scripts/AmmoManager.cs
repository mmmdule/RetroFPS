using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AmmoManager : MonoBehaviour
{
    public Text AmmoText;

    public int shotgunAmmo;
    public int pistolAmmo;
    public int currentWeapon;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ClearText(){
        AmmoText.text = "";
    }
    string[] weaponMark = {"", "   I", "   III"};
    public void ChangeAmmoText(int value, int weaponNum){

        AmmoText.text = value.ToString() + weaponMark[weaponNum];
    }

    public void ChangeAmmoText(int value, int weaponNum, bool pickup){
        if(currentWeapon == weaponNum + 1)
            AmmoText.text = value.ToString() + weaponMark[weaponNum];
    }

    public int RevolverLimit = 100, ShotgunLimit = 50;
    public void IncreaseAmmo(int value, int weaponNum){
        
        switch(weaponNum){
            case 2:
                if(pistolAmmo + value > RevolverLimit)
                    pistolAmmo = RevolverLimit;
                else
                    pistolAmmo += value;
                ChangeAmmoText(pistolAmmo, weaponNum - 1, true);
                break;
            case 3:
                if(shotgunAmmo + value > ShotgunLimit)
                    shotgunAmmo = ShotgunLimit;
                else
                    shotgunAmmo += value;
                ChangeAmmoText(shotgunAmmo, weaponNum - 1, true);
                break;
        }
    }
}
