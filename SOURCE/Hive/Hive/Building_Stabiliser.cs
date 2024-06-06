using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Hive
{
    public class Building_Stabiliser : Building
    {


        private CompRefuelable refuelableComp;
        private bool CanWorkWithoutFuel => this.refuelableComp == null;



        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            base.SpawnSetup(map, respawningAfterLoad);
        }
    }
}
