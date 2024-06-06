using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace Hive
{
    public class HediffGiver_Always : HediffGiver
    {
        public float minAge = -1;

        public override void OnIntervalPassed(Pawn pawn, Hediff cause)
        {
            if(pawn.ageTracker.AgeBiologicalYears < minAge) { return; }

            if (HasHediff(pawn, this.hediff) || !TryApply(pawn))
            {
                return;
            }

        //    this.SendLetter(pawn, cause);

        }

        bool HasHediff(Pawn pawn, HediffDef hediff)
        {
            HediffSet hediffSet = pawn.health.hediffSet;

            for (int i = 0; i < hediffSet.hediffs.Count; ++i)
            {
                if (hediffSet.hediffs[i].def == hediff)
                {
                    return true;
                }

            }

            return false;
        }
    }
}
