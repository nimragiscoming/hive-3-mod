using RimWorld;
using Verse;

namespace Hive
{
    public class CompSlimeTrail : ThingComp
    {

        private int Progress = 0;

        private int SlimeWait = 20;

        public CompProperties_SlimeTrail Props => (CompProperties_SlimeTrail)this.props;

        public override void CompTick()
        {
            ++Progress;
            if (Progress <= SlimeWait)
                return;
            FilthMaker.TryMakeFilth(this.parent.PositionHeld, this.parent.MapHeld, ThingDefOf.Filth_Slime, 1, FilthSourceFlags.None);
            Progress = 0;
        }

    }
}
