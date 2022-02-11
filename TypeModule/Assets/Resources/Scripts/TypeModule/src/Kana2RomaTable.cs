using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace tpInner {

    /// <summary>
    /// ひらがな文字列からローマ字列に変換する為のテーブルを管理するクラスです。
    /// お台文からガイド用のローマ字列を作成する時に使用
    /// </summary>
    /// <example><code>
    /// using tpInner;
    /// 
    ///     ...
    ///     
    /// //初期化処理
    /// Kana2RomaTable table = new Kana2RomaTable(csvSrc);
    /// 
    /// 
    /// //ひらがな文字列からローマ字文字列へ変換
    /// Debug.Log(table.Convert("あ"));          //a
    /// Debug.Log(table.Convert("ん"));          //nn
    /// Debug.Log(table.Convert("ん", "x"));     //xn
    /// Debug.Log(table.Convert("ちゃ"));        //tya
    /// Debug.Log(table.Convert("ちゃ", "c"));   //cha
    /// 
    /// </code></example>
    public class Kana2RomaTable {
        #region 生成
        ///<summary>
        /// ひらがな文字列からローマ字列に変換する為のテーブルを管理するクラスです。
        /// </summary>
        ///<param name="aCSV">
        ///<para>ローマ字列からひらがな文字列への変換テーブルを定義したファイル</para>
        ///<para>［形式］ローマ字列,ひらがな文字列,</para>
        ///<para>［例］kya,きゃ</para>
        ///</param>
        public Kana2RomaTable(TextAsset aCSV) {
            CreateTable(aCSV);
        }
        #endregion

        #region メソッド
        /// <summary>
        /// ひらがな文字列[aKana]から変換できるローマ字文字列を取得。
        /// </summary>
        /// <param name="aKana">ひらかな文字列</param>
        /// <param name="aRomaStart">変換先ローマ字文字列の先頭部分を指定
        /// <para>(ひらがなに対応するローマ字文字列は数種類ある為、先頭部分を指定して絞り込みたい時に使用)</para>
        /// </param>
        /// <returns>ローマ字文字列、変換できない場合は空文字列</returns>
        public string Convert(string aKana, string aRomaStart = "") {
            List<string> romaList;
            if (!m_table.TryGetValue(aKana, out romaList)) { return ""; }

            if(aRomaStart.Length == 0) {
                return romaList[0];
            }
            foreach(string roma in romaList) {
                if(string.Compare(roma,0,  aRomaStart, 0, aRomaStart.Length) == 0) {
                    return roma;
                }
            }
            return "";
        }

        #endregion
        #region 内部メソッド
        ///<summary>
        ///ローマ字列からひらがな文字列に変換するためのツリーを作成
        ///</summary>
        ///<param name="aCSV">変換テーブルを定義したファイル</param>
        private void CreateTable(TextAsset aCSV) {
            const int CSV_ROMA_FIELD = 0;
            const int CSV_KANA_FIELD = 1;

            m_table = new SortedDictionary<string, List<string>>();

            CsvReadHelper csv = new CsvReadHelper(aCSV);
            foreach (List<string> record in csv.Datas) {
                List<string> romaList;
                if (!m_table.TryGetValue(record[CSV_KANA_FIELD], out romaList)) {
                    m_table.Add(record[CSV_KANA_FIELD], new List<string>());
                    romaList = m_table[record[CSV_KANA_FIELD]];
                }
                romaList.Add(record[CSV_ROMA_FIELD].ToLower());
            }
        }
        #endregion

        #region メンバ
        private SortedDictionary<string, List<string>> m_table;
        #endregion
    }
}