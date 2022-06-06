using System;
using UnityEngine;
using ICities;
using ColossalFramework.UI;
using ColossalFramework.Globalization;


namespace GarbageBinManager
{
    /// <summary>
    /// Garbage Bin Manager options panel.
    /// </summary>
    internal static class OptionsPanelManager
    {
        // Instance references.
        private static GameObject uiGameObject;
        private static GBMOptionsPanel panel;
        internal static GBMOptionsPanel Panel => panel;

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

                    // Create a base panel attached to our game object, perfectly overlaying the game options panel.
                    panel = uiGameObject.AddComponent<GBMOptionsPanel>();
                    panel.width = optionsPanel.width - 10f;
                    panel.height = 725f;
                    panel.clipChildren = false;

                    // Needed to ensure position is consistent if we regenerate after initial opening (e.g. on language change).
                    panel.relativePosition = new Vector2(10f, 10f);

                    // Set up and show panel.
                    Panel.Setup();
                }
            }
            catch (Exception e)
            {
                Logging.LogException(e, "exception creating options panel");
            }
        }


        /// <summary>
        /// Closes the panel by destroying the object (removing any ongoing UI overhead).
        /// </summary>
        private static void Close()
        {
            // Save settings first.
            ModSettings.Save();

            // Enforce C# garbage collection by setting to null.
            if (panel != null)
            {
                GameObject.Destroy(panel);
                panel = null;
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
                Logging.Error("couldn't find OptionsPanel");
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
}

