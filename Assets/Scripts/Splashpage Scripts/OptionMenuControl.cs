﻿using UnityEngine;
using UnityEngine.UI;

public class OptionMenuControl : MonoBehaviour {

    public GameObject[] stuff;
    public GameObject WindowsCTRL;
    private int lookAt;

#if UNITY_STANDALONE
	// Use this for initialization
	void Start () {
        foreach (GameObject t in stuff)
        {
            //
        }
        lookAt = 0;
	}
	
	// Update is called once per frame
	void Update () {
        if (!WindowsCTRL.activeSelf)
        {
            if (Input.GetKeyDown(KeyCode.UpArrow) && lookAt > 0)
            {
                --lookAt;
                GetComponent<RectTransform>().anchoredPosition = stuff[lookAt].GetComponent<RectTransform>().anchoredPosition;
            }
            else if (Input.GetKeyDown(KeyCode.DownArrow) && lookAt < (stuff.Length - 1))
            {
                ++lookAt;
                GetComponent<RectTransform>().anchoredPosition = stuff[lookAt].GetComponent<RectTransform>().anchoredPosition;
            }

            if(Input.GetKeyDown(KeyCode.Return))
            {
                switch(lookAt)
                {
                    case 0:
                    {
                        WindowsCTRL.SetActive(!WindowsCTRL.activeSelf);
                        break;
                    }
                    case 1:
                    {
                        //some other things
                        break;
                    }
                }
            }
        }
    }
#endif
}