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

        public override void CompPostPostAdd(DamageInfo? dinfo)
        {
            if(pawn.health.hediffSet.GetFirstHediffOfDef(HB_DefOf.HB_InfernalUtility) == null)
            {
                pawn.health.AddHediff(HB_DefOf.HB_InfernalUtility);
            }
            infernalUtility.UpdateValues(this.Props.maximumBase, this.Props.plasmaPerTick);
        }

        public override void CompPostPostRemoved()
        {
            infernalUtility.UpdateValues(-this.Props.maximumBase, -this.Props.plasmaPerTick);
            if(infernalUtility.MaximumPlasma == 0)
            {
                pawn.health.RemoveHediff(pawn.health.hediffSet.GetFirstHediffOfDef(HB_DefOf.HB_InfernalUtility));
            }
        }
    }
}