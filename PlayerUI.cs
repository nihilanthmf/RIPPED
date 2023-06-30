using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class PlayerUI : MonoBehaviour
{
    [SerializeField] Slider healthBar;
    [SerializeField] TMP_Text healthText;
    [SerializeField] Image bloodyScreen;
    public TMP_Text ammoText;

    [SerializeField] PlayerController playerController;

    float startHealth; // To divide the current health value by this to make it 0 - 1 for slider's value
    float deltaBloodFadeAway;

    public bool hasDied;


    public void TakeDamageEffect()
    {
        deltaBloodFadeAway = 1;
    }

    private void Start()
    {
        startHealth = playerController.health;
    }

    private void Update()
    {
        healthBar.value = playerController.health / startHealth;
        healthText.text = Mathf.Round(playerController.health).ToString();


        deltaBloodFadeAway -= 3f * Time.deltaTime;
        deltaBloodFadeAway = Mathf.Clamp01(deltaBloodFadeAway);

        if (hasDied)
        {
            healthBar.gameObject.SetActive(false);
            healthText.gameObject.SetActive(false);
            ammoText.gameObject.SetActive(false);
            bloodyScreen.color = new Color(1, 1, 1, 1);
        }
        else
        {
            bloodyScreen.color = new Color(bloodyScreen.color.r, bloodyScreen.color.g, bloodyScreen.color.b, deltaBloodFadeAway);
        }
    }   

    public void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
} 
