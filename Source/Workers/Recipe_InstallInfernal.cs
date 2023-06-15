using System.Collections.Generic;
using RimWorld;
using Verse;

namespace HellBionics
{
    public class Recipe_InstallInfernal : Recipe_InstallArtificialBodyPart
    {
        public override void ApplyOnPawn(Pawn pawn, BodyPartRecord part, Pawn billDoer, List<Thing> ingredients, Bill bill)
        {
            base.ApplyOnPawn(pawn, part, billDoer, ingredients, bill);
            if(!base.CheckSurgeryFail(billDoer, pawn, ingredients, part, bill))
            {
                pawn.InfernalComp().Notify_Infernal(pawn.InfernalComp(), part);
            }
        }
    }
}