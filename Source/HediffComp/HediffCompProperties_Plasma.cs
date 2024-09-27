using Verse;

namespace HellBionics
{
    public class HediffCompProperties_Plasma : HediffCompProperties
    {
        public HediffCompProperties_Plasma()
        {
            this.compClass = typeof(HediffComp_Plasma);
        }

        public float amountPerTick = 0f;

        public float maximumBase = 10f;

        public float plasmaPerTick = 0.01f;

        public float partEfficiency = 1f;
    }
}