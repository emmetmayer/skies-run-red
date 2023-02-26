using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;

namespace Abilities
{
    public enum Activation
    {
        None,
        Manual,
        Automatic,
        
    }
    public abstract class Segment : ScriptableObject
    {
        public int Damage;
        public float Distance;
        public float Duration;
        public float Cooldown;
        public Vector3 Direction;
        public Activation Type;
        public Element Attribute;

        public virtual int GetDamage()
        {
            return Damage;
        }
        public virtual float GetDistance()
        {
            return Distance;
        }
        public virtual float GetDuration()
        {
            return Duration;
        }
        public virtual Vector3 GetDirection()
        {
            return Direction;
        } 
        public virtual Activation GetActivationType()
        {
            return Type;
        }

        public virtual Element GetElementType()
        {
            return Attribute;
        }

        public virtual float GetCooldown()
        {
            return Cooldown;
        }
        
    }

    public class DamageInPlace : Segment
    {
        public Vector3 Direction;
        [Tooltip("Amount of time the hitbox is active")]
        public float Duration;
        public int Damage;
        //i need to figure out how i want to store the hitbox
       
    }
    

    public class Move : Segment
    {
        //i should probably add rotation or animation support to this part
        public float Distance;
        [Tooltip("Direction to travel relative to transform orientation")]
        public Vector3 Direction;
        [Tooltip("Amount of time it takes to completely move Distance")]
        public float Duration;
        
        
    }

    public class Defend : Segment
    {
        //[Tooltip("Direction to travel relative to transform orientation")]
        //public Vector3 Direction;
        [Tooltip("Amount of time the defense is active")]
        public float Duration;

      
    }
    
    public class MoveAndDamage : Segment
    {
        public float Distance;
        [Tooltip("Direction to travel relative to transform orientation")]
        public Vector3 Direction;
        [Tooltip("Amount of time it takes to completely move Distance")]
        public float Duration;
        public int Damage;
        //i need to figure out how i want to store the hitbox

    }

    public class DefendAndDamage : Segment
    {
        [Tooltip("Amount of time the defense is active")]
        public float Duration;
        public int Damage;
        //i need to figure out how i want to store the hitbox
    }

    public class DefendAndMove : Segment
    {
        public float Distance;
        [Tooltip("Direction to travel relative to transform orientation")]
        public Vector3 Direction;
        [Tooltip("Amount of time it takes to completely move Distance (also the time the defense is active)")]
        public float Duration;
        //i need to figure out how i want to store the hitbox
    }

    public class DDM : Segment
    {
        public float Distance;
        [Tooltip("Direction to travel relative to transform orientation")]
        public Vector3 Direction;
        [Tooltip("Amount of time it takes to completely move Distance")]
        public float Duration;
        public int Damage;
        //i need to figure out how i want to store the hitbox
    }
}

