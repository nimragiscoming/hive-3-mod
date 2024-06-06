using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Hive
{
    public class Gene_Hivemind: Gene
    {
        public override void PostAdd()
        {
            base.PostAdd();
            pawn.health.AddHediff(HediffDef.Named("HiveNearQueen"));
        }

        public override void PostRemove()
        {
            base.PostRemove();
            pawn.health.RemoveHediff(pawn.health.hediffSet.GetFirstHediffOfDef(HediffDef.Named("HiveNearQueen")));
        }

    }
}
