using UnityEngine;
using System;

public class WeaponSwitch : MonoBehaviour
{
    [HideInInspector] public int currentState;

    public WeaponDefault[] weapons;

    // Reaper
    [SerializeField] Reaper reaper;
    CameraController mainCamera;

    private void Start()
    {
        mainCamera = Camera.main.GetComponent<CameraController>();
    }

    private void Update()
    {
        if (weapons[0].isActivated)
        {
            if (Input.mouseScrollDelta.y != 0 && 
                    currentState + Convert.ToInt32(Input.mouseScrollDelta.y) >= 0 &&
                    currentState + Convert.ToInt32(Input.mouseScrollDelta.y) <= weapons.Length - 1)
            {
                weapons[currentState].gameObject.SetActive(false);

                // if the reaper is powered up then we reset powerup by shooting it before switching weapon
                if (currentState == 0)
                {
                    ResetingReaperPowerUp();
                }

                if (Convert.ToInt32(Input.mouseScrollDelta.y) > 0) // to jump over weapons if they arent active but those after them are
                {
                    for (int i = currentState + 1; i < weapons.Length; i++)
                    {
                        if (weapons[i].isActivated)
                        {
                            currentState = i;
                            break;
                        }
                    }
                }
                else
                {
                    for (int i = currentState - 1; i >= 0; i--)
                    {
                        if (weapons[i].isActivated)
                        {
                            currentState = i;
                            break;
                        }
                    }
                }
            }

            ChangingByKeyboard();

            currentState = Mathf.Clamp(currentState, 0, weapons.Length - 1);

            weapons[currentState].gameObject.SetActive(true);
        }
    }

    void ChangingByKeyboard()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1) && weapons[0].isActivated) // Reaper
        {
            // if the reaper is powered up then we reset powerup by shooting it before switching weapon
            if (reaper.powerUp != 1)
            {
                reaper.Shoot();
            }
            weapons[currentState].gameObject.SetActive(false);
            currentState = 0;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2) && weapons[1].isActivated)
        {
            weapons[currentState].gameObject.SetActive(false);
            currentState = 1;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3) && weapons[2].isActivated)
        {
            weapons[currentState].gameObject.SetActive(false);
            currentState = 2;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4) && weapons[3].isActivated)
        {
            weapons[currentState].gameObject.SetActive(false);
            currentState = 3;
        }
    }

    public void ResetingReaperPowerUp()
    {
        if (reaper.powerUp != 1)
        {
            reaper.Shoot();
        }
    }
}