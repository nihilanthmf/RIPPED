using UnityEngine;

public class WeaponToPickUp : MonoBehaviour
{
    [SerializeField] WeaponSwitch weaponSwitch;

    public void GetPickedUp()
    {
        if (tag == "Reaper")
        {
            PickingUp(0);
        }
        else if (tag == "Angelus Bane")
        {
            PickingUp(1);
        }
        else if (tag == "Shotgun")
        {
            PickingUp(2);
        }
        else if (tag == "Slap")
        {
            PickingUp(3);
        }
        Destroy(gameObject);
    }

    void PickingUp(int weaponNumber)
    {
        weaponSwitch.ResetingReaperPowerUp();
        weaponSwitch.weapons[weaponNumber].isActivated = true;

        weaponSwitch.weapons[weaponSwitch.currentState].gameObject.SetActive(false);
        weaponSwitch.currentState = weaponNumber;
        weaponSwitch.weapons[weaponNumber].GetPickedUp();
    }
}
