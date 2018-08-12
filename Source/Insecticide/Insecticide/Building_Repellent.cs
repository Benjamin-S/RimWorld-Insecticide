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
        private List<IntVec3> originalThickRoof = new List<IntVec3>();

        // Called when building is spawned.
        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            // Do the work of the base class (Building)
            base.SpawnSetup(map, respawningAfterLoad);

            // Get reference to the components CompPowerTrader and CompGlower
            SetPowerGlower();

            // List of IntVec3 cells to query later for changes
            GetOriginalRoofInRange();
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
                if (GridsUtility.Roofed(base.Position, this.Map))
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
                // Set roofs that were changed back to being thickRoof
                ResetRoofInRange();

                // Stop using power
                powerComp.PowerOutput = 0;
            }

        }

        /// <summary>
        /// Resets RoofClean RoofDefs back to RoofRockThick upon power loss or destroyed
        /// </summary>
        private void ResetRoofInRange()
        {
            foreach (IntVec3 cell in GenRadial.RadialCellsAround(base.Position, this.def.specialDisplayRadius, true))
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

        /// <summary>
        /// Sets a list of cells that will be changed so that they can be reverted if removed.
        /// </summary>
        private void GetOriginalRoofInRange()
        {
            foreach (IntVec3 cell in GenRadial.RadialCellsAround(base.Position, this.def.specialDisplayRadius, true))
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

        /// <summary>
        /// Stops removing roof exploit. Resets cells in range that may have been removed
        /// </summary>
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

        /// <summary>
        /// Sets thickRoofed cells in specialDisplayRadius to RoofClean RoofDef
        /// </summary>
        private void SetRoofInRange()
        {
            foreach (IntVec3 cell in GenRadial.RadialCellsAround(base.Position, this.def.specialDisplayRadius, true))
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
        }

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
                stringBuilder.Append("Repelling infestations.".Translate());
            }
            else
            {
                stringBuilder.Append("Not repelling infestations. You are in danger.".Translate());
            }

            return stringBuilder.ToString().TrimEndNewlines();
        }
    }
}
