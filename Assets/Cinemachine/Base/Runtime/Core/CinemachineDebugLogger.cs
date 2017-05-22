using UnityEngine;

using System.Linq;
using System.Text;
using System.Reflection;
using System.Collections.Generic;

namespace Cinemachine.Utility
{
    /// <summary>
    /// Sends well formatted messages to Unity's console. Ensuring that 
    /// Cinemachine logs are not confused with the client code's.
    /// </summary>
    public static class CinemachineDebugLogger
    {
        private static readonly StringBuilder sLogStringBuilder = new StringBuilder(1024);
        public static readonly string kVersionString = "2.0";

        static CinemachineDebugLogger()
        {
            AssemblyInformationalVersionAttribute versionInformation 
                = (AssemblyInformationalVersionAttribute)Assembly.GetExecutingAssembly().GetCustomAttributes(
                    typeof(AssemblyInformationalVersionAttribute), false).FirstOrDefault();
            if (versionInformation != null)
            {
                kVersionString = versionInformation.InformationalVersion;
            }
        }

        public static void LogInfo(string message)
        {
            Debug.Log(FormatMessage(message));
        }

        public static void LogInfo(string message, params object[] objects)
        {
            Debug.Log(FormatMessage(message, objects));
        }

        public static void LogInfo(Object context, string message)
        {
            Debug.Log(FormatMessage(message), context);
        }

        public static void LogInfo(Object context, string message, params object[] objects)
        {
            Debug.LogFormat(context, FormatMessage(message, objects));
        }

        public static void LogWarn(string message)
        {
            Debug.LogWarning(FormatMessage(message));
        }

        public static void LogWarn(string message, params object[] objects)
        {
            Debug.LogWarning(FormatMessage(message, objects));
        }

        public static void LogWarn(Object context, string message)
        {
            Debug.Log(FormatMessage(message), context);
        }

        public static void LogWarn(Object context, string message, params object[] objects)
        {
            Debug.LogFormat(context, FormatMessage(message, objects));
        }

        public static void LogError(string message)
        {
            Debug.LogError(FormatMessage(message));
        }

        public static void LogError(string message, params object[] objects)
        {
            Debug.LogError(FormatMessage(message, objects));
        }

        public static void LogError(Object context, string message)
        {
            Debug.Log(FormatMessage(message), context);
        }

        public static void LogError(Object context, string message, params object[] objects)
        {
            Debug.LogFormat(context, FormatMessage(message, objects));
        }

        private static string FormatMessage(string message)
        {
            sLogStringBuilder.Length = 0;
            sLogStringBuilder.AppendFormat("[CINEMACHINE v{0}]: {1}", kVersionString, message);
            return sLogStringBuilder.ToString();
        }

        private static string FormatMessage(string message, params object[] objects)
        {
            sLogStringBuilder.Length = 0;
            sLogStringBuilder.AppendFormat("[CINEMACHINE v{0}]: {1}", kVersionString, string.Format(message, objects));
            return sLogStringBuilder.ToString();
        }
    }

    /// <summary>Manages onscreen positions for Cinemachine debugging output</summary>
    public class CinemachineGameWindowDebug
    {
        static HashSet<Object> mClients;

        /// <summary>Release a screen rectangle previously obtained through GetScreenPos()</summary>
        /// <param name="client">The client caller.  Used as a handle.</param>
        public static void ReleaseScreenPos(Object client)
        {
            if (mClients != null && mClients.Contains(client))
                mClients.Remove(client);
        }

        /// <summary>Reserve an on-screen rectangle for debugging output.</summary>
        /// <param name="client">The client caller.  This is used as a handle.</param>
        /// <param name="text">Sample text, for determining rectangle size</param>
        /// <param name="style">What style will be used to draw, used here for 
        /// determining rect size</param>
        /// <returns>An area on the game screen large enough to print the text 
        /// in the style indicated</returns>
        public static Rect GetScreenPos(Object client, string text, GUIStyle style)
        {
            if (mClients == null)
                mClients = new HashSet<Object>();
            if (!mClients.Contains(client))
                mClients.Add(client);

            Vector2 pos = new Vector2(0, 0);
            Vector2 size = style.CalcSize(new GUIContent(text));
            if (mClients != null)
            {
                foreach (var c in mClients)
                {
                    if (c == client)
                        break;
                    pos.y += size.y;
                }
            }
            return new Rect(pos, size);
        }
    }
}
