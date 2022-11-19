// <copyright file="BinUtils.cs" company="algernon (K. Algernon A. Sheppard)">
// Copyright (c) algernon (K. Algernon A. Sheppard). All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
// </copyright>

namespace GarbageBinManager
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using AlgernonCommons;
    using AlgernonCommons.Translation;
    using AlgernonCommons.Utils;
    using ColossalFramework.Math;

    /// <summary>
    /// Utility class for dealing with garbage bins.
    /// </summary>
    internal static class BinUtils
    {
        /// <summary>
        /// List of bins with non-standard rotations.
        /// </summary>
        private static readonly Dictionary<string, float> CustomRotations = new Dictionary<string, float>
        {
            { "1584085578.Trash bin 01_Data", 0f },
            { "1584085578.Trash bin 02_Data", 0f },
            { "1584085578.Trash bin 03_Data", 0f },
            { "2023319576.Korea food waste bin(blue)_Data", (float)(-Math.PI / 2) },
            { "2023319576.Korea food waste bin(green)_Data", (float)(-Math.PI / 2) },
            { "2023319576.Korea food waste bin(orange)_Data", (float)(-Math.PI / 2) },
            { "519417315.UK Wheelie Bin - Black_Data", 0f },
            { "519417802.UK Wheelie Bin - Green_Data", 0f },
            { "519417584.UK Wheelie Bin - Blue Lid_Data", 0f },
            { "2055162053.dlx - Paper Bin_Data", 0f },
            { "2055162053.dlx - Trash Bin_Data", 0f },
            { "2055162053.dlx - Yellow Bin_Data", 0f },
        };

        // Internal references.
        private static string s_currentBinName;

        /// <summary>
        /// Gets the list of available bins.
        /// </summary>
        internal static SortedList<string, BinRecord> BinList { get; private set; }

        /// <summary>
        /// Gets or sets the curren bin.
        /// </summary>
        internal static BinRecord CurrentBin { get; set; }

        /// <summary>
        /// Gets or sets the current bin name.
        /// </summary>
        internal static string CurrentBinName
        {
            get => CurrentBin?.BinProp?.name ?? s_currentBinName ?? "random";

            set => s_currentBinName = value;
        }

        /// <summary>
        /// Gets a list of prop prefab names, cleaned up for UI display.
        /// </summary>
        internal static string[] DisplayPropList => Array.ConvertAll(BinList.Keys.ToArray(), propName => PrefabUtils.GetDisplayName(propName));

        /// <summary>
        /// Returns a random garbage bin prop from our list, NOT INCLUDING the vanilla garbage bin.
        /// </summary>
        /// <param name="randomizer">Randomizer instance.</param>
        /// <returns>Random bin prop.</returns>
        internal static BinRecord GetRandomBin(Randomizer randomizer) => BinList.Values[randomizer.Int32((uint)BinList.Count - 2) + 1];

        /// <summary>
        /// Initializes bin handling.
        /// </summary>
        internal static void Initialize()
        {
            // Initialise our list of valid bin replacement props with intial 'random' item..
            BinList = new SortedList<string, BinRecord>
            {
                { "000." + Translations.Translate("GBM_PRP_RDM"), null },
            };

            // Iterate through all loaded props, looking for props that meet our requirements.
            int propCount = PrefabCollection<PropInfo>.LoadedCount();
            for (uint i = 0; i < propCount; ++i)
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
                    else if (CustomRotations.ContainsKey(thisProp.name))
                    {
                        rotation = CustomRotations[thisProp.name];
                    }
                    else
                    {
                        // Not a supported bin; carry on!
                        continue;
                    }

                    // Check to make sure we don't have a duplicate here.
                    if (BinList.ContainsKey(thisProp.name))
                    {
                        Logging.Message("duplicate prop name ", thisProp.name);
                    }
                    else
                    {
                        // All good - add to list.
                        BinList.Add(thisProp.name, new BinRecord { BinProp = thisProp, Rotation = rotation });
                    }
                }
            }

            // Set initial current bin.
            SetCurrentBin();
        }

        /// <summary>
        /// Sets the current bin based on the name in currentBinName.
        /// </summary>
        internal static void SetCurrentBin()
        {
            // Ensure a name is set and that it corresponds to a loaded bin.
            if (s_currentBinName != null && BinList.ContainsKey(s_currentBinName))
            {
                CurrentBin = BinList[s_currentBinName];
            }
        }

        /// <summary>
        /// Class to hold info pertaining to a specific bin.
        /// Class, because it is nullable and mutable, and Mutable Structs Are Evil.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:Fields should be private", Justification = "Simple data class")]
        public class BinRecord
        {
            /// <summary>
            /// Bin prop prefab.
            /// </summary>
            public PropInfo BinProp;

            /// <summary>
            /// Bin rotation.
            /// </summary>
            public float Rotation;
        }
    }
}