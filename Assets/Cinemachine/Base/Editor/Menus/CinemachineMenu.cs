using UnityEngine;
using UnityEditor;
using System;

namespace Cinemachine.Editor
{
    public static class CinemachineMenu
    {
        public const string kCinemachineRootMenu = "Assets/Create/Cinemachine/";
        [MenuItem(kCinemachineRootMenu + "Blender/Settings")]
        private static void CreateBlenderSettingAsset()
        {
            ScriptableObjectUtility.Create<CinemachineBlenderSettings>();
        }

        [MenuItem(kCinemachineRootMenu + "Noise/Settings")]
        private static void CreateNoiseSettingAsset()
        {
            ScriptableObjectUtility.Create<NoiseSettings>();
        }

#if false
        [MenuItem("Cinemachine/Enable Timeline integration in pre-2017.x Unity versions", false, 1)]
        private static void EnablePostTimelineIntegration()
        {
            string msg = "Note: This feature is dependent on the presence of the Timeline feature in Unity.";
            msg += "\n\nIf you are not using a timeline-enabled build of Unity, please press Cancel.  Otherwise, you can proceed.";
            if (EditorUtility.DisplayDialog("Cinemachine Setup", msg, "Proceed", "Cancel"))
            {
                int numAdded = SetPlayerSettingDefineInAllBuildTargets("CINEMACHINE_TIMELINE");
                if (numAdded == 0)
                    msg = "CINEMACHINE_TIMELINE was already enabled for all build targets.";
                else
                    msg = "CINEMACHINE_TIMELINE has been added to the PlayerSettings/ScriptingDefineSymbols to " + numAdded + " build targets, wherever it was absent.";
                msg += "\n\nNote: This feature is dependent on a Timeline-enabled build of Unity.";
                msg += "\n\nIf you are getting console errors originating in the Cinemachine/Timeline cs files, it is most likely because you do not have an appropriate build of Unity.";
                msg += "\n\nYou can correct this by upgrading Unity, or by removing the CINEMACHINE_TIMELINE define, or by simply deleting the Cinemachine/Timeline asset from your project.";
                EditorUtility.DisplayDialog("Cinemachine Setup", msg, "Got it");
            }
        }

        static int SetPlayerSettingDefineInAllBuildTargets(string d)
        {
            int numAdded = 0;
            foreach (BuildTargetGroup t in Enum.GetValues(typeof(BuildTargetGroup)))
            {
                if (t == BuildTargetGroup.Unknown)
                    continue;

                // Skip the obsolete ones
                System.Reflection.MemberInfo mi = typeof(BuildTargetGroup).GetMember(t.ToString())[0];
                if (mi.GetCustomAttributes(typeof(ObsoleteAttribute), false).Length > 0)
                    continue;

                string all = PlayerSettings.GetScriptingDefineSymbolsForGroup(t);
                string[] defs = all.Split(';');
                bool foundIt = false;
                foreach (string s in defs)
                    if (s.Trim() == d)
                        foundIt = true;
                if (!foundIt)
                {
                    ++numAdded;
                    if (all.Trim().Length > 0)
                        all += ";";
                    all += d;
                    PlayerSettings.SetScriptingDefineSymbolsForGroup(t, all);
                }
            }
            return numAdded;
        }
#endif

        [MenuItem("Cinemachine/Create Virtual Camera", false, 1)]
        private static void CreateDefaultNewVirtualCamera()
        {
            CreateDefaultVirtualCamera();
        }

        [MenuItem("Cinemachine/Create FreeLook Camera", false, 1)]
        private static void CreateFreeLookCamera()
        {
            CreateCameraBrainIfAbsent();
            GameObject go = new GameObject(
                GenerateUniqueObjectName(typeof(CinemachineFreeLook), "FreeLook"));
            Undo.RegisterCreatedObjectUndo(go, "create FreeLook");
            Undo.AddComponent<CinemachineFreeLook>(go);
        }

        [MenuItem("Cinemachine/Create State-driven Camera", false, 1)]
        private static void CreateStateDivenCamera()
        {
            // Create a new virtual camera cloud
            CreateCameraBrainIfAbsent();
            GameObject go = new GameObject(
                GenerateUniqueObjectName(typeof(CinemachineStateDrivenCamera), "StateDrivenCamera"));
            Undo.RegisterCreatedObjectUndo(go, "create state driven camera");
            Undo.AddComponent<CinemachineStateDrivenCamera>(go);
            // Give it a child
            CreateDefaultVirtualCamera().gameObject.transform.parent = go.transform;
        }

        [MenuItem("Cinemachine/Create Camera Cloud", false, 1)]
        private static void CreateVirtualCameraCloud()
        {
            // Create a new virtual camera cloud
            CreateCameraBrainIfAbsent();
            GameObject go = new GameObject(
                GenerateUniqueObjectName(typeof(CinemachineCloud), "VirtualCameraCloud"));
            Undo.RegisterCreatedObjectUndo(go, "create camera cloud");
            Undo.AddComponent<CinemachineCloud>(go);
            // Give it a child
            CreateDefaultVirtualCamera().gameObject.transform.parent = go.transform;
        }

        /// <summary>
        /// Create a default Virtual Camera, with standard components
        /// </summary>
        public static CinemachineVirtualCameraBase CreateDefaultVirtualCamera()
        {
            // Create a new virtual camera
            CreateCameraBrainIfAbsent();
            GameObject go = new GameObject(
                GenerateUniqueObjectName(typeof(CinemachineVirtualCamera), "VirtualCamera"));
            Undo.RegisterCreatedObjectUndo(go, "create virtual camera");
            CinemachineVirtualCamera vcam = Undo.AddComponent<CinemachineVirtualCamera>(go);
            vcam.GetComponentOwner();  // force it to update its cache
            return vcam;
        }

        /// <summary>
        /// If there is no CinemachineBrain in the scene, try to create one on the main camera
        /// </summary>
        public static void CreateCameraBrainIfAbsent()
        {
            CinemachineBrain[] brains = UnityEngine.Object.FindObjectsOfType(
                typeof(CinemachineBrain)) as CinemachineBrain[];
            if (brains == null || brains.Length == 0)
            {
                Camera cam = Camera.main;
                if (cam == null)
                {
                    Camera[] cams = UnityEngine.Object.FindObjectsOfType(
                        typeof(Camera)) as Camera[];
                    if (cams != null && cams.Length > 0)
                        cam = cams[0];
                }
                if (cam != null)
                {
                    Undo.AddComponent<CinemachineBrain>(cam.gameObject);
                }
            }
        }

        /// <summary>
        /// Generate a unique name with the given prefix by adding a suffix to it
        /// </summary>
        public static string GenerateUniqueObjectName(Type type, string prefix)
        {
            int count = 0;
            UnityEngine.Object[] all = Resources.FindObjectsOfTypeAll(type);
            foreach (UnityEngine.Object o in all)
            {
                if (o != null && o.name.StartsWith(prefix))
                {
                    string suffix = o.name.Substring(prefix.Length);
                    int i;
                    if (Int32.TryParse(suffix, out i) && i > count)
                        count = i;
                }
            }
            return prefix + (count + 1);
        }
    }
}
