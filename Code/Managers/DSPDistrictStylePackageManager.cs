using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ColossalFramework;
using ColossalFramework.Packaging;
using DistrictStylesPlus.Code.GUI;
using DistrictStylesPlus.Code.Utils;
using HarmonyLib;

namespace DistrictStylesPlus.Code.Managers
{
    public static class DSPDistrictStylePackageManager
    {
        
        public const string VanillaPrefix = "System";

        /// <summary>
        /// District style meta datas for which assets have been added or removed. Uses district style name as key.
        /// </summary>
        private static readonly Dictionary<String, DistrictStyleMetaData> ModifiedDistrictStyleMetaDatas = 
            new Dictionary<String, DistrictStyleMetaData>();
        
        internal static void RemoveDistrictStylePackage(string dsFullName)
        {
            PackageManager.DisableEvents();
            ModifiedDistrictStyleMetaDatas.Remove(dsFullName);
            var dsAsset = PackageManager.FindAssetByName(dsFullName);
            PackageManager.Remove(dsAsset.package);
            File.Delete(dsAsset.package.packagePath);
            PackageManager.EnabledEvents();
        }
        
        /**
         * Enabled empty styles needs to be loaded to game too as they can contain only vanilla buildings, which
         * are loaded to the style only in the game
         */
        internal static void AddEmptyEnabledStylesToGame()
        {
            foreach (var districtStyleAsset in PackageManager.FilterAssets(UserAssetType.DistrictStyleMetaData))
            {
                if (districtStyleAsset == null || !districtStyleAsset.isEnabled) continue;
                
                var districtStyleMetaData = districtStyleAsset.Instantiate<DistrictStyleMetaData>();

                var styleExists = Singleton<DistrictManager>.instance.m_Styles
                    .Any(style => style.Name.Equals(districtStyleMetaData.name));

                if (styleExists) continue;
                
                var newStyle = new DistrictStyle(districtStyleMetaData.name, false);
                var styles = Singleton<DistrictManager>.instance.m_Styles.AddItem(newStyle);
                Singleton<DistrictManager>.instance.m_Styles = styles.ToArray();
            }
        }
        
        /**
         * Vanilla buildings has to be loaded to styles after game load. If style contains only vanilla buildings,
         * it has to be created in district manager.
         */
        internal static void LoadVanillaBuildingsToStyles()
        {
            foreach (var districtStyleAsset in PackageManager.FilterAssets(UserAssetType.DistrictStyleMetaData))
            {
                if (districtStyleAsset == null || !districtStyleAsset.isEnabled) continue;
                
                var districtStyleMetaData = districtStyleAsset.Instantiate<DistrictStyleMetaData>();
                var vanillaAssetsArray = districtStyleMetaData.assets?
                    .Where(assetName => assetName.StartsWith(VanillaPrefix + "."))
                    .ToArray();
                
                if (vanillaAssetsArray == null || vanillaAssetsArray.Length <= 0) continue;
                
                var districtStyle = Singleton<DistrictManager>.instance.m_Styles
                    .FirstOrDefault(ds => ds.Name.Equals(districtStyleMetaData.name));
                
                if (districtStyle == null) continue;

                foreach (var vanillaAssetName in vanillaAssetsArray)
                {
                    string buildingName = vanillaAssetName.Substring(
                        vanillaAssetName.IndexOf(".", StringComparison.Ordinal) + 1);
                    Logging.DebugLog($"Try to find building {buildingName} for style {districtStyle.Name}");
                    var buildingInfo = PrefabCollection<BuildingInfo>.FindLoaded(buildingName);
                    if (buildingInfo != null && !districtStyle.GetBuildingInfos().Contains(buildingInfo))
                    {
                        districtStyle.Add(buildingInfo);
                    }
                }
            }
        }

        /// <summary>
        /// It refreshes district style assets metadata.
        /// To save changes to disk, call <see cref="SaveChangedAssetsOfDistrictStyleMetaData"/>.
        /// </summary>
        /// <param name="districtStyle">District Style which metadata should be refreshed</param>
        internal static void RefreshDistrictStyleAssetsMetaData(DistrictStyle districtStyle)
        {
            var dsMeta = GetDistrictStyleMetaDataByName(districtStyle.Name);
            
            if (dsMeta == null)
            {
                Logging.ErrorLog($"District style metadata not found! (DS: ${districtStyle.Name})");
                return; // TODO: should we throw exception?
            }

            if (dsMeta.assets == null)
            {
                dsMeta.assets = new string[0];
            }

            var styleBuildingInfos = districtStyle.GetBuildingInfos();
            var newAssetList = new string[styleBuildingInfos.Length];

            if (styleBuildingInfos.Length > 0)
            {
                for (int i = 0; i < styleBuildingInfos.Length; i++)
                {
                    var assetName = GetPackageAssetName(styleBuildingInfos[i].name);
                    newAssetList[i] = assetName;
                }
            }

            dsMeta.assets = newAssetList;
            ModifiedDistrictStyleMetaDatas[districtStyle.Name] = dsMeta;
        }
        
        /// <summary>
        /// To save changes to disk, call <see cref="SaveChangedAssetsOfDistrictStyleMetaData"/>.
        /// </summary>
        internal static void AddAssetToDistrictStyleMetaData(string districtStyleName, string buildingInfoName)
        {
            var dsMeta = GetDistrictStyleMetaDataByName(districtStyleName);

            if (dsMeta == null)
            {
                Logging.DebugLog("district style metadata not found!");
                return; // TODO: should it be exception?
            }

            if (dsMeta.assets == null)
            {
                dsMeta.assets = new string[0];
            }

            var assetName = GetPackageAssetName(buildingInfoName);            
            if (!dsMeta.assets.Contains(assetName))
            {
                var newAssetList = new string[dsMeta.assets.Length + 1];
                dsMeta.assets.CopyTo(newAssetList, 0);

                Logging.DebugLog($"Add asset {assetName}");
                newAssetList[newAssetList.Length - 1] = assetName;
                dsMeta.assets = newAssetList;
                ModifiedDistrictStyleMetaDatas[districtStyleName] = dsMeta;
            }
        }
        
        /// <summary>
        /// To save changes to disk, call <see cref="SaveChangedAssetsOfDistrictStyleMetaData"/>.
        /// </summary>
        internal static void RemoveAssetFromDistrictStyleMetaData(string districtStyleName, string buildingInfoName)
        {            
            var dsMeta = GetDistrictStyleMetaDataByName(districtStyleName);
            if (dsMeta == null)
            {
                Logging.DebugLog("district style metadata not found!");
                return; // TODO: should it be exception?
            }

            if (dsMeta.assets == null)
            {
                dsMeta.assets = new string[0];
            }
            else
            {
                var assetName = GetPackageAssetName(buildingInfoName);
                for (int k = 0; k < dsMeta.assets.Length; k++)
                {
                    if (dsMeta.assets[k].Equals(assetName))
                    {
                        string[] array2 = new string[dsMeta.assets.Length - 1];
                        Array.Copy(dsMeta.assets, 0, array2, 0, k);
                        Array.Copy(dsMeta.assets, k + 1, array2, k, dsMeta.assets.Length - k - 1);
                        dsMeta.assets = array2;
                        ModifiedDistrictStyleMetaDatas[districtStyleName] = dsMeta;
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Save changes of the district style meta data performed by 
        /// <see cref="RemoveAssetFromDistrictStyleMetaData"/>,
        /// <see cref="AddAssetToDistrictStyleMetaData"/> and
        /// <see cref="RefreshDistrictStyleAssetsMetaData"/>.
        /// This is a slow operation, as it writes data to disk.
        /// Implemented as Unity Coroutine. 
        /// </summary>
        internal static IEnumerator SaveChangedAssetsOfDistrictStyleMetaData() 
        {
            // PackageManager.DisableEvents() gets already called by StylesHelper.SaveStyle

            if (ModifiedDistrictStyleMetaDatas.Count == 0) {
                yield break;
            }

            foreach (DistrictStyleMetaData dsMeta in ModifiedDistrictStyleMetaDatas.Values)
            {
                yield return 0;
                StylesHelper.SaveStyle(dsMeta, dsMeta.name, false);
            }
            ModifiedDistrictStyleMetaDatas.Clear();

            // PackageManager.EnabledEvents() gets already called by StylesHelper.SaveStyle
            // PackageManager.ForcePackagesChanged() does it have to be called? It can't be called in a async Task 
            // => If it is required, move it out of the async Task and wait for it to finish before calling..  
            yield return 0;
        }

        internal static IEnumerator SaveChangedAssetsOfDistrictStyleMetaDataAsync() 
        {
            // Use LoadSaveStatus just like the game does to save a game 
            AsyncTask task = (AsyncTask)(LoadSaveStatus.activeTask = Singleton<SimulationManager>.instance.AddAction(
                "Saving", DSPDistrictStylePackageManager.SaveChangedAssetsOfDistrictStyleMetaData()
            ));
            while (!task.completedOrFailed)
            {
                yield return 0;
            }
        }
        
        internal static string GetPackageAssetName(string buildingInfoName)
        {
            var assetName = buildingInfoName.Replace("_Data", "");
            // For vanilla buildings
            if (!BuildingInfoHelper.IsCustomAsset(buildingInfoName)) assetName = VanillaPrefix + "." + assetName;
            return assetName;
        }
        
        private static DistrictStyleMetaData GetDistrictStyleMetaDataByName(string dsName)
        {
            if (ModifiedDistrictStyleMetaDatas.ContainsKey(dsName)) {
                return ModifiedDistrictStyleMetaDatas.GetValueSafe(dsName);
            }

            DistrictStyleMetaData dsMeta = null;
            var dsMetaList = GetDistrictStyleMetaDataList();
            foreach (var dsMetaInfo in dsMetaList)
            {
                Logging.DebugLog($"Searching - expected name {dsName} " +
                                 $"... offered name {dsMetaInfo.name}");
                if (!dsMetaInfo.name.Equals(dsName)) continue;
                dsMeta = dsMetaInfo;
                break;
            }
            return dsMeta;
        }
        
        private static List<DistrictStyleMetaData> GetDistrictStyleMetaDataList()
        {
            List<DistrictStyleMetaData> list = new List<DistrictStyleMetaData>();
            foreach (Package.Asset item in PackageManager.FilterAssets(UserAssetType.DistrictStyleMetaData))
            {
                if (item != null)
                {
                    list.Add(item.Instantiate<DistrictStyleMetaData>());
                }
            }
            return list;
        }
        
    }
}