using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiplayerUNO.UI.BUtils {
    public class BUtil {
        private static Random rdm = new Random();

        public static string RandomNumberString(int length) {
            string suffix = rdm.Next().ToString();
            int len = suffix.Length;
            if (len < length) {
                suffix = new string('c', 3 - len);
            } else if (len > length) {
                suffix = suffix.Substring(0, 3);
            }
            return suffix;
        }
    }
}
