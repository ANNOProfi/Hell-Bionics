using Verse;
using Verse.AI;
using RimWorld;

namespace HellBionics
{
    public static class JumpUtility
    {
        public static bool DoJump(Pawn pawn, LocalTargetInfo currentTarget, CompReloadable comp, VerbProperties verbProps)
		{
			if (!ModLister.CheckRoyaltyOrBiotech("Jumping"))
			{
				return false;
			}
			if (comp != null && !comp.CanBeUsed)
			{
				return false;
			}
			if (comp != null)
			{
				comp.UsedOnce();
			}
			IntVec3 position = pawn.Position;
			IntVec3 cell = currentTarget.Cell;
			Map map = pawn.Map;
			bool flag = Find.Selector.IsSelected(pawn);
			PawnFlyer pawnFlyer = PawnFlyer.MakeFlyer(HB_DefOf.HB_PawnDasher, pawn, cell, verbProps.flightEffecterDef, verbProps.soundLanding, verbProps.flyWithCarriedThing);
			if (pawnFlyer != null)
			{
				FleckMaker.ThrowDustPuff(position.ToVector3Shifted() + Gen.RandomHorizontalVector(0.1f), map, 2f);
				GenSpawn.Spawn(pawnFlyer, cell, map, WipeMode.Vanish);
				if (flag)
				{
					Find.Selector.Select(pawn, false, false);
				}
				return true;
			}
			return false;
		}

        public static void OrderJump(Pawn pawn, LocalTargetInfo target, Verb verb, float range)
		{
			Map map = pawn.Map;
			IntVec3 intVec = RCellFinder.BestOrderedGotoDestNear(target.Cell, pawn, (IntVec3 c) => JumpUtility.ValidJumpTarget(map, c) && JumpUtility.CanHitTargetFrom(pawn, pawn.Position, c, range));
			Job job = JobMaker.MakeJob(JobDefOf.CastJump, intVec);
			job.verbToUse = verb;
			if (pawn.jobs.TryTakeOrderedJob(job, new JobTag?(JobTag.Misc), false))
			{
				FleckMaker.Static(intVec, map, FleckDefOf.FeedbackGoto, 1f);
			}
		}

        public static bool CanHitTargetFrom(Pawn pawn, IntVec3 root, LocalTargetInfo targ, float range)
		{
			float num = range * range;
			IntVec3 cell = targ.Cell;
			return (float)pawn.Position.DistanceToSquared(cell) <= num && GenSight.LineOfSight(root, cell, pawn.Map, false, null, 0, 0);
		}

        public static bool ValidJumpTarget(Map map, IntVec3 cell)
		{
			if (!cell.IsValid || !cell.InBounds(map))
			{
				return false;
			}
			if (cell.Impassable(map) || !cell.Walkable(map) || cell.Fogged(map))
			{
				return false;
			}
			Building edifice = cell.GetEdifice(map);
			Building_Door building_Door;
			return edifice == null || (building_Door = (edifice as Building_Door)) == null || building_Door.Open;
		}
    }
}