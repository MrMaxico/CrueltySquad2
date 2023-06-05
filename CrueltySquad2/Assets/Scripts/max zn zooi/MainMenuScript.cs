using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuScript : MonoBehaviour
{
    public GameObject menuUI;
    public GameObject levelSelectUI;
    public GameObject settingsUI;
    public GameObject creditsUI;

    public AudioSource buttonClick;

    public AudioMixer audioMixer;

    Resolution[] resolutions;
    public TMPro.TMP_Dropdown resolutionsDropdown;

    void Start()
    {

        resolutions = Screen.resolutions;

        resolutionsDropdown.ClearOptions();

        List<string> options = new List<string>();

        int currentResolutionIndex = 0;
        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + " x " + resolutions[i].height;       //dit calculeerd alle unity resoluties, geen idee hoe en wat precies lmao
            options.Add(option);

            if (resolutions[i].width == Screen.currentResolution.width && resolutions[i].height == Screen.currentResolution.height)
            {
                currentResolutionIndex = i;
            }
        }

        resolutionsDropdown.AddOptions(options);
        resolutionsDropdown.value = currentResolutionIndex;
        resolutionsDropdown.RefreshShownValue();
        QualitySettings.SetQualityLevel(5);
    }

    public void SetResolution(int resolutionIndex)
    {
        Resolution resolution = resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
    }

    public void setFullScreen(bool isFullScreen)
    {
        buttonClick.Play();
        Screen.fullScreen = isFullScreen;
    }


    //Graphics Quality//

    public void low(bool quality)
    {
        if (quality)
        {
            buttonClick.Play();
            QualitySettings.SetQualityLevel(0);
        }
    }

    public void medium(bool quality)
    {
        if (quality)
        {
            buttonClick.Play();
            QualitySettings.SetQualityLevel(2);
        }
    }

    public void high(bool quality)
    {
        if (quality)
        {
            buttonClick.Play();
            QualitySettings.SetQualityLevel(3);
        }
    }

    public void ultra(bool quality)
    {
        if (quality)
        {
            buttonClick.Play();
            QualitySettings.SetQualityLevel(5);
        }
    }

    //VOLUME SLIDERS//
    public void SetMasterVolume(float volume)
    {
        audioMixer.SetFloat("MasterVolume", Mathf.Log10(volume) * 20);
    }
    public void SetSoundVolume(float volume)
    {
        audioMixer.SetFloat("SoundVolume", Mathf.Log10(volume) * 20);
    }

    public void SetMusicVolume(float volume)
    {
        audioMixer.SetFloat("MusicVolume", Mathf.Log10(volume) * 20);
    }

    //BUTTONS//
    public void GoToLevelSelect()
    {
        buttonClick.Play();
        menuUI.SetActive(false);
        levelSelectUI.SetActive(true);
        settingsUI.SetActive(false);
        creditsUI.SetActive(false);
    }

    public void GoToSettings()
    {
        buttonClick.Play();
        menuUI.SetActive(false);
        levelSelectUI.SetActive(false);
        settingsUI.SetActive(true);
        creditsUI.SetActive(false);
    }

    public void GoToCredits()
    {
        buttonClick.Play();
        menuUI.SetActive(false);
        levelSelectUI.SetActive(false);
        settingsUI.SetActive(false);
        creditsUI.SetActive(true);
    }

    public void QuitGame()
    {
        buttonClick.Play();
        Application.Quit();
    }

    public void ReturnToMenu()
    {
        buttonClick.Play();
        menuUI.SetActive(true);
        levelSelectUI.SetActive(false);
        settingsUI.SetActive(false);
        creditsUI.SetActive(false);
    }

    public void LoadLevel1()
    {
        buttonClick.Play();
        SceneManager.LoadScene("franswildit");
        //FindObjectOfType<AudioManagerScript>().Play("DefenceSetupMusic");
    }

    public void LoadLevel2()
    {
        buttonClick.Play();
        SceneManager.LoadScene("SunsetLevel");
        //FindObjectOfType<AudioManagerScript>().Play("DefenceSetupMusic");
    }

    //SETTINGS//

    public void SetQuality(int qualityIndex)
    {
        buttonClick.Play();
        QualitySettings.SetQualityLevel(qualityIndex);
    }

    // public void AdjustBrightness(float value)
    //   {
    //if(value != 0)
    //     {
    //exposure.keyValue.value = value;
    //   }
    // else
    //    {
    //exposure.keyValue.value = 0.5f;
    //   }

    //}
}
