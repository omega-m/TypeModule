using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using System.Text;
using NotVisualBasic.FileIO;

namespace tpInner
{
    ///<summary>
    ///<para>CSV�ǂݍ��ݗp�̃w���p�[�N���X�ł��B</para>
    ///<para>���̃t�@�C�����g�p����ɂ́ANuget�p�b�P�[�W�yCsvTextFieldParser�z���C���X�g�[������Ă���K�v������܂��B</para>
    ///</summary>
    /// <example><code>
    /// using tpInner;
    /// 
    ///     ...
    ///     
    /// public TextAsset csvSrc;
    ///
    ///     ...
    ///     
    /// //csv�f�[�^�ǂݍ���
    /// csvReadHelper csv = new csvReadHelper(csvSrc);
    /// 
    /// //�e�f�[�^�ɃA�N�Z�X (1)
    /// for(int i = 0;i &lt; csv.RecordNum;++i){
    ///     for(int j = 0;j &lt; csv.FieldMax;++i){
    ///         Debug.Log(csv.Datas[i][j]);
    ///     }
    /// }
    /// 
    /// //�e�f�[�^�ɃA�N�Z�X (2)
    /// foreach(List&lt;string&gt; record in csv.Datas){
    ///     foreach(string d in record){
    ///         Debug.Log(d);
    ///     }
    /// }
    /// </code></example>
    public class csvReadHelper
    {
#region ����
        ///<summary>
        ///<para>CSV�ǂݍ��ݗp�̃w���p�[�N���X�ł��B</para>
        ///</summary>
        ///<param name="aFile">
        ///<para>CSV(.csv)�`���̃t�@�C�����w��B�����R�[�h��[UTF-8]�Ƃ��Ă��������B</para>
        ///<para>�t�B�[���h�ɃR���}[,]���܂ޏꍇ�́A�_�u���N�H�[�g["]�ň͂ޕK�v������܂��B</para>
        ///</param>
        public csvReadHelper(TextAsset aFile)
        {
            StringReader reader = new StringReader(aFile.text);
            while (reader.Peek() != -1)
            {
                string line = reader.ReadLine();
                List<string> list = new List<string>();
                using (Stream stream = new MemoryStream(Encoding.Default.GetBytes(line)))
                {
                    CsvTextFieldParser parser = new CsvTextFieldParser(stream, Encoding.GetEncoding("UTF-8"));
                    parser.SetDelimiter(',');
                    parser.HasFieldsEnclosedInQuotes = true;
                    parser.TrimWhiteSpace = true;

                    while (!parser.EndOfData)
                    {
                        string[] row = parser.ReadFields();
                        foreach (string field in row)
                        {
                            string tmp = String.Copy(field);
                            tmp = tmp.Replace("\\\"", "\"");    // " �����G�X�P�[�v����Ă���̂ŏC��
                            list.Add(String.Copy(tmp));
                        }
                    }
                    Datas.Add(list);
                }
                FieldMax = Math.Max(list.Count, FieldMax);
            }
        }
#endregion

#region �v���p�e�B
        private List<List<string>> m_datas = new List<List<string>>();
        ///<summary>
        ///�ǂݍ���CSV�f�[�^
        ///</summary>
        public List<List<string>> Datas
        {
            get { return m_datas; }
            private set { m_datas = value; }
        }

        ///<summary>
        ///�t�B�[���h���ő�l
        ///</summary>
        public int FieldMax { get; private set; }

        ///<summary>
        ///���R�[�h��
        ///</summary>
        public int RecordNum { get { return Datas.Count; } }
#endregion
    };
}

