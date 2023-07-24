using RimWorld;
using Verse;
//using VFECore.Abilities;
using AthenaFramework;

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

        //public static HediffDef InfernalLeg;

        //public static HediffDef InfernalJaw;

        public static HediffDef HB_InfernalUtility;

        //public static ThingDef AbilityStraightPawnFlyer;

        //public static VFECore.Abilities.AbilityDef HB_InfernalDash;
    }
}