using System.Collections;

namespace DistrictStylesPlus.Code.GUI
{
    public class ModderPackInfos
    {
        public static readonly ModderPackInfo[] config = {
            new ModderPackInfo (
                "Art Deco",
                SteamHelper.ModderPackBitMask.Pack1,
                "DspAtlas",
                "ArtDecoIcon"
            ),
            new ModderPackInfo (
                "European Suburbia",
                SteamHelper.ModderPackBitMask.Pack3,
                "DspAtlas",
                "Modderpack3Icon"
            ),
            new ModderPackInfo (
                "University City",
                SteamHelper.ModderPackBitMask.Pack4,
                "DspAtlas",
                "Modderpack4Icon"
            ),
            new ModderPackInfo (
                "Modern City Center",
                SteamHelper.ModderPackBitMask.Pack5,
                "DspAtlas",
                "Modderpack5Icon"
            ),
            new ModderPackInfo (
                "Mid-Century Modern",
                SteamHelper.ModderPackBitMask.Pack11,
                "DspAtlas",
                "MidCenturyModernIcon"
            ),
            new ModderPackInfo (
                "Heart of Korea",
                SteamHelper.ModderPackBitMask.Pack14,
                "DspAtlas",
                "HeartOfKoreaIcon"
            ),
            new ModderPackInfo (
                "Shopping Malls",
                SteamHelper.ModderPackBitMask.Pack16,
                "DspAtlas",
                "ModderPack16Icon"
            ),
            new ModderPackInfo (
                "Africa in Miniature",
                SteamHelper.ModderPackBitMask.Pack18,
                "DspAtlas",
                "ModderPack18Icon"
            ),
            new ModderPackInfo (
                "Industrial Evolution",
                SteamHelper.ModderPackBitMask.Pack20,
                "DspAtlas",
                "ModderPack20Icon"
            ),
            new ModderPackInfo (
                "Brookyln and Queens",
                SteamHelper.ModderPackBitMask.Pack21,
                "DspAtlas",
                "ModderPack21Icon"
            ),
        };
    }

    public class ModderPackInfo
    {
        public readonly string name;
        public readonly SteamHelper.ModderPackBitMask bitmark;

        public readonly string atlas;
        public readonly string spriteName;

        public ModderPackInfo (string name, SteamHelper.ModderPackBitMask bitmark, string atlas, string spriteName)
        {
            this.name = name;
            this.bitmark = bitmark;
            this.atlas = atlas;
            this.spriteName = spriteName;
        }
    }
}