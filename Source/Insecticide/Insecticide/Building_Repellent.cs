using RimWorld;
using System.Collections.Generic;
using System.Text;
using Verse;

namespace Insecticide
{
    public class Building_Repellent : Building
    {
        private CompGlower glowerComp;
        private CompPowerTrader powerComp;
        private bool destroyedFlag = false;
        private IntVec3 centerLoc;

        private List<IntVec3> originalThickRoof = new List<IntVec3>();


        // Called when building is spawned.
        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            // Do the work of the base class (Building)
            base.SpawnSetup(map, respawningAfterLoad);

            // Get reference to the components CompPowerTrader and CompGlower
            SetPowerGlower();

            //Log.Message("Placing repellent Building with an effective radius of " + this.def.specialDisplayRadius);
            //Log.Message("Count of cells affected should be " + GenRadial.RadialCellsAround(this.Position, this.def.specialDisplayRadius, true).Count());

            //OutputAffectedCells(this.Position, this.def.specialDisplayRadius, map, "initialCells.txt");
            //CellMap(this.Position, map, "cellGrid.csv");

            GetOriginalRoofInRange();

            centerLoc = base.Position;
        }

        public void SetPowerGlower()
        {
            // Get references to the components CompPowerTrader and CompGlower
            powerComp = base.GetComp<CompPowerTrader>();
            glowerComp = base.GetComp<CompGlower>();

            // Default the power to 0 so that no power is going in or out
            powerComp.PowerOutput = 0;
        }

        public override void Destroy(DestroyMode mode = DestroyMode.Vanish)
        {
            // Block further ticker work
            destroyedFlag = true;

            // Make sure we reset the roof that we have affected
            ResetRoofInRange();

            // Do the work of the base class (Building)
            base.Destroy(mode);
        }

        public override void TickRare()
        {
            // Check if destroyed, and stop doing work if it is
            if (destroyedFlag)
            {
                return;
            }

            // Do the work of the base class (Building)
            base.Tick();

            // Call work function
            DoTickerWork(250);
        }

        public override void Tick()
        {
            // Check if destroyed, and stop doing work if it is
            if (destroyedFlag)
            {
                return;
            }

            // Do the work of the base class (Building)
            base.Tick();

            // Call work function
            DoTickerWork(1);
        }

        private void DoTickerWork(int tickerAmount)
        {

            if (powerComp.PowerOn)
            {
                if (GridsUtility.Roofed(this.centerLoc, this.Map))
                {
                    if (!BreakdownableUtility.IsBrokenDown(this))
                    {

                        SetRoofInRange();
                        ResetRemovedRoof();
                    }
                }
                CompProperties_Power props = powerComp.Props;
                powerComp.PowerOutput = -props.basePowerConsumption;
            }
            else
            {
                ResetRoofInRange();

                powerComp.PowerOutput = 0;
            }

        }

        //private void OutputAffectedCells(IntVec3 center, float radius, Map map, string outputFile)
        //{
        //    List<string> outputText = new List<string>();
        //    foreach (IntVec3 cell in GenRadial.RadialCellsAround(center, radius, true))
        //    {
        //        if(cell.Roofed(map))
        //            outputText.Add(GridsUtility.GetRoof(cell, map).defName + " isThickRoof: " + GridsUtility.GetRoof(cell, map).isThickRoof + " at " + cell.x + ", " + cell.z);
        //    }
        //    string formattedString = string.Join("\n", outputText.ToArray());
        //    System.IO.File.WriteAllText(outputFile, formattedString);

        //}

        //private void CellMap(IntVec3 center, Map map, string outputFile)
        //{
        //    int width = 100;
        //    int height = 100;

        //    int startX = center.x - (width / 2);
        //    int startZ = center.z - (width / 2);

        //    int endX = center.x + (width / 2);
        //    int endZ = center.z + (width / 2);

        //    string[,] vs = new string[width, height];

        //    Log.Message("Looking at cells between x" + startX + " and x" + endX);
        //    Log.Message("Looking at cells between z" + startZ + " and z" + endZ);

        //    //for (int i = 0; i < width; i++)
        //    //{
        //    //    for (int j = 0; j < height; j++)
        //    //    {
        //    //        RoofDef currRoof = GridsUtility.GetRoof(new IntVec3(startX + i, 0, endZ - j), map);
        //    //        if(currRoof != null)
        //    //        {
        //    //            if (currRoof.defName == "RoofConstructed")
        //    //            {
        //    //                vs[i, j] = "c";
        //    //            }
        //    //            else if (currRoof.defName == "RoofRockThin")
        //    //            {
        //    //                vs[i, j] = "r";
        //    //            }
        //    //            else if (currRoof.defName == "RoofRockThick")
        //    //            {
        //    //                vs[i, j] = "t";
        //    //            }
        //    //            else
        //    //            {
        //    //                vs[i, j] = "_";
        //    //            }
        //    //        }
        //    //        else
        //    //        {
        //    //            vs[i, j] = "n";
        //    //        }
        //    //    }
        //    //}

        //    for (int i = 0; i < width; i++)
        //    {
        //        for (int j = 0; j < height; j++)
        //        {
        //            RoofDef currRoof = GridsUtility.GetRoof(new IntVec3(startX + i, 0, endZ - j), map);
        //            if (currRoof != null)
        //            {
        //                if (currRoof.isThickRoof)
        //                {
        //                    vs[i, j] = "T";
        //                }
        //                else
        //                {
        //                    vs[i, j] = "_";
        //                }
        //            }
        //            else
        //            {
        //                vs[i, j] = "n";
        //            }
        //        }
        //    }

        //    using (var sw = new System.IO.StreamWriter(outputFile))
        //    {
        //        for (int i = 0; i < width; i++)
        //        {
        //            for (int j = 0; j < height; j++)
        //            {
        //                sw.Write(vs[j, i] + ", ");
        //            }
        //            sw.Write("\n");
        //        }

        //        sw.Flush();
        //        sw.Close();
        //    }
        //}

        private void ResetRoofInRange()
        {
            foreach (IntVec3 cell in GenRadial.RadialCellsAround(this.centerLoc, this.def.specialDisplayRadius, true))
            {
                if (cell.InBounds(base.Map))
                {
                    if (cell.GetRoof(base.Map) != null)
                    {
                        if (GridsUtility.GetRoof(cell, base.Map).defName == "RoofClean")
                        {
                            RoofDef roofType = DefDatabase<RoofDef>.GetNamed("RoofRockThick");
                            base.Map.roofGrid.SetRoof(cell, roofType);
                        }
                    }
                }
            }
        }


        private void GetOriginalRoofInRange()
        {
            foreach (IntVec3 cell in GenRadial.RadialCellsAround(this.centerLoc, this.def.specialDisplayRadius, true))
            {
                if (cell.InBounds(base.Map))
                {
                    if (cell.GetRoof(base.Map) != null)
                    {
                        if (GridsUtility.GetRoof(cell, base.Map).isThickRoof)
                        {
                            originalThickRoof.Add(cell);
                        }
                    }
                }
            }
        }

        private void ResetRemovedRoof()
        {
            foreach (IntVec3 cell in originalThickRoof)
            {
                if (cell.GetRoof(base.Map) == null)
                {
                    RoofDef roofType = DefDatabase<RoofDef>.GetNamed("RoofRockThick");
                    base.Map.roofGrid.SetRoof(cell, roofType);
                }
            }
        }

        private void SetRoofInRange()
        {
            foreach (IntVec3 cell in GenRadial.RadialCellsAround(this.centerLoc, this.def.specialDisplayRadius, true))
            {
                if (cell.InBounds(base.Map))
                {
                    if (cell.GetRoof(base.Map) != null)
                    {
                        if (GridsUtility.GetRoof(cell, base.Map).isThickRoof)
                        {
                            RoofDef roofType = DefDatabase<RoofDef>.GetNamed("RoofClean");
                            base.Map.roofGrid.SetRoof(cell, roofType);
                        }
                    }
                }
            }
            //Log.Message("Set " + count + " roof in range");
            //OutputAffectedCells(centerLoc, this.def.specialDisplayRadius, base.Map, "changedCells.txt");
            //CellMap(centerLoc, base.Map, "cellGrid_post.csv");
        }



        //private void SetRoofInRange()
        //{
        //    int count = 0;
        //    foreach (IntVec3 cell in GenRadial.RadialCellsAround(this.centerLoc, this.def.specialDisplayRadius, true))
        //    {
        //        if (cell.InBounds(base.Map))
        //        {
        //            bool flag = GridsUtility.Roofed(cell, base.Map);
        //            if (flag)
        //            {
        //                bool flag2 = roofDef.defName == "RockRoofThin";
        //                if (flag2)
        //                {

        //                }
        //                roofDef = GridsUtility.GetRoof(cell, base.Map);
        //                bool flag3 = roofDef.filthLeaving == ThingDef.Named("RockRubble");
        //                if (flag3)
        //                {
        //                    roofEdifice = GridsUtility.GetEdifice(cell, base.Map);
        //                    bool flag4 = roofEdifice != null && roofEdifice.def.building.isNaturalRock;
        //                    if (flag4)
        //                    {
        //                        roofEdifice.def.building.isNaturalRock = false;
        //                    }
        //                    //roofDef.isNatural = false;
        //                    roofDef.isThickRoof = false;
        //                    roofGrid.SetRoof(cell, roofDef);
        //                    count++;
        //                }
        //            }
        //        }
        //    }
        //    Log.Message("Set " + count + " roof in range");
        //    OutputAffectedCells(centerLoc, this.def.specialDisplayRadius, base.Map, "changedCells.txt");
        //    CellMap(centerLoc, base.Map, "cellGrid_post.csv");
        //}

        public override string GetInspectString()
        {
            StringBuilder stringBuilder = new StringBuilder();

            string baseString = base.GetInspectString();

            if (!baseString.NullOrEmpty())
            {
                stringBuilder.Append(baseString);
                stringBuilder.AppendLine();
            }

            if (this.powerComp.PowerOn)
            {
                stringBuilder.Append("Successfully repelling infestations.".Translate());
            }
            else
            {
                stringBuilder.Append("Not repelling infestations. You are in danger.".Translate());
            }

            return stringBuilder.ToString().TrimEndNewlines();
        }
    }
}
