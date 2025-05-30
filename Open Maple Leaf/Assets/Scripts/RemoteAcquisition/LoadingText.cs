using TMPro;
using UnityEngine;

namespace RemoteAcquisition
{
    public class LoadingText : MonoBehaviour
    {
        public TextMeshProUGUI loadingText; // 引用TextMeshProUGUI组件
        private const string BaseText = "正在加载中"; // 基础文本
        private int _dotCount; // 当前点的数量

        private void Start()
        {
            if (loadingText == null)
            {
                loadingText = GetComponent<TextMeshProUGUI>();
            }

            // 启动协程
            StartCoroutine(UpdateLoadingText());
        }

        private System.Collections.IEnumerator UpdateLoadingText()
        {
            while (true)
            {
                // 更新文本内容
                loadingText.text = BaseText + new string('.', _dotCount);

                // 增加点的数量
                _dotCount = (_dotCount + 1) % 4;

                // 等待0.5秒
                yield return new WaitForSeconds(0.25f);
            }
            // ReSharper disable once IteratorNeverReturns
        }
    }
}