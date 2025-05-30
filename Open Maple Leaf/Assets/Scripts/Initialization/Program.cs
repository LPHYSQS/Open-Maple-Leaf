using System.IO;
using UnityEngine;

namespace Initialization
{
    public class Program : MonoBehaviour
    {
        void Awake()
        {
            // 获取当前Unity应用程序的路径
            var currentDirectory = Application.dataPath;
        
            // 获取上级目录
            var parentDirectory = Directory.GetParent(currentDirectory)?.FullName;

            if (!string.IsNullOrEmpty(parentDirectory))
            {
                // 获取所有 .bak 文件
                var bakFiles = Directory.GetFiles(parentDirectory, "*.bak");

                if (bakFiles.Length > 0)
                {
                    // 遍历并删除 .bak 文件
                    foreach (var bakFile in bakFiles)
                    {
                        try
                        {
                            File.Delete(bakFile);
                            Debug.Log($"已删除.bak文件: {bakFile}");
                        }
                        catch (IOException ioEx)
                        {
                            Debug.LogError($"IOException 时删除.bak文件: {bakFile}, {ioEx.Message}");
                        }
                        catch (System.Exception ex)
                        {
                            Debug.LogError($"删除文件时.bak异常: {bakFile}, {ex.Message}");
                        }
                    }
                }
                else
                {
                    Debug.Log("没有找到 .bak 文件.");
                }
                
            }
            else
            {
                Debug.LogError("父目录为 null 或为空.");
            }
            
        }
    }
}
