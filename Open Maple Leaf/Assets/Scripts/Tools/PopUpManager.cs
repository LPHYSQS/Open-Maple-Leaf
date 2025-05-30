using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Tools
{
    public class PopUpManager
    {
        private readonly GameObject _popUpPrefab;
        private readonly Transform _canvasTransform;
        private readonly string _popUpText;

        /// <summary>
        /// PopUpManager注意
        /// </summary>
        /// <param name="text">弹窗显示文字</param>
        /// <param name="canvasName">Canvas所在位置名称</param>
        /// <param name="popUp">Resources文件夹下的弹窗预设体</param>
        public PopUpManager(string text,string canvasName,string popUp)
        {
            _popUpText = text;
            _popUpPrefab = Resources.Load<GameObject>(popUp); // 确认PopUp预设体被正确加载
            _canvasTransform = GameObject.Find(canvasName).transform; // 将你的Canvas名称替换为真实的Canvas名称
        }

        // ReSharper disable Unity.PerformanceAnalysis
        public void SpawnPopUp()
        {
            if (_canvasTransform != null && _popUpPrefab != null)
            {
                var newPopUp = Object.Instantiate(_popUpPrefab, _canvasTransform, false);
                // 获取 Text 子物体
                var popUpText = newPopUp.GetComponentInChildren<TMP_Text>();
                popUpText.text = _popUpText;
                // 设置初始透明度为0
                var startColor = new Color(0f, 0f, 0f, 0f);
                newPopUp.GetComponent<CanvasGroup>().alpha = startColor.a;
                popUpText.color = startColor;
                // 启动协程来实现渐显效果
                var mono = newPopUp.GetComponent<MonoBehaviour>();
                mono.StartCoroutine(FadeInOut(newPopUp.GetComponent<CanvasGroup>(), popUpText, startColor.a, 1f, 1f, 0.5f));
            }
            else
            {
                Debug.LogError("Canvas Transform或PopUp Prefab未正确指定或不存在！");
            }
        }
        
        
        // 协程实现渐显和渐隐效果
        private static IEnumerator FadeInOut(CanvasGroup canvasGroup, Graphic textComponent, float startAlpha, float targetAlpha, float stayDuration, float fadeDuration)
        {
            // 渐显
            var currentTime = 0f;
            while (currentTime < fadeDuration)
            {
                currentTime += Time.deltaTime;
                var alpha = Mathf.Lerp(startAlpha, targetAlpha, currentTime / fadeDuration);

                // 设置 CanvasGroup 的透明度
                canvasGroup.alpha = alpha;

                // 设置 Text 子物体的透明度
                var textColor = textComponent.color;
                textColor.a = alpha;
                textComponent.color = textColor;

                yield return null;
            }
            // 保持显示
            yield return new WaitForSeconds(stayDuration);
            // 渐隐
            currentTime = 0f;
            while (currentTime < fadeDuration)
            {
                currentTime += Time.deltaTime;
                var alpha = Mathf.Lerp(targetAlpha, startAlpha, currentTime / fadeDuration);
                // 设置 CanvasGroup 的透明度
                canvasGroup.alpha = alpha;
                // 设置 Text 子物体的透明度
                var textColor = textComponent.color;
                textColor.a = alpha;
                textComponent.color = textColor;
                yield return null;
            }
            // 销毁物体
            Object.Destroy(canvasGroup.gameObject);
        }
        public static void ShowPopUp(string message, string canvasName, string popUpPrefabName)
        {
            // Implement pop-up showing logic here, to avoid duplicating the PopUpManager instantiation
            // Assuming you have a method to show a pop-up with the given parameters
            var popUpManager = new PopUpManager(message, canvasName, popUpPrefabName);
            popUpManager.SpawnPopUp();
        }
    }
}
