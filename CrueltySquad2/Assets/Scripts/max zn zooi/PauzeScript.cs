using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauzeScript : MonoBehaviour
{
    public static bool gameIsPaused = false;

    public GameObject pauseMenuUI;

    public AudioSource buttonClick;

    public KeyCode pauseKey;
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (gameIsPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }

    public void Resume()
    {
        Time.timeScale = 1f;
        buttonClick.Play();
        pauseMenuUI.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        gameIsPaused = false;
    }

    public void Pause()
    {
        Time.timeScale = 0f;
        buttonClick.Play();
        pauseMenuUI.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        gameIsPaused = true;
    }
    public void ReturnToMenu()
    {
        buttonClick.Play();
        Time.timeScale = 1f;
        gameIsPaused = false;
        SceneManager.LoadScene("MainMenu");
        //FindObjectOfType<AudioManagerScript>().StopPlaying("DefenceSetupMusic");
       // FindObjectOfType<AudioManagerScript>().StopPlaying("WaveMusic1");
       // FindObjectOfType<AudioManagerScript>().StopPlaying("WaveMusic2");
       // FindObjectOfType<AudioManagerScript>().StopPlaying("MamaSquidMusic");
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        //FindObjectOfType<AudioManagerScript>().Play("DefenceSetupMusic");
    }
}
