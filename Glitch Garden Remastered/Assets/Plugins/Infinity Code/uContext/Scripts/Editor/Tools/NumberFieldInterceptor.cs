/*           INFINITY CODE          */
/*     https://infinity-code.com    */

using System;
using System.Globalization;
using System.Reflection;
using InfinityCode.uContext.UnityTypes;
using InfinityCode.uContext.Unsafe;
using UnityEditor;
using UnityEngine;

namespace InfinityCode.uContext
{
    [InitializeOnLoad]
    public static class NumberFieldInterceptor
    {
        private static string recycledText;
        internal static readonly string s_AllowedCharactersForFloat = "inftynaeINFTYNAE0123456789.,-*/+%^()";
        internal static readonly string s_AllowedCharactersForInt = "0123456789-*/+%^()";
        private static MemoryPatcher patcher;

        static NumberFieldInterceptor()
        {
            if (Prefs.changeNumberFieldValueByArrow) Patch();
        }

        internal static void DoNumberField(
            object editor,
            Rect position,
            Rect dragHotZone,
            int id,
            bool isDouble,
            ref double doubleVal,
            ref long longVal,
            string formatString,
            GUIStyle style,
            bool draggable,
            double dragSensitivity)
        {
            string allowedChars = isDouble ? s_AllowedCharactersForFloat : s_AllowedCharactersForInt;

            if (draggable)
            {
                EditorGUIRef.DragNumberValue(dragHotZone, id, isDouble, ref doubleVal, ref longVal, dragSensitivity);
            }
            
            Event e = Event.current;
            int v = 0;

            if (Prefs.changeNumberFieldValueByArrow && e.type == EventType.KeyDown && GUIUtility.keyboardControl == id)
            {
                if (e.keyCode == KeyCode.UpArrow)
                {
                    if (e.control || e.command) v = 100;
                    else if (e.shift) v = 10;
                    else v = 1;

                    e.Use();
                }
                else if (e.keyCode == KeyCode.DownArrow)
                {
                    if (e.control || e.command) v = -100;
                    else if (e.shift) v = -10;
                    else v = -1;
                    e.Use();
                }

                if (v != 0)
                {
                    if (isDouble)
                    {
                        if (!double.IsInfinity(doubleVal) && !double.IsNaN(doubleVal))
                        {
                            doubleVal += v;
                            recycledText = doubleVal.ToString(Culture.numberFormat);
                            GUI.changed = true;
                        }
                    }
                    else
                    {
                        longVal += v;
                        recycledText = longVal.ToString();
                        GUI.changed = true;
                    }

                    TextEditorRef.SetText(editor, recycledText);
                    TextEditorRef.SetCursorIndex(editor, 0);
                    TextEditorRef.SetSelectionIndex(editor, recycledText.Length);
                }
            }

            string text;
            if (EditorGUIRef.HasKeyboardFocus(id) || e.type == EventType.MouseDown && e.button == 0 && position.Contains(e.mousePosition))
            {
                if (!RecycledTextEditorRef.IsEditingControl(editor, id))
                {
                    text = recycledText = isDouble ? doubleVal.ToString(formatString, Culture.numberFormat) : longVal.ToString();
                }
                else
                {
                    text = recycledText;
                    if (e.type == EventType.ValidateCommand && e.commandName == "UndoRedoPerformed")
                    {
                        text = recycledText = isDouble ? doubleVal.ToString(formatString, Culture.numberFormat) : longVal.ToString();
                    }
                }
            }
            else
            {
                text = isDouble ? doubleVal.ToString(formatString, Culture.numberFormat) : longVal.ToString();
            }

            if (GUIUtility.keyboardControl == id)
            {
                bool changed;
                string str = EditorGUIRef.DoTextField(editor, id, position, text, style, allowedChars, out changed, false, false, false);
                if (!changed) return;
                GUI.changed = true;
                recycledText = str;
                if (isDouble) StringToDouble(str, out doubleVal);
                else StringToLong(str, out longVal);
            }
            else EditorGUIRef.DoTextField(editor, id, position, text, style, allowedChars, out bool _, false, false, false);
        }

        private static void Patch()
        {
            try
            {
                MethodInfo method = typeof(EditorGUI).GetMethod(
                    "DoNumberField",
                    Reflection.StaticLookup,
                    null,
                    new[]
                    {
                        RecycledTextEditorRef.type,
                        typeof(Rect),
                        typeof(Rect),
                        typeof(int),
#if !UNITY_2021_2_OR_NEWER
                        typeof(bool),
                        typeof(double).MakeByRefType(),
                        typeof(long).MakeByRefType(),
#else
                        EditorGUINumberFieldValueRef.type.MakeByRefType(),
#endif
                        typeof(string),
                        typeof(GUIStyle),
                        typeof(bool),
                        typeof(double)
                    },
                    null);
                MethodInfo replacement = typeof(NumberFieldInterceptor).GetMethod(
                    "DoNumberField",
                    Reflection.StaticLookup,
                    null,
                    new[]
                    {
                        typeof(object),
                        typeof(Rect),
                        typeof(Rect),
                        typeof(int),
#if !UNITY_2021_2_OR_NEWER
                        typeof(bool),
                        typeof(double).MakeByRefType(),
                        typeof(long).MakeByRefType(),
#else
                        typeof(EditorGUIRef.NumberFieldValue).MakeByRefType(),
#endif
                        typeof(string),
                        typeof(GUIStyle),
                        typeof(bool),
                        typeof(double)
                    },
                    null);

                patcher = MemoryPatcher.SwapMethods(method, replacement);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }

        public static void Refresh()
        {
            if (Prefs.changeNumberFieldValueByArrow) Patch();
            else patcher.Restore();
        }

        internal static bool StringToDouble(string str, out double value)
        {
            string lower = str.ToLower();
            if (lower == "inf" || lower == "infinity")
                value = double.PositiveInfinity;
            else if (lower == "-inf" || lower == "-infinity")
                value = double.NegativeInfinity;
            else if (lower == "nan")
                value = double.NaN;
            else
                return ExpressionEvaluator.Evaluate<double>(str, out value);
            return false;
        }

        internal static bool StringToLong(string str, out long value)
        {
            return ExpressionEvaluator.Evaluate<long>(str, out value);
        }

#if UNITY_2021_2_OR_NEWER
        #region UNITY_2021_2_OR_NEWER
        private static DragCandidateState s_DragCandidateState;
        private static double s_DragStartValue;
        private static long s_DragStartIntValue;
        private static Vector2 s_DragStartPos;
        private static double s_DragSensitivity;

        internal static double DiscardLeastSignificantDecimal(double v)
        {
            int digits = Math.Max(0, (int)(5.0 - Math.Log10(Math.Abs(v))));
            try
            {
                return Math.Round(v, digits);
            }
            catch (ArgumentOutOfRangeException)
            {
                return 0.0;
            }
        }

        internal static void DoNumberField(
            object editor,
            Rect position,
            Rect dragHotZone,
            int id,
            ref EditorGUIRef.NumberFieldValue value,
            string formatString,
            GUIStyle style,
            bool draggable,
            double dragSensitivity)
        {
            string allowedletters = value.isDouble ? s_AllowedCharactersForFloat : s_AllowedCharactersForInt;
            if (draggable && GUI.enabled)
            {
                DragNumberValue(dragHotZone, id, ref value, dragSensitivity);
            }

            Event e = Event.current;
            int v = 0;

            if (Prefs.changeNumberFieldValueByArrow && e.type == EventType.KeyDown && GUIUtility.keyboardControl == id)
            {
                if (e.keyCode == KeyCode.UpArrow)
                {
                    if (e.control || e.command) v = 100;
                    else if (e.shift) v = 10;
                    else v = 1;

                    e.Use();
                }
                else if (e.keyCode == KeyCode.DownArrow)
                {
                    if (e.control || e.command) v = -100;
                    else if (e.shift) v = -10;
                    else v = -1;
                    e.Use();
                }

                if (v != 0)
                {
                    if (value.isDouble)
                    {
                        if (!double.IsInfinity(value.doubleVal) && !double.IsNaN(value.doubleVal))
                        {
                            value.doubleVal += v;
                            value.success = true;
                            recycledText = value.doubleVal.ToString(Culture.numberFormat);
                            GUI.changed = true;
                        }
                    }
                    else
                    {
                        value.longVal += v;
                        value.success = true;
                        recycledText = value.longVal.ToString();
                        GUI.changed = true;
                    }

                    TextEditorRef.SetText(editor, recycledText);
                    TextEditorRef.SetCursorIndex(editor, 0);
                    TextEditorRef.SetSelectionIndex(editor, recycledText.Length);
                }
            }

            string text;
            if (EditorGUIRef.HasKeyboardFocus(id) || e.type == EventType.MouseDown && e.button == 0 && position.Contains(e.mousePosition))
            {
                if (!RecycledTextEditorRef.IsEditingControl(editor, id))
                {
                    text = recycledText = value.isDouble ? value.doubleVal.ToString(formatString, CultureInfo.InvariantCulture) : value.longVal.ToString(formatString, CultureInfo.InvariantCulture);
                }
                else
                {
                    text = recycledText;
                    if (e.type == EventType.ValidateCommand && e.commandName == "UndoRedoPerformed")
                    {
                        text = value.isDouble ? value.doubleVal.ToString(formatString, CultureInfo.InvariantCulture) : value.longVal.ToString(formatString, CultureInfo.InvariantCulture);
                    }
                }
            }
            else
            {
                text = value.isDouble ? value.doubleVal.ToString(formatString, CultureInfo.InvariantCulture) : value.longVal.ToString(formatString, CultureInfo.InvariantCulture);
            }
            if (GUIUtility.keyboardControl == id)
            {
                bool changed;
                string str = EditorGUIRef.DoTextField(editor, id, position, text, style, allowedletters, out changed, false, false, false);
                if (!changed) return;
                GUI.changed = true;
                recycledText = str;
                if (value.isDouble)
                {
                    if (StringToDouble(str, out value.doubleVal)) value.success = true;
                }
                else
                {
                    if (StringToLong(str, out value.longVal)) value.success = true;
                }
            }
            else
            {
                EditorGUIRef.DoTextField(editor, id, position, text, style, allowedletters, out bool _, false, false, false);
            }
        }

        private static void DragNumberValue(
            Rect dragHotZone,
            int id,
            ref EditorGUIRef.NumberFieldValue value,
            double dragSensitivity)
        {
            Event e = Event.current;
            switch (e.GetTypeForControl(id))
            {
                case EventType.MouseDown:
                    if (!HitTest(dragHotZone, e.mousePosition, 0) || e.button != 0) break;
                    EditorGUIUtility.editingTextField = false;
                    GUIUtility.hotControl = id;
                    object activeEditor = EditorGUIRef.GetActiveEditor();
                    if (activeEditor != null) RecycledTextEditorRef.EndEditing(activeEditor);
                    e.Use();
                    GUIUtility.keyboardControl = id;
                    s_DragCandidateState = DragCandidateState.InitiatedDragging;
                    s_DragStartValue = value.doubleVal;
                    s_DragStartIntValue = value.longVal;
                    s_DragStartPos = e.mousePosition;
                    s_DragSensitivity = dragSensitivity;
                    e.Use();
                    EditorGUIUtility.SetWantsMouseJumping(1);
                    break;
                case EventType.MouseUp:
                    if (GUIUtility.hotControl != id || (uint)s_DragCandidateState <= 0U)
                        break;
                    GUIUtility.hotControl = 0;
                    s_DragCandidateState = DragCandidateState.NotDragging;
                    e.Use();
                    EditorGUIUtility.SetWantsMouseJumping(0);
                    break;
                case EventType.MouseDrag:
                    if (GUIUtility.hotControl != id) break;
                    switch (s_DragCandidateState)
                    {
                        case DragCandidateState.InitiatedDragging:
                            if ((double)(e.mousePosition - s_DragStartPos).sqrMagnitude > 16.0)
                            {
                                s_DragCandidateState = DragCandidateState.CurrentlyDragging;
                                GUIUtility.keyboardControl = id;
                            }
                            e.Use();
                            break;
                        case DragCandidateState.CurrentlyDragging:
                            if (value.isDouble)
                            {
                                value.doubleVal += (double)HandleUtility.niceMouseDelta * s_DragSensitivity;
                                value.doubleVal = RoundBasedOnMinimumDifference(value.doubleVal, s_DragSensitivity);
                            }
                            else
                            {
                                value.longVal += (long)Math.Round((double)HandleUtility.niceMouseDelta * s_DragSensitivity);
                            }
                            value.success = true;
                            GUI.changed = true;
                            e.Use();
                            break;
                    }
                    break;
                case UnityEngine.EventType.KeyDown:
                    if (GUIUtility.hotControl != id || e.keyCode != KeyCode.Escape || (uint) s_DragCandidateState <= 0U)
                    {
                        break;
                    }
                    value.doubleVal = s_DragStartValue;
                    value.longVal = s_DragStartIntValue;
                    value.success = true;
                    GUI.changed = true;
                    GUIUtility.hotControl = 0;
                    e.Use();
                    break;
                case EventType.Repaint:
                    EditorGUIUtility.AddCursorRect(dragHotZone, MouseCursor.SlideArrow);
                    break;
            }
        }

        internal static int GetNumberOfDecimalsForMinimumDifference(double minDifference)
        {
            return (int)Math.Max(0.0, -Math.Floor(Math.Log10(Math.Abs(minDifference))));
        }

        internal static bool HitTest(Rect rect, Vector2 point, int offset)
        {
            return (double)point.x >= (double)rect.xMin - (double)offset && (double)point.x < (double)rect.xMax + (double)offset && (double)point.y >= (double)rect.yMin - (double)offset && (double)point.y < (double)rect.yMax + (double)offset;
        }

        internal static double RoundBasedOnMinimumDifference(double valueToRound, double minDifference)
        {
            return minDifference == 0.0 ? DiscardLeastSignificantDecimal(valueToRound) : Math.Round(valueToRound, GetNumberOfDecimalsForMinimumDifference(minDifference), MidpointRounding.AwayFromZero);
        }

        private enum DragCandidateState
        {
            NotDragging,
            InitiatedDragging,
            CurrentlyDragging,
        }

        #endregion
#endif
    }
}