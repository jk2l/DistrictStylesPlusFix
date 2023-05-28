using CitiesHarmony.API;
using DistrictStylesPlus.Code.Utils;
using HarmonyLib;

namespace DistrictStylesPlus.Code.Patches
{
    /// <summary>
    /// Class to manage the mod's Harmony patches.
    /// </summary>
    public static class Patcher
    {

        // Unique harmony ID
        private const string HarmonyId = "com.github.jk2l.csl.DistrictStylesPlus";
        
        // flag
        internal static bool patched { get; private set; } = false;

        /// <summary>
        /// Apply all Harmony patches.
        /// </summary>
        public static void PatchAll()
        {
            // Don't do anything if already patched.
            if (patched) return;
            
            // Ensure Harmony is ready before patching.
            if (HarmonyHelper.IsHarmonyInstalled)
            {
                Logging.InfoLog("deploying Harmony patches");

                // Apply all annotated patches and update flag.
                var harmonyInstance = new Harmony(HarmonyId);
                harmonyInstance.PatchAll();
                patched = true;
            }
            else
            {
                Logging.ErrorLog("Harmony not ready");
            }
        }
        
        public static void UnpatchAll()
        {
            // Only unapply if patches applied.
            if (!patched) return;
            Logging.InfoLog("reverting Harmony patches");

            // Unapply patches, but only with our HarmonyID.
            var harmonyInstance = new Harmony(HarmonyId);
            harmonyInstance.UnpatchAll(HarmonyId);
            patched = false;
        }
    }
}