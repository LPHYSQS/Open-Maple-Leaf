using System.IO;
using TMPro;
using UnityEngine;

namespace Concerning
{
    public class GetConcerning : MonoBehaviour
    {
        private const string LocalFolderPath = "Concerning";
        private const string LocalFileName = "ConcerningData.txt";
        private string _localFilePath;

        public TMP_Text concerningData;

        void Start()
        {
            // 初始化本地文件路径
            _localFilePath = Path.Combine(Application.streamingAssetsPath, LocalFolderPath, LocalFileName);

            if (concerningData == null)
            {
                Debug.LogError("缺少 TextMeshProUGUI 引用!");
                return;
            }

            // 确保本地文件夹存在
            if (!Directory.Exists(Path.Combine(Application.streamingAssetsPath, LocalFolderPath)))
            {
                Debug.LogError("本地文件夹不存在: " + Path.Combine(Application.streamingAssetsPath, LocalFolderPath));
                return;
            }

            // 读取本地文件
            if (File.Exists(_localFilePath))
            {
                string localText = File.ReadAllText(_localFilePath);
                SetTextAndAdjustHeight(localText);
            }
            else
            {
                Debug.LogError("本地文件不存在: " + _localFilePath);
            }
        }

        void SetTextAndAdjustHeight(string text)
        {
            text = text.Replace("\\n", "\n");
            concerningData.text = text;
            concerningData.ForceMeshUpdate();
            Bounds textBounds = concerningData.textBounds;
            RectTransform rectTransform = concerningData.GetComponent<RectTransform>();
            rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, textBounds.size.y);
        }
    }
}