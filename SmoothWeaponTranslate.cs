using UnityEngine;

public class SmoothWeaponTranslate : MonoBehaviour
{
    // This script must be on camera that renders only guns. The actual gun must not be its child!

    float speed = 0.04f;
    float lerpTime = 5;
    Vector3 startPosition;

    private void Start()
    {
        startPosition = transform.localPosition;
    } 

    private void Update()
    {
        float xMovement = Input.GetAxis("Mouse X") * speed;
        float yMovement = Input.GetAxis("Mouse Y") * speed;

        float horizontal = Input.GetAxis("Horizontal") * speed * 2;
        float vertical = Input.GetAxis("Vertical") * speed * 2;

        //Smooth gun translating when you move the camera
        Vector3 finalPosition = new Vector3(xMovement + horizontal, yMovement, vertical);
        transform.localPosition = Vector3.Lerp(transform.localPosition, finalPosition + startPosition, lerpTime * Time.deltaTime);
    }
}
