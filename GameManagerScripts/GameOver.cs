using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOver : MonoBehaviour
{
    public static GameOver instance;
    [HideInInspector] public bool gameOver = false;
    public GameObject gameOverMenu;
    public TextMeshProUGUI message;
    // Start is called before the first frame update
    private void Awake()
    {
        if (instance != null)
            Debug.LogError("There is another instance of Gameover");
        instance = this;
    }
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
    
    }

    public void Playerdied()
    {

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        gameOver = true;
        gameOverMenu.SetActive(true);
        message.text = "Game Over";


    }

    public void PlayerCompletedMission()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        gameOver = true;
        gameOverMenu.SetActive(true);
        message.text = "Congratulations";
    }

    public void Restart()
    {
        Time.timeScale = 1f;
        gameOver = false;
        SceneManager.LoadScene("GameScene");
    }

}
