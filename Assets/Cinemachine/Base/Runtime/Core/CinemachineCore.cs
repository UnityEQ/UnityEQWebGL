using UnityEngine;
using System.Collections.Generic;

namespace Cinemachine
{
    /// <summary>A singleton that manages complete lists of <see cref="CinemachineBrain"/>a,
    /// <see cref="ICinemachineCamera"/>s, and the piority queue.  Provides
    /// services to keeping track of whether <see cref="ICinemachineCamera"/>s have
    /// been updated each frame.</summary>
    public sealed class CinemachineCore
    {
        /// <summary>
        /// Stages in the  <see cref="ICinemachineComponent"/> pipeline, used for
        /// UI organization>.  This enum defines the pipeline order.
        /// </summary>
        public enum Stage 
        { 
            /// <summary>First stage of the pipeline: adjust lens settings</summary>
            Lens, 

            /// <summary>Second stage: position the camera in space</summary>
            Body, 

            /// <summary>Third stage: orient the camera to point at the target</summary>
            Aim, 

            /// <summary>Final stage: apply noise (this is done separately, in the 
            /// Correction channel of the <see cref="CameraState"/>)</summary>
            Noise 
        };
        
        private static CinemachineCore sInstance = null;

        /// <summary>Get the singleton instance</summary>
        public static CinemachineCore Instance
        {
            get
            {
                if (sInstance == null)
                    sInstance = new CinemachineCore();
                return sInstance;
            }
        }

        /// <summary>
        /// If true, show hidden Cinemachine objects, to make manual script mapping possible.
        /// </summary>
        public static bool sShowHiddenObjects = false;

        /// <summary>
        /// List of all active <see cref="CinemachineBrain"/>.
        /// </summary>
        private readonly List<CinemachineBrain> mActiveBrains = new List<CinemachineBrain>();

        /// <summary>Get the complete list of active <see cref="CinemachineBrain"/></summary>
        public IEnumerable<CinemachineBrain> AllBrains { get { return mActiveBrains; } }

        /// <summary>
        /// Called when a <see cref="CinemachineBrain"/> is enabled.
        /// </summary>
        internal void AddActiveBrain(CinemachineBrain brain)
        {
            // First remove it, just in case it's being added twice
            RemoveActiveBrain(brain);
            mActiveBrains.Insert(0, brain);
        }

        /// <summary>
        /// Called when a <see cref="CinemachineBrain"/> is disabled.
        /// </summary>
        internal void RemoveActiveBrain(CinemachineBrain brain)
        {
            mActiveBrains.Remove(brain);
        }

        /// <summary>
        /// List of all active <see cref="ICinemachineCamera"/>, for all brains.
        /// This list is kept sorted by priority.
        /// </summary>
        public IEnumerable<ICinemachineCamera> AllCameras { get { return mActiveCameras; } }
        private readonly List<ICinemachineCamera> mActiveCameras = new List<ICinemachineCamera>();

        /// <summary>
        /// Called when a <see cref="ICinemachineCamera"/> is enabled.
        /// </summary>
        internal void AddActiveCamera(ICinemachineCamera cam)
        {
            // Bring it to the top of the list
            RemoveActiveCamera(cam);

            // Keep list sorted by priority
            int insertIndex;
            for (insertIndex = 0; insertIndex < mActiveCameras.Count; ++insertIndex)
            {
                if (cam.Priority >= mActiveCameras[insertIndex].Priority)
                    break;
            }
            mActiveCameras.Insert(insertIndex, cam);
        }

        /// <summary>
        /// Called when a <see cref="ICinemachineCamera"/> is disabled.
        /// </summary>
        internal void RemoveActiveCamera(ICinemachineCamera cam)
        {
            mActiveCameras.Remove(cam);
        }

        /// <summary>
        /// Update a single <see cref="ICinemachineCamera"/> if and only if it
        /// hasn't already been updated this frame.  Always update vcams via this method.
        /// Calling this more than once per frame for the same camera will have no effect.
        /// </summary>
        internal bool UpdateVirtualCamera(ICinemachineCamera vcam, Vector3 worldUp, float deltaTime)
        {
            if (mUpdateStatus == null)
                mUpdateStatus = new Dictionary<ICinemachineCamera, UpdateStatus>();
            if (vcam.VirtualCameraGameObject == null)
            {
                if (mUpdateStatus.ContainsKey(vcam))
                    mUpdateStatus.Remove(vcam);
                return false; // camera was deleted
            }
            UpdateStatus status = new UpdateStatus();
            if (!mUpdateStatus.TryGetValue(vcam, out status))
            {
                status.frame = -1;
                status.subframe = 0;
                status.lastFixedUpdate = -1;
                status.targetPos = Matrix4x4.zero;
                mUpdateStatus.Add(vcam, status);
            }

            int subframes = (CurrentUpdateFilter == UpdateFilter.Late)
                ? 1 : CinemachineBrain.GetSubframeCount();
            int now = Time.frameCount;
            if (status.frame != now)
                status.subframe = 0;

            // If we're in smart update mode and the target moved, then we must update now
            bool updateNow = (CurrentUpdateFilter == UpdateFilter.Any);
            if (!updateNow)
            {
                Matrix4x4 targetPos;
                if (!GetTargetPosition(vcam, out targetPos))
                    updateNow = CurrentUpdateFilter == UpdateFilter.Late;
                else
                {
                    if (status.targetPos != targetPos)
                        updateNow = true;
                    status.targetPos = targetPos;
                }
            }

            // If we haven't been updated in a couple of frames, better update now
            if (CurrentUpdateFilter == UpdateFilter.Late && status.lastFixedUpdate < now-2)
                updateNow = true;

            if (updateNow)
            {
                while (status.subframe < subframes)
                {
//Debug.Log(vcam.Name + ": frame " + Time.frameCount + "." + status.subframe + ", " + CurrentUpdateFilter);
                    vcam.UpdateCameraState(worldUp, deltaTime);
                    if (CurrentUpdateFilter == UpdateFilter.Fixed)
                        status.lastFixedUpdate = now;
                    ++status.subframe;
                }
                status.frame = now;
            }

            mUpdateStatus[vcam] = status;
            return true;
        }

        struct UpdateStatus 
        { 
            public int frame; 
            public int subframe; 
            public int lastFixedUpdate;
            public Matrix4x4 targetPos;
        }
        Dictionary<ICinemachineCamera, UpdateStatus> mUpdateStatus;

        /// For CinemachineBrain use only
        internal enum UpdateFilter { Fixed, Late, Any };
        internal UpdateFilter CurrentUpdateFilter { get; set; }
        private bool GetTargetPosition(ICinemachineCamera vcam, out Matrix4x4 targetPos)
        {
            ICinemachineCamera vcamTarget = vcam.LiveChildOrSelf;
            if (vcamTarget == null || vcamTarget.VirtualCameraGameObject == null)
            {
                targetPos = Matrix4x4.identity;
                return false;
            }
            targetPos = vcamTarget.VirtualCameraGameObject.transform.worldToLocalMatrix;
            if (vcamTarget.LookAt != null)
            {
                targetPos = vcamTarget.LookAt.worldToLocalMatrix;
                return true;
            }
            if (vcamTarget.Follow != null)
            {
                targetPos = vcamTarget.Follow.worldToLocalMatrix;
                return true;
            }
            return false; // no target
        }

        /// <summary>
        /// Is this virtual camera currently actively controlling any <see cref="Camera"/>?
        /// </summary>
        public bool IsLive(ICinemachineCamera vcam)
        {
            if (vcam != null)
            {
                foreach (CinemachineBrain b in AllBrains)
                    if (b != null && b.IsLive(vcam))
                        return true;
            }
            return false;
        }

        /// <summary>
        /// Signal that the virtual has been activated.
        /// If the camera is live, then all CinemachineBrains that are showing it will 
        /// send an activation event.
        /// </summary>
        public void GenerateCameraActivationEvent(ICinemachineCamera vcam)
        {
            if (vcam != null)
            {
                foreach (CinemachineBrain b in AllBrains)
                    if (b != null && b.IsLive(vcam))
                        b.m_CameraActivatedEvent.Invoke();
            }
        }

        /// <summary>
        /// Signal that the virtual camera's content is discontinuous WRT the previous frame.
        /// If the camera is live, then all CinemachineBrains that are showing it will send a cut event.
        /// </summary>
        public void GenerateCameraCutEvent(ICinemachineCamera vcam)
        {
            if (vcam != null)
            {
                foreach (CinemachineBrain b in AllBrains)
                    if (b != null && b.IsLive(vcam))
                        b.m_CameraCutEvent.Invoke();
            }
        }

        /// <summary>
        /// Try to find a <see cref="CinemachineBrain"/> to associate with a 
        /// <see cref="ICinemachineCamera"/>.  The first <see cref="CinemachineBrain"/> 
        /// in which this <see cref="ICinemachineCamera"/> is live will be used.  
        /// If none, then the first active <see cref="CinemachineBrain"/> will be used.  
        /// Final result may be null.
        /// </summary>
        /// <param name="vcam">Virtual camer whose potential brain we need.</param>
        /// <returns>First <see cref="CinemachineBrain"/> found that might be 
        /// appropriate for this vcam, or null</returns>
        public CinemachineBrain FindPotentialTargetBrain(ICinemachineCamera vcam)
        {
            if (vcam != null)
            {
                foreach (CinemachineBrain b in AllBrains)
                    if (b != null && b.OutputCamera != null && b.IsLive(vcam))
                        return b;
            }
            foreach (CinemachineBrain b in AllBrains)
                if (b != null && b.OutputCamera != null)
                    return b;
            return null;
        }
    }
}
