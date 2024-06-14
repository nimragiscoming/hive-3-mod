using RimWorld;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using Verse;

namespace Hive
{
    public class Hediff_Stability : HediffWithComps
    {
        // this comp stores data about the pawn, nessacery for a pawnkind like this
        CompHiveling comp => pawn.TryGetComp<CompHiveling>();

        public float AvgSurvivalDays => comp.Props.AvgSurvivalDays;

        //stability of each part, stored as a large number sicne you cant save the decimals

        private Vector2 fleshStability = new Vector2(100, 100);

        private Vector2 boneStability = new Vector2(100, 100);

        private Vector2 mentalStability = new Vector2(100, 100);

        //random stability multiplier stored in y of vector 2

        public Vector2 FleshStability
        {
            get
            {
                return fleshStability/100;
            }
            set
            {
                fleshStability = value*100;
            }
        }

        public Vector2 BoneStability
        {
            get
            {
                return boneStability / 100;
            }
            set
            {
                boneStability = value * 100;
            }
        }

        public Vector2 MentalStability
        {
            get
            {
                return mentalStability / 100;
            }
            set
            {
                mentalStability = value * 100;
            }
        }


        //saving the stability values
        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look<Vector2>(ref fleshStability, "fleshStability");
            Scribe_Values.Look<Vector2>(ref boneStability, "boneStability");
            Scribe_Values.Look<Vector2>(ref mentalStability, "mentalStability");
        }


        // get the modifier offset for the given pawncapacity

        PawnCapacityModifier GetModifier(PawnCapacityDef DefOf, Vector2 StabilityType, float minStability, float maxValue)
        {
            PawnCapacityModifier modifier = new PawnCapacityModifier();

            modifier.capacity = DefOf;

            if(HiveSettings.UseSimpleStability)
            {
                modifier.offset = -Mathf.Clamp((1 - (Severity / minStability)) * maxValue, 0, 1);

                return modifier;
            }

            if(StabilityType.x > minStability)
            {
                modifier.offset = 0;
                return modifier;
            }

            modifier.offset = -Mathf.Clamp( (1-( StabilityType.x/minStability ))*maxValue,0,1);

            return modifier;
        }

        //average stability is essentially the same as severity
        public float AverageStability => (FleshStability.x + BoneStability.x + MentalStability.x) / 3;

        //the random change per tick of each stability type

        public float RandChange => (UnityEngine.Random.Range(0.8f, 1.2f) / (AvgSurvivalDays * 60000f) * (float)hashWaitDuration);

        //the random decay rate for each stability type, decided when first spawned

        public float PercentRand => (UnityEngine.Random.Range(0.5f, 1.5f));

        //the capacaties affected by each stability type

        List<PawnCapacityDef> Fcap = new List<PawnCapacityDef>
        {
            PawnCapacityDefOf.Talking,
            PawnCapacityDefOf.Hearing,
            //PawnCapacityDefOf.Eatingspeed, doesnt exist anymore, dunno what to replace it with, praying ensues
            PawnCapacityDefOf.Sight

        };
        List<PawnCapacityDef> Bcap = new List<PawnCapacityDef>
        {
            PawnCapacityDefOf.Moving,
            PawnCapacityDefOf.Manipulation

        };
        List<PawnCapacityDef> Mcap = new List<PawnCapacityDef>
        {
            PawnCapacityDefOf.Consciousness

        };

        bool usingSimpleStability = false;

        //counter for updating overall

        int hashWaitDuration = 200;

        List<PawnCapacityModifier> cachedCapMods = new List<PawnCapacityModifier>();


        // interpolates between these two colours as the pawn's health decays

        Color defaultLabelColour => this.def.defaultLabelColor;

        Color endLabelColour = new Color(1,0.2f, 0.2f);

        public override Color LabelColor => Color.Lerp(defaultLabelColour,endLabelColour,(1-AverageStability));

        public override string LabelInBrackets
        {
            get
            {

                string labelInBrackets = base.LabelInBrackets;
                string stringPercent = Severity.ToStringPercent("F0");
                return !labelInBrackets.NullOrEmpty() ? labelInBrackets + " " + stringPercent : stringPercent;
            }
        }

        public override string TipStringExtra
        {
            get
            {
                if (HiveSettings.AlwaysStable)
                {
                    StringBuilder stringBuilder = new StringBuilder();
                    stringBuilder.AppendLine((string)"Stability Never Decays enabled, Stability will never decay");
                    stringBuilder.AppendLine(base.TipStringExtra);
                    return stringBuilder.ToString().TrimEndNewlines();
                }

                if (HiveSettings.UseSimpleStability)
                {
                    StringBuilder stringBuilder = new StringBuilder();
                    stringBuilder.AppendLine((string)"Average days til death: " + Mathf.Round(AvgSurvivalDays * HiveSettings.LifeScaleMultiplier / 100));
                    stringBuilder.AppendLine(base.TipStringExtra);
                    return stringBuilder.ToString().TrimEndNewlines();
                }
                else
                {
                    StringBuilder stringBuilder = new StringBuilder();
                    stringBuilder.AppendLine((string)"Average days til death: " + Mathf.Round(AvgSurvivalDays * HiveSettings.LifeScaleMultiplier / 100) + Environment.NewLine);
                    stringBuilder.AppendLine((string)"Flesh Stability: " + FleshStability.x.ToStringPercent());
                    stringBuilder.AppendLine((string)"Bone Stability: " + BoneStability.x.ToStringPercent());
                    stringBuilder.AppendLine((string)"Mental Stability: " + MentalStability.x.ToStringPercent());
                    stringBuilder.AppendLine(base.TipStringExtra);
                    return stringBuilder.ToString().TrimEndNewlines();                    
                }


            }
        }

        public override void PostAdd(DamageInfo? dinfo)
        {
            base.PostAdd(dinfo);

            Log.Message("postAdd");

            FleshStability = new Vector2(1, PercentRand);

            BoneStability = new Vector2(1, PercentRand);

            MentalStability = new Vector2(1, PercentRand);

            Find.WorldPawns.AddPreservedPawnHediff(pawn, this);

            pawn.forceNoDeathNotification = true;

            if (pawn.ageTracker.AgeBiologicalYears > 14)
            {
                Severity = UnityEngine.Random.Range(0.4f,1f);

                usingSimpleStability = true;
            }
        }

        public override void Tick()
        {
            base.Tick();

            //    ++this.ageTicks;

            if (HiveSettings.AlwaysStable) { return; }

            CurStage.capMods = cachedCapMods;

            if (!this.pawn.IsHashIntervalTick(hashWaitDuration))
                return;

            //check stability type and use correct option

            if(HiveSettings.UseSimpleStability)
            {
                if(!usingSimpleStability)
                {
                    usingSimpleStability= true;
                }
                UpdateStabilitySimple();

                return;
            }

            if (usingSimpleStability)
            {
                FleshStability = new Vector2(Severity, FleshStability.y);

                BoneStability = new Vector2(Severity, BoneStability.y);

                MentalStability = new Vector2(Severity, MentalStability.y);

                usingSimpleStability= false;
            }
            UpdateStability();
            
            
        }

        void UpdateStabilitySimple()
        {
            Severity -= RandChange * (HiveSettings.LifeScaleMultiplier / 100);

            pawn.health.Notify_HediffChanged(this);

            cachedCapMods = ResolvePawnCapacity();

            if (Severity <= 0)
            {
                HiveTools.CleanKill(pawn, null, this, true);
            }
        }

        void UpdateStability()
        {

            // offset each stability part

            FleshStability -= new Vector2(RandChange * FleshStability.y * (100 / HiveSettings.LifeScaleMultiplier), 0);

            BoneStability -= new Vector2(RandChange * BoneStability.y * (100 / HiveSettings.LifeScaleMultiplier), 0);

            MentalStability -= new Vector2(RandChange * MentalStability.y * (100 / HiveSettings.LifeScaleMultiplier), 0);

            cachedCapMods = ResolvePawnCapacity();

            Severity = AverageStability;

            pawn.health.Notify_HediffChanged(this);

            if (FleshStability.x < 0 || BoneStability.x < 0 || MentalStability.x < 0)
            {
                HiveTools.CleanKill(pawn, null, this, true);
            }

        }

        //calculate and set each stability part

        List<PawnCapacityModifier> ResolvePawnCapacity()
        {

            List<PawnCapacityModifier> NewCapMods = new List<PawnCapacityModifier>();
         //   NewCapMods = new List<PawnCapacityModifier>();

            float minStability = 0.6f;

            float maxValue = 0.65f;

            foreach (PawnCapacityDef capDef in Fcap)
            {
                NewCapMods.Add(GetModifier(capDef, FleshStability, minStability, maxValue));

            }

            foreach (PawnCapacityDef capDef in Bcap)
            {
                NewCapMods.Add(GetModifier(capDef, BoneStability, minStability, maxValue));
            }

            foreach (PawnCapacityDef capDef in Mcap)
            {
                NewCapMods.Add(GetModifier(capDef, MentalStability, minStability, 0.5f));
            }

            return NewCapMods;
        }

    }
}
