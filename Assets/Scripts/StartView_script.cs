using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DUKE.Controls;

public class StartView_script : MonoBehaviour
{
    [SerializeField] private CanvasGroup fadeCanvas;

    [SerializeField]
    private VRInput leftController;

    [SerializeField]
    private VRInput rightController;

    private bool willLoad = false;



    private void Update()
    {
        if (UnityEngine.Input.GetButtonDown("Fire1"))
        {
            //StartCoroutine(fadeToNextLevel());
            willLoad = true;
        }
        if (leftController.TriggerPressed || rightController.TriggerPressed)
        {
            //StartCoroutine(fadeToNextLevel());
            willLoad = true;
        }

        if (willLoad)
        {
            showFade();
        }
        
    }
    
    IEnumerator fadeToNextLevel()
    {
        yield return null;
        AsyncOperation loadLevel = SceneManager.LoadSceneAsync(1);
        loadLevel.allowSceneActivation = false;
        while(!loadLevel.isDone)
        {
            float loadValue = Mathf.Clamp01(loadLevel.progress/2f);
            fadeCanvas.alpha = loadValue;
            if (loadLevel.progress >= 0.9f)
            {
                fadeCanvas.alpha = Mathf.Clamp01(loadLevel.progress);
                loadLevel.allowSceneActivation = true;
            }
            yield return null;
        }
    }

    private void showFade()
    {
        fadeCanvas.alpha += Time.deltaTime * 0.5f;
        if (fadeCanvas.alpha >= 1)
        {
            SceneManager.LoadScene(1, LoadSceneMode.Single);
        }
    }
}
