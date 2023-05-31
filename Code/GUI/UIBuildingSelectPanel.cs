using System.Collections.Generic;
using System.Linq;
using ColossalFramework;
using ColossalFramework.UI;
using DistrictStylesPlus.Code.Managers;
using DistrictStylesPlus.Code.Utils;
using DistrictStylesPlusFix.Code.Utils;
using UnityEngine;
using static DistrictStylesPlus.Code.GUI.DistrictStyleEditorPanel;

namespace DistrictStylesPlus.Code.GUI
{
    public class UIBuildingSelectPanel : UIPanel
    {
        private const int OrderByName = 0;
        private const int OrderByZone = 1;
        private const int OrderByLevel = 2;
        private const int OrderBySize = 3;
        private const int OrderByHeight = 4;

        
        private UIFastList _buildingInfoSelect;
        private UILabel _includeLabel;
        private UIButton _includeAllButton;
        private UIButton _includeNoneButton;
        internal BuildingInfo SelectedBuildingInfo;

        public static UIBuildingSelectPanel Instance { get; private set; }

        public override void Start()
        {
            base.Start();

            Instance = this;
            
            SetupBuildingInfoSelect();
            SetupIncludeNoneButton();
            SetupIncludeAllButton();
            SetupIncludeLabel();
        }

        internal void RefreshBuildingInfoSelectList()
        {
            _buildingInfoSelect.Refresh();
        }

        internal void FilterAndRefreshBuildingInfoSelectList()
        {
            _buildingInfoSelect.rowsData = FilterBuildingInfoList();
        }

        private void SetupBuildingInfoSelect()
        {
            _buildingInfoSelect = UIFastList.Create<UIBuildingInfoItem>(this);
            _buildingInfoSelect.backgroundSprite = "UnlockingPanel";
            _buildingInfoSelect.width = width;
            _buildingInfoSelect.height = height - 40;
            _buildingInfoSelect.canSelect = true;
            _buildingInfoSelect.rowHeight = 40;
            _buildingInfoSelect.autoHideScrollbar = true;
            _buildingInfoSelect.relativePosition = Vector3.zero;

            _buildingInfoSelect.rowsData = FilterBuildingInfoList();
            
            _buildingInfoSelect.eventSelectedIndexChanged += (component, value) =>
            {
                if (value > -1) {
                    SelectedBuildingInfo = _buildingInfoSelect.selectedItem as BuildingInfo;
                }
            };
            
            _buildingInfoSelect.eventMouseLeave += (component, param) =>
            {
                if (SelectedBuildingInfo != null)
                    UIBuildingDetailsPanel.instance.UpdateBuildingInfo(SelectedBuildingInfo);
            };
        }

        private FastList<object> FilterBuildingInfoList()
        {
            var filteredData = new List<object>();
            var selectedDistrictStyle = UIDistrictStyleSelectPanel.SelectedDistrictStyle;
            
            for (uint i = 0; i < PrefabCollection<BuildingInfo>.LoadedCount(); i++)
            {
                BuildingInfo item = PrefabCollection<BuildingInfo>.GetLoaded(i);

                if (item == null) continue;

                if (!DistrictStylesPlusManager.IsPrefabGrowable(item)) continue;

                // Origin
                if (UIBuildingFilter.Instance.buildingOrigin == Origin.Default && BuildingInfoHelper.IsCustomAsset(item.name)) continue;
                if (UIBuildingFilter.Instance.buildingOrigin == Origin.Custom && !BuildingInfoHelper.IsCustomAsset(item.name)) continue;
                // TODO: when cloning is implemented
                // if (buildingFilter.buildingOrigin == Origin.Cloned && !item.isCloned) continue;

                // Status
                if (selectedDistrictStyle != null)
                {
                    if (UIBuildingFilter.Instance.buildingStatus == Status.Included && !selectedDistrictStyle.Contains(item)) continue;
                    if (UIBuildingFilter.Instance.buildingStatus == Status.Excluded && selectedDistrictStyle.Contains(item)) continue;
                }

                // Level
                if (UIBuildingFilter.Instance.buildingLevel != ItemClass.Level.None 
                    && item.GetClassLevel() != UIBuildingFilter.Instance.buildingLevel) continue;

                // size
                Vector2 buildingSize = UIBuildingFilter.Instance.BuildingSize;
                if (!FitExpectedSize(buildingSize, item)) continue;

                // zone
                if (!UIBuildingFilter.Instance.IsAllZoneSelected())
                {
                    Category category = BuildingInfoHelper.GetBuildingCategory(item);
                    if (category == Category.None || !UIBuildingFilter.Instance.IsZoneSelected(category)) continue;
                }

                // ccp
                if (UIBuildingFilter.Instance.isDlcFilterSelected())
                {
                    if (item.m_requiredModderPack == 0 || !UIBuildingFilter.Instance.isDlcSelected(item.m_requiredModderPack)) continue;
                }

                // Name
                if (!UIBuildingFilter.Instance.buildingName.IsNullOrWhiteSpace() 
                    && !item.name.ToLower().Contains(UIBuildingFilter.Instance.buildingName.ToLower())) continue;
                
                // height
                if (!(UIBuildingFilter.Instance.minBuildingHeight <= BuildingInfoHelper.GetBuildingHeight(item))) continue;
                if (!(UIBuildingFilter.Instance.maxBuildingHeight >= BuildingInfoHelper.GetBuildingHeight(item))) continue;

                filteredData.Add(item);
            }

            var resultData = new FastList<object>();

            if (filteredData.Count <= 0) return resultData;

            var sorted = SortBy(UIBuildingFilter.Instance.orderBy, filteredData);
            
            foreach (var o in sorted)
            {
                resultData.Add(o);
            }

            return resultData;
        }
        
        private static IEnumerable<object> SortBy(int chosenOrder, IEnumerable<object> data)
        {
            switch (chosenOrder)
            {
                case OrderByName:
                    return data.OrderBy(item => BuildingInfoHelper.GetDisplayName(((BuildingInfo)item).name));
                case OrderByZone:
                    return data.OrderBy(item => ((BuildingInfo)item).m_class.m_service)
                        .ThenBy(item => ((BuildingInfo)item).m_class.m_subService);
                case OrderByLevel:
                    return data.OrderBy(item => ((BuildingInfo)item).m_class.m_level);
                case OrderBySize:
                    return data.OrderBy(item => ((BuildingInfo)item).m_cellWidth)
                        .ThenBy(item => ((BuildingInfo)item).m_cellLength);
                case OrderByHeight:
                    return data.OrderBy(item => BuildingInfoHelper.GetBuildingHeight((BuildingInfo)item));
                default:
                    return data.OrderBy(item => BuildingInfoHelper.GetDisplayName(((BuildingInfo)item).name));
            }
        }

        private static bool FitExpectedSize(Vector2 expectedSize, BuildingInfo buildingInfo)
        {
            return expectedSize == Vector2.zero
                   || (expectedSize.y == 0 && new Vector2(buildingInfo.m_cellWidth, 0) == expectedSize)
                   || (expectedSize.y > 0 
                       && new Vector2(buildingInfo.m_cellWidth, buildingInfo.m_cellLength) == expectedSize);
        }

        private void SetupIncludeNoneButton()
        {
            _includeNoneButton = UIUtils.CreateButton(this);
            _includeNoneButton.width = 55;
            _includeNoneButton.text = "None";
            _includeNoneButton.relativePosition = new Vector3(width - _includeNoneButton.width, 
                _buildingInfoSelect.height + Spacing);
            
            _includeNoneButton.eventClick += (component, clickEvent) =>
            {
                if (clickEvent.used) return;
                
                Logging.DebugLog("Number of assets: " + _buildingInfoSelect.rowsData.m_size);
                var selectedDistrictStyle = UIDistrictStyleSelectPanel.GetSelectedDistrictStyle();
                
                if (selectedDistrictStyle == null || selectedDistrictStyle.BuiltIn) return;
                
                var buildingInfosToRemove = 
                    _buildingInfoSelect.rowsData.m_buffer.Cast<BuildingInfo>().ToList();
                
                Logging.DebugLog("Try to remove multiple buildings from style.");
                
                DSPDistrictStyleManager.RemoveBuildingInfoListFromStyle(buildingInfosToRemove, selectedDistrictStyle);
                
                // TODO: is it needed?
                UIDistrictStyleSelectPanel.Instance.RefreshDistrictStyleSelectList();
                Instance.RefreshBuildingInfoSelectList();
            };
        }

        private void SetupIncludeAllButton()
        {
            _includeAllButton = UIUtils.CreateButton(this);
            _includeAllButton.width = 55;
            _includeAllButton.text = "All";
            _includeAllButton.relativePosition = new Vector3(
                _includeNoneButton.relativePosition.x - _includeAllButton.width - Spacing, 
                _buildingInfoSelect.height + Spacing);
            
            _includeAllButton.eventClick += (component, clickEvent) =>
            {
                if (clickEvent.used) return;
                
                Logging.DebugLog("Number of assets: " + _buildingInfoSelect.rowsData.m_size);
                var selectedDistrictStyle = UIDistrictStyleSelectPanel.GetSelectedDistrictStyle();
                
                if (selectedDistrictStyle == null || selectedDistrictStyle.BuiltIn) return;
                
                var buildingInfosToInclude = 
                    _buildingInfoSelect.rowsData.m_buffer.Cast<BuildingInfo>().ToList();
                
                Logging.DebugLog("Try to add all buildings to style.");
                
                DSPDistrictStyleManager.AddBuildingInfoListToStyle(buildingInfosToInclude, selectedDistrictStyle);
                
                // TODO: is it needed?
                UIDistrictStyleSelectPanel.Instance.RefreshDistrictStyleSelectList();
                Instance.RefreshBuildingInfoSelectList();
            };
            
        }

        private void SetupIncludeLabel()
        {
            _includeLabel = AddUIComponent<UILabel>();
            _includeLabel.width = 100;
            _includeLabel.padding = new RectOffset(0, 0, 8, 0);
            _includeLabel.textScale = 0.8f;
            _includeLabel.text = "Include:";
            _includeLabel.relativePosition = new Vector3(
                _includeAllButton.relativePosition.x - _includeLabel.width - Spacing, 
                _buildingInfoSelect.height + Spacing);

        }
    }
}