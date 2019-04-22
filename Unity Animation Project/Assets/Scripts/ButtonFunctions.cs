using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonFunctions : MonoBehaviour
{
    public string sceneToLoadIfNeeded;
    public Canvas canvasToLoadIfNeeded;
    private Canvas myCanvas;

    private void Awake()
    {
        myCanvas = GetComponentInParent<Canvas>();
    }

    public void Resume()
    {
        if (GameManager.Me)
        {
            GameManager.Me.Paused = false;
        }
    }

    public void Return()
    {
        if (sceneToLoadIfNeeded != "")
        {
            Time.timeScale = 1.0f;
            SceneManager.LoadSceneAsync(sceneToLoadIfNeeded);
        }
    }

    public void Quit()
    {
        Application.Quit();
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }

    public void ChangeCanvas()
    {
        if (canvasToLoadIfNeeded)
        {
            canvasToLoadIfNeeded.enabled = true;
            myCanvas.enabled = false;
        }
    }

    //Used since OptionsCanvasManager is persistent between scenes and is accessed from multiple menus
    public void LoadOptionsMenu()
    {
        if (OptionsCanvasManager.settings && OptionsCanvasManager.settings.back)
        {
            OptionsCanvasManager.settings.back.canvasToLoadIfNeeded = myCanvas;
            OptionsCanvasManager.settings.myCanvas.enabled = true;
            myCanvas.enabled = false;
        }
    }

    public void Apply()
    {
        if (OptionsCanvasManager.settings)
        {
            OptionsCanvasManager.settings.Apply();
        }
    }
}
