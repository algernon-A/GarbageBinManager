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
            ModSettings.propList = new SortedList<string, PropInfo>();

            // Add intial 'random' item.
            ModSettings.propList.Add("000." + Translations.Translate("GBM_PRP_RDM"), null);

            // Iterate through all loaded props, looking for props that meet our requirements.
            int propCount = PrefabCollection<PropInfo>.LoadedCount();
            for (uint i = 0; i < propCount; ++ i)
            {
                PropInfo thisProp = PrefabCollection<PropInfo>.GetLoaded(i);
                
                // Requirements are non-null, valid name, 'garbage' service, automatic (not manual) placement, and not requiring a height map
                if (thisProp != null && thisProp.name != null && thisProp.GetService() == ItemClass.Service.Garbage && thisProp.m_placementStyle == ItemClass.Placement.Automatic && !thisProp.m_requireHeightMap)
                {
                    // Check to make sure we don't have a duplicate here.
                    if (ModSettings.propList.ContainsKey(thisProp.name))
                    {
                        Debugging.Message("duplicate prop name " + thisProp.name);
                    }
                    else
                    {
                        // All good - add to list.
                        ModSettings.propList.Add(thisProp.name, thisProp);
                    }
                }
            }

            // Set up options panel event handler.
            OptionsPanelManager.OptionsEventHook();
        }
    }
}