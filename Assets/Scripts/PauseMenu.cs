using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseMenuUI;
    public InputActionReference pauseAction;

    private bool GameIsPaused = false;

    public void OnEnable()
    {
        pauseAction.action.performed += OnPause;
        pauseAction.action.Enable();
    }

    public void OnDisable()
    {
        pauseAction.action.performed -= OnPause;
        pauseAction.action.Disable();
    }

    public void OnPause(InputAction.CallbackContext context)
    {
        if (GameIsPaused)
            Resume();
        else
            Pause();
    }

    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        GameIsPaused = false;
    }

    public void Pause()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        GameIsPaused = true;
    }

    public void MainMenu()
    {
        SceneManager.LoadScene("Menu");
        Time.timeScale = 1f;
        GameIsPaused = false;
    }

    public void QuitGame()
    {
        Debug.Log("Quit");
        Application.Quit();
    }

}
