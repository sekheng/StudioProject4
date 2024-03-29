﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Pathfinding : MonoBehaviour {

    public Transform seeker, target;
    //public Grid GridForPathfinding;
    Grid grid;

    void Start()
    {//PathfindingAStarGrid
        grid = GameObject.FindGameObjectWithTag("PathfindingAStarGrid").GetComponent<Grid>();
    }

    void Update()
    {
        while(grid == null)
        {
            grid = GameObject.FindGameObjectWithTag("PathfindingAStarGrid").GetComponent<Grid>();
        }
        if (seeker != null && target != null)
        {
            //FindPath(seeker.position, target.position);
        }
    }

	public void FindPath(Vector2 startPos, Vector2 targetPos)
    {
        Node startNode = grid.NodeFromWorldPoint(startPos);
        Node targetNode = grid.NodeFromWorldPoint(targetPos);

        Heap<Node> openSet = new Heap<Node>(grid.MaxSize);
        HashSet<Node> closeSet = new HashSet<Node>();

        openSet.Add(startNode);
        
        //f(n) = g(n) + h(n)

        while(openSet.Count > 0)
        {
            //Node currNode = openSet[0];
            Node currNode = openSet.RemoveFirst();
            //for (int i = 1; i < openSet.Count; ++i)
            //{
            //    if(openSet[i].fCost < currNode.fCost/*if the openset node's cost is lower*/ || openSet[i].fCost == currNode.fCost && openSet[i].hCost < currNode.hCost/*if same fcost, check which has lower hCost*/)
            //    {
            //        currNode = openSet[i];
            //    }
            //}

            //openSet.Remove(currNode);
            closeSet.Add(currNode);

            if(currNode == targetNode)//reached
            {
                RetracePath(startNode, targetNode);
                return;
            }

            foreach(Node neighbour in grid.GetNeighbours(currNode))
            {
                if(!neighbour.m_bWalkable || closeSet.Contains(neighbour))
                {
                    continue;
                }
                int newMovementCostTONeighbour = currNode.gCost + GetDistance(currNode, neighbour);
                if(newMovementCostTONeighbour < neighbour.gCost || !openSet.Contains(neighbour))
                {
                    neighbour.gCost = newMovementCostTONeighbour;
                    neighbour.hCost = GetDistance(neighbour, targetNode);
                    neighbour.parent = currNode;

                    if(!openSet.Contains(neighbour))
                    {
                        openSet.Add(neighbour);
                    }
                    openSet.UpdateItem(neighbour);
                }
            }
        }
    }

    void RetracePath(Node startNode, Node endNode)// used to get the path after finding the target
    {
        List<Node> path = new List<Node>();

        Node currNode = endNode;

        while(currNode != startNode)
        {
            path.Add(currNode);
            currNode = currNode.parent;
        }
        path.Reverse();//chnge from target to start to start to target

        grid.path = path;
    }

    int GetDistance(Node nodeA, Node nodeB)// how many nodes in the x axis and y axis
    {
        int distX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
        int distY = Mathf.Abs(nodeA.gridY - nodeB.gridY);

        if (distX > distY)
            return 14 * distY + 10 * (distX - distY);
        return 14* distX +10 * (distY - distX);
    }

    public List<Node> getPath()
    {
        return grid.path;
    }

    public void setSeeker(Transform transform)
    {
        seeker = transform;
    }
    public void setTarget(Transform transform)
    {
        target = transform;
    }

    public Transform getSeeker()
    {
        return seeker ;
    }
    public Transform getTarget()
    {
        return target;
    }

    public Grid getGrid()
    {
        return grid;
    }
}
