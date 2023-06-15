using RimWorld;
using UnityEngine;
using Verse;

namespace HellBionics
{
    public class FlyingObject : ThingWithComps
    {
        private int verVal = 0;
        private int pwrVal = 0;

        protected Vector3 origin;
        private Vector3 trueOrigin;
        protected Vector3 destination;
        private Vector3 trueDestination;
        private Vector3 direction;
        private float trueAngle;

        private int dashStep = 0;
        private bool earlyImpact = false;
        private int drawTicks = 300;
        private bool shouldDrawPawn = true;

        protected float speed = 15f;

        protected int ticksToImpact = 60;

        protected Thing assignedTarget;
        protected Thing flyingThing;
        public DamageInfo? impactDamage;
        
        public bool damageLaunched = true;
        public bool explosion = false;
        public int weaponDmg = 0;
        private bool drafted = false;
        Pawn pawn;

        private float explosiveMagnitude = 1f;

        protected int StartingTicksToImpact
        {
            get
            {
                int num = Mathf.RoundToInt((this.origin - this.destination).magnitude / (this.speed / 100f));
                bool flag = num < 1;
                if (flag)
                {
                    num = 1;
                }
                return num;
            }
        }

        protected IntVec3 DestinationCell
        {
            get
            {
                return new IntVec3(this.destination);
            }
        }

        public Vector3 ExactPosition
        {
            get
            {
                Vector3 b = (this.destination - this.origin) * (1f - (float)this.ticksToImpact / (float)this.StartingTicksToImpact);
                return this.origin + b + Vector3.up * this.def.Altitude;
            }
        }

        public Quaternion ExactRotation
        {
            get
            {
                return Quaternion.LookRotation(this.destination - this.origin);
            }
        }

        public override Vector3 DrawPos
        {
            get
            {
                return this.ExactPosition;
            }
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look<Vector3>(ref this.origin, "origin", default(Vector3), false);
            Scribe_Values.Look<Vector3>(ref this.destination, "destination", default(Vector3), false);
            Scribe_Values.Look<Vector3>(ref this.trueOrigin, "trueOrigin", default(Vector3), false);
            Scribe_Values.Look<Vector3>(ref this.trueDestination, "trueDestination", default(Vector3), false);            
            Scribe_Values.Look<int>(ref this.ticksToImpact, "ticksToImpact", 0, false);
            Scribe_Values.Look<int>(ref this.weaponDmg, "weaponDmg", 0, false);
            Scribe_Values.Look<int>(ref this.dashStep, "dashStep", 0, false);
            Scribe_Values.Look<float>(ref this.trueAngle, "trueAngle", 0, false);
            Scribe_Values.Look<bool>(ref this.damageLaunched, "damageLaunched", true, false);
            Scribe_Values.Look<bool>(ref this.explosion, "explosion", false, false);
            Scribe_References.Look<Thing>(ref this.assignedTarget, "assignedTarget", false);
            Scribe_References.Look<Pawn>(ref this.pawn, "pawn", false);
            Scribe_Deep.Look<Thing>(ref this.flyingThing, "flyingThing", new object[0]);
        }

        private void Initialize()
        {
            if(pawn != null)
            {
                FleckMaker.Static(this.origin, this.Map, FleckDefOf.ExplosionFlash, 12f);
                SoundDefOf.Ambient_AltitudeWind.sustainFadeoutTime.Equals(30.0f);
                FleckMaker.ThrowDustPuff(this.origin, this.Map, Rand.Range(1.2f, 1.8f));
                verVal = 3;
                pwrVal = 3;
            }
        }

        public void Launch(Thing launcher, LocalTargetInfo targ, Thing flyingThing, DamageInfo? impactDamage)
        {
            this.Launch(launcher, base.Position.ToVector3Shifted(), targ, flyingThing, impactDamage);
        }

        public void Launch(Thing launcher, LocalTargetInfo targ, Thing flyingThing)
        {
            this.Launch(launcher, base.Position.ToVector3Shifted(), targ, flyingThing);
        }

        public void Launch(Thing launcher, Vector3 origin, LocalTargetInfo targ, Thing flyingThing, DamageInfo? newDamageInfo = null)
        {
            bool spawned = flyingThing.Spawned;
            pawn = launcher as Pawn;
            if (pawn.Drafted)
            {
                this.drafted = true;
            }
            if (spawned)
            {
                flyingThing.DeSpawn();
            }
            //
            //ModOptions.Constants.SetPawnInFlight(true);
            //
            this.origin = origin;
            this.trueOrigin = origin;
            this.impactDamage = newDamageInfo;
            this.flyingThing = flyingThing;
            bool flag = targ.Thing != null;
            if (flag)
            {
                this.assignedTarget = targ.Thing;
            }
            this.speed = 15;
            this.trueDestination = targ.Cell.ToVector3Shifted();            
            this.direction = GetVector(this.trueOrigin.ToIntVec3(), targ.Cell);
            this.trueAngle = (Quaternion.AngleAxis(90, Vector3.up) * this.direction).ToAngleFlat();
            this.destination = this.origin + (this.direction * 3f);         
            this.ticksToImpact = this.StartingTicksToImpact;
            this.Initialize();
        }

        public Vector3 GetVector(IntVec3 center, IntVec3 objectPos)
        {
            Vector3 heading = (objectPos - center).ToVector3();
            float distance = heading.magnitude;
            Vector3 direction = heading / distance;
            return direction;
        }

        public override void Tick()
        {
            this.drawTicks--;
            if(this.drawTicks <= 0)
            {
                this.shouldDrawPawn = false;
            }
            Vector3 exactPosition = this.ExactPosition;
            this.ticksToImpact--;

            if(!this.ExactPosition.InBoundsWithNullCheck(base.Map) || !this.ExactPosition.ToIntVec3().Walkable(base.Map) || this.ExactPosition.ToIntVec3().DistanceToEdge(base.Map) <= 1)
            {
                this.earlyImpact = true;
                this.ImpactSomething();
            }
            else
            {
                base.Position = this.ExactPosition.ToIntVec3();
                FleckMaker.ThrowDustPuff(base.Position, base.Map, Rand.Range(0.8f, 1.2f));
                ApplyDashDamage();

                if(this.ticksToImpact <=0)
                {
                    if(this.DestinationCell.InBoundsWithNullCheck(base.Map))
                    {
                        base.Position = this.DestinationCell;
                    }

                    this.ImpactSomething();
                }
            }
        }

        private void ImpactSomething()
        {
            if(this.dashStep == 0 && !this.earlyImpact)
            {
                this.speed = 30;
                this.origin = this.ExactPosition;
                this.destination = this.origin + (this.direction*2f);
                this.ticksToImpact = this.StartingTicksToImpact;
                this.dashStep = 1;
            }
            else if (this.dashStep == 1 && !this.earlyImpact)
            {
                ExplosiveStep(this.explosiveMagnitude);
                this.dashStep = 2;
            }
            else if (this.dashStep == 2 && !this.earlyImpact)
            {
                ExplosiveStep(this.explosiveMagnitude);
                this.dashStep = 3;
            }
            else if (this.dashStep == 3 && !this.earlyImpact)
            {
                ExplosiveStep(this.explosiveMagnitude);
                this.dashStep = 4;
            }
            else if (this.dashStep == 4 && !this.earlyImpact)
            {
                ExplosiveStep(this.explosiveMagnitude);
                this.dashStep = 5;
            }
            else if (this.dashStep == 5 && !this.earlyImpact)
            {
                ExplosiveStep(this.explosiveMagnitude);
                if(this.verVal > 0)
                {
                    this.dashStep = 6;
                }
                else
                {
                    this.dashStep = 20;
                }
            }
            else if (this.dashStep == 6 && !this.earlyImpact)
            {
                ExplosiveStep(this.explosiveMagnitude);
                this.dashStep = 7;
            }
            else if (this.dashStep == 7 && !this.earlyImpact)
            {
                ExplosiveStep(this.explosiveMagnitude);
                if(this.verVal > 1)
                {
                    this.dashStep = 8;
                }
                else
                {
                    this.dashStep = 21;
                }
            }
            else if (this.dashStep == 8 && !this.earlyImpact)
            {
                ExplosiveStep(this.explosiveMagnitude);
                this.dashStep = 9;
            }
            else if (this.dashStep == 9 && !this.earlyImpact)
            {
                ExplosiveStep(this.explosiveMagnitude);
                if(this.verVal > 2)
                {
                    this.dashStep = 10;
                }
                else
                {
                    this.dashStep = 20;
                }
            }
            else if (this.dashStep == 10 && !this.earlyImpact)
            {
                ExplosiveStep(this.explosiveMagnitude);
                this.dashStep = 11;
            }
            else if (this.dashStep == 11 && !this.earlyImpact)
            {
                ExplosiveStep(this.explosiveMagnitude);
                this.dashStep = 21;
            }
            else if (this.dashStep == 20 && !this.earlyImpact)
            {
                ExplosiveStep(this.explosiveMagnitude);
                this.dashStep = 22;
            }
            else if (this.dashStep == 21 && !this.earlyImpact)
            {
                ExplosiveStep(this.explosiveMagnitude);
                this.dashStep = 22;
            }
            else if (this.dashStep == 22 && !this.earlyImpact)
            {
                ExplosiveStep(this.explosiveMagnitude);
                this.dashStep = 50;
            }
            else if (this.dashStep == 50 && !this.earlyImpact)
            {
                ExplosiveStepFinal(this.explosiveMagnitude);
                if(this.assignedTarget != null)
                {
                    Pawn pawn = this.assignedTarget as Pawn;
                    if(pawn != null && pawn.GetPosture() != PawnPosture.Standing && (this.origin - this.destination).MagnitudeHorizontalSquared() >= 20.25f &&Rand.Value > 0.2f)
                    {
                        this.Impact(null);
                    }
                    else
                    {
                        this.Impact(this.assignedTarget);
                    }
                }
                else
                {
                    this.Impact(null);
                }
            }

        }

        private void ExplosiveStep(float forwardMagnitude)
        {
            this.drawTicks = 120;
            this.speed =40;
            GenExplosion.DoExplosion(base.Position, this.Map, 1.7f, DamageDefOf.Burn, this.pawn, Mathf.RoundToInt(Rand.Range(8, 14)*(1+ .1f * pwrVal)), 0, this.def.projectile.soundExplode, def, null, null, null, 0f, 1, null, false, null, 0f, 1, 0.0f, false);
            this.origin = this.ExactPosition;
            this.destination = this.origin + (this.direction * forwardMagnitude);
            this.ticksToImpact = this.StartingTicksToImpact;
        }

        private void ExplosiveStepFinal(float forwardMagnitude)
        {
            this.drawTicks = 60;
            this.speed = 20;
            GenExplosion.DoExplosion(base.Position, this.Map, 1.7f, DamageDefOf.Burn, this.pawn, Mathf.RoundToInt(Rand.Range(10, 16)*(1+ .1f * pwrVal)), 0, this.def.projectile.soundExplode, def, null, null, null, 0f, 1, null, false, null, 0f, 1, 0.0f, false);
            this.origin = this.ExactPosition;
            this.destination = this.origin + (this.direction * forwardMagnitude);
            this.ticksToImpact = this.StartingTicksToImpact;
        }

        protected void Impact(Thing hitThing)
        {
            if(hitThing == null)
            {
                Pawn pawn;
                if((pawn = (base.Position.GetThingList(base.Map).FirstOrDefault((Thing x) => x == this.assignedTarget) as Pawn)) != null)
                {
                    hitThing = pawn;
                }
            }

            bool hasValue = this.impactDamage.HasValue;
            if(hasValue)
            {
                if(this.damageLaunched)
                {
                    hitThing.TakeDamage(this.impactDamage.Value);
                }

                if(this.explosion)
                {
                    GenExplosion.DoExplosion(base.Position, base.Map, 0.9f, DamageDefOf.Stun, this, -1, 0, null, null, null, null, null, 0f, 1, null, false, null, 0f, 1, 0f, false);
                }
            }
            GenSpawn.Spawn(this.flyingThing, base.Position, base.Map);
            //ModOptions.Constants.SetPawnInFlight(false);
            Pawn p = this.flyingThing as Pawn;
            if(p.IsColonist && this.drafted)
            {
                p.drafter.Drafted = true;

            }
            this.Destroy(DestroyMode.Vanish);
        }

        public void ApplyDashDamage()
        {
            DamageInfo dinfo = new DamageInfo(DamageDefOf.Burn, Rand.Range(6, 10)*(1 + .1f * pwrVal), 0, (float)-1, pawn, null, null, DamageInfo.SourceCategory.ThingOrUnknown);

            if(base.Position != default(IntVec3))
            {
                for(int i = 0; i<8; i++)
                {
                    IntVec3 intVec = base.Position + GenAdj.AdjacentCells[i];

                    Pawn cleaveVictim = new Pawn();
                    cleaveVictim = intVec.GetFirstPawn(base.Map);
                    if(cleaveVictim != null && cleaveVictim.Faction != pawn.Faction)
                    {
                        cleaveVictim.TakeDamage(dinfo);
                        FleckMaker.ThrowMicroSparks(cleaveVictim.Position.ToVector3(), base.Map);
                    }
                }
            }
        }

        public override void Draw()
        {
            base.Draw();
        }
    }
}