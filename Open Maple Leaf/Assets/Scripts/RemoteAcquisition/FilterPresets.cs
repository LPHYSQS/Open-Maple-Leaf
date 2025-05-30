using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RemoteAcquisition
{
    public class FilterPresets : MonoBehaviour
    {
        public string type;
        public TMP_Text typeText;
        public bool isFilterOpen;
        public static event Action TypeListChanged;
        
        public void LoadInitialization()
        {
            typeText.text=type;
        }
        private void Start()
        {
            GetComponent<Button>().onClick.AddListener(() =>
            {
                isFilterOpen=!isFilterOpen;
                if (isFilterOpen)
                {
                    GetComponent<Image>().color = new Color(0, 0.7329025f, 1, 1f);
                    GetComponentInChildren<TMP_Text>().color = new Color(1, 1f, 1, 1f);
                    SearchContent.TYPES.Add(type);
                    TypeListChanged?.Invoke();
                }
                else
                {
                    GetComponent<Image>().color = new Color(1f, 1f, 1f, 1f);
                    GetComponentInChildren<TMP_Text>().color = new Color(0, 0f, 0, 1f);
                    SearchContent.TYPES.Remove(type);
                    TypeListChanged?.Invoke();
                }
            });
        }
    }
}