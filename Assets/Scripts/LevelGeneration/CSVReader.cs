﻿using UnityEngine;
using System.Collections;
using System.Linq; 

public class CSVReader : MonoBehaviour {

    public TextAsset file;
    public int offset = 0;
    //0
    public GameObject PlayerPrefab;
    
    //From 1 to 10.
    public GameObject OuterWallPrefab;
    public GameObject WallPrefab;
    public GameObject StonePrefab;
    public GameObject RelicPrefab;
    public GameObject PickableCompassPrefab;

    //Enemy ID will start from 10
    public GameObject SlimePrefab;

    //BOSS from 20
    public GameObject GolemPrefab;

    public GameObject PlayerController;
    public GameObject floorPrefab;

    private string[,] LevelLayoutArray;
    private string [] textRow;
    private GameObject lvlHolder;

	// Use this for initialization
	void Start () {
        lvlHolder = new GameObject("LevelHolder");

	    LevelLayoutArray = CSVReader.SplitCsvGrid(file.text);
        textRow = new string[LevelLayoutArray.GetUpperBound(0)];  // Instantiate the textRow array with the number of rows in csvData array.

        int height = ((LevelLayoutArray.Length / textRow.Length) - 1) + offset;
        int width = textRow.Length - 1;

        //Debug.Log(width.ToString() + " " + height.ToString());

        float offsetX = width % 2 == 0 ? 0.5f : 0.0f;
        float offsetY = -0.5f; //compulsory

        for (int y = 0; y < height; y++) // height
        {
            for (int x = 0; x < width; x++)
            {
                Vector3 objPos = new Vector3(x - (width >> 1) + offsetX, (-y + (height >> 1) + offsetY), 0);
                GameObject result;
                if (LevelLayoutArray[x, y] == "0")
                {
                    result = Instantiate(PlayerPrefab, objPos, Quaternion.identity) as GameObject;
                    Instantiate(PlayerController, objPos, Quaternion.identity);
                }
                else if (LevelLayoutArray[x, y] == "1")
                {
                    result = Instantiate(OuterWallPrefab, objPos, Quaternion.identity) as GameObject;
                }
                else if (LevelLayoutArray[x, y] == "2")
                {
                    result = Instantiate(WallPrefab, objPos, Quaternion.identity) as GameObject;
                }
                else if (LevelLayoutArray[x, y] == "3")
                {
                    result = Instantiate(StonePrefab, objPos, Quaternion.identity) as GameObject;
                }
                //else if (LevelLayoutArray[x, y] == "4")
                //{
                //    result = Instantiate(RelicPrefab, objPos, Quaternion.identity) as GameObject;
                //}
                else if (LevelLayoutArray[x, y] == "5")
                {
                    result = Instantiate(PickableCompassPrefab, objPos, Quaternion.identity) as GameObject;
                }
                else if(LevelLayoutArray[x, y] == "10")
                {
                    result = Instantiate(SlimePrefab, objPos, Quaternion.identity) as GameObject;
                }
                //else if (LevelLayoutArray[x, y] == "20")
                //{
                //    result = Instantiate(GolemPrefab, objPos, Quaternion.identity) as GameObject;
                //}
                else
                {
                    result = Instantiate(floorPrefab, objPos, Quaternion.identity) as GameObject;
                }
                if (result.name.Equals("wall(Clone)") || result.name.Equals("OuterWall(Clone)") || result.name.Equals("Floor(Clone)")) //Please remember to tag all your objects accordingly lest they be thrown into the holder!
                {
                    result.transform.SetParent(lvlHolder.transform);
                }
            }
        }
	}

    // splits a CSV file into a 2D string array
	static public string[,] SplitCsvGrid(string csvText)
	{
		string[] lines = csvText.Split("\n"[0]); 
 
		// finds the max width of row
		int width = 0; 
		for (int i = 0; i < lines.Length; i++)
		{
			string[] row = SplitCsvLine( lines[i] ); 
			width = Mathf.Max(width, row.Length); 
		}
 
		// creates new 2D string grid to output to
		string[,] outputGrid = new string[width + 1, lines.Length + 1]; 
		for (int y = 0; y < lines.Length; y++)
		{
			string[] row = SplitCsvLine( lines[y] ); 
			for (int x = 0; x < row.Length; x++) 
			{
				outputGrid[x,y] = row[x]; 
 
				// This line was to replace "" with " in my output. 
				// Include or edit it as you wish.
				outputGrid[x,y] = outputGrid[x,y].Replace("\"\"", "\"");
			}
		}
 
		return outputGrid; 
	}
    // splits a CSV row 
    static public string[] SplitCsvLine(string line)
    {
        return (from System.Text.RegularExpressions.Match m in System.Text.RegularExpressions.Regex.Matches(line,
        @"(((?<x>(?=[,\r\n]+))|""(?<x>([^""]|"""")+)""|(?<x>[^,\r\n]+)),?)",
        System.Text.RegularExpressions.RegexOptions.ExplicitCapture)
                select m.Groups[1].Value).ToArray();
    }
}
