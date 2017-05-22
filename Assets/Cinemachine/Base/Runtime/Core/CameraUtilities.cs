using UnityEngine;

namespace Cinemachine.Utility
{
    /// <summary>Various helper functions to assist with camera view calculations</summary>
    public static class CameraUtilities
    {
        /// <summary>
        /// Used to convert a viewport point to a ray for a camera at a position and 
        /// orientation different from where it is presently.
        /// </summary>
        /// <param name="aspect">Camera aspect ratio</param>
        /// <param name="withLens">The lens to apply to the camera when finding the viewport point</param>
        /// <param name="viewportPoint">The viewport point to use when creating the <see cref="Ray"/></param>
        /// <param name="fromPosition">The position to generate the ray from</param>
        /// <param name="withOrientation">The orientation of the camera used to generate the ray from</param>
        /// <returns>The ray for the camera at the specified <paramref name="fromPosition"/> 
        /// and <paramref name="withOrientation"/>. </returns>
        public static Ray GetRayFromViewportAtPoint(
            float aspect, LensSettings withLens, Vector3 viewportPoint, 
            Vector3 fromPosition, Quaternion withOrientation)
        {
            Matrix4x4 persp = Matrix4x4.Perspective(
                withLens.FieldOfView, aspect, withLens.NearClipPlane, withLens.FarClipPlane);

            viewportPoint.z = 0.1f;
            //Transform into clip space before passing through the inverse perspective matrix
            viewportPoint *= 2f;
            viewportPoint -= Vector3.one;
            Vector3 result = persp.inverse.MultiplyPoint(viewportPoint).normalized;
            result.z *= -1f;

            return new Ray(fromPosition, withOrientation * result);
        }

        /// <summary>
        /// Used to find the viewport point for the specified <see cref="Camera"/> at the specified 
        /// position and orientation
        /// </summary>
        /// <param name="aspect">Camera aspect ratio</param>
        /// <param name="withLens">The lens to apply to the camera when finding the viewport point</param>
        /// <param name="worldPoint">The world point to find the viewport position of</param>
        /// <param name="fromPosition">Camera position</param>
        /// <param name="withOrientation">Camera orientation</param>
        /// <returns>The viewport space position of <paramref name="worldPoint"/></returns>
        public static Vector3 GetViewportPointFromWorldPoint(
            float aspect, LensSettings withLens, Vector3 worldPoint, 
            Vector3 fromPosition, Quaternion withOrientation)
        {
            // If the world point is behind the camera, rotate it to bring it to the nearest 
            // edge of the front hemisphere
            Vector3 fwd = withOrientation * Vector3.forward;
            Vector3 dir = worldPoint - fromPosition;
            float angle = Vector3.Angle(fwd, dir);
            if (angle > 89)
            {
                Vector3 axis = Vector3.Cross(dir, fwd);
                if (axis.AlmostZero())
                    axis = withOrientation * Vector3.up;
                Quaternion q = Quaternion.AngleAxis(angle-89, axis.normalized);
                worldPoint = (q * (worldPoint - fromPosition)) + fromPosition;
            }

            Matrix4x4 persp = Matrix4x4.Perspective(
                withLens.FieldOfView, aspect, withLens.NearClipPlane, withLens.FarClipPlane);
            Vector3 localPoint = (withOrientation * (worldPoint - fromPosition)) + fromPosition;
            Vector4 viewportPoint = persp.MultiplyPoint(localPoint);
            if (Mathf.Abs(viewportPoint.z) > UnityVectorExtensions.Epsilon)
                viewportPoint /= -viewportPoint.z;

            viewportPoint += Vector4.one;
            viewportPoint *= 0.5f;

            return viewportPoint;
        }

        /// <summary>
        /// Calculate the horizontal FOV, given a vertical FOV and an aspect ratio. 
        /// </summary>
        public static float CalculateHorizontalFOV(float fov, float aspect)
        {
            double radHFOV = 2 * System.Math.Atan(System.Math.Tan(fov * Mathf.Deg2Rad / 2) * aspect);
            return (float)(Mathf.Rad2Deg * radHFOV);
        }
    }
}
