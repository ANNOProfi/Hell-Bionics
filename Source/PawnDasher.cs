using System;
using Verse;
using RimWorld;
using UnityEngine;
using Verse.Sound;

namespace HellBionics
{
    [DefOf]
    public static class ThingDefOf
    {
        public static ThingDef HB_PawnDasher;
    }

    public class HB_PawnDasher : PawnFlyer
    {
        private Material cachedShadowMaterial;

        private static readonly Func<float, float> FlightSpeed;

        //private static readonly Func<float, float> FlightCurveHeight = new Func<float, float>(GenMath.InverseParabola);

        private Effecter flightEffecter;

        private int positionLastComputedTick = -1;

        private Vector3 groundPos;

        private Vector3 effectivePos;

        private float effectiveHeight;

        private Material ShadowMaterial
        {
            get
            {
                if(this.cachedShadowMaterial == null && !this.def.pawnFlyer.shadow.NullOrEmpty())
                {
                    this.cachedShadowMaterial = MaterialPool.MatFrom(this.def.pawnFlyer.shadow, ShaderDatabase.Transparent);
                }
                return this.cachedShadowMaterial;
            }
        }

        static HB_PawnDasher()
        {
            Log.Message("PawnJumper called");
            AnimationCurve animationCurve = new AnimationCurve();
            animationCurve.AddKey(0f, 0f);
            animationCurve.AddKey(0.1f, 0.15f);
            animationCurve.AddKey(1f, 1f);
            HB_PawnDasher.FlightSpeed = new Func<float, float>(animationCurve.Evaluate);
        }

        public override Vector3 DrawPos
        {
            get
            {
                this.RecomputePosition();
                return this.effectivePos;
            }
        }

        protected override bool ValidateFlyer()
        {
            return ModLister.CheckRoyaltyOrBiotech("Jumping");
        }

        private void RecomputePosition()
        {
            Log.Message("PawnJumper RecomputePosition() called");
            if(this.positionLastComputedTick == this.ticksFlying)
            {
                return;
            }
            this.positionLastComputedTick = this.ticksFlying;

            //float arg = 0f;
            //float num = 0f;
            this.effectiveHeight = 0f;
            this.groundPos = Vector3.Lerp(this.startVec, base.DestinationPos, 0f);
            Vector3 a = new Vector3(0f, 0f, 0f);
            Vector3 b = Altitudes.AltIncVect * this.effectiveHeight;
            Vector3 b2 = a * this.effectiveHeight;
            this.effectivePos = this.groundPos;
        }

        public override void DrawAt(Vector3 drawLoc, bool flip = false)
        {
            this.RecomputePosition();
            this.DrawShadow(this.groundPos, this.effectiveHeight);
            base.FlyingPawn.DrawAt(this.effectivePos, flip);
            if(base.CarriedThing != null)
            {
                PawnRenderer.DrawCarriedThing(base.FlyingPawn, this.effectivePos, base.CarriedThing);
            }
        }

        private void DrawShadow(Vector3 drawLoc, float height)
        {
            Material shadowMaterial = this.ShadowMaterial;
            if(shadowMaterial == null)
            {
                return;
            }
            float num = Mathf.Lerp(0f, 0f, height);
            Vector3 s = new Vector3(num, 0f, num);
            Matrix4x4 matrix = default(Matrix4x4);
            matrix.SetTRS(drawLoc, Quaternion.identity, s);
            Graphics.DrawMesh(MeshPool.plane10, matrix, shadowMaterial, 0);
        }

        protected override void RespawnPawn()
        {
            this.LandingEffect();
            base.RespawnPawn();
        }

        public override void Tick()
        {
            if(this.flightEffecter == null && this.flightEffecterDef != null)
            {
                this.flightEffecter = this.flightEffecterDef.Spawn();
                this.flightEffecter.Trigger(this, TargetInfo.Invalid, -1);
            }
            else
            {
                Effecter effecter = this.flightEffecter;
                if(effecter != null)
                {
                    effecter.EffectTick(this, TargetInfo.Invalid);
                }
            }
            base.Tick();
        }

        private void LandingEffect()
        {
            if(this.soundLanding != null)
            {
                this.soundLanding.PlayOneShot(new TargetInfo(base.Position, base.Map, false));
            }
            FleckMaker.ThrowDustPuff(base.DestinationPos + Gen.RandomHorizontalVector(0.5f), base.Map, 2f);
        }

        public override void Destroy(DestroyMode mode = DestroyMode.Vanish)
        {
            Effecter effecter = this.flightEffecter;
            if(effecter != null)
            {
                effecter.Cleanup();
            }
            base.Destroy(mode);
        }
    }
}