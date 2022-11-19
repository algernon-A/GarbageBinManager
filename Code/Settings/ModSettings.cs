// <copyright file="ModSettings.cs" company="algernon (K. Algernon A. Sheppard)">
// Copyright (c) algernon (K. Algernon A. Sheppard). All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
// </copyright>

namespace GarbageBinManager
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Xml.Serialization;
    using AlgernonCommons;
    using AlgernonCommons.XML;

    /// <summary>
    /// Class to hold global mod settings.
    /// </summary>
    [XmlRoot(ElementName = "GarbageBinRemover", Namespace = "")]
    public class ModSettings : SettingsXMLBase
    {
        [XmlIgnore]
        private static readonly string SettingsFileName = "GarbageBinManager.xml";

        /// <summary>
        /// Gets or sets the configuration file version.
        /// </summary>
        [XmlAttribute("Version")]
        public int Version { get; set; } = 0;

        /// <summary>
        /// Gets or sets the configuration elements.
        /// </summary>
        [XmlArray("Configurations")]
        [XmlArrayItem("Configuration")]
        public GBRSettingElement[] SettingElements
        {
            // Write to XML.
            get
            {
                GBRSettingElement[] newElements = new GBRSettingElement[1];

                newElements[0] = new GBRSettingElement()
                {
                    Building = "global",
                    HideBins = RenderGarbageBins.HideBins,
                    PropName = BinUtils.CurrentBinName,
                    RenderRange = RenderGarbageBins.RenderRange,
                    BinThreshold = RenderGarbageBins.BinThreshold,
                    BinCapacity = RenderGarbageBins.BinCapacity,
                    MaxBins = RenderGarbageBins.MaxBins,
                    BinXOffset = RenderGarbageBins.BinXOffset,
                    BinZOffset = RenderGarbageBins.BinZOffset,
                    BinSpacing = RenderGarbageBins.BinSpacing,
                    FromRight = RenderGarbageBins.FromRight,
                    RandomRotation = RenderGarbageBins.RandomRotation,
                };

                return newElements;
            }

            // Read from XML.
            set
            {
                foreach (GBRSettingElement element in value)
                {
                    if (element.Building == "global")
                    {
                        RenderGarbageBins.HideBins = element.HideBins;
                        BinUtils.CurrentBinName = element.PropName;
                        RenderGarbageBins.RenderRange = element.RenderRange;
                        RenderGarbageBins.BinThreshold = element.BinThreshold;
                        RenderGarbageBins.BinCapacity = element.BinCapacity;
                        RenderGarbageBins.MaxBins = element.MaxBins;
                        RenderGarbageBins.BinXOffset = element.BinXOffset;
                        RenderGarbageBins.BinZOffset = element.BinZOffset;
                        RenderGarbageBins.BinSpacing = element.BinSpacing;
                        RenderGarbageBins.FromRight = element.FromRight;
                        RenderGarbageBins.RandomRotation = element.RandomRotation;
                    }
                }
            }
        }

        /// <summary>
        /// Load settings from XML file.
        /// </summary>
        internal static void Load()
        {
            try
            {
                // Check to see if configuration file exists.
                if (File.Exists(SettingsFileName))
                {
                    // Read it.
                    using (StreamReader reader = new StreamReader(SettingsFileName))
                    {
                        XmlSerializer xmlSerializer = new XmlSerializer(typeof(ModSettings));
                        if (!(xmlSerializer.Deserialize(reader) is ModSettings))
                        {
                            Logging.Error("couldn't deserialize settings file");
                        }
                    }
                }
                else
                {
                    Logging.Message("no settings file found");
                }
            }
            catch (Exception e)
            {
                Logging.LogException(e, "exception reading XML settings file");
            }
        }

        /// <summary>
        /// Save settings to XML file.
        /// </summary>
        internal static void Save()
        {
            try
            {
                // Pretty straightforward.  Serialisation is within GBRSettingsFile class.
                using (StreamWriter writer = new StreamWriter(SettingsFileName))
                {
                    XmlSerializer xmlSerializer = new XmlSerializer(typeof(ModSettings));
                    xmlSerializer.Serialize(writer, new ModSettings());
                }
            }
            catch (Exception e)
            {
                Logging.LogException(e, "exception saving XML settings file");
            }
        }

        /// <summary>
        /// Individual settings element.
        /// </summary>
        public class GBRSettingElement
        {
            /// <summary>
            /// Gets or sets the selected target building prefab name.
            /// </summary>
            [XmlAttribute("Prefab")]
            public string Building { get; set; } = string.Empty;

            /// <summary>
            /// Gets or sets a value indicating whether bins should be hidden.
            /// </summary>
            [XmlElement("HideBins")]
            public bool HideBins { get; set; } = false;

            /// <summary>
            /// Gets or sets the selected bin prefab name.
            /// </summary>
            [XmlElement("PropName")]
            public string PropName { get; set; } = string.Empty;

            /// <summary>
            /// Gets or sets the minimum garbage threshold at which bins will be displayed.
            /// </summary>
            [XmlElement("BinThreshold")]
            public float BinThreshold { get; set; } = 1000f;

            /// <summary>
            /// Gets or sets the maximum garbage capacity per bin (when this is full, a new bin will be put out).
            /// </summary>
            [XmlElement("BinCapacity")]
            public float BinCapacity { get; set; } = 1000f;

            /// <summary>
            /// Gets or sets the maximum number of garbage bins.
            /// </summary>
            [XmlElement("MaxBins")]
            public int MaxBins { get; set; } = 8;

            /// <summary>
            /// Gets or sets the maximum render range for generated bins.
            /// </summary>
            [XmlElement("RenderRange")]
            public float RenderRange { get; set; } = 500f;

            /// <summary>
            /// Gets or sets the bin starting position X-offset.
            /// </summary>
            [XmlElement("OffsetX")]
            public float BinXOffset { get; set; } = 0.4f;

            /// <summary>
            /// Gets or sets the bin starting position Z-offset.
            /// </summary>
            [XmlElement("OffsetZ")]
            public float BinZOffset { get; set; } = 0f;

            /// <summary>
            /// Gets or sets the spacing between bins.
            /// </summary>
            [XmlElement("Spacing")]
            public float BinSpacing { get; set; } = 0.4f;

            /// <summary>
            /// Gets or sets a value indicating whether bins should be put out from the right-hand side of the property, instead of the left (facing the property).
            /// </summary>
            [XmlElement("FromRight")]
            public bool FromRight { get; set; } = false;

            /// <summary>
            /// Gets or sets a value indicating whether bin rotations should be random (as opposed to all facing in the same direction).
            /// </summary>
            [XmlElement("RandomRotation")]
            public bool RandomRotation { get; set; } = false;
        }
    }
}