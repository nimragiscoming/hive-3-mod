using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Hive
{
    public class ThoughtWorker_QueenProximity : ThoughtWorker
    {
        protected override ThoughtState CurrentStateInternal(Pawn p)
        {
            if (!ModsConfig.BiotechActive || (p.health.hediffSet.GetFirstHediffOfDef(HediffDefOf.PsychicBond) is Hediff_PsychicBond firstHediffOfDef ? firstHediffOfDef.target : (Thing)null) == null)
                return ThoughtState.Inactive;
            return NearQueen(p) ? ThoughtState.ActiveAtStage(0) : ThoughtState.ActiveAtStage(1);
        }

        public static bool NearQueen(Pawn pawn)
        {
            List<Pawn> tmpColonists = Find.ColonistBar.GetColonistsInOrder();

            Pawn bondedPawn = null;

            foreach (Pawn tmpPawn in tmpColonists)
            {
                if(tmpPawn.def == ThingDef.Named("Hive_Queen"))
                {
                    if(tmpPawn.MapHeld == pawn.MapHeld && tmpPawn.Faction == pawn.Faction)
                    {
                        bondedPawn= tmpPawn;
                        break;
                    }
                }
            }


            if (bondedPawn == null)
                return false;
            bool flag1 = pawn.CarriedBy != null;
            bool flag2 = bondedPawn.CarriedBy != null;
            if (flag1 & flag2)
                return pawn.MapHeld == bondedPawn.MapHeld;
            bool flag3 = pawn.BrieflyDespawned();
            bool flag4 = bondedPawn.BrieflyDespawned();
            if (flag3 & flag4)
                return pawn.MapHeld == bondedPawn.MapHeld;
            if ((pawn.Spawned || bondedPawn.Spawned) && flag3 | flag4 | flag1 | flag2)
                return pawn.MapHeld == bondedPawn.MapHeld;
            IThingHolder parentHolder1 = pawn.ParentHolder;
            IThingHolder parentHolder2 = bondedPawn.ParentHolder;
            return parentHolder1 != null && parentHolder1 == parentHolder2 || (parentHolder1 == null || !ThingOwnerUtility.ContentsSuspended(parentHolder1)) && (parentHolder2 == null || !ThingOwnerUtility.ContentsSuspended(parentHolder2)) && QuestUtility.GetAllQuestPartsOfType<QuestPart_LendColonistsToFaction>().FirstOrDefault<QuestPart_LendColonistsToFaction>((Func<QuestPart_LendColonistsToFaction, bool>)(p => p.LentColonistsListForReading.Contains((Thing)pawn) && p.LentColonistsListForReading.Contains((Thing)bondedPawn))) != null;
        }
    }
}
