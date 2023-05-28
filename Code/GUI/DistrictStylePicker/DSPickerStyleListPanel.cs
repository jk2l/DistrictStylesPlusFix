using ColossalFramework.UI;
using DistrictStylesPlus.Code.Utils;
using UnityEngine;

namespace DistrictStylesPlus.Code.GUI.DistrictStylePicker
{
    public class DSPickerStyleListPanel : UIPanel
    {

        private UIFastList _styleSelect;
        
        public static DSPickerStyleListPanel instance { get; private set; }

        public override void Start()
        {
            base.Start();

            instance = this;
            
            _styleSelect = UIFastList.Create<DSPickerStyleItem>(this);
            _styleSelect.backgroundSprite = "UnlockingPanel";
            _styleSelect.width = width;
            _styleSelect.height = height - 40;
            _styleSelect.canSelect = true;
            _styleSelect.rowHeight = 40;
            _styleSelect.autoHideScrollbar = true;
            _styleSelect.relativePosition = Vector3.zero;

            _styleSelect.rowsData = DSPDistrictStylesUtils.StoredDistrictStyles;
        }

        internal void RefreshPickerStyleSelect()
        {
            _styleSelect.Refresh();
        }

        internal void RefreshStoredDistrictStyles()
        {
            _styleSelect.rowsData = DSPDistrictStylesUtils.StoredDistrictStyles;
        }
    }
}