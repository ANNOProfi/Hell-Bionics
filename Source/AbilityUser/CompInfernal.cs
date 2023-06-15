using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;
using Verse.AI;
using AbilityUser;
using UnityEngine;

namespace HellBionics
{
    public class CompInfernal : CompAbilityUser
    {
        #region Variables

        private int infernalLegs = 0;
        private bool infernalJaw = false;
        private double plasmaLevel = 0.0;
        private double rechargeRate = 0.0;

        #endregion Variables

        #region Access Properties

        public bool IsInfernal //=> Pawn?.health?.hediffSet?.HasHediff(InfernalDefOf.InfernalLeg) ?? Pawn?.health?.hediffSet?.HasHediff(InfernalDefOf.InfernalJaw) ?? false;
        {
            get
            {
                if(Pawn.health.hediffSet.HasHediff(InfernalDefOf.InfernalLeg) || Pawn.health.hediffSet.HasHediff(InfernalDefOf.InfernalJaw))
                {
                    return true;
                }

                return false;
            }
        }

        public int InfernalLegs
        {
            get => infernalLegs;
            
            set => infernalLegs = value;
        }

        public bool InfernalJaw
        {
            get => infernalJaw;

            set => infernalJaw = value;
        }

        public double PlasmaLevel
        {
            get => plasmaLevel;

            set => plasmaLevel = value;
        }

        public double RechargeRate
        {
            get => rechargeRate;

            set => rechargeRate = value;
        }

        #endregion Access Properties

        #region Methods

        public void Notify_Infernal(CompInfernal infernalComp, BodyPartRecord part)
        {
            if(part.def.defName == "Leg")
            {
                Log.Message("InfernalLegs++ called");
                infernalComp.InfernalLegs++;
            }
            else if(part.def.defName == "Jaw")
            {
                infernalComp.InfernalJaw = true;
            }
            /*foreach(var ing in ingredients)
            {
                if(ing.)
                {
                    Log.Message("InfernalLegs++ called");
                    infernalComp.InfernalLegs++;
                }
                if(ing.Equals(InfernalDefOf.HB_InfernalJaw))
                {
                    infernalComp.InfernalJaw = true;
                }
            }*/
            Notify_UpdateHediffs();
            Notify_UpdateAbilities();
        }

        public void Notify_UpdateHediffs()
        {
            if(InfernalLegs > 2)
            {
                InfernalLegs = 2;
            }
            else if(InfernalLegs < 0)
            {
                InfernalLegs = 0;
            }
        }

        public void Notify_UpdateAbilities()
        {
            Log.Message("Notify_UpdateAbilities called");
            if(!Pawn.IsInfernal())
            {
                return;
            }
            else
            {
                if(this?.InfernalLegs == 1)
                {
                    AddPawnAbility(InfernalDefOf.InfernalDash, true);
                    Log.Message("InfernalDash called");
                }
                else if(this?.InfernalLegs == 2)
                {
                    InfernalDefOf.InfernalDash.Range = InfernalDefOf.InfernalDash.Range*2;
                }

                if(this?.InfernalJaw == true)
                {
                    //AddPawnAbility(InfernalDefOf.InfernalBreath);
                }
                
                Log.Message(AbilityUser.abilities.Named("InfernalDash").ToString());
            }
        }

        #endregion Methods

        #region Overrides

        public override bool TryTransformPawn() => (InfernalLegs > 0 || InfernalJaw == true);

        public override IEnumerable<Gizmo> CompGetGizmosExtra()
        {
            if(Find.Selector.NumSelected != 1)
            {
                yield break;
            }

            for(int i =0;i < AbilityData.AllPowers.Count; i++)
            {
                if(AbilityData.AllPowers[i] is InfernalAbility p && p.ShouldShowGizmo() && p.AbilityDef.MainVerb.hasStandardCommand /*&& p.AbilityDef.plasmaCost != 0*/)
                {
                    yield return p.GetGizmo();
                }
            }
        }
        
        private bool infernalTriedToBeInitialized = false;
        public override void CompTick()
        {
            if(!infernalTriedToBeInitialized)
            {
                if(!IsInfernal)
                {
                    if(InfernalLegs == 0 && Pawn.health.hediffSet.HasHediff(InfernalDefOf.InfernalLeg))
                    {

                    }
                    if(InfernalJaw == false && Pawn.health.hediffSet.HasHediff(InfernalDefOf.InfernalJaw))
                    {

                    }
                    Log.Message("infernalTriedToBeInitialized called");
                    infernalTriedToBeInitialized = true;
                    Notify_UpdateAbilities();
                }
            }
            base.CompTick();
            //Log.Message("infernalTriedToBeInitialized: "+ infernalTriedToBeInitialized);

            if(!IsInfernal)
            {
                return;
            }
        }

        #endregion Overrides
    }
}