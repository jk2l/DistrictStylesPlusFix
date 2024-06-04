using System;
using DistrictStylesPlus.Code.Managers;
using HarmonyLib;
using UnityEngine;

namespace DistrictStylesPlus.Code.Patches
{
    
    [HarmonyPatch]
    public static class SavePanelPatch
    {
        
        /// <summary>
        /// Called to save the game (including auto save).
        /// To increase performance, district styles are only written on disk,
        /// when the game is saved.
        /// </summary>
        [HarmonyPrefix]
        [HarmonyPatch(typeof(SavePanel), "SaveGame", new Type[] { typeof(string), typeof(bool), typeof(bool) })]
        public static void SaveGamePrefix(SavePanel __instance, string saveName, bool useCloud, bool quitAfterSaving)
        {
            __instance.StartCoroutine(DSPDistrictStylePackageManager.SaveChangedAssetsOfDistrictStyleMetaData());
        }
        
    }
}