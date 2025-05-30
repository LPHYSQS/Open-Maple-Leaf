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
    public class LoadLocallyUI : MonoBehaviour
    {
        public Button refreshBtn;
        private LocalDataLoading _localDataLoading;
        public Button openTheFolder;
        
        public GameObject deletePrompt;
        public TMP_Text deletePromptTextName;
        public Button deleteBtn;
        public Button undeleteBtn;
        
        private void Awake()
        {
            _localDataLoading= GetComponent<LocalDataLoading>();
            refreshBtn.onClick.AddListener(() =>
            {
                _localDataLoading.Reload();
            });
            openTheFolder.onClick.AddListener(OpenTheFolder);
            undeleteBtn.onClick.AddListener(()=>
            {
                deletePrompt.SetActive(false);
            });
            deleteBtn.onClick.AddListener((DeleteFile));
            GetLoadLocally.OnDelete += Delete;
        }

        private void OnDestroy()
        {
            GetLoadLocally.OnDelete -= Delete;
        }


        public void OpenTheFolder()
        {
            string folderPath = Path.Combine(Application.streamingAssetsPath, "Programs");

            // 检查文件夹是否存在
            if (!Directory.Exists(folderPath))
            {
                Debug.LogWarning("Programs folder does not exist. Creating folder: " + folderPath);
                Directory.CreateDirectory(folderPath); // 创建文件夹
            }

            try
            {
                // 打开文件夹
                OpenFolder(folderPath);
            }
            catch (Exception ex)
            {
                Debug.LogError("Failed to open folder: " + ex.Message);
            }
        }
        private void OpenFolder(string folderPath)
        {
            Process.Start("explorer.exe", folderPath.Replace("/", "\\"));
        }

        private void Delete(string namePrompt)
        {
            deletePrompt.SetActive(true);
            deletePromptTextName.text = namePrompt;
        }
        
        
        void DeleteFile()
        {
            // 获取 StreamingAssets 文件夹的路径
            string streamingAssetsPath = Application.streamingAssetsPath;
            // 指定要删除的文件相对路径
            string filePath = Path.Combine(streamingAssetsPath, "Programs", deletePromptTextName.text);

            if (File.Exists(filePath))
            {
                // 删除文件
                File.Delete(filePath);
                Debug.Log("文件已成功删除: " + filePath);
                PopUpManager.ShowPopUp("已删除", "Canvas", "PopUp");
            }
            else
            {
                Debug.LogWarning("找不到文件: " + filePath);
                PopUpManager.ShowPopUp("找不到文件", "Canvas", "PopUp");
            }
            deletePrompt.SetActive(false);
            _localDataLoading.Reload();
        }
    }
}
