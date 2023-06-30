using UnityEngine;

public class WeaponDefault : MonoBehaviour
{
    Animator animator;

    [HideInInspector] public bool isActivated;
    Shotgun shotgun;
    Rifle rifle;

    private void Start()
    {
        animator = GetComponent<Animator>();

        if (GetComponent<Rifle>())
        {
            rifle = GetComponent<Rifle>();
        }
        else if (GetComponent<Shotgun>())
        {
            shotgun = GetComponent<Shotgun>();
        }
    }

    public void Hide()
    {
        animator.Play("Hide");
    }

    public void UnHide()
    {
        animator.Play("UnHide");
    }

    public void GetPickedUp()
    {
        if (shotgun)
        {
            shotgun.ammo = shotgun.maxAmmo;
        }
        else if (rifle)
        {
            rifle.ammo = rifle.maxAmmo;
        }
    }
}
