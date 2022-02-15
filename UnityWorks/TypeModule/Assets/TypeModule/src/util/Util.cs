using System.Text;
using System.Text.RegularExpressions;

namespace TypeModuleInner {

    ///<summary>共通メソッドなどを定義</summary>
    static public class Util {


        #region 列挙型
        /// <summary>変換対象のタイプ</summary>
        public enum ConvertTypes {
            /// <summary>カタカナ</summary>
            Katakana = 1 << 0,
            /// <summary>アルファベット</summary>
            Alphabet = 1 << 1,
            /// <summary>数字</summary>
            Number = 1 << 2,
            /// <summary>記号</summary>
            Symbol = 1 << 3,
            /// <summary>スペース</summary>
            Space = 1 << 4,
            /// <summary>すべて</summary>
            All = (1 << 5) - 1,
        }
        #endregion


        #region メソッド
        /// <summary>カタカナをひらがなに変換し返却</summary>
        /// <param name="aStr">変換する文字列</param>
        /// <returns>変換後の文字列</returns>
        public static string KatakanaToHiragana(string aStr) {
            StringBuilder sb = new StringBuilder();
            char[] target = aStr.ToCharArray();
            char c;
            for (int i = 0; i < target.Length; i++) {
                c = target[i];
                if ('ァ' <= c && c <= 'ヴ') { 
                    c = (char)(c - 0x0060);  
                }
                sb.Append(c);
            }
            return sb.ToString();
        }

        /// <summary>全角文字に変換します。</summary>
        /// <param name="aStr">変換する文字列</param>
        /// <param name="aType">変換対象のタイプ</param>
        /// <returns>変換後の文字列</returns>
        public static string HanToZen(this string aStr, ConvertTypes aType) {
            if ((aType & ConvertTypes.Alphabet) != 0) {
                aStr = HanToZenAlpha(aStr);

            }
            if ((aType & ConvertTypes.Katakana) != 0) {
                for (int i = (HalfSizeKatakana.Length - 1); i >= 0; i--) {
                    aStr = aStr.Replace(HalfSizeKatakana[i], FullSizeKatakana[i]);
                }
            }
            if ((aType & ConvertTypes.Number) != 0) {
                aStr = HanToZenNum(aStr);

            }
            if ((aType & ConvertTypes.Space) != 0) {
                for (int i = (HalfSizeSpace.Length - 1); i >= 0; i--) {
                    aStr = aStr.Replace(HalfSizeSpace[i], FullSizeSpace[i]);
                }
            }
            if ((aType & ConvertTypes.Symbol) != 0) {
                for (int i = (HalfSizeSymbol.Length - 1); i >= 0; i--) {
                    aStr = aStr.Replace(HalfSizeSymbol[i], FullSizeSymbol[i]);
                }
            }
            return aStr;
        }

        /// <summary>半角文字に変換します。</summary>
        /// <param name="aStr">変換する文字列</param>
        /// <param name="aType">変換対象のタイプ</param>
        /// <returns>変換後の文字列</returns>
        public static string ZenToHan(this string aStr, ConvertTypes aType) {
            if ((aType & ConvertTypes.Alphabet) != 0) {
                aStr = ZenToHanAlpha(aStr);
            }
            if ((aType & ConvertTypes.Katakana) != 0) {
                for (int i = (FullSizeKatakana.Length - 1); i >= 0; i--) {
                    aStr = aStr.Replace(FullSizeKatakana[i], HalfSizeKatakana[i]);
                }
            }
            if ((aType & ConvertTypes.Number) != 0) {
                aStr = ZenToHanNum(aStr);
            }
            if ((aType & ConvertTypes.Space) != 0) {
                for (int i = (FullSizeSpace.Length - 1); i >= 0; i--) {
                    aStr = aStr.Replace(FullSizeSpace[i], HalfSizeSpace[i]);
                }
            }
            if ((aType & ConvertTypes.Symbol) != 0) {
                for (int i = (FullSizeSymbol.Length - 1); i >= 0; i--) {
                    aStr = aStr.Replace(FullSizeSymbol[i], HalfSizeSymbol[i]);
                }
            }
            return aStr;
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

        /// <summary>ひらがなかどうかチェック</summary>
        /// <param name="aCh">チェックする文字</param>
        /// <returns>true:ひらがなである</returns>
        public static bool IsHiragana(char aCh) {
            return ('ぁ' <= aCh && aCh <= 'ゔ');
        }

        /// <summary>全角スペース、半角スペースかどうかチェック</summary>
        /// <param name="aCh">チェックする文字</param>
        /// <returns>true:全角スペース、半角スペースである</returns>
        public static bool IsSpace(char aCh) {
            for (int i = (FullSizeSpace.Length - 1); i >= 0; i--) {
                if(aCh == FullSizeSpace[i][0]) { return true; }
            }
            for (int i = (HalfSizeSpace.Length - 1); i >= 0; i--) {
                if (aCh == HalfSizeSpace[i][0]) { return true; }
            }
            return false;
        }

        /// <summary>英語文字かどうかチェック</summary>
        /// <param name="aChar">文字</param>
        /// <param name="isCheckZen">全角文字もチェック</param>
        /// <returns>true:英語文字である</returns>
        public static bool IsAlpha(char aChar, bool isCheckZen = false) {
            if (char.IsLower(char.ToLower(aChar))) {
                return true;
            }
            if (isCheckZen) {
                if (char.IsLower(char.ToLower(ZenToHanAlpha(aChar + "")[0]))){
                    return true;
                }
            }
            return false; 
        }

        /// <summary>記号文字かどうかチェック</summary>
        /// <param name="aChar">文字</param>
        /// <param name="isCheckZen">全角文字もチェック</param>
        /// <returns>true:記号文字である</returns>
        public static bool IsSymbol(char aChar, bool isCheckZen = false) {
            //if (char.IsSymbol(aChar)) {//色々不便なので使わない
            //   return true;
            //}
            for (int i = (HalfSizeSymbol.Length - 1); i >= 0; i--) {
                if (aChar == HalfSizeSymbol[i][0]) { return true; }
            }
            if (isCheckZen) {
                for (int i = (FullSizeSymbol.Length - 1); i >= 0; i--) {
                    if (aChar == FullSizeSymbol[i][0]) { return true; }
                }
            }
            return false;
        }

        /// <summary>日本語用記号文字かどうかチェック</summary>
        /// <param name="aChar">文字</param>
        /// <returns>true:日本語用記号文字である</returns>
        public static bool IsJPSymbol(char aChar, bool isCheckZen = false) {
            for (int i = (JpSymbol.Length - 1); i >= 0; i--) {
                if (aChar == JpSymbol[i][0]) { return true; }
            }
            return false;
        }
        #endregion

        #region 内部変数
        /// <summary>半角カタカナ</summary>
        static readonly string[] HalfSizeKatakana = new string[] { "ｱ", "ｲ", "ｳ", "ｴ", "ｵ", "ｶ", "ｷ", "ｸ", "ｹ", "ｺ", "ｻ", "ｼ", "ｽ", "ｾ", "ｿ", "ﾀ", "ﾁ", "ﾂ", "ﾃ", "ﾄ", "ﾅ", "ﾆ", "ﾇ", "ﾈ", "ﾉ", "ﾊ", "ﾋ", "ﾌ", "ﾍ", "ﾎ", "ﾏ", "ﾐ", "ﾑ", "ﾒ", "ﾓ", "ﾔ", "ﾕ", "ﾖ", "ﾗ", "ﾘ", "ﾙ", "ﾚ", "ﾛ", "ﾜ", "ｵ", "ﾝ", "ｧ", "ｨ", "ｩ", "ｪ", "ｫ", "ｯ", "ｬ", "ｭ", "ｮ", "ｰ", "ｶﾞ", "ｷﾞ", "ｸﾞ", "ｹﾞ", "ｺﾞ", "ｻﾞ", "ｼﾞ", "ｽﾞ", "ｾﾞ", "ｿﾞ", "ﾀﾞ", "ﾁﾞ", "ﾂﾞ", "ﾃﾞ", "ﾄﾞ", "ﾊﾞ", "ﾋﾞ", "ﾌﾞ", "ﾍﾞ", "ﾎﾞ", "ﾊﾟ", "ﾋﾟ", "ﾌﾟ", "ﾍﾟ", "ﾎﾟ", "ｳﾞ" };
        /// <summary>全角カタカナ</summary>
        static readonly string[] FullSizeKatakana = new string[] { "ア", "イ", "ウ", "エ", "オ", "カ", "キ", "ク", "ケ", "コ", "サ", "シ", "ス", "セ", "ソ", "タ", "チ", "ツ", "テ", "ト", "ナ", "ニ", "ヌ", "ネ", "ノ", "ハ", "ヒ", "フ", "ヘ", "ホ", "マ", "ミ", "ム", "メ", "モ", "ヤ", "ユ", "ヨ", "ラ", "リ", "ル", "レ", "ロ", "ワ", "オ", "ン", "ァ", "ィ", "ゥ", "ェ", "ォ", "ッ", "ャ", "ュ", "ョ", "ー", "ガ", "ギ", "グ", "ゲ", "ゴ", "ザ", "ジ", "ズ", "ゼ", "ゾ", "ダ", "ヂ", "ヅ", "デ", "ド", "バ", "ビ", "ブ", "ベ", "ボ", "パ", "ピ", "プ", "ペ", "ポ", "ヴ"};
        /// <summary>半角記号</summary>
        static readonly string[] HalfSizeSymbol = new string[] { "!", "\"", "#", "$", "%", "&", "'", "(", ")", "=", "~", "|", "-", "^", "\\", "`", "{", "@", "[", "+", "*", "}", ";", ":", "]", "<", ">", "?", ",", ".", "/" };
        /// <summary>全角記号</summary>
        static readonly string[] FullSizeSymbol = new string[] { "！", "”", "＃", "＄", "％", "＆", "’", "（", "）", "＝", "～", "｜", "ー", "＾", "￥", "‘", "｛", "＠", "［", "＋", "＊", "｝", "；", "：", "］", "＜", "＞", "？", "，", "．", "／" };
        /// <summary>半角スペース</summary>
        static readonly string[] HalfSizeSpace = new string[] { " " };
        /// <summary>全角スペース</summary>
        static readonly string[] FullSizeSpace = new string[] { "　" };
        /// <summary>日本語用記号</summary>
        static readonly string[] JpSymbol = new string[] { "、", "。", "・", "「", "」", "ー"};
        #endregion
    }
}