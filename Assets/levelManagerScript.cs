using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class levelManagerScript : MonoBehaviour
{
    public Animator fadeAnimator;

    public float transitionTime = 0.5f;

    public List<Animator> transitions;
   // private AudioManager _audioManager;
    
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void OnBeforeSceneLoadRuntimeMethod()
    {
        PlayerPrefs.SetInt("fadeIndex", 0);
    }

    private void Start()
    {
        //_audioManager = FindObjectOfType<AudioManager>();
        UpdateTransitions();
        fadeAnimator.SetTrigger("End");
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void LoadNextLevel(int buildIndex, int fadeIndex)
    {
        PlayerPrefs.SetInt("fadeIndex", fadeIndex);
        UpdateTransitions();
        StartCoroutine(LoadLevel(buildIndex));
    }

    public IEnumerator LoadNextLevelAfterTime(int buildIndex, float seconds)
    {
        yield return new WaitForSeconds(seconds);
        StartCoroutine(LoadLevel(buildIndex));
    }

    IEnumerator LoadLevel(int buildIndex)
    {
        fadeAnimator.SetTrigger("Start");
       /* for (var i = 0; i < _audioManager.sounds.Length; i++)
        {
            if (_audioManager.sounds[i].stopOnLoadScene)
            {
                _audioManager.Stop(_audioManager.sounds[i].name);
            }
        }*/
        yield return new WaitForSeconds(transitionTime);
        Time.timeScale = 1f;
        SceneManager.LoadScene(buildIndex);
    }

    public void UpdateTransitions()
    {
        int index = PlayerPrefs.GetInt("fadeIndex");
        fadeAnimator = transitions[index];
        for (var i = 0; i < transitions.Count; i++)
        {
            if (i != index)
            {
                transitions[i].gameObject.SetActive(false);
            }
            else
            {
                transitions[i].gameObject.SetActive(true);
            }
        }
    }
}
