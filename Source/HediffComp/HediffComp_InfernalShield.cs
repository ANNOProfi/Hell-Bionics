using Verse;
using RimWorld;
using System;
using BrokenPlankFramework;

namespace HellBionics
{
    public class HediffComp_InfernalShield : HediffComp_Shield
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
            base.CompPostTick(ref severityAdjustment);
            if(!(ticksToReset > 0) && !(energy >= MaxEnergy) && (InfernalUtility.RemainingPlasma >= (this.Props.plasmaCost*EnergyRechargeRate)+0.1))
            {
                InfernalUtility.OffsetPlasma(-this.Props.plasmaCost*EnergyRechargeRate);
            }
            else if(!(InfernalUtility.RemainingPlasma >= (this.Props.plasmaCost*EnergyRechargeRate)))
            {
                energy = Math.Max(energy - EnergyRechargeRate, 0f);
            }
        }
    }
}