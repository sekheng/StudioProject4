﻿using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Linq;

public class VIDE_Data : MonoBehaviour
{
    /*
     * This component is the source of all data you'll be needing to populate your dialogue interface.
     * It will manage the flow of the conversation based on the current node data stored in a variable called nodeData.
     * All you need is to attach this component to the game object that will manage your dialogue UI.
     * Then call BeginDialogue() on it to begin the conversation with an NPC. 
     * The rest is up to the Next() method to advance in the conversation up until you call EndDialogue()
     * Check exampleUI.cs for an already-setup UI manager example.
     */
    public int assignedIndex = 0;

    private List<CommentSet> playerNodes = new List<CommentSet>();
    private List<Answer> npcNodes = new List<Answer>();
    private List<ActionNode> actionNodes = new List<ActionNode>();	
    private Answer currentNPCStep;
    private CommentSet currentPlayerStep;
    private ActionNode currentActionNode;
    private ActionNode lastActionNode;
    private bool jumped = false;
    private int startPoint = -1;
	
    public VIDE_Assign assigned;
    public bool isLoaded;
    public NodeData nodeData;


    //The class that contains all of the node variables
    public class NodeData
    {
        public bool currentIsPlayer;
		public bool pausedAction;
        public bool isEnd;
        public int nodeID;
        public string[] playerComments;
        public string[] playerCommentExtraData;
        public string[] npcComment;
        public int npcCommentIndex;
        public int selectedOption;
        public string extraData;
        public string tag;
        public string playerTag;

        public NodeData(bool isP, bool isE, int id, string[] pt, string[] pcExtraD, string[] npt, string exData, string tagt, string ptag)
        {
            currentIsPlayer = isP;
            isEnd = isE;
            nodeID = id;
            playerComments = pt;
            playerCommentExtraData = pcExtraD;
            npcComment = npt;
            npcCommentIndex = 0;
            selectedOption = 0;
            extraData = exData;
            tag = tagt;
            playerTag = ptag;
			pausedAction = false;
        }
		
		public NodeData(){
			currentIsPlayer = true;
            isEnd = false;
            nodeID = -1;
            selectedOption = 0;
		}

    }

    /// <summary>
    /// Ignores current nodeData state and jumps directly to the specified node.
    /// </summary>
    /// <returns>
    /// The node.
    /// </returns>
    /// <param name='id'>
    /// The ID of your Node. Get it from the Dialogue Editor.
    /// </param>
    public NodeData SetNode(int id)
    {
        if (!isLoaded)
        {
            Debug.LogError("You must call the 'BeginDialogue()' method before calling the 'Next()' method!");
            return null;
        }

        //Look for Node with given ID
        bool foundID = false;
        bool isPl = false;
		bool isAct = false;
        for (int i = 0; i < playerNodes.Count; i++)
        {
            if (playerNodes[i].ID == id)
            {
                currentPlayerStep = playerNodes[i];
                isPl = true;
                foundID = true;
            }
        }
        if (!foundID)
        {
            for (int i = 0; i < npcNodes.Count; i++)
            {
                if (npcNodes[i].ID == id)
                {
                    currentNPCStep = npcNodes[i];
                    foundID = true;
                }
            }
        }
        if (!foundID)
		{
            for (int i = 0; i < actionNodes.Count; i++)
            {
                if (actionNodes[i].ID == id)
                {
                    currentActionNode = actionNodes[i];
                    foundID = true;
					isAct = true;
                }
            }
        }
		if (!foundID)
        {
            Debug.LogError("Could not find a Node with ID " + id.ToString());
            return null;
        }
		
		/* Action node */
		
		if (isAct){
			lastActionNode = currentActionNode;
			nodeData = new NodeData();
            DoAction();
            return nodeData;
		}
		
		/* Action end */

        if (isPl)
        {
            nodeData = new NodeData(true, false, currentPlayerStep.ID, GetOptions(), GetExtraData(), null, null, null, currentPlayerStep.playerTag);
            jumped = true;
            return nodeData;
        }
        else
        {
            List<string> ns = new List<string>();

            string[] rawSplit = Regex.Split(currentNPCStep.text, "<br>");
            foreach (string s in rawSplit)
            {
                if (s != "" && s != " ") ns.Add(s.Trim());
            }

            nodeData = new NodeData(isPl, false, id, null, null, ns.ToArray(), currentNPCStep.extraData, currentNPCStep.tag, "");
            return nodeData;
        }
    }
    /// <summary>
    /// Populates nodeData with the data from next Node based on the current nodeData.
    /// </summary>
    /// <returns></returns>
    public NodeData Next()
    {
        if (!isLoaded)
        {
            Debug.LogError("You must call the 'BeginDialogue()' method before calling the 'Next()' method!");
            return null;
        }

        int option = 0;
        bool nextIsPlayer = true;
		
		if (nodeData != null)
        option = nodeData.selectedOption;

        if (!jumped && nodeData != null) //Here's where we check if we end
        {
            if (!nodeData.currentIsPlayer && currentNPCStep != null)
            {
                if (currentNPCStep.outputNPC == null && currentNPCStep.outputSet == null && currentNPCStep.outAction == null && nodeData.npcCommentIndex == nodeData.npcComment.Length - 1)
                {
                    nodeData.isEnd = true;
                    isLoaded = false;
                    LocalDataSingleton.instance.talking = false;
                    LocalDataSingleton.instance.MainCanvas.transform.GetChild(1).GetChild(0).gameObject.SetActive(false);
                    LocalDataSingleton.instance.Inventorycanvas.SetActive(false);
                    return nodeData;
                }
                else if (currentNPCStep.outputNPC == null && currentNPCStep.outputSet == null && currentNPCStep.outAction == null && nodeData.npcComment.Length < 1)
                {
                    nodeData.isEnd = true;
                    isLoaded = false;
                    LocalDataSingleton.instance.talking = false;
                    LocalDataSingleton.instance.MainCanvas.transform.GetChild(1).GetChild(0).gameObject.SetActive(false);
                    LocalDataSingleton.instance.Inventorycanvas.SetActive(false);
                    return nodeData;
                }
            }
        }
		
		if (nodeData != null)
        if (nodeData.currentIsPlayer)
        {
            //Mark end of conversation for player node
			if (currentPlayerStep != null)
            if (currentPlayerStep.comment[option].outputAnswer == null && currentPlayerStep.comment[option].outAction == null)
            {
                nodeData.isEnd = true;
                isLoaded = false;
                LocalDataSingleton.instance.talking = false;
                LocalDataSingleton.instance.MainCanvas.transform.GetChild(1).GetChild(0).gameObject.SetActive(false);
                LocalDataSingleton.instance.Inventorycanvas.SetActive(false);
                return nodeData;
            }
        }
        //If action node is connected to nothing, then it's the end
        if (lastActionNode != null)
        {
            if (lastActionNode.outPlayer == null && lastActionNode.outNPC == null && lastActionNode.outAction == null)
            {
				nodeData.isEnd = true;
                isLoaded = false;
                LocalDataSingleton.instance.talking = false;
                LocalDataSingleton.instance.MainCanvas.transform.GetChild(1).GetChild(0).gameObject.SetActive(false);
                LocalDataSingleton.instance.Inventorycanvas.SetActive(false);
                return nodeData;
            }
        }

        jumped = false;

        /* Action Node? */
        
        if (currentActionNode == null)
        {
            if (nodeData.currentIsPlayer)
            {
                currentActionNode = currentPlayerStep.comment[option].outAction;
            }
            else
            {
                currentActionNode = currentNPCStep.outAction;
            }
        } else
        {
            currentActionNode = currentActionNode.outAction;
        }
		
        //If we found actio node, let's go to it.
        if (currentActionNode != null)
        {
            lastActionNode = currentActionNode;
            DoAction();
            return nodeData;
        } else if (lastActionNode != null) {
			if (lastActionNode.outNPC != null){
			}
		}

        /* END Action Node */

        if (!nodeData.currentIsPlayer)
        {
            nextIsPlayer = true;
            if (currentNPCStep.outputSet == null)
            {
                nextIsPlayer = false;
            }
        }
        else
        {
            nextIsPlayer = false;
        }

        if (!nodeData.currentIsPlayer) // WE ARE CURRENTLY NPC NODE
        {
            //Let's scroll through split comments first
            if (nodeData.npcComment.Length > 0)
            {
                if (nodeData.npcCommentIndex != nodeData.npcComment.Length - 1)
                {
                    nodeData.npcCommentIndex++;
					lastActionNode = null;
                    return nodeData;
                }
            }
			
			if (lastActionNode != null)
            if (lastActionNode.outNPC != null)
            {
                currentNPCStep = lastActionNode.outNPC;

                List<string> ns = new List<string>();
                string[] rawSplit = Regex.Split(currentNPCStep.text, "<br>");
                foreach (string s in rawSplit)
                { if (s != "" && s != " ") ns.Add(s.Trim()); }

				lastActionNode = null;
                nodeData = new NodeData(false, false, currentNPCStep.ID, null, null, ns.ToArray(), currentNPCStep.extraData, currentNPCStep.tag, "");
                return nodeData;
            }

			if (lastActionNode != null)
            if (lastActionNode.outPlayer != null)
            {
                currentPlayerStep = lastActionNode.outPlayer;
				lastActionNode = null;
                nodeData = new NodeData(true, false, currentPlayerStep.ID, GetOptions(), GetExtraData(), null, null, null, currentPlayerStep.playerTag);
                return nodeData;
            }


            if (nextIsPlayer) // NEXT IS PLAYER
            {
				lastActionNode = null;
                currentPlayerStep = currentNPCStep.outputSet;
                nodeData = new NodeData(true, false, currentPlayerStep.ID, GetOptions(), GetExtraData(), null, null, null, currentPlayerStep.playerTag);
            }
            else // NEXT IS ANOTHER NPC NODE
            {
                currentNPCStep = currentNPCStep.outputNPC;
                List<string> ns = new List<string>();

                string[] rawSplit = Regex.Split(currentNPCStep.text, "<br>");
                foreach (string s in rawSplit)
                { if (s != "" && s != " ") ns.Add(s.Trim()); }

				lastActionNode = null;
                nodeData = new NodeData(false, false, currentNPCStep.ID, null, null, ns.ToArray(), currentNPCStep.extraData, currentNPCStep.tag, "");
            }

            return nodeData;
        }
        else // WE ARE CURRENTLY PLAYER NODE
        {
            //Pick next NPC node based on player choice OR next Player Node if there was an Action Node beforehand
            if (lastActionNode == null)
            {
                currentNPCStep = currentPlayerStep.comment[option].outputAnswer;
            }
            else
            {
                if (lastActionNode.outNPC != null) currentNPCStep = lastActionNode.outNPC;
                if (lastActionNode.outPlayer != null)
                {
                    currentPlayerStep = lastActionNode.outPlayer;
					lastActionNode = null;				
                    nodeData = new NodeData(true, false, currentPlayerStep.ID, GetOptions(), GetExtraData(), null, null, null, currentPlayerStep.playerTag);
                    return nodeData;
                }
            }

            List<string> ns = new List<string>();

            string[] rawSplit = Regex.Split(currentNPCStep.text, "<br>");
            foreach (string s in rawSplit)
            { if (s != "" && s != " ") ns.Add(s.Trim()); }
			
			lastActionNode = null;
            nodeData = new NodeData(false, false, currentNPCStep.ID, null, null, ns.ToArray(), currentNPCStep.extraData, currentNPCStep.tag, "");
            return nodeData;
        }
    }

    void DoAction()
    {		
		var objects = Resources.FindObjectsOfTypeAll<GameObject>().Where(obj => obj.name == currentActionNode.gameObjectName);
		
		foreach(GameObject g in objects){
			if (currentActionNode.paramType < 1)
	        	g.SendMessage(currentActionNode.methodName, SendMessageOptions.DontRequireReceiver);
	        if (currentActionNode.paramType == 1)
	            g.SendMessage(currentActionNode.methodName, currentActionNode.param_bool, SendMessageOptions.DontRequireReceiver);
	        if (currentActionNode.paramType == 2)
	            g.SendMessage(currentActionNode.methodName, currentActionNode.param_string, SendMessageOptions.DontRequireReceiver);
	        if (currentActionNode.paramType == 3)
	            g.SendMessage(currentActionNode.methodName, currentActionNode.param_int, SendMessageOptions.DontRequireReceiver);
	        if (currentActionNode.paramType == 4)
	            g.SendMessage(currentActionNode.methodName, currentActionNode.param_float, SendMessageOptions.DontRequireReceiver);		  
		}
		
        if (!currentActionNode.pauseHere)
        {
            Next();
        } else {
			nodeData.pausedAction = true;	
		}
    }

    /// <summary>
    /// Loads up the dialogue just sent. Populates the nodeData variable with the first Node based on the Start Node. Also returns the current NodeData package.
    /// </summary>
    /// <param name="diagToLoad"></param>
    /// <returns>NodeData</returns>
    public NodeData BeginDialogue(VIDE_Assign diagToLoad)
    {
        if (diagToLoad.assignedIndex < 0 || diagToLoad.assignedIndex > diagToLoad.diags.Count-1)
        {
            Debug.LogError("No dialogue assigned to VIDE_Assign!");
            return null;
        }

        //First we load the dialogue we requested
        if (Load(diagToLoad.diags[diagToLoad.assignedIndex]))
        {
            isLoaded = true;
        }
        else
        {
            isLoaded = false;
            Debug.LogError("Failed to load '" + diagToLoad.diags[diagToLoad.assignedIndex] + "'");
            return null;
        }

        //Make sure that variables were correctly reset after last conversation
        if (nodeData != null)
        {
            Debug.LogError("You forgot to call 'EndDialogue()' on last conversation!");
            return null;
        }

        assigned = diagToLoad;

        if (assigned.overrideStartNode != -1)
            startPoint = assigned.overrideStartNode;

        int startIndex = -1;
        bool isPlayer = false;
        bool isAct = false;

        for (int i = 0; i < npcNodes.Count; i++)
            if (startPoint == npcNodes[i].ID) { startIndex = i; isPlayer = false; break; }
        for (int i = 0; i < playerNodes.Count; i++)
            if (startPoint == playerNodes[i].ID) { startIndex = i; isPlayer = true; break; }
        for (int i = 0; i < actionNodes.Count; i++)
            if (startPoint == actionNodes[i].ID) { startIndex = i; 
			currentActionNode = actionNodes[i]; isPlayer = true; isAct = true; break; }
		
		/* Action node */
		
		if (isAct){
			lastActionNode = currentActionNode;
			nodeData = new NodeData();
            DoAction();
            return nodeData;
		}
		
		/* Action end */

        if (startIndex == -1)
        {
            Debug.LogError("Start point not found! Check your IDs!");
            return null;
        }

        if (isPlayer)
        {
            currentPlayerStep = playerNodes[startIndex];

			lastActionNode = null;
            nodeData = new NodeData(true, false, currentPlayerStep.ID, GetOptions(), GetExtraData(), null, null, null, currentPlayerStep.playerTag);
            return nodeData;

        }
        else
        {
            currentNPCStep = npcNodes[startIndex];

            List<string> ns = new List<string>();

            string[] rawSplit = Regex.Split(currentNPCStep.text, "<br>");
            foreach (string s in rawSplit)
            { if (s != "" && s != " ") ns.Add(s.Trim()); }

			lastActionNode = null;
            nodeData = new NodeData(false, false, currentNPCStep.ID, null, null, ns.ToArray(), currentNPCStep.extraData, currentNPCStep.tag, "");
            return nodeData;
        }


    }
    /// <summary>
    /// Wipes out all data and unloads the current VIDE_Assign, raising its interactionCount.
    /// </summary>
    public void EndDialogue()
    {
        nodeData = null;
        if (assigned != null)
        assigned.interactionCount++;
        assigned = null;
		startPoint = -1;
        playerNodes = new List<CommentSet>(); ;
        npcNodes = new List<Answer>(); ;
        actionNodes = new List<ActionNode>(); ;
        isLoaded = false;
        currentNPCStep = null;
        currentPlayerStep = null;
        currentActionNode = null;
        lastActionNode = null;
    }

    private string[] GetOptions()
    {
        List<string> op = new List<string>();

        if (currentPlayerStep == null)
        {
            return op.ToArray();
        }

        for (int i = 0; i < currentPlayerStep.comment.Count; i++)
        {
            op.Add(currentPlayerStep.comment[i].text);
        }

        return op.ToArray();
    }

    private string[] GetExtraData()
    {
        List<string> op = new List<string>();

        if (currentPlayerStep == null)
        {
            return op.ToArray();
        }

        for (int i = 0; i < currentPlayerStep.comment.Count; i++)
        {
            if (currentPlayerStep.comment[i].extraData.Length > 0)
                op.Add(currentPlayerStep.comment[i].extraData);
            else
                op.Add(string.Empty);
        }

        return op.ToArray();
    }

    //The following are all of the classes and methods we need for constructing the nodes
    class SerializeHelper
    {
        //static string fileDataPath = Application.dataPath + "/VIDE/dialogues/";
        public static object ReadFromFile(string filename)
        {
            string jsonString = Resources.Load<TextAsset>("Dialogues/" + filename).text;
            return MiniJSON_VIDE.DiagJson.Deserialize(jsonString);
        }
    }

    class CommentSet
    {
        public List<Comment> comment;
        public int ID;
        public string playerTag;
        public bool endConversation = false;

        public CommentSet(int comSize, int id, string tag)
        {
            comment = new List<Comment>();
            ID = id;
            playerTag = tag;
            for (int i = 0; i < comSize; i++)
                comment.Add(new Comment());
        }
    }

    class Comment
    {
        public string text;
        public string extraData;
        public CommentSet inputSet;
        public ActionNode outAction;
        public Answer outputAnswer;

        public Comment()
        {
            text = "";
            extraData = "";
        }
        public Comment(CommentSet id)
        {
            outputAnswer = null;
            inputSet = id;
            text = "Comment...";
            extraData = "ExtraData...";
        }
    }

    class Answer
    {
        public string text;
        public CommentSet outputSet;
        public Answer outputNPC;
        public ActionNode outAction;

        public string extraData;
        public string tag;

        public int ID;

        public Answer(string t, int id, string exD, string tagt)
        {
            text = t;
            outputSet = null;
            outputNPC = null;
            extraData = exD;
            tag = tagt;
            ID = id;
        }

    }

    class ActionNode
    {
        public bool pauseHere = false;
        public string gameObjectName;
        public string methodName;
        public int paramType;

        public bool param_bool;
        public string param_string;
        public int param_int;
        public float param_float;

        public int ID;
        public CommentSet outPlayer;
        public Answer outNPC;
        public ActionNode outAction;

        public ActionNode(int id, string meth, string goMeth, bool pau, bool pb, string ps, int pi, float pf)
        {
            pauseHere = pau;
            methodName = meth;
            gameObjectName = goMeth;

            param_bool = pb;
            param_string = ps;
            param_int = pi;
            param_float = pf;

            outPlayer = null;
            outNPC = null;
            outAction = null;
            ID = id;
        }

    }

    void addComment(CommentSet id)
    {
        id.comment.Add(new Comment(id));
    }

    void addAnswer(string t, int id, string exD, string tagt)
    {
        npcNodes.Add(new Answer(t, id, exD, tagt));
    }

    void addSet(int cSize, int id, string tag)
    {
        playerNodes.Add(new CommentSet(cSize, id, tag));
    }

    //This method will load the dialogue from the DialogueAssign component sent.
    bool Load(string dName)
    {
        playerNodes = new List<CommentSet>();
        npcNodes = new List<Answer>();
        actionNodes = new List<ActionNode>();

        if (Resources.Load("Dialogues/" + dName) == null) return false;

        Dictionary<string, object> dict = SerializeHelper.ReadFromFile(dName) as Dictionary<string, object>;

        int pDiags = (int)((long)dict["playerDiags"]);
        int nDiags = (int)((long)dict["npcDiags"]);
        int aDiags = 0;
        if (dict.ContainsKey("actionNodes")) aDiags = (int)((long)dict["actionNodes"]);

        startPoint = (int)((long)dict["startPoint"]);


        //Create first...
        for (int i = 0; i < pDiags; i++)
        {
            string tagt = "";

            if (dict.ContainsKey("pd_pTag_" + i.ToString()))
                tagt = (string)dict["pd_pTag_" + i.ToString()];


            addSet(
                (int)((long)dict["pd_comSize_" + i.ToString()]),
                (int)((long)dict["pd_ID_" + i.ToString()]),
                tagt
                );
        }

        for (int i = 0; i < nDiags; i++)
        {
            string tagt = "";

            if (dict.ContainsKey("nd_tag_" + i.ToString()))
                tagt = (string)dict["nd_tag_" + i.ToString()];

            addAnswer(
                (string)dict["nd_text_" + i.ToString()],
                (int)((long)dict["nd_ID_" + i.ToString()]),
                (string)dict["nd_extraData_" + i.ToString()],
                tagt
                );
        }
        for (int i = 0; i < aDiags; i++)
        {
            float pFloat;
            var pfl = dict["ac_pFloat_" + i.ToString()];
            if (pfl.GetType() == typeof(System.Double))
                pFloat = System.Convert.ToSingle(pfl);
            else
                pFloat = (float)(long)pfl;


            actionNodes.Add(new ActionNode(
                (int)((long)dict["ac_ID_" + i.ToString()]),
                (string)dict["ac_meth_" + i.ToString()],
                (string)dict["ac_goName_" + i.ToString()],
                (bool)dict["ac_pause_" + i.ToString()],
                (bool)dict["ac_pBool_" + i.ToString()],
                (string)dict["ac_pString_" + i.ToString()],
                (int)((long)dict["ac_pInt_" + i.ToString()]),
                pFloat
                ));
        }

        //Connect now...
        for (int i = 0; i < playerNodes.Count; i++)
        {
            for (int ii = 0; ii < playerNodes[i].comment.Count; ii++)
            {
                playerNodes[i].comment[ii].text = (string)dict["pd_" + i.ToString() + "_com_" + ii.ToString() + "text"];

                if (dict.ContainsKey("pd_" + i.ToString() + "_com_" + ii.ToString() + "extraD"))
                    playerNodes[i].comment[ii].extraData = (string)dict["pd_" + i.ToString() + "_com_" + ii.ToString() + "extraD"];

                int index = (int)((long)dict["pd_" + i.ToString() + "_com_" + ii.ToString() + "iSet"]);

                if (index != -1)
                    playerNodes[i].comment[ii].inputSet = playerNodes[index];

                index = (int)((long)dict["pd_" + i.ToString() + "_com_" + ii.ToString() + "oAns"]);

                if (index != -1)
                    playerNodes[i].comment[ii].outputAnswer = npcNodes[index];

                index = -1;
                if (dict.ContainsKey("pd_" + i.ToString() + "_com_" + ii.ToString() + "oAct"))
                    index = (int)((long)dict["pd_" + i.ToString() + "_com_" + ii.ToString() + "oAct"]);

                if (index != -1)
                    playerNodes[i].comment[ii].outAction = actionNodes[index];
            }
        }

        for (int i = 0; i < npcNodes.Count; i++)
        {
            int index = (int)((long)dict["nd_oSet_" + i.ToString()]);
            if (index != -1)
                npcNodes[i].outputSet = playerNodes[index];

            if (dict.ContainsKey("nd_oNPC_" + i.ToString()))
            {
                int index2 = (int)((long)dict["nd_oNPC_" + i.ToString()]);
                if (index2 != -1)
                    npcNodes[i].outputNPC = npcNodes[index2];
            }

            if (dict.ContainsKey("nd_oAct_" + i.ToString()))
            {
                index = -1;
                index = (int)((long)dict["nd_oAct_" + i.ToString()]);
                if (index != -1)
                    npcNodes[i].outAction = actionNodes[index];
            }
        }

        for (int i = 0; i < actionNodes.Count; i++)
        {
            actionNodes[i].paramType = (int)((long)dict["ac_paramT_" + i.ToString()]);

            int index = -1;
            index = (int)((long)dict["ac_oSet_" + i.ToString()]);

            if (index != -1)
                actionNodes[i].outPlayer = playerNodes[index];

            if (dict.ContainsKey("ac_oNPC_" + i.ToString()))
            {
                index = -1;
                index = (int)((long)dict["ac_oNPC_" + i.ToString()]);
                if (index != -1)
                    actionNodes[i].outNPC = npcNodes[index];
            }

            if (dict.ContainsKey("ac_oAct_" + i.ToString()))
            {
                index = -1;
                index = (int)((long)dict["ac_oAct_" + i.ToString()]);
                if (index != -1)
                    actionNodes[i].outAction = actionNodes[index];
            }
        }

        return true;
    }
	
    //Return the default start node
    public int startNode
    {
        get
        {
            return startPoint;
        }
    }

    public string GetFirstTag(bool searchPlayer)
    {
		if (!isLoaded){
			Debug.LogError("No dialogue loaded!");
			return string.Empty;
		}
		
        string firstTag = string.Empty;
        if (searchPlayer)
        {
            for (int i = 0; i < playerNodes.Count; i++)
            {
                firstTag = playerNodes[i].playerTag;
                if (!string.IsNullOrEmpty(firstTag))
                    break;

            }
        }
        else
        {
            for (int i = 0; i < npcNodes.Count; i++)
            {
                firstTag = npcNodes[i].tag;
                if (!string.IsNullOrEmpty(firstTag))
                    break;

            }
        }
        return firstTag;
    }
}



