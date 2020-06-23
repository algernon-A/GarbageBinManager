using UnityEngine;
using ColossalFramework;
using ColossalFramework.Math;
using HarmonyLib;
using System;

namespace GarbageBinManager
{
	/// <summary>
	/// Harmony PRefix patch to replace garbage bins.
	/// </summary>
	[HarmonyPatch(typeof(CommonBuildingAI))]
	[HarmonyPatch("RenderGarbageBins")]
	class BuildingUpgradedPatch
	{
		/// <summary>
		/// Harmony Prefix patch to replace garbage bins.
		/// </summary>
		/// <param name="__instance">Instance reference</param>
		/// <param name="cameraInfo">Current camera info</param>
		/// <param name="buildingID">Building instance ID</param>
		/// <param name="data">Building data</param>
		/// <param name="layerMask">Layer mask</param>
		/// <param name="instance">RenderManager instance</param>
		public static bool Prefix(CommonBuildingAI __instance, RenderManager.CameraInfo cameraInfo, ushort buildingID, ref Building data, int layerMask, ref RenderManager.Instance instance)
		{
			// Don't do anything if building's garbage buffer is below set threshold (vanilla threshold: 1,000).
			if (data.m_garbageBuffer < ModSettings.binThreshold)
			{
				// Don't continue on to original method.
				return false;
			}

			// Check that the builiding is within the set render range. If not, we don't go any further.
			// It's not the *exact* distance to the individual prop, but close enough for our purposes - saves having to do all the work to calculate the exact prop position and then decide that we're not going to render.
			if (!cameraInfo.CheckRenderDistance(data.m_position, ModSettings.renderRange))
			{
				// Don't continue on to original method.
				return false;
			}

			// Set up a randomizer based on the building ID.
			Randomizer randomizer = new Randomizer(buildingID);

			// Get bin from settings (random one with 'garbage' service if none selected).
			PropInfo renderProp = ModSettings.currentBin ?? ModSettings.GetRandomProp(randomizer);

			// If no prop returned, or prop prefab data layer doesn't match the current one, return.
			if (renderProp == null || (layerMask & (1 << renderProp.m_prefabDataLayer)) == 0)
			{
				// Don't continue on to original method.
				return false;
			}

			// Determine number of garbage bins required to meet the building's current garbage levels with each bin 'containing' up to the set threshold, to a maximum of whatever we've set.
			int numBins = Mathf.Min(ModSettings.maxBins, (int)(data.m_garbageBuffer / ModSettings.binCapacity));

			// Prop position vector.
			Vector3 vector = default(Vector3);

			// Set up each bin.
			for (int i = 0; i < numBins; ++i)
			{
				// Get prop variation.
				PropInfo variation = renderProp.GetVariation(ref randomizer);

				// Colour and scale variation.  Scaling is from game code
				Color color = variation.GetColor(ref randomizer);
				float scale = variation.m_minScale + (float)randomizer.Int32(10000u) * (variation.m_maxScale - variation.m_minScale) * 0.0001f;

				// Calculate bin angle depending on settings.
				float angle;
				if (ModSettings.randomRot)
				{
					// Choose a random rotation, in radians.
					angle = (float)randomizer.Int32((uint)(Math.PI * 2000d)) / 1000f;
				}
				else
				{
					// Default angle is building angle; rotate by 180 degrees (in Radians!) to suit Arnold J. Rimmer, Bsc. Ssc.'s wheelie bins (front to curb). 
					angle = data.m_angle + (float)Math.PI;
				}

				// X is across building width.  Start at front left corner (width * 4, 8m per square and we want half that) and inset by our set X offset.
				// Then, work our way accross increasing by bin width plus our minimum distance between bins, with an additional random distance between 0 and 20cm.
				vector.x = ((float)data.Width * 4f) - ModSettings.binXOffset - (i * (variation.m_generatedInfo.m_size.x + ModSettings.binSpacing)) + (randomizer.Int32(2000) * 0.0001f);

				// If we want right-justified instead, we simply invert the x vector.
				if (ModSettings.fromRight)
                {
					vector.x *= -1;
                }

				// Z is depth from front of building.  Start at front left corner (width * 4, 8m per square and we want half that) and inset the depth of the bin.
				// Add a random factor up to 20cm inside from that point, and then add the forward offset from settings
				vector.z = ((float)data.Length * 4f) - variation.m_generatedInfo.m_size.z - (randomizer.Int32(2000) * 0.0001f) + ModSettings.binZOffset;

				// Use game code to finalise vector (y is calculated from ground height).
				vector.y = 0f;
				vector = instance.m_dataMatrix0.MultiplyPoint(vector);
				vector.y = (float)(int)instance.m_extraData.GetUShort(64 + i) * 0.015625f;

				// Setup - note four-component vector with values from game code.
				Vector4 objectIndex = new Vector4(0.001953125f, 0.00260416674f, 0f, 0f);
				InstanceID id = default(InstanceID);
				id.Building = buildingID;

				// Render the prop.
				PropInstance.RenderInstance(cameraInfo, variation, id, vector, scale, angle, color, objectIndex, (data.m_flags & Building.Flags.Active) != 0);
			}

			// Don't continue on to original method.
			return false;
		}
	}
}

