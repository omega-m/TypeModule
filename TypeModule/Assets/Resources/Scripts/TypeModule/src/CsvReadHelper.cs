using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using System.Text;
using NotVisualBasic.FileIO;

namespace tpInner{

    ///<summary>
    ///<para>CSV読み込み用のヘルパークラスです。</para>
    ///<para>このファイルを使用するには、Nugetパッケージ【CsvTextFieldParser】がインストールされている必要があります。</para>
    ///</summary>
    /// <example><code>
    /// using tpInner;
    /// 
    ///     ...
    ///     
    /// public TextAsset csvAsset;
    ///
    ///     ...
    ///     
    /// //csvデータ読み込み(どちらでもOK)
    /// CsvReadHelper csv = new CsvReadHelper(csvAsset);
    /// //CsvReadHelper csv = new CsvReadHelper("filePath");
    /// 
    /// //各データにアクセス (1)
    /// for(int i = 0;i &lt; csv.RecordNum;++i){
    ///     for(int j = 0;j &lt; csv.FieldMax;++i){
    ///         Debug.Log(csv.Datas[i][j]);
    ///     }
    /// }
    /// 
    /// //各データにアクセス (2)
    /// foreach(List&lt;string&gt; record in csv.Datas){
    ///     foreach(string d in record){
    ///         Debug.Log(d);
    ///     }
    /// }
    /// </code></example>
    public class CsvReadHelper{
        #region 生成
        ///<summary>
        ///<para>CSV読み込み用のヘルパークラスです。</para>
        ///</summary>
        ///<param name="aFilePath">
        ///<para>CSV(.csv)形式のファイル格納先を指定。Resourceフォルダ内がカレントディレクトリとなります。</para>
        ///<para>文字コードは[UTF-8]としてください。</para>
        ///<para>フィールドにコンマ[,]を含む場合は、ダブルクォート["]で囲む必要があります。</para>
        ///</param>
        public CsvReadHelper(string aFilePath){
            Load(aFilePath);
        }

        ///<summary>
        ///<para>CSV読み込み用のヘルパークラスです。</para>
        ///</summary>
        ///<param name="aFile">
        ///<para>CSV(.csv)形式のファイルアセットを指定。文字コードは[UTF-8]としてください。</para>
        ///<para>フィールドにコンマ[,]を含む場合は、ダブルクォート["]で囲む必要があります。</para>
        ///</param>
        public CsvReadHelper(TextAsset aFile){
            Load(aFile);
        }
        #endregion

        #region メソッド
        ///<summary>
        ///CSVファイルデータの読み込み
        ///</summary>
        ///<param name="aFilePath">
        ///<para>CSV(.csv)形式のファイル格納先を指定。Resourceフォルダ内がカレントディレクトリとなります。</para>
        ///<para>文字コードは[UTF-8]としてください。</para>
        ///<para>フィールドにコンマ[,]を含む場合は、ダブルクォート["]で囲む必要があります。</para>
        ///</param>
        public void Load(string aFilePath){
            TextAsset file = new TextAsset();
            file = Resources.Load(aFilePath, typeof(TextAsset)) as TextAsset;
            Load(file);
        }

        ///<summary>
        ///CSVファイルデータの読み込み
        ///</summary>
        ///<param name="aFile">
        ///<para>CSV(.csv)形式のファイルアセットを指定。文字コードは[UTF-8]としてください。</para>
        ///<para>フィールドにコンマ[,]を含む場合は、ダブルクォート["]で囲む必要があります。</para>
        ///</param>
        public void Load(TextAsset aFile) {
            m_datas.Clear();
            FieldMax = -1;

            StringReader reader = new StringReader(aFile.text);
            while (reader.Peek() != -1){
                string line = reader.ReadLine();
                List<string> list = new List<string>();
                using (Stream stream = new MemoryStream(Encoding.Default.GetBytes(line))){
                    CsvTextFieldParser parser = new CsvTextFieldParser(stream, Encoding.GetEncoding("UTF-8"));
                    parser.SetDelimiter(',');
                    parser.HasFieldsEnclosedInQuotes = true;
                    parser.TrimWhiteSpace = false;

                    while (!parser.EndOfData){
                        string[] row = parser.ReadFields();
                        foreach (string field in row){
                            string tmp = String.Copy(field);
                            tmp = tmp.Replace("\\\"", "\"");    // " だけエスケープされているので修正
                            list.Add(String.Copy(tmp));
                        }
                    }
                    Datas.Add(list);
                }
                FieldMax = Math.Max(list.Count, FieldMax);
            }
        }
        #endregion

        #region プロパティ
        private List<List<string>> m_datas = new List<List<string>>();
        ///<summary>
        ///読み込んだCSVデータ
        ///</summary>
        public List<List<string>> Datas{
            get { return m_datas; }
            private set { m_datas = value; }
        }

        ///<summary>
        ///フィールド数最大値
        ///</summary>
        public int FieldMax { get; private set; }

        ///<summary>
        ///レコード数
        ///</summary>
        public int RecordNum { get { return Datas.Count; } }
        #endregion
    };
}

