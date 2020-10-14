using System.Collections;
using System.Collections.Generic;
using System.Linq;
using PlayFab;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class OptionsMenu : MonoBehaviour
{
    List<Resolution> resolutions;
    public TMP_Dropdown ResolutionDropdown;
    public Toggle isFullscreen;
    public Slider musicSlider;
    public Slider sfxSlider;


    private void Start()
    {
        musicSlider.value = PlayerPrefs.GetFloat("MusicVol");
        sfxSlider.value = PlayerPrefs.GetFloat("SFXVol");
        int CurrentResolutionIndex =  PlayerPrefs.GetInt("ResInt");;
        resolutions = Screen.resolutions.ToList();
        
        ResolutionDropdown.ClearOptions();
        if (PlayerPrefs.GetInt("isFullscreen") == 1)
        {
            isFullscreen.isOn = true;
        }
        else
        {
            isFullscreen.isOn = false;
        }

        List<string> options = new List<string>();

        List<Resolution> toRemove = new List<Resolution>();
        for (int i = 0; i < resolutions.Count; i++)
        {
            string Option = resolutions[i].width + " x " + resolutions[i].height;
                 if (!options.Contains(Option) && Mathf.Approximately(resolutions[i].width/(float)resolutions[i].height, 16f/9f))
                 {
                     options.Add(Option);
                 }
                 else
                 {
                     toRemove.Add(resolutions[i]);
                 }
                 
           
        }

        for (var i = toRemove.Count - 1; i >= 0; i--)
        {
            resolutions.Remove(toRemove[i]);
        }
        

        ResolutionDropdown.AddOptions(options);
        ResolutionDropdown.value = CurrentResolutionIndex;
        ResolutionDropdown.RefreshShownValue();
    }

    public void SetResolution(int ResolutionIndex)
    {
        Resolution resolution = resolutions[ResolutionIndex];
        PlayerPrefs.SetInt("ResInt", ResolutionIndex);
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
    }

    public void SetMusicVolume(float volume)
    {
        PlayerPrefs.SetFloat("MusicVol", volume);
    }

    public void SetSFXVolume(float volume)
    {
        PlayerPrefs.SetFloat("SFXVol", volume);
    }

    


    public void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
        if (isFullscreen)
        {
            PlayerPrefs.SetInt("isFullscreen",1);
        }
        else
        {
            PlayerPrefs.SetInt("isFullscreen",0);
        }
        
    }
}