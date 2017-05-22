using UnityEngine;
using Cinemachine.Utility;
using System.Collections.Generic;

namespace Cinemachine
{
    /// <summary>
    /// A Cinemachine Camera geared towards a 3rd person camera experience. 
    /// The camera orbits around its subject with three separate camera rigs defining 
    /// rings around the target. Each rig has its own radius, height offset, and composer.
    /// Depending on the camera's position along the spline connecting these three rigs, 
    /// these settings are interpolated.
    /// </summary>
    [ExecuteInEditMode, DisallowMultipleComponent]
    [AddComponentMenu("Cinemachine/FreeLook")]
    public class CinemachineFreeLook : CinemachineVirtualCameraBase
    {
        /// <summary>Default object for the camera children to look at (the aim target), if not specified in a child rig.  May be empty</summary>
        [Tooltip("Default object for the camera children to look at (the aim target), if not specified in a child rig.  May be empty.")]
        public Transform m_LookAt = null;

        /// <summary>Default object for the camera children wants to move with (the body target), if not specified in a child rig.  May be empty</summary>
        [Tooltip("Default object for the camera children wants to move with (the body target), if not specified in a child rig.  May be empty.")]
        public Transform m_Follow = null;

        /// <summary>If enabled, this lens setting will apply to all three child rigs, otherwise the child rig lens settings will be used</summary>
        [Tooltip("If enabled, this lens setting will apply to all three child rigs, otherwise the child rig lens settings will be used")]
        public bool m_UseCommonLensSetting = false;

        /// <summary>Specifies the lens properties of this Virtual Camera</summary>
        [Tooltip("Specifies the lens properties of this Virtual Camera")]
        public LensSettings m_LensAttributes = LensSettings.Default;

        /// <summary>The Horizontal axis.  This is passed on to the rigs' OrbitalTransposer component</summary>
        [Header("Axis Control")]
        public CinemachineOrbitalTransposer.AxisState m_XAxis 
            = new CinemachineOrbitalTransposer.AxisState(3000f, 1f, 2f, 0f, "Mouse X");
        /// <summary>The Vertical axis.  Value is 0..1.  Chooses how to blend the child rigs</summary>
        public CinemachineOrbitalTransposer.AxisState m_YAxis 
            = new CinemachineOrbitalTransposer.AxisState(3f, 3f, 3f, 0.5f, "Mouse Y");

        /// <summary>Controls how automatic recentering of the X axis is accomplished</summary>
        public CinemachineOrbitalTransposer.Recentering m_RecenterToTargetHeading 
            = new CinemachineOrbitalTransposer.Recentering(
                false, 1, 2, 
                CinemachineOrbitalTransposer.Recentering.HeadingDerivationMode.EulerYRotation, 
                0f, 4);

        /// <summary>Controls how taut is the line that connects the rigs' orbits, 
        /// which determines final placement on the Y axis.</summary>
        [Header("Orbits")]
        [Range(0f, 1f)]
        public float m_SplineTension = 1f;

        /// <summary>Get a child rig</summary>
        /// <param name="i">Rig index.  Can be 0, 1, or 2</param>
        /// <returns>The rig, or null if index is bad.</returns>
        public CinemachineVirtualCamera GetRig(int i) { UpdateRigCache();  return (i < 0 || i > 2) ? null : m_Rigs[i]; }

        /// <summary>Names of the 3 child rigs</summary>
        public static string[] RigNames { get { return new string[] { "TopRig", "MiddleRig", "BottomRig" }; } }

        /// <summary>Default values for the child orbit radii</summary>
        public float[] DefaultRadius { get { return new float[] { 1.75f, 3f, 1.3f }; } }

        /// <summary>Default values for the child orbit heights</summary>
        public float[] DefaultHeight { get { return new float[] { 4.5f, 2.5f, 0.4f }; } }

        /// <summary>Updates the child rig cache</summary>
        protected override void OnEnable()
        {
            base.OnEnable();
            InvalidateRigCache(); 

            // Snap to target
            CinemachineBrain brain = CinemachineCore.Instance.FindPotentialTargetBrain(this);
            UpdateCameraState((brain != null) ? brain.DefaultWorldUp : Vector3.up, -1);
        }

        /// <summary>Makes sure that the child rigs get destroyed in an undo-firndly manner.  
        /// Invalidates the rig cache.</summary>
        protected override void OnDestroy() 
        {
            if (m_Rigs != null)
            {
                foreach (var rig in m_Rigs)
                {
                    if (DestroyRigOverride != null)
                        DestroyRigOverride(rig.gameObject);
                    else
                        DestroyImmediate(rig.gameObject);
                }
                m_Rigs = null;
            }
            InvalidateRigCache(); 
            base.OnDestroy();
        }

        /// <summary>Invalidates the rig cache</summary>
        void OnTransformChildrenChanged() 
        { 
            InvalidateRigCache(); 
        }

        void Reset()
        {
            CreateRigs(null);
        }

        /// <summary>The cacmera state, which will be a blend of the child rig states</summary>
        override public CameraState State { get { return m_State; } }

        /// <summary>Get the current LookAt target.  Returns parent's LookAt if parent 
        /// is non-null and no specific LookAt defined for this camera</summary>
        override public Transform LookAt 
        { 
            get { return ResolveLookAt(m_LookAt); } 
            set { m_LookAt = value; } 
        }

        /// <summary>Get the current Follow target.  Returns parent's Follow if parent 
        /// is non-null and no specific Follow defined for this camera</summary>
        override public Transform Follow 
        { 
            get { return ResolveFollow(m_Follow); } 
            set { m_Follow = value; }
        }

        /// <summary>Remove a Pipeline stage hook callback.  
        /// Make sure it is removed from all the children.</summary>
        /// <param name="d">The delegate to remove.</param>
        public override void RemovePostPipelineStageHook(OnPostPipelineStageDelegate d) 
        { 
            base.RemovePostPipelineStageHook(d);
            UpdateRigCache();
            foreach (var vcam in m_Rigs)
                vcam.RemovePostPipelineStageHook(d);
        }
        
        /// <summary>Called by <see cref="CinemachineCore"/> at designated update time
        /// so the vcam can position itself and track its targets.  All 3 child rigs are updated,
        /// and a blend calculated, depending on the value of the Y axis.</summary>
        /// <param name="worldUp">Default world Up, set by the <see cref="CinemachineBrain"/></param>
        /// <param name="deltaTime">Delta time for time-based effects (ignore if less than or equal to 0)</param>
        override public void UpdateCameraState(Vector3 worldUp, float deltaTime)
        {
            UpdateRigCache();

            // Read the Height
            bool activeCam = CinemachineCore.Instance.IsLive(this);
            if (activeCam)
                m_YAxis.Update(deltaTime, false);
                    
            // Reads the heading.  Make sure all the rigs get updated first
            PushSettingsToRigs();
            if (activeCam)
                UpdateHeading(deltaTime, m_State.ReferenceUp);

            // Drive the rigs 
            for (int i = 0; i < m_Rigs.Length; ++i)
                if (m_Rigs[i] != null)
                    m_Rigs[i].UpdateCameraState(worldUp, deltaTime);

            // Reset the base camera state, in case the game object got moved in the editor
            if (deltaTime <= 0)
                m_State = PullStateFromVirtualCamera(); // Not in gameplay

            // Update the current state by invoking the component pipeline
            m_State = CalculateNewState(worldUp, deltaTime);

            // Push the raw position back to the game object's transform, so it 
            // moves along with the camera.  Leave the orientation alone, because it 
            // screws up camera dragging when there is a LookAt behaviour.
            if (Follow != null)
                transform.position = State.RawPosition;
        }

        /// <summary>If we are transitioning from another FreeLook, grab the axis values from it.</summary>
        /// <param name="fromCam">The camera being deactivated.  May be null.</param>
        override public void OnTransitionFromCamera(ICinemachineCamera fromCam) 
        {
            if ((fromCam != null) && (fromCam is CinemachineFreeLook))
            {
                CinemachineFreeLook freeLookFrom = fromCam as CinemachineFreeLook;
                if (freeLookFrom.Follow == Follow)
                {
                    m_XAxis.Value = freeLookFrom.m_XAxis.Value;
                    m_YAxis.Value = freeLookFrom.m_YAxis.Value;
                    PushSettingsToRigs();
                }
            }
        }

        CameraState m_State = CameraState.Default;          // Current state this frame

        /// Serialized in order to support copy/paste
        [SerializeField] [HideInInspector][NoSaveDuringPlay] private CinemachineVirtualCamera[] m_Rigs = new CinemachineVirtualCamera[3];

        void InvalidateRigCache() { mOribitals = null; }
        CinemachineOrbitalTransposer[] mOribitals = null;
        CinemachineBlend mBlendA;
        CinemachineBlend mBlendB;

        /// <summary>
        /// Override component pipeline creation.  
        /// This needs to be done by the editor to support Undo.
        /// The override must do exactly the same thing as the CreatePipeline method in this class.
        /// </summary>
        public static CreateRigDelegate CreateRigOverride;

        /// <summary>
        /// Override component pipeline creation.  
        /// This needs to be done by the editor to support Undo.
        /// The override must do exactly the same thing as the CreatePipeline method in this class.
        /// </summary>
        public delegate CinemachineVirtualCamera CreateRigDelegate(
            CinemachineFreeLook vcam, string name, CinemachineVirtualCamera copyFrom);

        /// <summary>
        /// Override component pipeline destruction.  
        /// This needs to be done by the editor to support Undo.
        /// </summary>
        public static DestroyRigDelegate DestroyRigOverride;

        /// <summary>
        /// Override component pipeline destruction.  
        /// This needs to be done by the editor to support Undo.
        /// </summary>
        public delegate void DestroyRigDelegate(GameObject rig);

        
        private void CreateRigs(CinemachineVirtualCamera[] copyFrom)
        {
            // Invalidate the cache
            m_Rigs = null;
            mOribitals = null; 

            string[] rigNames = RigNames;
            float[] softCenterDefaultsV = new float[] { 0.5f, 0.55f, 0.6f };
            for (int i = 0; i < rigNames.Length; ++i)
            {
                CinemachineVirtualCamera src = null;
                if (copyFrom != null && copyFrom.Length > i)
                    src = copyFrom[i];

                CinemachineVirtualCamera rig = null;
                if (CreateRigOverride != null)
                    rig = CreateRigOverride(this, rigNames[i], src);
                else 
                {
                    // If there is an existing rig with this name, delete it
                    List<Transform> list = new List<Transform>();
                    foreach (Transform child in transform) 
                        if (child.GetComponent<CinemachineVirtualCamera>() != null
                                 && child.gameObject.name == rigNames[i])
                            list.Add(child);
                    foreach (Transform child in list)
                        DestroyImmediate(child.gameObject);

                    // Create a new rig with default components
                    GameObject go = new GameObject(rigNames[i]);
                    go.transform.parent = transform;
                    rig = go.AddComponent<CinemachineVirtualCamera>();
                    if (src != null)
                        ReflectionHelpers.CopyFields(src, rig);
                    else
                    {
                        go = rig.GetComponentOwner().gameObject;
                        go.AddComponent<CinemachineOrbitalTransposer>();
                        go.AddComponent<CinemachineComposer>();
                    }
                }

                // Set up the defaults
                rig.InvalidateComponentPipeline();
                CinemachineOrbitalTransposer orbital = rig.GetCinemachineComponent<CinemachineOrbitalTransposer>();
                if (orbital == null)
                    orbital = rig.AddCinemachineComponent<CinemachineOrbitalTransposer>(); // should not happen
                if (src == null)
                {
                    // Only set defaults if not copying
                    orbital.m_Radius = DefaultRadius[i];
                    orbital.m_HeightOffset = DefaultHeight[i];
                    CinemachineComposer composer = rig.GetCinemachineComponent<CinemachineComposer>();
                    if (composer != null)
                    {
                        composer.m_HorizontalDamping = composer.m_VerticalDamping = 0;
                        composer.m_ScreenX = 0.5f;
                        composer.m_ScreenY = softCenterDefaultsV[i];
                        composer.m_DeadZoneWidth = composer.m_DeadZoneHeight = 0;
                        composer.m_SoftZoneWidth = composer.m_SoftZoneHeight = 0.8f;
                        composer.m_BiasX = composer.m_BiasY = 0;
                    }
                }
            }
        }
        
        private void UpdateRigCache()
        {
            // Did we just get copy/pasted?
            string[] rigNames = RigNames;
            if (m_Rigs != null && m_Rigs.Length == rigNames.Length 
                    && m_Rigs[0] != null && m_Rigs[0].transform.parent != transform)
                CreateRigs(m_Rigs);

            // Early out if we're up to date
            if (mOribitals != null && mOribitals.Length == rigNames.Length)
                return;

            // Locate existiong rigs, and recreate them if any are missing
            if (LocateExistingRigs(rigNames, false) != rigNames.Length)
            {
                CreateRigs(null);
                LocateExistingRigs(rigNames, true);
            }

            foreach (var rig in m_Rigs)
            {
                // Hide the rigs from prying eyes
                if (CinemachineCore.sShowHiddenObjects)
                    rig.gameObject.hideFlags 
                        &= ~(HideFlags.HideInHierarchy | HideFlags.HideInInspector);
                else
                    rig.gameObject.hideFlags 
                        |= (HideFlags.HideInHierarchy | HideFlags.HideInInspector);

                // Configure the UI
                rig.m_HideHeaderInInspector = true;
                rig.m_ExcludedPropertiesInInspector = new string[] { "m_Script", "m_Priority" };
                rig.m_LockStageInInspector = new CinemachineCore.Stage[] { CinemachineCore.Stage.Body };

                // Chain into the pipeline callback
                rig.AddPostPipelineStageHook(PostPipelineStageCallback);
            }

            // Create the blend objects
            mBlendA = new CinemachineBlend(m_Rigs[1], m_Rigs[0], AnimationCurve.Linear(0, 0, 1, 1), 0);
            mBlendB = new CinemachineBlend(m_Rigs[2], m_Rigs[1], AnimationCurve.Linear(0, 0, 1, 1), 0);

            // Horizontal rotation clamped to [0,360] (with wraparound)
            m_XAxis.SetThresholds(0f, 360f, true);

            // Vertical rotation cleamped to [0,1] as it is a t-value for the 
            // catmull-rom spline going through the 3 points on the rig
            m_YAxis.SetThresholds(0f, 1f, false);
        }

        private int LocateExistingRigs(string[] rigNames, bool forceOrbital)
        {
            mOribitals = new CinemachineOrbitalTransposer[rigNames.Length];
            m_Rigs = new CinemachineVirtualCamera[rigNames.Length];
            int rigsFound = 0;
            foreach (Transform child in transform) 
            {
                CinemachineVirtualCamera vcam = child.GetComponent<CinemachineVirtualCamera>();
                if (vcam != null)
                {
                    GameObject go = child.gameObject;
                    for (int i = 0; i < rigNames.Length; ++i)
                    {
                        if (mOribitals[i] == null && go.name == rigNames[i])
                        {
                            // Must have an orbital transposer or it's no good
                            mOribitals[i] = vcam.GetCinemachineComponent<CinemachineOrbitalTransposer>();
                            if (mOribitals[i] == null && forceOrbital)
                                mOribitals[i] = vcam.AddCinemachineComponent<CinemachineOrbitalTransposer>();
                            if (mOribitals[i] != null)
                            {
                                mOribitals[i].m_HeadingIsSlave = true;
                                m_Rigs[i] = vcam;
                                ++rigsFound;
                            }
                        }
                    }
                }
            }
            return rigsFound;
        }

        void PostPipelineStageCallback(
            CinemachineVirtualCameraBase vcam, CinemachineCore.Stage stage, 
            ref CameraState newState, CameraState previousState, float deltaTime)
        {
            if (OnPostPipelineStage != null)
                OnPostPipelineStage(vcam, stage, ref newState, previousState, deltaTime);
        }

        void PushSettingsToRigs()
        {
            UpdateRigCache();
            for (int i = 0; i < m_Rigs.Length; ++i)
            {
                if (m_Rigs[i] == null)
                    continue;
                if (m_UseCommonLensSetting)
                    m_Rigs[i].m_LensAttributes = m_LensAttributes;
                mOribitals[i].m_HeadingIsSlave = true;
                mOribitals[i].SetXAxisState(m_XAxis);
                mOribitals[i].m_RecenterToTargetHeading = m_RecenterToTargetHeading;
                if (i > 0)
                    mOribitals[i].m_RecenterToTargetHeading.m_enabled = false;
                mOribitals[i].UseRadiusOverride = true;
                mOribitals[i].RadiusOverride = GetEffectiveRadius(GetEffectiveVertical());
            }
        }
        
        private CameraState CalculateNewState(Vector3 worldUp, float deltaTime) 
        {
            CameraState state = PullStateFromVirtualCamera();

            // Blend from the appropriate rigs
            float vertical = GetEffectiveVertical();
            if (vertical > 0.5f)
            {
                if (mBlendA != null)
                {
                    mBlendA.TimeInBlend = (vertical - 0.5f) * 2f;
                    mBlendA.UpdateCameraState(worldUp, deltaTime);
                    state = mBlendA.State;
                }
            }
            else
            {
                if (mBlendB != null)
                {
                    mBlendB.TimeInBlend = vertical * 2f;
                    mBlendB.UpdateCameraState(worldUp, deltaTime);
                    state = mBlendB.State;
                }
            }
            return state;
        }

        void UpdateHeading(float deltaTime, Vector3 up)
        {
            // We let the first rig calculate the heading
            if (mOribitals[0] != null)
            {
                mOribitals[0].UpdateHeading(deltaTime, up, true);
                m_XAxis.Value = mOribitals[0].m_XAxis.Value;
            }
            // Then push it to the other rigs
            for (int i = 1; i < mOribitals.Length; ++i)
                if (mOribitals[i] != null)
                    mOribitals[i].m_XAxis.Value = m_XAxis.Value;
        }

        private CameraState PullStateFromVirtualCamera()
        {
            CameraState state = CameraState.Default;
            state.RawPosition = transform.position;
            state.RawOrientation = transform.rotation;
            //state.ReferenceUp = state.RawOrientation * Vector3.up;
            state.Lens = m_LensAttributes;
            return state;
        }

        // Make the transition between top and bottom segments smooth
        private float GetEffectiveVertical()
        {
            if (mOribitals == null)
                return 0;

            // Start with the raw value
            float t = m_YAxis.Value;

            // Apply warping based on the distance difference between the 2 segments.
            // This is to ensure a smooth transition if the difference is great
            float x = mOribitals[0].m_Radius - mOribitals[1].m_Radius;
            float y = mOribitals[0].m_HeightOffset - mOribitals[1].m_HeightOffset;
            float b = Mathf.Sqrt(x*x + y*y);
            x = mOribitals[2].m_Radius - mOribitals[1].m_Radius;
            y = mOribitals[2].m_HeightOffset - mOribitals[1].m_HeightOffset;
            float a = Mathf.Sqrt(x*x + y*y);

            t = Mathf.Clamp01(t);
            if (t > 0.5f)
                return WarpForSmoothness(t - 0.5f, a, b) + 0.5f;
            return 1f - (WarpForSmoothness((1f - t) - 0.5f, b, a) + 0.5f);
        }

        // t = [0...0.5]
        private float WarpForSmoothness(float t, float a, float b)
        {
            float diff = b-a;
            if (diff > 0)
            {
                float c = (2f * a * t) + (4 * diff * t * t);
                t = 0.5f * c / (a + diff);
            }
            return t;
        }
        
        /// <summary>Calculates an interpol;ated radius, taking spline tension into account</summary>
        public float GetEffectiveRadius(float t)
        {
            if (mOribitals == null)
                return 0;

            t = Mathf.Clamp01(t);
            float t2 = t;
            if (t > 0.5f)
            {
                t2 = 0.5f + (1f - Mathf.Cos((t2-0.5f) * Mathf.PI)) / 2f;
                t2 = Mathf.Lerp(t, t2, m_SplineTension);
                return Mathf.Lerp(mOribitals[1].m_Radius, mOribitals[0].m_Radius, (t2-0.5f)*2f);
            }
            t2 = Mathf.Sin(t2 * Mathf.PI) / 2f;
            t2 = Mathf.Lerp(t, t2, m_SplineTension);
            return Mathf.Lerp(mOribitals[2].m_Radius, mOribitals[1].m_Radius, t2*2f);
        }

        /// <summary>
        /// Returns the local position of the camera along the spline used to connect the 
        /// three camera rigs. Does not take into account the current heading of the 
        /// camera (or its target)
        /// </summary>
        /// <param name="t">The t-value for the camera on its spline. Internally clamped to 
        /// the value [0,1]</param>
        /// <returns>The local offset (back + up) of the camera WRT its target based on the 
        /// supplied t-value</returns>
        public Vector3 GetLocalPositionForCameraFromInput(float t)
        {
            if (mOribitals == null)
                return Vector3.zero;

            Vector3 bottomPos = Vector3.up * mOribitals[2].m_HeightOffset;
            Vector3 middlePos = Vector3.up * mOribitals[1].m_HeightOffset;
            Vector3 topPos = Vector3.up * mOribitals[0].m_HeightOffset;

            if (t < 0.5f)
                return Vector3.Lerp(bottomPos, middlePos, t * 2f) 
                    + Vector3.back * GetEffectiveRadius(t);
            return Vector3.Lerp(middlePos, topPos, (t - 0.5f) * 2f) 
                + Vector3.back * GetEffectiveRadius(t);
        }
    }
}
