using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
namespace Computational_Geometry
{
    /*
     Problem definition
        Consider two general curves of degree 2  in 3D space and write a function which will return the intersection points between the two curves.
     API specification
        Input: Control points for the two curves
        Output: Intersection points
     Approach : The Bezier subdivision curve intersection algorithm relies on the convex hull property and the de
                Casteljau algorithm. Though we overview it in terms of Bezier curves, it will work for any curve 
                which obeys the convex hull property.The intersection algorithm proceeds by comparing the convex hulls of the two curves.
                If they do not overlap, the curves do not intersect. If they do overlap, the curves are subdivided and the
                two halves of one curve are checked for overlap against the two halves of the other curve. As this
                procedure continues, each iteration rejects regions of curves which do not contain intersection points.
                Once a pair of curves has been subdivided enough that they can each be approximated by a line
                segment.
    */


    public class BazierCurveIntersection : MonoBehaviour
    {
        List<Vector2> controlPoints1;
        List<Vector2> controlPoints2;

        HashSet<Vector2> pointIntersection = new HashSet<Vector2>();

        // Start is called before the first frame update
        void Start()
        {
            string filePath1 = @"assets/Resources/TestData1.txt";
            string filePath2 = @"assets/Resources/TestData2.txt";

            controlPoints1 = readData(filePath1);
            controlPoints2 = readData(filePath2);
            //Debug.Log(" List of control points of curve1");
            // PrintVector(controlPoints1);
            //Debug.Log("List of control points of curve2");
            //PrintVector(controlPoints2);
            List<Vector2> curvePoints1 = PointListCurve(controlPoints1);
            List<Vector2> curvePoints2 = PointListCurve(controlPoints2);
            //Debug.Log("Bazier points of curve1");
            //PrintVector(curvePoints1);
            //  Debug.Log("Bazier points of curve2");
            //PrintVector(curvePoints2);
            bezierCurveIntersection(curvePoints1, 0, curvePoints1.Count - 1, curvePoints2, 0, curvePoints1.Count - 1);
            Debug.Log("Point of intersection : ");
            PrintVector(pointIntersection);
        }

        //
        // Check whether the curves intersectes at vertex
        //

        public bool isOnVertex(List<Vector2> curvePoints1, List<Vector2> curvePoints2)
        {

            HashSet<Vector2> controlPointSet1 = new HashSet<Vector2>(curvePoints1);
            //PrintVector(controlPointSet1);
            foreach (Vector2 v in curvePoints2)
            {
                int size = controlPointSet1.Count;
                controlPointSet1.Add(v);
                if (controlPointSet1.Count == size)
                {
                    return true;
                }
            }
            return false;
        }

        //
        // Is the given point is inside the polygon 
        //
        public bool isPolygonOverlap(List<Vector2> curvePoints1, List<Vector2> curvePoints2)
        {

            foreach (Vector2 v in curvePoints2)
            {
                if (_Intersections.PointPolygon(ExtensionMethods.ToMyVector2List(curvePoints1), ExtensionMethods.ToMyVector2(v)))
                {
                    pointIntersection.Add(v);
                    if (curvePoints1.Count == 2 && curvePoints2.Count == 2)
                    {
                        MyVector2 p = _Intersections.GetLineLineIntersectionPoint(new Edge2(ExtensionMethods.ToMyVector2(curvePoints1[0]), ExtensionMethods.ToMyVector2(curvePoints1[1])), new Edge2(ExtensionMethods.ToMyVector2(curvePoints2[0]), ExtensionMethods.ToMyVector2(curvePoints2[1])));
                        pointIntersection.Add(ExtensionMethods.ToVector2(p));
                        Debug.Log("clipping line");
                        return true;
                    }
                    return true;
                }
            }
            return isOnVertex(curvePoints2, curvePoints1);


        }

        //
        //Finding the point of intersection of bezier curves using Bezier subdivision algorithm
        //

        public bool bezierCurveIntersection(List<Vector2> curvePoints1, int left1, int right1, List<Vector2> curvePoints2, int left2, int right2)
        {
            int size1 = right1 - left1;
            int size2 = right2 - left2;

            if (!isPolygonOverlap(curvePoints1.GetRange(left1, right1 - left1 + 1), curvePoints2.GetRange(left2, right2 - left2 + 1)))
            {
                return false;
            }


            int mid1 = left1 + (right1 - left1) / 2;
            int mid2 = left2 + (right2 - left2) / 2;
            bezierCurveIntersection(curvePoints1, left1, mid1, curvePoints2, left2, mid2);
            bezierCurveIntersection(curvePoints1, left1, mid1, curvePoints2, mid2 + 1, right2);
            bezierCurveIntersection(curvePoints1, mid1 + 1, right1, curvePoints2, left2, mid2);
            bezierCurveIntersection(curvePoints1, mid1 + 1, right1, curvePoints2, mid2 + 1, right2);
            return false;
        }

        //
        //Read the control points from the file
        //

        public List<Vector2> readData(String filePath)
        {
            List<Vector2> controlPoints = new List<Vector2>();
            string line;
            if (File.Exists(filePath))
            {
                StreamReader file = null;
                try
                {
                    file = new StreamReader(filePath);

                    while ((line = file.ReadLine()) != null)
                    {
                        String[] pos = line.Split(null);
                        controlPoints.Add(new Vector2(float.Parse(pos[0]), float.Parse(pos[1])));
                    }
                }
                finally
                {
                    if (file != null)
                        file.Close();
                }
            }
            return controlPoints;
        }

        //
        //Print the list vector
        //

        public void PrintVector(List<Vector2> controlPoints)
        {

            foreach (Vector2 v in controlPoints)
            {
                Debug.Log("Listing points : " + v.ToString());
            }

        }

        //
        //Print the hash set
        //

        public void PrintVector(HashSet<Vector2> controlPoints)
        {

            foreach (Vector2 v in controlPoints)
            {
                Debug.Log("Listing points : " + v.ToString());
            }
        }


        //
        // Generate bezier curve points using control points
        //

        public static List<Vector2> PointListCurve(List<Vector2> controlPoints, float interval = 0.01f)
        {
            List<Vector2> points = new List<Vector2>();
            int N = controlPoints.Count - 1;
            if (N < 2)
            {
                Debug.Log("The minimum control points needed is 3. Exiting..");
                return points;
            }
            if (N >= 3)
            {
                Debug.Log("The maximum control points allowed is 3 as this program is limited to degree 2 curves.");
                controlPoints.RemoveRange(3, controlPoints.Count - 3);
            }


            for (float t = 0.0f; t <= 1.0f + interval - 0.0001f; t += interval)
            {
                Vector2 p = new Vector2();
                float u = 1 - t;
                float tt = t * t;
                float uu = u * u;

                p += uu * controlPoints[0];
                p += 2 * u * t * controlPoints[1];
                p += tt * controlPoints[2];
                points.Add(p);
            }

            return points;
        }
    }
}