using System;
using UnityEngine;
using System.IO;
using RemoteAcquisition;
using UnityEngine.UI;

namespace LoadLocally
{
    public class LocalDataLoading : MonoBehaviour
    {
        public GameObject localDataContent;

        private void Awake()
        {
            Debug.Log("Awake called. Subscribing to DownloadEndOfEvent.");
            LoadPrefabs();
            GetPresetsRemotely.DownloadEndOfEvent += Reload;
        }

        private void OnDestroy()
        {
            Debug.Log("OnDestroy called. Unsubscribing from DownloadEndOfEvent.");
            GetPresetsRemotely.DownloadEndOfEvent -= Reload;
        }

        private void LoadPrefabs()
        {
            Debug.Log("Loading prefabs...");
            string programsPath = Path.Combine(Application.streamingAssetsPath, "Programs");
            if (!Directory.Exists(programsPath))
            {
                Debug.LogError("Programs directory does not exist: " + programsPath);
                return;
            }

            string[] filePaths = Directory.GetFiles(programsPath);
            GameObject prefab = Resources.Load<GameObject>("LoadLocally/GetLoadLocally");

            if (prefab == null)
            {
                Debug.LogError("Cannot find prefab at path: LoadLocally/GetLoadLocally");
                return;
            }

            string latestFilePath = null;
            DateTime latestFileTime = DateTime.MinValue;
            foreach (var filePath in filePaths)
            {
                if (Path.GetExtension(filePath) == ".meta")
                {
                    continue;
                }

                DateTime fileTime = File.GetLastWriteTime(filePath);
                if (fileTime > latestFileTime)
                {
                    latestFileTime = fileTime;
                    latestFilePath = filePath;
                }
            }

            foreach (var filePath in filePaths)
            {
                if (Path.GetExtension(filePath) == ".meta")
                {
                    continue;
                }

                var instance = Instantiate(prefab, localDataContent.transform, false);
                var fileName = Path.GetFileName(filePath);
                var getLoadLocally = instance.GetComponent<GetLoadLocally>();
                if (getLoadLocally != null)
                {
                    getLoadLocally.localName = fileName;
                    getLoadLocally.LoadInitialization();

                    if (filePath == latestFilePath)
                    {
                        var image = getLoadLocally.GetComponent<Image>();
                        if (image != null)
                        {
                            image.color = new Color(1, 0.4657505f, 0.01568627f);
                        }
                        else
                        {
                            Debug.LogError("Prefab does not contain an Image component.");
                        }
                    }
                }
                else
                {
                    Debug.LogError("Prefab does not contain a GetLoadLocally component.");
                }
            }
        }

        public void Reload()
        {
            Debug.Log("Reload called.");
            foreach (Transform child in localDataContent.transform)
            {
                Destroy(child.gameObject);
            }
            LoadPrefabs();
        }
    }
}
