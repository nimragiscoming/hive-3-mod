using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Hive
{
    public class CompMote : ThingComp
    {
        public CompProperties_Mote Props => (CompProperties_Mote)this.props;
    }
}
