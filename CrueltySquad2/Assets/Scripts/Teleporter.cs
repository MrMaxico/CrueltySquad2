using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Teleporter : MonoBehaviour
{
    public int spawnersLeft;
    bool open;
    public GameObject LoadingScreen;

    private void Start()
    {
        LoadingScreen.SetActive(false);
    }

    private void Update()
    {
        if (spawnersLeft < 1)
        {
            open = true;
        }
        else
        {
            open = false;
        }
    }

    public IEnumerator Teleport()
    {
        if (open)
        {
            LoadingScreen.SetActive(true);
            //fancy animations
            yield return new WaitForSecondsRealtime(1); //replace 1 with the amount of seconds of the length of the animation
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
        else
        {
            yield return new WaitForEndOfFrame();
        }
    }
}
