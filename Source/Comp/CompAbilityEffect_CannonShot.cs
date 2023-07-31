using Verse;
using RimWorld;
using AthenaFramework;
using UnityEngine;

namespace HellBionics
{
    public class CompAbilityEffect_CannonShot : CompAbilityEffect
    {
        //private float cost_temp;

        private new CompProperties_AbilityCannonShot Props
        {
            get
            {
                return (CompProperties_AbilityCannonShot)this.props;
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

        public override void Apply(LocalTargetInfo target, LocalTargetInfo dest)
        {
            base.Apply(target, dest);
            infernalUtility.OffsetPlasma(-(this.Props.plasmaCost));
            //for(int i = 0; i< this.Props.burstCount; i++)
            //{
                this.LaunchProjectile(target);
            //}
        }

        private void LaunchProjectile(LocalTargetInfo target)
		{
			Pawn pawn = this.parent.pawn;
			((Projectile)GenSpawn.Spawn(this.Props.projectileDef, pawn.Position, pawn.Map, WipeMode.Vanish)).Launch(pawn, pawn.DrawPos, target, target, ProjectileHitFlags.IntendedTarget, false, null, null);
		}

        public override bool AICanTargetNow(LocalTargetInfo target)
		{
			return target.Pawn != null && this.HasEnoughPlasma;
		}

        /*public override void AddAbility()
        {
            base.AddAbility();
            this.Props.burstCount = (this.abilityCount+1);
            this.cost_temp = this.Props.plasmaCost / this.abilityCount;
            this.Props.plasmaCost = this.cost_temp*(this.abilityCount+1);
        }

        public override void RemoveAbility()
        {
            base.RemoveAbility();
            this.Props.burstCount = (this.abilityCount+1);
            this.cost_temp = this.Props.plasmaCost / (this.abilityCount+2);
            this.Props.plasmaCost = this.cost_temp*(this.abilityCount+1);
        }*/

        public override void DrawEffectPreview(LocalTargetInfo target)
        {
            GenDraw.DrawRadiusRing(target.Cell, this.Props.projectileDef.projectile.explosionRadius);
        }
    }
}