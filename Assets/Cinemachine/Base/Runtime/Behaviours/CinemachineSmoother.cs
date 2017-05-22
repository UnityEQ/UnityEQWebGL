using UnityEngine;
using System.Collections.Generic;
using Cinemachine.Utility;

namespace Cinemachine
{
    /// <summary>
    /// An add-on module for <see cref="CinemachineVirtualCameraBase"/> which post-processes 
    /// the final position and  orientation of the virtual camera, as a kind of low-pass filter.
    /// </summary>
    [ExecuteInEditMode]
    [AddComponentMenu("Cinemachine/Smoother")]
    public class CinemachineSmoother : MonoBehaviour
    {
        /// <summary>
        /// For reducing jitter, we apply a simple filter to the effect of the collider
        /// </summary>
        [Range(0f, 10f)]
        [Tooltip("The strength of the smoothing for position.  Higher numbers smooth more but reduce performance and introduce lag.")]
        public float m_PositionSmoothing = 1;

        /// <summary>
        /// For reducing jitter, we apply a simple filter to the effect of the collider
        /// </summary>
        [Range(0f, 10f)]
        [Tooltip("The strength of the smoothing for rotation.  Higher numbers smooth more but reduce performance and introduce lag.")]
        public float m_RotationSmoothing = 1;

        private void Start()
        {
            OnEnable();
        }

        private void OnEnable()
        {
            VirtualCamera = GetComponent<CinemachineVirtualCameraBase>();
            if (VirtualCamera == null)
            {
                CinemachineDebugLogger.LogError("CinemachineSmoother requires a Cinemachine Virtual Camera component");
                enabled = false;
            }
            else
            {
                VirtualCamera.AddPostPipelineStageHook(PostPipelineStageCallback);
                enabled = true;
            }
            mSmoothingFilter = null;
            mSmoothingFilterRotation = null;
        }

        private void OnDisable()
        {
            if (VirtualCamera != null)
                VirtualCamera.RemovePostPipelineStageHook(PostPipelineStageCallback);
        }

        /// <summary>Get the associated <see cref="CinemachineVirtualCameraBase"/>.</summary>
        public CinemachineVirtualCameraBase VirtualCamera { get; private set; }

        private void PostPipelineStageCallback(
            CinemachineVirtualCameraBase vcam,
            CinemachineCore.Stage stage, ref CameraState state, CameraState previousState, float deltaTime)
        {
            if (enabled)
            {
                if (stage == CinemachineCore.Stage.Body)
                {
                    if (m_PositionSmoothing > 0)
                        state.PositionCorrection 
                            += ApplySmoothing(vcam, state.CorrectedPosition) - state.CorrectedPosition;
                }
                if (stage == CinemachineCore.Stage.Aim)
                {
                    if (m_RotationSmoothing > 0)
                    {
                        Quaternion q = Quaternion.Inverse(state.CorrectedOrientation)
                            * ApplySmoothing(vcam, state.CorrectedOrientation, state.ReferenceUp);
                        state.OrientationCorrection = state.OrientationCorrection * q;
                    }
                }
            }
        }

        private Dictionary<CinemachineVirtualCameraBase, GaussianWindow1D_Vector3> mSmoothingFilter;
        private Vector3 ApplySmoothing(CinemachineVirtualCameraBase vcam, Vector3 pos)
        {
            if (mSmoothingFilter == null)
                mSmoothingFilter = new Dictionary<CinemachineVirtualCameraBase, GaussianWindow1D_Vector3>();
            GaussianWindow1D_Vector3 filter = null;
            if (!mSmoothingFilter.TryGetValue(vcam, out filter) || filter.Sigma != m_PositionSmoothing)
                mSmoothingFilter[vcam] = filter = new GaussianWindow1D_Vector3(m_PositionSmoothing);
            return filter.Filter(pos);
        }

        private Dictionary<CinemachineVirtualCameraBase, GaussianWindow1D_CameraRotation> mSmoothingFilterRotation;
        private Quaternion ApplySmoothing(CinemachineVirtualCameraBase vcam, Quaternion rot, Vector3 up)
        {
            if (mSmoothingFilterRotation == null)
                mSmoothingFilterRotation = new Dictionary<CinemachineVirtualCameraBase, GaussianWindow1D_CameraRotation>();
            GaussianWindow1D_CameraRotation filter = null;
            if (!mSmoothingFilterRotation.TryGetValue(vcam, out filter) || filter.Sigma != m_RotationSmoothing)
                mSmoothingFilterRotation[vcam] = filter = new GaussianWindow1D_CameraRotation(m_RotationSmoothing);

            Vector3 camRot = Quaternion.identity.GetCameraRotationToTarget(rot * Vector3.forward, up);
            return Quaternion.identity.ApplyCameraRotation(filter.Filter(camRot), up);
        }
    }
}
