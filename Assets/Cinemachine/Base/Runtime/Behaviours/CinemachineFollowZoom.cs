using UnityEngine;
using Cinemachine.Utility;

namespace Cinemachine
{
    /// <summary>
    /// An add-on module for <see cref="CinemachineVirtualCameraBase"/> which adjusts
    /// the FOV of the lens to keep the target object at a constant size on the screen, 
    /// regardless of camera position.
    /// </summary>
    [ExecuteInEditMode]
    [AddComponentMenu("Cinemachine/FollowZoom")]
    public class CinemachineFollowZoom : MonoBehaviour
    {
        /// <summary>The shot width to maintain, in world units, at target distance.
        /// FOV will be adusted as far as possible to maintain this width at the 
        /// target distance from the camera.</summary>
        [Tooltip("The shot width to maintain, in world units, at target distance.")]
        public float m_Width = 2f;

        private void Start()
        {
            OnEnable();
        }

        private void OnEnable()
        {
            VirtualCamera = GetComponent<CinemachineVirtualCameraBase>();
            if (VirtualCamera == null)
            {
                CinemachineDebugLogger.LogError("CinemachineFollowZoom requires a Cinemachine Virtual Camera component");
                enabled = false;
            }
            else
            {
                VirtualCamera.AddPostPipelineStageHook(PostPipelineStageCallback);
                enabled = true;
            }
        }

        private void OnDisable()
        {
            if (VirtualCamera != null)
                VirtualCamera.RemovePostPipelineStageHook(PostPipelineStageCallback);
        }

        /// <summary>Cache of the <see cref="CinemachineVirtualCameraBase"/> component</summary>
        public CinemachineVirtualCameraBase VirtualCamera { get; private set; }

        private void PostPipelineStageCallback(
            CinemachineVirtualCameraBase vcam,
            CinemachineCore.Stage stage, ref CameraState state, CameraState previousState, float deltaTime)
        {
            if (enabled)
            {
                // Set the zoom after the body has been positioned, but before the aim,
                // so that composer can compose using the updated fov.
                if (stage == CinemachineCore.Stage.Body)
                {
                    // Try to reproduce the target width
                    float width = Mathf.Max(m_Width, 0);
                    float fov = 179f;
                    float d = Vector3.Distance(state.CorrectedPosition, state.ReferenceLookAt);
                    if (d > UnityVectorExtensions.Epsilon)
                        fov = 2f * Mathf.Atan(width / (2 * d)) * Mathf.Rad2Deg;
                    LensSettings lens = state.Lens;
                    lens.FieldOfView = Mathf.Clamp(fov, 1, 179f);
                    state.Lens = lens;
                }
            }
        }
    }
}
