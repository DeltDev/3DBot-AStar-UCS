using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
public class Node
{
    public bool walkable; //menentukan apakah node/kubus ini obstacle atau bukan. (Jika node ini adalah obstacle, walkabble = false. Jika bukan, walkable = true)
    public Vector3 worldPosition; //Posisi grid sebenarnya di ruang 3 dimensi di Unity
    public int gCost,hCost; //nilai gCost dan hCost
    public int gridX,gridY,gridZ; //Posisi node satuan di grid pada sumbu X,Y,dan Z
    public int fCost{ //nilai fCost = gCost + hCost
        get{
            return gCost+hCost;
        }
    }

    public Node parent; //node sebelumnya untuk konstruksi path
    public Node(bool walkable,Vector3 worldPos,int gridX, int gridY, int gridZ){ //Konstruktor Node
        this.walkable = walkable;
        worldPosition = worldPos;
        this.gridX = gridX;
        this.gridY = gridY;
        this.gridZ = gridZ;
    }
}