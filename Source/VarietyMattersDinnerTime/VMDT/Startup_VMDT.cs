using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;

namespace VarietyMattersDT
{
    [StaticConstructorOnStartup]
    public static class Startup_VMDT 
    {

        static Startup_VMDT()
        {
            if (ModSettings_VMDT.tablelessMeals == null || ModSettings_VMDT.tablelessMeals.Count == 0) //First run
            {
                ModSettings_VMDT.tablelessMeals = new Dictionary<string, int>();
                ModSettings_VMDT.tablelessFoods = new Dictionary<string, int>();
                InitializeTablelessDict();
            }
            else
            {
                RemoveOldDefs();
                UpdateDefMods();
            }
                //UpdateDef.DisplayUpdate();
        }

        public static void UpdateDefMods()
        {
            bool newFoods = false;
            int value;
            DefMod_VMDT mod = new DefMod_VMDT();
            foreach (ThingDef def in DefDatabase<ThingDef>.AllDefsListForReading.Where(x => x.IsNutritionGivingIngestible && x.ingestible.HumanEdible && x.ingestible.chairSearchRadius > 10f))
            {
                if (ModSettings_VMDT.tablelessMeals.ContainsKey(def.defName))
                {
                    value = ModSettings_VMDT.tablelessMeals[def.defName];
                    if (value == 0)
                    {
                        if (def.HasModExtension<VarietyMattersDT.DefMod_VMDT>())
                        {
                            mod = def.GetModExtension<VarietyMattersDT.DefMod_VMDT>();
                            def.modExtensions.Remove(mod);
                        }
                    }
                    else if (!def.HasModExtension<VarietyMattersDT.DefMod_VMDT>())
                    {
                        if (def.modExtensions == null) def.modExtensions = new List<DefModExtension>();
                        mod = new VarietyMattersDT.DefMod_VMDT();
                        if (value == 2) mod.packBonus = 58;
                        def.modExtensions.Add(mod);
                    }
                    else
                    {
                        mod = def.GetModExtension<VarietyMattersDT.DefMod_VMDT>();
                        if (value == 1) mod.packBonus = 25;
                        else mod.packBonus = 58;
                    }
                }
                else if (ModSettings_VMDT.tablelessFoods.ContainsKey(def.defName))
                {
                    value = ModSettings_VMDT.tablelessFoods[def.defName];
                    if (value == 0)
                    {
                        if (def.HasModExtension<VarietyMattersDT.DefMod_VMDT>())
                        {
                            mod = def.GetModExtension<VarietyMattersDT.DefMod_VMDT>();
                            def.modExtensions.Remove(mod);
                        }
                    }
                    else if (!def.HasModExtension<VarietyMattersDT.DefMod_VMDT>())
                    {
                        if (def.modExtensions == null) def.modExtensions = new List<DefModExtension>();
                        mod = new VarietyMattersDT.DefMod_VMDT();
                        if (value == 2) mod.packBonus = 58;
                        def.modExtensions.Add(mod);
                    }
                    else
                    {
                        mod = def.GetModExtension<VarietyMattersDT.DefMod_VMDT>();
                        if (value == 1) mod.packBonus = 25;
                        else mod.packBonus = 58;
                    }
                }
                else if (!def.IsCorpse) newFoods = true;
            }
            if (newFoods)
            {
                InitializeTablelessDict();
            }
        }

        public static void InitializeTablelessDict()
        {
            Log.Message("[VMDT] Detected new foods");
            List<ThingDef> list = new List<ThingDef>();
            foreach (ThingDef food in DefDatabase<ThingDef>.AllDefsListForReading.Where(x => x.IsNutritionGivingIngestible && x.ingestible.HumanEdible && x.ingestible.chairSearchRadius > 10f))
            {
                if (food.ingestible.preferability >= FoodPreferability.MealAwful)
                {
                    if (!food.HasModExtension<DefMod_VMDT>())
                    {
                        ModSettings_VMDT.tablelessMeals[food.defName] = 0;
                    }
                    else if (food.GetModExtension<DefMod_VMDT>().packBonus == 25)
                    {
                        ModSettings_VMDT.tablelessMeals[food.defName] = 1;
                    }
                    else ModSettings_VMDT.tablelessMeals[food.defName] = 2;
                }
                else if (!food.IsCorpse)
                {
                    list.Add(food);
                }
            }
            list.OrderBy(x => x.ingestible.foodType).ThenBy(x => x.defName);
            for (int i = 0; i < list.Count; i++)
            {
                ThingDef food = list[i];
                if (!food.HasModExtension<DefMod_VMDT>())
                {
                    ModSettings_VMDT.tablelessFoods[food.defName] = 0;
                }
                else if (food.GetModExtension<DefMod_VMDT>().packBonus == 25)
                {
                    ModSettings_VMDT.tablelessFoods[food.defName] = 1;
                }
                else ModSettings_VMDT.tablelessFoods[food.defName] = 2;
            }
        }

        public static void RemoveOldDefs()
        {
            List<string> list = ModSettings_VMDT.tablelessMeals.Keys.ToList();
            int i = 0;
            while (i < list.Count)
            {
                if (DefDatabase<ThingDef>.GetNamedSilentFail(list[i]) == null)
                {
                    ModSettings_VMDT.tablelessMeals.Remove(list[i]);
                }
                i++;
            }
            list.AddRange(ModSettings_VMDT.tablelessFoods.Keys.ToList());
            while (i < list.Count)
            {
                if (DefDatabase<ThingDef>.GetNamedSilentFail(list[i]) == null)
                {
                    ModSettings_VMDT.tablelessFoods.Remove(list[i]);
                }
                i++;
            }
            Log.Message("[VMDT] Done removing old defs");
        }
    }
}
