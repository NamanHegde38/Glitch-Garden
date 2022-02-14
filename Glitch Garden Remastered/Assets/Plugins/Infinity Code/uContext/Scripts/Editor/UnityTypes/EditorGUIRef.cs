/*           INFINITY CODE          */
/*     https://infinity-code.com    */

using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace InfinityCode.uContext.UnityTypes
{
    public static class EditorGUIRef
    {
        private static FieldInfo _activeEditorField;
        private static MethodInfo _dragNumberValueMethod;
        private static MethodInfo _hasKeyboardFocusMethod;
        private static FieldInfo _recycledEditorField;
        private static MethodInfo _doTextFieldMethod;
        private static MethodInfo _isEditingTextFieldMethod;

        private static MethodInfo doTextFieldMethod
        {
            get
            {
                if (_doTextFieldMethod == null)
                {
                    _doTextFieldMethod = type.GetMethod(
                        "DoTextField",
                        Reflection.StaticLookup,
                        null,
                        new[]
                        {
                            RecycledTextEditorRef.type,
                            typeof(int),
                            typeof(Rect),
                            typeof(string),
                            typeof(GUIStyle),
                            typeof(string),
                            typeof(bool).MakeByRefType(),
                            typeof(bool),
                            typeof(bool),
                            typeof(bool)
                        }, 
                        null
                    );
                }
                return _doTextFieldMethod;
            }
        }

        private static MethodInfo dragNumberValueMethod
        {
            get
            {
                if (_dragNumberValueMethod == null)
                {
                    _dragNumberValueMethod = type.GetMethod(
                        "DragNumberValue",
                        Reflection.StaticLookup,
                        null,
                        new[]
                        {
                            typeof(Rect),
                            typeof(int),
                            typeof(bool),
                            typeof(double).MakeByRefType(),
                            typeof(long).MakeByRefType(),
                            typeof(double)
                        }, 
                        null
                    );
                }
                return _dragNumberValueMethod;
            }
        }

        private static MethodInfo hasKeyboardFocusMethod
        {
            get
            {
                if (_hasKeyboardFocusMethod == null)
                {
                    _hasKeyboardFocusMethod = type.GetMethod(
                        "HasKeyboardFocus", 
                        Reflection.StaticLookup,
                        null,
                        new []{typeof(int)},
                        null);
                }
                return _hasKeyboardFocusMethod;
            }
        }

        private static MethodInfo isEditingTextFieldMethod
        {
            get
            {
                if (_isEditingTextFieldMethod == null)
                {
                    _isEditingTextFieldMethod = Reflection.GetMethod(type, "IsEditingTextField", Reflection.StaticLookup);
                }

                return _isEditingTextFieldMethod;
            }
        }

        private static FieldInfo recycledEditorField
        {
            get
            {
                if (_recycledEditorField == null) _recycledEditorField = type.GetField("s_RecycledEditor", Reflection.StaticLookup);
                return _recycledEditorField;
            }
        }

        private static FieldInfo activeEditorField
        {
            get
            {
                if (_activeEditorField == null) _activeEditorField = type.GetField("activeEditor", Reflection.StaticLookup);
                return _activeEditorField;
            }
        }

        private static Type type
        {
            get { return typeof(EditorGUI); }
        }

        public static string DoTextField(
            object editor,
            int id,
            Rect position,
            string text,
            GUIStyle style,
            string allowedletters,
            out bool changed,
            bool reset,
            bool multiline,
            bool passwordField)
        {
            object[] args = new object[]
            {
                editor,
                id,
                position,
                text,
                style,
                allowedletters,
                null,
                reset,
                multiline,
                passwordField
            };
            string result = (string) doTextFieldMethod.Invoke(null, args);
            changed = (bool)args[6];
            return result;
        }

        public static void DragNumberValue(Rect dragHotZone,
            int id,
            bool isDouble,
            ref double doubleVal,
            ref long longVal,
            double dragSensitivity)
        {
            object[] args = new object[]
            {
                dragHotZone,
                id,
                isDouble,
                doubleVal,
                longVal,
                dragSensitivity
            };
            dragNumberValueMethod.Invoke(null, args);
            doubleVal = (double)args[3];
            longVal = (long)args[4];
        }

        public static object GetActiveEditor()
        {
            return activeEditorField.GetValue(null);
        }

        public static object GetRecycledEditor()
        {
            return recycledEditorField.GetValue(null);
        }

        public static bool HasKeyboardFocus(int id)
        {
            return (bool) hasKeyboardFocusMethod.Invoke(null, new object[] {id});
        }

        public static bool IsEditingTextField()
        {
            return (bool)isEditingTextFieldMethod.Invoke(null, new object[0]);
        }

        public struct NumberFieldValue
        {
            public bool isDouble;
            public double doubleVal;
            public long longVal;
            public object expression;
            public bool success;

            public NumberFieldValue(double v)
            {
                this.isDouble = true;
                this.doubleVal = v;
                this.longVal = 0L;
                this.expression = null;
                this.success = false;
            }

            public NumberFieldValue(long v)
            {
                this.isDouble = false;
                this.doubleVal = 0.0;
                this.longVal = v;
                this.expression = null;
                this.success = false;
            }

            public bool hasResult
            {
                get
                {
                    return this.success || this.expression != null;
                }
            }
        }
    }
}