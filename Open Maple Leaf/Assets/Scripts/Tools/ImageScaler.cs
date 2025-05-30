using UnityEngine;
using UnityEngine.UI;

namespace Tools
{
    public class ImageScaler : MonoBehaviour
    {
        private Image _targetImage;
        public float scaleMin = 1f;
        public float scaleMax = 1.5f;
        public float speed = 2f;

        private RectTransform _rectTransform;
        private bool _increasing = true;

        void Start()
        {
            _targetImage= GetComponent<Image>();
            if (_targetImage != null)
            {
                _rectTransform = _targetImage.GetComponent<RectTransform>();
            }
        }

        void Update()
        {
            if (_rectTransform != null)
            {
                float scale = _rectTransform.localScale.x;
                if (_increasing)
                {
                    scale += Time.deltaTime * speed;
                    if (scale >= scaleMax)
                    {
                        scale = scaleMax;
                        _increasing = false;
                    }
                }
                else
                {
                    scale -= Time.deltaTime * speed;
                    if (scale <= scaleMin)
                    {
                        scale = scaleMin;
                        _increasing = true;
                    }
                }
                _rectTransform.localScale = new Vector3(scale, scale, scale);
            }
        }
    }
}