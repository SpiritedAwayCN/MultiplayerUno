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

        /// <summary>
        /// 在构造函数内部可能会对一些 GameControl 中有意义的值进行赋值
        /// </summary>
        /// <param name="json"></param>
        public TurnInfo(JsonData json) {
            State = (int)json["state"];
            // 注意, 无意义字段可能缺失
            // 有意义也不一定是指定的意义, 需要小心设置, DEBUG 需要谨慎

            LastCardID = (int)json["lastCard"];
            if (State == 1 || State == 3 || State == 5) {
                GameControl.CardChange = (GameControl.LastCardID != LastCardID);
                GameControl.LastCardID = LastCardID;
                GameControl.LastCard = new Utils.Card(LastCardID);
            }

            if (!(State == 6 || (State == 1 && LastCardID == -1))) {
                IntInfo = (int)json["intInfo"];
            }

            if (State >= 1 && State <= 5) {
                Time = (int)json["time"];
                GameControl.TimeForYou = Time;
            }

            TurnID = (int)json["turnID"];
            GameControl.TurnID = TurnID;

            if ((State == 4 && TurnID == GameControl.MainForm.MyID)
                || State == 6 || State == 7) {
                var jsa = json["playerCards"];
                PlayerCards = new int[jsa.Count];
                for (int i = 0; i < jsa.Count; ++i) {
                    PlayerCards[i] = (int)jsa[i];
                }
            }

            if (State == 1 || State == 2 || State == 3 || State == 5) {
                QueryID = (int)json["queryID"];
                GameControl.QueryID = QueryID;
            }

            if (State == 1 || State == 5 || State == 7) {
                GameControl.LastColor = (CardColor)(IntInfo & 0b11);
            }
        }

        ////////////////////////////////////////////////
        // 注意如下函数的调用都需要由调用者保证属性的存在 //
        // 否则可能出现奇怪的 BUG 或者是奇怪的结果       //
        ////////////////////////////////////////////////

        public int GetPlayerID() {
            return IntInfo >> 2;
        }

        public int GetPlayerIndex() {
            return GameControl.PlayerId2PlayerIndex[GetPlayerID()];
        }
    }
}
