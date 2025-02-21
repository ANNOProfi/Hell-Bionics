using System.Collections.Generic;
using UnityEngine;
using Verse;
using RimWorld;
using BrokenPlankFramework;

namespace HellBionics
{
    public class CompAbilityEffect_InfernalBreath : CompAbility_SingularTracker
    {
        private List<IntVec3> tmpCells = new List<IntVec3>();

        private new CompProperties_AbilityInfernalBreath Props
        {
            get
            {
                return (CompProperties_AbilityInfernalBreath)this.props;
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
                return InfernalUtility.MaximumPlasma > 0 && InfernalUtility.RemainingPlasma >= Props.plasmaCost;
            }
        }

        public override bool GizmoDisabled(out string reason)
        {
            if(InfernalUtility.MaximumPlasma == 0)
            {
                reason = "No Hediff for this ability. If you are seeing this, something has gone wrong.";
                return true;
            }
            if(InfernalUtility.RemainingPlasma < Props.plasmaCost)
            {
                reason = "Not enough Plasma".Translate(Pawn);
                return true;
            }



            reason = null;
            return false;
        }

        public override void Apply(LocalTargetInfo target, LocalTargetInfo dest)
		{
            InfernalUtility.OffsetPlasma(-this.Props.plasmaCost);

			IntVec3 position = this.parent.pawn.Position;
			float num = Mathf.Atan2((float)(-(float)(target.Cell.z - position.z)), (float)(target.Cell.x - position.x)) * 57.29578f;
			FloatRange value = new FloatRange(num - 10f, num + 10f);
			GenExplosion.DoExplosion(position, this.parent.pawn.MapHeld, this.Props.range, DamageDefOf.Flame, Pawn, -1, -1f, null, null, null, null, null, 1f, 1, null, false, null, 0f, 1, 1f, false, null, null, new FloatRange?(value), false, 0.6f, 0f, false, null, 1f);
			base.Apply(target, dest);
		}

        public override IEnumerable<PreCastAction> GetPreCastActions()
		{
			yield return new PreCastAction
			{
				action = delegate(LocalTargetInfo a, LocalTargetInfo b)
				{
					this.parent.AddEffecterToMaintain(HB_DefOf.HB_InfernalBreath.Spawn(this.parent.pawn.Position, a.Cell, this.parent.pawn.Map, 1f), Pawn.Position, a.Cell, 17, Pawn.MapHeld);
				},
				ticksAwayFromCast = 17
			};
			yield break;
		}

        public override void DrawEffectPreview(LocalTargetInfo target)
		{
			GenDraw.DrawFieldEdges(this.AffectedCells(target));
		}

        public override bool AICanTargetNow(LocalTargetInfo target)
        {
            if (Pawn.Faction != null)
			{
				foreach (IntVec3 c in this.AffectedCells(target))
				{
					List<Thing> thingList = c.GetThingList(Pawn.Map);
					for (int i = 0; i < thingList.Count; i++)
					{
						if (thingList[i].Faction == Pawn.Faction)
						{
							return false;
						}
					}
				}

                if(!this.HasEnoughPlasma)
                {
                    return false;
                }
				return true;
			}
			return true;
        }

        private List<IntVec3> AffectedCells(LocalTargetInfo target)
		{
			this.tmpCells.Clear();
			Vector3 b = Pawn.Position.ToVector3Shifted().Yto0();
			IntVec3 intVec = target.Cell.ClampInsideMap(Pawn.Map);
			if (Pawn.Position == intVec)
			{
				return this.tmpCells;
			}
			float lengthHorizontal = (intVec - Pawn.Position).LengthHorizontal;
			float num = (float)(intVec.x - Pawn.Position.x) / lengthHorizontal;
			float num2 = (float)(intVec.z - Pawn.Position.z) / lengthHorizontal;
			intVec.x = Mathf.RoundToInt((float)Pawn.Position.x + num * this.Props.range);
			intVec.z = Mathf.RoundToInt((float)Pawn.Position.z + num2 * this.Props.range);
			float target2 = Vector3.SignedAngle(intVec.ToVector3Shifted().Yto0() - b, Vector3.right, Vector3.up);
			float num3 = this.Props.lineWidthEnd / 2f;
			float num4 = Mathf.Sqrt(Mathf.Pow((intVec - Pawn.Position).LengthHorizontal, 2f) + Mathf.Pow(num3, 2f));
			float num5 = 57.29578f * Mathf.Asin(num3 / num4);
			int num6 = GenRadial.NumCellsInRadius(this.Props.range);
			for (int i = 0; i < num6; i++)
			{
				IntVec3 intVec2 = Pawn.Position + GenRadial.RadialPattern[i];
				if (this.CanUseCell(intVec2) && Mathf.Abs(Mathf.DeltaAngle(Vector3.SignedAngle(intVec2.ToVector3Shifted().Yto0() - b, Vector3.right, Vector3.up), target2)) <= num5)
				{
					this.tmpCells.Add(intVec2);
				}
			}
			List<IntVec3> list = GenSight.BresenhamCellsBetween(Pawn.Position, intVec);
			for (int j = 0; j < list.Count; j++)
			{
				IntVec3 intVec3 = list[j];
				if (!this.tmpCells.Contains(intVec3) && this.CanUseCell(intVec3))
				{
					this.tmpCells.Add(intVec3);
				}
			}
			return this.tmpCells;
		}

        private bool CanUseCell(IntVec3 c)
		{
			return c.InBounds(Pawn.Map) && !(c == Pawn.Position) && !c.Filled(Pawn.Map) && c.InHorDistOf(Pawn.Position, this.Props.range) && GenSight.LineOfSight(Pawn.Position, c, Pawn.Map, true, null, 0, 0);
		}
    }
}