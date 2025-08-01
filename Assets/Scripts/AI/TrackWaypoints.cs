using System.Collections.Generic;
using UnityEngine;

public class TrackWaypoints : MonoBehaviour
{
    public Color lineColor;
    [Range(0f, 2f)] public float sphereRadius;
    public List<Transform> nodes = new List<Transform>();

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = lineColor;

        Transform[] path = GetComponentsInChildren<Transform>();

        nodes = new List<Transform>();
        for (int i = 1; i < path.Length; i++)
        {
            nodes.Add(path[i]);
        }

        for (int i = 0; i < nodes.Count; i++)
        {
            Vector3 currentPoint = nodes[i].position;
            Vector3 previousPoint = Vector3.zero;

            if (i != 0) previousPoint = nodes[i - 1].position;
            else if (i == 0) previousPoint = nodes[nodes.Count - 1].position;

            Gizmos.DrawLine(currentPoint, previousPoint);
            Gizmos.DrawSphere(currentPoint, sphereRadius);
        }
    }
}
