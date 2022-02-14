/*           INFINITY CODE          */
/*     https://infinity-code.com    */

using System;

namespace InfinityCode.uContext.UnityTypes
{
    public static class EditorGUINumberFieldValueRef
    {
        private static Type _type;

        public static Type type
        {
            get
            {
                if (_type == null) _type = Reflection.GetEditorType("EditorGUI+NumberFieldValue");
                return _type;
            }
        }
    }
}