using RimWorld;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace Hive
{
    [StaticConstructorOnStartup]
    public class MainTabWindow_Hive : MainTabWindow
    {

        private bool JustClicked = false;

        public override void DoWindowContents(Rect rect)
        {
            this.DoAutoPrioritiesCheckbox();

        }

        private void DoAutoPrioritiesCheckbox()
        {
            Text.Font = GameFont.Small;
            GUI.color = Color.white;
            Text.Anchor = TextAnchor.UpperLeft;
            Rect rect = new Rect(5f, 5f, 140f, 30f);
            int num1 = DefaultWorkPriorities.AutoSetPriorities ? 1 : 0;
            Widgets.CheckboxLabeled(rect, (string)"Auto Set Priorities", ref DefaultWorkPriorities.AutoSetPriorities);
            int num2 = DefaultWorkPriorities.AutoSetPriorities ? 1 : 0;
            if (num1 != num2)
            {

            }
            if (DefaultWorkPriorities.AutoSetPriorities)
            {
                GUI.color = new Color(1f, 1f, 1f, 0.5f);
                Text.Font = GameFont.Tiny;
                Widgets.Label(new Rect(rect.x, (float)((double)rect.y + (double)rect.height + 4.0), rect.width, 60f), "Minimum Level Reqired for Priority");
                Text.Font = GameFont.Small;
                GUI.color = Color.white;

                DrawAllAutoPriorityBoxes();

                Text.Font = GameFont.Small;
                GUI.color = Color.white;
                Text.Anchor = TextAnchor.UpperLeft;
                Rect rect1 = new Rect(160f, 5f, 180f, 30f);
                Widgets.CheckboxLabeled(rect1, (string)"Only Update On Age Up", ref DefaultWorkPriorities.OnlyUpdateOnAgeUp);

            }
            if (DefaultWorkPriorities.AutoSetPriorities)
                return;
            UIHighlighter.HighlightOpportunity(rect, "Auto Set Priorities -Off");
        }

        private void DrawAllAutoPriorityBoxes()
        {
            int boxSize = 25;

            int spacing = 30;


            Vector2Int startPos = new Vector2Int(15, 80);

            GUI.color = Color.white;
            Text.Font = GameFont.Small;

            for (int i = 0; i < DefaultWorkPriorities.MinLevelRequired.Count-1; i++)
            {
                Rect rect = new Rect(startPos.x, startPos.y + (spacing*i), boxSize, boxSize);

                int p = i + 1;

                Widgets.Label(new Rect(rect.x+boxSize+10, rect.y, 80, 60f), "Priority "+ p);

                DrawAutoPriorityBox(rect, p);
            }

            Text.Font = GameFont.Small;

        }

        private void DrawAutoPriorityBox(Rect rect, int pLevel)
        {

            DrawPriorityBoxBackground(rect);

            Verse.Text.Anchor = TextAnchor.MiddleCenter;
            GUI.color = Color.white;
            Widgets.Label(rect.ContractedBy(-3f), DefaultWorkPriorities.MinLevelRequired[pLevel].ToStringCached());
            GUI.color = Color.white;
            Verse.Text.Anchor = TextAnchor.UpperLeft;

            if (!JustClicked)
            {
                if (Mouse.IsOver(rect))
                {
                    if (Input.GetMouseButtonDown(0))
                    {
                        PriorityBoxClicked(pLevel, true);
                    }

                    if (Input.GetMouseButtonDown(1))
                    {
                        PriorityBoxClicked(pLevel, false);
                    }
                }
            }
            else if(Input.GetMouseButtonUp(1)|| Input.GetMouseButtonUp(0))
            {
                JustClicked= false;
            }

        }

        private void DrawPriorityBoxBackground(Rect rect)
        {
            Texture2D image;

            image = WidgetsWork.WorkBoxBGTex_Mid;

            GUI.DrawTexture(rect, (Texture)image);
        }

        private void PriorityBoxClicked(int pLevel, bool L)
        {
            JustClicked = true;

            int currentPriority = DefaultWorkPriorities.MinLevelRequired[pLevel];

            if (!L)
            {
                int priority = currentPriority - 1;
                if (priority < 0)
                    return;

                if (pLevel != 4 && currentPriority <= DefaultWorkPriorities.MinLevelRequired[pLevel + 1])
                    return;

                DefaultWorkPriorities.MinLevelRequired[pLevel] = priority;
            }
            if (L)
            {
                int priority1 = currentPriority + 1;
                if (priority1 > 20)
                    return;

                if (pLevel != 1 && currentPriority >= DefaultWorkPriorities.MinLevelRequired[pLevel - 1])
                    return;

                DefaultWorkPriorities.MinLevelRequired[pLevel] = priority1;
            }
        }
    }
}
