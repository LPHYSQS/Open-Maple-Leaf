using System.Collections.Generic;
using UnityEngine;

namespace Data
{
    public class GameData : MonoBehaviour
    {
        public static bool isOffline;// 是否为离线模式
        
        [System.Serializable]
        public class SoftwareData
        {
            public string softwareName;
            public string type;
            public string softwareDetail;
            public string softwareNetworkDiskUrl;
            public string downloadUrl;
            public string comments;
        }

        [System.Serializable]
        public class SoftwareDataList
        {
            public List<SoftwareData> data;
        }
    }
}
