using System;

namespace VuforiaWebServices
{
    [Serializable]
    public class VWSGet
    {
        public string idTarget;
        public string accessKey;
        public string secretKey;
        public string name;
        public float width;
        public string image;
        public int active_flag;
        public string application_metadata;
    }
}