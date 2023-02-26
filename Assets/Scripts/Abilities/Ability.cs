using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Abilities
{
    public enum Element
    {
        NONE,
        Fire,
        Ice,
        Lightning,
        Shadow
    }
    public abstract class Ability : ScriptableObject
    {
        public string Name;
        public Element Type;
        public int SegmentCount
        {
            get => Segments.Length;
        }
        public Segment[] Segments;
        
        //possibly defunct
        public int[] Amounts;
        public int Damage(int index)
        {
            if (index >= 0 && index < Segments.Length) return Segments[index].GetDamage();
            else if (index < 0) throw new Exception("Index may not be negative");
            else throw new Exception("Index exceeds the number of segments of this ability");
        }
        public float Distance(int index)
        {
            if (index >= 0 && index < Segments.Length) return Segments[index].GetDistance();
            else if (index < 0) throw new Exception("Index may not be negative");
            else throw new Exception("Index exceeds the number of segments of this ability");
        }
        public float Duration(int index)
        {
            if (index >= 0 && index < Segments.Length) return Segments[index].GetDuration();
            else if (index < 0) throw new Exception("Index may not be negative");
            else throw new Exception("Index exceeds the number of segments of this ability");
        }
        public Activation ActivationType(int index)
        {
            if (index >= 0 && index < Segments.Length) return Segments[index].GetActivationType();
            else if (index < 0) throw new Exception("Index may not be negative");
            else throw new Exception("Index exceeds the number of segments of this ability");
        }
        public Element ElementType(int index)
        {
            if (index >= 0 && index < Segments.Length) return Segments[index].GetElementType();
            else if (index < 0) throw new Exception("Index may not be negative");
            else throw new Exception("Index exceeds the number of segments of this ability");
        }
        public float Cooldown(int index)
        {
            if (index >= 0 && index < Segments.Length) return Segments[index].GetCooldown();
            else if (index < 0) throw new Exception("Index may not be negative");
            else throw new Exception("Index exceeds the number of segments of this ability");
        }
    }

    public class AggroAbility : Ability
    {
        
    }
    public class DashAbility : Ability
    {
        
    }
    public class DefenseAbility : Ability
    {
        
    }
}

