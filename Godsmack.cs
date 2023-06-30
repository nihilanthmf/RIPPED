using UnityEngine;
using System.Collections;

public class Godsmack : MonoBehaviour
{
    Animator animator;
    Transform mainCamera;

    CameraController cameraController;

    [SerializeField] WeaponSwitch weaponSwitch;
    [SerializeField] PlayerController playerController;
    [SerializeField] GameObject killingRange;
    [SerializeField] GameObject healingArea;

    public float damage = 5000;
    float healingLifetime = 5;

    [SerializeField] LayerMask enemyLayer;

    private void Start()
    {
        animator = GetComponent<Animator>();
        mainCamera = Camera.main.transform;
        cameraController = mainCamera.GetComponent<CameraController>();
    }

    private void Update()
    {
        if (Input.GetMouseButton(0) && !animator.GetBool("ToSlap") && !animator.GetBool("ToHeal"))
        {
            animator.SetBool("ToSlap", true);
        }
        if (Input.GetMouseButton(1) && !animator.GetBool("ToSlap") && !animator.GetBool("ToHeal") && playerController.health < 100)
        {
            animator.SetBool("ToHeal", true);
        }
    }

    void Slapping() // gets called via animation event
    {
        killingRange.SetActive(true);

        StartCoroutine(cameraController.Shaking(.1f, 1, null));
        StartCoroutine(WaitToDisableKillingRange());
    }

    IEnumerator WaitToDisableKillingRange()
    {
        yield return new WaitForSeconds(0.1f);
        killingRange.SetActive(false);
    }

    void Heal() // gets called via animation event
    {
        RaycastHit ground;
        if (Physics.Raycast(transform.position, -transform.up, out ground, 1000, LayerMask.GetMask("Static")))
        {
            GameObject healingAreaInstance = Instantiate(healingArea, new Vector3(transform.position.x, ground.transform.position.y + 0.01f, transform.position.z), healingArea.transform.rotation);
            Destroy(healingAreaInstance, healingLifetime);
        }
    }

    void StopSlappingAnimation() // Called after slapping animation via animation event
    {
        animator.SetBool("ToSlap", false);
    }

    void StopHealingAnimation() // Called after slapping animation via animation event
    {
        animator.SetBool("ToHeal", false);
    }
}
