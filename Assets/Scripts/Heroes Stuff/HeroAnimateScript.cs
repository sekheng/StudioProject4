﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Used to animate the hero's movement, melee attack, and range attack!
/// </summary>
[RequireComponent(typeof(Animator))]
public class HeroAnimateScript : MonoBehaviour {
    private Animator heroAnimationController;

    [Tooltip("All the animation for movement")]
    public List<string> m_heroAnimationMovement = new List<string>();
    //[Tooltip("All Melee attack animation")]
    //public List<string> m_heroMeleeAnimation = new List<string>();

    // This will be used to be in aligned with all other animation.
    // For example, moving up animation index is 0. When player melee attacks, it will melee attack up animation whose index happens to be 0!
    private int indexOfPlayerAnimation = 0;
    private bool isAttacking = false, isShooting = false;

    // To make slow down so that hero looks idling
    //private bool slowAnimSpeed = false;

	// Use this for initialization
	void Start () {
        heroAnimationController = GetComponentInChildren<Animator>();
	}
	
    /// <summary>
    /// For now, it will have to hardcoding!
    /// </summary>
    /// <param name="zeDir">
    /// The direction of the heroe is facing!
    /// </param>
    public void moveAnimation(Vector2 zeDir)
    {
        stopRangeAttackAnimation();
        // We have to make sure the idling animation stops
        heroAnimationController.SetBool("Idling", false);

        heroAnimationController.SetBool(m_heroAnimationMovement[indexOfPlayerAnimation], false);
        //Debug.Log("Hero movement direction: " + zeDir);
        if (zeDir.x >= 1)
            indexOfPlayerAnimation = 0;
        else if (zeDir.y >= 1)
            indexOfPlayerAnimation = 1;
        else if (zeDir.x <= -1)
        {
            indexOfPlayerAnimation = 2;
            //Debug.Log(m_heroAnimationMovement[2]);
        }
        else
        {
            indexOfPlayerAnimation = 3;
            //Debug.Log(m_heroAnimationMovement[3]);
        }
        // Need to check whether the player is purely walking or attacking!
        //if (!isAttacking)
            //heroAnimationController.Play(m_heroAnimationMovement[indexOfPlayerAnimation]);
        //else
        //{
        //    heroAnimationController.Ge
        //}
        //Debug.Log(m_heroAnimationMovement[indexOfPlayerAnimation]);
        heroAnimationController.SetBool(m_heroAnimationMovement[indexOfPlayerAnimation], true);
    }

    public void stopAnimation()
    {
        heroAnimationController.StopPlayback();
    }

    public void meleeAttackAnimation()
    {
        //Debug.Log("Melee animation");
        //heroAnimationController.Play(m_heroMeleeAnimation[indexOfPlayerAnimation]);
        heroAnimationController.SetBool("Attack", true);
        isAttacking = true;
        stopRangeAttackAnimation();
        heroAnimationController.SetBool("Idling", false);
    }
    public void stopMeleeAttack()
    {
        //Debug.Log("Stop melee animation: " + isAttacking);
        switch (isAttacking)
        {
            case true:
                heroAnimationController.SetBool("Attack", false);
                //heroAnimationController.Play(m_heroAnimationMovement[indexOfPlayerAnimation]);
                //Debug.Log("Moving the Player");
                isAttacking = false;
                break;
            case false:
                break;
        }
    }

    /// <summary>
    /// This will start the range attack animation!
    /// </summary>
    public void rangeAttackAnimation()
    {
        stopMeleeAttack();
        // If there isn't any shooting animation to begin with
        if (!isShooting)
        {
            heroAnimationController.SetBool("Idling", false);
            heroAnimationController.SetBool("RangeAttack", true);
            isShooting = true;
        }
    }
    public void stopRangeAttackAnimation()
    {
        if (isShooting)
        {
            heroAnimationController.SetBool("RangeAttack", false);
            isShooting = false;
        }
    }

    public void startIdlingAnimation()
    {
        heroAnimationController.SetBool("Idling", true);
    }
}
