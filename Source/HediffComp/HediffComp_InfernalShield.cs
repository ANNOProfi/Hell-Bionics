using AthenaFramework;
using Verse;
using RimWorld;

namespace HellBionics
{
    public class HediffComp_InfernalShield : AthenaFramework.HediffComp_Shield
    {
        public HediffCompProperties_InfernalShield Props
        {
            get
            {
                return (HediffCompProperties_InfernalShield)this.props;
            }
        }

        private HediffComp_InfernalUtility InfernalUtility
		{
			get
			{
				return this.Pawn.health.hediffSet.GetFirstHediffOfDef(HB_DefOf.HB_InfernalUtility).TryGetComp<HediffComp_InfernalUtility>();
			}
		}

        public override void CompPostTick(ref float severityAdjustment)
        {
            if(!(ticksToReset >0) && !(energy >= MaxEnergy) && (InfernalUtility.RemainingPlasma >= (this.Props.plasmaCost*EnergyRechargeRate)))
            {
                InfernalUtility.OffsetPlasma(-this.Props.plasmaCost*EnergyRechargeRate);
                base.CompPostTick(ref severityAdjustment);
            }
        }
    }
}