using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public static PauseMenu instance;
    [SerializeField] public GameObject PauseMenuPanel;
    public  bool isPaused = false;
    
    private void Awake()
    {
        if(instance != null)
        {
            Debug.LogError("There is an instance of pausemenu in the scene");
        }
        instance = this;
    }
    private void Start()
    {
    }

    private void Update()
    {
        
    }
    public void Pause()
    {
      
        PauseMenuPanel.SetActive(true);
        isPaused = true;
        //Debug.Log("ON Paused " + "isPaused bool is " + isPaused);
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void Resume()
    {
       
        if(PauseMenuPanel != null)
            PauseMenuPanel.SetActive(false);

        if (UserSettings.instance.optionsMenu.activeSelf)
            UserSettings.instance.CloseMenu();
        isPaused = false;
       // Debug.Log("ON Resume " + "isPaused bool is  " + isPaused);
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

     public void Restart()
     {
        Time.timeScale = 1f;
        SceneManager.LoadScene("GameScene");
       

     }

    public void Exit()
    {
        isPaused = false;
        Time.timeScale = 0f;
        Application.Quit();
    }

    public void MainMenu()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
    }
}
