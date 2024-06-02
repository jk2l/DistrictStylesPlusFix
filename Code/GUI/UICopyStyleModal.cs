using System.Linq;
using ColossalFramework;
using ColossalFramework.UI;
using DistrictStylesPlus.Code.Managers;
using DistrictStylesPlus.Code.Utils;
using UnityEngine;

// Consider merge common code of New Style and Copy Style 
namespace DistrictStylesPlus.Code.GUI
{
    public class UICopyStyleModal : UIPanel
    {
        private UITitleBar _title;
        private UITextField _name;
        private UIButton _okBtn;
        private UIButton _cancelBtn;

        private static UICopyStyleModal _instance;

        public static UICopyStyleModal Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = UIView.GetAView().AddUIComponent(typeof(UICopyStyleModal)) as UICopyStyleModal;
                }
                return _instance;
            }
        }

        public override void Start()
        {
            base.Start();

            backgroundSprite = "UnlockingPanel2";
            isVisible = false;
            canFocus = true;
            isInteractive = true;
            width = 250;

            // Title Bar
            _title = AddUIComponent<UITitleBar>();
            _title.Title = "Create New Theme";
            _title.IconSprite = "ToolbarIconZoomOutCity";
            _title.isModal = true;

            // Name
            UILabel newStyleNameLbl = AddUIComponent<UILabel>();
            newStyleNameLbl.height = 30;
            newStyleNameLbl.text = "Theme name:";
            newStyleNameLbl.relativePosition = new Vector3(5, _title.height);

            _name = UIUtils.CreateTextField(this);
            _name.width = width - 10;
            _name.height = 30;
            _name.padding = new RectOffset(6, 6, 6, 6);
            _name.relativePosition = new Vector3(5, newStyleNameLbl.relativePosition.y + newStyleNameLbl.height + 5);

            _name.Focus();
            _name.eventTextChanged += (c, s) =>
            {
                _okBtn.isEnabled = !s.IsNullOrWhiteSpace() && !DSPDistrictStylesUtils.getDistrictStyleShortNames().Contains(s);
            };

            _name.eventTextSubmitted += (c, s) =>
            {
                if (_okBtn.isEnabled && Input.GetKey(KeyCode.Return)) _okBtn.SimulateClick();
            };

            // Ok
            _okBtn = UIUtils.CreateButton(this);
            _okBtn.text = "Create";
            _okBtn.isEnabled = false;
            _okBtn.relativePosition = new Vector3(5, _name.relativePosition.y + _name.height + 5);

            _okBtn.eventClick += (c, p) =>
            {
                var selectedDistrictStyle = UIDistrictStyleSelectPanel.GetSelectedDistrictStyle();

                if (selectedDistrictStyle != null) {

                    DistrictStyle newStyle = DSPDistrictStyleManager.CreateDistrictStyle(_name.text);

                    var buildingInfosToInclude =
                    UIDistrictStyleSelectPanel.SelectedDistrictStyle.GetBuildingInfos();

                    Logging.DebugLog("Try to add all buildings to style.");

                    foreach (BuildingInfo info in buildingInfosToInclude)
                    {
                        newStyle.Add(info);
                    }

                    DSPDistrictStyleManager.AddBuildingInfoListToStyle(
                        new List<BuildingInfo>(buildingInfosToInclude), selectedDistrictStyle
                    );
                }

                UIDistrictStyleSelectPanel.Instance.RefreshDistrictStyleSelect();
                UIView.PopModal();
                Hide();
            };

            // Cancel
            _cancelBtn = UIUtils.CreateButton(this);
            _cancelBtn.text = "Cancel";
            _cancelBtn.relativePosition = new Vector3(width - _cancelBtn.width - 5, _okBtn.relativePosition.y);

            _cancelBtn.eventClick += (c, p) =>
            {
                UIView.PopModal();
                Hide();
            };

            height = _cancelBtn.relativePosition.y + _cancelBtn.height + 5;
            relativePosition = new Vector3(Mathf.Floor((GetUIView().fixedWidth - width) / 2), 
                Mathf.Floor((GetUIView().fixedHeight - height) / 2));

            isVisible = true;
        }

        protected override void OnVisibilityChanged()
        {
            base.OnVisibilityChanged();

            UIComponent modalEffect = GetUIView().panelsLibraryModalEffect;

            if (isVisible)
            {
                _name.text = "";
                _name.Focus();

                if (modalEffect != null)
                {
                    modalEffect.Show(false);
                    ValueAnimator.Animate("NewThemeModalEffect", delegate(float val)
                    {
                        modalEffect.opacity = val;
                    }, new AnimatedFloat(0f, 1f, 0.7f, EasingType.CubicEaseOut));
                }
            }
            else if (modalEffect != null)
            {
                ValueAnimator.Animate("NewThemeModalEffect", delegate(float val)
                {
                    modalEffect.opacity = val;
                }, new AnimatedFloat(1f, 0f, 0.7f, EasingType.CubicEaseOut), delegate
                {
                    modalEffect.Hide();
                });
            }
        }

        protected override void OnKeyDown(UIKeyEventParameter p)
        {
            if (Input.GetKey(KeyCode.Escape))
            {
                p.Use();
                UIView.PopModal();
                Hide();
            }

            base.OnKeyDown(p);
        }
    }
}