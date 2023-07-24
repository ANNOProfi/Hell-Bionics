using System;
using System.Collections.Generic;
using RimWorld;
using Verse;

namespace HellBionics
{
    public class HediffComp_Plasma : HediffComp
    {
        private bool initialized = false;

        public HediffCompProperties_Plasma Props
        {
            get
            {
                return (HediffCompProperties_Plasma)this.props;
            }
        }

        private Pawn pawn
        {
            get
            {
                return this.parent.pawn;
            }
        }

        private HediffComp_InfernalUtility infernalUtility
		{
			get
			{
				return this.pawn.health.hediffSet.GetFirstHediffOfDef(HB_DefOf.HB_InfernalUtility).TryGetComp<HediffComp_InfernalUtility>();
			}
		}

        private void Initialize()
        {
            if(pawn.health.hediffSet.GetFirstHediffOfDef(HB_DefOf.HB_InfernalUtility) == null)
            {
                pawn.health.AddHediff(HB_DefOf.HB_InfernalUtility);
            }
            infernalUtility.UpdateValues(this.Props.maximumBase);
            this.initialized = true;
        }

        public override void CompPostTick(ref float severityAdjustment)
        {
            if(!this.initialized)
            {
                Initialize();
            }
            if(infernalUtility.RemainingPlasma < infernalUtility.MaximumPlasma)
            {
                infernalUtility.OffsetPlasma(this.Props.amountPerTick);
            }  
        }

        public override void CompPostPostRemoved()
        {
            infernalUtility.UpdateValues(-this.Props.maximumBase);
        }
    }
}