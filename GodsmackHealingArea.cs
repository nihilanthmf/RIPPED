using UnityEngine;

public class GodsmackHealingArea : MonoBehaviour
{
    [SerializeField] PlayerController playerController;
    float healingDelta = 15;

    private void OnTriggerStay(Collider collision)
    {
        if (collision.gameObject.layer == 8)
        {
            playerController.health += healingDelta * Time.deltaTime;
        }
    }
}
