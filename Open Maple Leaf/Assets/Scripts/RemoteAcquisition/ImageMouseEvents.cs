using UnityEngine;
using UnityEngine.EventSystems;

namespace RemoteAcquisition
{
    public class ImageMouseEvents : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        public RemoteAcquisitionUI remoteAcquisitionUI;
        private bool _isMouseOver;

        // 当指针进入 Image 时，将调用此方法
        public void OnPointerEnter(PointerEventData eventData)
        {
            _isMouseOver = true;
        }

        // 当指针退出 Image 时，将调用此方法
        public void OnPointerExit(PointerEventData eventData)
        {
            _isMouseOver = false;
        }
        
        void Update()
        {
            // 检测鼠标是否未在图像上，并且鼠标左键是否已单击
            if (!_isMouseOver && Input.GetMouseButtonDown(0))
            {
                remoteAcquisitionUI.filterContent.SetActive(false);
            }
        }
    }
}
