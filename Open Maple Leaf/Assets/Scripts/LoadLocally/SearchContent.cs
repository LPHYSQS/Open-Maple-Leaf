using RemoteAcquisition;
using TMPro;
using UnityEngine;

namespace LoadLocally
{
    public class SearchContent : MonoBehaviour
    {
        public TMP_InputField searchInput;
        public GameObject remoteAcquisitionContent;
       
        private void Start()
        {
            // 为searchInput的onValueChanged事件添加监听器
            searchInput.onValueChanged.AddListener(OnSearchValueChanged);
        }

        // 这是当搜索框中的文本改变时被调用的方法
        private void OnSearchValueChanged(string searchText)
        {
            // 将搜索文本转换为小写
            searchText = searchText.ToLower();

            // 遍历remoteAcquisitionContent的所有子物体
            for (int i = 0; i < remoteAcquisitionContent.transform.childCount; i++)
            {
                GameObject child = remoteAcquisitionContent.transform.GetChild(i).gameObject;
                GetLoadLocally presets = child.GetComponent<GetLoadLocally>();

                if (presets != null)
                {
                    // 将子物体的softwareName转换为小写并检查是否包含搜索文本
                    if (string.IsNullOrEmpty(searchText) || presets.localName.ToLower().Contains(searchText))
                    {
                        child.SetActive(true); // 匹配则显示
                    }
                    else
                    {
                        child.SetActive(false); // 不匹配则隐藏
                    }
                }
            }
        }
    }
}
