using AbilityUser;
using RimWorld;
using Verse;

namespace HellBionics
{
    public class InfernalEffect_InfernalDash : Verb_UseAbility
    {
        bool validTarg;

        public override bool CanHitTargetFrom(IntVec3 root, LocalTargetInfo targ)
        {
            if(targ.Thing != null && targ.Thing == this.caster)
            {
                return this.verbProps.targetParams.canTargetSelf;
            }
            
            if(targ.IsValid && targ.CenterVector3.InBoundsWithNullCheck(base.CasterPawn.Map) && !targ.Cell.Fogged(base.CasterPawn.Map) && targ.Cell.Walkable(base.CasterPawn.Map))
            {
                if((root - targ.Cell).LengthHorizontal < this.verbProps.range)
                {
                    ShootLine shootLine;
                    validTarg = this.TryFindShootLineFromTo(root, targ, out shootLine);
                }
                else
                {
                    validTarg = false;
                }
            }
            else
            {
                validTarg = false;
            }

            return validTarg;
        }

        public virtual void Effect()
        {
            LocalTargetInfo t = this.TargetsAoE[0];

            if(t.Cell != default(IntVec3))
            {
                Pawn casterPawn = base.CasterPawn;

                LongEventHandler.QueueLongEvent(delegate
                {
                    FlyingObject flyingObject = (FlyingObject)GenSpawn.Spawn(ThingDef.Named("HB_FlyingObject"), this.CasterPawn.Position, this.CasterPawn.Map);
                    flyingObject.Launch(this.CasterPawn, t.Cell, this.CasterPawn);
                }, "LaunchingFlyerDash", false, null);
            }
        }

        public override void PostCastShot(bool inResult, out bool outResult)
        {
            if(inResult)
            {
                this.Effect();
                outResult = true;
            }

            outResult = inResult;
        }
    }
}