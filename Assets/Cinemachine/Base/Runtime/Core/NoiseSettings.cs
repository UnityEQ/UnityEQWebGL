using UnityEngine;
using System;

namespace Cinemachine
{
    /// <summary>
    /// Container describing the behaviour of the Cinemachine Noise module when 
    /// applied to a <see cref="ICinemachineCamera"/>
    /// </summary>
    public sealed class NoiseSettings : ScriptableObject
    {
        /// <summary>
        /// Describes the behaviour for a channel of noise
        /// </summary>
        [Serializable]
        public struct NoiseParams
        {
            /// <summary>
            /// The amplitude of the noise for this channel
            /// </summary>
            public float Amplitude;
            /// <summary>
            /// The frequency of noise for this channel
            /// </summary>
            public float Frequency;
        }

        /// <summary>
        /// Contains the behaviour of noise for the noise module for all 3 cardinal axes of the camera
        /// </summary>
        [Serializable]
        public struct TransformNoiseParams
        {
            /// <summary>
            /// Noise data for X-axis
            /// </summary>
            public NoiseParams X;
            /// <summary>
            /// Noise data for Y-axis
            /// </summary>
            public NoiseParams Y;
            /// <summary>
            /// Noise data for Z-axis
            /// </summary>
            public NoiseParams Z;
        }

        [SerializeField]
        [Tooltip("This is the number of noise channels for the virtual camera's position. Convincing noise setups typically mix low, medium and high frequencies together, so start with a size of 3")]
        private TransformNoiseParams[] m_Position = new TransformNoiseParams[0];

        /// <summary>
        /// Gets the array of positional noise channels for this <c>NoiseSettings</c>
        /// </summary>
        public TransformNoiseParams[] PositionNoise { get { return m_Position; } }

        [SerializeField]
        [Tooltip("This is the number of noise channels for the virtual camera's position. Convincing noise setups typically mix low, medium and high frequencies together, so start with a size of 3")]
        private TransformNoiseParams[] m_Orientation = new TransformNoiseParams[0];

        /// <summary>
        /// Gets the array of orientation noise channels for this <c>NoiseSettings</c>
        /// </summary>
        public TransformNoiseParams[] OrientationNoise { get { return m_Orientation; } }
    }
}
