using System.Collections;
using Game.Scripts.UI.Theme;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace Game.Scripts.UI.Component
{
    public class ButtonStateAnimator : MonoBehaviour,
        IPointerEnterHandler, IPointerExitHandler,
        IPointerDownHandler,  IPointerUpHandler,
        ISelectHandler,       IDeselectHandler
    {
        [SerializeField] private Image _fillImage;
        [SerializeField] private float _duration = 0.2f;
        [SerializeField] private HorizontalLayoutGroup _content;
        [SerializeField] private LayoutElement _layoutElement;
        [SerializeField] private TextMeshProUGUI _text;

        private Coroutine _animation;

        private void Awake()
        {
            _text.color = UITheme.TextWhite;
            
            _fillImage.color     = UITheme.AccentDim;
            _fillImage.fillAmount = 0f;

            _content.padding = new RectOffset(
                (int)UITheme.ButtonPaddingX,
                (int)UITheme.ButtonPaddingX,
                (int)UITheme.ButtonPaddingY,
                (int)UITheme.ButtonPaddingY
            );
            _layoutElement.minWidth = UITheme.ButtonMinWidth;
        }

        public void OnPointerEnter(PointerEventData e) => Animate(1f);
        public void OnPointerExit(PointerEventData e)  => Animate(0f);
        public void OnPointerDown(PointerEventData e)  => Animate(1f);
        public void OnPointerUp(PointerEventData e)    => Animate(1f);
        public void OnSelect(BaseEventData e)          => Animate(1f);
        public void OnDeselect(BaseEventData e)        => Animate(0f);

        private void Animate(float target)
        {
            if (_animation != null) StopCoroutine(_animation);
            _animation = StartCoroutine(AnimateRoutine(target));
        }

        private IEnumerator AnimateRoutine(float target)
        {
            float start   = _fillImage.fillAmount;
            float elapsed = 0f;

            while (elapsed < _duration)
            {
                elapsed += Time.unscaledDeltaTime;
                float t = Mathf.SmoothStep(0f, 1f, elapsed / _duration);
                _fillImage.fillAmount = Mathf.Lerp(start, target, t);
                yield return null;
            }

            _fillImage.fillAmount = target;
            _animation = null;
        }
    }
}