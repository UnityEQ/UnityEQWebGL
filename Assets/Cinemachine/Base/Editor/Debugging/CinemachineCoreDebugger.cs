using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using UnityEngine;
using UnityEditor;
using System.Text;

namespace Cinemachine.Editor
{
    internal static class CinemachineCoreDebugger
    {
        private static Rect sDebuggerWindowRect = new Rect(0f, 0f, 300f, 200f);
        private static readonly Dictionary<string, Action> sAdditionalDebuggers 
            = new Dictionary<string, Action>();

        private static int sCurrentDebuggerIndex = 0;
        private static string[] sAttachedDebuggers = new string[0];

        private const string kMainDebuggerTitle = "Brain 0";

        private static HashSet<ICinemachineCamera> sDebuggedVirtualCameras 
            = new HashSet<ICinemachineCamera>();
        private static Dictionary<ICinemachineCamera, StreamWriter> sOpenWriters 
            = new Dictionary<ICinemachineCamera, StreamWriter>();

        public static Type[] sAllVirtualCameraTypes { get; private set; }
    
        static CinemachineCoreDebugger()
        {
            Type virtualCameraInterface = typeof(ICinemachineCamera);
            sAllVirtualCameraTypes 
                = Cinemachine.Utility.ReflectionHelpers.GetTypesInAllLoadedAssemblies(
                    (Type t) => Array.Exists(t.GetInterfaces(), 
                        (i) => i == virtualCameraInterface)).ToArray();

            EditorApplication.playmodeStateChanged += OnPlayStateChanged;
            RegisterDebugger(kMainDebuggerTitle, DrawCoreDebugger);
        }

        public static void AttachDebugger()
        {
            CinemachineBrain.CoreDebuggerUICallback = CinemachineCoreDebugUI;
            CinemachineBrain.CoreDebuggerPostFrameUpdateCallback = CinemachineCoreFrameDebug;
        }

        public static void RemoveDebugger()
        {
            CinemachineBrain.CoreDebuggerUICallback = null;
            CinemachineBrain.CoreDebuggerPostFrameUpdateCallback = null;
        }

        public static void RegisterDebugger(string debuggerKey, Action drawCallback)
        {
            if (!sAdditionalDebuggers.ContainsKey(debuggerKey))
            {
                List<string> tempList = new List<string>(sAdditionalDebuggers.Count + 1);
                tempList.AddRange(sAdditionalDebuggers.Keys);
                tempList.Add(debuggerKey);
                sAttachedDebuggers = tempList.ToArray();
            }
            sAdditionalDebuggers[debuggerKey] = drawCallback;
        }

        public static void RemoveDebugger(string debuggerKey)
        {
            if (sAdditionalDebuggers.ContainsKey(debuggerKey))
            {
                int index = Array.IndexOf(sAttachedDebuggers, debuggerKey);
                if (index == sCurrentDebuggerIndex)
                    sCurrentDebuggerIndex = 0;

                sAdditionalDebuggers.Remove(debuggerKey);
                List<string> tempList = new List<string>(sAdditionalDebuggers.Count);
                tempList.AddRange(sAdditionalDebuggers.Keys);
                sAttachedDebuggers = tempList.ToArray();
            }
        }

        private static string GetCameraDebugFilePath(ICinemachineCamera vCam)
        {
            return string.Format("{0}_debug.csv", vCam.Name);
        }

        private static void StartCameraDebugLogging(ICinemachineCamera vCam)
        {
            if (sDebuggedVirtualCameras.Add(vCam))
            {
                string filePath = GetCameraDebugFilePath(vCam);
                if (File.Exists(filePath))
                    File.Delete(filePath);

                StreamWriter writer = new StreamWriter(filePath);
                writer.WriteLine("Frame,DeltaTime,IsActive,x,y,z,tilt,pan,roll,lookat x, lookat y, lookat z");
                sOpenWriters[vCam] = writer;
            }
        }

        private static void StopCameraDebugLogging(ICinemachineCamera vCam)
        {
            if (sDebuggedVirtualCameras.Remove(vCam))
            {
                StreamWriter writer;
                if (sOpenWriters.TryGetValue(vCam, out writer))
                {
                    writer.Close();
                    sOpenWriters.Remove(vCam);
                }
            }
        }

        public static void StopAllCameraDebugLogging()
        {
            List<ICinemachineCamera> allCams = new List<ICinemachineCamera>(sDebuggedVirtualCameras);
            foreach (ICinemachineCamera cam in allCams)
            {
                if (cam != null)
                    StopCameraDebugLogging(cam);
            }
            
            foreach (StreamWriter openWriter in sOpenWriters.Values)
            {
                openWriter.Close();
            }

            sOpenWriters.Clear();
            sDebuggedVirtualCameras.Clear();
        }

        private static void OnPlayStateChanged()
        {
            StopAllCameraDebugLogging();
        }

         private static string StripParentheses(string s)
         {
             if (s.StartsWith ("(") && s.EndsWith (")"))
                 return s.Substring(1, s.Length-2);
             return s;
        }

        private static void CinemachineCoreFrameDebug()
        {
            StringBuilder builder = new StringBuilder();
            foreach (KeyValuePair<ICinemachineCamera, StreamWriter> debuggedCams in sOpenWriters)
            {
                builder.Length = 0;
                ICinemachineCamera vcam = debuggedCams.Key;
                bool isActiveCam = CinemachineCore.Instance.IsLive(vcam);
                CameraState state = vcam.State;

                //"Frame,DeltaTime,IsActive,x,y,z,pitch,yaw,roll,lookat x, lookat y, lookat z");
                builder.AppendFormat("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11}", 
                    Time.frameCount, 
                    Time.deltaTime, 
                    isActiveCam, 
                    state.FinalPosition.x, state.FinalPosition.y, state.FinalPosition.z,
                    state.FinalOrientation.x, state.FinalOrientation.y, state.FinalOrientation.z,
                    state.ReferenceLookAt.x, state.ReferenceLookAt.y, state.ReferenceLookAt.z);
                debuggedCams.Value.WriteLine(builder.ToString());
            }
        }

        private static void CinemachineCoreDebugUI()
        {
            sDebuggerWindowRect = GUILayout.Window(
                0xBEEF, sDebuggerWindowRect, DrawCoreDebugUI, "Cinemachine Debugger");
        }

        private static void DrawCoreDebugUI(int windowID)
        {
            if (sAttachedDebuggers.Length == 1)
            {
                DrawCoreDebugger();
            }
            else
            {
                sCurrentDebuggerIndex = GUILayout.Toolbar(sCurrentDebuggerIndex, sAttachedDebuggers);
                GUILayout.BeginVertical(GUI.skin.box);
                sAdditionalDebuggers[sAttachedDebuggers[sCurrentDebuggerIndex]]();
                GUILayout.EndVertical();
            }
        }

        private static void DrawCoreDebugger()
        {
            var allCams = CinemachineCore.Instance.AllCameras.ToArray<ICinemachineCamera>();
            int numVirtualCameras = allCams.Length;
            GUILayout.Label("Active Cameras");
            GUILayout.BeginHorizontal(GUI.skin.box);
            {
                GUILayout.BeginVertical();
                GUILayout.Label("Index");
                for (int i = 0; i < numVirtualCameras; ++i)
                    GUILayout.Label(i.ToString());
                GUILayout.EndVertical();

                GUILayout.BeginVertical();
                GUILayout.Label("Camera Name");
                for (int i = 0; i < numVirtualCameras; ++i)
                    GUILayout.Label(allCams[i].Name);
                GUILayout.EndVertical();

                GUILayout.BeginVertical();
                GUILayout.Label("Priority");
                for (int i = 0; i < numVirtualCameras; ++i)
                    GUILayout.Label(allCams[i].Priority.ToString());
                GUILayout.EndVertical();

                if (Application.isPlaying)
                {
                    GUILayout.BeginVertical();
                    GUILayout.Label("Logging");
                    foreach (ICinemachineCamera cam in allCams)
                    {
                        if (sDebuggedVirtualCameras.Contains(cam) && GUILayout.Button("Stop"))
                            StopCameraDebugLogging(cam);
                        else if (!sDebuggedVirtualCameras.Contains(cam) && GUILayout.Button("Start"))
                            StartCameraDebugLogging(cam);
                    }
                    GUILayout.EndVertical();
                }
            }
            GUILayout.EndHorizontal();

            foreach (CinemachineBrain brain in CinemachineCore.Instance.AllBrains)
            {
                if (brain == null)
                    continue;

                GUI.color = Color.white;
                if (brain.IsSuspended)
                {
                    GUI.color = Color.red;
                    GUILayout.Label("SUSPENDED");
                }
                GUILayout.Label("Blending");
                GUILayout.BeginVertical(GUI.skin.box);
                if (brain.IsBlending)
                {
                    float progress = brain.ActiveBlend.BlendWeight;
                    GUILayout.Label(brain.ActiveBlend.Description);
                    AnimationCurve blendCurve = brain.ActiveBlend.BlendCurve;
                    if (blendCurve != null)
                    {
                        EditorGUILayout.CurveField(blendCurve);
                        Rect drawRect = GUILayoutUtility.GetLastRect();
                        float xPx = progress * drawRect.width;
                        Color prevColour = GUI.color;
                        GUI.color = Color.red;
                        GUI.DrawTexture(new Rect(xPx + drawRect.xMin, drawRect.yMin, 1f, drawRect.height), Texture2D.whiteTexture);
                        GUI.color = prevColour;
                    }
                }
                else
                {
                    GUILayout.Label("No active blends");
                }
                GUILayout.EndVertical();
            }
        }
    }
}
