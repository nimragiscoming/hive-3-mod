using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Hive
{
    public class Hediff_NearQueen : HediffWithComps
    {
        public override bool Visible => CurStage.becomeVisible;


        public override void PostTick()
        {
            base.PostTick();
            if (!this.pawn.IsHashIntervalTick(65))
                return;
            Severity = ThoughtWorker_QueenProximity.NearQueen(pawn) ? 0.5f : 1.5f;
        }

    }
}
