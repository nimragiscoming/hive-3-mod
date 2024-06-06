using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Hive
{
    public class CompProperties_Hiveling : CompProperties
    {
        public bool highCaste = false;

        public float AvgSurvivalDays = 5;

        public CompProperties_Hiveling() => this.compClass = typeof(CompHiveling);
    }
}
