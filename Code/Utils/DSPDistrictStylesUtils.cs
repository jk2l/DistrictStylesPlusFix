﻿using System;
using System.Linq;
using ColossalFramework;
using DistrictStylesPlus.Code.Managers;
using ColossalFramework.Globalization;

namespace DistrictStylesPlus.Code.Utils
{
    public static class DSPDistrictStylesUtils
    {

        /// <summary>
        /// Get list of short names (without package name) of Styles.
        /// </summary>
        /// <returns>Array of district style names</returns>
        internal static string[] getDistrictStyleShortNames()
        {
            return Singleton<DistrictManager>.instance.m_Styles.Select(style => style.Name).ToArray();
        }

        public static FastList<object> StoredDistrictStyles
        {
            get
            {
                var districtStyles = Singleton<DistrictManager>.instance.m_Styles;

                var resultData = new FastList<object>();
                if (districtStyles.Length <= 0)
                    return resultData;

                foreach (var districtStyle in districtStyles)
                {
                    // transient style for district should not be shown
                    if (districtStyle.PackageName.Equals(DSPTransientStyleManager.TransientStylePackage))
                        continue;

                    if (districtStyle.BuiltIn)
                    {
                        var cutover = "BuiltinStyle";
                        var index = districtStyle.Name.IndexOf(cutover);
                        var builtin_name = districtStyle.Name.Substring(0, index);

                        SteamHelper.DLC dlc_id;
                        // Maybe scope creep over the year, there was no proper style in between old and new DLC naming format
                        if (builtin_name.Equals("European"))
                        {
                            dlc_id = SteamHelper.DLC.None;
                        }
                        else if (builtin_name.Equals("EuropeanSuburbia"))
                        {
                            dlc_id = SteamHelper.DLC.ModderPack3;
                        }
                        else if (builtin_name.Equals("ModderPack5"))
                        {
                            dlc_id = SteamHelper.DLC.ModderPack5;
                        }
                        else
                        {
                            dlc_id = (SteamHelper.DLC)Enum.Parse(typeof(SteamHelper.DLC), builtin_name);
                        }

                        // styles for not owned DLCs or CCP should not be shown
                        if (!SteamHelper.IsDLCOwned(dlc_id))
                            continue;

                        resultData.Add(districtStyle);
                    }
                    else
                    {
                        // Cities Skylines don't mark CCP that is not owned as BuiltIn
                        // Styles will still available to load but won't funtional make a hack around it
                        var cutover = "BuiltinStyle";
                        var index = districtStyle.Name.IndexOf(cutover);
                        if (index > "ModderPack".Length)
                        {
                            continue;
                        }

                        resultData.Add(districtStyle);
                    }
                }

                return resultData;
            }
        }

        public static string GetUserFriendlyName(DistrictStyle districtStyle)
        {
            if (!districtStyle.BuiltIn)
                return districtStyle.Name;

            Logging.DebugLog(districtStyle.Name);

            var cutover = "BuiltinStyle";
            var index = districtStyle.Name.IndexOf(cutover);
            var builtin_name = districtStyle.Name.Substring(0, index);
            string district_name;

            // Maybe scope creep over the year, there was no proper style in between old and new DLC naming format
            if (builtin_name.Equals("European"))
            {
                district_name = "European / Vanilla";
            }
            else if (builtin_name.Equals("EuropeanSuburbia"))
            {
                district_name = "European Suburbia";
            }
            else if (builtin_name.Equals("ModderPack5"))
            {
                district_name = Locale.Get("DLC_NAME", "ModernCityCenter");
            }
            else
            {
                district_name = Locale.Get("DLC_NAME", builtin_name);
            }
            return district_name;
        }

    }

}