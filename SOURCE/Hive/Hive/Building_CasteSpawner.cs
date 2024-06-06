using RimWorld;
using System.Text;
using UnityEngine;
using Verse;
using RimWorld.Planet;
using System.Reflection;
using System.Collections.Generic;

namespace Hive
{
    [StaticConstructorOnStartup]
    public class Building_CasteSpawner : Building_WorkTable
    {
        private CompRefuelable refuelableComp;

        private CompMote moteComp;

        private Mote workingMote;

        ThingDef moteDef = (ThingDef)null;


        public ThingOwner innerContainer;


        public bool CanWorkWithoutFuel => this.refuelableComp == null;

        private bool billDone = true;

        private float gestateProgress = 0f;

        public float daysToSpawn = 0.02f;

        public int maximum;

        private float fuelPerTick = 2f;

        private float failureChance = 0.05f;


        private Bill_Production activeBill;



        string letterLabelFail = "Spawn Failed";

        string LetterTextFail = "Something went wrong in the growing process, and resulting creature is malformed";


        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            base.SpawnSetup(map, respawningAfterLoad);
            this.refuelableComp = this.GetComp<CompRefuelable>();

            moteComp= this.GetComp<CompMote>();

            if(refuelableComp!= null )
            {
                fuelPerTick = refuelableComp.Props.fuelConsumptionRate / 60000f;
            }
            if(moteComp!= null )
            {
                moteDef = moteComp.Props.moteDef;
            }

        }

        public override void Tick()
        {
            base.Tick();

            if(!CanWorkWithoutFuel)
            {
                if (!refuelableComp.HasFuel) 
                {
                    if(gestateProgress > 0.8f)
                    {
                        gestateProgress = 0;
                        SpawnFail();
                    }

                    return; 
                }
            }

            if (billStack.Bills.Count == 0) { gestateProgress = 0; return; }

            if(activeBill != null)
            {
                if (activeBill.suspended) 
                {
                    GetActiveBill();
                    return; 
                }

            }


            if (billDone)
            {
                GetActiveBill();
            }
            else
            {

                if (moteDef != null)
                {
                    if (this.workingMote == null || this.workingMote.Destroyed || this.workingMote.def != moteDef)
                        this.workingMote = MoteMaker.MakeAttachedOverlay((Thing)this, moteDef, Vector3.zero);
                    this.workingMote.Maintain();
                }

                TryStartSpawn();
                return;
            }

        }

        void GetActiveBill()
        {
            foreach (Bill_Production bill in billStack.Bills)
            {
                if (bill.suspended) { continue; }   

                if (bill.repeatMode == BillRepeatModeDefOf.RepeatCount)
                {
                    if (bill.repeatCount > 0)
                    {
                        activeBill = bill;
                        break;
                    }
                }

                if (bill.repeatMode == BillRepeatModeDefOf.TargetCount)
                {
                    if(bill.recipe.WorkerCounter.CountProducts(bill) < bill.targetCount)
                    {
                        activeBill = bill;
                        break;
                    }

                }

                if (bill.repeatMode == BillRepeatModeDefOf.Forever)
                {
                    activeBill = bill;
                    break;
                }


                    continue;
            }

            if(activeBill == null) { return; }

            billDone = false;

            daysToSpawn = activeBill.recipe.workAmount;

        }

        public void TryStartSpawn()
        {
            this.gestateProgress += (float)(1.0 / (daysToSpawn * 60000)) * this.GetStatValue(StatDefOf.WorkTableWorkSpeedFactor) * (HiveSettings.PodGrowthScaleMultiplier/100);

            if (!CanWorkWithoutFuel)
            {
                float amount = fuelPerTick * this.GetStatValue(StatDefOf.WorkTableWorkSpeedFactor) / this.GetStatValue(StatDefOf.WorkTableEfficiencyFactor);

                refuelableComp.ConsumeFuel(amount);
            }

            if ((double)this.gestateProgress <= 1.0)
            {
                return;
            }

        //    for (int i = 0; i < activeBill.recipe.products.Count; i++)
        //    {
        //        Spawn(i);
        //    }

            Spawn(0);
        }

        public void Spawn(int index)
        {
            if(getActiveThing(index).category == ThingCategory.Item)
            {
                SpawnItem(index);
                return;
            }

            if(Random.Range(0f,1f) < failureChance)
            {
                SpawnFail();
                return;
            }

            PawnGenerationRequest request = new PawnGenerationRequest(getActivePawn(index), Faction.OfPlayer, developmentalStages: DevelopmentalStage.Newborn, allowDowned: true, tile: -1);

            Pawn pawn = PawnGenerator.GeneratePawn(request);

            if (pawn.RaceProps.Humanlike)
            {
                PawnGenerationRequest tmprequest = new PawnGenerationRequest(getActivePawn(index), developmentalStages: DevelopmentalStage.Adult, allowDowned: true, tile: -1);

                Pawn tmppawn = PawnGenerator.GeneratePawn(tmprequest);


                foreach (SkillRecord skill in pawn.skills.skills)
                {
                    SkillRecord tmpSkills = tmppawn.skills.GetSkill(skill.def);
                    skill.levelInt = tmpSkills.levelInt;
                    skill.passion = tmpSkills.passion;

                }

                pawn.story.Childhood = tmppawn.story.Childhood;
                pawn.story.Adulthood = tmppawn.story.Adulthood;

                tmppawn.Discard(true);


            }


            if (PawnUtility.TrySpawnHatchedOrBornPawn(pawn, (Thing)this, this.Position))
            {

                if (this.Spawned)

                    for (int i = 0; i < 15; i++)
                    {
                        FilthMaker.TryMakeFilth(this.Position, this.Map, ThingDefOf.Filth_AmnioticFluid);
                    }


            }
            else
                Find.WorldPawns.PassToWorld(pawn, PawnDiscardDecideMode.Discard);

            FinishBill();
        }


        public void SpawnFail()
        {
            PawnGenerationRequest request = new PawnGenerationRequest(PawnKindDef.Named("Hive_Malformed"), Faction.OfPlayer, developmentalStages: DevelopmentalStage.Newborn, allowDowned: true, tile: -1);

            Pawn pawn = PawnGenerator.GeneratePawn(request);


            if (PawnUtility.TrySpawnHatchedOrBornPawn(pawn, (Thing)this, this.Position))
            {

                if (this.Spawned)
                {
                    FilthMaker.TryMakeFilth(this.Position, this.Map, ThingDefOf.Filth_AmnioticFluid);

                    Find.LetterStack.ReceiveLetter((Letter)LetterMaker.MakeLetter((TaggedString)letterLabelFail, (TaggedString)LetterTextFail, LetterDefOf.NegativeEvent, (LookTargets)pawn));

                }

            }
            else
            Find.WorldPawns.PassToWorld(pawn, PawnDiscardDecideMode.Discard);

        //    DefaultWorkPriorities.SetPriorities(pawn, DefaultWorkPriorities.Type.Hiveling);

            FinishBill(false);
        }

        PawnKindDef getActivePawn(int index)
        {
            string pawnName = activeBill.recipe.products[index].thingDef.defName;

            return PawnKindDef.Named(pawnName);
        }

        ThingDef getActiveThing(int index)
        {
            return activeBill.recipe.products[index].thingDef;
        }

        public void EjectContents()
        {
            this.innerContainer.TryDropAll(this.InteractionCell, this.Map, ThingPlaceMode.Near);
        }


        public void FinishBill(bool clearBill = true)
        {
            if(clearBill)
            {
                if (activeBill.repeatMode == BillRepeatModeDefOf.RepeatCount)
                {
                    activeBill.repeatCount -= 1;
                    if (activeBill.repeatCount < 0)
                    {
                        activeBill.repeatCount = 0;
                    }
                }
            }

            gestateProgress = 0;
            activeBill = null;

            billDone = true;
        }


        public void SpawnItem(int index)
        {
            Thing thing;
            thing = ThingMaker.MakeThing(this.getActiveThing(index));
            thing.stackCount = activeBill.recipe.products[0].count;

            FinishBill();
        }

        public override string GetInspectString()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append(base.GetInspectString());
            if (stringBuilder.Length != 0)
                stringBuilder.AppendLine();


            stringBuilder.AppendLine((string)"Progress".Translate() + ": " + this.gestateProgress.ToStringPercent());

            //    stringBuilder.AppendLine((string)("Temperature".Translate() + ": " + this.AmbientTemperature.ToStringTemperature("F0")));
            //    stringBuilder.AppendLine((string)("IdealFermentingTemperature".Translate() + ": " + 7f.ToStringTemperature("F0") + " ~ " + comp.Props.maxSafeTemperature.ToStringTemperature("F0")));
            return stringBuilder.ToString().TrimEndNewlines();
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref gestateProgress, "gestateProgress");
        }
    }

}
