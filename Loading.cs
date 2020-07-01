using System;
using System.Collections.Generic;
using ICities;


namespace GarbageBinManager
{
    /// <summary>
    /// Main loading class: the mod runs from here.
    /// </summary>
    public class Loading : LoadingExtensionBase
    {
        /// <summary>
        /// Called by the game when level loading is complete.
        /// </summary>
        /// <param name="mode">Loading mode (e.g. game, editor, scenario, etc.)</param>
        public override void OnLevelLoaded(LoadMode mode)
        {
            // Initialise our list of valid bin replacement props.
            ModSettings.binList = new SortedList<string, BinRecord>();

            // Add intial 'random' item.
            ModSettings.binList.Add("000." + Translations.Translate("GBM_PRP_RDM"), null);

            // Iterate through all loaded props, looking for props that meet our requirements.
            int propCount = PrefabCollection<PropInfo>.LoadedCount();
            for (uint i = 0; i < propCount; ++ i)
            {
                PropInfo thisProp = PrefabCollection<PropInfo>.GetLoaded(i);

                // Check for valid prop, and that it doesn't require a height map.
                if (thisProp?.name != null && !thisProp.m_requireHeightMap)
                {
                    float rotation;

                    // Does this meet our automatic criteria ('garbage' service and automatic (not manual) placement)?
                    if (thisProp.GetService() == ItemClass.Service.Garbage && thisProp.m_placementStyle == ItemClass.Placement.Automatic)
                    {
                        // Default roation is 180 degrees (in Radians!) to suit Arnold J. Rimmer, Bsc. Ssc.'s wheelie bins (front to curb). 
                        rotation = (float)Math.PI;
                    }
                    // Otherwise, is it one of our custom ones?
                    else if (CustomBins.binList.ContainsKey(thisProp.name))
                    {
                        rotation = CustomBins.binList[thisProp.name];
                    }
                    else
                    {
                        // No match; carry on!
                        continue;
                    }

                    // Check to make sure we don't have a duplicate here.
                    if (ModSettings.binList.ContainsKey(thisProp.name))
                    {
                        Debugging.Message("duplicate prop name " + thisProp.name);
                    }
                    else
                    {
                        // All good - add to list.
                        ModSettings.binList.Add(thisProp.name, new BinRecord { binProp = thisProp, rotation = rotation });
                    }
                }
            }

            // Set current bin.
            ModSettings.currentBin = ModSettings.binList[ModSettings.currentBinName];

            // Set up options panel event handler.
            OptionsPanelManager.OptionsEventHook();
        }
    }
}