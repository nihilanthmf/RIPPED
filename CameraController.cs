using UnityEngine;
using System.Collections;
using System;

public class CameraController : MonoBehaviour
{
    [SerializeField] GameObject player;
    Animator cameraAnimator;
    Animator playerAnimator;
    Camera cam;

    public float MouseSensivity { get; set; } = 0.9f; // 0.9f
    public float xRotation { get; set; }

    float x;
    float y;

    float startCameraFieldOfView;
    public bool toResetFieldOfView { get; set; }

    Action movingCameraAction;

    Vector3 originalPosition;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        movingCameraAction = MovingCamera;
        cameraAnimator = transform.parent.GetComponent<Animator>();
        originalPosition = transform.localPosition;
        playerAnimator = player.GetComponent<Animator>();
        cam = GetComponent<Camera>();
        startCameraFieldOfView = cam.fieldOfView;
    }

    void ResetingFieldOfView()
    {
        cam.fieldOfView = Mathf.MoveTowards(cam.fieldOfView, startCameraFieldOfView, 250 * Time.deltaTime);
        cam.fieldOfView = Mathf.Clamp(cam.fieldOfView, 0f, startCameraFieldOfView);
        if (cam.fieldOfView == startCameraFieldOfView)
        {
            toResetFieldOfView = false;
        }
    }

    private void Update()
    {
        if (toResetFieldOfView)
        {
            ResetingFieldOfView();
        }
    }

    private void LateUpdate()
    {
        x = Input.GetAxis("Mouse X") * MouseSensivity;
        y = Input.GetAxis("Mouse Y") * MouseSensivity;

        xRotation = Mathf.Clamp(xRotation, -90, 80);

        CameraSwayRightLeft();

        movingCameraAction.Invoke();
    }


    // This method shakes the camera with you shoot
    public IEnumerator Shaking(float magnitude, float elapsed, GameObject destroyAfterShake)
    {
        playerAnimator.enabled = false;
        movingCameraAction = DelegateFix;

        for (float t = 0; t < elapsed; t += .1f)
        {
            float x = UnityEngine.Random.Range(-1f, 1f) * magnitude;
            float y = UnityEngine.Random.Range(-1f, 1f) * magnitude;

            transform.localPosition = new Vector3(x, y, originalPosition.z) + new Vector3(0, originalPosition.y, 0);

            yield return null;
        }

        Destroy(destroyAfterShake);

        transform.localPosition = originalPosition;
        playerAnimator.enabled = true;
        movingCameraAction = MovingCamera;
    }

    void CameraSwayRightLeft()
    {
        cameraAnimator.SetFloat("Leaning", Input.GetAxisRaw("Horizontal"));
    }


    void MovingCamera()
    {
        player.transform.Rotate(new Vector3(0, x, 0)); //player.transform.Rotate(new Vector3(0, x, 0));
        transform.localRotation = Quaternion.Euler(xRotation, transform.localRotation.y, transform.localRotation.z);


        float toSubtract = y; // + xDelta
        xRotation -= toSubtract;
    }

    // This is a method that you put in Action variable in order not to move camera during shaking process
    void DelegateFix() { }
}
