using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace Hive
{
    public class HiveSettings : ModSettings
    {
        /// <summary>
        /// The three settings our mod has.
        /// </summary>
        public static bool UseSimpleStability;

        public static float LifeScaleMultiplier = 100f;
        public static float PodGrowthScaleMultiplier = 100f;

        public static bool AlwaysStable = false;

        public static bool DontNotifyDeath = false;


        public static int[] MinLevelRequired = new int[] { 0, 12, 9, 6, 3 };


        /// <summary>
        /// The part that writes our settings to file. Note that saving is by ref.
        /// </summary>
        public override void ExposeData()
        {
            Scribe_Values.Look(ref UseSimpleStability, "UseSimpleStability");
            Scribe_Values.Look(ref LifeScaleMultiplier, "LifeScaleMultiplier", 100);
            Scribe_Values.Look(ref PodGrowthScaleMultiplier, "PodGrowthScaleMultiplier", 100);
            Scribe_Values.Look(ref AlwaysStable, "AlwaysStable");
            Scribe_Values.Look(ref DontNotifyDeath, "DontNotifyDeath");

            Scribe_Values.Look(ref MinLevelRequired, "MinLevelRequired");

            base.ExposeData();
        }
    }


    public class HiveMod : Mod
    {
        /// <summary>
        /// A reference to our settings.
        /// </summary>
        HiveSettings settings;

        /// <summary>
        /// A mandatory constructor which resolves the reference to our settings.
        /// </summary>
        /// <param name="content"></param>
        public HiveMod(ModContentPack content) : base(content)
        {
            this.settings = GetSettings<HiveSettings>();
        }

        /// <summary>
        /// The (optional) GUI part to set your settings.
        /// </summary>
        /// <param name="inRect">A Unity Rect with the size of the settings window.</param>
        public override void DoSettingsWindowContents(Rect inRect)
        {
            Listing_Standard listingStandard = new Listing_Standard();
            listingStandard.Begin(inRect);
            listingStandard.CheckboxLabeled("Use Simple Stability", ref HiveSettings.UseSimpleStability, "Use a simplified version of stability for hive creatures for better performance");
            listingStandard.CheckboxLabeled("Stability Never Decays", ref HiveSettings.AlwaysStable, "Stability will always be 100% and never decays.");
            listingStandard.CheckboxLabeled("Don't Notify When Pawn Dies", ref HiveSettings.DontNotifyDeath, "Don't send a message whenever a hive creature dies.");
            listingStandard.Label("Life Scale Multiplier (default = 100): " + Mathf.Round(HiveSettings.LifeScaleMultiplier) + "%");
            HiveSettings.LifeScaleMultiplier = listingStandard.Slider(HiveSettings.LifeScaleMultiplier, 10f, 500f);
            listingStandard.Label("Pod Growth Multiplier (default = 100): " + Mathf.Round(HiveSettings.PodGrowthScaleMultiplier) + "%");
            HiveSettings.PodGrowthScaleMultiplier = listingStandard.Slider(HiveSettings.PodGrowthScaleMultiplier, 10f, 500f);
            listingStandard.End();
            base.DoSettingsWindowContents(inRect);
        }

        /// <summary>
        /// Override SettingsCategory to show up in the list of settings.
        /// Using .Translate() is optional, but does allow for localisation.
        /// </summary>
        /// <returns>The (translated) mod name.</returns>
        public override string SettingsCategory()
        {
            return "Human Hive";
        }
    }
}
