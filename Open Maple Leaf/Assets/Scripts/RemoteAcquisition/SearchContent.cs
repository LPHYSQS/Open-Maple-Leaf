using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace RemoteAcquisition
{
    // 搜索内容
    public class SearchContent : MonoBehaviour
    {
        public TMP_InputField searchInput;
        public GameObject remoteAcquisitionContent;
        public static readonly List<string> TYPES = new();
        
        private void Start()
        {
            // 为searchInput的onValueChanged事件添加监听器
            searchInput.onValueChanged.AddListener(OnSearchValueChanged);
            FilterPresets.TypeListChanged += MatchTypesAndShow;
        }

        private void OnDestroy()
        {
            FilterPresets.TypeListChanged -= MatchTypesAndShow;
        }


        // 这是当搜索框中的文本改变时被调用的方法
        private void OnSearchValueChanged(string searchText)
        {
            searchText = searchText.ToLower();
            bool typesEmpty = TYPES.Count == 0;

            for (int i = 0; i < remoteAcquisitionContent.transform.childCount; i++)
            {
                GameObject child = remoteAcquisitionContent.transform.GetChild(i).gameObject;
                GetPresetsRemotely presets = child.GetComponent<GetPresetsRemotely>();

                if (presets != null)
                {
                    if (typesEmpty || TYPES.Contains(presets.type)) // 检查 Types 是否为空或者包含子物体的类型
                    {
                        if (string.IsNullOrEmpty(searchText) || presets.softwareName.ToLower().Contains(searchText))
                        {
                            child.SetActive(true); // 匹配则显示
                        }
                        else
                        {
                            child.SetActive(false); // 不匹配则隐藏
                        }
                    }
                    else
                    {
                        child.SetActive(false); // 不匹配类型则隐藏
                    }
                }
            }
        }

        private void MatchTypesAndShow()
        {
            bool typesEmpty = TYPES.Count == 0;
            string searchText = searchInput.text.ToLower(); // 获取搜索框中的文本

            for (int i = 0; i < remoteAcquisitionContent.transform.childCount; i++)
            {
                var child = remoteAcquisitionContent.transform.GetChild(i).gameObject;
                var presets = child.GetComponent<GetPresetsRemotely>();

                if (presets != null)
                {
                    if (string.IsNullOrEmpty(searchText) && (typesEmpty || TYPES.Contains(presets.type))) // 如果搜索框为空，且 Types 不为空或包含子物体的类型
                    {
                        child.SetActive(true); // 显示子物体
                    }
                    else
                    {
                        if ((typesEmpty || TYPES.Contains(presets.type)) && presets.softwareName.ToLower().Contains(searchText)) // 如果搜索框不为空且 Types 不为空或包含子物体的类型，且软件名称包含搜索文本
                        {
                            child.SetActive(true); // 显示子物体
                        }
                        else
                        {
                            child.SetActive(false); // 隐藏子物体
                        }
                    }
                }
            }
        }
    }
}