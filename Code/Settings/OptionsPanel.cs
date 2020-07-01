using System;
using UnityEngine;
using ICities;
using ColossalFramework.UI;
using ColossalFramework.Globalization;


namespace GarbageBinManager
{
    /// <summary>
    /// Class to handle the mod settings options panel.
    /// </summary>
    internal static class OptionsPanelManager
    {
        // Instance references.
        private static GameObject uiGameObject;
        private static GBMOptionsPanel _panel;
        internal static GBMOptionsPanel Panel => _panel;

        // Parent UI panel reference.
        private static UIScrollablePanel optionsPanel;
        private static UIPanel gameOptionsPanel;


        /// <summary>
        /// Options panel setup.
        /// </summary>
        /// <param name="helper">UIHelperBase parent</param>
        internal static void Setup(UIHelperBase helper)
        {
            // Set up tab strip and containers.
            optionsPanel = ((UIHelper)helper).self as UIScrollablePanel;
            optionsPanel.autoLayout = false;
        }


        /// <summary>
        /// Creates the panel object in-game and displays it.
        /// </summary>
        private static void Create()
        {
            try
            {
                // If no instance already set, create one.
                if (uiGameObject == null)
                {
                    // Give it a unique name for easy finding with ModTools.
                    uiGameObject = new GameObject("GBMOptionsPanel");
                    uiGameObject.transform.parent = optionsPanel.transform;

                    _panel = uiGameObject.AddComponent<GBMOptionsPanel>();

                    // Set up and show panel.
                    Panel.Setup(optionsPanel.width, optionsPanel.height);
                }
            }
            catch (Exception e)
            {
                Debugging.LogException(e);
            }
        }


        /// <summary>
        /// Closes the panel by destroying the object (removing any ongoing UI overhead).
        /// </summary>
        private static void Close()
        {
            // Save settings first.
            GBMSettingsFile.SaveSettings();

            // Enforce C# garbage collection by setting to null.
            if (_panel != null)
            {
                GameObject.Destroy(_panel);
                _panel = null;
            }

            if (uiGameObject != null)
            {
                GameObject.Destroy(uiGameObject);
                uiGameObject = null;
            }
        }


        /// <summary>
        /// Attaches an event hook to options panel visibility, to activate/deactivate our options panel as appropriate.
        /// Deactivating when not visible saves UI overhead and performance impacts, especially with so many UITextFields.
        /// </summary>
        internal static void OptionsEventHook()
        {
            // Get options panel instance.
            gameOptionsPanel = UIView.library.Get<UIPanel>("OptionsPanel");

            if (gameOptionsPanel == null)
            {
                Debugging.Message("couldn't find OptionsPanel");
            }
            else
            {
                // Simple event hook to enable/disable GameObject based on appropriate visibility.
                gameOptionsPanel.eventVisibilityChanged += (control, isVisible) =>
                {
                    if (isVisible)
                    {
                        Create();
                    }
                    else
                    {
                        Close();
                    }
                };

                // Recreate panel on system locale change.
                LocaleManager.eventLocaleChanged += LocaleChanged;
            }
        }


        /// <summary>
        /// Refreshes the options panel (destroys and rebuilds) on a locale change when the options panel is open.
        /// </summary>
        public static void LocaleChanged()
        {
            if (gameOptionsPanel != null && gameOptionsPanel.isVisible)
            {
                Close();
                Create();
            }
        }
    }


    /// <summary>
    /// Garbage Bin Manager options panel.
    /// </summary>
    public class GBMOptionsPanel : UIPanel
    {
        /// <summary>
        /// Performs initial setup for the panel; we don't use Start() as that's not sufficiently reliable (race conditions), and is not needed with the dynamic create/destroy process.
        /// </summary>
        internal void Setup(float width, float height)
        {
            // Size and placement.
            this.width = width - (this.relativePosition.x * 2);
            this.height = height - (this.relativePosition.y * 2);
            this.autoLayout = true;
            this.autoLayoutDirection = LayoutDirection.Vertical;

            // Add controls.
            Debugging.Message("creating options panel");

            // Prop selection.
            if (ModSettings.binList == null)
            {
                // If the dictionary hasn't been initialised yet, then we're not in-game; display message instead.
                UILabel noPropLabel = this.AddUIComponent<UILabel>();

                noPropLabel.textScale = 1.2f;
                noPropLabel.text = Translations.Translate("GBM_OPT_NOGAME");

                // Add vertical spacing margin.
                noPropLabel.autoSize = false;
                noPropLabel.height += 30f;
            }
            else
            {
                // We have a dictionary (game has loaded); create dropdown, populate with our prop list, and add 'Random' to the end.
                UIDropDown propSelection = PanelUtils.AddPlainDropDown(this, Translations.Translate("GBM_OPT_PROP"), ModSettings.DisplayPropList, ModSettings.binList.IndexOfValue(ModSettings.currentBin));

                // Event handler.
                propSelection.eventSelectedIndexChanged += (control, index) =>
                {
                    ModSettings.currentBin = ModSettings.binList.Values[index];
                };
            }

            // Sliders for render range, spacing between bins, and forward offset of bins.
            UISlider rangeSlider = PanelUtils.AddSliderWithValue(this, Translations.Translate("GBM_OPT_DIST"), 100f, 1000f, 10f, ModSettings.renderRange, (value) => { ModSettings.renderRange = value; });
            UISlider thresholdSlider = PanelUtils.AddSliderWithValue(this, Translations.Translate("GBM_OPT_THLD"), 0f, 2000f, 100f, ModSettings.binThreshold, (value) => { ModSettings.binThreshold = value; });
            UISlider capacitySlider = PanelUtils.AddSliderWithValue(this, Translations.Translate("GBM_OPT_CAP"), 100f, 2000f, 100f, ModSettings.binCapacity, (value) => { ModSettings.binCapacity = value; });
            UISlider maxSlider = PanelUtils.AddSliderWithValue(this, Translations.Translate("GBM_OPT_MAX"), 1f, 50, 1f, ModSettings.maxBins, (value) => { ModSettings.maxBins = (int)value; });
            UISlider xPosSlider = PanelUtils.AddSliderWithValue(this, Translations.Translate("GBM_OPT_XPOS"), 0f, 6f, 0.1f, ModSettings.binXOffset, (value) => { ModSettings.binXOffset = value; });
            UISlider zPosSlider = PanelUtils.AddSliderWithValue(this, Translations.Translate("GBM_OPT_ZPOS"), 0f, 6f, 0.1f, ModSettings.binZOffset, (value) => { ModSettings.binZOffset = value; });
            UISlider spaceSlider = PanelUtils.AddSliderWithValue(this, Translations.Translate("GBM_OPT_SPAC"), 0.4f, 4f, 0.1f, ModSettings.binSpacing, (value) => { ModSettings.binSpacing = value; });

            // Random rotation checkbox.
            UICheckBox rotationCheck = PanelUtils.AddPlainCheckBox(this, Translations.Translate("GBM_OPT_ROT"));
            rotationCheck.isChecked = ModSettings.randomRot;
            rotationCheck.eventCheckChanged += (control, isChecked) => { ModSettings.randomRot = isChecked; };

            // Put out bins from right corner instead of left.
            UICheckBox fromRightCheck = PanelUtils.AddPlainCheckBox(this, Translations.Translate("GBM_OPT_RIGHT"));
            fromRightCheck.isChecked = ModSettings.fromRight;
            fromRightCheck.eventCheckChanged += (control, isChecked) => { ModSettings.fromRight = isChecked; };

            // Language dropdown.
            PanelUtils.AddPanelSpacer(this);
            UIDropDown translationDropDown = PanelUtils.AddPlainDropDown(this, Translations.Translate("TRN_CHOICE"), Translations.LanguageList, Translations.Index);

            // Event handler.
            translationDropDown.eventSelectedIndexChanged += (control, index) =>
            {
                Translations.Index = index;
            };
        }
    }
}