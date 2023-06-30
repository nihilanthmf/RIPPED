using UnityEngine;

public class GameStateManager : MonoBehaviour
{
    [SerializeField] GameObject pauseMenu;

    [SerializeField] PlayerController player;

    bool isPaused;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !player.dead)
        {
            if (!isPaused)
            {
                Pause();
            }
            else
            {
                Resume();
            }
        }
    }

    public void Pause()
    {
        pauseMenu.SetActive(true);
        Cursor.lockState = CursorLockMode.Confined;
        player.gameObject.SetActive(false);

        Time.timeScale = 0;

        isPaused = true;
    }

    public void Resume()
    {
        pauseMenu.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        player.gameObject.SetActive(true);

        Time.timeScale = 1;

        isPaused = false;
    }
}
