using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Tools
{
    public class ImageColorChange : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        public Image targetImage; // 目标Image组件
        public Color hoverColor = Color.red; // 鼠标移入时的颜色
        public Color normalColor = Color.white; // 鼠标离开时的颜色

        void Start()
        {
            if (targetImage == null)
            {
                targetImage = GetComponent<Image>();
            }

            if (targetImage != null)
            {
                targetImage.color = normalColor;
            }
        }

        // 当鼠标指针进入Image区域时调用此方法
        public void OnPointerEnter(PointerEventData eventData)
        {
            if (targetImage != null)
            {
                targetImage.color = hoverColor;
            }
        }

        // 当鼠标指针离开Image区域时调用此方法
        public void OnPointerExit(PointerEventData eventData)
        {
            if (targetImage != null)
            {
                targetImage.color = normalColor;
            }
        }
    }
}