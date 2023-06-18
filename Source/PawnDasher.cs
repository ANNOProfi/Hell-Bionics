using System;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace HellBionics
{
    public class HB_PawnDasher : PawnJumper
    {
        private Material cachedShadowMaterial;

        private static readonly Func<float, float> FlightSpeed;

        private static readonly Func<float, float> FlightCurveHeight = GenMath.InverseParabola;

        private int positionLastComputedTick = -1;

        private Vector3 groundPos;

        private Vector3 effectivePos;

        private float effectiveHeight;

        private Material ShadowMaterial
        {
            get
            {
                if (cachedShadowMaterial == null && !def.pawnFlyer.shadow.NullOrEmpty())
                {
                    cachedShadowMaterial = MaterialPool.MatFrom(def.pawnFlyer.shadow, ShaderDatabase.Transparent);
                }

                return cachedShadowMaterial;
            }
        }

        static HB_PawnDasher()
        {
            Log.Message("PawnJumper called");
            AnimationCurve animationCurve = new AnimationCurve();
            animationCurve.AddKey(0f, 0f);
            animationCurve.AddKey(0.1f, 0.15f);
            animationCurve.AddKey(1f, 1f);
            FlightSpeed = animationCurve.Evaluate;
        }

        protected override bool ValidateFlyer()
        {
            Log.Message("PawnJumper ValidateFlyer() called");
            return base.ValidateFlyer();
        }

        private void RecomputePosition()
        {
            Log.Message("PawnJumper RecomputePosition() called");
            if (positionLastComputedTick == ticksFlying)
            {
                return;
            }

            positionLastComputedTick = ticksFlying;
            float arg = ticksFlying / (float)ticksFlightTime;
            float num = FlightSpeed(arg);
            effectiveHeight = FlightCurveHeight(num);
            groundPos = Vector3.Lerp(startVec, DestinationPos, num);
            Vector3 a = new(0f, 0f, 2f);
            Vector3 b = Altitudes.AltIncVect * effectiveHeight;
            Vector3 b2 = a * effectiveHeight;
            effectivePos = groundPos + b + b2;
        }

        private void DrawShadow(Vector3 drawLoc, float height)
        {
            Material shadowMaterial = ShadowMaterial;
            if (shadowMaterial == null)
            {
                return;
            }

            float num = Mathf.Lerp(1f, 0.6f, height);
            Vector3 s = new(num, 1f, num);
            Matrix4x4 matrix = default;
            matrix.SetTRS(drawLoc, Quaternion.identity, s);
            Graphics.DrawMesh(MeshPool.plane10, matrix, shadowMaterial, 0);
        }

        private void LandingEffect()
        {
            if (soundLanding != null)
            {
                soundLanding.PlayOneShot(new TargetInfo(Position, Map));
            }

            FleckMaker.ThrowDustPuff(DestinationPos + Gen.RandomHorizontalVector(0.5f), Map, 2f);
        }
    }
}
