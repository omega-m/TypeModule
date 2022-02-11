using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace tpInner {

    /// <summary>
    /// キーボードの入力から文字列生成をエミュレートします
    /// </summary>
    /// <example><code>
    ///     
    ///     ...
    ///     
    /// //初期化処理
    /// 
    /// ConvertTableMgr table = new ConvertTableMgr();
    /// InputEmulator input = new InputEmulator(table);
    /// 
    ///     ...
    ///     
    /// //キーコードから文字を生成
    /// private void OnGUI(){
    ///     if (Event.current.type == EventType.KeyDown) {
    ///         input.AddInput(Event.current);
    ///     }
    /// }
    /// 
    ///     ...
    ///     
    /// //生成された文字列を取得
    /// Debug.Log(input.OutStr);        //生成された文字列
    /// Debug.Log(input.OutStrRaw);     //生成された、変換される前の文字列
    /// 
    /// //入力された文字列を全てクリア
    /// input.Clear();
    /// 
    /// 
    /// //以下オプションです================================
    /// 
    /// //ローマ字入力方式から、かな入力方式に切り替え
    /// input.IsKana = true;
    /// 
    /// //英語入力モードから、日本語入力モードに切り替え
    /// input.IsInputEng = false;
    /// 
    /// //バックスペースキーで文字を消せないようにする
    /// input.isBS = false;
    /// 
    /// </code></example>
    public class InputEmulator{

        #region 生成
        /// <summary>
        /// キーボードの入力から文字列生成をエミュレートします
        /// </summary>
        ///<param name="aConvertTableMgr">文字列生成時に使用する、変換テーブルを管理するクラス</param>
        public InputEmulator(ConvertTableMgr aConvertTableMgr) {
            m_convertTableMgr = aConvertTableMgr;
            Clear();
        }
        #endregion

        #region メソッド
        /// <summary>
        /// <para>内部データをクリアします。</para>
        /// <para>入力された文字列は全てクリアされます</para>
        /// </summary>
        public void Clear() {
            m_strDone.Clear();
            m_strDoneRaws.Clear();
            m_strWork = "";
        }

        /// <summary>
        /// キーボードからの入力文字を追加
        /// </summary>
        /// <param name="aEvent">入力イベント</param>
        public void AddInput(Event aEvent) {
            if(aEvent.keyCode == KeyCode.None) { return; }
            if(aEvent.keyCode == KeyCode.Backspace) {//bs
                if (IsBS) {
                    if(m_strWork.Length > 0) {
                        m_strWork = m_strWork.Substring(0, m_strWork.Length - 1);
                    } else if(m_strDone.Count > 0) {
                        int idx = m_strDone.Count - 1;
                        m_strDone[idx] = m_strDone[idx].Substring(0, m_strDone[idx].Length - 1);
                        if (m_strDone[idx].Length == 0) {
                            m_strDone.RemoveAt(idx);
                            m_strDoneRaws.RemoveAt(m_strDoneRaws.Count - 1);
                        }
                    }
                }
            }
            else if (IsInputEng) {//英語入力
                char nCh = m_convertTableMgr.Key2Roma.Convert(aEvent.keyCode, aEvent.shift, aEvent.functionKey);
                if (nCh == '\0') { return; }
                m_strDone.Add(nCh + "");
                m_strDoneRaws.Add(nCh + "");
            } else {
                if (IsKana) {//かな入力
                    char nCh = m_convertTableMgr.Key2kanaMid.Convert(aEvent.keyCode, aEvent.shift, aEvent.functionKey);
                    if (nCh == '\0') { return; }
                    m_strWork += nCh;
                    
                    while(m_strWork.Length > 0) {
                        if (m_convertTableMgr.KanaMid2Kana.CanConvert(m_strWork)) {
                            m_strDone.Add(m_convertTableMgr.KanaMid2Kana.Convert(m_strWork));
                            m_strDoneRaws.Add(m_strWork);
                            m_strWork = "";
                            break;
                        } else if (m_convertTableMgr.KanaMid2Kana.CanConvert(m_strWork, true)) {
                            break;
                        }
                        m_strDone.Add(m_strWork[0] + "");
                        m_strDoneRaws.Add(m_strWork[0] + "");
                        m_strWork = m_strWork.Substring(1);
                    }
                } else {//ローマ字入力
                    char nCh = m_convertTableMgr.Key2Roma.Convert(aEvent.keyCode, aEvent.shift, aEvent.functionKey);
                    if (nCh == '\0') { return; }
                    m_strWork += nCh;

                    if (Roma2KanaTable.CanConverFirstN(m_strWork)){
                        m_strDone.Add("ん");
                        m_strDoneRaws.Add("n");
                        m_strWork = m_strWork.Substring(1);
                    }
                    if (Roma2KanaTable.CanConverFirstHyphen(m_strWork)) {
                        m_strDone.Add("ー");
                        m_strDoneRaws.Add("-");
                        m_strWork = m_strWork.Substring(1);
                    }

                    while (m_strWork.Length > 0) {
                        if (m_convertTableMgr.Roma2Kana.CanConvert(m_strWork)) {
                            m_strDone.Add(m_convertTableMgr.Roma2Kana.Convert(m_strWork));
                            m_strDoneRaws.Add(m_strWork);
                            m_strWork = "";
                            break;
                        } else if (m_convertTableMgr.Roma2Kana.CanConvert(m_strWork, true)) {
                            break;
                        }
                        m_strDone.Add(m_strWork[0] + "");
                        m_strDoneRaws.Add(m_strWork[0] + "");
                        m_strWork = m_strWork.Substring(1);
                    }
                }
            }
        }
        #endregion

        #region フィールド
        /// <summary>
        /// 生成された文字列
        /// </summary>
        public string OutStr { 
            get{
                string ret = "";
                foreach (string r in m_strDone) {
                    ret += r;
                }
                ret += m_strWork;
                return ret;
            } 
        }

        /// <summary>
        /// 生成された、変換される前の文字列
        /// </summary>
        public string OutStrRaw {
            get {
                string ret = "";
                foreach(string r in m_strDoneRaws) {
                    ret += r;
                }
                ret += m_strWork;
                return ret;
            }
        }

        private bool m_isInputEng;
        /// <summary>
        /// <para>入力モード</para>
        /// <para>[true]英語入力モード</para>
        /// <para>[false]日本語入力モード</para>
        /// </summary>
        public bool IsInputEng {
            get { return m_isInputEng; } 
            set {
                m_isInputEng = value;
                if (m_isInputEng) {
                    //もし変換チェック中の文字列がある場合は、そのままの状態で確定
                    foreach (char ch in m_strWork) {
                        m_strDone.Add(ch + "");
                        m_strDoneRaws.Add(ch + "");
                    }
                    m_strWork = "";
                }
            }
        }

        private bool m_isKana;
        /// <summary>
        /// JISかな入力など、日本語を直接入力する方式を使用してエミュレートするかどうか
        /// </summary>
        public bool IsKana {
            get {return m_isKana; }
            set {
                m_isKana = value;
                //もし変換チェック中の文字列がある場合は、そのままの状態で確定
                foreach (char ch in m_strWork) {
                    m_strDone.Add(ch + "");
                    m_strDoneRaws.Add(ch + "");
                }
                m_strWork = "";
            }
        }

        /// <summary>
        /// <para>BackSoaceで文字を消せるかどうか</para>
        /// </summary>
        public bool IsBS { get; set; }
        #endregion

        #region メンバ
        private ConvertTableMgr m_convertTableMgr;
        private List<string>    m_strDone = new List<string>();         //変換確定済みの文字列
        private List<string>    m_strDoneRaws = new List<string>();     //変換確定前文字列
        private string          m_strWork;                              //現在チェック中の文字
        #endregion
    }
}