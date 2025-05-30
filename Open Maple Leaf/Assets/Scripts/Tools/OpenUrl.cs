using UnityEngine;
using UnityEngine.UI;

namespace Tools
{
    public class OpenUrl : MonoBehaviour
    {
        public string url;
        
        private Button _openButton;
        
        private void Start()
        {
            _openButton = GetComponent<Button>();
            _openButton.onClick.AddListener(() => { Application.OpenURL(url);});
        }
    }
}
