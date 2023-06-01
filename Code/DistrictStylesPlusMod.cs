using System;
using CitiesHarmony.API;
using ColossalFramework.UI;
using DistrictStylesPlus.Code.BuildingThemesImport;
using DistrictStylesPlus.Code.Patches;
using ICities;

namespace DistrictStylesPlus.Code
{
    public class DistrictStylesPlusMod : IUserMod
    {
        internal static string modName => "District Styles Plus";
        internal static string version => "0.6.2";

        public string Name => modName + " " + version;
        public string Description => "Enhances district styles functionality and extends DS configuration possibilities";

        /// <summary>
        /// Called by the game when the mod is enabled.
        /// </summary>
        public void OnEnabled()
        {
            // Apply Harmony patches via Cities Harmony.
            // Called here instead of OnCreated to allow the auto-downloader to do its work prior to launch.
            HarmonyHelper.DoOnHarmonyReady(Patcher.PatchAll);
            
            SettingsUtils.LoadSettings();
            
            // TODO: add attaching of options panel hook if any options panel ever exists
        }
        
        /// <summary>
        /// Called by the game when the mod is disabled.
        /// </summary>
        public void OnDisabled()
        {
            // Unapply Harmony patches via Cities Harmony.
            if (HarmonyHelper.IsHarmonyInstalled)
            {
                Patcher.UnpatchAll();
            }
        }

        public void OnSettingsUI(UIHelperBase uiHelperBase)
        {
            var basicModSettingsGroup = uiHelperBase.AddGroup("District Styles Plus");

            try
            {

                basicModSettingsGroup.AddCheckbox(
                    "Generate Debug Log", 
                    ModSettings.enableDebugLog, 
                    delegate(bool c)
                    {
                        ModSettings.enableDebugLog = c; 
                        SettingsUtils.SaveSettings();
                    });
                
                basicModSettingsGroup.AddCheckbox(
                    "Apply styles only for building levels which are included (vanilla behaviour)", 
                    ModSettings.checkServiceLevel, 
                    delegate(bool c)
                    {
                        ModSettings.checkServiceLevel = c; 
                        SettingsUtils.SaveSettings();
                    });

                basicModSettingsGroup.AddCheckbox(
                    "Show standalone District Styles Editor button (UUI button is always shown)", 
                    ModSettings.showDistrictStylesEditorButton, 
                    delegate(bool c)
                    {
                        ModSettings.showDistrictStylesEditorButton = c; 
                        SettingsUtils.SaveSettings();
                    });
            }
            catch (Exception)
            {
                basicModSettingsGroup.AddGroup("District Styles Plus is unable to read the DistrictStylesPlus.xml file\n" +
                               "that stores configuration of mod!\n" +
                               "To fix it, delete this file and restart the game:\n" +
                               "{Steam folder}\\steamapps\\common\\Cities_Skylines\\DistrictStylesPlus.xml");
            }
            
            if (Loading.CheckImportAllowed())
            {
                var importThemesGroup = uiHelperBase.AddGroup("Import Building Themes as Styles");

                var btFilePathField = 
                    importThemesGroup.AddTextfield("Path to BuildingThemes xml file", "", delegate(string text)
                        {
                            // nothing to do...
                        }) 
                        as UITextField;
                btFilePathField.width *= 2f;

                importThemesGroup.AddButton("Import Themes", delegate
                {
                    BuildingThemesImportManager.ImportBuildingThemes(btFilePathField.text);
                });
            }
            else
            {
                uiHelperBase.AddGroup("Import of Building Themes is possible only from main menu. (Before game load)");
            }
        }
        
    }
}