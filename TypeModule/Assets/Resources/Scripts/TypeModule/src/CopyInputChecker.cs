using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// CopyInputCheckerによって処理されたデータへのアクセス用クラスです。
/// TypeModuleでModeをMODE_COPYにした時、入力発生時のイベントリスナで返却されます。
/// </summary>
public class CopyInputCheckerResults {

    #region 内部イベントタイプ
    /// <summary>
    /// イベント発生時の内部タイプです
    /// </summary>
    public enum INNER_EVENT_TYPE{
        /// <summary>
        /// どの入力タイプにも属さない
        /// <summary>
        NONE,
        /// <summary>
        /// 正しくタイプされた時
        /// <summary>
        CORRECT,
        /// <summary>
        /// ミスタイプした時
        /// <summary>
        MISS,
        /// <summary>
        /// 全て打ち終わった時
        /// <summary>
        COMPLETE,
        /// <summary>
        /// 内部データクリア・セットアップされた時
        /// <summary>
        CLEAR,
    }
    #endregion

    #region フィールド
    /// <summary>
    /// 比較対象の文字列(タイピングのお台文)
    /// </summary>
    public string TargetStr {
        get {
            return m_params.m_targetStr;
        }
    }

    /// <summary>
    /// 既に打ち終わった文字列
    /// </summary>
    public string StrDone {
        get {
            if (Dirty) { CreateChche(); }
            return m_strDoneCache;
        }
    }

    /// <summary>
    /// 現在打っている文字
    /// </summary>
    public string StrCurrent {
        get {
            return m_params.m_strCurrent;
        }
    }

    /// <summary>
    /// まだ打っていない文字列
    /// </summary>
    public string StrYet {
        get {
            if (Dirty) { CreateChche(); }
            return m_strYetCache;
        }
    }

    /// <summary>
    /// 既に打ち終わった文字列(変換前)
    /// </summary>
    public string StrDoneRaw {
        get {
            if (Dirty) { CreateChche(); }
            return m_strDoneRawCache;
        }
    }

    /// <summary>
    /// 現在打っている文字(変換前)
    /// </summary>
    public string StrCurrentRaw {
        get {
            return m_params.m_strCurrentRaw;
        }
    }

    /// <summary>
    /// 既に打ち終わった文字列(変換前)
    /// </summary>
    public string StrYetRaw {
        get {
            if (Dirty) { CreateChche(); }
            return m_strYetRawCache;
        }
    }

    /// <summary>
    /// 前回正しく入力された文字(ミスした時は空文字列)
    /// </summary>
    public string PrevCorrectChar {
        get {
            return m_params.m_prevCorrectChar;
        }
    }

    /// <summary>
    /// 前回ミスしたされた文字(正しく入力された時は空文字列)
    /// </summary>
    public string PrevMissChar {
        get {
            return m_params.m_prevMissChar;
        }
    }

    /// <summary>
    /// 正しくタイプした数
    /// </summary>
    public int CorrectNum {
        get {
            return m_params.m_correctNum;  
        }
    }

    /// <summary>
    /// 正しく打てた文字数
    /// </summary>
    public int CorrectCharNum {
        get {
            return m_params.m_correctCharNum;
        }
    }

    /// <summary>
    /// ミスタイプした数
    /// </summary>
    public int MissNum {
        get {
            return m_params.m_missNum;
        }
    }

    /// <summary>
    /// 打ち終わったか
    /// </summary>
    public bool IsComplete {
        get {
            return m_params.m_isComplete;
        }
    }

    /// <summary>
    /// 前回入力発生時のUnityイベント
    /// </summary>
    public Event Event {
        get {
            return m_params.m_event;
        }
    }

    /// <summary>
    /// 前回イベント発生時の内部タイプ
    /// </summary>
    public INNER_EVENT_TYPE InnerEvent {
        get { return m_params.m_innerEvent; }
    }

    /// <summary>
    /// JISかな入力など、日本語を直接入力する方式を使用しているか
    /// </summary>
    public bool IsKana{
        get {
            return m_params.m_isKana;
        }
    }

    /// <summary>
    /// 英語の大文字と小文字入力を区別して判定するか
    /// </summary>
    public bool IsCaseSensitive {
        get { return m_params.m_isCaseSensitive; }
    }
    #endregion

    #region 生成
    /// <summary>
    /// CopyInputCheckerより作成されます。外からは作成しないでください。
    /// </summary>    
    public CopyInputCheckerResults(in tpInner.CopyInputCheckerParams aParams) {
        m_params = aParams;
    }
    #endregion

    #region 内部使用プロパティ
    /// <summary>
    /// 汚しフラグです。trueになっている時に文字列にアクセスされた場合、データを再度作成します。
    /// </summary>
    public bool Dirty { get; set; } = true;
    #endregion

    #region 内部メソッド
    /// <summary>
    /// 返却パラメータのキャッシュを生成します。
    /// </summary>
    private void CreateChche() {
        m_strDoneCache = "";
        foreach (string r in m_params.m_strDone) {
            m_strDoneCache += r;
        }

        m_strYetCache = "";
        foreach (string r in m_params.m_strYet) {
            m_strYetCache += r;
        }

        m_strDoneRawCache = "";
        foreach (string r in m_params.m_strDoneRaws) {
            m_strDoneRawCache += r;
        }

        m_strYetRawCache = m_params.m_strCurrentRawWork;
        foreach (string r in m_params.m_strYetRaws) {
            m_strYetRawCache += r;
        }

        Dirty = false;
    }
    #endregion

    #region メンバ
    private tpInner.CopyInputCheckerParams m_params;   //参照

    private string m_strDoneCache = "";
    private string m_strYetCache = "";
    private string m_strDoneRawCache = "";
    private string m_strYetRawCache = "";
    #endregion
}

namespace tpInner {

    /// <summary>
    /// CopyInputCheckerの内部パラメータクラス
    /// CopyInputCheckerResultsの参照渡しに使用。
    /// </summary>
    public class CopyInputCheckerParams {

        #region メソッド
        /// <summary>
        /// <para>内部データをクリアします。</para>
        /// </summary>
        public void Clear() {
            m_strDone.Clear();
            m_strCurrent = "";
            m_strYet.Clear();
            m_strDoneRaws.Clear();
            m_strCurrentRaw = "";
            m_strYetRaws.Clear();
            m_prevCorrectChar = "";
            m_prevMissChar = "";
            m_correctNum = 0;
            m_correctCharNum = 0;
            m_missNum = 0;
            m_strCurrentRawWork = "";
            m_strCurrentRawDone = "";
            m_isComplete = false;
            m_event = new Event();
            m_innerEvent = CopyInputCheckerResults.INNER_EVENT_TYPE.NONE;
        }
        #endregion

        #region メンバ
        public string       m_targetStr = "";                   //比較対象の文字列(タイピングのお台文)
        public List<string> m_strDone = new List<string>();     //既に打ち終わった文字列
        public List<string> m_strDoneRaws = new List<string>(); //既に打ち終わった文字列(変換前)
        public string       m_strCurrent;                       //現在打っている文字
        public string       m_strCurrentRaw;                    //現在打っている文字(変換前)
        public List<string> m_strYet = new List<string>();      //まだ打っていない文字列
        public List<string> m_strYetRaws = new List<string>();  //まだ打っていない文字列(変換前)
        public string       m_prevCorrectChar;                  //前回正しく入力された文字(ミスした時は空文字列)
        public string       m_prevMissChar;                     //前回ミスしたされた文字(正しく入力された時は空文字列)
        public int          m_correctNum;                       //正しくタイプした数
        public int          m_correctCharNum;                   //正しく打てた文字数
        public int          m_missNum;                          //ミスタイプした数
        public bool         m_isKana;
        public bool         m_isCaseSensitive;
        public bool         m_isComplete;
        public Event        m_event = new Event();
        public CopyInputCheckerResults.INNER_EVENT_TYPE m_innerEvent;
        public string       m_strCurrentRawWork;        
        public string       m_strCurrentRawDone;
        #endregion
    }


    /// <summary>
    /// <para>指定された文字列が正しく打ててるかを確認します。</para>
    /// <para>タイピングゲームで、お題の文字を真似して打たせる時に使用してください。</para>
    /// </summary>
    public class CopyInputChecker {

        #region 生成
        /// <summary>
        /// 指定された文字列が正しく打ててるかを確認します。
        /// タイピングゲームで、お題の文字を真似して打たせる時に使用してください。
        /// </summary>
        ///<param name="aConvertTableMgr">文字列生成時に使用する、変換テーブルを管理するクラス</param>
        public CopyInputChecker(in ConvertTableMgr aConvertTableMgr) {
            m_convertTableMgr = aConvertTableMgr;
            m_results = new CopyInputCheckerResults(in m_params);
            Clear();
        }
        #endregion

        #region メソッド
        /// <summary>
        /// <para>キーボードからの入力文字を追加</para>
        /// <para>比較対象の文字列がセットされていない場合はなにもしません。</para>
        /// </summary>
        /// <param name="aEvent">入力イベント</param>
        public void AddInput(in Event aEvent) {
            
            if(IsComplete) { return; }
            var p = m_params;
            var cvt = m_convertTableMgr;
            //IMEによって、1回のキー入力に対して二回呼び出しが発生する為、更新しない
            if (aEvent.keyCode == KeyCode.None) {return;}

            char nCh;
            //英語チェック
            if (p.m_strCurrent.Length == 1) {
                nCh = cvt.Key2Roma.Convert(aEvent.keyCode, aEvent.shift, aEvent.functionKey);
                if (nCh == '\0') { return; }
                if (IsCaseSensitive) {
                    if (p.m_strCurrent[0] == nCh) {
                        p.m_event = aEvent;
                        CorrectType();
                        return;
                    } else if(char.IsLower(char.ToLower(p.m_strCurrent[0]))) {//英語入力中ならミス判定
                        p.m_event = aEvent;
                        MissType();
                        return;
                    }
                } else {
                    if (char.ToLower(p.m_strCurrent[0]) == char.ToLower(nCh)) {
                        p.m_event = aEvent;
                        CorrectType();
                        return;
                    }
                }
                //全角チェック
                string nChZen;
                if (cvt.NumMarkTable.TryHanToZen(nCh + "", out nChZen)) {
                    if (string.Compare(p.m_strCurrent, nChZen) == 0) {
                        p.m_event = aEvent;
                        CorrectType();
                        return;
                    }
                }
            }
            //かなチェック
            if (IsKana) {
                nCh = cvt.Key2kanaMid.Convert(aEvent.keyCode, aEvent.shift, aEvent.functionKey);
                if (nCh == '\0') { return; }
                if (p.m_strCurrentRaw[0] == nCh) {
                    p.m_event = aEvent;
                    CorrectType();
                    return;
                }
            } else {//ローマ字入力
                nCh = cvt.Key2Roma.Convert(aEvent.keyCode, aEvent.shift, aEvent.functionKey);
                if (nCh == '\0') { return; }

                //「ん」の単発入力確定処理
                if (string.Compare(p.m_strCurrent, "ん") == 0) {
                    string tmpRoma = p.m_strDoneRaws[p.m_strDoneRaws.Count - 1] + nCh;
                    if (Roma2KanaTable.CanConverFirstN(tmpRoma) && p.m_strYet.Count > 0) {
                        CorrectType(false);
                        string prevStr = p.m_strDoneRaws[p.m_strDoneRaws.Count - 2];
                        p.m_strDoneRaws[p.m_strDoneRaws.Count - 2] = prevStr.Substring(0, prevStr.Length - 1);
                        p.m_correctNum--;
                    }
                }

                if (char.ToLower(p.m_strCurrentRaw[0]) == char.ToLower(nCh)) {
                    p.m_event = aEvent;
                    CorrectType();
                    return;
                }
        
                //ローマ字入力の場合、途中でローマ字パターンが変わる事がある。
                //例)「にゃ」が[nya]から[ni][xya]に変わる事がある
                //string tmpRomaStart = p.m_strCurrentRawDone + nCh;
                //for (int chLen = p.m_strCurrent.Length; chLen > 0; --chLen) {
                //    string tmpKana = p.m_strCurrent.Substring(0, chLen);
                //    string outRoma;
                //    if (cvt.Kana2Roma.TryConvert(tmpKana, out outRoma, tmpRomaStart)) {
                //        outRoma = outRoma.Substring(tmpKana.Length);
                //        p.m_strCurrentRaw = outRoma[0] + "";
                //        p.m_strCurrentRawWork = outRoma;
                //        p.m_event = aEvent;
                //        CorrectType();
                //        return;
                //    }
                //}

            }

            //ミス
            p.m_event = aEvent;
            MissType();
        }

        /// <summary>
        /// <para>内部データをクリアします。</para>
        /// <para>入力された文字列は全てクリアされます</para>
        /// <para>比較対象文字[TargetStr]のみクリアされません。</para>
        /// <para>クリアされた後、セットアップされます。</para>
        /// </summary>
        public void Clear() {
            m_results.Dirty = true;
            m_params.Clear();
            m_params.m_isKana = m_isKana;
            m_params.m_isCaseSensitive = m_isCaseSensitive;
            InitStrYet();
            CorrectType(false);
            m_params.m_innerEvent = CopyInputCheckerResults.INNER_EVENT_TYPE.CLEAR; 
        }
        #endregion

        #region フィールド
        /// <summary>
        /// <para>比較対象の文字列(タイピングのお台文)</para>
        /// <para>値を変更した時点で、内部でClear()を自動で呼び出します。</para>
        /// </summary>
        public string TargetStr {
            get {return m_results.TargetStr;}
            set {
                m_params.m_targetStr = value;
                Clear();
            }
        }

        /// <summary>
        /// 既に打ち終わった文字列
        /// </summary>
        public string StrDone {
            get {return m_results.StrDone;}
        }

        /// <summary>
        /// 現在打っている文字
        /// </summary>
        public string StrCurrent {
            get { return m_results.StrCurrent; }
        }

        /// <summary>
        /// まだ打っていない文字列
        /// </summary>
        public string StrYet {
            get { return m_results.StrYet; }
        }

        /// <summary>
        /// 既に打ち終わった文字列(変換前)
        /// </summary>
        public string StrDoneRaw {
            get { return m_results.StrDoneRaw; }
        }

        /// <summary>
        /// 現在打っている文字(変換前)
        /// </summary>
        public string StrCurrentRaw {
            get { return m_results.StrCurrentRaw; }
        }

        /// <summary>
        /// 既に打ち終わった文字列(変換前)
        /// </summary>
        public string StrYetRaw {
            get { return m_results.StrYetRaw; }
        }

        /// <summary>
        /// 前回正しく入力された文字(ミスした時は空文字列)
        /// </summary>
        public string PrevCorrectChar {
            get { return m_results.PrevCorrectChar; }
        }

        /// <summary>
        /// 前回ミスしたされた文字(正しく入力された時は空文字列)
        /// </summary>
        public string PrevMissChar {
            get {return m_results.PrevMissChar;}
        }

        /// <summary>
        /// 正しくタイプした数
        /// </summary>
        public int CorrectNum {
            get {return m_results.CorrectNum;}
        }

        /// <summary>
        /// 正しく打てた文字数
        /// </summary>
        public int CorrectCharNum {
            get { return m_results.CorrectCharNum; }
        }

        /// <summary>
        /// ミスタイプした数
        /// </summary>
        public int MissNum {
            get {
                return m_results.MissNum;
            }
        }

        /// <summary>
        /// 打ち終わったか
        /// </summary>
        public bool IsComplete {
            get {return m_results.IsComplete;}
        }

        /// <summary>
        /// 前回入力発生時のUnityイベント
        /// </summary>
        public Event Event {
            get {return m_results.Event;}
        }

        /// <summary>
        /// 前回イベント発生時の内部タイプ
        /// </summary>
        public CopyInputCheckerResults.INNER_EVENT_TYPE InnerEvent {
            get { return m_params.m_innerEvent; }
        }

        private bool m_isKana = false;
        /// <summary>
        /// <para>JISかな入力など、日本語を直接入力する方式を使用してエミュレートするかどうか</para>
        /// <para>処理中にセットされた場合は、即座に反映されません。</para>
        /// <para>Clear()が呼び出された時か、TargetStrに値がセットされた時に更新されます。</para>
        /// </summary>
        public bool IsKana {
            get { return m_results.IsKana; }
            set {
                m_isKana = value;
            }
        }

        private bool m_isCaseSensitive = false;
        /// <summary>
        /// <para>英語の大文字と小文字入力を区別して判定するか</para>
        /// <para>処理中にセットされた場合は、即座に反映されません。</para>
        /// <para>Clear()が呼び出された時か、TargetStrに値がセットされた時に更新されます。</para>
        /// </summary>
        public bool IsCaseSensitive{
            get { return m_results.IsCaseSensitive; }
            set {
                m_isCaseSensitive = value;
            }
        }
        #endregion

        #region イベントハンドラ
        /// <summary>
        /// キーボードから入力処理を行った時のイベントリスナを追加します。
        /// </summary>
        /// <param name="aEvent">イベントリスナ</param>
        public void AddEventListenerOnInput(UnityAction<CopyInputCheckerResults> aEvent) {
            m_onInputCallbacks.AddListener(aEvent);
        }

        /// <summary>
        /// キーボードから入力処理を行った時のイベントリスナを削除します。
        /// </summary>
        /// <param name="aEvent">イベントリスナ</param>
        public void RemoveEventListenerOnInput(UnityAction<CopyInputCheckerResults> aEvent) {
            m_onInputCallbacks.RemoveListener(aEvent);
        }

        /// <summary>
        /// 比較対象の文字に対して、正しく入力された時のイベントリスナを追加します。
        /// </summary>
        /// <param name="aEvent">イベントリスナ</param>
        public void AddEventListenerOnCorrect(UnityAction<CopyInputCheckerResults> aEvent) {
            m_onCorrectCallbacks.AddListener(aEvent);
        }

        /// <summary>
        /// 比較対象の文字に対して、正しく入力された時のイベントリスナを削除します。
        /// </summary>
        /// <param name="aEvent">イベントリスナ</param>
        public void RemoveEventListenerOnCorrect(UnityAction<CopyInputCheckerResults> aEvent) {
            m_onCorrectCallbacks.RemoveListener(aEvent);
        }

        /// <summary>
        /// 比較対象の文字に対して、ミスタッチした時のイベントリスナを追加します。
        /// </summary>
        /// <param name="aEvent">イベントリスナ</param>
        public void AddEventListenerOnMiss(UnityAction<CopyInputCheckerResults> aEvent) {
            m_onMissCallbacks.AddListener(aEvent);
        }

        /// <summary>
        /// 比較対象の文字に対して、ミスタッチした時のイベントリスナを削除します。
        /// </summary>
        /// <param name="aEvent">イベントリスナ</param>
        public void RemoveEventListenerOnMiss(UnityAction<CopyInputCheckerResults> aEvent) {
            m_onMissCallbacks.RemoveListener(aEvent);
        }

        /// <summary>
        /// 比較対象の文字が全て打てた時のイベントリスナを追加します。
        /// </summary>
        /// <param name="aEvent">イベントリスナ</param>
        public void AddEventListenerOnComplete(UnityAction<CopyInputCheckerResults> aEvent) {
            m_onCompleteCallbacks.AddListener(aEvent);
        }

        /// <summary>
        /// 比較対象の文字が全て打てた時のイベントリスナを追加します。
        /// </summary>
        /// <param name="aEvent">イベントリスナ</param>
        public void RemoveEventListenerOnComplete(UnityAction<CopyInputCheckerResults> aEvent) {
            m_onCompleteCallbacks.RemoveListener(aEvent);
        }
        #endregion

        #region 内部メソッド
        /// <summary>
        /// 指定された文字列を打つ為に必要な中間文字をセット
        /// </summary>
        private void InitStrYet() {
            int idx = 0;
            var p = m_params;
            var cvt = m_convertTableMgr;

            string target = p.m_targetStr;
            
            if (target.Length == 0) {
                p.m_isComplete = true;
                return; 
            }

            while (idx < target.Length) {
                int len = 1;
                string str = "";
                string strRaw = "";
                if (IsKana) {
                    for (; len <= cvt.Kana2KanaMid.KanaMaxLength; ++len) {
                        if (idx + len > target.Length) { break; }
                        string strTmp = target.Substring(idx, len);
                        string strRawTmp = "";
                        if (!cvt.Kana2KanaMid.TryConvert(strTmp, out strRawTmp)) {
                            break;
                        }
                        str = strTmp;
                        strRaw = strRawTmp;
                    }
                } else {
                    for (; len <= cvt.Kana2Roma.KanaMaxLength; ++len) {
                        if (idx + len > target.Length) { break; }
                        string strTmp = target.Substring(idx, len);
                        string strRawTmp = "";
                        if (!cvt.Kana2Roma.TryConvert(strTmp, out strRawTmp)) {
                            break;
                        }
                        str = strTmp;
                        strRaw = strRawTmp;
                    }
                }
                if (str.Length == 0) {
                    p.m_strYet.Add(target[idx] + "");
                    p.m_strYetRaws.Add(target[idx] + "");
                    idx++;
                } else {
                    p.m_strYet.Add(str);
                    p.m_strYetRaws.Add(strRaw);
                    idx += (len - 1);
                }
            }
        }

        /// <summary>
        /// 正しく打てた時の処理　現在打っている文字を更新
        /// </summary>
        /// <param name="isThrowEvent">true:内部でイベントを投げます</param>
        private void CorrectType(bool isThrowEvent = true) {
            if (IsComplete) { return; }
            m_results.Dirty = true;
            var p = m_params;
            if (p.m_strCurrentRaw.Length > 0) {
                p.m_strDoneRaws[p.m_strDoneRaws.Count - 1] += p.m_strCurrentRaw[0];
                p.m_strCurrentRaw = p.m_strCurrentRaw.Substring(1);
                p.m_strCurrentRawDone = "";
                if (p.m_strCurrentRawWork.Length == 0) {
                    p.m_strDone[p.m_strDone.Count - 1] += p.m_strCurrent;

                    p.m_correctCharNum += p.m_strCurrent.Length;
                }
                p.m_correctNum++;
            }
            if (p.m_strCurrentRawWork.Length == 0) {
                if (p.m_strYet.Count != 0) {
                    p.m_strDone.Add("");
                    p.m_strDoneRaws.Add("");
                    p.m_strCurrent = p.m_strYet[0];
                    p.m_strCurrentRawWork = p.m_strYetRaws[0];
                    p.m_strCurrentRawDone = "";
                    p.m_strYet.RemoveAt(0);
                    p.m_strYetRaws.RemoveAt(0);
                } else {//打ち切った
                    p.m_strCurrent = "";
                    p.m_isComplete =true;
                    if (isThrowEvent) {
                        p.m_innerEvent = CopyInputCheckerResults.INNER_EVENT_TYPE.COMPLETE;
                        m_onCompleteCallbacks.Invoke(m_results);
                    }
                    return;
                }
            }
            p.m_strCurrentRaw = p.m_strCurrentRawWork[0] + "";
            p.m_strCurrentRawWork = p.m_strCurrentRawWork.Substring(1);

            if (isThrowEvent) {
                p.m_innerEvent = CopyInputCheckerResults.INNER_EVENT_TYPE.CORRECT;
                m_onCorrectCallbacks.Invoke(m_results);
            }
        }

        /// <summary>
        /// ミスした時の処理
        /// </summary>
        /// <param name="isThrowEvent">true:内部でイベントを投げます</param>
        private void MissType(bool isThrowEvent = true) {
            if (IsComplete) { return; }
            m_params.m_missNum++;
            m_params.m_innerEvent = CopyInputCheckerResults.INNER_EVENT_TYPE.MISS;
            m_onMissCallbacks.Invoke(m_results);
        }
        #endregion

        #region メンバ
        private ConvertTableMgr m_convertTableMgr;

        private CopyInputCheckerParams m_params = new CopyInputCheckerParams();
        private CopyInputCheckerResults m_results;

        private UnityEvent<CopyInputCheckerResults> m_onInputCallbacks      = new UnityEvent<CopyInputCheckerResults>();
        private UnityEvent<CopyInputCheckerResults> m_onCorrectCallbacks    = new UnityEvent<CopyInputCheckerResults>();
        private UnityEvent<CopyInputCheckerResults> m_onMissCallbacks       = new UnityEvent<CopyInputCheckerResults>();
        private UnityEvent<CopyInputCheckerResults> m_onCompleteCallbacks   = new UnityEvent<CopyInputCheckerResults>();
        #endregion
    }
}
