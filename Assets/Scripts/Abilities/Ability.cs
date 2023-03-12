using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
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
        private bool onCD;
        public int SegmentCount
        {
            get => Segments.Length;
        }

        private int SegmentIndex = 0;
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

        public void Use()
        {
            if (onCD) return;
            
            Segments[SegmentIndex].Perform();
            SegmentIndex++;
            if (SegmentIndex >= SegmentCount)
            {
                onCD = true;
                Task.Delay(TimeSpan.FromSeconds(Segments[^1].GetCooldown())).ContinueWith(t => CoolDownTracker());
            }
        }

        private void CoolDownTracker()
        {
            onCD = false;
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

