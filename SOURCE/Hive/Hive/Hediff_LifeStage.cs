using RimWorld;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace Hive
{
    public class Hediff_LifeStage : HediffWithComps
    {
        private int lastStage;

        int Progress = 0;

        int ticksPerUpdate = 3000;

        bool CanUpdatePriorites = true;

        readonly int[] lifeStageAges = new int[]
        {
            0, 14, 18, 20
        };

        private BodyTypeDef cachedBodyType = BodyTypeDefOf.Thin;

        public override void PostAdd(DamageInfo? dinfo)
        {
            base.PostAdd(dinfo);

            cachedBodyType = pawn.story.bodyType;

            this.lastStage = -1;

            if(pawn.ageTracker.AgeBiologicalYears>1)
            {
                this.lastStage = 2;
                Severity = 0.8f;
            }

            SetBodyType();

        }

        public float LifeStageProgress
        {
            get => this.Severity;
            private set => this.Severity = value;
        }




        public override void Tick()
        {
            base.Tick();

            ++this.ageTicks;

            if (DefaultWorkPriorities.AutoSetPriorities && pawn.Faction.IsPlayer)
            {
                if (CanUpdatePriorites)
                {
                    CanUpdatePriorites = false;
                    UpdatePriorities();
                }

                if (!DefaultWorkPriorities.OnlyUpdateOnAgeUp)
                {
                    PeriodicPriorityUpdate();
                }
            }

            //Apparel

            if (this.CurStageIndex != this.lastStage)
            {
                //    this.UpdateLifeStageIndex(CurStageIndex);

                long ticks = (lifeStageAges[CurStageIndex] * 3600000L) + 10L;

                pawn.ageTracker.AgeBiologicalTicks = ticks;

                UpdateSkills();

                SetBodyType();

                CanUpdatePriorites= true;

                this.lastStage = this.CurStageIndex;

                
            }


            if ((double)this.Severity < 1.0)
                return;
        }

        public void SetBodyType()
        {
            if (pawn.story.bodyType != BodyTypeDefOf.Thin)
            {
                cachedBodyType = pawn.story.bodyType;
            }

            if (CurStageIndex == 0)
            {
                pawn.story.bodyType = BodyTypeDefOf.Thin;
            }
            else
            {
                pawn.story.bodyType = cachedBodyType;
            }
        }

        public void PeriodicPriorityUpdate()
        {
            Progress++;

            if(Progress >= ticksPerUpdate)
            {
                UpdatePriorities();
                Progress = 0;
            }
        }

        public void UpdatePriorities()
        {
            DefaultWorkPriorities.CalculateAndSetPriorities(pawn);

        }

        void UpdateSkills()
        {
            List<WorkTypeDef> tmpEnabledWorkTypes = new List<WorkTypeDef>();

            List<LifeStageWorkSettings> stageWorkSettings = pawn.RaceProps.lifeStageWorkSettings;
            for (int index = 0; index < stageWorkSettings.Count; ++index)
            {
                if (stageWorkSettings[index].minAge == pawn.ageTracker.AgeBiologicalYears)
                    tmpEnabledWorkTypes.Add(stageWorkSettings[index].workType);
            }
            if (tmpEnabledWorkTypes.Count > 0)
                this.pawn.Notify_DisabledWorkTypesChanged();

            //    pawn.workSettings.EnableAndInitialize();


        }
    }

}
