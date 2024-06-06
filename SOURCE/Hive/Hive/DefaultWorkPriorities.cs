using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace Hive
{
    public class DefaultWorkPriorities: IExposable
    {
        public static bool AutoSetPriorities = true;

        public static List<int> MinLevelRequired = new List<int> { 0, 12, 9, 6, 3 };

        public static bool OnlyUpdateOnAgeUp;

        public void ExposeData()
        {
            Scribe_Collections.Look(ref MinLevelRequired, "MinLevelRequired", LookMode.Value);
        }

        public static void CalculateAndSetPriorities(Pawn pawn)
        {
            if (!AutoSetPriorities) { return; }

            //    foreach (WorkTypeDef w in DefDatabase<WorkTypeDef>.AllDefs.Where<WorkTypeDef>((Func<WorkTypeDef, bool>)(w => w.alwaysStartActive)))
            foreach (WorkTypeDef w in DefDatabase<WorkTypeDef>.AllDefs)
            {

                if (!pawn.WorkTypeIsDisabled(w))
                {
                    if(w == WorkTypeDefOf.Firefighter)
                    {
                        pawn.workSettings.SetPriority(w, 1);
                        continue;
                    }

                    if (w.defName == "PatientBedRest")
                    {
                        pawn.workSettings.SetPriority(w, 4);
                        continue;
                    }

                    int level =PriorityLevelCalc(avgReleventSkills(w,pawn));
                    pawn.workSettings.SetPriority(w, level);
                }


            }

            //    pawn.workSettings.EnableAndInitialize();
        }

        static int avgReleventSkills(WorkTypeDef w, Pawn pawn)
        {
            List<int> values = new List<int>();

            foreach (SkillDef skill in w.relevantSkills)
            {
                values.Add(pawn.skills.GetSkill(skill).Level);
            }

            if(values.Count == 0)
            {
                return 9;
            }

            int total = 0;

            foreach (int value in values)
            {
                total += value;
            }

            return total/values.Count;
        }

        static int PriorityLevelCalc(int avgSkills)
        {
            if(avgSkills >= MinLevelRequired[1])
            {
                return 1;
            }
            else if(avgSkills >= MinLevelRequired[2])
            {
                return 2;
            }
            else if (avgSkills >= MinLevelRequired[3])
            {
                return 3;
            }
            else if (avgSkills >= MinLevelRequired[4])
            {
                return 4;
            }
            else
            {
                return 0;
            }
        }

    }

}
