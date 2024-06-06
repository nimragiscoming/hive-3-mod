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
    public class DeathActionWorker_HiveMelt : DeathActionWorker
    {

        public override void PawnDied(Corpse corpse)
        {
            corpse.InnerPawn.ideo.SetIdeo(null);

            Thing thing;

            thing = ThingMaker.MakeThing(ThingDef.Named("HiveOrganicMatter"));
            thing.stackCount = 5;

            GenPlace.TryPlaceThing(thing, corpse.Position, corpse.Map, ThingPlaceMode.Near);

            FilthMaker.TryMakeFilth(corpse.PositionHeld, corpse.MapHeld, ThingDefOf.Filth_Slime, 5, FilthSourceFlags.None);

            corpse.DeSpawn();

        }
    }
}
