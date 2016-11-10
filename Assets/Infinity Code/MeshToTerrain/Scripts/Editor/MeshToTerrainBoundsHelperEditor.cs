/*       INFINITY CODE 2013 - 2016         */
/*     http://www.infinity-code.com        */

using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MeshToTerrainBoundsHelper))]
public class MeshToTerrainBoundsHelperEditor:Editor
{
    private BoxEditor boxEditor;

    private MeshToTerrainBoundsHelper helper;

    private void OnDisable()
    {
        if (helper != null)
        {
            DestroyImmediate(helper.gameObject);
            if (helper.OnDestroyed != null) helper.OnDestroyed();
        }
    }

    private void OnEnable()
    {
        try
        {
            helper = target as MeshToTerrainBoundsHelper;

            if (target == null)
            {
                return;
            }
            if (helper.bounds == new Bounds())
            {
                DestroyImmediate(helper);
                return;
            }

            boxEditor = new BoxEditor(true, -1);
        }
        catch
        {
        }         
    }

    public void OnSceneGUI()
    {
        Color color = new Color32(145, 244, 139, 210);

        Vector3 center = helper.bounds.center;
        Vector3 size = helper.bounds.size;

        if (boxEditor.OnSceneGUI(helper.transform, color, ref center, ref size))
        {
            helper.bounds.center = center;
            helper.bounds.size = size;
            if (helper.OnBoundChanged != null) helper.OnBoundChanged();
        }
    }

    internal class BoxEditor
    {
        private int m_ControlIdHint;
        private bool m_DisableZaxis;
        private bool m_UseLossyScale;
        private static float s_ScaleSnap = float.MinValue;

        private static float SnapSettingsScale
        {
            get
            {
                if (s_ScaleSnap == float.MinValue)
                {
                    s_ScaleSnap = EditorPrefs.GetFloat("ScaleSnap", 0.1f);
                }
                return s_ScaleSnap;
            }
        }

        public BoxEditor(bool useLossyScale, int controlIdHint)
        {
            m_UseLossyScale = useLossyScale;
            m_ControlIdHint = controlIdHint;
        }

        public BoxEditor(bool useLossyScale, int controlIdHint, bool disableZaxis)
        {
            m_UseLossyScale = useLossyScale;
            m_ControlIdHint = controlIdHint;
            m_DisableZaxis = disableZaxis;
        }

        private void AdjustMidpointHandleColor(Vector3 localPos, Vector3 localTangent, Vector3 localBinormal, Matrix4x4 transform, float alphaFactor)
        {
            float num;
            Vector3 vector = transform.MultiplyPoint(localPos);
            Vector3 lhs = transform.MultiplyVector(localTangent);
            Vector3 rhs = transform.MultiplyVector(localBinormal);
            Vector3 normalized = Vector3.Cross(lhs, rhs).normalized;
#if UNITY_4_3 || UNITY_4_5 || UNITY_4_6
            if (Camera.current.isOrthoGraphic)
#else
            if (Camera.current.orthographic)
#endif
            {
                num = Vector3.Dot(-Camera.current.transform.forward, normalized);
            }
            else
            {
                Vector3 vector6 = Camera.current.transform.position - vector;
                num = Vector3.Dot(vector6.normalized, normalized);
            }
            if (num < -0.0001f)
            {
                alphaFactor *= 0.2f;
            }
            if (alphaFactor < 1f)
            {
                Handles.color = new Color(Handles.color.r, Handles.color.g, Handles.color.b, Handles.color.a * alphaFactor);
            }
        }

        public void DrawWireframeBox(Vector3 center, Vector3 siz)
        {
            Vector3 vector = siz * 0.5f;
            Vector3[] points = { center + new Vector3(-vector.x, -vector.y, -vector.z), center + new Vector3(-vector.x, vector.y, -vector.z), center + new Vector3(vector.x, vector.y, -vector.z), center + new Vector3(vector.x, -vector.y, -vector.z), center + new Vector3(-vector.x, -vector.y, -vector.z), center + new Vector3(-vector.x, -vector.y, vector.z), center + new Vector3(-vector.x, vector.y, vector.z), center + new Vector3(vector.x, vector.y, vector.z), center + new Vector3(vector.x, -vector.y, vector.z), center + new Vector3(-vector.x, -vector.y, vector.z) };
            Handles.DrawPolyLine(points);
            Handles.DrawLine(points[1], points[6]);
            Handles.DrawLine(points[2], points[7]);
            Handles.DrawLine(points[3], points[8]);
        }

        private Vector3 MidpointHandle(Vector3 localPos, Vector3 localTangent, Vector3 localBinormal, Matrix4x4 transform)
        {
            Color color = Handles.color;
            float alphaFactor = 1f;
            AdjustMidpointHandleColor(localPos, localTangent, localBinormal, transform, alphaFactor);
            int controlID = GUIUtility.GetControlID(m_ControlIdHint, FocusType.Keyboard);
            if (alphaFactor > 0f)
            {
                Vector3 normalized = Vector3.Cross(localTangent, localBinormal).normalized;
                localPos = Slider1D.Do(controlID, localPos, normalized, HandleUtility.GetHandleSize(localPos) * 0.03f, Handles.DotCap, SnapSettingsScale);
            }
            Handles.color = color;
            return localPos;
        }

        private void MidpointHandles(ref Vector3 minPos, ref Vector3 maxPos, Matrix4x4 transform)
        {
            Vector3 localTangent = new Vector3(1f, 0f, 0f);
            Vector3 vector2 = new Vector3(0f, 1f, 0f);
            Vector3 localBinormal = new Vector3(0f, 0f, 1f);
            Vector3 vector4 = (maxPos + minPos) * 0.5f;
            Vector3 localPos = new Vector3(maxPos.x, vector4.y, vector4.z);
            Vector3 vector6 = MidpointHandle(localPos, vector2, localBinormal, transform);
            maxPos.x = vector6.x;
            localPos = new Vector3(minPos.x, vector4.y, vector4.z);
            vector6 = MidpointHandle(localPos, vector2, -localBinormal, transform);
            minPos.x = vector6.x;
            localPos = new Vector3(vector4.x, maxPos.y, vector4.z);
            vector6 = MidpointHandle(localPos, localTangent, -localBinormal, transform);
            maxPos.y = vector6.y;
            localPos = new Vector3(vector4.x, minPos.y, vector4.z);
            vector6 = MidpointHandle(localPos, localTangent, localBinormal, transform);
            minPos.y = vector6.y;
            if (!m_DisableZaxis)
            {
                localPos = new Vector3(vector4.x, vector4.y, maxPos.z);
                vector6 = MidpointHandle(localPos, vector2, -localTangent, transform);
                maxPos.z = vector6.z;
                localPos = new Vector3(vector4.x, vector4.y, minPos.z);
                vector6 = MidpointHandle(localPos, vector2, localTangent, transform);
                minPos.z = vector6.z;
            }
        }

        public bool OnSceneGUI(Matrix4x4 transform, Color color, ref Vector3 center, ref Vector3 size)
        {
            Color color2 = Handles.color;
            Handles.color = color;
            Vector3 minPos = center - (size * 0.5f);
            Vector3 maxPos = center + (size * 0.5f);
            Matrix4x4 matrix = Handles.matrix;
            Handles.matrix = transform;
            DrawWireframeBox((maxPos - minPos) * 0.5f + minPos, maxPos - minPos);
            MidpointHandles(ref minPos, ref maxPos, Handles.matrix);

            bool changed = GUI.changed;
            if (changed)
            {
                center = (maxPos + minPos) * 0.5f;
                size = maxPos - minPos;
            }
            Handles.color = color2;
            Handles.matrix = matrix;
            return changed;
        }

        public bool OnSceneGUI(Transform transform, Color color, ref Vector3 center, ref Vector3 size)
        {
            if (m_UseLossyScale)
            {
                Matrix4x4 matrixx = Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one);
                size.Scale(transform.lossyScale);
                center = transform.TransformPoint(center);
                center = matrixx.inverse.MultiplyPoint(center);
                bool flag = OnSceneGUI(matrixx, color, ref center, ref size);
                center = matrixx.MultiplyPoint(center);
                center = transform.InverseTransformPoint(center);
                size.Scale(new Vector3(1f / transform.lossyScale.x, 1f / transform.lossyScale.y, 1f / transform.lossyScale.z));
                return flag;
            }
            return OnSceneGUI(transform.localToWorldMatrix, color, ref center, ref size);
        }
    }

    internal class Slider1D
    {
        private static Vector2 s_CurrentMousePosition;
        private static Vector2 s_StartMousePosition;
        private static Vector3 s_StartPosition;

        internal static Vector3 Do(int id, Vector3 position, Vector3 direction, float size,
            Handles.DrawCapFunction drawFunc, float snap)
        {
            return Do(id, position, direction, direction, size, drawFunc, snap);
        }

        internal static Vector3 Do(int id, Vector3 position, Vector3 handleDirection, Vector3 slideDirection, float size,
            Handles.DrawCapFunction drawFunc, float snap)
        {
            Event current = Event.current;
            switch (current.GetTypeForControl(id))
            {
                case EventType.MouseDown:
                    if ((((HandleUtility.nearestControl == id) && (current.button == 0)) ||
                         ((GUIUtility.keyboardControl == id) && (current.button == 2))) && (GUIUtility.hotControl == 0))
                    {
                        int num2 = id;
                        GUIUtility.keyboardControl = num2;
                        GUIUtility.hotControl = num2;
                        s_CurrentMousePosition = s_StartMousePosition = current.mousePosition;
                        s_StartPosition = position;
                        current.Use();
                        EditorGUIUtility.SetWantsMouseJumping(1);
                    }
                    return position;

                case EventType.MouseUp:
                    if ((GUIUtility.hotControl == id) && ((current.button == 0) || (current.button == 2)))
                    {
                        GUIUtility.hotControl = 0;
                        current.Use();
                        EditorGUIUtility.SetWantsMouseJumping(0);
                    }
                    return position;

                case EventType.MouseMove:
                case EventType.KeyDown:
                case EventType.KeyUp:
                case EventType.ScrollWheel:
                    return position;

                case EventType.MouseDrag:
                    if (GUIUtility.hotControl == id)
                    {
                        s_CurrentMousePosition += current.delta;
                        float num =
                            Handles.SnapValue(
                                HandleUtility.CalcLineTranslation(s_StartMousePosition, s_CurrentMousePosition,
                                    s_StartPosition, slideDirection), snap);
                        Vector3 vector = Handles.matrix.MultiplyVector(slideDirection);
                        Vector3 v = Handles.matrix.MultiplyPoint(s_StartPosition) + vector * num;
                        position = Handles.matrix.inverse.MultiplyPoint(v);
                        GUI.changed = true;
                        current.Use();
                    }
                    return position;

                case EventType.Repaint:
                    {
                        Color white = Color.white;
                        if ((id == GUIUtility.keyboardControl) && GUI.enabled)
                        {
                            white = Handles.color;
                            Handles.color = Color.green;
                        }
                        drawFunc(id, position, Quaternion.LookRotation(handleDirection), size);
                        if (id == GUIUtility.keyboardControl)
                        {
                            Handles.color = white;
                        }
                        return position;
                    }
                case EventType.Layout:
                    if (drawFunc != Handles.ArrowCap)
                    {
                        HandleUtility.AddControl(id, HandleUtility.DistanceToCircle(position, size * 0.2f));
                        return position;
                    }
                    HandleUtility.AddControl(id,
                        HandleUtility.DistanceToLine(position, position + slideDirection * size));
                    HandleUtility.AddControl(id,
                        HandleUtility.DistanceToCircle(position + slideDirection * size, size * 0.2f));
                    return position;
            }
            return position;
        }
    }

    internal static class Slider2D
    {
        private static Vector2 s_CurrentMousePosition;
        private static Vector2 s_StartPlaneOffset;
        private static Vector3 s_StartPosition;

        private static Vector2 CalcDeltaAlongDirections(int id, Vector3 handlePos, Vector3 offset, Vector3 handleDir, Vector3 slideDir1, Vector3 slideDir2, float handleSize, Handles.DrawCapFunction drawFunc, Vector2 snap, bool drawHelper)
        {
            Vector2 vector = new Vector2(0f, 0f);
            Event current = Event.current;
            switch (current.GetTypeForControl(id))
            {
                case EventType.MouseDown:
                    if ((((HandleUtility.nearestControl == id) && (current.button == 0)) || ((GUIUtility.keyboardControl == id) && (current.button == 2))) && (GUIUtility.hotControl == 0))
                    {
                        Plane plane = new Plane(Handles.matrix.MultiplyVector(handleDir), Handles.matrix.MultiplyPoint(handlePos));
                        Ray ray = HandleUtility.GUIPointToWorldRay(current.mousePosition);
                        float enter;
                        plane.Raycast(ray, out enter);
                        int num5 = id;
                        GUIUtility.keyboardControl = num5;
                        GUIUtility.hotControl = num5;
                        s_CurrentMousePosition = current.mousePosition;
                        s_StartPosition = handlePos;
                        Vector3 lhs = Handles.matrix.inverse.MultiplyPoint(ray.GetPoint(enter)) - handlePos;
                        s_StartPlaneOffset.x = Vector3.Dot(lhs, slideDir1);
                        s_StartPlaneOffset.y = Vector3.Dot(lhs, slideDir2);
                        current.Use();
                        EditorGUIUtility.SetWantsMouseJumping(1);
                    }
                    return vector;

                case EventType.MouseUp:
                    if ((GUIUtility.hotControl == id) && ((current.button == 0) || (current.button == 2)))
                    {
                        GUIUtility.hotControl = 0;
                        current.Use();
                        EditorGUIUtility.SetWantsMouseJumping(0);
                    }
                    return vector;

                case EventType.MouseMove:
                case EventType.KeyDown:
                case EventType.KeyUp:
                case EventType.ScrollWheel:
                    return vector;

                case EventType.MouseDrag:
                    if (GUIUtility.hotControl == id)
                    {
                        s_CurrentMousePosition += current.delta;
                        Vector3 a = Handles.matrix.MultiplyPoint(handlePos);
                        Vector3 normalized = Handles.matrix.MultiplyVector(slideDir1).normalized;
                        Vector3 vector6 = Handles.matrix.MultiplyVector(slideDir2).normalized;
                        Ray ray2 = HandleUtility.GUIPointToWorldRay(s_CurrentMousePosition);
                        Plane plane2 = new Plane(a, a + normalized, a + vector6);
                        float num2;
                        if (plane2.Raycast(ray2, out num2))
                        {
                            Vector3 point = Handles.matrix.inverse.MultiplyPoint(ray2.GetPoint(num2));
                            vector.x = HandleUtility.PointOnLineParameter(point, s_StartPosition, slideDir1);
                            vector.y = HandleUtility.PointOnLineParameter(point, s_StartPosition, slideDir2);
                            vector -= s_StartPlaneOffset;
                            if ((snap.x > 0f) || (snap.y > 0f))
                            {
                                vector.x = Handles.SnapValue(vector.x, snap.x);
                                vector.y = Handles.SnapValue(vector.y, snap.y);
                            }
                            GUI.changed = true;
                        }
                        current.Use();
                    }
                    return vector;

                case EventType.Repaint:
                    if (drawFunc != null)
                    {
                        Vector3 position = handlePos + offset;
                        Quaternion rotation = Quaternion.LookRotation(handleDir, slideDir1);
                        Color white = Color.white;
                        if (id == GUIUtility.keyboardControl)
                        {
                            white = Handles.color;
                            Handles.color = Color.green;
                        }
                        drawFunc(id, position, rotation, handleSize);
                        if (id == GUIUtility.keyboardControl)
                        {
                            Handles.color = white;
                        }
                        if (drawHelper && (GUIUtility.hotControl == id))
                        {
                            Vector3[] verts = new Vector3[4];
                            float num3 = handleSize * 10f;
                            verts[0] = position + (slideDir1 * num3 + slideDir2 * num3);
                            verts[1] = verts[0] - slideDir1 * num3 * 2f;
                            verts[2] = verts[1] - slideDir2 * num3 * 2f;
                            verts[3] = verts[2] + slideDir1 * num3 * 2f;
                            Color color = Handles.color;
                            Handles.color = Color.white;
                            float r = 0.6f;
                            Handles.DrawSolidRectangleWithOutline(verts, new Color(1f, 1f, 1f, 0.05f), new Color(r, r, r, 0.4f));
                            Handles.color = color;
                        }
                        return vector;
                    }
                    return vector;

                case EventType.Layout:
                    if (drawFunc != Handles.ArrowCap)
                    {
                        if (drawFunc == Handles.RectangleCap)
                        {
                            HandleUtility.AddControl(id, HandleUtility.DistanceToRectangle(handlePos + offset, Quaternion.LookRotation(handleDir, slideDir1), handleSize));
                            return vector;
                        }
                        HandleUtility.AddControl(id, HandleUtility.DistanceToCircle(handlePos + offset, handleSize * 0.5f));
                        return vector;
                    }
                    HandleUtility.AddControl(id, HandleUtility.DistanceToLine(handlePos + offset, handlePos + handleDir * handleSize));
                    HandleUtility.AddControl(id, HandleUtility.DistanceToCircle((handlePos + offset) + handleDir * handleSize, handleSize * 0.2f));
                    return vector;
            }
            return vector;
        }

        public static Vector3 Do(int id, Vector3 handlePos, Vector3 handleDir, Vector3 slideDir1, Vector3 slideDir2, float handleSize, Handles.DrawCapFunction drawFunc, float snap, bool drawHelper)
        {
            return Do(id, handlePos, new Vector3(0f, 0f, 0f), handleDir, slideDir1, slideDir2, handleSize, drawFunc, new Vector2(snap, snap), drawHelper);
        }

        public static Vector3 Do(int id, Vector3 handlePos, Vector3 offset, Vector3 handleDir, Vector3 slideDir1, Vector3 slideDir2, float handleSize, Handles.DrawCapFunction drawFunc, float snap, bool drawHelper)
        {
            return Do(id, handlePos, offset, handleDir, slideDir1, slideDir2, handleSize, drawFunc, new Vector2(snap, snap), drawHelper);
        }

        public static Vector3 Do(int id, Vector3 handlePos, Vector3 offset, Vector3 handleDir, Vector3 slideDir1, Vector3 slideDir2, float handleSize, Handles.DrawCapFunction drawFunc, Vector2 snap, bool drawHelper)
        {
            bool changed = GUI.changed;
            GUI.changed = false;
            Vector2 vector = CalcDeltaAlongDirections(id, handlePos, offset, handleDir, slideDir1, slideDir2, handleSize, drawFunc, snap, drawHelper);
            if (GUI.changed)
            {
                handlePos = (s_StartPosition + (slideDir1 * vector.x)) + (slideDir2 * vector.y);
            }
            GUI.changed |= changed;
            return handlePos;
        }
    }
}