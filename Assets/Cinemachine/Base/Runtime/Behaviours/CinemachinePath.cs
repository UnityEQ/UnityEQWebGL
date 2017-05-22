using UnityEngine;
using System.Collections.Generic;
using System;
using Cinemachine.Utility;

namespace Cinemachine
{
    /// <summary>Defines a world-space path, consisting of an array of waypoints, 
    /// each of which has position, tangent, and roll settings.  Bezier interpolation
    /// is performed between the waypoints, to get a smooth and continuous path.</summary>
    [AddComponentMenu("Cinemachine/Path")]
    public class CinemachinePath : MonoBehaviour
    {
        /// <summary>This class holds the settings that control how the path
        /// will appear in the editor scene view.</summary>
        [Serializable] public class Appearance
        {
            /// <summary>The color of the path itself when it is active in the editor</summary>
            public Color pathColor = Color.green;
            /// <summary>The color of the path itself when it is inactive in the editor</summary>
            public Color inactivePathColor = Color.gray;
            /// <summary>The color of tanhent controls on the waypoints</summary>
            public Color handleColor = Color.yellow;
            [Range(0f, 10f)]
            /// <summary>The width of the railroad-tracks that are drawn to represent the path</summary>
            public float width = 0.2f;
            /// <summary>How many steps are taken between the waypoints when the path is 
            /// drawn in the editor.  Each step is marked by a perpendicular line, 
            /// like a railway tie.</summary>
            [Range(1, 100)]
            public int steps = 20;
        }
        /// <summary>The settings that control how the path
        /// will appear in the editor scene view.</summary>
        [Tooltip("The settings that control how the path will appear in the editor scene view.")]
        public Appearance m_Appearance;

        /// <summary>A waypoint along the path</summary>
        [Serializable] public class Waypoint
        {
            /// <summary>Position in path-local space</summary>
            public Vector3 position;
            /// <summary>Offset from the position, which defines the tangent of the curve at 
            /// the waypoint.  The length of the tangent encodes the strength of the 
            /// bezier handle.  The same handle is used symmetrically on both sides of 
            /// the waypoint, to ensure smoothness.</summary>
            public Vector3 tangent;
            /// <summary>Defines the roll of the path at this waypoint.  The other orientation
            /// axes are inferred from the tangent and world up.</summary>
            public float roll;
        }

        /// <summary>If checked, then the path ends are joined to form a continuous loop</summary>        
        [Tooltip("If checked, then the path ends are joined to form a continuous loop.")]
        public bool m_Looped;

        /// <summary>The waypoints that define the path.  
        /// They will be interpolated using a bezier curve</summary>
        [Tooltip("The waypoints that define the path.  They will be interpolated using a bezier curve.")]
        public List<Waypoint> m_Waypoints = new List<Waypoint>();

        /// <summary>The minimum value for the path position</summary>
        public float MinPos { get { return 0; } }

        /// <summary>The maximum value for the path position</summary>
        public float MaxPos 
        { 
            get 
            { 
                int count = m_Waypoints.Count-1;
                if (count < 1)
                    return 0;
                return m_Looped ? count + 1 : count; 
            }
        }

        /// <summary>Get a normalized path position, taking spins into account if looped</summary>
        /// <param name="pos">Position along the path</param>
        /// <returns>Norlamized position, between MinPos and MaxPos</returns>
        public float NormalizePos(float pos)
        {
            if (MaxPos == 0)
                return 0;
            if (m_Looped)
            {
                pos = pos % MaxPos;
                if (pos < 0) 
                    pos += MaxPos;
                return pos;
            }
            return Mathf.Clamp(pos, 0, MaxPos);
        }

        /// <summary>Returns normalized position</summary>
        float GetBoundingIndices(float pos, out int indexA, out int indexB)
        {
            pos = NormalizePos(pos);
            int rounded = Mathf.RoundToInt(pos);
            if (Mathf.Abs(pos - rounded) < UnityVectorExtensions.Epsilon)
                indexA = indexB = (rounded == m_Waypoints.Count) ? 0 : rounded;
            else 
            {
                indexA = Mathf.FloorToInt(pos);
                if (indexA >= m_Waypoints.Count)
                {
                    pos -= MaxPos;
                    indexA = 0;
                }
                indexB = Mathf.CeilToInt(pos);
                if (indexB >= m_Waypoints.Count)
                    indexB = 0;
            }
            return pos;
        }

        /// <summary>Get a worldspace position of a point along the path</summary>
        /// <param name="pos">Postion along the path.  Need not be normalized.</param>
        /// <returns>World-space position of the point along at path at pos</returns>
        public Vector3 EvaluatePosition(float pos)
        {
            Vector3 result = new Vector3();
            if (m_Waypoints.Count == 0)
                result = transform.position;
            else 
            {
                int indexA, indexB;
                pos = GetBoundingIndices(pos, out indexA, out indexB);
                if (indexA == indexB)
                    result = m_Waypoints[indexA].position;
                else 
                {
                    // interpolate
                    Waypoint wpA = m_Waypoints[indexA];
                    Waypoint wpB = m_Waypoints[indexB];
                    float t = pos - indexA;
		            float d = 1f - t;
                    Vector3 ctrl1 = wpA.position + wpA.tangent;
                    Vector3 ctrl2 = wpB.position - wpB.tangent;
		            result = d * d * d * wpA.position + 3f * d * d * t * ctrl1 
                        + 3f * d * t * t * ctrl2 + t * t * t * wpB.position;
                }
            }
            return transform.TransformPoint(result);
        }

        /// <summary>Get the tangent of the curve at a point along the path.</summary>
        /// <param name="pos">Postion along the path.  Need not be normalized.</param>
        /// <returns>World-space direction of the path tangent.  
        /// Length of the vector represents the tangent strength</returns>
        public Vector3 EvaluateTangent(float pos)
        {
            Vector3 result = new Vector3();
            if (m_Waypoints.Count == 0)
                result = transform.rotation * Vector3.forward;
            else 
            {
                int indexA, indexB;
                pos = GetBoundingIndices(pos, out indexA, out indexB);
                if (indexA == indexB)
                    result = m_Waypoints[indexA].tangent;
                else 
                {
                    Waypoint wpA = m_Waypoints[indexA];
                    Waypoint wpB = m_Waypoints[indexB];
                    float t = pos - indexA;
                    Vector3 ctrl1 = wpA.position + wpA.tangent;
                    Vector3 ctrl2 = wpB.position - wpB.tangent;
		            result = (-3f * wpA.position + 9f * ctrl1 - 9f * ctrl2 + 3f * wpB.position) * t * t
			            +  (6f * wpA.position - 12f * ctrl1 + 6f * ctrl2) * t
			            -  3f * wpA.position + 3f * ctrl1;
                }
            }
            return transform.TransformDirection(result);
        }

        /// <summary>Get the orientation the curve at a point along the path.</summary>
        /// <param name="pos">Postion along the path.  Need not be normalized.</param>
        /// <returns>World-space orientation of the path, as defined by tangent, up, and roll.<returns>
        public Quaternion EvaluateOrientation(float pos)
        {
            Quaternion result = transform.rotation;
            if (m_Waypoints.Count > 0)
            {
                float roll = 0;
                int indexA, indexB;
                pos = GetBoundingIndices(pos, out indexA, out indexB);
                if (indexA == indexB)
                    roll = m_Waypoints[indexA].roll;
                else 
                {
                    float rollA = m_Waypoints[indexA].roll;
                    float rollB = m_Waypoints[indexB].roll;
                    if (indexB == 0)
                    {
                        // Special handling at the wraparound - cancel the spins
                        rollA = rollA % 360;
                        rollB = rollB % 360;
                    }
                    roll = Mathf.Lerp(rollA, rollB, pos - indexA);
                }

                Vector3 fwd = EvaluateTangent(pos).normalized;
                Vector3 up = transform.rotation * Vector3.up;
                Quaternion q = Quaternion.LookRotation(fwd, up);
                result = Quaternion.AngleAxis(roll, fwd) * q;
            }
            return result;
        }

        /// <summary>Find the closest point on the path to a given worldspace target point.</summary>
        /// <remarks>Performance could be improved by checking the bounding polygon of each segment, 
        /// and only entering the best segment(s)</remarks>
        /// <param name="p">Worldspace target that we want to approach</param>
        /// <param name="startSegment">In what segment of the path to start the search</param>
        /// <param name="searchRadius">How many segments on either side of the startSegment 
        /// to search.  -1 means no limmit, i.e. search the entire path</param>
        /// <param name="stepsPerSegment">We search a segment by dividing it into this many 
        /// straight pieces.  The higher the number, the more accurate the result, but performance
        /// is proportionally slower for higher numbers</param>
        /// <returns>The position along the path that is closest to the target point</returns>
        public float FindClosestPoint(Vector3 p, int startSegment, int searchRadius, int stepsPerSegment)
        {
            float start = MinPos;
            float end = MaxPos;
            if (searchRadius >= 0)
            {
                float r = Mathf.Min(searchRadius, (end-start) / 2f);
                start = startSegment - r;
                end = startSegment + r + 1;
            }
            stepsPerSegment = Mathf.RoundToInt(Mathf.Clamp(stepsPerSegment, 1f, 100f));
            float stepSize = 1f / stepsPerSegment;
            float bestPos = startSegment;
            float bestDistance = float.MaxValue;
            int iterations = (stepsPerSegment == 1) ? 1 : 3;
            for (int i = 0; i < iterations; ++i)
            {
                Vector3 v0 = EvaluatePosition(start);
                for (float f = start + stepSize; f <= end; f += stepSize)
                {
                    Vector3 v = EvaluatePosition(f);
                    float t = p.ClosestPointOnSegment(v0, v);
                    float d = Vector3.SqrMagnitude(p - Vector3.Lerp(v0, v, t));
                    if (d < bestDistance)
                    {
                        bestDistance = d;
                        bestPos = f - (1-t) * stepSize;
                    }
                    v0 = v;
                }
                start = bestPos - stepSize;
                end = bestPos + stepSize;
                stepSize /= stepsPerSegment;
            }
            return bestPos;
        }
    }
}
