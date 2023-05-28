// <copyright file="RenderGarbageBins.cs" company="algernon (K. Algernon A. Sheppard)">
// Copyright (c) algernon (K. Algernon A. Sheppard). All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
// </copyright>

namespace GarbageBinManager
{
    using System;
    using ColossalFramework.Math;
    using HarmonyLib;
    using UnityEngine;

    /// <summary>
    /// Harmony patch to replace garbage bins.
    /// </summary>
    [HarmonyPatch(typeof(CommonBuildingAI))]
    [HarmonyPatch("RenderGarbageBins")]
    public static class RenderGarbageBins
    {
        // Main toggle.
        private static bool s_hideBins = false;

        // Defaults from game.
        private static float s_renderRange = 500f;
        private static float s_binThreshold = 1000f;
        private static float s_binCapacity = 1000f;
        private static int s_maxBins = 8;

        // Reasonable defaults to work with to suit Arnold J. Rimmer, Bsc. Ssc.'s bins.
        private static float s_binXOffset = 0.4f;
        private static float s_binZOffset = 0f;
        private static float s_binSpacing = 0.4f;
        private static bool s_randomRot = false;
        private static bool s_fromRight = false;

        /// <summary>
        /// Gets or sets a value indicating whether bins should be hidden.
        /// </summary>
        internal static bool HideBins
        {
            get => s_hideBins; set => s_hideBins = value;
        }

        /// <summary>
        /// Gets or sets the minimum garbage threshold at which bins will be displayed.
        /// </summary>
        internal static float BinThreshold
        {
            get => s_binThreshold; set => s_binThreshold = value;
        }

        /// <summary>
        /// Gets or sets the maximum garbage capacity per bin (when this is full, a new bin will be put out).
        /// </summary>
        internal static float BinCapacity
        {
            get => s_binCapacity; set => s_binCapacity = value;
        }

        /// <summary>
        /// Gets or sets the maximum number of garbage bins.
        /// </summary>
        internal static int MaxBins
        {
            get => s_maxBins; set => s_maxBins = value;
        }

        /// <summary>
        /// Gets or sets the maximum render range for generated bins.
        /// </summary>
        internal static float RenderRange
        {
            get => s_renderRange; set => s_renderRange = value;
        }

        /// <summary>
        /// Gets or sets the bin starting position X-offset.
        /// </summary>
        internal static float BinXOffset
        {
            get => s_binXOffset; set => s_binXOffset = value;
        }

        /// <summary>
        /// Gets or sets the bin starting position Z-offset.
        /// </summary>
        internal static float BinZOffset
        {
            get => s_binZOffset; set => s_binZOffset = value;
        }

        /// <summary>
        /// Gets or sets the spacing between bins.
        /// </summary>
        internal static float BinSpacing
        {
            get => s_binSpacing; set => s_binSpacing = value;
        }

        /// <summary>
        /// Gets or sets a value indicating whether bins should be put out from the right-hand side of the property, instead of the left (facing the property).
        /// </summary>
        internal static bool FromRight
        {
            get => s_fromRight; set => s_fromRight = value;
        }

        /// <summary>
        /// Gets or sets a value indicating whether bin rotations should be random (as opposed to all facing in the same direction).
        /// </summary>
        internal static bool RandomRotation
        {
            get => s_randomRot; set => s_randomRot = value;
        }

        /// <summary>
        /// Pre-emptive Harmony prefix to replace garbage bins.
        /// </summary>
        /// <param name="cameraInfo">Current camera info.</param>
        /// <param name="buildingID">Building instance ID.</param>
        /// <param name="data">Building data.</param>
        /// <param name="layerMask">Layer mask.</param>
        /// <param name="instance">RenderManager instance.</param>
        /// <returns>Always false (never execute original game method).</returns>
        public static bool Prefix(RenderManager.CameraInfo cameraInfo, ushort buildingID, ref Building data, int layerMask, ref RenderManager.Instance instance)
        {
            // Don't do anything if building's garbage buffer is below set threshold (vanilla threshold: 1,000), or if hide all bins is checked.
            if (s_hideBins || data.m_garbageBuffer < s_binThreshold)
            {
                // Don't continue on to original method.
                return false;
            }

            // Check that the builiding is within the set render range. If not, we don't go any further.
            // It's not the *exact* distance to the individual prop, but close enough for our purposes - saves having to do all the work to calculate the exact prop position and then decide that we're not going to render.
            if (!cameraInfo.CheckRenderDistance(data.m_position, s_renderRange))
            {
                // Don't continue on to original method.
                return false;
            }

            // Set up a randomizer based on the building ID.
            Randomizer randomizer = new Randomizer(buildingID);

            // Get bin from settings (random one with 'garbage' service if none selected).
            BinUtils.BinRecord renderBin = BinUtils.CurrentBin ?? BinUtils.GetRandomBin(randomizer);
            PropInfo renderProp = renderBin.BinProp;

            // If no prop returned, or prop prefab data layer doesn't match the current one, return.
            if (renderProp == null || (layerMask & (1 << renderProp.m_prefabDataLayer)) == 0)
            {
                // Don't continue on to original method.
                return false;
            }

            // Determine number of garbage bins required to meet the building's current garbage levels with each bin 'containing' up to the set threshold, to a maximum of whatever we've set.
            int numBins = Mathf.Min(s_maxBins, (int)(data.m_garbageBuffer / s_binCapacity));

            // Prop position vector.
            Vector3 vector = default;

            // Set up each bin.
            for (int i = 0; i < numBins; ++i)
            {
                // Get prop variation.
                PropInfo variation = renderProp.GetVariation(ref randomizer);

                // Colour and scale variation.  Scaling is from game code
                Color color = variation.GetColor(ref randomizer);
                float scale = variation.m_minScale + ((float)randomizer.Int32(10000u) * (variation.m_maxScale - variation.m_minScale) * 0.0001f);

                // Calculate bin angle depending on settings.
                float angle;
                if (s_randomRot)
                {
                    // Choose a random rotation, in radians.
                    angle = (float)randomizer.Int32((uint)(Math.PI * 2000d)) / 1000f;
                }
                else
                {
                    // Default angle is building angle; rotate by 180 degrees (in Radians!) to suit Arnold J. Rimmer, Bsc. Ssc.'s wheelie bins (front to curb).
                    angle = data.m_angle + renderBin.Rotation;
                }

                // X is across building width.  Start at front left corner (width * 4, 8m per square and we want half that) and inset by our set X offset.
                // Then, work our way accross increasing by bin width plus our minimum distance between bins, with an additional random distance between 0 and 20cm.
                vector.x = ((float)data.Width * 4f) - s_binXOffset - (i * (variation.m_generatedInfo.m_size.x + s_binSpacing)) + (randomizer.Int32(2000) * 0.0001f);

                // If we want right-justified instead, we simply invert the x vector.
                if (s_fromRight)
                {
                    vector.x *= -1;
                }

                // Z is depth from front of building.  Start at front left corner (width * 4, 8m per square and we want half that) and inset the depth of the bin.
                // Add a random factor up to 20cm inside from that point, and then add the forward offset from settings
                vector.z = ((float)data.Length * 4f) - variation.m_generatedInfo.m_size.z - (randomizer.Int32(2000) * 0.0001f) + s_binZOffset;

                // Use game code to finalise vector (y is calculated from ground height).
                vector.y = 0f;
                vector = instance.m_dataMatrix0.MultiplyPoint(vector);
                vector.y = (float)(int)instance.m_extraData.GetUShort(64 + i) * 0.015625f;

                // Setup - note four-component vector with values from game code.
                Vector4 objectIndex = new Vector4(0.001953125f, 0.00260416674f, 0f, 0f);
                InstanceID id = default;
                id.Building = buildingID;

                // Render the prop.
                PropInstance.RenderInstance(cameraInfo, variation, id, vector, scale, angle, color, objectIndex, (data.m_flags & Building.Flags.Active) != 0);
            }

            // Don't continue on to original method.
            return false;
        }
    }
}