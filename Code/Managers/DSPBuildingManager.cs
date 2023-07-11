using ColossalFramework;
using DistrictStylesPlus.Code.Utils;
using HarmonyLib;
using System;

namespace DistrictStylesPlus.Code.Managers
{
    
    /// <summary>
    /// Enhanced Building Manager for DSP
    /// </summary>
    public class DSPBuildingManager : Singleton<DSPBuildingManager>
    {

        /// <summary>
        /// When Styles list is modified, it needs to be refreshed in BuildingManager
        /// </summary>
        internal void RefreshStylesInBuildingManager()
        {
            // flag we are starting to refresh buildings
            Traverse.Create(Singleton<BuildingManager>.instance).Field("m_buildingsRefreshed").SetValue(false);
            
            var districtStyles = Singleton<DistrictManager>.instance.m_Styles;
            
            Logging.DebugLog($"Styles here: {districtStyles} - count styles: {districtStyles.Length}");
            
            // there is no dictionary for default style (vanilla buildings -> styleId = 0)
            var mStyleBuildingsDictSize = districtStyles.Length;
            
            Logging.DebugLog($"How many we want to create {mStyleBuildingsDictSize}");
            
            Singleton<BuildingManager>.instance.InitializeStyleArray(mStyleBuildingsDictSize);
            
            for (var i = 0; i < mStyleBuildingsDictSize; i++)
            {
                var districtStyle = districtStyles[i];
                var buildingInfos = districtStyle.GetBuildingInfos();
                var buildingIndices = districtStyle.GetBuildingIndices(buildingInfos);
                
                Logging.DebugLog($"Refreshing id {i} style {districtStyle.Name} infos {buildingInfos.Length} indices {buildingIndices.Length}");


                object[] p = new object[3];
                p[0] = buildingInfos;
                p[1] = buildingIndices;
                p[2] = i + 1;
                
                var m = AccessTools.Method(typeof(BuildingManager),"ApplyRefreshBuildingsStyles");
                Logging.DebugLog("INVOKING ApplyRefreshBuildingStyles");
                m.Invoke(Singleton<BuildingManager>.instance, p);
            }

            // flag we are starting to refresh buildings
            Traverse.Create(Singleton<BuildingManager>.instance).Field("m_buildingsRefreshed").SetValue(true);
            Logging.DebugLog("Building manager buildings has been refreshed.");
        }
    }
}