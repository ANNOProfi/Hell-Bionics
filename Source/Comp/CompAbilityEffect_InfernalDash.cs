using Verse;
using RimWorld;
using BrokenPlankFramework;

namespace HellBionics
{
    public class CompAbilityEffect_InfernalDash : CompAbility_SingularTracker
    {

        private new CompProperties_AbilityInfernalDash Props
        {
            get
            {
                return (CompProperties_AbilityInfernalDash)this.props;
            }
        }

        private Pawn Pawn
        {
            get
            {
                return this.parent.pawn;
            }
        }

        private HediffComp_InfernalUtility InfernalUtility
		{
			get
			{
				return Pawn.health.hediffSet.GetFirstHediffOfDef(HB_DefOf.HB_InfernalUtility).TryGetComp<HediffComp_InfernalUtility>();
			}
		}

        private bool HasEnoughPlasma
        {
            get
            {
                return InfernalUtility.MaximumPlasma > 0 && InfernalUtility.RemainingPlasma >= this.Props.plasmaCost;
            }
        }

        public override void AddAbility()
        {
            base.AddAbility();
            InfernalUtility.dashCount = this.abilityCount+1;
            if(!InfernalUtility.dashInitialised)
            {
                InfernalUtility.InfernalDashRange = Props.range;
                InfernalUtility.dashCost = Props.plasmaCost;
                InfernalUtility.dashInitialised = true;
            }
        }

        public override void RemoveAbility()
        {
            base.RemoveAbility();
            InfernalUtility.dashCount = this.abilityCount+1;
        }

        public override void Apply(LocalTargetInfo target, LocalTargetInfo dest)
        {
            base.Apply(target, dest);
            InfernalUtility.OffsetPlasma(-(this.Props.plasmaCost*((int)(Pawn.Position.DistanceTo(target.CenterVector3.ToIntVec3())/Props.range)+1)));
        }

        public override bool GizmoDisabled(out string reason)
        {
            if(InfernalUtility.MaximumPlasma == 0)
            {
                reason = "No Hediff for this ability. If you are seeing this, something has gone wrong.";
                return true;
            }
            if(InfernalUtility.RemainingPlasma < this.Props.plasmaCost)
            {
                reason = "Not enough Plasma".Translate(Pawn);
                return true;
            }

            reason = null;
            return false;
        }

        public override bool AICanTargetNow(LocalTargetInfo target)
        {
            return this.HasEnoughPlasma;
        }

        /*private float TotalPlasmaCostOfQueuedAbilities()
        {
            Pawn_JobTracker jobs = this.parent.pawn.jobs;
            object obj;

            if(jobs == null)
            {
                obj = null;
            }
            else
            {
                Job curJob = jobs.curJob;
                obj = ((curJob != null) ? curJob.verbToUse : null);
            }

            Verb_CastAbility verb_CastAbility = obj as Verb_CastAbility;
            float num;

            if(verb_CastAbility == null)
            {
                num = 0f;
            }
            else
            {
                InfernalAbility ability = (InfernalAbility)verb_CastAbility.ability;
                num = ((ability != null) ? ability.PlasmaCost() : 0f);
            }
        }*/
    }
}