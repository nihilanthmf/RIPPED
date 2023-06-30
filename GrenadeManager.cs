using UnityEngine;
using System.Collections;

public class GrenadeManager : MonoBehaviour
{
    [SerializeField] GameObject grenadeSample;

    Transform mainCamera;
    GameObject currentGrenadeInstance;
    Rigidbody grenadeRB;

    Vector3 throwDirecion;

    [SerializeField] float forceMultiplier = 20;
    const float explosionTime = 3;
    const float throwCoolDownTime = 1;
    float timeToThrowGrenade;

    float startTime;
    float timeButtonWasPressed;

    bool toApplyForce;

    private void Start()
    {
        mainCamera = Camera.main.transform;
    }

    private void Update()
    {
        throwDirecion = mainCamera.forward + Vector3.up / 6;

        if (Input.GetKeyDown(KeyCode.G))
        {
            startTime = Time.time;
        }

        if (Input.GetKeyUp(KeyCode.G) && Time.time >= timeToThrowGrenade)
        {
            currentGrenadeInstance = Instantiate(grenadeSample, transform.position, Quaternion.identity);
            grenadeRB = currentGrenadeInstance.GetComponent<Rigidbody>();

            timeToThrowGrenade = Time.time + throwCoolDownTime;

            timeButtonWasPressed = Time.time - startTime;
            timeButtonWasPressed = Mathf.Clamp(timeButtonWasPressed, 0, 3);

            grenadeRB.isKinematic = false;
            toApplyForce = true;
        }
    }

    private void FixedUpdate()
    {
        if (toApplyForce)
        {
            grenadeRB.AddForce(throwDirecion * timeButtonWasPressed * forceMultiplier, ForceMode.Impulse);
            StartCoroutine(Explosion(currentGrenadeInstance.GetComponent<Grenade>()));

            toApplyForce = false;
        }
    }

    IEnumerator Explosion(Grenade grenade)
    {
        yield return new WaitForSeconds(explosionTime);
        if (grenade != null)
        {
            grenade.BlowUp();
        }
        //grenade.BlowUp();
        //grenade.GetComponent<Grenade>().explosionColldier.enabled = true;
        //yield return new WaitForSeconds(0.1f);
        //Destroy(grenade.gameObject);
    }
}
