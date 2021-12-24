using MultiplayerUNO.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiplayerUNO.Backend.Player
{
    /// <summary>
    /// 本地开服的玩家，房间中有且只有一个
    /// </summary>
    public class LocalPlayer : Player
    {
        protected LocalPlayerAdapter adapter;

        public LocalPlayer(LocalPlayerAdapter localPlayerAdapter)
        {
            adapter = localPlayerAdapter;
            name = adapter.PlayerName;
            sendQueue = adapter.RecvQueue; // 服务器直接发到本地对应的ReceiveQueue
        }
    }
}
