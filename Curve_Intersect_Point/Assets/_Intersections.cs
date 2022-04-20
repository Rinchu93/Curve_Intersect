using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Computational_Geometry
{
    //Help enum, to make it easier to deal with three cases: intersecting inside, intersecting on edge, not intersecting 
    //If we have two cases we can just return a bool
    public enum IntersectionCases
    {
        IsInside,
        IsOnEdge,
        NoIntersection
    }

    public static class _Intersections
    {

        //Clamp list indices
        //Will even work if index is larger/smaller than listSize, so can loop multiple times
        public static int ClampListIndex(int index, int listSize)
        {
            index = ((index % listSize) + listSize) % listSize;

            return index;
        }

        //
        // Are two lines intersecting?
        //

        public static bool LineLine(Edge2 a, Edge2 b, bool includeEndPoints)
        {
            //To avoid floating point precision issues we can use a small value
            float epsilon = 0.00001f;

            bool isIntersecting = false;

            float denominator = (b.p2.y - b.p1.y) * (a.p2.x - a.p1.x) - (b.p2.x - b.p1.x) * (a.p2.y - a.p1.y);

            //Make sure the denominator is != 0 (or the lines are parallel)
            if (denominator > 0f + epsilon || denominator < 0f - epsilon)
            {
                float u_a = ((b.p2.x - b.p1.x) * (a.p1.y - b.p1.y) - (b.p2.y - b.p1.y) * (a.p1.x - b.p1.x)) / denominator;
                float u_b = ((a.p2.x - a.p1.x) * (a.p1.y - b.p1.y) - (a.p2.y - a.p1.y) * (a.p1.x - b.p1.x)) / denominator;

                //Are the line segments intersecting if the end points are the same
                if (includeEndPoints)
                {
                    //The only difference between endpoints not included is the =, which will never happen so we have to subtract 0 by epsilon
                    float zero = 0f - epsilon;
                    float one = 1f + epsilon;

                    //Are intersecting if u_a and u_b are between 0 and 1 or exactly 0 or 1
                    if (u_a >= zero && u_a <= one && u_b >= zero && u_b <= one)
                    {
                        isIntersecting = true;
                    }
                }
                else
                {
                    float zero = 0f + epsilon;
                    float one = 1f - epsilon;

                    //Are intersecting if u_a and u_b are between 0 and 1
                    if (u_a > zero && u_a < one && u_b > zero && u_b < one)
                    {
                        isIntersecting = true;
                    }
                }

            }

            return isIntersecting;
        }



        //Whats the coordinate of the intersection point between two lines in 2d space if we know they are intersecting
        //http://thirdpartyninjas.com/blog/2008/10/07/line-segment-intersection/        
        public static MyVector2 GetLineLineIntersectionPoint(Edge2 a, Edge2 b)
        {
            float denominator = (b.p2.y - b.p1.y) * (a.p2.x - a.p1.x) - (b.p2.x - b.p1.x) * (a.p2.y - a.p1.y);

            float u_a = ((b.p2.x - b.p1.x) * (a.p1.y - b.p1.y) - (b.p2.y - b.p1.y) * (a.p1.x - b.p1.x)) / denominator;

            MyVector2 intersectionPoint = a.p1 + u_a * (a.p2 - a.p1);

            return intersectionPoint;
        }

        //
        // Is a point intersecting with a polygon?
        //
        //The list describing the polygon has to be sorted either clockwise or counter-clockwise because we have to identify its edges
        //TODO: May sometimes fail because of floating point precision issues
        public static bool PointPolygon(List<MyVector2> polygonPoints, MyVector2 point)
        {
            //Step 1. Find a point outside of the polygon
            //Pick a point with a x position larger than the polygons max x position, which is always outside
            MyVector2 maxXPosVertex = polygonPoints[0];

            for (int i = 1; i < polygonPoints.Count; i++)
            {
                if (polygonPoints[i].x > maxXPosVertex.x)
                {
                    maxXPosVertex = polygonPoints[i];
                }
            }

            //The point should be outside so just pick a number to move it outside
            //Should also move it up a little to minimize floating point precision issues
            //This is where it fails if this line is exactly on a vertex
            MyVector2 pointOutside = maxXPosVertex + new MyVector2(1f, 0.01f);


            //Step 2. Create an edge between the point we want to test with the point thats outside
            MyVector2 l1_p1 = point;
            MyVector2 l1_p2 = pointOutside;

            //Debug.DrawLine(l1_p1.XYZ(), l1_p2.XYZ());


            //Step 3. Find out how many edges of the polygon this edge is intersecting with
            int numberOfIntersections = 0;

            for (int i = 0; i < polygonPoints.Count; i++)
            {
                //Line 2
                MyVector2 l2_p1 = polygonPoints[i];

                int iPlusOne = ClampListIndex(i + 1, polygonPoints.Count);

                MyVector2 l2_p2 = polygonPoints[iPlusOne];

                //Are the lines intersecting?
                if (_Intersections.LineLine(new Edge2(l1_p1, l1_p2), new Edge2(l2_p1, l2_p2), includeEndPoints: true))
                {
                    numberOfIntersections += 1;
                }
            }


            //Step 4. Is the point inside or outside?
            bool isInside = true;

            //The point is outside the polygon if number of intersections is even or 0
            if (numberOfIntersections == 0 || numberOfIntersections % 2 == 0)
            {
                isInside = false;
            }

            return isInside;
        }



    }
}
