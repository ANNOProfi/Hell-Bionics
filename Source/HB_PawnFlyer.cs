using RimWorld;
using UnityEngine;
using Verse;

namespace HellBionics
{
    public class HB_PawnFlyer : PawnFlyer
    {
        public new Vector3 DestinationPos
        {
            get
            {
                Pawn flyingPawn = this.FlyingPawn;
				return GenThing.TrueCenter(base.Position, flyingPawn.Rotation, flyingPawn.def.size, 0.0001f);
            }
        }
    }
}