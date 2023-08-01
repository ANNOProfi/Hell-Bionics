using RimWorld;
using Verse;

namespace HellBionics
{
    [DefOf]
    public static class HB_DefOf
    {
        static HB_DefOf()
		{
			DefOfHelper.EnsureInitializedInCtor(typeof(HB_DefOf));
		}

        //[MayRequireRoyalty]
        public static ThingDef HB_PawnDasher;

        public static EffecterDef HB_InfernalBreath;

        public static HediffDef HB_InfernalUtility;
    }
}