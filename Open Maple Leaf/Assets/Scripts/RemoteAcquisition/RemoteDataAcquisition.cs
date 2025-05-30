using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Data;
using Tools;
using UnityEngine;
using UnityEngine.Networking;

namespace RemoteAcquisition
{
    // 数据获取与资源加载
    public class RemoteDataAcquisition : MonoBehaviour
    {
        public Settings.Initialization initialization; // 将此变量在 Inspector 中链接到 Initialization 实例
        private string _localFilePath;
        private string _jsonData;
        public GameObject remoteAcquisitionContent;
        public GameObject filterContent;
        public GameObject load;
        private readonly HashSet<string> _types = new(); // 存储类型集合

        private void Start()
        {
            if (initialization != null)
            {
                _localFilePath = Path.Combine(Application.streamingAssetsPath, "GetItRemotely/SoftwareInformation.json");
                StartCoroutine(GetJsonData(initialization.GetUrl()));
            }
            else
            {
                Debug.LogError("Initialization instance is not set.");
            }
        }

        // ReSharper disable Unity.PerformanceAnalysis
        private IEnumerator GetJsonData(string url)
        {
            // 如果离线模式开启，则只读取本地文件
            if (GameData.isOffline)
            {
                Debug.Log("离线模式开启，仅读取本地文件。");
                if (File.Exists(_localFilePath))
                {
                    string localJson = File.ReadAllText(_localFilePath);
                    Debug.Log("读取本地 JSON: " + localJson);
                    _jsonData = localJson;
                }
                else
                {
                    Debug.LogError("本地文件不存在: " + _localFilePath);
                    var popUpManager = new PopUpManager("本地文件不存在", "Canvas", "PopUp");
                    popUpManager.SpawnPopUp();
                }

                ParseAndInstantiateJsonData();
                Debug.Log("不同类型的数量：" + _types.Count);
                LoadFilterPresets();

                // 隐藏加载指示器
                if (load != null)
                {
                    load.SetActive(false);
                }
                yield break; // 结束协程，不再进行网络请求
            }

            // 显示加载指示器
            if (load != null)
            {
                load.SetActive(true);
            }
            print("链接：" + url);
            string timeStamp = DateTime.Now.ToString("yyyyMMddHHmmssfff");
            string fullUrl = url + "?t=" + timeStamp; // 添加时间戳以避免缓存问题
            using UnityWebRequest request = UnityWebRequest.Get(fullUrl);
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Error: " + request.error);
                var popUpManager = new PopUpManager("网络获取失败，尝试本地读取", "Canvas", "PopUp");
                popUpManager.SpawnPopUp();
                if (File.Exists(_localFilePath))
                {
                    string localJson = File.ReadAllText(_localFilePath);
                    Debug.Log("读取本地 JSON: " + localJson);
                    _jsonData = localJson;
                }
                else
                {
                    Debug.LogError("本地文件不存在: " + _localFilePath);
                    popUpManager = new PopUpManager("获取失败，本地文件不存在", "Canvas", "PopUp");
                    popUpManager.SpawnPopUp();
                }
            }
            else
            {
                string jsonData = request.downloadHandler.text;
                _jsonData = jsonData;
                SaveJsonToFile(jsonData);
                var popUpManager = new PopUpManager("网络获取同步成功", "Canvas", "PopUp");
                popUpManager.SpawnPopUp();
            }

            ParseAndInstantiateJsonData();
            Debug.Log("不同类型的数量：" + _types.Count);
            LoadFilterPresets();

            // 隐藏加载指示器
            if (load != null)
            {
                load.SetActive(false);
            }
        }
        private void SaveJsonToFile(string jsonData)
        {
            try
            {
                string directoryPath = Path.GetDirectoryName(_localFilePath);
                if (!Directory.Exists(directoryPath))
                {
                    if (directoryPath != null) Directory.CreateDirectory(directoryPath);
                }

                File.WriteAllText(_localFilePath, jsonData);
                Debug.Log("JSON 数据已保存到本地文件: " + _localFilePath);
            }
            catch (Exception ex)
            {
                Debug.LogError("保存 JSON 数据到本地文件时出错: " + ex.Message);
                var popUpManager = new PopUpManager("保存配置失败", "Canvas", "PopUp");
                popUpManager.SpawnPopUp();
            }
        }

        private void ParseAndInstantiateJsonData()
        {
            if (string.IsNullOrEmpty(_jsonData))
            {
                Debug.LogError("JSON 数据为空或无效");
                return;
            }

            var softwareDataList = JsonUtility.FromJson<GameData.SoftwareDataList>(_jsonData);
            if (softwareDataList?.data == null)
            {
                Debug.LogError("解析JSON数据失败");
                return;
            }
            if (remoteAcquisitionContent == null)
            {
                Debug.LogError("Content对象为null");
                return;
            }

            var prefab = Resources.Load<GameObject>("RemoteAcquisition/GetPresetsRemotely");
            if (prefab == null)
            {
                Debug.LogError("找不到预设体'RemoteAcquisition/GetPresetsRemotely'");
                return;
            }

            foreach (var softwareData in softwareDataList.data)
            {
                _types.Add(softwareData.type);

                var instance = Instantiate(prefab, remoteAcquisitionContent.transform);
                var script = instance.GetComponent<GetPresetsRemotely>();
                if (script != null)
                {
                    script.softwareName = softwareData.softwareName;
                    script.type = softwareData.type;
                    script.softwareDetail = softwareData.softwareDetail;
                    script.softwareNetworkDiskUrl = softwareData.softwareNetworkDiskUrl;
                    script.downloadUrl = softwareData.downloadUrl;
                    script.comments = softwareData.comments;
                    script.LoadInitialization();
                }
                else
                {
                    Debug.LogError("预设体上没有找到'GetPresetsRemotely'脚本");
                }
            }
        }

        private void LoadFilterPresets()
        {
            var filterPrefab = Resources.Load<GameObject>("RemoteAcquisition/FilterPresets");
            if (filterPrefab == null)
            {
                Debug.LogError("找不到预设体'RemoteAcquisition/FilterPresets'");
                return;
            }
            foreach (var type in _types)
            {
                var instance = Instantiate(filterPrefab, filterContent.transform);
                var script = instance.GetComponent<FilterPresets>();
                if (script != null)
                {
                    script.type = type;
                    script.LoadInitialization();
                }
                else
                {
                    Debug.LogError("预设体上没有找到'FilterPresets'脚本");
                }
            }
        }
        
        //用于重新加载数据（包括网络获取，包括刷新）
        public void Reload()
        {
            foreach (Transform child in remoteAcquisitionContent.transform)
            {
                Destroy(child.gameObject);
            }
            foreach (Transform child in filterContent.transform)
            {
                Destroy(child.gameObject);
            }
            SearchContent.TYPES.Clear();
            _localFilePath = Path.Combine(Application.streamingAssetsPath, "GetItRemotely/SoftwareInformation.json");
            StartCoroutine(GetJsonData(initialization.GetUrl()));
        }
    }
}
