using System.Collections;
using System.Collections.Generic;
using RemoteAcquisition;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Tools
{
    public class FloatingDisplay : MonoBehaviour
    {
        public TextDisplay textDisplay;
        public Canvas canvas;
        private bool _isPointerInside; // 标志位
        private Coroutine _hideCoroutine; // 协程引用
        private RectTransform _rectTransform;
        public string emergeText;
        private GraphicRaycaster _graphicRaycaster;
        private PointerEventData _pointerEventData;
        private EventSystem _eventSystem;
        private Vector3 _lastMousePos;
        private const float CheckInterval = 0.1f; // 每隔0.1秒检测一次
        private float _nextCheckTime;

        private void Start()
        {
            canvas = GameObject.Find("Canvas").GetComponent<Canvas>();
            textDisplay = canvas.transform.Find("TextDisplay").GetComponent<TextDisplay>();
            _rectTransform = GetComponent<RectTransform>();
            textDisplay.gameObject.SetActive(false); // 确保初始状态为隐藏
            _graphicRaycaster = canvas.GetComponent<GraphicRaycaster>();
            _eventSystem = EventSystem.current;
            _lastMousePos = Input.mousePosition;
            _nextCheckTime = Time.time;
        }

        private void Update()
        {
            Vector3 mousePos = Input.mousePosition;

            // 每隔_checkInterval秒检测一次，或者当鼠标位置发生变化时
            if (Time.time >= _nextCheckTime || mousePos != _lastMousePos)
            {
                _nextCheckTime = Time.time + CheckInterval;
                _lastMousePos = mousePos;

                if (IsPointerOverUIElement(mousePos))
                {
                    if (!_isPointerInside)
                    {
                        _isPointerInside = true;
                        ShowTextDisplay();
                    }
                    UpdateTextDisplayPosition(mousePos);
                }
                else
                {
                    if (_isPointerInside)
                    {
                        _isPointerInside = false;
                        HideTextDisplayWithDelay();
                    }
                }
            }
        }

        private bool IsPointerOverUIElement(Vector3 mousePos)
        {
            _pointerEventData = new PointerEventData(_eventSystem)
            {
                position = mousePos
            };

            var results = new List<RaycastResult>();
            _graphicRaycaster.Raycast(_pointerEventData, results);

            foreach (var result in results)
            {
                if (result.gameObject == gameObject)
                {
                    // 检查是否有任何其他元素在其上方
                    if (results[0].gameObject == gameObject)
                    {
                        return true;
                    }
                    break;
                }
            }
            return false;
        }

        private void ShowTextDisplay()
        {
            if (_hideCoroutine != null)
            {
                StopCoroutine(_hideCoroutine);
                _hideCoroutine = null;
            }

            if (transform.name == "GetPresetsRemotelyTrigger" && transform.parent.GetComponent<GetPresetsRemotely>() != null)
            {
                textDisplay.SetText(transform.parent.GetComponent<GetPresetsRemotely>().comments);
            }
            else
            {
                textDisplay.SetText(emergeText);
            }

            if (transform.name=="Name"&&transform.GetComponent<TMP_Text>()!=null)
            {
                textDisplay.SetText(transform.GetComponent<TMP_Text>().text);
            }
            
            
            textDisplay.gameObject.SetActive(true);
        }

        private void HideTextDisplayWithDelay()
        {
            _hideCoroutine = StartCoroutine(HideAfterDelay());
        }

        private IEnumerator HideAfterDelay()
        {
            if (!_isPointerInside)
            {
                textDisplay.gameObject.SetActive(false);
            }
            yield break;
        }

        private void UpdateTextDisplayPosition(Vector3 mousePos)
        {
            RectTransformUtility.ScreenPointToWorldPointInRectangle(canvas.transform as RectTransform, mousePos, canvas.worldCamera, out Vector3 worldPos);
            textDisplay.transform.position = worldPos;
        }
    }
}
