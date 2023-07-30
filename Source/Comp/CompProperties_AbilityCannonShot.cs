using RimWorld;
using Verse;

namespace HellBionics
{
	public class CompProperties_AbilityCannonShot : CompProperties_AbilityEffect
	{
		public CompProperties_AbilityCannonShot()
		{
			this.compClass = typeof(CompAbilityEffect_CannonShot);
		}

        public float plasmaCost;

		public int burstCount;

		public ThingDef projectileDef;
	}
}
