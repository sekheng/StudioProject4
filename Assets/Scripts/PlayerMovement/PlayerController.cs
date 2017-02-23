﻿using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

/// <summary>
/// Used for keyboard and android mobile controller!
/// Too lazy to create another script for android mobile
/// Will add for mobile soon!
/// </summary>
public class PlayerController : MonoBehaviour {
    // There will only be 1 hero throughout the entire game!
    private HeroesMovement theOnlyHero;
    // This is to check if the player is not moving the character, then stop the movement!
    private bool checkPlayerMoved = false;
    // We will need this to check which key is pressed!
    private KeyCode currentKeyPressed;
    private Vector2 GeneralDir = Vector2.zero;

    private exampleUI diagUI;
    void Start()
    {
        if (diagUI == null)
        {
            diagUI = GameObject.Find("LocalDataSingleton").transform.GetComponentInChildren<exampleUI>();

        }
    }

#if UNITY_STANDALONE
    // Update is called once per frame
	void Update () {
        if (theOnlyHero == null && (
            SceneManager.GetActiveScene().buildIndex != 0 && //splashscreen
            SceneManager.GetActiveScene().buildIndex != 1 && //mainmenu
            SceneManager.GetActiveScene().buildIndex != 8 && //winpage
            SceneManager.GetActiveScene().buildIndex != 9 ))//losepage
        {
            theOnlyHero = GameObject.FindGameObjectWithTag("Player").GetComponent<HeroesMovement>();
        }
        else if (theOnlyHero != null && (
            SceneManager.GetActiveScene().buildIndex != 0 && //splashscreen
            SceneManager.GetActiveScene().buildIndex != 1 && //mainmenu
            SceneManager.GetActiveScene().buildIndex != 8 && //winpage
            SceneManager.GetActiveScene().buildIndex != 9))//losepage
        {
            // Here we shall check which key is pressed so that interception can happen!
            if (!LocalDataSingleton.instance.talking)
            {
                if (Input.GetKeyDown(KeyCode.UpArrow))
                    currentKeyPressed = KeyCode.UpArrow;
                if (Input.GetKeyDown(KeyCode.DownArrow))
                    currentKeyPressed = KeyCode.DownArrow;
                if (Input.GetKeyDown(KeyCode.RightArrow))
                    currentKeyPressed = KeyCode.RightArrow;
                if (Input.GetKeyDown(KeyCode.LeftArrow))
                    currentKeyPressed = KeyCode.LeftArrow;
                if (Input.GetKeyDown(KeyCode.Z))
                    currentKeyPressed = KeyCode.Z;
                if (Input.GetKeyDown(KeyCode.X))
                    currentKeyPressed = KeyCode.X;

                // Here we check whether the player has continuously press it for movement
                if (Input.GetKey(currentKeyPressed))
                {
                    switch (currentKeyPressed)
                    {
                        case KeyCode.UpArrow:
                            theOnlyHero.moveDirection(new Vector2(0, 1));
                            GeneralDir = new Vector2(0, 1);
                            break;
                        case KeyCode.DownArrow:
                            theOnlyHero.moveDirection(new Vector2(0, -1));
                            GeneralDir = new Vector2(0, -1);
                            break;
                        case KeyCode.RightArrow:
                            theOnlyHero.moveDirection(new Vector2(1, 0));
                            GeneralDir = new Vector2(1, 0);
                            break;
                        case KeyCode.LeftArrow:
                            theOnlyHero.moveDirection(new Vector2(-1, 0));
                            GeneralDir = new Vector2(-1, 0);
                            break;
                        default:
                            theOnlyHero.passInKeyPressed(currentKeyPressed);
                            //Debug.Log("Something is wrong with current keypressed");
                            break;
                    }
                    checkPlayerMoved = true;
                }
            }
            if (theOnlyHero.GetComponent<HealthScript>().m_health <= 0)
            {
                LocalDataSingleton.instance.onLose();
                theOnlyHero.GetComponent<HealthScript>().resetToMaxHealth(theOnlyHero.GetComponent<HealthScript>().max_health);
            }
        }

    }

    void LateUpdate()
    {
        if (theOnlyHero != null && !LocalDataSingleton.instance.talking)
        {
            switch (checkPlayerMoved)
            {
                case false:
                    // If player hasn't move, stop the hero's movement!
                    theOnlyHero.stopMovement();
                    break;
                case true:
                    // Then initialized it to be false!
                    checkPlayerMoved = false;
                    break;
            }
        }
    }
#endif

    public void TryInteract()
    {
        if (theOnlyHero == null)
        {
            theOnlyHero = GameObject.FindGameObjectWithTag("Player").GetComponent<HeroesMovement>();
        }
        Vector2 choosenOne = GeneralDir == Vector2.zero ? GameObject.FindGameObjectWithTag("Player").GetComponent<HeroesMovement>().directionOfHero : GeneralDir;

        RaycastHit2D rHit = Physics2D.Raycast((Vector2)theOnlyHero.transform.position + choosenOne, choosenOne, 1.0f);
        if (rHit.collider != null)
        {
            //In this example, we will try to interact with any collider the raycast finds
            //Lets grab the NPC's DialogueAssign script... if there's any

            VIDE_Assign assigned;
            if (rHit.collider.GetComponent<VIDE_Assign>() != null)
            {
                assigned = rHit.collider.GetComponent<VIDE_Assign>();
            }
            else return;

            if (!diagUI.dialogue.isLoaded)
            {
                //... and use it to begin the conversation
                diagUI.Begin(assigned);
                LocalDataSingleton.instance.talking = true;
            }
            else
            {
                //If conversation already began, let's just progress through it
                diagUI.NextNode();
            }
//#if UNITY_ANDROID
//            if (rHit.collider.GetComponent<minUIExample>() != null && !LocalDataSingleton.instance.talking)
//            {
//                LocalDataSingleton.instance.talking = true;
//                rHit.collider.GetComponent<minUIExample>().dialogue.BeginDialogue(rHit.collider.GetComponent <VIDE_Assign>());
//                Debug.Log("AC");
//            }
//#endif
        }
    }
}
