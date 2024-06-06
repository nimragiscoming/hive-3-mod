using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Hive
{
    public static class HiveTools
    {
        public static void CleanKill(Pawn pawn, DamageInfo? dInfo, Hediff exactCulprit = null ,bool sendDeathMessage = false)
        {
            if(sendDeathMessage && !HiveSettings.DontNotifyDeath)
            {
                pawn.forceNoDeathNotification = false;
            }

            Ideo pawnIdeo = pawn.Ideo;
            pawn.ideo.SetIdeo(null);
            pawn.Kill(dInfo, exactCulprit);
        //    pawn.ideo.SetIdeo(pawnIdeo);
        }

    }
}
