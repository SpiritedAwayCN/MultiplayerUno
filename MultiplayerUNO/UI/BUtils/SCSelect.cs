using MultiplayerUNO.UI.Players;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiplayerUNO.UI.BUtils {
    /// <summary>
    /// 服务端、客户端选择的一些配置信息
    /// </summary>
    public static class SCSelect {
        /// <summary>
        /// 是服务器还是客户端
        /// </summary>
        public static PlayerKind PlayerKind = PlayerKind.Client;

        /// <summary>
        /// 默认信息(域名、端口、名字)
        /// </summary>
        public const string DEFAULT_HOST = "127.0.0.1",
                            DEFAULT_PORT = "25564",
                            DEFAULT_NAME = "Alice";

        /// <summary>
        /// 描述信息(域名、端口)
        /// </summary>
        private static string hostInfo = "域名信息", portInfo = "端口信息";
        public static string HostInfo {
            get =>
                (PlayerKind == PlayerKind.Client ? "服务器" : "自身") + hostInfo;
        }
        public static string PortInfo {
            get =>
                (PlayerKind == PlayerKind.Client ? "服务器" : "自身") + portInfo;
        }

        /// <summary>
        /// 用户信息(域名、端口、名字)
        /// </summary>
        public static string UserHost, UserPort, UserName;
    }
}
