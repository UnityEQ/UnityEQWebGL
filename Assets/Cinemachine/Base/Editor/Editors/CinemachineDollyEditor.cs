using UnityEngine;
using UnityEditor;

namespace Cinemachine.Editor
{
    [CustomEditor(typeof(CinemachineDolly))]
    internal sealed class CinemachineDollyEditor : UnityEditor.Editor
    {
        private CinemachineDolly Target { get { return target as CinemachineDolly; } }
        private static readonly string[] m_excludeFields = new string[] { "m_Script" };

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            DrawPropertiesExcluding(serializedObject, m_excludeFields);
            serializedObject.ApplyModifiedProperties();
        }

        [DrawGizmo(GizmoType.Active | GizmoType.InSelectionHierarchy, typeof(CinemachineDolly))]
        private static void DrawTransposerGizmos(CinemachineDolly target, GizmoType selectionType)
        {
            if (target.IsValid)
            {
                CinemachinePathEditor.DrawPathGizmos(target.m_Path, selectionType);
            }
        }
    }
}
