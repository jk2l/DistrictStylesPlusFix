using System;
using ColossalFramework.UI;
using DistrictStylesPlus.Code.Utils;
using Epic.OnlineServices.Leaderboards;
using ICities;
using UnifiedUI.Helpers;
using UnityEngine;

namespace DistrictStylesPlus.Code.GUI
{
    public class DistrictStyleEditorPanel : UIPanel
    {
        #region Constant values for UI formatting
        private const float StyleSelectPanelWidth = 250;
        private const float BuildingSelectPanelWidth = 600;
        private const float BuildingDetailsPanelWidth = 250;
        private const float Height = 600;
        internal const float Spacing = 5;
        private const float TitleHeight = 40;
        #endregion
        
        // object instances
        private static GameObject _gameObject;

        public static DistrictStyleEditorPanel Instance { get; private set; }

        // UI Components
        private UITitleBar _editorTitleBar;
        private UIBuildingFilter _buildingFilter;
        private UIDistrictStyleSelectPanel _styleSelectPanel;
        private UIBuildingSelectPanel _buildingSelectPanel;
        private UIBuildingDetailsPanel _buildingDetailsPanel;

        private UUICustomButton _uuiButton; 

        /// <summary>
        /// Initialize District style editor panel including button at tool bar.
        /// </summary>
        internal static void Initialize()
        {
            // do nothing if instance exists already
            if (Instance != null) return;

            try
            {
                // Create our own gameObject to help to find it in ModTools
                _gameObject = new GameObject("DSPEditorPanel");
                _gameObject.transform.parent = UIView.GetAView().transform;

                Instance = _gameObject.AddComponent<DistrictStyleEditorPanel>();
                Instance.transform.parent = _gameObject.transform.parent;

                // draw the editor panel
                Instance.DrawDistrictStyleEditorPanel();
                _gameObject.SetActive(false);
            } 
            catch (Exception e)
            {
                Logging.LogException(e, "Exception when initializing DistrictStyleEditorPanel");
            }
        }

        public override void Awake()
        {
            try
            {
                base.Awake();

                var iconPath = UUIHelpers.GetFullPath<DistrictStylesPlusMod>("Resources", "DSEdit.png");
                var icon = UUIHelpers.LoadTexture(iconPath);

                _uuiButton = UUIHelpers.RegisterCustomButton(
                    name: nameof(DistrictStylesPlusMod),
                    groupName: null,
                    tooltip: "Open district styles editor",
                    icon: icon,
                    onToggle: (value) => Toggle()
                );
            }
            catch (Exception e) 
            {
                Logging.LogException(e, "Exception when awake DistrictStyleEditorPanel");
            }
        }

        /// <summary>
        /// Destroy gameObject of District Styles Editor
        /// </summary>
        public static void Destroy()
        {
            try
            {
                if (_gameObject != null)
                    GameObject.Destroy(_gameObject);
            }
            catch (Exception e)
            {
                // Catching any exception to not block the unloading process of other mods
                Logging.LogException(e, "Exception when destroying DistrictStyleEditorPanel");
            }
        }

        internal void Toggle()
        {
            if (isVisible)
            {
                _gameObject.SetActive(false);
                Instance.Hide();
                Logging.DebugLog("Hide DSP Editor panel.");
            }
            else
            {
                _gameObject.SetActive(true);
                Instance.Show(true);
                Logging.DebugLog("Show DSP Editor panel.");
            }
        }
        
        private void DrawDistrictStyleEditorPanel()
        {
            backgroundSprite = "UnlockingPanel2";
            isVisible = false;
            canFocus = true;
            isInteractive = true;
            width = Spacing + StyleSelectPanelWidth + Spacing + BuildingSelectPanelWidth + Spacing + 
                    BuildingDetailsPanelWidth + Spacing;
            height = TitleHeight + Height + Spacing;
            relativePosition = new Vector3(
                Mathf.Floor((UIView.GetAView().fixedWidth - width) / 2),
                Mathf.Floor((UIView.GetAView().fixedHeight - height) / 2));

            SetupTitleBar();
            SetupBuildingFilter();
            SetupDistrictStyleSelectPanel();
            SetupBuildingSelectPanel();
            SetupBuildingDetailsPanel();
        }
        
        private void SetupTitleBar()
        {
            _editorTitleBar = AddUIComponent<UITitleBar>();
            _editorTitleBar.Title = "District Style Editor";
            _editorTitleBar.IconSprite = "ToolbarIconZoomOutCity";
        }
        
        private void SetupBuildingFilter()
        {
            _buildingFilter = AddUIComponent<UIBuildingFilter>();
            _buildingFilter.width = width - Spacing * 2;
            _buildingFilter.height = 120;
            _buildingFilter.relativePosition = new Vector3(Spacing, TitleHeight);
            _buildingFilter.eventFilteringChanged += (component, value) =>
            {
                UIBuildingSelectPanel.Instance.FilterAndRefreshBuildingInfoSelectList();
            };
        }

        private void SetupDistrictStyleSelectPanel()
        {
            _styleSelectPanel = AddUIComponent<UIDistrictStyleSelectPanel>();
            _styleSelectPanel.width = StyleSelectPanelWidth;
            _styleSelectPanel.height = Height - _buildingFilter.height;
            _styleSelectPanel.relativePosition = new Vector3(Spacing, TitleHeight + _buildingFilter.height + Spacing);
        }
        
        private void SetupBuildingSelectPanel()
        {
            _buildingSelectPanel = AddUIComponent<UIBuildingSelectPanel>();
            _buildingSelectPanel.width = BuildingSelectPanelWidth;
            _buildingSelectPanel.height = Height - _buildingFilter.height;
            _buildingSelectPanel.relativePosition = new Vector3(StyleSelectPanelWidth + Spacing * 2, 
                TitleHeight + _buildingFilter.height + Spacing);
        }

        private void SetupBuildingDetailsPanel()
        {
            _buildingDetailsPanel = AddUIComponent<UIBuildingDetailsPanel>();
            _buildingDetailsPanel.width = BuildingDetailsPanelWidth;
            _buildingDetailsPanel.height = Height - _buildingFilter.height;
            _buildingDetailsPanel.relativePosition = new Vector3(
                StyleSelectPanelWidth + BuildingSelectPanelWidth + Spacing * 3,
                TitleHeight + _buildingFilter.height + Spacing);
        }
    }
}