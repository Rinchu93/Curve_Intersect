using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Computational_Geometry
{
    public static class ExtensionMethods
    {


        //Vector2 - MyVector2
        public static MyVector2 ToMyVector2(this Vector2 v)
        {
            return new MyVector2(v.x, v.y);
        }


        //MyVector2 -> Vector2
        public static Vector2 ToVector2(this MyVector2 v)
        {
            return new Vector2(v.x, v.y);
        }



        public static List<MyVector2> ToMyVector2List(List<Vector2> vList)
        {
            List<MyVector2> myVList = new List<MyVector2>();
            foreach (Vector2 v in vList)
            {
                myVList.Add(ToMyVector2(v));
            }
            return myVList;
        }



    }
}
