using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
public class GridCustom : MonoBehaviour
{
    public TextMeshProUGUI text;
    public bool onlyDisplayPath;
    public List<Node> path;
    public Transform player;

    public Vector3 gridSize; //ukuran grid
    public float nodeRadius; //jari-jari tiap node (untuk menentukan ukuran tiap node)
    public LayerMask unwalkableLayer; //Layer obstacle yang membuat bot tidak bisa melewati objek ini
    Node[,,] grid; //representasi grid

    float nodeDiameter; //diameter dari node
    int gridSizeX, gridSizeY, gridSizeZ; //gridSize untuk setiap sumbu dalam tipe integer untuk pembuatan array 3 dimensi
    //berfungsi sebagai koordinat untuk setiap kubus pada grid
    
    void Start() //fungsi Start pada Unity untuk melakukan inisialisasi
    {
        nodeDiameter = nodeRadius * 2;
        gridSizeX = Mathf.RoundToInt(gridSize.x / nodeDiameter);
        gridSizeY = Mathf.RoundToInt(gridSize.y / nodeDiameter);
        gridSizeZ = Mathf.RoundToInt(gridSize.z / nodeDiameter);
        CreateGrid();
    }

    void CreateGrid()
    {
        grid = new Node[gridSizeX, gridSizeY, gridSizeZ]; //buat grid baru dengan ukuran gridSizeX *gridSizeY * gridSizeZ
        //tentukan posisi pojok kiri bawah untuk referensi koordinat grid
        Vector3 worldBottomLeft = transform.position - Vector3.right * gridSize.x / 2 - Vector3.forward * gridSize.z / 2 - Vector3.up * gridSize.y / 2; 
        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                for (int z = 0; z < gridSizeZ; z++)
                {
                    //tentukan posisi node pada ruang 3 dimensi di Unity
                    Vector3 worldPoint = worldBottomLeft + Vector3.right * (x * nodeDiameter + nodeRadius) + Vector3.up * (y * nodeDiameter + nodeRadius) + Vector3.forward * (z * nodeDiameter + nodeRadius);
                    //Periksa apakah node tersebut adalah obstacle atau bukan dengan mengecek bola dengan ukuran radius sama dengan radius node
                    bool walkable = !Physics.CheckSphere(worldPoint, nodeRadius, unwalkableLayer);
                    //tambahkan node baru ke grid
                    grid[x, y, z] = new Node(walkable, worldPoint, x, y, z); 
                }
            }
        }
    }

    public Node NodeFromWorldPoint(Vector3 worldPosition) //fungsi untuk mendapatkan node dari posisi sebenarnya di Unity
    {
        float percentX = (worldPosition.x + gridSize.x / 2) / gridSize.x;
        float percentY = (worldPosition.y + gridSize.y / 2) / gridSize.y;
        float percentZ = (worldPosition.z + gridSize.z / 2) / gridSize.z;
        percentX = Mathf.Clamp01(percentX);
        percentY = Mathf.Clamp01(percentY);
        percentZ = Mathf.Clamp01(percentZ);

        int x = Mathf.RoundToInt((gridSizeX - 1) * percentX);
        int y = Mathf.RoundToInt((gridSizeY - 1) * percentY);
        int z = Mathf.RoundToInt((gridSizeZ - 1) * percentZ);
        return grid[x, y, z]; 
    }

    public List<Node> getNeighbors(Node node) //fungsi untuk mendapatkan semua node neighbor dari node posisi pemain saat ini di Unity
    {
        List<Node> neighbors = new List<Node>();

        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                for (int z = -1; z <= 1; z++)
                {
                    if (x == 0 && y == 0 && z == 0)
                    {
                        continue;
                    }
                    int checkX = node.gridX + x;
                    int checkY = node.gridY + y;
                    int checkZ = node.gridZ + z;
                    if (checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY && checkZ >= 0 && checkZ < gridSizeZ)
                    {
                        neighbors.Add(grid[checkX, checkY, checkZ]); 
                    }
                }
            }
        }
        return neighbors;
    }
    void OnDrawGizmos()
    {
        Node PlayerNode = NodeFromWorldPoint(player.position);
        Gizmos.DrawWireCube(transform.position, new Vector3(gridSize.x, gridSize.y, gridSize.z)); 

        if (grid != null)
        {
            foreach (Node n in grid)
            {
                Gizmos.color = Color.clear;
                if (!onlyDisplayPath)
                {
                    Gizmos.color = (n.walkable) ? Color.white : Color.red;
                }
                if (path != null)
                {
                    if (path.Contains(n))
                    {
                        Gizmos.color = Color.black;
                    }
                }
                if (PlayerNode == n)
                {
                    Gizmos.color = Color.cyan;
                }
                Gizmos.DrawCube(n.worldPosition, Vector3.one * (nodeDiameter - .1f));
            }
        }
    }
}
