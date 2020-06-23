using System;
using System.IO;
using System.Collections.Generic;
using System.Xml.Serialization;


namespace GarbageBinManager
{
    /// <summary>
    /// XML settings file.
    /// </summary>
    [XmlRoot(ElementName = "GarbageBinRemover", Namespace = "", IsNullable = false)]
    public class GBMSettingsFile
    {
        [XmlIgnore]
        private static readonly string SettingsFileName = "GarbageBinManager.xml";

        // List of individual setting elements.
        [XmlIgnore]
        public List<GBRSettingElement> elementList = new List<GBRSettingElement>();

        // Version.
        [XmlAttribute("Version")]
        public int version = 0;

        // Global settings.
        [XmlArray("Configurations", IsNullable = false)]
        [XmlArrayItem("Configuration", IsNullable = false)]
        public GBRSettingElement[] settingElements
        {
            // Write to XML.
            get
            {
                GBRSettingElement[] newElements = new GBRSettingElement[1];

                newElements[0] = new GBRSettingElement() {
                    prefab = "global",
                    propName = ModSettings.currentBin?.name ?? "random",
                    renderRange = ModSettings.renderRange,
                    binThreshold = ModSettings.binThreshold,
                    binCapacity = ModSettings.binCapacity,
                    maxBins = ModSettings.maxBins,
                    binXOffset = ModSettings.binXOffset,
                    binZOffset = ModSettings.binZOffset,
                    binSpacing = ModSettings.binSpacing,
                    fromRight = ModSettings.fromRight,
                    randomRot = ModSettings.randomRot
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
                        ModSettings.currentBin = ModSettings.propList?[element.propName];
                        ModSettings.renderRange = element.renderRange;
                        ModSettings.binThreshold = element.binThreshold;
                        ModSettings.binCapacity = element.binCapacity;
                        ModSettings.maxBins = element.maxBins;
                        ModSettings.binXOffset = element.binXOffset;
                        ModSettings.binZOffset = element.binZOffset;
                        ModSettings.binSpacing = element.binSpacing;
                        ModSettings.fromRight = element.fromRight;
                        ModSettings.randomRot = element.randomRot;
                    }
                }
            }
        }


        /// <summary>
        /// Load settings from XML file.
        /// </summary>
        internal static void LoadSettings()
        {
            try
            {
                // Check to see if configuration file exists.
                if (File.Exists(SettingsFileName))
                {
                    // Read it.
                    using (StreamReader reader = new StreamReader(SettingsFileName))
                    {
                        XmlSerializer xmlSerializer = new XmlSerializer(typeof(GBMSettingsFile));
                        if (!(xmlSerializer.Deserialize(reader) is GBMSettingsFile gbrSettingsFile))
                        {
                            Debugging.Message("couldn't deserialize settings file");
                        }
                    }
                }
                else
                {
                    Debugging.Message("no settings file found");
                }
            }
            catch (Exception e)
            {
                Debugging.Message("exception reading XML settings file");
                Debugging.LogException(e);
            }
        }


        /// <summary>
        /// Save settings to XML file.
        /// </summary>
        internal static void SaveSettings()
        {
            try
            {
                // Pretty straightforward.  Serialisation is within GBRSettingsFile class.
                using (StreamWriter writer = new StreamWriter(SettingsFileName))
                {
                    XmlSerializer xmlSerializer = new XmlSerializer(typeof(GBMSettingsFile));
                    xmlSerializer.Serialize(writer, new GBMSettingsFile());
                }
            }
            catch (Exception e)
            {
                Debugging.Message("exception saving XML settings file");
                Debugging.LogException(e);
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