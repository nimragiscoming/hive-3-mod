using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Hive
{
    public class CompProperties_Mote : CompProperties
    {
        public ThingDef moteDef;

        public CompProperties_Mote() => this.compClass = typeof(CompMote);
    }
}
