/*           INFINITY CODE          */
/*     https://infinity-code.com    */

using System;
using System.Reflection;
using UnityEngine;

namespace InfinityCode.uContext.UnityTypes
{
    public static class GUILayoutUtilityRef
    {
        private static PropertyInfo _topLevelProp;

        public static Type type
        {
            get { return typeof(GUILayoutUtility); }
        }

        public static PropertyInfo topLevelProp
        {
            get
            {
                if (_topLevelProp == null) _topLevelProp = type.GetProperty("topLevel", Reflection.StaticLookup);
                return _topLevelProp;
            }
        }

        public static object GetTopLevel()
        {
            return topLevelProp.GetValue(null);
        }
    }
}