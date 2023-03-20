using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Shoot : MonoBehaviour
{
    [SerializeField]
    Camera FpsCam;

    private Animator animator;
    public AmmoManager ammoManager;
    public AudioSource playerAudioSource;
    public AudioClip RevolverSoundClip;

    public RuntimeAnimatorController newController;
    void Start(){
        animator = gameObject.GetComponent<Animator>();
        newController = (RuntimeAnimatorController)Resources.Load("Animator/Player 1");
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
                ammoManager.ChangeAmmoText(--ammoManager.shotgunAmmo, 2);
                playerAudioSource.Play();
                break;
            case 2:
                if(ammoManager.pistolAmmo == 0)
                    return;    
                Debug.Log("BANG! Shot type: " + shotType);
                animator.Play("ShootingRevolver");
                //Invoke("CastRaySphere", 0.25f); //shoots sphere out after 0.25f (approx. length of the shotgun animation)
                //Invoke("CastRay", 0.25f); //ray = slim, sphere = thick
                Invoke("DoDamagIfShotHits", 0.20f); //invoke after 0.25f (approx. length of the shotgun animation)
                ammoManager.ChangeAmmoText(--ammoManager.pistolAmmo, 1);
                playerAudioSource.PlayOneShot(RevolverSoundClip);
                break;
        }
        ToggleLight();
        Invoke("ToggleLight", lightDelay);
        //change the sound based on the weapon. Use the same thing as changing the level/music.
        //Load from resources/Sounds/<weapon_sound_name>
    }

    private void DoDamagIfShotHits(){
        //maybe add check to see if playing in Keyboard-only mode
        CastRay(); //ray = slim, sphere = thick
        CastRaySphere(); //shoots sphere out
        CastRayToScreen();

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
        if(!animator.GetBool("CanShoot"))
            return;

        if( (Input.GetMouseButtonDown(0) || Input.GetMouseButton(0) 
             || Input.GetKey(KeyCode.LeftControl) || Input.GetKeyDown(KeyCode.LeftControl)) 
             && canShootAgain){

            ShootBullet(currentWeapon);
            canShootAgain = false;
            switch(currentWeapon){
                case 2:
                    Invoke("changeCanShootBool", 0.45f);
                    break;
                case 3:
                    Invoke("changeCanShootBool", 0.70f);
                    break;
            }
            
        }

        if(Input.GetKeyDown(KeyCode.Alpha2)){
            if(currentWeapon != 2){
                animator.Play("ShotgunToRevolver");
                animator.SetBool("CanShoot", false);   
                animator.SetInteger("Weapon",2);
                changeCanShootBool(false);
                Invoke("changeCanShootBool",0.77f);//changes to true
                //Invoke(nameof(ChangeSprite), 0.77f);
            }
            currentWeapon = 2;
            ammoManager.ChangeAmmoText(ammoManager.pistolAmmo, 1);
            ammoManager.currentWeapon = currentWeapon;
        }
        if(Input.GetKeyDown(KeyCode.Alpha3)){
            if(currentWeapon != 3){
                animator.Play("RevolverToShotgun");
                animator.SetBool("CanShoot", false);   
                animator.SetInteger("Weapon",3);
                changeCanShootBool(false);
                Invoke("changeCanShootBool",0.77f);//changes to true
                //Invoke(nameof(ChangeSprite), 0.77f);
            }
            currentWeapon = 3;
            ammoManager.ChangeAmmoText(ammoManager.shotgunAmmo, 2);
            ammoManager.currentWeapon = currentWeapon;
        }
    }
    public void Die(){
        animator.runtimeAnimatorController = newController; 
        canShootAgain = false;
        animator.Play("PlayerDeath");
    }
    public Sprite[] weaponSprites;
    public Image WeaponImage;
    public void ChangeSprite(/*int Sprite*/){
        //WeaponImage.sprite = weaponSprites[Sprite-1];
        //GetComponentsInChildren<Image>()[0].sprite = weaponSprites[Sprite-1];
        GetComponentsInChildren<Image>()[0].sprite = weaponSprites[currentWeapon-1];
        Debug.Log("Sprite Index is now: " + (currentWeapon-1));
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
            case 2:
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
            case 2:
            // Cast a sphere wrapping character controller 64 meters forward
            // to see if it is about to hit anything.
                if (Physics.SphereCast(p1, charCtrl.height / 2.5f, transform.forward, out hit, 64.00f))
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

    void CastRayToScreen(){
        // Create a vector at the center of our camera's viewport
        Vector3 rayOrigin = FpsCam.ViewportToWorldPoint (new Vector3(0.5f, 0.5f, 0.0f));

        // Declare a raycast hit to store information about what our raycast has hit
        RaycastHit hit;

        switch(currentWeapon){
            case 3:
            // Cast a sphere wrapping character controller 64 meters forward
            // to see if it is about to hit anything.
                if (Physics.Raycast (rayOrigin, FpsCam.transform.forward, out hit, 64.00f)) //64 == WeaponRange
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
            case 2:
            // Cast a sphere wrapping character controller 64 meters forward
            // to see if it is about to hit anything.
                if (Physics.Raycast (rayOrigin, FpsCam.transform.forward, out hit, 64.00f)) //64 == WeaponRange
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

    public void changeCanShootBool(bool boolValue){
        canShootAgain = boolValue;
        hitTarget = false;
        target = null;
    }

    [HeaderAttribute("Gun light")]
    public Light gunLight;
    public float lightDelay = 0.5f;
    void ToggleLight(){
        gunLight.enabled = !gunLight.enabled;
    }
}
