﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using System.IO;
using MiniJSON_VIDE;

[CustomEditor(typeof(VIDE_Assign))]
public class VIDE_AssignC : Editor
{
    /*
     * Custom Inspector for the VIDE_Assign component
     */
    VIDE_Assign d;

    private void openVIDE_Editor(int idx)
    {
        if (d != null)
            loadFiles();

        if (!Directory.Exists(Application.dataPath + "/" + VIDE_Editor.pathToVide + "VIDE"))
        {
            Debug.LogError("Cannot find VIDE folder! If you moved the VIDE folder from the root, make sure you set the 'pathToVide' variable in VIDE_Editor.cs");
            return;
        }

        VIDE_Editor editor = EditorWindow.GetWindow<VIDE_Editor>();
        editor.Init(idx, true);

    }

    void Awake()
    {
        loadFiles();
    }

    public class SerializeHelper
    {
        static string fileDataPath = Application.dataPath + "/" + VIDE_Editor.pathToVide + "VIDE/Resources/Dialogues/";
        public static void WriteToFile(object data, string filename)
        {
            string outString = DiagJson.Serialize(data);
            File.WriteAllText(fileDataPath + filename, outString);
        }
        public static object ReadFromFile(string filename)
        {
            string jsonString = File.ReadAllText(fileDataPath + filename);
            return DiagJson.Deserialize(jsonString);
        }
    }

    public override void OnInspectorGUI()
    {

        d = (VIDE_Assign)target;
		Color defColor = GUI.color;
		GUI.color = Color.yellow;

		//Create a button to open up the VIDE Editor and load the currently assigned dialogue
        if (GUILayout.Button("Open VIDE Editor"))
        {
            openVIDE_Editor(d.assignedIndex);
        }

        GUI.color = defColor;

        //Refresh dialogue list
        if (Event.current.type == EventType.MouseDown)
        {
            if (d != null)
                loadFiles();
        }
        GUILayout.BeginHorizontal();

        GUILayout.Label("Dialogue name: ");

        Undo.RecordObject(d, "Changed dialogue name");
        d.dialogueName = EditorGUILayout.TextField(d.dialogueName);

        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        GUILayout.Label("Assigned dialogue:");
        if (d.diags.Count > 0)
        {
            EditorGUI.BeginChangeCheck();

            Undo.RecordObject(d, "Changed dialogue index");

            d.assignedIndex = EditorGUILayout.Popup(d.assignedIndex, d.diags.ToArray());

            if (EditorGUI.EndChangeCheck())
            {
                int theID = 0;
                if (File.Exists(Application.dataPath + "/" + VIDE_Editor.pathToVide + "VIDE/Resources/Dialogues/" + d.diags[d.assignedIndex] + ".json"))
                {
                    Dictionary<string, object> dict = SerializeHelper.ReadFromFile(d.diags[d.assignedIndex] + ".json") as Dictionary<string, object>;
                    if (dict.ContainsKey("dID"))
                        theID = ((int)((long)dict["dID"]));
                    else Debug.LogError("Could not read dialogue ID!");
                }

                d.assignedID = theID;
                d.assignedDialogue = d.diags[d.assignedIndex];
            }
        }
        else
        {
            GUILayout.Label("No saved Dialogues!");

        }
        GUILayout.EndHorizontal();

        GUILayout.Label("Interaction Count: " + d.interactionCount.ToString());
		
        GUILayout.BeginHorizontal();
        GUILayout.Label("Override Start Node: ");
        Undo.RecordObject(d, "Changed override start node");
		d.overrideStartNode = EditorGUILayout.IntField(d.overrideStartNode);
        GUILayout.EndHorizontal();
		
    }

        //Refresh dialogue list
    public void OnFocus()
    {
        if (d != null)
            loadFiles();
    }

        //Refresh dialogue list
    public void loadFiles()
    {
		AssetDatabase.Refresh();
        d = (VIDE_Assign)target;
		
        TextAsset[] files = Resources.LoadAll<TextAsset>("Dialogues");
        d.diags = new List<string>();

        if (files.Length < 1) return;

        foreach (TextAsset f in files)
        {
            d.diags.Add(f.name);
        }

        d.diags.Sort();

        if (d.assignedIndex >= d.diags.Count)
            d.assignedIndex = 0;

        if (d.assignedIndex != -1)
        d.assignedDialogue = d.diags[d.assignedIndex];

        //Lets make sure we still have the right file
        IDCheck();
        Repaint();

    }

    void IDCheck()
    {
        int theID = 0;
        List<int> theIDs = new List<int>();

        if (d.assignedIndex == -1) return;

        if (File.Exists(Application.dataPath + "/" + VIDE_Editor.pathToVide + "/" + d.diags[d.assignedIndex] + ".json"))
        {
            Dictionary<string, object> dict = SerializeHelper.ReadFromFile(d.diags[d.assignedIndex] + ".json") as Dictionary<string, object>;
            if (dict.ContainsKey("dID"))
            {
                theID = ((int)((long)dict["dID"]));
            }
            else { Debug.LogError("Could not read dialogue ID!"); return; }
        }

        if (theID != d.assignedID)
        {
            //Retrieve all IDs
            foreach (string s in d.diags)
            {
                if (File.Exists(Application.dataPath + "/" + VIDE_Editor.pathToVide + "VIDE/Resources/Dialogues/" + s + ".json"))
                {
                    Dictionary<string, object> dict = SerializeHelper.ReadFromFile(s + ".json") as Dictionary<string, object>;
                    if (dict.ContainsKey("dID"))
                        theIDs.Add((int)((long)dict["dID"]));
                }
            }

            var theRealID_Index = theIDs.IndexOf(d.assignedID);
            d.assignedIndex = theRealID_Index;
            if (d.assignedIndex != -1)
                d.assignedDialogue = d.diags[d.assignedIndex];
        }
    }

}
