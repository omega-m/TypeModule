using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace tpInner {

    /// <summary>
    /// <para>ひらがな文字列からひらがな中間文字列に変換する為のテーブルを管理するクラスです。</para>
    /// <para>お台文からガイド用のローマ字列を作成する時などに使用</para>
    /// </summary>
    /// <example><code>
    /// using tpInner;
    /// 
    ///     ...
    ///     
    /// //初期化処理
    /// Kana2KanaMidTable table = new Kana2KanaMidTable(in csvSrc);
    /// 
    /// 
    /// //ひらがな文字列から変換できる、ひらがな中間文字列があるかを取得
    /// string outCvt;
    /// Debug.Log(table.TryConvert("あ", out outCvt));           // false
    /// Debug.Log(table.TryConvert("が", out outCvt));           // true
    /// Debug.Log(table.TryConvert("ぱ", out outCvt));           // true
    /// 
    /// </code></example>
    public class Kana2KanaMidTable{


        #region 生成
        ///<summary>ひらがな文字列からひらがな中間文字列に変換する為のテーブルを管理するクラスです。</summary>
        ///<param name="aCSV">
        ///<para> ひらがなの中間文字列から、ひらがな文字列への変換テーブルを定義したファイルアセット</para>
        ///<para>［形式］ひらがな中間文字列, ひらがな文字列,</para>
        ///<para>［例］か゛,が</para>
        ///</param>
        public Kana2KanaMidTable(in TextAsset aCSV) {
            CreateTable(in aCSV);
        }
        #endregion


        #region メソッド
        /// <summary>ひらがな文字列から変換できる、ひらがな中間文字列があるか。</summary>
        /// <param name="aKana">ひらかな文字列</param>
        /// <param name="aOutKanaMid">(変換できる場合)変換先ひらがな中間文字列</param>
        /// </param>
        /// <returns>true:打つことができる文字列がある</returns>
        public bool TryConvert(string aKana, out string aOutKanaMid) {
            return m_table.TryGetValue(aKana, out aOutKanaMid);
        }
        #endregion


        #region プロパティ
        /// <summaryひらがな文字列に変換できるひらがな中間文字列の最大文字数</summary>
        public int KanaMidMaxLength { get; private set; }

        /// <summary>
        /// ひらがな中間文字列に変換できるひらがな文字列の最大文字数
        /// </summary>
        public int KanaMaxLength { get; private set; }
        #endregion 


        #region 内部メソッド
        ///<summary>ひらがな文字列からひらがな中間文字列に変換するためのテーブルを作成</summary>
        ///<param name="aCSV">変換テーブルを定義したファイル</param>
        private void CreateTable(in TextAsset aCSV) {
            const int CSV_KANA_MID_FIELD = 0;
            const int CSV_KANA_FIELD = 1;

            m_table = new Dictionary<string, string>();
            KanaMidMaxLength = 0;
            KanaMaxLength = 0;

            CsvReadHelper csv = new CsvReadHelper(in aCSV);
            foreach (List<string> record in csv.Datas) {
                KanaMidMaxLength = Mathf.Max(KanaMidMaxLength, record[CSV_KANA_MID_FIELD].Length);
                KanaMaxLength = Mathf.Max(KanaMaxLength, record[CSV_KANA_FIELD].Length);

                if (!m_table.ContainsKey(record[CSV_KANA_FIELD])) {
                    m_table.Add(record[CSV_KANA_FIELD], record[CSV_KANA_MID_FIELD]);
                }
            }
        }
        #endregion


        #region メンバ
        private Dictionary<string, string> m_table;     //SortedDictionaryだと、なぜか 「は゜」「は゛」が同じキーとして認識されてエラーが出る
        #endregion
    }

}