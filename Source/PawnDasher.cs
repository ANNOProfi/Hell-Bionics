using System;
using Verse;
using RimWorld;
using UnityEngine;
using Verse.Sound;

namespace HellBionics
{

    public class HB_PawnDasher : PawnJumper
    {
        private Material cachedShadowMaterial;

        private static readonly Func<float, float> FlightSpeed;

        private static readonly Func<float, float> FlightCurveHeight = new Func<float, float>(GenMath.InverseParabola);

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

        protected override bool ValidateFlyer()
        {
            Log.Message("PawnJumper ValidateFlyer() called");
            return base.ValidateFlyer();
        }

        private void RecomputePosition()
        {
            Log.Message("PawnJumper RecomputePosition() called");
            if (this.positionLastComputedTick == this.ticksFlying)
			{
				return;
			}
			this.positionLastComputedTick = this.ticksFlying;
			float arg = (float)this.ticksFlying / (float)this.ticksFlightTime;
			float num = HB_PawnDasher.FlightSpeed(arg);
			this.effectiveHeight = HB_PawnDasher.FlightCurveHeight(num);
			this.groundPos = Vector3.Lerp(this.startVec, base.DestinationPos, num);
			Vector3 a = new Vector3(0f, 0f, 2f);
			Vector3 b = Altitudes.AltIncVect * this.effectiveHeight;
			Vector3 b2 = a * this.effectiveHeight;
			this.effectivePos = this.groundPos + b + b2;
        }

        private void DrawShadow(Vector3 drawLoc, float height)
		{
			Material shadowMaterial = this.ShadowMaterial;
			if (shadowMaterial == null)
			{
				return;
			}
			float num = Mathf.Lerp(1f, 0.6f, height);
			Vector3 s = new Vector3(num, 1f, num);
			Matrix4x4 matrix = default(Matrix4x4);
			matrix.SetTRS(drawLoc, Quaternion.identity, s);
			Graphics.DrawMesh(MeshPool.plane10, matrix, shadowMaterial, 0);
		}

        private void LandingEffect()
        {
            if(this.soundLanding != null)
            {
                this.soundLanding.PlayOneShot(new TargetInfo(base.Position, base.Map, false));
            }
            FleckMaker.ThrowDustPuff(base.DestinationPos + Gen.RandomHorizontalVector(0.5f), base.Map, 2f);
        }
    }
}