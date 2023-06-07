using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Teleporter : MonoBehaviour
{
    public int spawnersLeft;
    bool open;

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
        //fancy animations
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        yield return null;
    }
}
