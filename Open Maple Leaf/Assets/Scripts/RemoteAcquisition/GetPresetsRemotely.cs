using System;
using System.Collections;
using System.IO;
using TMPro;
using Tools;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace RemoteAcquisition
{
    public class GetPresetsRemotely : MonoBehaviour
    {
        public string softwareName;
        public string type;
        public string softwareDetail;
        public string softwareNetworkDiskUrl;
        public string downloadUrl;
        public string comments;

        public TMP_Text displayName;
        public Image downloadProgress;
        public Button softwareDetailButton;
        public Button softwareNetworkDiskButton;
        public Button downloadButton;
        public TMP_Text downloadProgressTMPText;
        
        private string _filePath;
        private bool _isDownloading;
        public static event Action DownloadEndOfEvent;

        public void LoadInitialization()
        {
            displayName.text = softwareName;
            softwareDetailButton.gameObject.SetActive(!string.IsNullOrEmpty(softwareDetail));
            softwareNetworkDiskButton.gameObject.SetActive(!string.IsNullOrEmpty(softwareNetworkDiskUrl));
            downloadButton.gameObject.SetActive(!string.IsNullOrEmpty(downloadUrl));
        }

        private void Start()
        {
            downloadButton.onClick.AddListener(OnDownloadButtonClick);
            softwareDetailButton.onClick.AddListener(() => { Application.OpenURL(softwareDetail); });
            softwareNetworkDiskButton.onClick.AddListener(() => { Application.OpenURL(softwareNetworkDiskUrl); });
        }

        private static string GetFileNameFromURL(string url)
        {
            var lastSlashIndex = url.LastIndexOf('/');
            return url.Substring(lastSlashIndex + 1);
        }

        private void OnDownloadButtonClick()
        {
            
            // 查找Canvas下名为TextDisplay的子对象
            var textDisplay = GameObject.Find("Canvas/TextDisplay");

            if (textDisplay != null)
            {
                // 使TextDisplay隐藏
                textDisplay.SetActive(false);
            }
            else
            {
                Debug.LogError("TextDisplay对象未找到！");
            }
            
            if (_isDownloading)
            {
                Debug.Log("Already downloading...");
                PopUpManager.ShowPopUp("下载中...", "Canvas", "PopUp");
                return;
            }

            var directoryPath = Path.Combine(Application.streamingAssetsPath, "Programs");
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            _filePath = Path.Combine(directoryPath, GetFileNameFromURL(downloadUrl));
            StartDownload();
        }

        private void StartDownload()
        {
            StartCoroutine(DownloadFile());
        }

        private IEnumerator DownloadFile()
        {
            _isDownloading = true;
            downloadButton.interactable = false;
            downloadButton.gameObject.SetActive(false);
            downloadProgressTMPText.gameObject.SetActive(true);

            using (var webRequest = UnityWebRequest.Get(downloadUrl))
            {
                webRequest.downloadHandler = new DownloadHandlerFile(_filePath);
                var downloadTask = webRequest.SendWebRequest();

                while (!downloadTask.isDone)
                {
                    downloadProgress.fillAmount = downloadTask.progress;
                    downloadProgressTMPText.text= (int)(downloadTask.progress * 100) + "%";
                    yield return null;
                }

                downloadProgress.fillAmount = 1f;
                

                if (webRequest.result != UnityWebRequest.Result.Success)
                {
                    Debug.LogError($"Error downloading file: {webRequest.error}");
                    PopUpManager.ShowPopUp("下载错误", "Canvas", "PopUp");

                    if (File.Exists(_filePath))
                    {
                        File.Delete(_filePath);
                    }
                }
                else
                {
                    Debug.Log("File successfully downloaded to " + _filePath);
                    PopUpManager.ShowPopUp("下载成功", "Canvas", "PopUp");
                }
            }

            _isDownloading = false;
            downloadButton.interactable = true;
            downloadButton.gameObject.SetActive(true);
            downloadProgressTMPText.gameObject.SetActive(false);
            downloadButton.GetComponent<Image>().sprite = downloadButton.GetComponent<MouseHover>().exit;
            downloadProgressTMPText.text = "";
            DownloadEndOfEvent?.Invoke();
            downloadProgress.fillAmount = 0f;

            Debug.Log("DownloadEndOfEvent invoked.");
        }
    }
}
