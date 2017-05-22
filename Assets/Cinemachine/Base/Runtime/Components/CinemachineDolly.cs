using UnityEngine;
using System;
using Cinemachine.Utility;

namespace Cinemachine
{
    /// <summary>
    /// A Cinemachine Virtual Camera Body component that constrains camera motion 
    /// to a <see cref="CinemachinePath"/>
    /// </summary>
    [AddComponentMenu("")] // Don't display in add component menu
    [RequireComponent(typeof(CinemachinePipeline))]
    public class CinemachineDolly : MonoBehaviour, ICinemachineComponent
    {
        /// <summary>The path to which the camera will be constrained.  This must be non-null.</summary>
        [Tooltip("The path to which the camera will be constrained.  This must be non-null.")]
        public CinemachinePath m_Path;

        /// <summary>The position along the path at which the camera will be placed.  
        /// This can be animated directly, or set automatically by the Auto-Dolly feature 
        /// to get as close as possible to the Follow target.</summary>
        [Tooltip("The position along the path at which the camera will be placed.  This can be animated directly, or set automatically by the Auto-Dolly feature to get as close as possible to the Follow target.")]
        public float m_PathPosition;

        /// <summary>Where to put the camera realtive to the path postion.  X is perpendicular to the path, Y is up, and Z is parallel to the path.</summary>
        [Tooltip("Where to put the camera realtive to the path postion.  X is perpendicular to the path, Y is up, and Z is parallel to the path.")]
        public Vector3 m_PathOffset = Vector3.zero;

        /// <summary>How aggressively the camera tries to maintain the offset perpendicular to the path.  
        /// Small numbers are more responsive, rapidly translating the camera to keep the target's 
        /// x-axis offset.  Larger numbers give a more heavy slowly responding camera. 
        /// Using different settings per axis can yield a wide range of camera behaviors</summary>
        [Range(0f, 20f)]
        [Tooltip("How aggressively the camera tries to maintain the offset perpendicular to the path.  Small numbers are more responsive, rapidly translating the camera to keep the target's x-axis offset.  Larger numbers give a more heavy slowly responding camera. Using different settings per axis can yield a wide range of camera behaviors.")]
        public float m_XDamping = 0f;

        /// <summary>How aggressively the camera tries to maintain the offset in the path-local up direction.  
        /// Small numbers are more responsive, rapidly translating the camera to keep the target's 
        /// y-axis offset.  Larger numbers give a more heavy slowly responding camera. 
        /// Using different settings per axis can yield a wide range of camera behaviors</summary>
        [Range(0f, 20f)]
        [Tooltip("How aggressively the camera tries to maintain the offset in the path-local up direction.  Small numbers are more responsive, rapidly translating the camera to keep the target's y-axis offset.  Larger numbers give a more heavy slowly responding camera. Using different settings per axis can yield a wide range of camera behaviors.")]
        public float m_YDamping = 0f;

        /// <summary>How aggressively the camera tries to maintain the offset parallel to the path.  
        /// Small numbers are more responsive, rapidly translating the camera to keep the 
        /// target's z-axis offset.  Larger numbers give a more heavy slowly responding camera. 
        /// Using different settings per axis can yield a wide range of camera behaviors</summary>
        [Range(0f, 20f)]
        [Tooltip("How aggressively the camera tries to maintain the offset parallel to the path.  Small numbers are more responsive, rapidly translating the camera to keep the target's z-axis offset.  Larger numbers give a more heavy slowly responding camera. Using different settings per axis can yield a wide range of camera behaviors.")]
        public float m_ZDamping = 1f;

        /// <summary>How to set the camera's Up vector.  If checked, Camera up will follow the path up</summary>
        [Tooltip("How to set the camera's Up vector.  If checked, Camera up will follow the path up")]
        public bool m_RollWithPath = false;

        /// <summary>Controls how automatic dollying occurs</summary>
        [Serializable]
        public struct AutoDolly
        {
            /// <summary>If checked, will enable automatic dolly, which chooses a path position 
            /// that is as close as possible to the Follow target.</summary>
            [Tooltip("If checked, will enable automatic dolly, which chooses a path position that is as close as possible to the Follow target.  Note: this can have significant performance impact")]
            public bool m_Enabled;

            /// <summary>How many segments on either side of the current segment.  Use 0 for Entire path</summary>
            [Tooltip("How many segments on either side of the current segment.  Use 0 for Entire path.")]
            public int m_SearchRadius;

            /// <summary>We search a segment by dividing it into this many straight pieces.  
            /// The higher the number, the more accurate the result, but performance is 
            /// proportionally slower for higher numbers</summary>
            [Tooltip("We search a segment by dividing it into this many straight pieces.  The higher the number, the more accurate the result, but performance is proportionally slower for higher numbers")]
            public int m_StepsPerSegment;
#if false
            /// <summary>Wait this long after target has moved before beginning to dolly.</summary>
            [Tooltip("Wait this long after target has moved before beginning to dolly.")]
            public float m_WaitTime;

            /// <summary>Maximum speed of Dollying.  Will accelerate into and decelerate out of this</summary>
            [Tooltip("Maximum speed of Dollying.  Will accelerate into and decelerate out of this.")]
            public float m_MaxSpeed;

            /// <summary>The amount of time in seconds it takes to accelerate to MaxSpeed</summary>
            [Tooltip("The amount of time in seconds it takes to accelerate to MaxSpeed")]
            public float m_AccelTime;

            /// <summary>The amount of time in seconds it takes to decelerate the axis to zero position</summary>
            [Tooltip("The amount of time in seconds it takes to decelerate the axis to zero.position")]
            public float m_DecelTime;

            private float mCurrentSpeed;
#endif
            /// <summary>Constructor with specific field values</summary>
            public AutoDolly(
                bool enabled, int searchRadius, int stepsPerSegment, 
                float waitTime,  float maxSpeed, 
                float accelTime, float decelTime)
            {
                m_Enabled = enabled;
                m_SearchRadius = searchRadius;
                m_StepsPerSegment = stepsPerSegment;
                //m_WaitTime = waitTime;
                //m_MaxSpeed = maxSpeed;
                //m_AccelTime = accelTime;
                //m_DecelTime = decelTime;
                //mCurrentSpeed = 0;
            }
        };

        /// <summary>Controls how automatic dollying occurs</summary>
        [Tooltip("Controls how automatic dollying occurs.  A Follow target is necessary to use this feature.")]
        public AutoDolly m_AutoDolly = new AutoDolly(false, 2, 5, 0, 2f, 1, 1);

        /// <summary>True if component is enabled and has a path</summary>
        public bool IsValid { get { return enabled && m_Path != null; } }
        
        /// <summary>Get the <see cref="ICinemachineCamera"/> affected by this component</summary>
        public ICinemachineCamera VirtualCamera 
            { get { return gameObject.transform.parent.gameObject.GetComponent<ICinemachineCamera>(); } }

        /// <summary>Returns the Body stage</summary>
        public CinemachineCore.Stage Stage { get { return CinemachineCore.Stage.Body; } }

        /// <summary>Positions the virtual camera according to the transposer rules.</summary>
        /// <param name="curState">The current camera state</param>
        /// <param name="statePrevFrame">The camera state on the previous frame (unused)</param>
        /// <param name="deltaTime">Used for damping.  If 0 or less, no damping is done.</param>
        /// <returns>curState with new RawPosition</returns>
        public CameraState MutateCameraState(
            CameraState curState, CameraState statePrevFrame, float deltaTime)
        {
            if (!IsValid)
                return curState;

            if (deltaTime <= 0)
                m_PreviousPathPosition = m_PathPosition;

            CameraState newState = curState;

            // Get the new ideal path base position
            if (m_AutoDolly.m_Enabled)
                m_PathPosition = PerformAutoDolly(m_PreviousPathPosition, deltaTime);
            float newPathPosition = m_PathPosition;

            if (deltaTime > 0)
            {
                // Normalize previous position to find the shortest path
                if (m_Path.MaxPos > 0)
                {
                    float prev = m_Path.NormalizePos(m_PreviousPathPosition);
                    float next = m_Path.NormalizePos(newPathPosition);
                    if (m_Path.m_Looped && Mathf.Abs(next - prev) > m_Path.MaxPos/2)
                    {
                        if (next > prev)
                            prev += m_Path.MaxPos;
                        else
                            prev -= m_Path.MaxPos;
                    }
                    m_PreviousPathPosition = prev;
                    newPathPosition = next;
                }

                // Apply damping along the path direction
                float offset = m_PreviousPathPosition - newPathPosition;
                offset *= deltaTime / Mathf.Max(m_ZDamping * kDampingScale, deltaTime);
                newPathPosition = m_PreviousPathPosition - offset;
            }
            m_PreviousPathPosition = newPathPosition;
            Quaternion newPathOrientation = m_Path.EvaluateOrientation(newPathPosition); 

            // Apply the offset to get the new camera position
            Vector3 newCameraPos = m_Path.EvaluatePosition(newPathPosition);
            Vector3[] offsetDir = new Vector3[3];
            offsetDir[2] = newPathOrientation * Vector3.forward;
            offsetDir[1] = newPathOrientation * Vector3.up;
            offsetDir[0] = Vector3.Cross(offsetDir[1], offsetDir[2]);
            for (int i = 0; i < 3; ++i)
                newCameraPos += m_PathOffset[i] * offsetDir[i];

            // Apply damping to the remaining directions
            if (deltaTime > 0)
            {
                Vector3 currentCameraPos = statePrevFrame.RawPosition;
                Vector3 delta = (currentCameraPos - newCameraPos);
                Vector3 delta1 = Vector3.Dot(delta, offsetDir[1]) * offsetDir[1];
                Vector3 delta0 = delta - delta1;
                delta = delta0 * deltaTime / Mathf.Max(m_XDamping * kDampingScale, deltaTime)
                    + delta1 * deltaTime / Mathf.Max(m_YDamping * kDampingScale, deltaTime);
                newCameraPos = currentCameraPos - delta;
            }
            newState.RawPosition = newCameraPos;

            // Set the up
            if (m_RollWithPath)
                newState.ReferenceUp = newPathOrientation * Vector3.up;

            return newState;
        }

        private const float kDampingScale = 0.1f;
        private float m_PreviousPathPosition = 0;

        float PerformAutoDolly(float currentPos, float deltaTime)
        {
            if (m_AutoDolly.m_Enabled && VirtualCamera.Follow != null)
            {
                float pos = m_Path.FindClosestPoint(
                    VirtualCamera.Follow.transform.position,
                    Mathf.FloorToInt(currentPos),
                    (deltaTime <= 0 || m_AutoDolly.m_SearchRadius <= 0) ? -1 : m_AutoDolly.m_SearchRadius,
                    m_AutoDolly.m_StepsPerSegment);
                return pos;
            }
            return m_PathPosition;
        }
    }
}
