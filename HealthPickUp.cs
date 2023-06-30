using UnityEngine;

public class HealthPickUp : MonoBehaviour
{
    [SerializeField] float value;

    [SerializeField] PlayerController playerController;

    ParticleSystem particlesOnDisappearing;

    private void Start()
    {
        particlesOnDisappearing = transform.GetChild(0).GetComponent<ParticleSystem>();
    }

    void PickUp()
    {
        playerController.health += value;
    }

    void Disappearing()
    {
        particlesOnDisappearing.gameObject.SetActive(true);
        particlesOnDisappearing.transform.SetParent(null);
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 8 && playerController.health < 100)
        {
            PickUp();
            value = 0; // not to increase health multiple times
            Disappearing();
        }
    }
}
