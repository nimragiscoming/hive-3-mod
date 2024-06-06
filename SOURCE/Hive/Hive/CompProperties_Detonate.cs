using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hive
{
    public class CompProperties_Detonate : CompProperties_AbilityEffect
    {
        public float radius = 5;

        public CompProperties_Detonate() => this.compClass = typeof(CompAbilityEffect_Detonate);
    }
}
