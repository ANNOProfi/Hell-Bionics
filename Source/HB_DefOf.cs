using RimWorld;
using Verse;
using System;

namespace HellBionics
{
    [DefOf]
    public static class HB_ThingDefOf
    {
        static HB_ThingDefOf()
		{
			DefOfHelper.EnsureInitializedInCtor(typeof(ThingDefOf));
		}

        [MayRequireRoyalty]
        public static ThingDef HB_PawnDasher;
    }
}