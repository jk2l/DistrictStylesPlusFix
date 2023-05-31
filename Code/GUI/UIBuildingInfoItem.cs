using ColossalFramework.PlatformServices;
using ColossalFramework.UI;
using DistrictStylesPlus.Code.Managers;
using DistrictStylesPlus.Code.Utils;
using DistrictStylesPlusFix.Code.Utils;
using UnityEngine;

namespace DistrictStylesPlus.Code.GUI
{
    public class UIBuildingInfoItem : UIPanel, UIFastListRow
    {
        private UICheckBox m_name;
        private UISprite m_steamIconSprite;
        private UISprite m_categorySprite; // service subclass
        private UILabel m_level;
        private UILabel m_size;
        private UIPanel m_background;
        private BuildingInfo m_buildingInfo;
        private UILabel m_height;

        public void Display(object data, bool isRowOdd)
        {
            m_buildingInfo = data as BuildingInfo;

            SetupControls();

            float maxLabelWidth = width - 120;

            if (m_buildingInfo == null) return;

            m_name.text = BuildingInfoHelper.GetDisplayName(m_buildingInfo.name);
            m_name.label.textColor = new Color32(255, 255, 255, 255); // TODO: check if status color really not needed
            m_name.label.isInteractive = false;
            var selectedDistrictStyle = UIDistrictStyleSelectPanel.GetSelectedDistrictStyle();
            m_name.isChecked = BuildingInfoHelper.IsBuildingIncludedInStyle(m_buildingInfo, selectedDistrictStyle); 
            
            

            m_level.text = "L" + BuildingInfoHelper.GetLevelNumber(m_buildingInfo);
            m_size.text = BuildingInfoHelper.GetFootprintDimension(m_buildingInfo);
            m_height.text = BuildingInfoHelper.GetBuildingHeight(m_buildingInfo) + "m";

            var buildingCategoryId = (int) BuildingInfoHelper.GetBuildingCategory(m_buildingInfo);
            if (buildingCategoryId != -1) {
                m_categorySprite.atlas = UIUtils.GetAtlas(CategoryIcons.atlases[buildingCategoryId]);
                m_categorySprite.spriteName = CategoryIcons.spriteNames[buildingCategoryId];
                m_categorySprite.tooltip = CategoryIcons.tooltips[buildingCategoryId];
                m_categorySprite.isVisible = true;
            }
            else
            {
                m_categorySprite.isVisible = false;
            }

            var steamId = BuildingInfoHelper.GetSteamId(m_buildingInfo.name);
            if (steamId != null)
            {
                m_steamIconSprite.tooltip = steamId;
                m_steamIconSprite.isVisible = true;
                maxLabelWidth -= 30;
                m_name.label.relativePosition = new Vector3(52, 2);
            }
            else
            {
                m_steamIconSprite.isVisible = false;
                m_name.label.relativePosition = new Vector3(22, 2);
            }

            if (isRowOdd)
            {
                m_background.backgroundSprite = "UnlockingItemBackground";
                m_background.color = new Color32(0, 0, 0, 128); 
            }
            else
            {
                m_background.backgroundSprite = null;
            }

            UIUtils.TruncateLabel(m_name.label, maxLabelWidth);
        }

        protected override void OnMouseEnter(UIMouseEventParameter p)
        {
            base.OnMouseEnter(p);
            
            if (enabled) UIBuildingPreview.instance.Show(m_buildingInfo);
        }
        
        protected override void OnMouseWheel(UIMouseEventParameter p)
        {
            base.OnMouseWheel(p);
            if (enabled) UIBuildingPreview.instance.Show(m_buildingInfo);
        }

        public void Select(bool isRowOdd)
        {
            m_background.backgroundSprite = "ListItemHighlight";
            m_background.color = new Color32(255, 255, 255, 255);
            UIBuildingPreview.instance.Show(m_buildingInfo);
        }

        public void Deselect(bool isRowOdd)
        {
            if (isRowOdd)
            {
                m_background.backgroundSprite = "UnlockingItemBackground";
                m_background.color = new Color32(0, 0, 0, 128);
            }
            else
            {
                m_background.backgroundSprite = null;
            }
        }

        private void SetupControls()
        {
            if (m_name != null) return;

            // set up panel basics
            isVisible = true;
            canFocus = true;
            isInteractive = true;
            width = parent.width;
            height = 40;

            SetupBackgroundPanel();
            SetupNameCheckbox();
            SetupSteamIcon();
            SetupBuildingBasicAttributeLabels();
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

        private void SetupNameCheckbox()
        {
            m_name = UIUtils.CreateCheckBox(this);
            m_name.width = 20;
            m_name.clipChildren = false;
            m_name.relativePosition = new Vector3(5, 13);
            
            m_name.eventCheckChanged += (c, state) =>
            {
                if (state)
                {
                    if (UIDistrictStyleSelectPanel.SelectedDistrictStyle != null &&
                        !UIDistrictStyleSelectPanel.SelectedDistrictStyle.BuiltIn &&
                        !UIDistrictStyleSelectPanel.SelectedDistrictStyle.Contains(m_buildingInfo))
                    {
                        DSPDistrictStyleManager
                            .AddBuildingInfoToStyle(m_buildingInfo, UIDistrictStyleSelectPanel.SelectedDistrictStyle);
                    }
                }
                else
                {
                    if (UIDistrictStyleSelectPanel.SelectedDistrictStyle != null &&
                        !UIDistrictStyleSelectPanel.SelectedDistrictStyle.BuiltIn &&
                        UIDistrictStyleSelectPanel.SelectedDistrictStyle.Contains(m_buildingInfo))
                    {
                        DSPDistrictStyleManager
                            .RemoveBuildingInfoFromStyle(m_buildingInfo, UIDistrictStyleSelectPanel.SelectedDistrictStyle);
                    }
                } 
                
                UIDistrictStyleSelectPanel.Instance.RefreshDistrictStyleSelectList();
                UIBuildingSelectPanel.Instance.RefreshBuildingInfoSelectList();
            };
        }

        private void SetupSteamIcon()
        {
            m_steamIconSprite = m_name.AddUIComponent<UISprite>();
            m_steamIconSprite.spriteName = "SteamWorkshop";
            m_steamIconSprite.isVisible = false;
            m_steamIconSprite.relativePosition = new Vector3(22, 0);

            UIUtils.ResizeIcon(m_steamIconSprite, new Vector2(25, 25));

            if (PlatformService.IsOverlayEnabled())
            {
                m_steamIconSprite.eventClick += (c, p) =>
                {
                    string steamId = BuildingInfoHelper.GetSteamId(m_buildingInfo.name);
                    if (steamId == null) return;
                    p.Use();
                    PlatformService.ActivateGameOverlayToWorkshopItem(new PublishedFileId(ulong.Parse(steamId)));
                };
            }
        }

        private void SetupBuildingBasicAttributeLabels()
        {
            m_height = AddUIComponent<UILabel>();
            m_height.width = 40;
            m_height.textAlignment = UIHorizontalAlignment.Center;
            m_height.relativePosition = new Vector3(width - 50f, 15);
            
            m_size = AddUIComponent<UILabel>();
            m_size.width = 30;
            m_size.textAlignment = UIHorizontalAlignment.Center;
            m_size.relativePosition = new Vector3(width - 90f, 15);

            m_level = AddUIComponent<UILabel>();
            m_level.width = 30;
            m_level.textAlignment = UIHorizontalAlignment.Center;
            m_level.relativePosition = new Vector3(width - 120f, 15);

            m_categorySprite = AddUIComponent<UISprite>();
            m_categorySprite.size = new Vector2(20, 20);
            m_categorySprite.relativePosition = new Vector3(width - 150f, 10);
        }
    }
}