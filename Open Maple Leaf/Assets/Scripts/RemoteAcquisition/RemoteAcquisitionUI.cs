using UnityEngine;
using UnityEngine.UI;

namespace RemoteAcquisition
{
    // 远程获取UI控制
    public class RemoteAcquisitionUI : MonoBehaviour
    {
        public Button filterBtn;
        public GameObject filterContent;
       // public static bool isFilterOpen;

        public Button refreshLoading;// 刷新按钮
        private RemoteDataAcquisition _remoteDataAcquisition;
        private SearchContent _searchContent;
        private void Start()
        {
            _remoteDataAcquisition= GetComponent<RemoteDataAcquisition>();
            _searchContent = GetComponent<SearchContent>();
            filterBtn.onClick.AddListener(() =>
            {
                if (filterContent.activeSelf==false)
                {
                    filterContent.SetActive(true);
                }
            });
            refreshLoading.onClick.AddListener(() =>
            {
                _remoteDataAcquisition.Reload();
                _searchContent.searchInput.text = "";
            });
        }
    }
}
