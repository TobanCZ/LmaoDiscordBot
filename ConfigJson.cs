using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace DiscordBot
{
    class ConfigJson
    {
        [JsonProperty("token")]
        public string Token { get; private set; }
    }
}
