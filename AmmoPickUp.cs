using UnityEngine;

public class AmmoPickUp : MonoBehaviour
{
    public int value;

    [SerializeField] Weapons weapon;

    enum Weapons { rifle, shotgun };

    [SerializeField] Shotgun shotgun;
    [SerializeField] Rifle rifle;

    ParticleSystem particlesOnDisappearing;

    private void Start()
    {
        particlesOnDisappearing = transform.GetChild(0).GetComponent<ParticleSystem>();
    }

    public void PickUp()
    {
        if (weapon == Weapons.rifle)
        {
            rifle.ammo += value;
        }
        else if (weapon == Weapons.shotgun)
        {
            shotgun.ammo += value;
        }
    }

    void Disappearing()
    {
        particlesOnDisappearing.gameObject.SetActive(true);
        particlesOnDisappearing.transform.SetParent(null);
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 8)
        {
            if (weapon == Weapons.rifle)
            {
                if (rifle.ammo < rifle.maxAmmo)
                {
                    PickUp();
                    value = 0; // not to increase ammo multiple times
                    Disappearing();
                }
            }
            else if (weapon == Weapons.shotgun)
            {
                if (shotgun.ammo < shotgun.maxAmmo)
                {
                    PickUp();
                    value = 0; // not to increase ammo multiple times
                    Disappearing();
                }
            }
        }
    }
}
