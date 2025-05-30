using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Tools
{
    public class TextDisplay : MonoBehaviour
    {
        // 引用Image组件
        public Image image;

        // 引用TextMeshPro组件
        public TMP_Text textMeshPro;

        // 文字到Image边框的边距
        public float padding = 10f;
        
        public void SetText(string text)
        {
            // 设置文本内容
            textMeshPro.text = text;

            // 强制更新TextMeshPro以获取文本的实际宽度
            textMeshPro.ForceMeshUpdate();

            // 获取文本的实际宽度
            float textWidth = textMeshPro.preferredWidth;

            // 根据文本宽度和边距调整Image的宽度
            RectTransform imageRectTransform = image.GetComponent<RectTransform>();
            imageRectTransform.sizeDelta = new Vector2(textWidth + padding * 2, imageRectTransform.sizeDelta.y);
        }
        
    }
}
