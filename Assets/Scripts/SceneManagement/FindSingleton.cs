﻿using UnityEngine;

public class FindSingleton : MonoBehaviour {

    public void GoScene()
    {
       LocalDataSingleton.instance.GoNext();
    }

    public void OpenOptions()
    {
        LocalDataSingleton.instance.OptionsCanvas.SetActive(true);
    }

    public void Quit()
    {
        LocalDataSingleton.instance.QuitApp();
    }
}
