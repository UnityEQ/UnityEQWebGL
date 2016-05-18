/* CharacterColliderController - Is responsible for controlling the state of any dynamic (movable) collider (e.g. player or NPC)
 * Need to keep track of things like whether character is sitting, ducking or standing
 * Created - April 14 2013
 * PegLegPete (goatdude@gmail.com)
 */

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using EQBrowser;

public class CharacterColliderController : MonoBehaviour
{
    Collider m_collider;

    public bool isLocal;

    protected float m_maxHeight;

    public float MaxHeight
    {
        get { return m_maxHeight; }
        set 
        { 
            m_maxHeight = value;

            if (m_collider is CharacterController)
            {
                (m_collider as CharacterController).stepOffset = m_maxHeight * 0.2f;
            }
        }
    }

    public enum ColliderState
    {
        Standing, // = 100% height
        Ducking, // = 50%
        Sitting // 40%
    }

    public static float[] HeightMultipliers = new float[] { 1f, 0.5f, 0.4f };

    protected ColliderState m_curState;

    void Awake()
    {
        m_collider = this.gameObject.GetComponent<Collider>();

        if (m_collider == null)
        {
            Debug.LogWarning(string.Format("Cannot find Collider on character {0}, adding one.", this.gameObject.name));
            if (isLocal)
            {
                m_collider = this.gameObject.AddComponent<CharacterController>();
            }
            else
            {
                m_collider = this.gameObject.AddComponent<CapsuleCollider>();
            }
        }

        if (m_collider is CharacterController)
        {
            MaxHeight = (m_collider as CharacterController).height;
        }
        else if (m_collider is CapsuleCollider)
        {
            MaxHeight = (m_collider as CapsuleCollider).height;
        }
        
    }

    public void SetState(ColliderState newState)
    {
        m_curState = newState;

        if (isLocal)
        {
            (m_collider as CharacterController).height = m_maxHeight * HeightMultipliers[(int)m_curState];
        }
        else
        {
            (m_collider as CapsuleCollider).height = m_maxHeight * HeightMultipliers[(int)m_curState];
        }
    }

    public ColliderState GetState()
    {
        return m_curState;
    }

    public float GetCurrentColliderHeight()
    {
        return (m_collider as CapsuleCollider).height;
    }
}