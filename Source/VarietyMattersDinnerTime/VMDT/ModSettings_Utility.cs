using System.Reflection;
using UnityEngine;
using RimWorld;
using Verse;
using Verse.Sound;
using HarmonyLib;

namespace VarietyMattersDT
{
    public class ModSettings_Utility 
    {
        public static void LabeledFloatEntry(Rect rect, string label, ref float value, ref string editBuffer, float multiplier, float largeMultiplier, float min, float max)
        {
            rect.width -= 12f;
            float num = rect.width / 15f;
            Widgets.Label(rect, label);
            if (multiplier != largeMultiplier)
            {
                if (Widgets.ButtonText(new Rect(rect.xMax - num * 5f, rect.yMin, num, rect.height), (-1 * largeMultiplier).ToString(), true, true, true))
                {
                    value -= largeMultiplier * GenUI.CurrentAdjustmentMultiplier();
                    editBuffer = value.ToString();
                    SoundDefOf.Checkbox_TurnedOff.PlayOneShotOnCamera(null);
                }
                if (Widgets.ButtonText(new Rect(rect.xMax - num, rect.yMin, num, rect.height), "+" + largeMultiplier.ToString(), true, true, true))
                {
                    value += largeMultiplier * GenUI.CurrentAdjustmentMultiplier();
                    editBuffer = value.ToString();
                    SoundDefOf.Checkbox_TurnedOn.PlayOneShotOnCamera(null);
                }
            }
            if (Widgets.ButtonText(new Rect(rect.xMax - num * 4f, rect.yMin, num, rect.height), (-1f * multiplier).ToString(), true, true, true))
            {
                value -= multiplier * GenUI.CurrentAdjustmentMultiplier();
                editBuffer = value.ToString();
                SoundDefOf.Checkbox_TurnedOff.PlayOneShotOnCamera(null);
            }
            if (Widgets.ButtonText(new Rect(rect.xMax - (num * 2f), rect.yMin, num, rect.height), "+" + multiplier.ToString(), true, true, true))
            {
                value += multiplier * GenUI.CurrentAdjustmentMultiplier();
                editBuffer = value.ToString();
                SoundDefOf.Checkbox_TurnedOn.PlayOneShotOnCamera(null);
            }
            Widgets.TextFieldNumeric<float>(new Rect(rect.xMax - (num * 3f), rect.yMin, num, rect.height), ref value, ref editBuffer, min, max);
        }

        public static void IndentedCheckboxLabeled(Listing_Standard listing, string label, ref bool checkOn, string tooltip = null)
        {
            float lineHeight = Text.LineHeight;
            Rect rect = listing.GetRect(lineHeight);
            rect.width -= 12f;
            if (listing.BoundingRectCached == null || rect.Overlaps(listing.BoundingRectCached.Value))
            {
                if (!tooltip.NullOrEmpty())
                {
                    if (Mouse.IsOver(rect))
                    {
                        Widgets.DrawHighlight(rect);
                    }
                    TooltipHandler.TipRegion(rect, tooltip);
                }
                Widgets.CheckboxLabeled(rect, label, ref checkOn, false, null, null, false);
            }
            listing.Gap(listing.verticalSpacing);
        }
    }
}
