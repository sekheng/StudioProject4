﻿using UnityEngine;
using System.Collections;

/// <summary>
/// This will be used to save the slot the moment the player pressed it!
/// </summary>
public class SaveSlotScript : PauseButtonScript {
    // We will needthe LoadGameScript so as to save the data!
    private LoadGameScript toSaveToSQLite;
    // We will need the GameObject of the save canvas
    private GameObject theSaveCanvas;

	// Use this for initialization
	void Start () {
        toSaveToSQLite = GetComponent<LoadGameScript>();
        // We will need to access the parent's inGamePause to disable it's update!
        theSaveCanvas = GameObject.FindGameObjectWithTag("SaveCanvas");
    }

    public override bool executeButton()
    {
        //Debug.Log("Saving to the button: " + toSaveToSQLite.m_loadNumber);
        toSaveToSQLite.saveData();
        theSaveCanvas.SetActive(false);
        //LocalDataSingleton.instance.InGamePauseCanvas.GetComponent<InGamePauseScript>().m_disableUpdate = false;
        LocalDataSingleton.instance.turnOnInGamePauseMenu();
        return true;
    }
}
