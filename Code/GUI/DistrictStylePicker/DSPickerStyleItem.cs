using ColossalFramework.UI;
using DistrictStylesPlus.Code.Utils;
using UnityEngine;

namespace DistrictStylesPlus.Code.GUI.DistrictStylePicker
{
    public class DSPickerStyleItem : UIPanel, UIFastListRow
    {

        private UICheckBox m_name;
        private UIPanel m_background;
        private DistrictStyle m_districtStyle;
        
        public void Display(object data, bool isRowOdd)
        {

            m_districtStyle = data as DistrictStyle;
            SetupControls();

            if (m_districtStyle == null) return;
            
            m_name.text = DSPDistrictStylesUtils.GetUserFriendlyName(m_districtStyle); 
            m_name.label.textColor = new Color32(255, 255, 255, 255);
            m_name.label.isInteractive = false;
            m_name.isChecked = IsSelectedStyle(m_districtStyle.FullName);
        }

        public void Select(bool isRowOdd)
        {
            // do nothing
        }

        public void Deselect(bool isRowOdd)
        {
            // do nothing
        }

        private bool IsSelectedStyle(string fullName)
        {
            var selectedStyleFullNames = DistrictStylePickerPanel.instance.GetSelectedStyles();
            return selectedStyleFullNames.Contains(fullName);
        }

        private void SetupControls()
        {
            if (m_name != null) return;

            isVisible = true;
            canFocus = true;
            isInteractive = true;
            width = parent.width;
            height = 40;
            
            SetupBackgroundPanel();
            SetupNameCheckbox();
        }

        private void SetupNameCheckbox()
        {
            m_name = UIUtils.CreateCheckBox(this);
            m_name.width = 40;
            m_name.clipChildren = false;
            m_name.relativePosition = new Vector3(5, 13);

            m_name.eventCheckChanged += (component, state) =>
            {
                if (state)
                {
                    Logging.DebugLog($"Select DS {m_districtStyle.FullName}");
                    DistrictStylePickerPanel.instance.AddStyleToSelected(m_districtStyle);
                }
                else
                {
                    Logging.DebugLog($"Remove DS {m_districtStyle.FullName}");
                    DistrictStylePickerPanel.instance.RemoveStyleFromSelected(m_districtStyle);
                }
                
                DSPickerStyleListPanel.instance.RefreshPickerStyleSelect();
            };
        }
        
        private void SetupBackgroundPanel()
        {
            if (m_background != null) return;
            
            m_background = AddUIComponent<UIPanel>();
            m_background.width = width;
            m_background.height = 40;
            m_background.relativePosition = Vector2.zero;
            m_background.zOrder = 0;
        }
    }
}