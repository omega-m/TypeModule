using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace tpInner {

    /// <summary>
    /// ひらがなの中間文字列から、ひらがな文字列に変換する為のテーブルを管理するクラスです。
    /// </summary>
    /// <example><code>
    /// using tpInner;
    /// 
    ///     ...
    ///     
    /// //初期化処理
    /// KanaMid2KanaTable table = new KanaMid2KanaTable(in csvSrc);
    /// 
    /// 
    /// //ひらがなの中間文字列から、ひらがな文字列に変換
    /// Debug.Log(table.Convert("あ"));     //""
    /// Debug.Log(table.Convert("か゛"));   //"が"
    /// Debug.Log(table.Convert("か"));     //""
    /// Debug.Log(table.Convert("は"));     //""
    /// Debug.Log(table.Convert("は゛"));   //"ば"
    ///
    /// 
    /// //ひらがなの中間文字列から、変換できるひらがながあるかを取得
    /// Debug.Log(m_convertTableMgr.KanaMid2Kana.CanConvert("あ"));          // false
    /// Debug.Log(m_convertTableMgr.KanaMid2Kana.CanConvert("か"));          // false
    /// Debug.Log(m_convertTableMgr.KanaMid2Kana.CanConvert("か゛"));        // true
    /// //将来変換できる可能性があるかもチェック
    /// Debug.Log(m_convertTableMgr.KanaMid2Kana.CanConvert("か", true));    // true
    /// Debug.Log(m_convertTableMgr.KanaMid2Kana.CanConvert("か゛", true));  // true
    /// 
    /// 
    /// //ひらがなの中間文字列から、指定したひらがな文字列へ変換できるかどうかを取得
    /// Debug.Log(table.CanConvert("は゛", "ば"));            //true
    /// Debug.Log(table.CanConvert("は゛", "は"));            //false
    /// Debug.Log(table.CanConvert("は", "ば"));              //false
    /// //将来打てる可能性があるかもチェック
    /// Debug.Log(table.CanConvert("は", "ば", true));        //true
    ///
    /// </code></example>
    public class KanaMid2KanaTable{

        #region 生成
        ///<summary>
        /// ひらがなの中間文字列から、ひらがな文字列に変換する為のテーブルを管理するクラスです。
        /// </summary>
        ///<param name="aCSV">
        ///<para> ひらがなの中間文字列から、ひらがな文字列への変換テーブルを定義したファイルアセット</para>
        ///<para>［形式］ひらがな中間文字列, ひらがな文字列,</para>
        ///<para>［例］か゛,が</para>
        ///</param>
        public KanaMid2KanaTable(in TextAsset aCSV) {
            CreateTable(in aCSV);
        }
        #endregion

        #region メソッド
        /// <summary>
        /// ひらがなの中間文字列[aKanaMid]から変換できるひらがな文字列を取得。
        /// </summary>
        /// <param name="aKanaMid">ひらかな中間文字列</param>
        /// <returns>ひらがな文字列、変換できない場合は空文字列</returns>
        public string Convert(string aKanaMid) {
            string ret;
            if (!m_mid2Kana.TryGetValue(aKanaMid, out ret)) { return ""; }
            return ret;
        }

        /// <summary>
        /// ひらがなの中間文字列[aKanaMid]に対して、変換できるひらがな文字列があるか
        /// </summary>
        /// <param name="aKanaMid">ひらかな中間文字列</param>
        /// <param name="aIsPossibility">true:[aKanaMid]に、追加でひらがなの中間文字列を足すことで、打つ方法があるかもチェックする</param>
        /// <returns>true:打つことができる文字列がある</returns>
        public bool CanConvert(string aKanaMid, bool aIsPossibility = false) {
            if (m_mid2Kana.ContainsKey(aKanaMid)) {
                return true;
            }
            if (aIsPossibility) {
                //「゛」と「゜」を後ろに付けることで、打てるひらがながあるかチェック
                if (m_mid2Kana.ContainsKey(aKanaMid + "゛")) {
                    return true;
                }
                if (m_mid2Kana.ContainsKey(aKanaMid + "゛")) {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// ひらがなの中間文字列[aKanaMid]に対して、ひらがな文字列[aKana]を打つことができるか
        /// </summary>
        /// <param name="aKanaMid">ひらがなの中間文字列(例:さ゛)</param>
        /// <param name="aKana">ひらがな文字列(例:ざ)</param>
        /// <param name="aIsPossibility">true:[aKanaMid]に、追加でひらがなの中間文字列を足すことで、打つ方法があるかもチェックする</param>
        /// <returns>true:打つことができる</returns>
        public bool CanConvert(string aKanaMid, string aKana, bool aIsPossibility = false) {
            string kanaTmp;
            if (m_mid2Kana.TryGetValue(aKanaMid, out kanaTmp)) {
                return (string.Compare(kanaTmp, aKana) == 0);
            }
            if (aIsPossibility) {
                string midTmp;
                if (!m_Kana2Mid.TryGetValue(aKana, out midTmp)) {
                    return false;
                }
                int cmpLen = aKanaMid.Length;
                return (string.Compare(midTmp,0, aKanaMid,0, cmpLen) == 0);
            }
            return false;
        }
        #endregion

        #region プロパティ
        /// <summary>
        /// ひらがな文字列に変換できるひらがな中間文字列の最大文字数
        /// </summary>
        public int KanaMidMaxLength { get; private set; }
        #endregion 

        #region 内部メソッド
        ///<summary>
        ///ローマ字列からひらがな文字列に変換するためのテーブルを作成
        ///</summary>
        ///<param name="aCSV">変換テーブルを定義したファイル</param>
        private void CreateTable(in TextAsset aCSV) {
            const int CSV_KANA_MID_FIELD = 0;
            const int CSV_KANA_FIELD = 1;

            m_mid2Kana = new Dictionary<string, string>();
            m_Kana2Mid = new Dictionary<string, string>();
            KanaMidMaxLength = 0;

            CsvReadHelper csv = new CsvReadHelper(in aCSV);
            foreach (List<string> record in csv.Datas) {
                m_mid2Kana.Add(record[CSV_KANA_MID_FIELD], record[CSV_KANA_FIELD]);
                KanaMidMaxLength = Mathf.Max(KanaMidMaxLength, record[CSV_KANA_MID_FIELD].Length);
                
                if (!m_Kana2Mid.ContainsKey(record[CSV_KANA_FIELD])) {
                    m_Kana2Mid.Add(record[CSV_KANA_FIELD], record[CSV_KANA_MID_FIELD]);
                }
            }
        }
        #endregion

        #region メンバ
        private Dictionary<string, string> m_mid2Kana;  //SortedDictionaryだと、なぜか 「は゜」「は゛」が同じキーとして認識されてエラーが出る
        private Dictionary<string, string> m_Kana2Mid;
        #endregion
    }
}
