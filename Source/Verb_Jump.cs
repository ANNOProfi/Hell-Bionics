using RimWorld;
using UnityEngine;
using Verse;

namespace HellBionics
{
    public class Verb_InfernalDash : Verb_Jump
    {
        private float cachedEffectiveRange = -1f;

        protected override float EffectiveRange
        {
            get
            {
                if (!(cachedEffectiveRange < 0f)) return cachedEffectiveRange;
                cachedEffectiveRange = EquipmentSource?.GetStatValue(StatDefOf.JumpRange) ?? verbProps.range;

                return cachedEffectiveRange;
            }
        }

        public override bool MultiSelect => true;


        protected override bool TryCastShot()
        {
            Log.Message("Infernal_Dash called");
            return JumpUtility.DoJump(CasterPawn, currentTarget, ReloadableCompSource, verbProps);
        }

        public override void OrderForceTarget(LocalTargetInfo target)
        {
            JumpUtility.OrderJump(CasterPawn, target, this, EffectiveRange);
        }

        public override bool ValidateTarget(LocalTargetInfo target, bool showMessages = true)
        {
            return caster != null && CanHitTarget(target) && JumpUtility.ValidJumpTarget(caster.Map, target.Cell) &&
                   ReloadableUtility.CanUseConsideringQueuedJobs(CasterPawn, EquipmentSource);
        }

        public override bool CanHitTargetFrom(IntVec3 root, LocalTargetInfo targ)
        {
            return JumpUtility.CanHitTargetFrom(CasterPawn, root, targ, EffectiveRange);
        }

        public override void OnGUI(LocalTargetInfo target)
        {
            if (CanHitTarget(target) && JumpUtility.ValidJumpTarget(caster.Map, target.Cell))
            {
                base.OnGUI(target);
                return;
            }

            GenUI.DrawMouseAttachment(TexCommand.CannotShoot);
        }

        public override void DrawHighlight(LocalTargetInfo target)
        {
            if (target.IsValid && JumpUtility.ValidJumpTarget(caster.Map, target.Cell))
            {
                GenDraw.DrawTargetHighlightWithLayer(target.CenterVector3, AltitudeLayer.MetaOverlays);
            }

            GenDraw.DrawRadiusRing(caster.Position, EffectiveRange, Color.white,
                c => GenSight.LineOfSight(caster.Position, c, caster.Map) && JumpUtility.ValidJumpTarget(caster.Map, c));
        }
    }
}
