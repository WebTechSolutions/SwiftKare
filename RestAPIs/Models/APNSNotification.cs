using Newtonsoft.Json;

using PushNotiProject.Models;

namespace PushNotiProject.Models
{
    [JsonObject(MemberSerialization.OptIn)]
    public class APNSNotification
    {
        [JsonProperty("priority")]
        public int Priority { get; set; }

        [JsonProperty("content-available")]
        public bool ContentAvailable { get; set; }

        [JsonProperty("aps")]
        public PN_AppModel Aps { get; set; }
    }
}