using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Tools
{
    /// <summary>
    /// 用于控制鼠标移入移出效果(一般用于按钮)
    /// </summary>
    public class MouseHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        public Sprite enter;//鼠标移入Sprite

        public Sprite exit;//鼠标移出Sprite
    
        public float scale = 1.2f;
        public float initialScaling = 1f;
    
        public void OnPointerEnter(PointerEventData eventData)
        {
            //Debug.Log("鼠标移入");
            if (GetComponent<Image>()!=null&&enter!=null)
            {
                GetComponent<Image>().sprite = enter;
            }
            var sequenceUserField1 = DOTween.Sequence();
            sequenceUserField1.Append(transform.DOScale(new Vector3(scale,scale,scale),0.3f));
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            //Debug.Log("鼠标移出");
            if (GetComponent<Image>()!=null&&exit!=null)
            {
                GetComponent<Image>().sprite = exit;
            }
            var sequenceUserField = DOTween.Sequence();
            sequenceUserField.Append(transform.DOScale(new Vector3(initialScaling,initialScaling,initialScaling),0.3f));
        }
    }
}