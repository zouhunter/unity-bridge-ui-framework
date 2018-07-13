using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace NodeGraph
{
    public class BezierUtil
    {
        public static Vector3[] GetBezierPoints(Vector3 p0, Vector3 p1, Vector3 p2,Vector3 p3, int count)
        {
            var points = new Vector3[count];
            for (int i = 0; i < count; i++)
            {
                points[i] = CalculateCubicBezierPoint(i / (count - 1f), p0, p1, p2,p3);
            }
            return points;
        }
        public static Vector3 CalculateCubicBezierPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
        {
            return Mathf.Pow(1 - t, 3) * p0 +
                3 * t * Mathf.Pow(1 - t, 2) * p1 +
                3 * (1 - t) * Mathf.Pow(t, 2) * p2 +
                Mathf.Pow(t, 3) * p3;
        }
    }

}