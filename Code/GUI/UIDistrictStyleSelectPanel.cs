using ColossalFramework.UI;
using DistrictStylesPlus.Code.Managers;
using DistrictStylesPlus.Code.Utils;
using UnityEngine;
using static DistrictStylesPlus.Code.GUI.DistrictStyleEditorPanel;

namespace DistrictStylesPlus.Code.GUI
{
    public class UIDistrictStyleSelectPanel : UIPanel
    {
        private UIFastList _districtStyleSelect;
        private UIButton _addStyle;
        private UIButton _removeStyle;
        private UIButton _copyStyle;
        internal static DistrictStyle SelectedDistrictStyle;
        
        public static UIDistrictStyleSelectPanel Instance { get; private set; }
        
        public override void Start()
        {
            base.Start();

            Instance = this;
            
            SetupDistrictStyleSelect();
            SetupAddStyleButton();
            SetupRemoveStyleButton();
            SetupCopyStyleButton();
        }
        
        internal void RefreshDistrictStyleSelect()
        {
            _districtStyleSelect.rowsData = DSPDistrictStylesUtils.StoredDistrictStyles;
        }

        private void SetupAddStyleButton()
        {
            _addStyle = UIUtils.CreateButton(this);
            _addStyle.width = (width - Spacing) / 3;
            _addStyle.text = "New";
            _addStyle.relativePosition = new Vector3(0, _districtStyleSelect.height + Spacing);

            _addStyle.eventClick += (c, p) =>
            {
                UIView.PushModal(UINewStyleModal.Instance);
                UINewStyleModal.Instance.Show(true);
            };
        }
        
        private void SetupRemoveStyleButton()
        {
            _removeStyle = UIUtils.CreateButton(this);
            _removeStyle.width = (width - Spacing) / 3;
            _removeStyle.text = "Delete";
            _removeStyle.isEnabled = false;
            _removeStyle.relativePosition = new Vector3(_addStyle.width + Spacing/2, _districtStyleSelect.height + Spacing);

            _removeStyle.eventClick += (component, param) =>
            {
                ConfirmPanel.ShowModal("Delete District Style", 
                    $"Are you sure you want to delete '{SelectedDistrictStyle.Name}' style?",
                    (uiComponent, result) =>
                    {
                        if (result == 1) DSPDistrictStyleManager.DeleteDistrictStyle(SelectedDistrictStyle, false);
                        SelectedDistrictStyle = null;
                        UIBuildingSelectPanel.Instance.RefreshBuildingInfoSelectList();
                        RefreshDistrictStyleSelect();
                        component.isEnabled = false;
                    });
            };
        }

        private void SetupCopyStyleButton()
        {
            _copyStyle = UIUtils.CreateButton(this);
            _copyStyle.width = (width - Spacing) / 3;
            _copyStyle.text = "Copy";
            _copyStyle.relativePosition = new Vector3(width - _copyStyle.width, _districtStyleSelect.height + Spacing);

            _copyStyle.eventClick += (c, p) =>
            {
                UIView.PushModal(UICopyStyleModal.Instance);
                UICopyStyleModal.Instance.Show(true);
            };
        }

        private void SetupDistrictStyleSelect()
        {
            _districtStyleSelect = UIFastList.Create<UIDistrictStyleItem>(this);
            _districtStyleSelect.backgroundSprite = "UnlockingPanel";
            _districtStyleSelect.width = width;
            _districtStyleSelect.height = height - 40;
            _districtStyleSelect.canSelect = true;
            _districtStyleSelect.rowHeight = 40;
            _districtStyleSelect.autoHideScrollbar = true;
            _districtStyleSelect.relativePosition = Vector3.zero;

            _districtStyleSelect.rowsData = DSPDistrictStylesUtils.StoredDistrictStyles;

            _districtStyleSelect.eventSelectedIndexChanged += (component, value) =>
            {
                if (value == -1) return;
                
                SelectedDistrictStyle = _districtStyleSelect.selectedItem as DistrictStyle;
                _removeStyle.isEnabled = !SelectedDistrictStyle?.BuiltIn ?? false;

                UIBuildingSelectPanel.Instance.FilterAndRefreshBuildingInfoSelectList();
                UIBuildingSelectPanel.Instance.RefreshBuildingInfoSelectList();
            };
        }
        
        public static DistrictStyle GetSelectedDistrictStyle()
        {
            return SelectedDistrictStyle;
        }

        internal void RefreshDistrictStyleSelectList()
        {
            _districtStyleSelect.Refresh();
        }
        
    }
}