﻿using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class LocalDataSingleton : MonoBehaviour {

    public static LocalDataSingleton instance = null;

    public GameObject MainCanvas;
    public GameObject OptionsCanvas;
    public GameObject MainMenuCanvas; // only use once, so don't really need to care.
    [Tooltip("Inventory Canvas")]
    public GameObject Inventorycanvas;

    [HideInInspector]
    public int previousSceneFrom = -1;
    [HideInInspector]
    public bool talkedToDragon = false;
    [HideInInspector]
    public bool movementDisabled = false;
    [HideInInspector]
    public bool talking = false;

    private float Volume { get; set; }
    private bool Transiting = false;

    // Earlier than start
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(this.gameObject);
        }
        DontDestroyOnLoad(transform.gameObject);
    }

    void Update()
    {
        AudioListener.volume = Volume;
        if ((SceneManager.GetActiveScene().buildIndex != 0 || SceneManager.GetActiveScene().buildIndex != 1) && !transform.GetChild(3).gameObject.activeSelf)
        {
            transform.GetChild(3).gameObject.SetActive(!transform.GetChild(3).gameObject.activeSelf);
        }

        if (MainMenuCanvas == null && SceneManager.GetActiveScene().buildIndex == 1)
        {
            MainMenuCanvas = GameObject.Find("MainMenucanvas");
        }
        else if (SceneManager.GetActiveScene().buildIndex == 1)
        {
#if UNITY_STANDALONE
            MainMenuCanvas.transform.GetChild(2).gameObject.SetActive(true);
#else
            MainMenuCanvas.transform.GetChild(2).gameObject.SetActive(false);
#endif
        }

#if UNITY_STANDALONE
        MainCanvas.transform.GetChild(0).gameObject.SetActive(false);
        MainCanvas.transform.GetChild(2).gameObject.SetActive(false);
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            if (SceneManager.GetActiveScene().buildIndex != 0 || SceneManager.GetActiveScene().buildIndex != 1)
            {
                //activate the child canvas
                OptionsCanvas.gameObject.SetActive(!OptionsCanvas.gameObject.activeSelf);
            }
        }
#endif
#if UNITY_ANDROID
        if(SceneManager.GetActiveScene().buildIndex != 0 || SceneManager.GetActiveScene().buildIndex != 1)
        {
            MainCanvas.transform.GetChild(2).gameObject.SetActive(true);
            MainCanvas.transform.GetChild(0).gameObject.SetActive(true);
        }
        else
        {
            MainCanvas.transform.GetChild(2).gameObject.SetActive(false);
            MainCanvas.transform.GetChild(0).gameObject.SetActive(false);
        }
#endif
    }

    public void QuitApp()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
#if UNITY_STANDALONE
        Application.Quit();
#endif
    }

//FROM HERE ONE ARE OPTIONS TO HELP DO SCENE MOVEMENT
    public void ReturntoMain()
    {
        if(OptionsCanvas.gameObject.activeSelf)
        {
            OptionsCanvas.gameObject.SetActive(false);
        }
        if (!Transiting)
        {
            StartCoroutine(ChangeLevel(1));
        }
    }

    public void ReturntoPrevious()
    {
        if (!Transiting)
        {
            StartCoroutine(ChangeLevel(LocalDataSingleton.instance.previousSceneFrom));
        }
    }

    public void GoNext()
    {
        if (!Transiting)
        {
            StartCoroutine(ChangeLevel(SceneManager.GetActiveScene().buildIndex + 1));
        }
    }

    public void onWin()
    {
        if (!Transiting)
        {
            StartCoroutine(ChangeLevel(8));
        }
    }

    public void onLose()
    {
        if (!Transiting)
        {
            StartCoroutine(ChangeLevel(9));
        }
    }

    IEnumerator ChangeLevel(int index)
    {
        Transiting = true;
        LocalDataSingleton.instance.previousSceneFrom = SceneManager.GetActiveScene().buildIndex;
        float fadeTime = GetComponent<Fading>().BeginFade(1);
        yield return new WaitForSeconds(fadeTime);
        if (talking)
            talking = false;
        SceneManager.LoadScene(index);
        Transiting = false;
    }
}
