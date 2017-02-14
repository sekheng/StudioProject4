﻿using UnityEngine;
using System.Collections;

/// <summary>
/// This is for player to press the certain key then it will activate the button!
/// </summary>
[RequireComponent(typeof(MeleeScript))]
public class HeroMeleeButton : MonoBehaviour {
    private MeleeScript theHeroMeleeSystem;

	// Use this for initialization
	void Start () {
        theHeroMeleeSystem = GetComponent<MeleeScript>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}