using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace tpInner {

    /// <summary>
    /// <para>ひらがな文字列からローマ字列に変換する為のテーブルを管理するクラスです。</para>
    /// <para></para>お台文からガイド用のローマ字列を作成する時などに使用</para>
    /// </summary>
    /// <example><code>
    /// using tpInner;
    /// 
    ///     ...
    ///     
    /// //初期化処理
    /// Kana2RomaTable table = new Kana2RomaTable(in csvSrc);
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
        ///<summary>ひらがな文字列からローマ字列に変換する為のテーブルを管理するクラスです。</summary>
        ///<param name="aCSV">
        ///<para>ローマ字列からひらがな文字列への変換テーブルを定義したファイルアセット</para>
        ///<para>［形式］ローマ字列,ひらがな文字列,</para>
        ///<para>［例］kya,きゃ</para>
        ///</param>
        public Kana2RomaTable(in TextAsset aCSV) {
            CreateTable(in aCSV);
        }
        #endregion


        #region メソッド
        /// <summary>ひらがな文字列[aKana]から変換できるローマ字文字列を取得。</summary>
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

        /// <summary>ひらがな文字列[aKana]から変換できるローマ字文字列があるか</summary>
        /// <param name="aKana">ひらかな文字列</param>
        /// <param name="aOutRoma">(変換できる場合)変換先ローマ字文字列</param>
        /// <param name="aRomaStart">変換先ローマ字文字列の先頭部分を指定
        /// <para>(ひらがなに対応するローマ字文字列は数種類ある為、先頭部分を指定して絞り込みたい時に使用)</para>
        /// </param>
        /// <returns>true:打つことができる文字列がある</returns>
        public bool TryConvert(string aKana,out string aOutRoma, string aRomaStart = "") {
            aOutRoma = "";
            List<string> romaList;
            if (!m_table.TryGetValue(aKana, out romaList)) {
                return false; 
            }

            if (aRomaStart.Length == 0) {
                aOutRoma = romaList[0];
                return true;
            }
            foreach (string roma in romaList) {
                if (string.Compare(roma, 0, aRomaStart, 0, aRomaStart.Length) == 0) {
                    aOutRoma = roma;
                    return true;
                }
            }
            return false;
        }
        #endregion


        #region プロパティ
        /// <summary>ローマ字列に変換できるひらがな文字列の最大文字数</summary>
        public int KanaMaxLength { get; private set; }
        #endregion 


        #region 内部メソッド
        ///<summary>ひらがな文字列からローマ字列に変換するためのテーブルを作成</summary>
        ///<param name="aCSV">変換テーブルを定義したファイル</param>
        private void CreateTable(in TextAsset aCSV) {
            const int CSV_ROMA_FIELD = 0;
            const int CSV_KANA_FIELD = 1;

            m_table = new SortedDictionary<string, List<string>>();
            KanaMaxLength = 0;

            CsvReadHelper csv = new CsvReadHelper(in aCSV);
            foreach (List<string> record in csv.Datas) {
                List<string> romaList;
                if (!m_table.TryGetValue(record[CSV_KANA_FIELD], out romaList)) {
                    m_table.Add(record[CSV_KANA_FIELD], new List<string>());
                    KanaMaxLength = Mathf.Max(KanaMaxLength, record[CSV_KANA_FIELD].Length);
                    
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