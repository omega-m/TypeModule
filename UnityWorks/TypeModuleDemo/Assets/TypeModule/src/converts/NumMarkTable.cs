using System.Collections.Generic;
using UnityEngine;

namespace tm {
namespace Inner {


    /// <summary>数字と記号の、全角と半角を変換する為のテーブルを管理するクラスです。</summary>
    /// /// <example><code>
    /// using Inner;
    /// 
    ///     ...
    ///     
    /// //初期化処理
    /// NumMarkTable table = new NumMarkTable(in csvSrc);
    /// 
    /// 
    /// //半角から全角へ変換
    /// string outStr;
    /// if(table.TryHanToZen(",", out outStr)){
    ///     Debug.Log(outStr);         //"、"
    /// }
    /// if(table.TryHanToZen("、", out outStr)){
    ///     Debug.Log(outStr);         //none
    /// }
    /// 
    /// //全角から半角へ変換
    /// if(table.TryZenToHan(",", out outStr)){
    ///     Debug.Log(outStr);         //none
    /// }
    /// if(table.TryZenToHan("、", out outStr)){
    ///     Debug.Log(outStr);         //","
    /// }
    /// 
    /// </code></example>
    public class NumMarkTable{


        #region 生成
        /// <summary>数字と記号の、全角と半角を変換する為のテーブルを管理するクラスです。</summary>
        ///<param name="aCSV">
        ///<para>数字と記号の、全角半角の変換テーブルを定義したファイルアセット</para>
        ///<para>［形式］半角文字,全角文字</para>
        ///<para>［例］</para>
        ///<para>.,。</para>
        ///<para>?,？</para>
        ///</param>
        public NumMarkTable(in TextAsset aCSV) {
            CreateTable(aCSV);
        }
        #endregion


        #region メソッド
        /// <summary>数字と記号を半角から全角に変換できるかをチェックし、変換できる場合はaOutZenkakuに格納</summary>
        /// <param name="aHankaku">半角文字</param>
        /// <param name="aOutZenkaku">(変換可能な場合)半角を全角に変換した文字を格納</param>
        /// <returns>true:変換できた</returns>
        public bool TryHanToZen(string aHankaku, out string aOutZenkaku) {
            string str = "";
            bool ret = false;
            if (m_han2Zen.TryGetValue(aHankaku, out str)) {
                ret = true;
            }
            aOutZenkaku = str;
            return ret;
        }

        ///<summary>数字と記号を半角から全角に変換します。</summary>
        ///<param name="aHankaku">半角文字</param>
        ///<returns>半角を全角に変換した文字</returns>
        public string HanToZen(string aHankaku) {
            string ret = "";
            if(m_han2Zen.TryGetValue(aHankaku, out ret)) {
                return ret;
            }
            return "";
        }

        /// <summary>数字と記号を全角から半角に変換できるかをチェックし、変換できる場合はaOutHankakuに格納</summary>
        /// <param name="aZenkaku">全角文字</param>
        /// <param name="aOutHankaku">(変換可能な場合)全角を半角に変換した文字を格納</param>
        /// <returns>true:変換できた</returns>
        public bool TryZenToHan(string aZenkaku, out string aOutHankaku) {
            string str = "";
            bool ret = false;
            if (m_zen2Han.TryGetValue(aZenkaku, out str)) {
                ret = true;
            }
            aOutHankaku = str;
            return ret;
        }

        ///<summary>数字と記号を全角から半角に変換します。</summary>
        ///<param name="aZenkaku">全角文字</param>
        ///<returns>全角を半角に変換した文字</returns>
        public string ZenToHan(string aZenkaku) {
            string ret = "";
            if (m_zen2Han.TryGetValue(aZenkaku, out ret)) {
                return ret;
            }
            return "";
        }
        #endregion


        #region 内部メソッド
        /// <summary>数字と記号の、全角と半角を変換する為のテーブルを作成</summary>
        /// <param name="aCSV">変換テーブルを定義したファイル</param>
        private void CreateTable(in TextAsset aCSV) {
            const int CSV_HAN_FIELD = 0;
            const int CSV_ZEN_FIELD = 1;

            CsvReadHelper csv = new CsvReadHelper(in aCSV);
            foreach (List<string> record in csv.Datas) {
                m_han2Zen.Add(record[CSV_HAN_FIELD], record[CSV_ZEN_FIELD]);
                m_zen2Han.Add(record[CSV_ZEN_FIELD], record[CSV_HAN_FIELD]);
            }
        }
        #endregion


        #region メンバ
        private SortedDictionary<string, string> m_han2Zen = new SortedDictionary<string, string>();
        private SortedDictionary<string, string> m_zen2Han = new SortedDictionary<string, string>();
        #endregion
    }
}
}