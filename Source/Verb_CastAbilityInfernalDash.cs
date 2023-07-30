using UnityEngine;
using Verse;
using RimWorld;

namespace HellBionics
{
    public class Verb_CastAbilityInfernalDash : Verb_CastAbility
    {
        public override bool MultiSelect
		{
			get
			{
				return true;
			}
		}

		private HediffComp_InfernalUtility infernalUtility
		{
			get
			{
				return this.CasterPawn.health.hediffSet.GetFirstHediffOfDef(HB_DefOf.HB_InfernalUtility).TryGetComp<HediffComp_InfernalUtility>();
			}
		}

        protected override float EffectiveRange
		{
			get
			{
	
				return infernalUtility.InfernalDashRange;
				
			}
		}

        protected override bool TryCastShot()
		{
			return base.TryCastShot() && HB_JumpUtility.DoJump(this.CasterPawn, this.currentTarget, base.ReloadableCompSource, this.verbProps);
		}

        public override void OnGUI(LocalTargetInfo target)
		{
			if (this.CanHitTarget(target) && HB_JumpUtility.ValidJumpTarget(this.caster.Map, target.Cell))
			{
				base.OnGUI(target);
				return;
			}
			GenUI.DrawMouseAttachment(TexCommand.CannotShoot);
		}

        public override void OrderForceTarget(LocalTargetInfo target)
		{
			HB_JumpUtility.OrderJump(this.CasterPawn, target, this, this.EffectiveRange);
		}

        public override bool ValidateTarget(LocalTargetInfo target, bool showMessages = true)
		{
			return this.caster != null && this.CanHitTarget(target) && HB_JumpUtility.ValidJumpTarget(this.caster.Map, target.Cell) && ReloadableUtility.CanUseConsideringQueuedJobs(this.CasterPawn, base.EquipmentSource, true);
		}

        public override bool CanHitTargetFrom(IntVec3 root, LocalTargetInfo targ)
		{
			return HB_JumpUtility.CanHitTargetFrom(this.CasterPawn, root, targ, this.EffectiveRange);
		}

        public override void DrawHighlight(LocalTargetInfo target)
		{
			if (target.IsValid && HB_JumpUtility.ValidJumpTarget(this.caster.Map, target.Cell))
			{
				GenDraw.DrawTargetHighlightWithLayer(target.CenterVector3, AltitudeLayer.MetaOverlays);
			}
			GenDraw.DrawRadiusRing(this.caster.Position, this.EffectiveRange, Color.white, (IntVec3 c) => GenSight.LineOfSight(this.caster.Position, c, this.caster.Map, false, null, 0, 0) && HB_JumpUtility.ValidJumpTarget(this.caster.Map, c));
		}
    }
}