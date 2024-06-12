using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeekerMovement : MonoBehaviour
{
    public float speed = 5f;
    private List<Node> path;
    private int targetIndex;

    public void SetPath(List<Node> newPath)
    {
        path = newPath;
        StopCoroutine("FollowPath");
        StartCoroutine("FollowPath");
    }

    IEnumerator FollowPath()
    {
        if (path == null || path.Count == 0)
        {
            yield break;
        }

        Vector3 currentWaypoint = path[0].worldPosition;
        targetIndex = 0;

        while (true)
        {
            if (transform.position == currentWaypoint)
            {
                targetIndex++;
                if (targetIndex >= path.Count)
                {
                    yield break;
                }
                currentWaypoint = path[targetIndex].worldPosition;
            }

            transform.position = Vector3.MoveTowards(transform.position, currentWaypoint, speed * Time.deltaTime);
            yield return null;
        }
    }

    void OnDrawGizmos()
    {
        if (path != null)
        {
            for (int i = targetIndex; i < path.Count; i++)
            {
                Gizmos.color = Color.black;
                Gizmos.DrawCube(path[i].worldPosition, Vector3.one);

                if (i == targetIndex)
                {
                    Gizmos.DrawLine(transform.position, path[i].worldPosition);
                }
                else
                {
                    Gizmos.DrawLine(path[i - 1].worldPosition, path[i].worldPosition);
                }
            }
        }
    }
}
