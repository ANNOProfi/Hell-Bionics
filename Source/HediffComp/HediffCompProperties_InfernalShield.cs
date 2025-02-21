using Verse;
using RimWorld;
using BrokenPlankFramework;

namespace HellBionics
{
    public class HediffCompProperties_InfernalShield : HediffCompProperties_Shield
    {
        public HediffCompProperties_InfernalShield()
        {
            this.compClass = typeof(HediffComp_InfernalShield);
        }

        public float plasmaCost = 1.5f;
    }
}