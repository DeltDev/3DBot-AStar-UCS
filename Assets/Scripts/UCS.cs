using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UCS : MonoBehaviour
{
    public Transform seeker,target;
    GridCustom grid;

    void Awake(){
        grid = GetComponent<GridCustom>();
    }

    void Update(){
        UCSFindPath(seeker.position,target.position);
    }
void UCSFindPath(Vector3 startPos, Vector3 endPos){
    Node startNode = grid.NodeFromWorldPoint(startPos);
    Node endNode = grid.NodeFromWorldPoint(endPos);

    List<Node> openNode = new List<Node>();
    HashSet<Node> closedNode = new HashSet<Node>();
    openNode.Add(startNode);

    while(openNode.Count > 0){
        Node curNode = openNode[0];

        for(int i = 1; i < openNode.Count; i++){
            if(openNode[i].gCost < curNode.gCost){
                curNode = openNode[i];
            }
        }

        openNode.Remove(curNode);
        closedNode.Add(curNode);

        if(curNode == endNode){
            retracePath(startNode, endNode);
            return;
        }

        foreach(Node neighbor in grid.getNeighbors(curNode)){
            if(!neighbor.walkable || closedNode.Contains(neighbor)){
                continue;
            }

            int costToNeighbor = curNode.gCost + getDist(curNode, neighbor);
            if(costToNeighbor < neighbor.gCost || !openNode.Contains(neighbor)){
                neighbor.gCost = costToNeighbor;
                neighbor.parent = curNode;

                if(!openNode.Contains(neighbor)){
                    openNode.Add(neighbor);
                }
            }
        }
    }
}


    void retracePath(Node startNode, Node endNode){
        List<Node> path = new List<Node>();
        Node curNode = endNode;

        while(curNode != startNode){
            path.Add(curNode);
            curNode = curNode.parent;
        }

        path.Reverse();
        grid.path = path;
    }
    int getDist(Node nodeA, Node nodeB){
        int distX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
        int distY = Mathf.Abs(nodeA.gridY - nodeB.gridY);
        if(distX>distY){
            return 14 * distY + 10 * (distX-distY);
        }

        return 14 * distX + 10 * (distY-distX);
    }
}
