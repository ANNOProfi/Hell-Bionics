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

        public float RemainingPlasma { get => remainingPlasma; set => remainingPlasma = value;}

        public float MaximumPlasma { get => maximumPlasma; set => maximumPlasma = value;}

        public float InfernalDashRange { get => infernalDashRange; set => infernalDashRange = value; }

        private float maximumPlasma = 0f;

        private float remainingPlasma = 0f;

        private float infernalDashRange = 10f;

        public void UpdateValues(float maximum)
        {
            MaximumPlasma += maximum;
            if(RemainingPlasma > MaximumPlasma)
            {
                RemainingPlasma = MaximumPlasma;
            }
        }

        public void OffsetPlasma(float amountPerTick)
        {
            if((RemainingPlasma + amountPerTick) <= MaximumPlasma)
            {
                RemainingPlasma += amountPerTick;
            }
            else if((RemainingPlasma + amountPerTick) >= MaximumPlasma)
            {
                RemainingPlasma = MaximumPlasma;
            }
        }
    }
}