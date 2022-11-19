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
        // Main toggle.
        [XmlIgnore]
        internal static bool hideBins = false;

        // Defaults from game.
        [XmlIgnore]
        internal static float renderRange = 500f;
        [XmlIgnore]
        internal static float binThreshold = 1000f;
        [XmlIgnore]
        internal static float binCapacity = 1000f;
        [XmlIgnore]
        internal static int maxBins = 8;

        // Reasonable defaults to work with to suit Arnold J. Rimmer, Bsc. Ssc.'s bins.
        [XmlIgnore]
        internal static float binXOffset = 0.4f;
        [XmlIgnore]
        internal static float binZOffset = 0f;
        [XmlIgnore]
        internal static float binSpacing = 0.4f;
        [XmlIgnore]
        internal static bool randomRot = false;
        [XmlIgnore]
        internal static bool fromRight = false;

        [XmlIgnore]
        private static readonly string SettingsFileName = "GarbageBinManager.xml";

        // List of individual setting elements.
        [XmlIgnore]
        public List<GBRSettingElement> elementList = new List<GBRSettingElement>();

        // Version.
        [XmlAttribute("Version")]
        public int version = 0;

        // Configuration settings.
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
                    prefab = "global",
                    hideBins = hideBins,
                    propName = BinUtils.CurrentBinName,
                    renderRange = renderRange,
                    binThreshold = binThreshold,
                    binCapacity = binCapacity,
                    maxBins = maxBins,
                    binXOffset = binXOffset,
                    binZOffset = binZOffset,
                    binSpacing = binSpacing,
                    fromRight = fromRight,
                    randomRot = randomRot
                };

                return newElements;
            }

            // Read from XML.
            set
            {
                foreach (GBRSettingElement element in value)
                {
                    if (element.prefab == "global")
                    {
                        hideBins = element.hideBins;
                        BinUtils.CurrentBinName = element.propName;
                        renderRange = element.renderRange;
                        binThreshold = element.binThreshold;
                        binCapacity = element.binCapacity;
                        maxBins = element.maxBins;
                        binXOffset = element.binXOffset;
                        binZOffset = element.binZOffset;
                        binSpacing = element.binSpacing;
                        fromRight = element.fromRight;
                        randomRot = element.randomRot;
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
    }


    /// <summary>
    /// Individual settings element.
    /// </summary>
    public class GBRSettingElement
    {
        [XmlAttribute("Prefab")]
        public string prefab = string.Empty;

        [XmlElement("HideBins")]
        public bool hideBins = false;

        [XmlElement("PropName")]
        public string propName = string.Empty;

        [XmlElement("BinThreshold")]
        public float binThreshold = 1000f;

        [XmlElement("BinCapacity")]
        public float binCapacity = 1000f;

        [XmlElement("MaxBins")]
        public int maxBins = 8;

        [XmlElement("RenderRange")]
        public float renderRange = 500f;

        [XmlElement("OffsetX")]
        public float binXOffset = 0.4f;

        [XmlElement("OffsetZ")]
        public float binZOffset = 0f;

        [XmlElement("Spacing")]
        public float binSpacing = 0.4f;

        [XmlElement("FromRight")]
        public bool fromRight = false;

        [XmlElement("RandomRotation")]
        public bool randomRot = false;
    }
}