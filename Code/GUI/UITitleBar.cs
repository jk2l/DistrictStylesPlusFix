using ColossalFramework;
using ColossalFramework.UI;
using DistrictStylesPlus.Code.Managers;
using UnityEngine;

namespace DistrictStylesPlus.Code.GUI
{
    public class UITitleBar : UIPanel
    {
        private UISprite _icon;
        private UILabel _title;
        private UIButton _close;
        private UIDragHandle _drag;

        public bool isModal = false;

        public string IconSprite
        {
            get => _icon.spriteName;
            set
            {
                if (_icon == null) SetupControls();
                _icon.spriteName = value;

                if (_icon.spriteInfo != null)
                {
                    UIUtils.ResizeIcon(_icon, new Vector2(30, 30));
                    _icon.relativePosition = new Vector3(10, 5);
                }
            }
        }

        public UIButton CloseButton => _close;

        public string Title
        {
            get => _title.text;
            set
            {
                if (_title == null) SetupControls();
                _title.text = value;
            }
        }

        private void SetupControls()
        {
            width = parent.width;
            height = 40;
            isVisible = true;
            canFocus = true;
            isInteractive = true;
            relativePosition = Vector3.zero;

            _drag = AddUIComponent<UIDragHandle>();
            _drag.width = width - 50;
            _drag.height = height;
            _drag.relativePosition = Vector3.zero;
            _drag.target = parent;

            _icon = AddUIComponent<UISprite>();
            _icon.spriteName = IconSprite;
            _icon.relativePosition = new Vector3(10, 5);

            _title = AddUIComponent<UILabel>();
            _title.relativePosition = new Vector3(50, 13);
            _title.text = Title;

            _close = AddUIComponent<UIButton>();
            _close.relativePosition = new Vector3(width - 35, 2);
            _close.normalBgSprite = "buttonclose";
            _close.hoveredBgSprite = "buttonclosehover";
            _close.pressedBgSprite = "loading_icon";
            _close.tooltip = "Save and Close";
            _close.eventClick += (component, param) =>
            {
                StartCoroutine(DSPDistrictStylePackageManager.SaveChangedAssetsOfDistrictStyleMetaDataAsync());
                if (isModal)
                    UIView.PopModal();
                parent.Hide();
            };
        }
    }
}