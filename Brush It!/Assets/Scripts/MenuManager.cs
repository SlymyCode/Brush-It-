using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    [SerializeField] private Canvas mainMenu;
    [SerializeField] private Canvas pauseMenu;
    [SerializeField] private Canvas optionsMenu;
    [SerializeField] private GameObject playerUI;
    [SerializeField] private GameObject timer;
    [SerializeField] private GameObject playerCamera;
    [SerializeField] private GameObject mainMenuCamera;
    public LevelMusic levelMusic;
    private bool showMenu = false;
    private bool inGame = false;
    
    void Start()
    {
        if (SceneManager.GetActiveScene().name == "Level 1")
        {
            playerUI.SetActive(true);
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            inGame = true;
        }
        else
        {
            playerCamera.SetActive(false);
            mainMenuCamera.SetActive(true);
            playerUI.SetActive(false);
            mainMenu.enabled = true;
            pauseMenu.enabled = false;
            optionsMenu.enabled = false;
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;   
        }
    }
    
    void Update()
    {
        if (inGame)
        {
            Pause();
        }
        else
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
    }

    public void Play()
    {
        inGame = true;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        mainMenu.enabled = false;
        mainMenuCamera.SetActive(false);
        playerUI.SetActive(true);
        playerCamera.SetActive(true);
        levelMusic.PlayTrack(1);
    }
    
    public void Pause()
    {
        if ((Keyboard.current.escapeKey.wasPressedThisFrame || Gamepad.current?.startButton.wasPressedThisFrame == true) && !showMenu && inGame)
        {
            pauseMenu.enabled = true;
            playerUI.SetActive(false);
            optionsMenu.enabled = false;
            timer.SetActive(false);
            showMenu = true;
            Time.timeScale = 0;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else if ((Keyboard.current.escapeKey.wasPressedThisFrame || (Gamepad.current?.startButton.wasPressedThisFrame == true || Gamepad.current?.buttonEast.wasPressedThisFrame == true)) && optionsMenu.enabled && inGame)
        {
            optionsMenu.enabled = false;
            pauseMenu.enabled = true;
            playerUI.SetActive(false);
        }
        else if ((Keyboard.current.escapeKey.wasPressedThisFrame || (Gamepad.current?.startButton.wasPressedThisFrame == true || Gamepad.current?.buttonEast.wasPressedThisFrame == true)) && showMenu && inGame)
        {
            pauseMenu.enabled = false;
            playerUI.SetActive(true);
            optionsMenu.enabled = false;
            timer.SetActive(true);
            showMenu = false;
            Time.timeScale = 1;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    public void Unpause()
    {
        pauseMenu.enabled = false;
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
        if (!inGame)
        {
            mainMenu.enabled = false;
            optionsMenu.enabled = true;
        }
        else if (inGame)
        {
            pauseMenu.enabled = false;
            optionsMenu.enabled = true;
        }
    }

    public void CloseOptionsMenu()
    {
        if (!inGame)
        {
            mainMenu.enabled = true;
            optionsMenu.enabled = false;
        }
        else if (inGame)
        {
            pauseMenu.enabled = true;
            optionsMenu.enabled = false;
        }
    }
}