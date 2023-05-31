using System.Collections;

namespace DistrictStylesPlusFix.Code.Utils
{
    public enum Origin
    {
        All,
        Default,
        Custom
        // Cloned TODO: when cloning is implemented
    }

    public enum Status
    {
        All,
        Included,
        Excluded
    }

    public enum Category
    {
        None = -1,
        ResidentialLow = 0,
        ResidentialHigh,
        ResidentialEco, // gc
        CommercialLow,
        CommercialHigh,
        CommercialLeisure,
        CommercialTourism,
        CommercialEco, // gc
        Industrial,
        Farming,
        Forestry,
        Oil,
        Ore,
        Office,
        OfficeHightech, // gc
        ResidentialWallToWall, // pp
        CommercialWallToWall,  // pp
        OfficeWallToWall, // pp
        OfficeFinancial // fd
    }

    public class CategoryIcons
    {

        public static readonly string[] atlases = {
            "Thumbnails",
            "Thumbnails",
            "Thumbnails", // gc
            "Thumbnails",
            "Thumbnails",
            "Thumbnails",
            "Thumbnails",
            "Thumbnails", // gc
            "Thumbnails",
            "Ingame",
            "Ingame",
            "Ingame",
            "Ingame",
            "Thumbnails",
            "Thumbnails", //gc
            "Thumbnails",
            "Thumbnails",
            "Thumbnails",
            "Thumbnails"
        };

        public static readonly string[] spriteNames = {
            "ZoningResidentialLow",
            "ZoningResidentialHigh",
            "DistrictSpecializationSelfsufficient",
            "ZoningCommercialLow",
            "ZoningCommercialHigh",
            "DistrictSpecializationLeisure",
            "DistrictSpecializationTourist",
            "DistrictSpecializationOrganic",
            "ZoningIndustrial",
            "IconPolicyFarming",
            "IconPolicyForest",
            "IconPolicyOil",
            "IconPolicyOre",
            "ZoningOffice",
            "DistrictSpecializationHightech",
            "DistrictSpecializationResidentialWallToWall",
            "DistrictSpecializationCommercialWallToWall",
            "DistrictSpecializationOfficeWallToWall",
            "DistrictSpecializationOfficeFinancial"
        };

        public static readonly string[] tooltips = {
            "Low density residential",
            "High density residential",
            "Eco residential",
            "Low density commercial",
            "High density commercial",
            "Leisure commercial",
            "Tourism commercial",
            "Eco commercial",
            "Generic Industry",
            "Farming Industry",
            "Forest Industry",
            "Oil Industry",
            "Ore Industry",
            "Office",
            "Hightech office",
            "Residential wall2wall",
            "Commercial wall2wall",
            "Office wall2wall",
            "Office financial"
        };
    }

}