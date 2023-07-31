using System;
using Verse;
using RimWorld;
using UnityEngine;
using Verse.Sound;

namespace HellBionics
{

    public class HB_PawnDasher : HB_PawnFlyer
    {
        private Material cachedShadowMaterial;

        private static readonly Func<float, float> FlightSpeed;

        private static readonly Func<float, float> FlightCurveHeight = new Func<float, float>(GenMath.InverseParabola);

        private Effecter flightEffecter;

        private int positionLastComputedTick = -1;

        private Vector3 groundPos;

        private Vector3 effectivePos;

        private float effectiveHeight;

        private Vector3 lastPos;

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
            AnimationCurve animationCurve = new AnimationCurve();
            animationCurve.AddKey(0f, 0f);
            //animationCurve.AddKey(0.1f, 0.15f);
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
            return base.ValidateFlyer();
        }

        private void RecomputePosition()
        {
            if (this.positionLastComputedTick == this.ticksFlying)
			{
				return;
			}
			if(Vector3.Distance(this.lastPos, this.effectivePos) > 1)
            {
                this.lastPos = this.effectivePos;
            }
			this.positionLastComputedTick = this.ticksFlying;
			float arg = (float)this.ticksFlying / (float)this.ticksFlightTime;
			float num = HB_PawnDasher.FlightSpeed(arg);
			this.effectiveHeight = HB_PawnDasher.FlightCurveHeight(0);
			this.groundPos = Vector3.Lerp(this.startVec, base.DestinationPos, num);
			Vector3 a = new Vector3(0f, 0f, 2f);
			Vector3 b = Altitudes.AltIncVect * this.effectiveHeight;
			Vector3 b2 = a * this.effectiveHeight;
			this.effectivePos = this.groundPos + b + b2;
        }

        public override void DrawAt(Vector3 drawLoc, bool flip = false)
		{
			this.RecomputePosition();
			this.DrawShadow(this.groundPos, this.effectiveHeight);
			base.FlyingPawn.DrawAt(this.effectivePos, flip);
			if (base.CarriedThing != null)
			{
				PawnRenderer.DrawCarriedThing(base.FlyingPawn, this.effectivePos, base.CarriedThing);
			}
		}

        private void DrawShadow(Vector3 drawLoc, float height)
		{
			Material shadowMaterial = this.ShadowMaterial;
			if (shadowMaterial == null)
			{
				return;
			}
			float num = Mathf.Lerp(1f, 0.6f, 0);
			Vector3 s = new Vector3(num, 1f, 0);
			Matrix4x4 matrix = default(Matrix4x4);
			matrix.SetTRS(drawLoc, Quaternion.identity, s);
			Graphics.DrawMesh(MeshPool.plane10, matrix, shadowMaterial, 0);
		}

        protected override void RespawnPawn()
		{
			this.LandingEffects();
			base.RespawnPawn();
		}

        public override void Tick()
		{
			this.RecomputePosition();
			if (this.flightEffecter == null && this.flightEffecterDef != null)
			{
				this.flightEffecter = this.flightEffecterDef.Spawn();
				this.flightEffecter.Trigger(this, TargetInfo.Invalid, -1);
			}
			else
			{
				Effecter effecter = this.flightEffecter;
				if (effecter != null)
				{
					effecter.EffectTick(this, TargetInfo.Invalid);
				}
			}

			Fire fire = (Fire)ThingMaker.MakeThing(ThingDefOf.Fire, null);
			fire.fireSize = 0.6f;

			if(this.lastPos.ToIntVec3() != this.PositionHeld)
			{
				GenSpawn.Spawn(fire, this.lastPos.ToIntVec3(), base.Map, Rot4.North, WipeMode.Vanish, false);
			}
            
			base.Tick();
		}

        private void LandingEffects()
        {
            if(this.soundLanding != null)
            {
                this.soundLanding.PlayOneShot(new TargetInfo(base.Position, base.Map, false));
            }
            Vector3 max = new Vector3(0.5f, 0.5f, 0f);
            FleckMaker.ThrowDustPuff(base.DestinationPos + Gen.Random2DVector(max), base.Map, 2f);
        }

        public override void Destroy(DestroyMode mode = DestroyMode.Vanish)
		{
			Effecter effecter = this.flightEffecter;
			if (effecter != null)
			{
				effecter.Cleanup();
			}
			base.Destroy(mode);
		}
    }
}