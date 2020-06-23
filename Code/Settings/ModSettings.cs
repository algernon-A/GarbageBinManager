using System;
using System.Collections.Generic;
using System.Linq;
using ColossalFramework.Math;


namespace GarbageBinManager
{
    /// <summary>
    /// Static class to hold global mod settings.
    /// </summary>
    public static class ModSettings
    {
        // Defaults from game.
        public static float renderRange = 500f;
        public static float binThreshold = 1000f;
        public static float binCapacity = 1000f;
        public static int maxBins = 8;

        // Reasonable defaults to work with to suit Arnold J. Rimmer, Bsc. Ssc.'s bins.
        public static float binXOffset = 0.4f;
        public static float binZOffset = 0f;
        public static float binSpacing = 0.4f;
        public static bool randomRot = false;
        public static bool fromRight = false;

        // Internal references.
        internal static PropInfo currentBin;
        internal static SortedList<string, PropInfo> propList;


        // Provides a list of prop prefab names, cleaned up for UI display.
        internal static string[] DisplayPropList => Array.ConvertAll(propList.Keys.ToArray(), propName => GetDisplayName(propName));


        /// <summary>
        /// Returns a random garbage bin prop from our list, NOT INCLUDING the vanilla garbage bin.
        /// </summary>
        /// <param name="seed">Seed for randomiser</param>
        /// <returns>Random bin prop</returns>
        internal static PropInfo GetRandomProp(Randomizer randomizer) => propList.Values[randomizer.Int32((uint)propList.Count - 2) + 1];


        /// <summary>
        /// Returns the name of a prefab cleaned up for display.
        /// </summary>
        /// <param name="fullName">Raw prefab name</param>
        /// <returns></returns>
        private static string GetDisplayName(string fullName)
        {
            // Filter out leading package number and trailing '_Data'.
            return fullName.Substring(fullName.IndexOf('.') + 1).Replace("_Data", "");
        }
    }
}