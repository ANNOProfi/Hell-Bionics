using RimWorld;
using Verse;

namespace HellBionics
{
    public class HediffComp_Plasma : HediffComp
    {
        public HediffCompProperties_Plasma Props
        {
            get
            {
                return (HediffCompProperties_Plasma)this.props;
            }
        }

        private HediffComp_InfernalUtility InfernalUtility
		{
			get
			{
				return Pawn.health.hediffSet.GetFirstHediffOfDef(HB_DefOf.HB_InfernalUtility).TryGetComp<HediffComp_InfernalUtility>();
			}
		}

        //private bool disabled = false;

        //private float partEfficiencyCached = 0f;

        //private float ticksToReset = 60;

        public override void CompPostPostAdd(DamageInfo? dinfo)
        {
            if(Pawn.health.hediffSet.GetFirstHediffOfDef(HB_DefOf.HB_InfernalUtility) == null)
            {
                Pawn.health.AddHediff(HB_DefOf.HB_InfernalUtility);
            }
            InfernalUtility.UpdateValues(this.Props.maximumBase, this.Props.plasmaPerTick);
            InfernalUtility.OffsetPlasma(1f);
        }

        public override void CompPostPostRemoved()
        {
            InfernalUtility.UpdateValues(-this.Props.maximumBase, -this.Props.plasmaPerTick);
            if(InfernalUtility.MaximumPlasma == 0)
            {
                Pawn.health.RemoveHediff(Pawn.health.hediffSet.GetFirstHediffOfDef(HB_DefOf.HB_InfernalUtility));
            }
        }

        /*public override void CompPostTick(ref float severityAdjustment)
        {
            if(ticksToReset > 0)
            {
                ticksToReset--;
            }
        }

        public HediffStage GetStage(HediffStage stage)
        {
            if(partEfficiencyCached == 0f)
            {
                partEfficiencyCached = Def.addedPartProps.partEfficiency;
            }

            if(disabled && InfernalUtility.RemainingPlasma > 0f && ticksToReset <= 0)
            {
                stage.partEfficiencyOffset = 0f;

                disabled = false;

                ticksToReset = -1;
            }
            else if(!disabled && InfernalUtility.RemainingPlasma == 0f)
            {
                stage.partEfficiencyOffset = -partEfficiencyCached;

                disabled = true;
                
                ticksToReset = 60;
            }

            return stage;
        }

        public override void CompExposeData()
        {
            Scribe_Values.Look(ref disabled, "disabled", false);
            Scribe_Values.Look(ref partEfficiencyCached, "partEfficiencyCached", 0f);
        }*/
    }
}