using UnityEngine;
using UnityEngine.UI;

namespace Concerning
{
    public class ConcerningUI : MonoBehaviour
    {
        public Button openConcerning;
        public GameObject concerningPanel;
        public Button closeConcerning;
        
        void Start()
        {
            openConcerning.onClick.AddListener(() =>
            {
                concerningPanel.SetActive(true);
            });
            closeConcerning.onClick.AddListener(() =>
            {
                concerningPanel.SetActive(false);
            });
        }
    }
}
