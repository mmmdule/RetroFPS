using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shoot : MonoBehaviour
{
    private Animator animator;
    public AmmoManager ammoManager;
    public AudioSource playerAudioSource;

    void Start(){
        animator = gameObject.GetComponent<Animator>();
    }

    public void ShootBullet(int shotType){
        switch(shotType){
            case 3:
                if(ammoManager.shotgunAmmo == 0)
                    return;    
                Debug.Log("BANG! Shot type: " + shotType);
                animator.Play("ShootingShotgun");
                //Invoke("CastRaySphere", 0.25f); //shoots sphere out after 0.25f (approx. length of the shotgun animation)
                //Invoke("CastRay", 0.25f); //ray = slim, sphere = thick
                Invoke("DoDamagIfShotHits", 0.25f); //invoke after 0.25f (approx. length of the shotgun animation)
                ammoManager.ChangeAmmoText(--ammoManager.shotgunAmmo);
                break;
        }
        //change the sound based on the weapon. Use the same thing as changing the level/music.
        //Load from resources/Sounds/<weapon_sound_name>
        playerAudioSource.Play();
    }

    private void DoDamagIfShotHits(){
        CastRay(); //ray = slim, sphere = thick
        CastRaySphere(); //shoots sphere out
        if(!hitTarget || target == null)
            return;

        if(target.tag == "Imp"){
           target.GetComponent<Imp>().TakeDamage(weaponDamageVals[currentWeapon]);
        }
        else if(target.tag  == "Tri-Imp"){
            target.GetComponent<TriImp>().TakeDamage(weaponDamageVals[currentWeapon]);
        }
    }

    private bool canShootAgain = true;
    public CharacterController charCtrl;
    public int currentWeapon = 3; //3 - shotgun, 2 - pistol
    void Update(){
        if( (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.LeftControl)) && canShootAgain){
            ShootBullet(currentWeapon);
            canShootAgain = false;
            Invoke("changeCanShootBool", 0.70f);
            
        }
    }
    
    //indexes match currentWeapon values
    int[] weaponDamageVals = {0, 10, 30, 70}; //0 - skipped, 10 - melee(?), 30 - pistol, 70 - shotgun
    private GameObject target;
    private bool hitTarget = false;
    void CastRay(){
         RaycastHit hit;
        //Stara raycast metoda (slim ray)
        //Bit shift the index of the layer (7) to get a bit mask
        //int layerMask = 1 << 7;
        //This would cast rays only against colliders in layer 7.
        switch(currentWeapon){
            case 3:
            if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, 64.00f)) {
                Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * hit.distance, Color.yellow);
                if(hit.transform.gameObject.tag == "Imp"){
                    //hit.transform.gameObject.GetComponent<Imp>().TakeDamage(weaponDamageVals[currentWeapon]);
                    target = hit.transform.gameObject;
                    hitTarget = true;
                    Debug.Log("Hit Imp.");
                }
                else if(hit.transform.gameObject.tag == "Tri-Imp"){
                    //hit.transform.gameObject.GetComponent<TriImp>().TakeDamage(weaponDamageVals[currentWeapon]);
                    target = hit.transform.gameObject;
                    hitTarget = true;
                    Debug.Log("Hit Tri-Imp.");
                }
            }
            break;
        }
    }

    void CastRaySphere(){
        RaycastHit hit;
        Vector3 p1 = transform.position + charCtrl.center;
        switch(currentWeapon){
            case 3:
            // Cast a sphere wrapping character controller 64 meters forward
            // to see if it is about to hit anything.
            if (Physics.SphereCast(p1, charCtrl.height / 2, transform.forward, out hit, 64.00f))
            {
                if(hit.transform.gameObject.tag == "Imp"){
                    //hit.transform.gameObject.GetComponent<Imp>().TakeDamage(weaponDamageVals[currentWeapon]);
                    hitTarget = true;
                    Debug.Log("Hit Imp.");
                }
                else if(hit.transform.gameObject.tag == "Tri-Imp"){
                    //hit.transform.gameObject.GetComponent<TriImp>().TakeDamage(weaponDamageVals[currentWeapon]);
                    hitTarget = true;
                    Debug.Log("Hit Tri-Imp.");
                }
            }
            break;
        }
    }

    void OnDrawGizmos()
    {
        //Gizmos.DrawSphere(transform.position + charCtrl.center, 64.00f);
    }
    void changeCanShootBool(){
        canShootAgain = true;
        hitTarget = false;
        target = null;
    }
}
