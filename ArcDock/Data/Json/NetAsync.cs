using Newtonsoft.Json;

namespace ArcDock.Data.Json
{
    /// <summary>
    /// Socket传输当前模板配置所用的JSON解析类
    /// </summary>
    [JsonObject]
    public class NetAsync
    {
        /// <summary>
        /// 数据包版本，用于网络传输客户端校正
        /// </summary>
        [JsonProperty("version")]
        public int Version { get; set; }

        /// <summary>
        /// 返回代码
        /// </summary>
        [JsonProperty("code")]
        public string Code { get; set; }

        /// <summary>
        /// 返回消息
        /// </summary>
        [JsonProperty("message")]
        public string Message { get; set; }

        /// <summary>
        /// 认证Token
        /// </summary>
        [JsonProperty("token")]
        public string Token { get; set; }

        /// <summary>
        /// 模板文件大小
        /// </summary>
        [JsonProperty("size")]
        public int Size { get; set; }

        /// <summary>
        /// 全局配置对象
        /// </summary>
        [JsonProperty("global_setting")]
        public GlobalSettingJson GlobalSetting { get; set; }
    }
}
