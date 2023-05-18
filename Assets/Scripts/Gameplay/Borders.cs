using System.Collections.Generic;
using UnityEngine;

public class Borders : MonoBehaviour
{
    public List<Transform> limiters;
    public Color colorMain;

    void OnDrawGizmos()
    {
        if (limiters.Count <= 0) return;
        Gizmos.color = colorMain;
        Gizmos.DrawWireCube(GetCenterPoint(limiters), GetGreatestDistance(limiters));
    }

    public Vector3 GetCenterPos()
    {
        return GetCenterPoint(limiters);
    }

    Vector3 GetCenterPoint(List<Transform> targetsMain)
    {
        if (targetsMain.Count == 1)
        {
            return targetsMain[0].position;
        }

        var bounds = new Bounds(targetsMain[0].position, Vector3.zero);
        for (int i = 0; i < targetsMain.Count; i++)
        {
            bounds.Encapsulate(targetsMain[i].position);
        }

        return bounds.center;
    }

    Vector3 GetGreatestDistance(List<Transform> targetsMain) // only for Draw Gizmos
    {
        var bounds = new Bounds(targetsMain[^1].position, Vector3.zero);
        for (int i = 0; i < targetsMain.Count; i++)
        {
            bounds.Encapsulate(targetsMain[i].position);
        }

        return bounds.size;
    }
}