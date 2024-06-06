using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Hive
{
    public class CompAbilityEffect_Detonate : CompAbilityEffect
    {

        private CompProperties_Detonate Props => (CompProperties_Detonate)this.props;

        private Pawn pawn => this.parent.pawn;

        public override void Apply(LocalTargetInfo target, LocalTargetInfo dest)
        {

            float radius = pawn.ageTracker.CurLifeStageIndex != 0 ? (pawn.ageTracker.CurLifeStageIndex != 1 ? Props.radius : Props.radius/2) : Props.radius/4;
            GenExplosion.DoExplosion(pawn.Position, pawn.Map, radius, DamageDefOf.Flame, (Thing)pawn);

            HiveTools.CleanKill(pawn, null, null, true);

            base.Apply(target, dest);
        }
    }
}