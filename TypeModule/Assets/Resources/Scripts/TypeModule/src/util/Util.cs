using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace tpInner {

    ///<summary>共通で使われるメソッドを定義</summary>
    public class Util {

        /// <summary>英語文字かどうかをチェック</summary>
        /// <param name="aChar">文字</param>
        /// <returns>英語大小文字ならtrue</returns>
        public static bool IsAlpha(char aChar) {
            return char.IsLower(char.ToLower(aChar));
        }
    }
}