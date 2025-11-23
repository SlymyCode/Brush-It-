using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject optionsMenu;
    [SerializeField] private GameObject playerUI;
    [SerializeField] private GameObject timer;
    [SerializeField] private Key key = Key.Escape;
    private bool showMenu = false;
    
    void Start()
    {
        pauseMenu.SetActive(showMenu);
    }
    
    void Update()
    {
        Pause();
    }

    public void Pause()
    {
        if (Keyboard.current[key].wasPressedThisFrame && !showMenu)
        {
            pauseMenu.SetActive(true);
            playerUI.SetActive(false);
            timer.SetActive(false);
            showMenu = true;
            Time.timeScale = 0;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

        }
        else if (Keyboard.current[key].wasPressedThisFrame && showMenu) 
        {
            pauseMenu.SetActive(false);
            playerUI.SetActive(true);
            timer.SetActive(true);
            showMenu = false;
            Time.timeScale = 1;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    public void Unpause()
    {
        pauseMenu.SetActive(false);
        playerUI.SetActive(true);
        timer.SetActive(true);
        showMenu = false;
        Time.timeScale = 1;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        Time.timeScale = 1;
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void OpenOptionsMenu()
    {
        pauseMenu.SetActive(false);
        optionsMenu.SetActive(true);
    }
}