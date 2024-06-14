using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace Hive
{
    public class CompHiveling : ThingComp
    {
        Pawn pawn => parent as Pawn;

        public override void PostPreApplyDamage(ref DamageInfo dinfo, out bool absorbed)
        {

            if (pawn.health.ShouldBeDead() || pawn.health.ShouldBeDeadFromLethalDamageThreshold())
            {
                absorbed = true;
                HiveTools.CleanKill(pawn, null, null, true);
            }

            base.PostPreApplyDamage(ref dinfo, out absorbed);
        }

        public CompProperties_Hiveling Props => (CompProperties_Hiveling)this.props;

    }
}
