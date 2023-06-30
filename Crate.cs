using UnityEngine;
using System.Collections.Generic;

public class Crate : MonoBehaviour
{
    [SerializeField] ParticleSystem onDestroyParticles; // disabled
    [SerializeField] Rigidbody[] piecesOfBrokenVersion;
    Vector3 startParticleScale;
    float force = 50f;

    bool wasDestroyedByGodsmack;

    [SerializeField] PlayerController playerController;

    [SerializeField] HealthPickUp healthPickUp;
    [SerializeField] AmmoPickUp rifleAmmoPickUp, shotgunAmmoPickUp;

    Transform mainCamera;

    [SerializeField] Crates crate;
    enum Crates { HealthCrate, AmmoCrate, None };

    private void Start()
    {
        onDestroyParticles.GetComponent<ParticleSystemRenderer>().material = GetComponent<Renderer>().material;
        startParticleScale = onDestroyParticles.transform.localScale;
        mainCamera = Camera.main.transform;
    }

    public void OnHit()
    {
        onDestroyParticles.transform.parent = null;
        onDestroyParticles.transform.localScale = startParticleScale;
        //onDestroyParticles.Play();

        foreach (var piece in piecesOfBrokenVersion)
        {
            piece.gameObject.SetActive(true);
            piece.transform.parent = null;
            piece.transform.localScale *= 0.9f;
            if (wasDestroyedByGodsmack)
            {
                piece.AddForce(new Vector3(mainCamera.forward.x + Random.Range(-0.6f, 0.6f), Random.Range(0.5f, 1f), mainCamera.forward.z + Random.Range(-0.6f, 0.6f)) * force * 0.8f, ForceMode.Impulse);
            }
            else
            {
                piece.AddExplosionForce(force, transform.position + Vector3.down * 0.2f, 1f, 0.8f, ForceMode.Impulse);
            }
            //piece.AddForce(new Vector3(transform.position.x - piece.transform.position.x, 1, transform.position.z - piece.transform.position.z) * force);
            Destroy(piece, 10);
        }

        if (crate == Crates.HealthCrate)
        {
            Instantiate(healthPickUp, transform.position, Quaternion.identity);
        }
        else if (crate == Crates.AmmoCrate)
        {
            FindingMinAmmoWeapon();
        }

        Destroy(gameObject, 0.05f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Godsmack Kill Area")
        {
            wasDestroyedByGodsmack = true;
        }
    }

    void FindingMinAmmoWeapon()
    {
        // Making a list of the unlocked weapon ammos so that it wont give an ammo pickup of a locked weapon
        List<float> unlockedWeaponAmmos = new List<float>();

        if (playerController.rifle.weaponDefault.isActivated)
        {
            unlockedWeaponAmmos.Add(playerController.rifle.ammo);
        }
        if (playerController.shotgun.weaponDefault.isActivated)
        {
            unlockedWeaponAmmos.Add(playerController.shotgun.ammo);
        }

        // Finding the lowest ammo of all unlocked weapons 
        unlockedWeaponAmmos.Sort();
        float minAmmoNumber = 0;
        bool doesntHaveWeapons = false;
        if (unlockedWeaponAmmos.Count != 0)
        {
            minAmmoNumber = unlockedWeaponAmmos[0];
        }
        else
        {
            doesntHaveWeapons = true;
        }

        if (minAmmoNumber == playerController.rifle.ammo || doesntHaveWeapons) // if rifle has lowest ammo or player doesnt have any weapons unlocked
        {
            Instantiate(rifleAmmoPickUp, transform.position, Quaternion.identity);

            //float secondMinAmmoNumber = Mathf.Min(playerController.shotgun.ammo, playerController.rifle.ammo + rifleAmmoPickUp.value); // etc.
            // ^^^ finding the 2nd lowest ammo including the rifle ammo increase from the 1st pickup

            //FindingSecondMinAmmoWeapon(secondMinAmmoNumber);
        }
        else if (minAmmoNumber == playerController.shotgun.ammo) // if shotgun has lowest ammo 
        {
            Instantiate(shotgunAmmoPickUp, transform.position, Quaternion.identity);

            //float secondMinAmmoNumber = Mathf.Min(playerController.shotgun.ammo + shotgunAmmoPickUp.value, playerController.rifle.ammo); // etc.
            // ^^^ finding the 2nd lowest ammo including shotgun ammo the increase from the 1st pickup

            //FindingSecondMinAmmoWeapon(secondMinAmmoNumber);
        }
    }

    void FindingSecondMinAmmoWeapon(float secondMinAmmoNumber)
    {
        if (secondMinAmmoNumber == playerController.shotgun.ammo)
        {
            Instantiate(shotgunAmmoPickUp, transform.position, Quaternion.identity);
        }
        else if (secondMinAmmoNumber == playerController.rifle.ammo)
        {
            Instantiate(rifleAmmoPickUp, transform.position, Quaternion.identity);
        }

        // checks if playerController.XXX.ammo + XXXAmmoPickUp.value is still less then the others (one weapon need both the pickups)
        else if (secondMinAmmoNumber == playerController.shotgun.ammo + playerController.rifle.ammo)
        {
            Instantiate(shotgunAmmoPickUp, transform.position, Quaternion.identity);
        }
        else if (secondMinAmmoNumber == playerController.rifle.ammo + rifleAmmoPickUp.value)
        {
            Instantiate(rifleAmmoPickUp, transform.position, Quaternion.identity);
        }
    }
}
