// <copyright file="BinUtils.cs" company="algernon (K. Algernon A. Sheppard)">
// Copyright (c) algernon (K. Algernon A. Sheppard). All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
// </copyright>

namespace GarbageBinManager
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using AlgernonCommons.Utils;
    using ColossalFramework.Math;

    /// <summary>
    /// Utility class for dealing with garbage bins.
    /// </summary>
    internal static class BinUtils
    {
        // Internal references.
        private static string currentBinName;
        internal static BinRecord currentBin;
        internal static SortedList<string, BinRecord> binList;

        /// <summary>
        /// Current bin name.
        /// </summary>
        internal static string CurrentBinName
        {
            get => currentBin?.binProp?.name ?? currentBinName ?? "random";

            set => currentBinName = value;
        }

        /// <summary>
        /// Provides a list of prop prefab names, cleaned up for UI display.
        /// </summary>
        internal static string[] DisplayPropList => Array.ConvertAll(binList.Keys.ToArray(), propName => PrefabUtils.GetDisplayName(propName));

        /// <summary>
        /// Returns a random garbage bin prop from our list, NOT INCLUDING the vanilla garbage bin.
        /// </summary>
        /// <param name="randomizer">Randomizer instance.</param>
        /// <returns>Random bin prop</returns>
        internal static BinRecord GetRandomBin(Randomizer randomizer) => binList.Values[randomizer.Int32((uint)binList.Count - 2) + 1];

        /// <summary>
        /// Sets the current bin based on the name in currentBinName.
        /// </summary>
        internal static void SetCurrentBin()
        {
            // Ensure a name is set and that it corresponds to a loaded bin.
            if (currentBinName != null && binList.ContainsKey(currentBinName))
            {
                currentBin = binList[currentBinName];
            }
        }
    }

    /// <summary>
    /// Custom bin list.
    /// </summary>
    internal static class CustomBins
    {
        internal static Dictionary<String, float> binList = new Dictionary<string, float>
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
            { "2055162053.dlx - Yellow Bin_Data", 0f }
        };
    }

    /// <summary>
    /// Class to hold info pertaining to a specific bin.
    /// Class, because it is nullable and mutable, and Mutable Structs Are Evil.
    /// </summary>
    public class BinRecord
    {
        public PropInfo binProp;
        public float rotation;
    }
}