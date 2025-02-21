using System.Collections.Generic;
using Verse;

namespace HellBionics
{
    public class HediffComp_InfernalUtility : HediffComp
    {
        public HediffCompProperties_InfernalUtility Props
        {
            get
            {
                return (HediffCompProperties_InfernalUtility)this.props;
            }
        }

        public override IEnumerable<Gizmo> CompGetGizmos()
        {
            Gizmo plasmaStatus = new HB_Gizmo_PlasmaStatus(this);
            yield return plasmaStatus;
        }

        public int dashCount = 0;

        public float dashCost = 0;

        public bool dashInitialised = false;

        public float RemainingPlasma { get => remainingPlasma; set => remainingPlasma = value;}

        public float MaximumPlasma { get => maximumPlasma; set => maximumPlasma = value;}

        public float InfernalDashRange { get => infernalDashRange; set => infernalDashRange = value; }

        public float PlasmaPerTick { get => plasmaPerTick; set => plasmaPerTick = value; }

        private float maximumPlasma = 0f;

        private float remainingPlasma = 0f;

        private float plasmaPerTick = 0f;

        private float infernalDashRange = 10f;

        private int ticksToOffset = 30;

        public void UpdateValues(float maximum, float plasmaPerTick)
        {
            MaximumPlasma += maximum;
            PlasmaPerTick += plasmaPerTick;

            if(RemainingPlasma > MaximumPlasma)
            {
                RemainingPlasma = MaximumPlasma;
            }
        }

        public void OffsetPlasma(float amountPerTick)
        {
            if((RemainingPlasma + amountPerTick) <= 0f)
            {
                RemainingPlasma = 0f;
            }
            else if((RemainingPlasma + amountPerTick) >= MaximumPlasma)
            {
                RemainingPlasma = MaximumPlasma;
            }
            else if((RemainingPlasma + amountPerTick) <= MaximumPlasma)
            {
                RemainingPlasma += amountPerTick;
            }
        }

        public override void CompPostTick(ref float severityAdjustment)
        {
            ticksToOffset--;

            if(ticksToOffset <= 0)
            {
                if(RemainingPlasma < MaximumPlasma || PlasmaPerTick < 0f && RemainingPlasma == MaximumPlasma)
                {
                    OffsetPlasma(PlasmaPerTick*30);
                    ticksToOffset = 30;
                }
            }
        }

        public override void CompExposeData()
        {
            Scribe_Values.Look(ref remainingPlasma, "remainingPlasma");
            Scribe_Values.Look(ref maximumPlasma, "maximumPlasma");
            Scribe_Values.Look(ref plasmaPerTick, "plasmaPerTick");
            Scribe_Values.Look(ref infernalDashRange, "infernalDashRange");
            Scribe_Values.Look(ref ticksToOffset, "ticksToOffset");
        }
    }
}