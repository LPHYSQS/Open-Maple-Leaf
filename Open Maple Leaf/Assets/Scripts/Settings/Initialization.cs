using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Xml;
using Data;
using PimDeWitte.UnityMainThreadDispatcher;
using RemoteAcquisition;
using TMPro;
using Tools;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;

namespace Settings
{
    public class Initialization : MonoBehaviour
    {
        public GameObject settingsPanel;
        
        private string _xmlFilePath;// 设置XML文件路径
        
        private string _url;  // 动态读取的URL
        
        private string _offline;// 离线
        
        public TMP_InputField softwareRepositoriesUrl;//软件仓库URL
        
        public Toggle offline;// 是否开启离线模式

        public Button restoreDefault;// 还原默认
        public Button applied;// 应用
        
        public RemoteDataAcquisition remoteDataAcquisition;// 远程数据获取
        public SearchContent searchContent;
        private bool _whetherTheStoreIsOfflineByDefault;

        public Button locationButton;//打开文件所在位置
        public Button textEditorButton;//文本编辑器打开
        public Button infoEditorButton;//内置编辑器打开
        public UnityMainThreadDispatcher mainThreadDispatcher;//回归主线程
        
        private void Awake()
        {
            ReadSettingsFile();
            GetComponent<Button>().onClick.AddListener(() =>
            {
                _whetherTheStoreIsOfflineByDefault = offline.isOn;
            });
           
            applied.onClick.AddListener(() =>
            {
                DynamicChanges(softwareRepositoriesUrl.text, offline.isOn);
                //settingsPanel.SetActive(false);
                var popUpManager = new PopUpManager("应用完成", "Canvas", "PopUp");
                popUpManager.SpawnPopUp();
            });
            restoreDefault.onClick.AddListener(() =>
            {
                DynamicChanges("https://raw.githubusercontent.com/LPHYSQS/Maple-Leaf/main/SoftwareInformation.json",  false);
                //settingsPanel.SetActive(false);
                var popUpManager = new PopUpManager("恢复完成", "Canvas", "PopUp");
                popUpManager.SpawnPopUp();
            });
            
            textEditorButton.onClick.AddListener(() =>
            {
                // 获取StreamingAssets文件夹下的路径
                string streamingAssetsPath = Application.streamingAssetsPath;
        
                // 拼接 Notepad3.exe 和 SoftwareInformation.json 的路径
                string notepad3Path = Path.Combine(streamingAssetsPath, "Notepad3", "Notepad3.exe");
                string jsonFilePath = Path.Combine(streamingAssetsPath, "GetItRemotely", "SoftwareInformation.json");
        
                if (File.Exists(notepad3Path) && File.Exists(jsonFilePath))
                {
                    // 创建一个进程启动信息
                    ProcessStartInfo startInfo = new ProcessStartInfo
                    {
                        FileName = notepad3Path,
                        Arguments = jsonFilePath,  // 将要打开的文件作为参数传入
                        WorkingDirectory = Path.Combine(streamingAssetsPath, "Notepad3"),  // 设置工作目录
                        UseShellExecute = false,
                    };

                    // 创建并启动进程
                    Process process = new Process();
                    process.StartInfo = startInfo;
                    process.EnableRaisingEvents = true; // 启用 Exited 事件
                    process.Exited += (_, _) => 
                    {
                        Debug.Log("Notepad3编辑完成。");
                        
                        mainThreadDispatcher.Enqueue(() =>
                        {
                            remoteDataAcquisition.Reload();
                            searchContent.searchInput.text = "";
                        });
                    };
            
                    // 启动 Notepad3.exe
                    process.Start();
                }
                else
                {
                    //Debug.LogError("Notepad3 或 SoftwareInformation.json 文件不存在！");
                    PopUpManager.ShowPopUp("启动错误", "Canvas", "PopUp");
                }
            });
            infoEditorButton.onClick.AddListener(() =>
            {
                // 获取StreamingAssets文件夹下的路径
                string streamingAssetsPath = Application.streamingAssetsPath;
        
                // 拼接 disposition.exe 的路径
                string dispositionExePath = Path.Combine(streamingAssetsPath, "Disposition", "disposition.exe");
        
                if (File.Exists(dispositionExePath))
                {
                    // 创建一个进程启动信息
                    ProcessStartInfo startInfo = new ProcessStartInfo
                    {
                        FileName = dispositionExePath,
                        WorkingDirectory = Path.Combine(streamingAssetsPath, "Disposition"),  // 设置工作目录为 Disposition 文件夹
                        UseShellExecute = false,
                    };

                    // 创建并启动进程
                    Process process = new Process();
                    process.StartInfo = startInfo;
                    process.EnableRaisingEvents = true; // 启用 Exited 事件
                    process.Exited += (_, _) => 
                    {
                        Debug.Log("disposition编辑完成。");
                        mainThreadDispatcher.Enqueue(() =>
                        {
                            remoteDataAcquisition.Reload();
                            searchContent.searchInput.text = "";
                        });
                    };

                    // 启动 disposition.exe
                    process.Start();
                }
                else
                {
                    Debug.LogError("disposition.exe 文件不存在！");
                }
            });
            
            locationButton.onClick.AddListener(() =>
            {
                // 获取完整的文件路径
               var filePath = Path.Combine(Application.streamingAssetsPath, "GetItRemotely", "SoftwareInformation.json");
            
               if (File.Exists(filePath))
               {
                   // 处理路径中的反斜杠和转义字符
                   string correctPath = filePath.Replace("/", "\\");
            
                   // 使用 explorer 来打开文件所在的文件夹并选中该文件
                   Process.Start("explorer.exe", $"/select,\"{correctPath}\"");
               }
               else
               {
                   Debug.LogError("文件未找到: " + filePath);
               }
            });
            
        }


        // 动态修改
        private void DynamicChanges(string url, bool isOffline)
        {
            //写
            WriteSettingsFile(url,isOffline);
            //读
            ReadSettingsFile();

            if (isOffline!=_whetherTheStoreIsOfflineByDefault)
            {
                remoteDataAcquisition.Reload();
                searchContent.searchInput.text = "";
            }
        }
        
        // 写入设置文件
        private void WriteSettingsFile(string url, bool isOffline)
        {
            // 获取StreamingAssets文件夹下的完整路径
            _xmlFilePath = Path.Combine(Application.streamingAssetsPath, "SoftwareRepositories/Settings.xml");

            // 创建一个XML文档对象
            XmlDocument xmlDoc = new XmlDocument();
    
            // 如果XML文件存在，加载现有文件
            if (File.Exists(_xmlFilePath))
            {
                xmlDoc.Load(_xmlFilePath);
            }
            else
            {
                // 如果文件不存在，创建根节点<Settings>
                XmlDeclaration xmlDeclaration = xmlDoc.CreateXmlDeclaration("1.0", "UTF-8", null);
                XmlElement root = xmlDoc.DocumentElement;
                xmlDoc.InsertBefore(xmlDeclaration, root);
        
                XmlElement settingsElement = xmlDoc.CreateElement(string.Empty, "Settings", string.Empty);
                xmlDoc.AppendChild(settingsElement);
            }

            // 查找或创建Url节点
            XmlNode urlNode = xmlDoc.SelectSingleNode("Settings/Url");
            if (urlNode == null)
            {
                urlNode = xmlDoc.CreateElement("Url");
                xmlDoc.DocumentElement?.AppendChild(urlNode);
            }
            urlNode.InnerText = url;

            // 查找或创建Offline节点
            XmlNode offlineNode = xmlDoc.SelectSingleNode("Settings/Offline");
            if (offlineNode == null)
            {
                offlineNode = xmlDoc.CreateElement("Offline");
                xmlDoc.DocumentElement?.AppendChild(offlineNode);
            }
            offlineNode.InnerText = isOffline ? "true" : "false";

            // 保存XML文件
            xmlDoc.Save(_xmlFilePath);
        }
        
        
        
        // 读取设置文件
        private void ReadSettingsFile()
        {
            // 获取StreamingAssets文件夹下的完整路径
            _xmlFilePath = Path.Combine(Application.streamingAssetsPath, "SoftwareRepositories/Settings.xml");

            // 启动协程读取XML文件
            StartCoroutine(LoadXMLFile());
        }
        public string GetUrl()
        {
            return _url;
        }

        private IEnumerator LoadXMLFile()
        {
            string xmlContent = "";

            // 如果在Android或WebGL平台，使用UnityWebRequest
            if (_xmlFilePath.Contains("://") || _xmlFilePath.Contains(":///"))
            {
                using var uwr = UnityWebRequest.Get(_xmlFilePath);
                yield return uwr.SendWebRequest();

                if (uwr.result is UnityWebRequest.Result.ConnectionError or UnityWebRequest.Result.ProtocolError)
                {
                    Debug.LogError("Error reading XML file: " + uwr.error);
                }
                else
                {
                    xmlContent = uwr.downloadHandler.text;
                }
            }
            else
            {
                // 其他平台直接读取文件
                if (File.Exists(_xmlFilePath))
                {
                    xmlContent = File.ReadAllText(_xmlFilePath);
                }
                else
                {
                    Debug.LogError("XML file not found at path: " + _xmlFilePath);
                    yield break;
                }
            }

            // 解析并打印XML内容
            if (!string.IsNullOrEmpty(xmlContent))
            {
                ParseXML(xmlContent);
            }
        }

        private void ParseXML(string xmlContent)
        {
            // 创建XML文档对象
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xmlContent);

            // 读取Url节点的内容
            XmlNode urlNode = xmlDoc.SelectSingleNode("Settings/Url");
            _url = urlNode != null ? urlNode.InnerText : "Url not found";
            

            // 读取Offline节点的内容
            XmlNode offlineNode = xmlDoc.SelectSingleNode("Settings/Offline");
            _offline = offlineNode != null ? offlineNode.InnerText : "Offline not found";
            
            
            // 打印读取的内容
            Debug.Log("软件仓库链接: " + _url);
            
            Debug.Log("离线: " + _offline);
            
            softwareRepositoriesUrl.text=_url;

            switch (_offline)
            {
                case "true":
                    GameData.isOffline = true;
                    offline.isOn = true;
                    break;
                case "false":
                    GameData.isOffline = false;
                    offline.isOn = false;
                    break;
                default:
                    GameData.isOffline = false;
                    offline.isOn = false;
                    break;
            }
        }
    }
}