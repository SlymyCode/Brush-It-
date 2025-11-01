using NUnit.Framework.Constraints;
using UnityEngine;
using TMPro;
using Unity.UI;
using UnityEngine.InputSystem;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private GameObject pauseMenu;
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
            showMenu = true;
            Time.timeScale = 0;
            Cursor.visible = true;
            
        }
        else if (Keyboard.current[key].wasPressedThisFrame && showMenu) 
        {
            pauseMenu.SetActive(false);
            showMenu = false;
            Time.timeScale = 1;
        }
    }

    public void Test()
    {
        Debug.Log("Test");
    }
}