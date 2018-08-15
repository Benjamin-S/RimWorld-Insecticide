using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;
using Harmony;
using System.Reflection;
using RimWorld;

namespace Insecticide
{
    [StaticConstructorOnStartup]
    class Main
    {
        static Main()
        {
            var harmony = HarmonyInstance.Create("com.github.harmony.insecticide");
            harmony.PatchAll(Assembly.GetExecutingAssembly());

            Log.Message("Insecticide: Adding Harmony PRefix to Designator_AreaNoRoof.CanDesignateCell");
        }

        [HarmonyPatch(typeof(Designator_AreaNoRoof), "CanDesignateCell")]
        static class Patch_Designator_AreaNoRoof_CanDesignateCell
        {
            static void Postfix(Designator_AreaNoRoof __instance, IntVec3 c, ref AcceptanceReport __result)
            {
                RoofDef roofDef = __instance.Map.roofGrid.RoofAt(c);
                if (roofDef.defName == "RoofClean")
                {
                    __result = "You can't remove clean roof".Translate();
                }
            }
        }
    }
}
