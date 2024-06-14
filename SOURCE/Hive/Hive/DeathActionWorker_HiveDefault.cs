using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using Verse.AI.Group;

namespace Hive
{
    public class DeathActionWorker_HiveDefault: DeathActionWorker
    {
        public override void PawnDied(Corpse corpse, Lord prevLord)
        {
            corpse.InnerPawn.ideo.SetIdeo(null);

            corpse.InnerPawn.Strip();
        }
    }
}
