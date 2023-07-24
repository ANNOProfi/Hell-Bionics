using Verse;

namespace HellBionics
{
    public class HediffCompProperties_InfernalUtility : HediffCompProperties
    {
        public HediffCompProperties_InfernalUtility()
        {
            this.compClass = typeof(HediffComp_InfernalUtility);
        }

        public float maximumPlasma = 0f;

        public float remainingPlasma = 0f;
    }
}