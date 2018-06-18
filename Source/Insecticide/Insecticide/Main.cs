//using System.Collections.Generic;
//using System.Linq;
//using Verse;
//using Harmony;
//using System.Reflection;
//using UnityEngine;
//using RimWorld;

//namespace Insecticide
//{
//    [StaticConstructorOnStartup]
//    class Main
//    {
//        //private static ByteGrid distToRepellantBuilding;

//        //private static List<IntVec3> tmpRepellantBuildingsLocs = new List<IntVec3>();

//        //private static List<KeyValuePair<IntVec3, float>> tmpDistanceResult = new List<KeyValuePair<IntVec3, float>>();

//        static Main()
//        {
//            HarmonyInstance harmonyInstance = HarmonyInstance.Create("com.symons.insecticide");
//            harmonyInstance.PatchAll(Assembly.GetExecutingAssembly());

//            //MethodInfo target = AccessTools.Method(typeof(RimWorld.InfestationCellFinder), "GetScoreAt");
//            //HarmonyMethod prefixMethod = new HarmonyMethod(typeof(Main).GetMethod("PatchAtInsectRepellant_Prefix"));

//            //harmony.Patch(target, prefixMethod, null);

//            Log.Message("Insecticide: Adding Harmony Prefix to Rimworld.InfestationCellFinder");
//        }

//        //static bool PatchAtInsectRepellant_Prefix(IntVec3 cell, Map map, ref float __result)
//        //{
//        //    CalculateDistanceToRepellantBuildingGrid(map);
//        //    Log.Message("Dist to Repellant Building is " + (float)distToRepellantBuilding[cell]);
//        //    if ((float)distToRepellantBuilding[cell] < 32f)
//        //    {
//        //        __result = 0f;
//        //        return false;
//        //    }
//        //    Log.Message("Dist to Repellant Building is " + (float)distToRepellantBuilding[cell]);
//        //    return true;
//        //}

//        //private static void CalculateDistanceToRepellantBuildingGrid(Map map)
//        //{
//        //    if (distToRepellantBuilding == null)
//        //    {
//        //        distToRepellantBuilding = new ByteGrid(map);
//        //    }
//        //    else if (!distToRepellantBuilding.MapSizeMatches(map))
//        //    {
//        //        distToRepellantBuilding.ClearAndResizeTo(map);
//        //    }
//        //    distToRepellantBuilding.Clear(255);
//        //    tmpRepellantBuildingsLocs.Clear();
//        //    List<Building_Repellant> allRepellantColonist = map.listerBuildings.AllBuildingsColonistOfClass<Building_Repellant>().ToList();
//        //    for (int i = 0; i < allRepellantColonist.Count; i++)
//        //    {
//        //        tmpRepellantBuildingsLocs.Add(allRepellantColonist[i].Position);
//        //    }
//        //    Dijkstra<IntVec3>.Run(tmpRepellantBuildingsLocs, (IntVec3 x) => DijkstraUtility.AdjacentCellsNeighborsGetter(x, map), delegate (IntVec3 a, IntVec3 b)
//        //    {
//        //        if (a.x == b.x || a.z == b.z)
//        //        {
//        //            return 1f;
//        //        }
//        //        return 1.41421354f;
//        //    }, tmpDistanceResult, null);
//        //    for (int j = 0; j < tmpDistanceResult.Count; j++)
//        //    {
//        //        distToRepellantBuilding[tmpDistanceResult[j].Key] = (byte)Mathf.Min(tmpDistanceResult[j].Value, 254.999f);
//        //    }
//        //}
//    }


//    [HarmonyPatch(typeof(InfestationCellFinder))]
//    [HarmonyPatch("GetScoreAt")]
//    static class Patch_InfestationCellFinder_GetScoreAt
//    {
//        static List<IntVec3> cellsToIgnore;
//        static bool PreFix(IntVec3 cell, Map map, ref float __result)
//        {
//            if (cellsToIgnore == null)
//            {
//                cellsToIgnore = new List<IntVec3>();
//            }

//            List<Building_Repellant> repellantBuildingList = map.listerBuildings.AllBuildingsColonistOfClass<Building_Repellant>().ToList();
//            if (repellantBuildingList != null)
//            {
//                foreach (Building_Repellant repellant in repellantBuildingList)
//                {
//                    if (!cellsToIgnore.Contains(cell))
//                    {
//                        foreach (IntVec3 cellsToAdd in GenRadial.RadialCellsAround(cell, 6f, true))
//                        {
//                            cellsToIgnore.Add(cellsToAdd);
//                        }
//                    }
//                }
//            }
//            if (cellsToIgnore.Contains(cell))
//            {
//                __result = 0f;
//                return false;
//            }
//            return true;
//        }
//    }
//}
