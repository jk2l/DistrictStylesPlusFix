using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using ColossalFramework.Globalization;
using DistrictStylesPlusFix.Code.Utils;

namespace DistrictStylesPlus.Code.GUI
{
    /**
     * This is a helper to get data from BuildingInfo in UI friendly form
     */
    public static class BuildingInfoHelper
    {
        
        /**
         * Returns height of building in meters
         */
        internal static int GetBuildingHeight(BuildingInfo buildingInfo)
        {
            if (buildingInfo == null) return 0;

            if (buildingInfo.m_generatedInfo?.m_heights == null) return 0;

            if (buildingInfo.m_generatedInfo.m_heights.Length != 0)
            {
                return (int) buildingInfo.m_generatedInfo.m_heights.Max();
            }

            return 0;
        }

        /**
         * Checks if building is included in provided style
         */
        internal static bool IsBuildingIncludedInStyle(BuildingInfo buildingInfo, DistrictStyle districtStyle)
        {
            if (buildingInfo == null || districtStyle == null) return false;

            return districtStyle.Contains(buildingInfo);
        }

        /**
         * Returns more user friendly building name
         */
        internal static string GetDisplayName(string buildingInfoName)
        {
            // TODO: remove completely eventually
            //Logging.DebugLog($"Building Themes: Real building name is {buildingInfoName}");
            
            var displayName = Locale.GetUnchecked(LocaleID.BUILDING_TITLE, buildingInfoName);

            if (displayName.StartsWith(LocaleID.BUILDING_TITLE))
            {
                displayName = buildingInfoName
                    .Substring(buildingInfoName.IndexOf('.') + 1)
                    .Replace("_Data", "");
            }

            return CleanName(displayName, !buildingInfoName.Contains("."));
        }

        /**
         * Returns building category - which correspond to the building subService. It is used in filtering.
         */
        internal static Category GetBuildingCategory(BuildingInfo buildingInfo)
        {
            if (buildingInfo == null || buildingInfo.m_class == null) return Category.None;

            var itemClass = buildingInfo.m_class;
            switch (itemClass.m_subService)
            {
                case ItemClass.SubService.ResidentialLow:
                    return Category.ResidentialLow;
                case ItemClass.SubService.ResidentialHigh:
                    return Category.ResidentialHigh;
                case ItemClass.SubService.ResidentialLowEco:
                case ItemClass.SubService.ResidentialHighEco:
                    return Category.ResidentialEco;
                case ItemClass.SubService.CommercialLow:
                    return Category.CommercialLow;
                case ItemClass.SubService.CommercialHigh:
                    return Category.CommercialHigh;
                case ItemClass.SubService.CommercialLeisure:
                    return Category.CommercialLeisure;
                case ItemClass.SubService.CommercialTourist:
                    return Category.CommercialTourism;
                case ItemClass.SubService.CommercialEco:
                    return Category.CommercialEco;
                case ItemClass.SubService.IndustrialGeneric:
                    return Category.Industrial;
                case ItemClass.SubService.IndustrialFarming:
                    return Category.Farming;
                case ItemClass.SubService.IndustrialForestry:
                    return Category.Forestry;
                case ItemClass.SubService.IndustrialOil:
                    return Category.Oil;
                case ItemClass.SubService.IndustrialOre:
                    return Category.Ore;
                case ItemClass.SubService.OfficeGeneric:
                    return Category.Office;
                case ItemClass.SubService.OfficeHightech:
                    return Category.OfficeHightech;
                case ItemClass.SubService.ResidentialWallToWall:
                    return Category.ResidentialWallToWall;
                case ItemClass.SubService.CommercialWallToWall:
                    return Category.CommercialWallToWall;
                case ItemClass.SubService.OfficeWallToWall:
                    return Category.OfficeWallToWall;
                case ItemClass.SubService.OfficeFinancial:
                    return Category.OfficeFinancial;
                default:
                    return Category.None;
            }
        }

        /**
         * Return building level number
         */
        internal static int GetLevelNumber(BuildingInfo buildingInfo)
        {
            int level = 0;
            if (buildingInfo != null && buildingInfo.m_class != null)
            {
                level = (int) buildingInfo.m_class.m_level + 1;
            }
            else
            {
                string cleanName = Regex.Replace(buildingInfo.name, @"^{{.*?}}\.", "");
                int.TryParse(Regex.Match(cleanName, @"(?<=[HL])\d").Value, out level);
            }

            return level;
        }

        /**
         * Returns building footprint dimension as string AxB
         */
        internal static string GetFootprintDimension(BuildingInfo buildingInfo)
        {
            return $"{buildingInfo.m_cellWidth}x{buildingInfo.m_cellLength}";
        }

        /**
         * Returns steamId if building is from workshop - otherwise null
         */
        internal static string GetSteamId(string buildingInfoName)
        {
            if (!IsCustomAsset(buildingInfoName)) return null;
            
            var steamId = buildingInfoName.Substring(0, buildingInfoName.IndexOf("."));
            
            if (!ulong.TryParse(steamId, out var controlResult) || controlResult == 0)
            {
                return null;
            }

            return steamId;

        }

        internal static bool IsCustomAsset(string buildingInfoName)
        {
            return Regex.Replace(buildingInfoName, @"^{{.*?}}\.", "").Contains(".");
        }

        private static string CleanName(string name, bool cleanNumbers = false)
        {
            name = Regex.Replace(name, @"^{{.*?}}\.", "");
            name = Regex.Replace(name, @"[_+\.]", " ");
            name = Regex.Replace(name, @"(\d[xX]\d)|([HL]\d)", "");
            if (cleanNumbers)
            {
                name = Regex.Replace(name, @"(\d+[\da-z])", "");
                name = Regex.Replace(name, @"\s\d+", " ");
            }
            name = Regex.Replace(name, @"\s+", " ").Trim();

            return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(name);
        }
    }
}