using RimWorld;
using BrokenPlankFramework;

namespace HellBionics
{
    public class CompProperties_AbilityInfernalDash : CompProperties_AbilitySingularTracker
    {
        public CompProperties_AbilityInfernalDash()
        {
            this.compClass = typeof(CompAbilityEffect_InfernalDash);
        }

        public float range = 10f;

        public float plasmaCost = 10f;
    }
}