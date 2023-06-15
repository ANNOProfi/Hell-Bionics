using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;

namespace HellBionics
{
    public static partial class InfernalUtility
    {
        public static bool IsInfernal(this Pawn pawn)
        {
            Log.Message("IsInfernal() called");
            if(pawn != null && pawn?.TryGetComp<CompInfernal>() is { } i && i.IsInfernal)
            {
                Log.Message("IsInfernal() = true called");
                return true;
            }

            Log.Message("IsInfernal() = false called");
            return false;
        }

        public static bool HasInfernalHediffs(this Pawn pawn)
        {
            if(pawn == null)
            {
                return false;
            }

            return pawn.health.hediffSet.HasHediff(InfernalDefOf.InfernalLeg) || pawn.health.hediffSet.HasHediff(InfernalDefOf.InfernalJaw);
        }

        public static CompInfernal InfernalComp (this Pawn pawn)
        {
            return pawn?.TryGetComp<CompInfernal>();
        }

        /*public static int GetPartCount(this BodyPartRecord part, string child)
        {
            part.GetChildParts().;
        }*/
    }
}