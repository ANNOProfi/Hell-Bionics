using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using Verse;
using RimWorld;
using AthenaFramework;
using Verse.AI;

namespace HellBionics
{
    public class CompAbilityEffect_InfernalDash : CompAbility_SingularTracker
    {
        private float range_temp;

        private new CompProperties_AbilityInfernalDash Props
        {
            get
            {
                return (CompProperties_AbilityInfernalDash)this.props;
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

        private bool HasEnoughPlasma
        {
            get
            {
                return infernalUtility.MaximumPlasma > 0 && infernalUtility.RemainingPlasma >= this.Props.plasmaCost;
            }
        }

        public override void AddAbility()
        {
            base.AddAbility();
            this.range_temp = this.Props.range / this.abilityCount;
            adjustRange();
        }

        public override void RemoveAbility()
        {
            base.RemoveAbility();
            this.range_temp = this.Props.range / (this.abilityCount+2);
            adjustRange();
        }

        public void adjustRange()
        {
            this.Props.range = this.range_temp*(this.abilityCount+1);
            infernalUtility.InfernalDashRange = this.Props.range;
        }

        public override void Apply(LocalTargetInfo target, LocalTargetInfo dest)
        {
            base.Apply(target, dest);
            infernalUtility.OffsetPlasma(-(this.Props.plasmaCost*(target.Cell.DistanceTo(pawn.PositionHeld)/this.Props.range)));
        }

        public override bool GizmoDisabled(out string reason)
        {
            if(infernalUtility.MaximumPlasma == 0)
            {
                reason = "No Hediff for this ability. If you are seeing this, something has gone wrong.";
                return true;
            }
            if(infernalUtility.RemainingPlasma < this.Props.plasmaCost)
            {
                reason = "Not enough Plasma".Translate(pawn);
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