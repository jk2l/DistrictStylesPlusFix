using System;
using System.Collections.Generic;
using ColossalFramework;
using ColossalFramework.UI;
using DistrictStylesPlus.Code.GUI;
using DistrictStylesPlus.Code.GUI.DistrictStylePicker;
using DistrictStylesPlus.Code.Managers;
using DistrictStylesPlus.Code.Patches;
using DistrictStylesPlus.Code.Utils;
using Epic.OnlineServices.Presence;
using HarmonyLib;
using ICities;

namespace DistrictStylesPlus.Code
{
    /// <summary>
    /// Main loading class: the mod runs from here.
    /// </summary>
    public class Loading : LoadingExtensionBase
    {
        internal static bool IsModEnabled = false;
        internal static bool IsModLoaded = false;
        private static bool _harmonyLoaded = false;
        private static bool AllowImport = true;

        internal static bool CheckImportAllowed()
        {
            return AllowImport;
        }

        /// <summary>
        /// Called by the game when the mod is initialised at the start of the loading process.
        /// </summary>
        /// <param name="loading">Loading mode (e.g. game, editor, scenario, etc.)</param>
        public override void OnCreated(ILoading loading)
        {
            base.OnCreated(loading);

            // do not load mod if it is not in game mode
            if (loading.currentMode != AppMode.Game)
            {

                IsModEnabled = false;
                Logging.InfoLog("Not in Game mod -> skip mod activation");

                // Set harmonyLoaded flag to suppress Harmony warning when e.g. loading into editor.
                _harmonyLoaded = true;

                Patcher.UnpatchAll();
                return;
            }

            AllowImport = false;

            // Ensure that Harmony patches have been applied.
            _harmonyLoaded = Patcher.patched;
            if (!_harmonyLoaded)
            {
                IsModEnabled = false;
                Logging.ErrorLog("Harmony patches not applied; aborting");
                return;
            }

            // Mod is enabled - do nothing
            if (IsModEnabled) return;

            // Passed all checks - okay to load (if we haven't already fo some reason).
            IsModEnabled = true;
            Logging.InfoLog("v " + DistrictStylesPlusMod.version + " loading");
        }

        /// <summary>
        /// Called by the game when level loading is complete.
        /// </summary>
        /// <param name="mode">Loading mode (e.g. game, editor, scenario, etc.)</param>
        public override void OnLevelLoaded(LoadMode mode)
        {
            base.OnLevelLoaded(mode);

            // Check to see that Harmony 2 was properly loaded.
            if (!_harmonyLoaded)
            {
                // TODO: improve handling of issue when harmony not loaded
                throw new Exception("Harmony did not load properly!");
            }
            
            if (!IsModEnabled)
            {
                return;
            }

            string[] spriteNames = new string[ModderPackInfos.config.Length * 2];
            for (int i = 0; i < ModderPackInfos.config.Length; i++)
            {
                spriteNames[i*2]      = ModderPackInfos.config[i].spriteName;
                spriteNames[i*2 + 1]    = ModderPackInfos.config[i].spriteName + "Disabled";
            }

            ResourceLoader.CreateTextureAtlas("DspAtlas", spriteNames, "DistrictStylesPlusFix.Icons.");
            Singleton<DistrictStylesPlusManager>.instance.SetupDistrictStylesPlusManager();
            DistrictStyleEditorPanel.Initialize();
            DistrictStylesEditorButton.CreateDistrictStylesEditorButton();
            UIStylesSelectorReplacer.AddStyleSelectorToCityPanel();
            UIStylesSelectorReplacer.AddStylePickerToDistrictPanel();
            DistrictStylePickerPanel.Initialize();
            DSPTransientStyleManager.LoadDataFromSave();

            Logging.InfoLog("loading complete");
            
            IsModLoaded = true;
        }
    }
}