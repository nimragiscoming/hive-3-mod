using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace Hive
{
    public class CompEssenceSpawner : ThingComp
    {
        private float Progress;
        private int Count;

        public bool CanLayNow => (double)this.Progress >= 1.0;

        public CompProperties_EssenceSpawner Props => (CompProperties_EssenceSpawner)this.props;

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Values.Look<float>(ref this.Progress, "Progress");
            Scribe_Values.Look<int>(ref this.Count, "Count");
        }

        public override void CompTick()
        {
            float num = (float)(1.0 / ((double)this.Props.spawnIntervalDays * 60000.0));
            if (this.parent is Pawn parent)
                num *= PawnUtility.BodyResourceGrowthSpeed(parent);
            this.Progress += num;
            if ((double)this.Progress > 1.0)
            {
              this.Progress = 1f;
                SpawnEssence();
            }
  
        }

        public Thing SpawnEssence()
        {
            this.Progress = 0.0f;

            int randomInRange = this.Props.countRange.RandomInRange;
            if (randomInRange == 0)
                return (Thing)null;
            Thing thing;

            thing = ThingMaker.MakeThing(this.Props.essenceDef);
            this.Count = Mathf.Max(0, this.Count - randomInRange);

            thing.stackCount = randomInRange;

            GenPlace.TryPlaceThing(thing, this.parent.Position, this.parent.Map, ThingPlaceMode.Near);
            return thing;
        }

        public override string CompInspectStringExtra()
        {
            string str = (string)("Essence Production: " + this.Progress.ToStringPercent());

            return str;
        }
    }
}
