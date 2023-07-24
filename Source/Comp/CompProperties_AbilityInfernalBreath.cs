using RimWorld;

namespace HellBionics
{
    public class CompProperties_AbilityInfernalBreath : CompProperties_AbilityEffect
    {
        public CompProperties_AbilityInfernalBreath()
        {
            this.compClass = typeof(CompAbilityEffect_InfernalBreath);
        }

        public float range = 10f;

        public float plasmaCost = 10f;

        public float lineWidthEnd;
    }
}