using System;
using Newtonsoft.Json;
using UnityEngine;

namespace OKRT.TechnokomTestTask.Main.Code.Data
{
    /// <summary>
    /// The <see cref="OtherPayoutData"/> class.
    /// </summary>
    [Serializable]
    internal sealed class OtherPayoutData
    {
        [JsonProperty("category")]
        [field: SerializeField] public string Category { get; set; }

        [JsonProperty("icon")]
        [field: SerializeField] public string Icon { get; set; }

        [JsonProperty("requiredLevel")]
        [field: SerializeField] public int RequiredLevel { get; set; }
    }
}
