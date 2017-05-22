using UnityEngine;

namespace Cinemachine
{
    /// <summary>
    /// Property applied to <see cref="CinemachineBlendDefinition"/>.
    /// </summary>
    public sealed class CinemachineBlendDefinitionPropertyAttribute : PropertyAttribute
    {
    }

    /// <summary>
    /// Suppresses live-save for a property
    /// </summary>
    public sealed class NoSaveDuringPlayAttribute : PropertyAttribute
    {
    }

    /// <summary>
    /// Specify a minimum value on an int, float, or vector
    /// </summary>
    public sealed class MinAttribute : PropertyAttribute
    {
        public readonly float min;
        public MinAttribute(float min)
        {
            this.min = min;
        }
    }

    /// <summary>
    /// Get the inspector to invoke Get/Set property accessors for a field
    /// </summary>
    public sealed class GetSetAttribute : PropertyAttribute
    {
        public readonly string name;
        public bool dirty;
        public GetSetAttribute(string name)
        {
            this.name = name;
        }
    }}
