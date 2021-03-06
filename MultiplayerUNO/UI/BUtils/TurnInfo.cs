using LitJson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MultiplayerUNO.Utils.Card;

namespace MultiplayerUNO.UI.BUtils {
    public class TurnInfo {
        // 一些初始字段(详见 excel)
        public readonly int State, LastCardID, IntInfo, Time, TurnID, QueryID;
        public readonly int[] PlayerCards;
        public readonly JsonData JsonMsg;

        /// <summary> 
        /// 在构造函数内部可能会对一些 GameControl 中有意义的值进行赋值
        /// </summary>
        public TurnInfo(JsonData json) {
            // 检查解析 turninfo 的正确性(OK)
            JsonMsg = json;
            State = (int)json["state"];
            // state=7 平局的消息比较奇怪, 直接不解析了, 在外面解析
            if (State == 7 && (int)(json["turnID"]) == 0) { return; }

            // 注意, 无意义字段可能缺失
            // 有意义也不一定是指定的意义, 需要小心设置, DEBUG 需要谨慎

            LastCardID = (int)json["lastCard"];
            if (State == 1 || State == 3 || State == 5 || (State == 7 && LastCardID != -1)) {
                GameControl.CardChange = (GameControl.LastCardID != LastCardID);
                GameControl.LastCardID = LastCardID;
                GameControl.LastCard = new Utils.Card(LastCardID);
            }

            // ATT State=4 没有 intInfo
            if (!(State == 6 || State == 4 || (State == 1 && LastCardID == -1))) {
                IntInfo = (int)json["intInfo"];
            }

            // ATT State=4 没有 time
            if (State >= 1 && State <= 5 && State != 4) {
                Time = (int)json["time"];
                GameControl.TimeForYou = Time;
            }

            TurnID = (int)json["turnID"];
            GameControl.TurnID = TurnID;

            if ((State == 4 && TurnID == MsgAgency.MainForm.MyID)
                || State == 6) {
                var jsa = json["playerCards"];
                PlayerCards = new int[jsa.Count];
                for (int i = 0; i < jsa.Count; ++i) {
                    PlayerCards[i] = (int)jsa[i];
                }
            } else if (State == 7) {
                // PlayerMap 在外面单独处理了
            }

            if (State == 1 || State == 2 || State == 3 || State == 5) {
                QueryID = (int)json["queryID"];
                GameControl.QueryID = QueryID;
            }

            if (State == 1 || State == 5 || State == 7) {
                if (LastCardID == -1) {
                    // 开局的时候, 上一张牌为 -1
                    // State=7 时可能为 -1(平局, 这个在外面处理)
                    GameControl.LastColor = CardColor.Invalid;
                } else if (GameControl.LastCard.Color == CardColor.Invalid) {
                    GameControl.LastColor = (CardColor)(IntInfo & 0b11);
                } else {
                    GameControl.LastColor = GameControl.LastCard.Color;
                }
            }
        }

        ////////////////////////////////////////////////
        // 注意如下函数的调用都需要由调用者保证属性的存在 //
        // 否则可能出现奇怪的 BUG 或者是奇怪的结果       //
        ////////////////////////////////////////////////

        /// <summary>
        /// 通过 intInfo 获取 playerID
        /// (state = 1,7)
        /// </summary>
        public int GetPlayerID() {
            return IntInfo >> 2;
        }

        /// <summary>
        /// 调用 GetPlayerID() 实现, 使用条件受到其限制
        /// (state = 1,7)
        /// </summary>
        /// <returns></returns>
        public int GetPlayerIndex() {
            return GameControl.PlayerId2PlayerIndex[GetPlayerID()];
        }
    }
}