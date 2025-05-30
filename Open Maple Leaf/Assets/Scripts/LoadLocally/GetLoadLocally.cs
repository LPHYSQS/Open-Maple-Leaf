using System;
using System.Diagnostics;
using System.IO;
using TMPro;
using Tools;
using UnityEngine;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;

namespace LoadLocally
{
    public class GetLoadLocally : MonoBehaviour
    {
        public string localName;
        public Image localIcon;
        public TMP_Text localNameText;
        public Button openFileLocation;
        public Button delBtn;
        public Button installation;
        public static event Action<string> OnDelete;

        private void Start()
        {
            delBtn.onClick.AddListener(() =>
            {
                OnDelete?.Invoke(localName);
            });
            installation.onClick.AddListener(OpenWithDefaultApplication);
            openFileLocation.onClick.AddListener(OpenFileLocation);  // 添加事件监听器
        }

        public void LoadInitialization()
        {
            // 设置文件名
            localNameText.text = localName;

            // 加载文件图标
            var icon = new IconLoader();
            string path = Path.Combine(Application.streamingAssetsPath, "Programs", localName);
            localIcon.sprite = icon.LoadIcon(path);

            // 获取文件大小并显示
            if (File.Exists(path))
            {
                FileInfo fileInfo = new FileInfo(path);
                // 将文件大小转换为MB，保留两位小数
                double fileSizeInMb = fileInfo.Length / (1024.0 * 1024.0);
                // 更新localNameText以显示文件名和大小
                localNameText.text = $"{localName} ({fileSizeInMb:F2} MB)";
            }
            else
            {
                // 文件不存在时处理
                Debug.LogError($"文件 {path} 不存在");
                localNameText.text = $"{localName} (文件不存在)";
            }
        }


        private void OpenWithDefaultApplication()
        {
            string path = Path.Combine(Application.streamingAssetsPath, "Programs", localName);

            if (File.Exists(path))
            {
                try
                {
                    Process.Start(new ProcessStartInfo(path) { UseShellExecute = true });
                }
                catch (Exception ex)
                {
                    Debug.LogError($"无法打开文件 {path} 使用默认应用程序: {ex.Message}");
                }
            }
            else
            {
                PopUpManager.ShowPopUp("文件不存在", "Canvas", "PopUp");
            }
        }

        //打开文件位置并选中文件
        private void OpenFileLocation()
        {
            string path = Path.Combine(Application.streamingAssetsPath, "Programs", localName);

            if (File.Exists(path))
            {
                try
                {
                    // 获取文件的完整路径
                    string fullPath = Path.GetFullPath(path);
                    //Debug.Log($"文件完整路径: {fullPath}");
                    // 打开文件所在目录并选中该文件
                    Process.Start(new ProcessStartInfo("explorer.exe", $"/select,\"{fullPath}\""));
                }
                catch (Exception ex)
                {
                    Debug.LogError($"无法打开文件位置 {path}: {ex.Message}");
                }
            }
            else
            {
                PopUpManager.ShowPopUp("文件不存在", "Canvas", "PopUp");
            }
        }
    }
}
