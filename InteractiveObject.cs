using UnityEngine;
using System;

public class InteractiveObject : MonoBehaviour
{
    Action methodToExecute;
    Door door;
    Button button;

    private void Start()
    {
        if (GetComponent<Door>() != null)
        {
            methodToExecute = OpenDoor;
            door = GetComponent<Door>();
        }
        if (GetComponent<Button>() != null)
        {
            methodToExecute = ButtonPress;
            button = GetComponent<Button>();
        }
    }

    public void PerformAction() { methodToExecute(); }

    void OpenDoor() { door.OpeningClosing(); }

    void ButtonPress() { button.Press(); }
}
