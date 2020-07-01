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
        internal static string currentBinName;
        internal static BinRecord currentBin;
        internal static SortedList<string, BinRecord> binList;


        // Provides a list of prop prefab names, cleaned up for UI display.
        internal static string[] DisplayPropList => Array.ConvertAll(binList.Keys.ToArray(), propName => GetDisplayName(propName));


        /// <summary>
        /// Returns a random garbage bin prop from our list, NOT INCLUDING the vanilla garbage bin.
        /// </summary>
        /// <param name="seed">Seed for randomiser</param>
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


    /// <summary>
    /// Custom bin list.
    /// </summary>
    internal static class CustomBins
    {
        internal static Dictionary<String,float> binList = new Dictionary<string, float>
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