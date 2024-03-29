﻿using UnityEngine;
using System.Collections;

public class Compass : MonoBehaviour {
    private GameObject target;
    private GameObject thePlayer;
    private Vector3 targetDir;
    private float angle;

    public float offsetForRotation;
	
    
	// Update is called once per frame
	void Update () {
        if (target != null && thePlayer != null)
        {
            targetDir = target.transform.position - thePlayer.transform.position;
            angle = Vector3.Angle(targetDir, Vector3.up);
            //Debug.Log(angle.ToString());
            //transform.rotation = new Quaternion(transform.rotation.x, transform.rotation.y, angle + 90, transform.rotation.w);
            if (thePlayer.transform.position.x > target.transform.position.x)
            {
                transform.rotation = Quaternion.Euler(0, 0, angle + offsetForRotation);
            }
            else//vector3 angle is < 180 always
            {
                transform.rotation = Quaternion.Euler(0, 0, -angle + offsetForRotation);
            }
            //Debug.Log((angle + offsetForRotation).ToString());
        }
        else
        {
            if (target == null)
            {
                target = GameObject.FindGameObjectWithTag("Exit");
            }
            if (thePlayer == null)
            {
                thePlayer = GameObject.FindGameObjectWithTag("Player");
            }
            transform.rotation = Quaternion.Euler(0, 0, angle + offsetForRotation);
        }
    }
}
