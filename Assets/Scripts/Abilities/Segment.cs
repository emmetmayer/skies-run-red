using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.Analytics;

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

        public abstract void Perform();
    }

    public class DamageInPlace : Segment
    {
       // public Vector3 Direction;
        [Tooltip("Amount of time the hitbox is active")]
        public float Duration;
        //public int Damage;
        //i need to figure out how i want to store the hitbox
        public override void Perform()
        {
            
            //invoke damaging state in player for duration
            //move hitbox to the direction indicated relative to the player
            //this is done with colliders?
            throw new System.NotImplementedException();
        }
    }
    

    public class Move : Segment
    {
        //i should probably add rotation or animation support to this part
        //public float Distance;
        [Tooltip("Direction to travel relative to transform orientation")]
        public Vector3 Direction;
        [Tooltip("Amount of time it takes to completely move Distance")]
        public float Duration;
        
        public override void Perform()
        {
            //calculate how far to move per fixed frame in order to finish by duration
            //offload movement onto character controller
            throw new System.NotImplementedException();
        }
        
    }

    public class Defend : Segment
    {
        //[Tooltip("Direction to travel relative to transform orientation")]
        //public Vector3 Direction;
        [Tooltip("Amount of time the defense is active")]
        public float Duration;

        public override void Perform()
        {
            //determine how it defends. apply the affect if damage would be taken
            throw new System.NotImplementedException();
        }
    }
    
    public class MoveAndDamage : Segment
    {
        //public float Distance;
        [Tooltip("Direction to travel relative to transform orientation")]
        public Vector3 Direction;
        [Tooltip("Amount of time it takes to completely move Distance")]
        public float Duration;
        //public int Damage;
        //i need to figure out how i want to store the hitbox

        public override void Perform()
        {
            //move but have the hitbox active?
            throw new System.NotImplementedException();
        }
    }

    public class DefendAndDamage : Segment
    {
        [Tooltip("Amount of time the defense is active")]
        public float Duration;
        //public int Damage;
        //i need to figure out how i want to store the hitbox
        public override void Perform()
        {
            //i-frames while active hitbox 
            throw new System.NotImplementedException();
        }
    }

    public class DefendAndMove : Segment
    {
        //public float Distance;
        [Tooltip("Direction to travel relative to transform orientation")]
        public Vector3 Direction;
        [Tooltip("Amount of time it takes to completely move Distance (also the time the defense is active)")]
        public float Duration;
        //i need to figure out how i want to store the hitbox
        public override void Perform()
        {
            //i frame dash
            throw new System.NotImplementedException();
        }
    }

    public class DamageDefendMove : Segment
    {
        public float Distance;
        [Tooltip("Direction to travel relative to transform orientation")]
        public Vector3 Direction;
        [Tooltip("Amount of time it takes to completely move Distance")]
        public float Duration;
        //public int Damage;
        //i need to figure out how i want to store the hitbox
        public override void Perform()
        {
            
            throw new System.NotImplementedException();
        }
    }
}

