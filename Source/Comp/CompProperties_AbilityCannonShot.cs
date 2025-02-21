using RimWorld;
using Verse;
using BrokenPlankFramework;

namespace HellBionics
{
	public class CompProperties_AbilityCannonShot : CompProperties_AbilitySingularTracker
	{
		public CompProperties_AbilityCannonShot()
		{
			this.compClass = typeof(CompAbilityEffect_CannonShot);
		}

        public float plasmaCost;

		//public int burstCount;

		public ThingDef projectileDef;
	}
}
