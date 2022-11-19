// <copyright file="OptionsPanel.cs" company="algernon (K. Algernon A. Sheppard)">
// Copyright (c) algernon (K. Algernon A. Sheppard). All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
// </copyright>

namespace GarbageBinManager
{
    using AlgernonCommons.Translation;
    using AlgernonCommons.UI;
    using ColossalFramework.UI;
    using UnityEngine;

    /// <summary>
    /// Garbage Bin Manager options panel.
    /// </summary>
    public class OptionsPanel : UIPanel
    {
        // Layout constants.
        private const float Margin = 5f;
        private const float LeftMargin = 24f;
        private const float CheckRowHeight = 22f;
        private const float DoubleMargin = 10f;
        private const float SliderHeight = 65f;
        private const float DropDownHeight = 80f;

        // Panel components.
        private readonly UIDropDown _propSelection;
        private readonly UILabel _noPropLabel;
        private readonly UISlider _rangeSlider;
        private readonly UISlider _thresholdSlider;
        private readonly UISlider _capacitySlider;
        private readonly UISlider _maxSlider;
        private readonly UISlider _xPosSlider;
        private readonly UISlider _zPosSlider;
        private readonly UISlider _spaceSlider;
        private readonly UICheckBox _rotationCheck;
        private readonly UICheckBox _fromRightCheck;
        private readonly UICheckBox _hideCheck;

        /// <summary>
        /// Initializes a new instance of the <see cref="OptionsPanel"/> class.
        /// </summary>
        public OptionsPanel()
        {
            // Basic setup.
            autoLayout = false;

            // Y position indicator.
            float currentY = Margin;

            // Language selection.
            UIDropDown languageDropDown = UIDropDowns.AddPlainDropDown(this, LeftMargin, currentY, Translations.Translate("LANGUAGE_CHOICE"), Translations.LanguageList, Translations.Index);
            languageDropDown.eventSelectedIndexChanged += (control, index) =>
            {
                Translations.Index = index;
                OptionsPanelManager<OptionsPanel>.LocaleChanged();
            };
            currentY += DropDownHeight;

            // Spacer.
            UISpacers.AddOptionsSpacer(this, LeftMargin, currentY, this.width - (LeftMargin * 2f));
            currentY += DoubleMargin;

            // Checkbox to hide all bins.
            _hideCheck = UICheckBoxes.AddPlainCheckBox(this, Translations.Translate("GBM_OPT_HIDE"));
            _hideCheck.relativePosition = new Vector2(LeftMargin, currentY);
            _hideCheck.isChecked = RenderGarbageBins.HideBins;
            _hideCheck.eventCheckChanged += (control, isChecked) =>
            {
                RenderGarbageBins.HideBins = isChecked;
                UpdateVisibility();
            };
            currentY += CheckRowHeight + Margin;

            // Spacer.
            UISpacers.AddOptionsSpacer(this, LeftMargin, currentY, this.width - (LeftMargin * 2f));
            currentY += DoubleMargin;

            // Prop selection.
            if (BinUtils.BinList == null)
            {
                // If the dictionary hasn't been initialised yet, then we're not in-game; display message instead.
                _noPropLabel = UILabels.AddLabel(this, LeftMargin, currentY + DoubleMargin, Translations.Translate("GBM_OPT_NOGAME"), textScale: 1.2f);
            }
            else
            {
                // We have a dictionary (game has loaded); create dropdown, populate with our prop list, and add 'Random' to the end.
                _propSelection = UIDropDowns.AddPlainDropDown(this, LeftMargin, currentY, Translations.Translate("GBM_OPT_PROP"), BinUtils.DisplayPropList, BinUtils.BinList.IndexOfValue(BinUtils.CurrentBin), 400f);

                // Event handler.
                _propSelection.eventSelectedIndexChanged += (control, index) =>
                {
                    BinUtils.CurrentBin = BinUtils.BinList.Values[index];
                };
            }

            currentY += DropDownHeight;

            // Sliders for render range, spacing between bins, and forward offset of bins.
            _rangeSlider = UISliders.AddPlainSliderWithValue(this, LeftMargin, currentY, Translations.Translate("GBM_OPT_DIST"), 100f, 1000f, 10f, RenderGarbageBins.RenderRange);
            _rangeSlider.eventValueChanged += (c, value) => { RenderGarbageBins.RenderRange = value; };
            currentY += SliderHeight;

            _thresholdSlider = UISliders.AddPlainSliderWithValue(this, LeftMargin, currentY, Translations.Translate("GBM_OPT_THLD"), 0f, 2000f, 100f, RenderGarbageBins.BinThreshold);
            _thresholdSlider.eventValueChanged += (c, value) => { RenderGarbageBins.BinThreshold = value; };
            currentY += SliderHeight + (_thresholdSlider.relativePosition.y - 28f);

            _capacitySlider = UISliders.AddPlainSliderWithValue(this, LeftMargin, currentY, Translations.Translate("GBM_OPT_CAP"), 100f, 2000f, 100f, RenderGarbageBins.BinCapacity);
            _capacitySlider.eventValueChanged += (c, value) => { RenderGarbageBins.BinCapacity = value; };
            currentY += SliderHeight + (_capacitySlider.relativePosition.y - 28f);

            _maxSlider = UISliders.AddPlainSliderWithValue(this, LeftMargin, currentY, Translations.Translate("GBM_OPT_MAX"), 1f, 50, 1f, RenderGarbageBins.MaxBins);
            _maxSlider.eventValueChanged += (c, value) => { RenderGarbageBins.MaxBins = (int)value; };
            currentY += SliderHeight;

            _xPosSlider = UISliders.AddPlainSliderWithValue(this, LeftMargin, currentY, Translations.Translate("GBM_OPT_XPOS"), 0f, 6f, 0.1f, RenderGarbageBins.BinXOffset);
            _xPosSlider.eventValueChanged += (c, value) => { RenderGarbageBins.BinXOffset = value; };
            currentY += SliderHeight;

            _zPosSlider = UISliders.AddPlainSliderWithValue(this, LeftMargin, currentY, Translations.Translate("GBM_OPT_ZPOS"), 0f, 6f, 0.1f, RenderGarbageBins.BinZOffset);
            _zPosSlider.eventValueChanged += (c, value) => { RenderGarbageBins.BinZOffset = value; };
            currentY += SliderHeight;

            _spaceSlider = UISliders.AddPlainSliderWithValue(this, LeftMargin, currentY, Translations.Translate("GBM_OPT_SPAC"), 0.4f, 4f, 0.1f, RenderGarbageBins.BinSpacing);
            _spaceSlider.eventValueChanged += (c, value) => { RenderGarbageBins.BinSpacing = value; };
            currentY += SliderHeight;

            // Random rotation checkbox.
            _rotationCheck = UICheckBoxes.AddPlainCheckBox(this, Translations.Translate("GBM_OPT_ROT"));
            _rotationCheck.relativePosition = new Vector2(LeftMargin, currentY);
            _rotationCheck.isChecked = RenderGarbageBins.RandomRotation;
            _rotationCheck.eventCheckChanged += (control, isChecked) => { RenderGarbageBins.RandomRotation = isChecked; };
            currentY += CheckRowHeight;

            // Put out bins from right corner instead of left.
            _fromRightCheck = UICheckBoxes.AddPlainCheckBox(this, Translations.Translate("GBM_OPT_RIGHT"));
            _fromRightCheck.relativePosition = new Vector2(LeftMargin, currentY);
            _fromRightCheck.isChecked = RenderGarbageBins.FromRight;
            _fromRightCheck.eventCheckChanged += (control, isChecked) => { RenderGarbageBins.FromRight = isChecked; };

            // Update visibility.
            UpdateVisibility();
        }

        /// <summary>
        /// Shows or hides controls based on state of 'hide bins' checkbox.
        /// </summary>
        private void UpdateVisibility()
        {
            // Get local reference.
            bool shown = !_hideCheck.isChecked;

            // Set visibility.
            _rangeSlider.parent.isVisible = shown;
            _thresholdSlider.parent.isVisible = shown;
            _capacitySlider.parent.isVisible = shown;
            _maxSlider.parent.isVisible = shown;
            _xPosSlider.parent.isVisible = shown;
            _zPosSlider.parent.isVisible = shown;
            _spaceSlider.parent.isVisible = shown;
            _rotationCheck.isVisible = shown;
            _fromRightCheck.isVisible = shown;

            // Dropdown box or in-game only label - these may or may not exist, so need explicit null checks.
            if (_noPropLabel != null)
            {
                _noPropLabel.isVisible = shown;
            }

            if (_propSelection != null)
            {
                _propSelection.parent.isVisible = shown;
            }
        }
    }
}