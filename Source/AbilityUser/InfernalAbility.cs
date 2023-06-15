using System.Text;
using AbilityUser;
using UnityEngine;
using Verse;

namespace HellBionics
{
    public class InfernalAbility : PawnAbility
    {
        public InfernalAbility()
        {
        }

        public InfernalAbility(CompAbilityUser abilityUser) : base(abilityUser)
        {
            this.AbilityUser = abilityUser as CompInfernal;
        }

        public InfernalAbility(Pawn user, AbilityDef pdef) : base(user, pdef)
        {
        }

        public InfernalAbility(AbilityData data) : base (data)
        {
        }

        //public CompInfernal => Pawn.InfernalComp();

        public InfernalAbilityDef AbilityDef => Def as InfernalAbilityDef;

        public CompInfernal Infernal => Pawn.InfernalComp();

        public override void PostAbilityAttempt()
        {
            base.PostAbilityAttempt();

            //double plasmaNeed = ;
        }

        public override bool ShouldShowGizmo()
        {
            if(Find.Selector.NumSelected == 1 && (this?.AbilityDef?.MainVerb?.hasStandardCommand ?? false) && (!this?.Pawn?.Downed ?? false) && (!this?.Pawn?.Dead ?? false))
            {
                return true;
            }

            return false;
        }
    }
}