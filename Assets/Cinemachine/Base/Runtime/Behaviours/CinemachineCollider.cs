using UnityEngine;
using System.Collections.Generic;
using Cinemachine.Utility;

namespace Cinemachine
{
    /// <summary>
    /// An add-on module for <see cref="CinemachineVirtualCameraBase"/> which post-processes 
    /// the final position of the virtual camera. Based on the supplied settings, 
    /// the <see cref="CinemachineCollider"/> will attempt to preserve the line of sight 
    /// with the compositional target of the <see cref="CinemachineVirtualCameraBase"/> and/or 
    /// keep a certain distance away from objects around the <see cref="CinemachineVirtualCameraBase"/>.
    /// </summary>
    [ExecuteInEditMode]
    [AddComponentMenu("Cinemachine/Collider")]
    public class CinemachineCollider : MonoBehaviour
    {
        /// <summary>
        /// The Unity layer mask by which the collider will raycast against. 
        /// </summary>
        [Tooltip("The Unity layer mask by which the collider will raycast against. ")]
        public LayerMask m_CollideAgainst = 1;

        /// <summary>
        /// When <b>TRUE</b>, will move the camera in front of anything which intersects the ray 
        /// based on the supplied layer mask and within the line of sight feeler distance
        /// </summary>
        [Tooltip("When enabled, will move the camera in front of anything which intersects the Line of Sight feeler")]
        public bool m_PreserveLineOfSight = true;

        /// <summary>
        /// The raycast distance to test for when checking if the line of sight to this camera's target is clear.
        /// </summary>
        [Tooltip("The raycast distance to test for when checking if the line of sight to this camera's target is clear.")]
        [Min(0)]
        public float m_LineOfSightFeelerDistance = 3f;

        /// <summary>
        /// Never get closer to the target than this.
        /// </summary>
        [Tooltip("Never get closer to the target than this.")]
        [Min(0)]
        public float m_MinimumDistanceFromTarget = 2f;

        /// <summary>
        /// When <b>TRUE</b>, will push the camera away from any feeler which raycasts against an 
        /// object within the feeler ray distance
        /// </summary>
        [Tooltip("When enabled, will push the camera away from any object touching a curb feeler")]
        public bool m_UseCurbFeelers = true;

        /// <summary>
        /// The raycast distance used to check if the camera is colliding against objects in the world.
        /// </summary>
        [Tooltip("The raycast distance used to check if the camera is colliding against objects in the world.")]
        [Min(0)]
        public float m_CurbFeelerDistance = 2f;

        /// <summary>
        /// The firmness by which the camera collider will push back against any object it is colliding with
        /// </summary>
        [Range(1f, MaxCurbResistance)]
        [Tooltip("The firmness with which the collider will push back against any object")]
        public float m_CurbResistance = 1f;

        /// <summary>
        /// For reducing jitter, we apply a simple position filter to the effect of the collider.
        /// This duplicates the functionality of <see cref="CinemachineSmoother"/>
        /// </summary>
        [Range(0f, 10f)]
        [Tooltip("The strength of the jitter reduction for position.  Higher numbers smooth more but reduce performance and introduce lag.")]
        public float m_PositionSmoothing = 0;

        /// <summary>API for the Editor to draw gizmos.</summary>
        public struct CompiledCurbFeeler
        {
            public readonly Vector3 LocalVector;
            public readonly float RayDistance;
            public readonly float DampingConstant;
            public bool IsHit;
            public float HitDistance;

            public CompiledCurbFeeler(Vector3 localDirection, float rayDistance, float dampingConstant)
            {
                LocalVector = localDirection;
                RayDistance = rayDistance;
                DampingConstant = dampingConstant;
                IsHit = false;
                HitDistance = float.MaxValue;
            }
        }
        /// <summary>API for the Editor to draw gizmos.</summary>
        public IEnumerable<CompiledCurbFeeler> Feelers { get { return mCompiledFeelers; } }

        /// <summary>Get the associated <see cref="CinemachineVirtualCameraBase"/>.</summary>
        public CinemachineVirtualCameraBase VirtualCamera { get; private set; }

        /// <summary>
        /// If line of sight (LoS) checks are enabled, will be <b>TRUE</b> if 
        /// the <see cref="CinemachineCollider"/> moved the <see cref="CinemachineVirtualCameraBase"/> due 
        /// to a blockage in LoS. <b>FALSE</b> otherwise.
        /// </summary>
        public bool IsTargetObscured { get; private set; }

        /// <summary>
        /// Distance that the camera was moved as a result of collision resolution.
        /// </summary>
        public float ColliderDisplacement { get; private set; }

        private CompiledCurbFeeler[] mCompiledFeelers = null;

        private static readonly Vector3 kLocalUpRight = (Vector3.right + Vector3.up + Vector3.back).normalized;
        private static readonly Vector3 kLocalUpLeft = (Vector3.left + Vector3.up + Vector3.back).normalized;
        private static readonly Vector3 kLocalDownRight = (Vector3.right + Vector3.down + Vector3.back).normalized;
        private static readonly Vector3 kLocalDownLeft = (Vector3.left + Vector3.down + Vector3.back).normalized;

        private float MinCurbDistance { get { return m_CurbFeelerDistance / 20f; } }
        private const float MaxCurbResistance = 10f;

        private void Awake()
        {
            if (m_UseCurbFeelers && (m_CurbResistance < 1f))
                m_CurbResistance = 1f;

            IsTargetObscured = false;
            ColliderDisplacement = 0;
        }

        private void Start()
        {
            OnEnable();
        }

        private void OnEnable()
        {
            VirtualCamera = GetComponent<CinemachineVirtualCameraBase>();
            if (VirtualCamera == null)
            {
                CinemachineDebugLogger.LogError("CinemachineCollider requires a Cinemachine Virtual Camera component");
                enabled = false;
            }
            else
            {
                VirtualCamera.AddPostPipelineStageHook(PostPipelineStageCallback);
                enabled = true;
            }
            RebuildCurbFeelers();
            mSmoothingFilter = null;
        }

        private void OnDisable()
        {
            if (VirtualCamera != null)
                VirtualCamera.RemovePostPipelineStageHook(PostPipelineStageCallback);
        }

        private void PostPipelineStageCallback(
            CinemachineVirtualCameraBase vcam,
            CinemachineCore.Stage stage, ref CameraState state, CameraState previousState, float deltaTime)
        {
            if (stage == CinemachineCore.Stage.Body)
            {
                IsTargetObscured = false; 
                ColliderDisplacement = 0;
            }
            if (enabled)
            {
                // Move the body before the Aim is calculated
                if (stage == CinemachineCore.Stage.Body)
                {
                    if (m_PreserveLineOfSight && m_LineOfSightFeelerDistance > UnityVectorExtensions.Epsilon)
                        state = PreserveLignOfSight(state);
                    if (m_UseCurbFeelers && m_CurbFeelerDistance > UnityVectorExtensions.Epsilon)
                        state = ApplyCurbFeelers(state);

                    // Apply the smoothing filter
                    if (m_PositionSmoothing > 0)
                        state.PositionCorrection 
                            += ApplySmoothing(vcam, state.CorrectedPosition) - state.CorrectedPosition;
                }
                // Rate the shot after the aim was set
                if (stage == CinemachineCore.Stage.Aim)
                {
                    IsTargetObscured = CheckForTargetObstructions(state);

                    // GML these values are an initial arbitrary attempt at rating quality
                    if (IsTargetObscured)
                        state.ShotQuality *= 0.2f; 
                    if (ColliderDisplacement > 0.1f)
                        state.ShotQuality *= 0.9f; 
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

        private CameraState PreserveLignOfSight(CameraState state)
        {
            if (state.HasLookAt)
            {
                Vector3 lookAtPos = state.ReferenceLookAt;
                Vector3 pos = state.CorrectedPosition;
                Vector3 dir = lookAtPos - pos;
                float distance = dir.magnitude;
                if (distance > UnityVectorExtensions.Epsilon)
                {
                    dir.Normalize();
                    float rayDistance = Mathf.Min(
                        m_LineOfSightFeelerDistance, distance-m_MinimumDistanceFromTarget);

                    // Make a ray that looks towards the camera, to get the most distant obstruction
                    Ray ray = new Ray(pos + rayDistance * dir, -dir);
                    int raycastLayerMask = m_CollideAgainst.value;

                    RaycastHit hitInfo;
                    if (Physics.Raycast(ray, out hitInfo, rayDistance, raycastLayerMask))
                    {
                        float adjustment = hitInfo.distance;
                        if (m_UseCurbFeelers)
                            adjustment -= MinCurbDistance;
                        pos = ray.GetPoint(adjustment);
                        Vector3 displacement = pos - state.CorrectedPosition;
                        state.PositionCorrection += displacement;
                        ColliderDisplacement += displacement.magnitude;
                    }
                }
            }
            return state;
        }

        private CameraState ApplyCurbFeelers(CameraState state)
        {
            Vector3 pos = state.CorrectedPosition;
            Quaternion orientation = state.CorrectedOrientation;
            RaycastHit hitInfo;
            int raycastLayerMask = m_CollideAgainst.value;

            Ray feelerRay = new Ray();
            int numHits = 0;
            Vector3 resultingPosition = Vector3.zero;
            for (int i = 0; i < mCompiledFeelers.Length; ++i)
            {
                CompiledCurbFeeler feeler = mCompiledFeelers[i];
                feelerRay.origin = pos;
                feelerRay.direction = orientation * feeler.LocalVector;
                if (Physics.Raycast(feelerRay, out hitInfo, feeler.RayDistance, raycastLayerMask))
                {
                    float compressionPercent = Mathf.Clamp01((feeler.RayDistance - hitInfo.distance) / feeler.RayDistance);
                    compressionPercent = 1f - Mathf.Pow(compressionPercent, feeler.DampingConstant);
                    resultingPosition += hitInfo.point - feelerRay.direction * (compressionPercent * feeler.RayDistance);
                    feeler.IsHit = true;
                    feeler.HitDistance = hitInfo.distance;
                    numHits++;
                }
                else
                {
                    feeler.IsHit = false;
                    feeler.HitDistance = float.MaxValue;
                }
                mCompiledFeelers[i] = feeler;
            }

            // Average the resulting positions if feelers hit anything
            if (numHits > 0)
            {
                Vector3 displacement = (resultingPosition / (float)numHits) - state.CorrectedPosition;
                ColliderDisplacement += displacement.magnitude;
                state.PositionCorrection += displacement;
            }
            return state;
        }

        private bool CheckForTargetObstructions(CameraState state)
        {
            if (state.HasLookAt)
            {
                Vector3 lookAtPos = state.ReferenceLookAt;
                Vector3 pos = state.CorrectedPosition;
                Vector3 dir = lookAtPos - pos;
                float distance = dir.magnitude;
                if (distance < Mathf.Max(m_MinimumDistanceFromTarget, UnityVectorExtensions.Epsilon))
                    return true;
                Ray ray = new Ray(pos, dir.normalized);
                RaycastHit hitInfo;
                if (Physics.Raycast(ray, out hitInfo, distance-m_MinimumDistanceFromTarget, m_CollideAgainst.value))
                    return true;
            }
            return false;
        }

        /// <summary>API for the inspection Editor.  Called when curb parameters are changed</summary>
        public void RebuildCurbFeelers()
        {
            mCompiledFeelers = null;
            if (m_UseCurbFeelers)
            {
                List<CompiledCurbFeeler> curbFeelers = new List<CompiledCurbFeeler>(9);
                Vector3 localRight = Vector3.right;
                Vector3 localLeft = Vector3.left;
                Vector3 localBack = Vector3.back;
                Vector3 localUp = Vector3.up;
                Vector3 localDown = Vector3.down;

                float feelerDamping = m_CurbResistance;
                curbFeelers.Add(new CompiledCurbFeeler(localBack, m_CurbFeelerDistance, feelerDamping));
                curbFeelers.Add(new CompiledCurbFeeler(localRight, m_CurbFeelerDistance, feelerDamping));
                curbFeelers.Add(new CompiledCurbFeeler(localLeft, m_CurbFeelerDistance, feelerDamping));

                curbFeelers.Add(new CompiledCurbFeeler(localUp, m_CurbFeelerDistance, feelerDamping));
                curbFeelers.Add(new CompiledCurbFeeler(localDown, m_CurbFeelerDistance, feelerDamping));

                curbFeelers.Add(new CompiledCurbFeeler(kLocalUpRight, m_CurbFeelerDistance, feelerDamping));
                curbFeelers.Add(new CompiledCurbFeeler(kLocalUpLeft, m_CurbFeelerDistance, feelerDamping));
                curbFeelers.Add(new CompiledCurbFeeler(kLocalDownRight, m_CurbFeelerDistance, feelerDamping));
                curbFeelers.Add(new CompiledCurbFeeler(kLocalDownLeft, m_CurbFeelerDistance, feelerDamping));

                mCompiledFeelers = curbFeelers.ToArray();
            }
        }
    }
}
