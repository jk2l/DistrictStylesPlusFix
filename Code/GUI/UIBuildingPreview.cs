using ColossalFramework.UI;
using DistrictStylesPlusFix.Code.Utils;
using UnityEngine;

namespace DistrictStylesPlus.Code.GUI
{
    public class UIBuildingPreview : UIPanel
    {
        private BuildingInfo m_buildingInfo;

        private UITextureSprite m_preview;
        private UISprite m_noPreview;
        private PreviewRenderer m_previewRenderer;

        private UILabel m_buildingName;
        private UISprite m_categoryIcon;

        private UILabel m_level;
        private UILabel m_size;

        private static UIBuildingPreview _instance;

        public static UIBuildingPreview instance => _instance;

        public override void Start()
        {
            base.Start();

            _instance = this;

            backgroundSprite = "GenericPanel";

            // Preview
            m_preview = AddUIComponent<UITextureSprite>();
            m_preview.size = size;
            m_preview.relativePosition = Vector3.zero;

            m_noPreview = AddUIComponent<UISprite>();
            m_noPreview.spriteName = "Niet";
            m_noPreview.relativePosition = new Vector3(
                (width - m_noPreview.spriteInfo.width) / 2, 
                (height - m_noPreview.spriteInfo.height) / 2);

            m_previewRenderer = gameObject.AddComponent<PreviewRenderer>();
            m_previewRenderer.size = m_preview.size * 2; // Twice the size for anti-aliasing

            eventMouseDown += (c, p) =>
            {
                eventMouseMove += RotateCamera;
            };

            eventMouseUp += (c, p) =>
            {
                eventMouseMove -= RotateCamera;
            };

            eventMouseWheel += (c, p) =>
            {
                m_previewRenderer.zoom -= Mathf.Sign(p.wheelDelta) * 0.25f;
                RenderPreview();
            };

            // Name
            m_buildingName = AddUIComponent<UILabel>();
            m_buildingName.textScale = 0.9f;
            m_buildingName.useDropShadow = true;
            m_buildingName.dropShadowColor = new Color32(80, 80, 80, 255);
            m_buildingName.dropShadowOffset = new Vector2(2, -2);
            m_buildingName.text = "Name";
            m_buildingName.isVisible = false;
            m_buildingName.relativePosition = new Vector3(5, 10);

            // Category icon
            m_categoryIcon = AddUIComponent<UISprite>();
            m_categoryIcon.size = new Vector2(35, 35);
            m_categoryIcon.isVisible = false;
            m_categoryIcon.relativePosition = new Vector3(width - 37, 2);

            // Level
            m_level = AddUIComponent<UILabel>();
            m_level.textScale = 0.9f;
            m_level.useDropShadow = true;
            m_level.dropShadowColor = new Color32(80, 80, 80, 255);
            m_level.dropShadowOffset = new Vector2(2, -2);
            m_level.text = "Level";
            m_level.isVisible = false;
            m_level.relativePosition = new Vector3(5, height - 20);

            // Size
            m_size = AddUIComponent<UILabel>();
            m_size.textScale = 0.9f;
            m_size.useDropShadow = true;
            m_size.dropShadowColor = new Color32(80, 80, 80, 255);
            m_size.dropShadowOffset = new Vector2(2, -2);
            m_size.text = "Size";
            m_size.isVisible = false;
            m_size.relativePosition = new Vector3(width - 50, height - 20);
        }

        public void Show(BuildingInfo buildingInfo)
        {
            if (buildingInfo == null) return;
            m_buildingInfo = buildingInfo;
            
            // Preview
            if (buildingInfo.m_mesh != null)
            {
                m_previewRenderer.cameraRotation = 210f;
                m_previewRenderer.zoom = 4f;
                m_previewRenderer.mesh = m_buildingInfo.m_mesh;
                m_previewRenderer.material = m_buildingInfo.m_material;
                RenderPreview();
                m_preview.texture = m_previewRenderer.texture;
                m_noPreview.isVisible = false;
            }
            else
            {
                m_preview.texture = null;
                m_noPreview.isVisible = true;
            }
            
            m_buildingName.isVisible = false;
            m_categoryIcon.isVisible = false;
            m_level.isVisible = false;
            m_size.isVisible = false;
            
            // Name
            m_buildingName.isVisible = true;
            m_buildingName.text = BuildingInfoHelper.GetDisplayName(m_buildingInfo.name);
            UIUtils.TruncateLabel(m_buildingName, width - 45);
            m_buildingName.autoHeight = true;
            
            // Category icon //TODO: duplicate with UIBuildingInfoItem
            var buildingCategoryId = (int) BuildingInfoHelper.GetBuildingCategory(m_buildingInfo);
            if (buildingCategoryId != -1) {
                m_categoryIcon.atlas = UIUtils.GetAtlas(CategoryIcons.atlases[buildingCategoryId]);
                m_categoryIcon.spriteName = CategoryIcons.spriteNames[buildingCategoryId];
                m_categoryIcon.tooltip = CategoryIcons.tooltips[buildingCategoryId];
                m_categoryIcon.isVisible = true;
            }
            else
            {
                m_categoryIcon.isVisible = false;
            }
            
            // Level //TODO: duplicate with UIBuildingInfoItem
            m_level.text = "L" + BuildingInfoHelper.GetLevelNumber(m_buildingInfo);
            m_level.isVisible = true;
            
            // Size //TODO: kind of duplicate with UIBuildingInfoItem
            m_size.text = BuildingInfoHelper.GetFootprintDimension(m_buildingInfo) 
                          + " (" + BuildingInfoHelper.GetBuildingHeight(m_buildingInfo) + " m)";
            m_size.textColor = new Color32(255, 0, 0, 255);
            m_size.isVisible = true;
            m_size.autoSize = true;
            m_size.relativePosition = new Vector3(width - m_size.width - 7, height - 20);
        }

        private void RenderPreview()
        {
            if (m_buildingInfo == null) return;

            if (m_buildingInfo.m_useColorVariations)
            {
                Color materialColor = m_buildingInfo.m_material.color;
                m_buildingInfo.m_material.color = m_buildingInfo.m_color0;
                m_previewRenderer.Render();
                m_buildingInfo.m_material.color = materialColor;
            }
            else
            {
                m_previewRenderer.Render();
            }
        }

        private void RotateCamera(UIComponent c, UIMouseEventParameter p)
        {
            m_previewRenderer.cameraRotation -= p.moveDelta.x / m_preview.width * 360f;
            RenderPreview();
        }
    }
}