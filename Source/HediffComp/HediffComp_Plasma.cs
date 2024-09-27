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

        private bool disabled = false;

        public override void CompPostPostAdd(DamageInfo? dinfo)
        {
            if(Pawn.health.hediffSet.GetFirstHediffOfDef(HB_DefOf.HB_InfernalUtility) == null)
            {
                Pawn.health.AddHediff(HB_DefOf.HB_InfernalUtility);
            }
            InfernalUtility.UpdateValues(this.Props.maximumBase, this.Props.plasmaPerTick);
        }

        public override void CompPostTick(ref float severityAdjustment)
        {
            base.CompPostTick(ref severityAdjustment);

            if(parent.CurStage == null)
            {
                return;
            }

            if(Props.partEfficiency >= 1f)
            {
                if(disabled && InfernalUtility.RemainingPlasma > 0f)
                {
                    parent.CurStage.partEfficiencyOffset = Props.partEfficiency - 1f;

                    disabled = false;
                }
                else if(!disabled && InfernalUtility.RemainingPlasma == 0f)
                {
                    parent.CurStage.partEfficiencyOffset = -1f;

                    disabled = true;
                }
            }
            else if(Props.partEfficiency < 1f)
            {
                parent.CurStage.partEfficiencyOffset = Props.partEfficiency - 1f;
            }
        }

        public override void CompPostPostRemoved()
        {
            InfernalUtility.UpdateValues(-this.Props.maximumBase, -this.Props.plasmaPerTick);
            if(InfernalUtility.MaximumPlasma == 0)
            {
                Pawn.health.RemoveHediff(Pawn.health.hediffSet.GetFirstHediffOfDef(HB_DefOf.HB_InfernalUtility));
            }
        }
    }
}