using Verse;
using RimWorld;
using BrokenPlankFramework;
using System;
using Unity.Mathematics;

namespace HellBionics
{
    public class CompAbilityEffect_CannonShot : CompAbility_SingularTracker
    {
        //private float cost_temp;

        private new CompProperties_AbilityCannonShot Props
        {
            get
            {
                return (CompProperties_AbilityCannonShot)this.props;
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
				return this.Pawn.health.hediffSet.GetFirstHediffOfDef(HB_DefOf.HB_InfernalUtility).TryGetComp<HediffComp_InfernalUtility>();
			}
		}

        private bool HasEnoughPlasma
        {
            get
            {
                return InfernalUtility.MaximumPlasma > 0 && InfernalUtility.RemainingPlasma >= this.Props.plasmaCost;
            }
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

        public override void Apply(LocalTargetInfo target, LocalTargetInfo dest)
        {
            base.Apply(target, dest);
            InfernalUtility.OffsetPlasma(-(this.Props.plasmaCost));
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
			return target.Pawn != null && this.HasEnoughPlasma && parent.RemainingCharges > 0;
		}

        /*public override void AddAbility()
        {
            base.AddAbility();
        }

        public override void RemoveAbility()
        {
            base.RemoveAbility();
        }*/

        public override void DrawEffectPreview(LocalTargetInfo target)
        {
            GenDraw.DrawRadiusRing(target.Cell, this.Props.projectileDef.projectile.explosionRadius);
        }
    }
}