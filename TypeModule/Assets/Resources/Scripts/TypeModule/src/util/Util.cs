using System.Text;
using System.Text.RegularExpressions;

namespace tpInner {

    ///<summary>共通で使われるメソッドを定義</summary>
    static public class Util {

        /// <summary>英語文字かどうかをチェック</summary>
        /// <param name="aChar">文字</param>
        /// <returns>英語大小文字ならtrue</returns>
        public static bool IsAlpha(char aChar) {
            return char.IsLower(char.ToLower(aChar));
        }

        /// <summary>カタカナをひらがなに変換し返却</summary>
        /// <param name="aStr">変換する文字列</param>
        /// <returns>変換後の文字列</returns>
        public static string KatakanaToHiragana(string aStr) {
            StringBuilder sb = new StringBuilder();
            char[] target = aStr.ToCharArray();
            char c;
            for (int i = 0; i < target.Length; i++) {
                c = target[i];
                if (c >= 'ァ' && c <= 'ヴ') { 
                    c = (char)(c - 0x0060);  
                }
                sb.Append(c);
            }
            return sb.ToString();
        }

        /// <summary>半角数字を全角数字に変換する。</summary>
        /// <param name="aStr">変換する文字列</param>
        /// <returns>変換後の文字列</returns>
        public static string HanToZenNum(this string aStr) {
            return Regex.Replace(aStr, "[0-9]", p => ((char)(p.Value[0] - '0' + '０')).ToString());
        }

        /// <summary>全角数字を半角数字に変換する。</summary>
        /// <param name="aStr">変換する文字列</param>
        /// <returns>変換後の文字列</returns>
        public static string ZenToHanNum(this string aStr) {
            return Regex.Replace(aStr, "[０-９]", p => ((char)(p.Value[0] - '０' + '0')).ToString());
        }

        /// <summary>半角アルファベットを全角アルファベットに変換する。</summary>
        /// <param name="aStr">変換する文字列</param>
        /// <returns>変換後の文字列</returns>
        public static string HanToZenAlpha(this string aStr) {
            var tmp = Regex.Replace(aStr, "[a-z]", p => ((char)(p.Value[0] - 'a' + 'ａ')).ToString());
            return Regex.Replace(tmp, "[A-Z]", p => ((char)(p.Value[0] - 'A' + 'Ａ')).ToString());
        }

        /// <summary>全角アルファベットを半角アルファベットに変換する。</summary>
        /// <param name="aStr">変換する文字列</param>
        /// <returns>変換後の文字列</returns>
        public static string ZenToHanAlpha(this string aStr) {
            var tmp = Regex.Replace(aStr, "[ａ-ｚ]", p => ((char)(p.Value[0] - 'ａ' + 'a')).ToString());
            return Regex.Replace(tmp, "[Ａ-Ｚ]", p => ((char)(p.Value[0] - 'Ａ' + 'A')).ToString());
        }

        /// <summary>半角英数字を全角英数字に変換する。</summary>
        /// <param name="aStr">変換する文字列</param>
        /// <returns>変換後の文字列</returns>
        public static string HanToZenAlNum(this string aStr) {
            var tmp = HanToZenNum(aStr);
            return HanToZenAlpha(tmp);
        }

        /// <summary>全角英数字を半角英数字に変換する。</summary>
        /// <param name="aStr">変換する文字列</param>
        /// <returns>変換後の文字列</returns>
        public static string ZenToHanAlNum(this string aStr) {
            var tmp = ZenToHanNum(aStr);
            return ZenToHanAlpha(tmp);
        }
    }
}