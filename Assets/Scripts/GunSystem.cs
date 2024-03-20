using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using TMPro;


//Gun System inspired from DaveGameDev

public class GunSystem :  NetworkBehaviour
{
    //Gun stats
    public int damage;
    public float timeBetweenShooting, spread, range, reloadTime, timeBetweenShots;
    public int magazineSize, bulletsPerTap;
    public bool allowButtonHold;
    int bulletsLeft, bulletsShot;

    //bools
    bool shooting, readyToShoot, reloading;

    //Reference
    public Camera fpsCam;
    public Transform attackPoint;
    public RaycastHit rayHit;
    public LayerMask whatIsEnemy;

    //Gun sounds
    public AudioClip shootSound;
    public AudioClip reloadSound; // reload sound 
    private AudioSource audioSource;


    // Graphics for gun
    public GameObject muzzleFlash, bulletHoleGraphic;

    // public CamShake camShake;
    // public float camShakeMagnitude, camShakeDuration;

    public TextMeshProUGUI text;

    // Provide a feature for starting magazine size and ability to shoot and audio for shooting and reloading
    private void Awake() {
        bulletsLeft = magazineSize;
        readyToShoot = true;

        audioSource = GetComponent<AudioSource>(); // Ensure an AudioSource component is attached to the same GameObject
    }

    // Update the total amount of bullets left 
    private void Update() 
    {
        MyInput();

        // Set the text for how much ammo you have
        text.SetText(bulletsLeft + " / " + magazineSize);
    }

    // Input for handling mouse and shooting buttons 
    private void MyInput() {
        if (allowButtonHold) shooting = Input.GetKey(KeyCode.Mouse0);
        else shooting = Input.GetKeyDown(KeyCode.Mouse0);

        if (Input.GetKeyDown(KeyCode.R) && bulletsLeft < magazineSize && !reloading) Reload();

        //shooting
        if (readyToShoot && shooting && !reloading && bulletsLeft > 0) {
            Shoot();
        }

    }

    //reload function
    private void Shoot()
    {
        readyToShoot = false;

        // Play the shooting sound
        if (audioSource != null && shootSound != null)
        {
            Debug.Log("Playing shooting sound");
            audioSource.PlayOneShot(shootSound);
        }
        else
        {
            Debug.Log("AudioSource or shootSound is not set");
        }
        
        // Spread 
        float x = Random.Range(-spread, spread);
        float y = Random.Range(-spread, spread);

        //Calulate direction with the spread
        Vector3 direction = fpsCam.transform.forward + new Vector3(x, y, 0);

        // Ray cast for the bullets ad shooting
        if (Physics.Raycast(fpsCam.transform.position, direction, out rayHit, range, whatIsEnemy))
        {
            // Check if we are getting hits 
            Debug.Log(rayHit.collider.name);

            // Check if the hit is an enemy 
            if (rayHit.collider.CompareTag("Enemy"))
            {
                var enemyHealth = rayHit.collider.GetComponent<Health>();
                if(enemyHealth != null)
                {
                    // use server rpc to provide the damage for the enemies 
                    enemyHealth.TakeDamageServerRpc(damage);
                }
            }
        }

         //ShakeCamera
         //camShake.Shake(camShakeDuration, camShakeMagnitude);

        //Graphics of the muzzle flash and the bullet hole 
        Instantiate(bulletHoleGraphic, rayHit.point, Quaternion.Euler(0, 180, 0));
        Instantiate(muzzleFlash, attackPoint.position, Quaternion.identity);

        // When we shoot decrease total bullets and bullets in magazine 
        bulletsLeft--;
        bulletsShot--;
        Invoke("ResetShot", timeBetweenShooting);

        if (bulletsShot > 0 && bulletsLeft > 0) 
            Invoke("Shoot", timeBetweenShots);
    }

    //reset the shot 
    private void ResetShot()
    {
        readyToShoot = true;
    }

    //reload weapon
    private void Reload() {
        reloading = true;

         // Play the reload sound
        if (audioSource != null && reloadSound != null) {
            audioSource.PlayOneShot(reloadSound);
        } else {
            Debug.Log("AudioSource or reloadSound is not set");
        }
        
        Invoke("ReloadFinished", reloadTime);
    }

    //function to check if reloading is complete
    private void ReloadFinished() {
        bulletsLeft = magazineSize;
        reloading = false;
    }

}
