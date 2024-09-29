using AthenaFramework;
using Verse;

namespace HellBionics
{
    public class HediffComp_Plasma : HediffComp, IStageOverride
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

        private bool disabled = false;

        private float partEfficiencyCached = 0f;

        public override void CompPostPostAdd(DamageInfo? dinfo)
        {
            if(Pawn.health.hediffSet.GetFirstHediffOfDef(HB_DefOf.HB_InfernalUtility) == null)
            {
                Pawn.health.AddHediff(HB_DefOf.HB_InfernalUtility);
            }
            InfernalUtility.UpdateValues(this.Props.maximumBase, this.Props.plasmaPerTick);
        }

        public override void CompPostPostRemoved()
        {
            InfernalUtility.UpdateValues(-this.Props.maximumBase, -this.Props.plasmaPerTick);
            if(InfernalUtility.MaximumPlasma == 0)
            {
                Pawn.health.RemoveHediff(Pawn.health.hediffSet.GetFirstHediffOfDef(HB_DefOf.HB_InfernalUtility));
            }
        }

        public HediffStage GetStage(HediffStage stage)
        {
            if(partEfficiencyCached == 0f)
            {
                partEfficiencyCached = Def.addedPartProps.partEfficiency;
            }

            if(partEfficiencyCached >= 1f)
            {
                if(disabled && InfernalUtility.RemainingPlasma > 0f)
                {
                    stage.partEfficiencyOffset = 0f;

                    disabled = false;
                }
                else if(!disabled && InfernalUtility.RemainingPlasma == 0f)
                {
                    stage.partEfficiencyOffset = -partEfficiencyCached;

                    disabled = true;
                }
            }
            else if(partEfficiencyCached < 1f)
            {
                stage.partEfficiencyOffset = 0f;
            }

            return stage;
        }

        public override void CompExposeData()
        {
            Scribe_Values.Look(ref disabled, "disabled", false);
            Scribe_Values.Look(ref partEfficiencyCached, "partEfficiencyCached", 0f);
        }
    }
}