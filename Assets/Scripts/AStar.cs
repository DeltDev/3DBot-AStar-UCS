using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;
using System.IO;
public class AStar : MonoBehaviour
{
    public Transform seeker, target;
    GridCustom grid;
    public bool isUsingUCS;
    Node startNode;
    Node endNode;
    List<Node> openNode;
    HashSet<Node> closedNode;
    void Awake()
    {
        grid = GetComponent<GridCustom>();
    }

    void Update()
    {
        FindPath(seeker.position, target.position);
    }

    void FindPath(Vector3 startPos, Vector3 endPos)
    {
        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();
        if(!isUsingUCS){
             // A*
            startNode = grid.NodeFromWorldPoint(startPos); //node awal
            endNode = grid.NodeFromWorldPoint(endPos); //node tujuan

            openNode = new List<Node>(); //node yang belum dikunjungi
            closedNode = new HashSet<Node>(); // node yang sudah dikunjungi
            openNode.Add(startNode);//masukkan ke open node

            while (openNode.Count > 0)
            {
                Node curNode = openNode[0]; //node sekarang

                for (int i = 1; i < openNode.Count; i++)
                {
                    if (openNode[i].fCost <= curNode.fCost && openNode[i].hCost < curNode.gCost)
                    { // ganti node sekarang (A*)
                        curNode = openNode[i];
                    }
                }

                openNode.Remove(curNode);
                closedNode.Add(curNode);
                // pindahkan node sekarang ke closed node
                if (curNode == endNode)
                { //sudah ketemu pathnya
                    List<Node> path = retracePath(startNode, endNode); //bangun ulang pathnya
                    //hentikan pencarian
                    stopwatch.Stop();
                    UnityEngine.Debug.Log("Waktu eksekusi A*: " + stopwatch.ElapsedMilliseconds + " ms");
                    seeker.GetComponent<SeekerMovement>().SetPath(path); //gerakkan agentnya
                    return;
                }

                foreach (Node neighbor in grid.getNeighbors(curNode))
                { //periksa semua tetangga dari node yang sekarang
                    if (!neighbor.walkable || closedNode.Contains(neighbor))
                    { //node tidak bisa dilangkahi (adalah obstacle) atau nodenya sudah closed
                        continue; //lewati
                    }

                    int costToNeighbor = curNode.gCost + getDist(curNode, neighbor); //hitung jarak dari titik ke neighbor yang sekarang
                    if (costToNeighbor < neighbor.gCost || !openNode.Contains(neighbor))
                    {
                        neighbor.gCost = costToNeighbor; //ubah gCost neighbor
                        neighbor.hCost = getDist(neighbor, endNode); //hitung heuristik (jarak dari titik akhir ke neighbor)
                        neighbor.parent = curNode;

                        if (!openNode.Contains(neighbor))
                        {
                            openNode.Add(neighbor);
                        }
                    }
                }
            }
            stopwatch.Stop();
            UnityEngine.Debug.Log("Waktu eksekusi A*: " + stopwatch.ElapsedMilliseconds + " ms");
            return;
        }
       
    
        //UCS
        startNode = grid.NodeFromWorldPoint(startPos); //node awal
        endNode = grid.NodeFromWorldPoint(endPos); //node tujuan

        openNode = new List<Node>(); //node yang belum dikunjungi
        closedNode = new HashSet<Node>(); // node yang sudah dikunjungi
        openNode.Add(startNode);//masukkan ke open node

        while (openNode.Count > 0)
        {
            Node curNode = openNode[0]; //node sekarang

            for (int i = 1; i < openNode.Count; i++)
            {
                if (openNode[i].gCost < curNode.gCost)
                { // ganti node sekarang (UCS)
                    curNode = openNode[i];
                }
            }

            openNode.Remove(curNode);
            closedNode.Add(curNode);
            // pindahkan node sekarang ke closed node
            if (curNode == endNode)
            { //sudah ketemu pathnya
                List<Node> path = retracePath(startNode, endNode); //bangun ulang pathnya
                //hentikan pencarian
                stopwatch.Stop();
                UnityEngine.Debug.Log("Waktu eksekusi UCS: " + stopwatch.ElapsedMilliseconds + " ms");
                seeker.GetComponent<SeekerMovement>().SetPath(path); //gerakkan agentnya
                return;
            }

            foreach (Node neighbor in grid.getNeighbors(curNode))
            { //periksa semua tetangga dari node yang sekarang
                if (!neighbor.walkable || closedNode.Contains(neighbor))
                { //node tidak bisa dilangkahi (adalah obstacle) atau nodenya sudah closed
                    continue; //lewati
                }

                int costToNeighbor = curNode.gCost + getDist(curNode, neighbor); //hitung jarak dari titik ke neighbor yang sekarang
                if (costToNeighbor < neighbor.gCost || !openNode.Contains(neighbor))
                {
                    neighbor.gCost = costToNeighbor; //ubah gCost neighbor
                    neighbor.parent = curNode;

                    if (!openNode.Contains(neighbor))
                    {
                        openNode.Add(neighbor);
                    }
                }
            }
        }
        stopwatch.Stop();
        UnityEngine.Debug.Log("Waktu eksekusi UCS: " + stopwatch.ElapsedMilliseconds + " ms");
    }

    List<Node> retracePath(Node startNode, Node endNode) //fungsi untuk mendapatkan path dari awal sampai akhir
    {
        List<Node> path = new List<Node>();
        Node curNode = endNode;

        while (curNode != startNode)
        {
            path.Add(curNode);
            curNode = curNode.parent;
        }

        path.Reverse();
        grid.path = path;
        return path;
    }

    int getDist(Node nodeA, Node nodeB)//fungsi untuk menghitung jarak antar node
    {
        //Pembobotan jarak sudah di bahas pada bagian III.B.
        int distX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
        int distY = Mathf.Abs(nodeA.gridY - nodeB.gridY);
        int distZ = Mathf.Abs(nodeA.gridZ - nodeB.gridZ);

        int maxDist = Mathf.Max(distX, distY, distZ);
        int midDist = distX + distY + distZ - maxDist - Mathf.Min(distX, distY, distZ);
        int minDist = Mathf.Min(distX, distY, distZ);

        return 17 * minDist + 14 * (midDist - minDist) + 10 * (maxDist - midDist);
    }
}