using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Hive
{
    public class CompProperties_EssenceSpawner : CompProperties
    {

        public float spawnIntervalDays = 1f;
        public IntRange countRange = IntRange.one;
        public ThingDef essenceDef;

        public CompProperties_EssenceSpawner() => this.compClass = typeof(CompEssenceSpawner);
    }
}
