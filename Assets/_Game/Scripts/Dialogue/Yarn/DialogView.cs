using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Mechanics.Dialog
{
    public class DialogView : MonoBehaviour
    {
        public TextMeshProUGUI Txt_dialog => _txt_dialog;
        public Image Img_dialog => _img_dialog;
        public Image Img_portrait => _img_portrait;
        public GameObject P_characterName => _p_characterName;
        public TextMeshProUGUI Txt_characterName => _txt_characterName;
        public GameObject Btn_continue => _btn_continue;
        public Slider Sldr_progressbar => _sldr_progressbar;

        public Sprite DefaultBoxSprite => _defaultBoxSprite;
        public Color DefaultBoxColor => _defaultBoxColor;

        [SerializeField] private TextMeshProUGUI _txt_dialog = null;
        [SerializeField] private Image _img_dialog = null;
        [SerializeField] private Image _img_portrait = null;
        [SerializeField] private GameObject _p_characterName = null;
        [SerializeField] private TextMeshProUGUI _txt_characterName = null;
        [SerializeField] private GameObject _btn_continue = null;
        [SerializeField] private Slider _sldr_progressbar = null;

        private Sprite _defaultBoxSprite;
        private Color _defaultBoxColor = Color.white;

        private void Awake()
        {
            if (_img_dialog != null)
            {
                _defaultBoxSprite = _img_dialog.sprite;
                _defaultBoxColor = _img_dialog.color;
            }
        }
    }
}