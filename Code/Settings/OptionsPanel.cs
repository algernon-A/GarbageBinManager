using UnityEngine;
using ColossalFramework.UI;


namespace GarbageBinManager
{
    /// <summary>
    /// Garbage Bin Manager options panel.
    /// </summary>
    public class GBMOptionsPanel : UIPanel
    {
        // Layout constants.
        private const float Margin = 5f;
        private const float LeftMargin = 24f;
        private const float CheckRowHeight = 22f;
        private const float DoubleMargin = 10f;
        private const float SliderHeight = 65f;
        private const float DropDownHeight = 80f;

        // Panel components.
        private UIDropDown propSelection;
        private UILabel noPropLabel;
        private UISlider rangeSlider;
        private UISlider thresholdSlider;
        private UISlider capacitySlider;
        private UISlider maxSlider;
        private UISlider xPosSlider;
        private UISlider zPosSlider;
        private UISlider spaceSlider;
        private UICheckBox rotationCheck;
        private UICheckBox fromRightCheck;
        private UICheckBox hideCheck;


        /// <summary>
        /// Performs initial setup for the panel; we don't use Start() as that's not sufficiently reliable (race conditions), and is not needed with the dynamic create/destroy process.
        /// </summary>
        internal void Setup()
        {
            // Basic setup.
            autoLayout = false;

            // Y position indicator.
            float currentY = Margin;

            // Language selection.
            UIDropDown languageDropDown = UIControls.AddPlainDropDown(this,LeftMargin, currentY, Translations.Translate("TRN_CHOICE"), Translations.LanguageList, Translations.Index);
            languageDropDown.eventSelectedIndexChanged += (control, index) =>
            {
                Translations.Index = index;
                OptionsPanelManager.LocaleChanged();
            };
            currentY += DropDownHeight;

            // Spacer.
            UIControls.OptionsSpacer(this, LeftMargin, currentY, this.width - (LeftMargin * 2f));
            currentY += DoubleMargin;

            // Checkbox to hide all bins.
            hideCheck = UIControls.AddPlainCheckBox(this, Translations.Translate("GBM_OPT_HIDE"));
            hideCheck.relativePosition = new Vector2(LeftMargin, currentY);
            hideCheck.isChecked = ModSettings.hideBins;
            hideCheck.eventCheckChanged += (control, isChecked) => { ModSettings.hideBins = isChecked; UpdateVisibility(); };
            currentY += CheckRowHeight + Margin;

            // Spacer.
            UIControls.OptionsSpacer(this, LeftMargin, currentY, this.width - (LeftMargin * 2f));
            currentY += DoubleMargin;

            // Prop selection.
            if (BinUtils.binList == null)
            {
                // If the dictionary hasn't been initialised yet, then we're not in-game; display message instead.
                noPropLabel = UIControls.AddLabel(this, LeftMargin, currentY + DoubleMargin, Translations.Translate("GBM_OPT_NOGAME"), textScale: 1.2f);
            }
            else
            {
                // We have a dictionary (game has loaded); create dropdown, populate with our prop list, and add 'Random' to the end.
                propSelection = UIControls.AddPlainDropDown(this, LeftMargin, currentY, Translations.Translate("GBM_OPT_PROP"), BinUtils.DisplayPropList, BinUtils.binList.IndexOfValue(BinUtils.currentBin));

                // Event handler.
                propSelection.eventSelectedIndexChanged += (control, index) =>
                {
                    BinUtils.currentBin = BinUtils.binList.Values[index];
                };
            }
            currentY += DropDownHeight;

            // Sliders for render range, spacing between bins, and forward offset of bins.
            rangeSlider = UIControls.AddSliderWithValue(this, LeftMargin, currentY, Translations.Translate("GBM_OPT_DIST"), 100f, 1000f, 10f, ModSettings.renderRange, (value) => { ModSettings.renderRange = value; });
            currentY += SliderHeight;

            thresholdSlider = UIControls.AddSliderWithValue(this, LeftMargin, currentY, Translations.Translate("GBM_OPT_THLD"), 0f, 2000f, 100f, ModSettings.binThreshold, (value) => { ModSettings.binThreshold = value; });
            currentY += SliderHeight + (thresholdSlider.relativePosition.y - 28f);

            capacitySlider = UIControls.AddSliderWithValue(this, LeftMargin, currentY, Translations.Translate("GBM_OPT_CAP"), 100f, 2000f, 100f, ModSettings.binCapacity, (value) => { ModSettings.binCapacity = value; });
            currentY += SliderHeight + (capacitySlider.relativePosition.y - 28f);

            maxSlider = UIControls.AddSliderWithValue(this, LeftMargin, currentY, Translations.Translate("GBM_OPT_MAX"), 1f, 50, 1f, ModSettings.maxBins, (value) => { ModSettings.maxBins = (int)value; });
            currentY += SliderHeight;

            xPosSlider = UIControls.AddSliderWithValue(this, LeftMargin, currentY, Translations.Translate("GBM_OPT_XPOS"), 0f, 6f, 0.1f, ModSettings.binXOffset, (value) => { ModSettings.binXOffset = value; });
            currentY += SliderHeight;

            zPosSlider = UIControls.AddSliderWithValue(this, LeftMargin, currentY, Translations.Translate("GBM_OPT_ZPOS"), 0f, 6f, 0.1f, ModSettings.binZOffset, (value) => { ModSettings.binZOffset = value; });
            currentY += SliderHeight;

            spaceSlider = UIControls.AddSliderWithValue(this, LeftMargin, currentY, Translations.Translate("GBM_OPT_SPAC"), 0.4f, 4f, 0.1f, ModSettings.binSpacing, (value) => { ModSettings.binSpacing = value; });
            currentY += SliderHeight;

            // Random rotation checkbox.
            rotationCheck = UIControls.AddPlainCheckBox(this, Translations.Translate("GBM_OPT_ROT"));
            rotationCheck.relativePosition = new Vector2(LeftMargin, currentY);
            rotationCheck.isChecked = ModSettings.randomRot;
            rotationCheck.eventCheckChanged += (control, isChecked) => { ModSettings.randomRot = isChecked; };
            currentY += CheckRowHeight;

            // Put out bins from right corner instead of left.
            fromRightCheck = UIControls.AddPlainCheckBox(this, Translations.Translate("GBM_OPT_RIGHT"));
            fromRightCheck.relativePosition = new Vector2(LeftMargin, currentY);
            fromRightCheck.isChecked = ModSettings.fromRight;
            fromRightCheck.eventCheckChanged += (control, isChecked) => { ModSettings.fromRight = isChecked; };

            // Update visibility.
            UpdateVisibility();
        }


        /// <summary>
        /// Shows or hides controls based on state of 'hide bins' checkbox.
        /// </summary>
        private void UpdateVisibility()
        {
            // Get local reference.
            bool shown = !hideCheck.isChecked;

            // Set visibility.
            rangeSlider.parent.isVisible = shown;
            thresholdSlider.parent.isVisible = shown;
            capacitySlider.parent.isVisible = shown;
            maxSlider.parent.isVisible = shown;
            xPosSlider.parent.isVisible = shown;
            zPosSlider.parent.isVisible = shown;
            spaceSlider.parent.isVisible = shown;
            rotationCheck.isVisible = shown;
            fromRightCheck.isVisible = shown;

            // Dropdown box or in-game only label - these may or may not exist, so need explicit null checks.
            if (noPropLabel != null)
            {
                noPropLabel.isVisible = shown;
            }
            if (propSelection != null)
            {
                propSelection.parent.isVisible = shown;
            }
        }
    }
}