using UnityEngine;

using System;

namespace Cinemachine
{
    /// <summary>
    /// Describes the FoV and clip planes for a camera when the Composer is active 
    /// on a <see cref="ICinemachineCamera"/>
    /// </summary>
    [Serializable]
    public struct LensSettings
    {
        /// <summary>Default Lens Settings</summary>
        public static LensSettings Default = new LensSettings(40f, 0.1f, 5000f, 0);

        /// <summary>
        /// This is the camera view in horizontal degrees. For cinematic people, a 50mm lens 
        /// on a super-35mm sensor would equal a 19.6 degree FOV
        /// </summary>
        [Range(1f, 179f)]
        [Tooltip("This is the camera view in horizontal degrees. For cinematic people, a 50mm lens on a super-35mm sensor would equal a 19.6 degree FOV")]
        public float FieldOfView;

        /// <summary>
        /// The near clip plane for this <see cref="LensSettings"/>
        /// </summary>
        [Tooltip("This defines the near region in the renderable range of the camera frustum. Raising this value will stop the game from drawing things near the camera, which can sometimes come in handy.  Larger values will also increase your shadow resolution.")]
        public float NearClipPlane;

        /// <summary>
        /// The far clip plane for this <see cref="LensSettings"/>
        /// </summary>
        [Tooltip("This defines the far region of the renderable range of the camera frustum. Typically you want to set this value as low as possible without cutting off desired distant objects")]
        public float FarClipPlane;

        /// <summary>
        /// The dutch (tilt) to be applied to the camera. In degrees
        /// </summary>
        [Range(-179f, 180f)]
        [Tooltip("Camera Z roll, or tilt, in degrees.")]
        public float Dutch;
        
        /// <summary>
        /// Creates a new <see cref="LensSettings"/>, copying the values from the supplied <see cref="Camera"/>
        /// </summary>
        /// <param name="fromCamera">The <see cref="Camera"/> from which the FoV, near and far clip 
        /// planes will be copied from.</param>
        public LensSettings(Camera fromCamera)
        {
            FieldOfView = fromCamera.fieldOfView;
            NearClipPlane = fromCamera.nearClipPlane;
            FarClipPlane = fromCamera.farClipPlane;
            Dutch = 0;
        }

        /// <summary>
        /// Explicit constructor for this <see cref="LensSettings"/>
        /// </summary>
        /// <param name="fov">The Vertical field of view</param>
        /// <param name="nearClip">The near clip plane</param>
        /// <param name="farClip">The far clip plane</param>
        /// <param name="dutch">Camera roll, in degrees.  This is applied at the end after shot composition.</param>
        public LensSettings(float fov, float nearClip, float farClip, float dutch)
        {
            FieldOfView = fov;
            NearClipPlane = nearClip;
            FarClipPlane = farClip;
            Dutch = dutch;
        }

        /// <summary>
        /// Linearly blends the fields of two <see cref="LensSettings"/> and returns the result
        /// </summary>
        /// <param name="lensA">The <see cref="LensSettings"/> to blend from</param>
        /// <param name="lensB">The <see cref="LensSettings"/> to blend to</param>
        /// <param name="t">The interpolation value. Internally clamped to the range [0,1]</param>
        /// <returns>Interpolated settings</returns>
        public static LensSettings Lerp(LensSettings lensA, LensSettings lensB, float t)
        {
            t = Mathf.Clamp01(t);
            LensSettings blendedLens = new LensSettings();
            blendedLens.FarClipPlane = Mathf.Lerp(lensA.FarClipPlane, lensB.FarClipPlane, t);
            blendedLens.NearClipPlane = Mathf.Lerp(lensA.NearClipPlane, lensB.NearClipPlane, t);
            blendedLens.FieldOfView = Mathf.Lerp(lensA.FieldOfView, lensB.FieldOfView, t);
            blendedLens.Dutch = Mathf.Lerp(lensA.Dutch, lensB.Dutch, t);
            return blendedLens;
        }
    }
}
