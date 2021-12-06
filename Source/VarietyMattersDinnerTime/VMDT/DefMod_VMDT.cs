using System.Collections.Generic;
using Verse;

namespace VarietyMattersDT
{
    [StaticConstructorOnStartup]
    public class DefMod_VMDT : DefModExtension
    {
        public int packBonus = 25;

        public static void AddOrRemove(string name, int value)
        {
            DefMod_VMDT mod = new DefMod_VMDT();
            ThingDef def = DefDatabase<ThingDef>.GetNamed(name);
            if (value == 0)
            {
                if (def.HasModExtension<DefMod_VMDT>())
                {
                    mod = def.GetModExtension<DefMod_VMDT>();
                    def.modExtensions.Remove(mod);
                }
            }
            else if (!def.HasModExtension<DefMod_VMDT>())
            {
                if (def.modExtensions == null) def.modExtensions = new List<DefModExtension>();
                mod = new VarietyMattersDT.DefMod_VMDT();
                if (value == 2) mod.packBonus = 58;
                def.modExtensions.Add(mod);
            }
            else
            {
                mod = def.GetModExtension<DefMod_VMDT>();
                if (value == 1) mod.packBonus = 25;
                else mod.packBonus = 58;
            }
        }
    }
}
