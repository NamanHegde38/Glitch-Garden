/*           INFINITY CODE          */
/*     https://infinity-code.com    */

using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace InfinityCode.uContext.UnityTypes
{
    public static class HandleUtilityRef
    {
        private static MethodInfo _intersectRayMeshMethod;

        private static MethodInfo intersectRayMeshMethod
        {
            get
            {
                if (_intersectRayMeshMethod == null)
                {
                    _intersectRayMeshMethod = Reflection.GetMethod(type, "IntersectRayMesh", new []{typeof(Ray), typeof(Mesh), typeof(Matrix4x4), typeof(RaycastHit).MakeByRefType()}, Reflection.StaticLookup);
                }

                return _intersectRayMeshMethod;
            }
        }

        public static Type type
        {
            get { return typeof(HandleUtility); }
        }

        public static bool IntersectRayMesh(Ray ray, Mesh mesh, Matrix4x4 matrix, out RaycastHit hit)
        {
            object[] obj = new object[]
            {
                ray,
                mesh,
                matrix,
                null
            };
            bool ret = (bool)intersectRayMeshMethod.Invoke(null, obj);
            hit = (RaycastHit) obj[3];
            return ret;
        }
    }
}