using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using Verse;
using HarmonyLib;

namespace VarietyMattersDT
{
    public class Mod_VMDT : Mod
    {
        private static Listing_Standard listing = new Listing_Standard();
        public static ModSettings_VMDT settings;
        private static int currentTab;
        private static Vector2 mealScrollPos = Vector2.zero;
        private static Vector2 foodScrollPos = Vector2.zero;

        public Mod_VMDT(ModContentPack content) : base(content)
        {
            settings = GetSettings<ModSettings_VMDT>();
            if (ModSettings_VMDT.freshUpdate < 1) ModSettings_VMDT.freshUpdate++;
            WriteSettings();
            Harmony harmony = new Harmony("rimworld.varietymattersDT");
            harmony.PatchAll(Assembly.GetExecutingAssembly());
        }

        public override string SettingsCategory()
        {
            return "VarietyMattersDinnerTime";
        }

        public override void WriteSettings()
        {
            base.WriteSettings();
        }

        public override void DoSettingsWindowContents(Rect inRect)
        {
            base.DoSettingsWindowContents(inRect);
            Rect canvas = new Rect(inRect.x, inRect.y + 30f, inRect.width, inRect.height - 30f);
            Widgets.DrawMenuSection(canvas);
            List<TabRecord> tabs = new List<TabRecord>
            {
                new TabRecord("VMDT.Tab0".Translate(), delegate()
                {
                    currentTab = 0;
                },  currentTab == 0),
                new TabRecord("VMDT.Tab1".Translate(), delegate()
                {
                    currentTab = 1;
                }, currentTab == 1),
            };
            TabDrawer.DrawTabs(canvas, tabs, 200f);
            if (currentTab == 0) DoMainSettings(canvas);
            if (currentTab == 1) DoTablelessFoods(canvas);
        }

        public static void DoMainSettings(Rect rect)
        {
            listing.Begin(rect.ContractedBy(15f));
            //Meal Time
            Text.Anchor = TextAnchor.MiddleCenter;
            listing.Label("VMDT.MealTimeLabel".Translate());
            Text.Anchor = TextAnchor.UpperLeft;
            listing.Indent();
            string foodPosBuffer = ModSettings_VMDT.assignmentPos.ToString();
            string foodPosLabel = "VMDT.MealTimePos".Translate();
            ModSettings_Utility.LabeledFloatEntry(listing.GetRect(24f), foodPosLabel, ref ModSettings_VMDT.assignmentPos, ref foodPosBuffer, 1f, 1f, 0f, 4f);
            listing.Outdent();
            listing.GapLine();
            //Food Selection
            Text.Anchor = TextAnchor.MiddleCenter;
            listing.Label("VMDT.FoodSelectionLabel".Translate());
            Text.Anchor = TextAnchor.UpperLeft;
            listing.Indent();
            ModSettings_Utility.IndentedCheckboxLabeled(listing, "VMDT.RoomFood".Translate(), ref ModSettings_VMDT.preferDiningFood);
            ModSettings_Utility.IndentedCheckboxLabeled(listing,"VMDT.Spoiling".Translate(), ref ModSettings_VMDT.preferSpoiling);
            listing.Outdent();
            listing.GapLine();

            //Quality Cooking
            Text.Anchor = TextAnchor.MiddleCenter;
            listing.Label("VMDT.QualityLabel".Translate());
            Text.Anchor = TextAnchor.UpperLeft;
            listing.Indent();
            ModSettings_Utility.IndentedCheckboxLabeled(listing,"VMDT.QualityToggle".Translate(), ref ModSettings_VMDT.cookingQuality);
            ModSettings_Utility.IndentedCheckboxLabeled(listing,"VMDT.LavishThought".Translate(), ref ModSettings_VMDT.memorableLavish);
            listing.Outdent();
            listing.GapLine();
            //FreshlyCooked
            Text.Anchor = TextAnchor.MiddleCenter;
            listing.Label("VMDT.FreshLabel".Translate());
            Text.Anchor = TextAnchor.UpperLeft;
            listing.Indent();
            ModSettings_Utility.IndentedCheckboxLabeled(listing,"VMDT.Hot".Translate(), ref ModSettings_VMDT.warmMeals);
            ModSettings_Utility.IndentedCheckboxLabeled(listing,"VMDT.Leftovers".Translate(), ref ModSettings_VMDT.leftoverMeals);
            ModSettings_Utility.IndentedCheckboxLabeled(listing,"VMDT.Frozen".Translate(), ref ModSettings_VMDT.frozenMeals);
            if (ModSettings_VMDT.warmMeals)
            {
                string warmLabel = "VMDT.WarmHours".Translate();
                string warmBuffer = ModSettings_VMDT.warmHours.ToString();
                ModSettings_Utility.LabeledFloatEntry(listing.GetRect(24f), warmLabel, ref ModSettings_VMDT.warmHours, ref warmBuffer, 1f, 5f, 1f, 72f);
            }
            if (ModSettings_VMDT.leftoverMeals)
            {
                string leftoverLabel = "VMDT.RefrigHours".Translate();
                string leftoverBuffer = ModSettings_VMDT.leftoverHours.ToString();
                ModSettings_Utility.LabeledFloatEntry(listing.GetRect(24f), leftoverLabel, ref ModSettings_VMDT.leftoverHours, ref leftoverBuffer, 1f, 10f, ModSettings_VMDT.warmHours, 72f);
            }
            if (ModSettings_VMDT.warmMeals || ModSettings_VMDT.leftoverMeals)
            {
                string refrigLabel = "VMDT.RefrigMulti".Translate();
                string refrigBuffer = ModSettings_VMDT.refrigMulti.ToString();
                ModSettings_Utility.LabeledFloatEntry(listing.GetRect(24f), refrigLabel, ref ModSettings_VMDT.refrigMulti, ref refrigBuffer, .1f, 1f, 1f, 10f);
                string freshTempLabel = "VMDT.WarmTemp".Translate();
                string freshTempBuffer = ModSettings_VMDT.minFreshTemp.ToString();
                ModSettings_Utility.LabeledFloatEntry(listing.GetRect(24f), freshTempLabel, ref ModSettings_VMDT.minFreshTemp, ref freshTempBuffer, 1f, 10f, 1f, 100f);
            }
            listing.Outdent();
            listing.End();
        }

        public static void DoTablelessFoods(Rect rect)
        {
            List<string> meals = ModSettings_VMDT.tablelessMeals.Keys.ToList();
            List<string> foods = ModSettings_VMDT.tablelessFoods.Keys.ToList();

            Rect firstCol = new Rect(rect.x + 5f, rect.y + 5f, rect.width * .48f, rect.height);
            listing.Begin(firstCol);
            listing.CheckboxLabeled("VMDT.TablesToggle".Translate(), ref ModSettings_VMDT.foodsWithoutTable);
            listing.Gap(24f);
            listing.GapLine();
            if (ModSettings_VMDT.foodsWithoutTable)
            {
                Text.Anchor = TextAnchor.MiddleCenter;
                listing.Label("Meals:");
                Text.Anchor = TextAnchor.UpperLeft;
                Rect mealScroll = new Rect(0f, listing.CurHeight, firstCol.width, rect.height - listing.CurHeight - 10f);
                Rect mealView = new Rect(0f, 0f, mealScroll.width - 20f, meals.Count * 32f);
                Widgets.BeginScrollView(mealScroll, ref mealScrollPos, mealView, true);
                listing.Begin(mealView);
                meals.Sort();
                foreach (string meal in meals)
                {
                    int value = ModSettings_VMDT.tablelessMeals[meal];
                    string label = "VMDT.TableReq".Translate();
                    if (value == 1)
                    {
                        label = ColoredText.Colorize("VMDT.Tableless".Translate(), Color.cyan);
                    }
                    if (value == 2)
                    {
                        label = ColoredText.Colorize("VMDT.Packable".Translate(), Color.green);
                    }
                    if (listing.ButtonTextLabeled(meal, label))
                    {
                        if (value == 2) value = 0;
                        else value += 1;
                        DefMod_VMDT.AddOrRemove(meal, value);
                        ModSettings_VMDT.tablelessMeals[meal] = value;
                    }
                }
                listing.End();
                Widgets.EndScrollView();
            }
            listing.End();

            Rect secCol = new Rect(rect.width * .5f + 5f, rect.y + 5f, rect.width * .48f, rect.height);
            listing.Begin(secCol);
            listing.CheckboxLabeled("VMDT.TablesThought".Translate(), ref ModSettings_VMDT.useTableThought);
            listing.Gap(24f);
            listing.GapLine();
            if (ModSettings_VMDT.foodsWithoutTable)
            {
                Text.Anchor = TextAnchor.MiddleCenter;
                listing.Label("Other Foods:");
                Text.Anchor = TextAnchor.UpperLeft;
                Rect foodScroll = new Rect(0f, listing.CurHeight, secCol.width, rect.height - listing.CurHeight - 10f);
                Rect foodView = new Rect(0f, 0f, foodScroll.width - 20f, foods.Count * 32f);
                Widgets.BeginScrollView(foodScroll, ref foodScrollPos, foodView, true);
                listing.Begin(foodView);
                foreach (string food in foods)
                {
                    int value = ModSettings_VMDT.tablelessFoods[food];
                    string label = "VMDT.TableReq".Translate();
                    if (value == 1)
                    {
                        label = ColoredText.Colorize("VMDT.Tableless".Translate(), Color.cyan);
                    }
                    if (value == 2)
                    {
                        label = ColoredText.Colorize("VMDT.Packable".Translate(), Color.green);
                    }
                    if (listing.ButtonTextLabeled(food, label))
                    {
                        if (value == 2) value = 0;
                        else value += 1;
                        DefMod_VMDT.AddOrRemove(food, value);
                        ModSettings_VMDT.tablelessFoods[food] = value;
                    }
                }
                listing.End();
                Widgets.EndScrollView();
            }
            listing.End();
        }
    }
}
