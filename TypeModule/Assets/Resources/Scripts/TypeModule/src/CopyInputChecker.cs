using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// CopyInputCheckerによって処理されたデータへのアクセス用クラスです。
/// TypeModuleでModeをMODE_COPYにした時、入力発生時のイベントリスナで返却されます。
/// </summary>
public class CopyInputCheckerResults {

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
    /// 前回入力発生時のUnityイベント
    /// </summary>
    public Event Event {
        get {
            return m_params.m_event;
        }
    }

    /// <summary>
    /// JISかな入力など、日本語を直接入力する方式を使用してエミュレートするかどうか
    /// </summary>
    public bool IsKana{
        get {
            return m_params.m_isKana;
        }
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

        m_strYetRawCache = "";
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
            m_event = new Event();
        }
        #endregion

        #region メンバ
        public string       m_targetStr;                        //比較対象の文字列(タイピングのお台文)
        public List<string> m_strDone = new List<string>();     //既に打ち終わった文字列
        public string       m_strCurrent;                       //現在打っている文字
        public List<string> m_strYet = new List<string>();      //まだ打っていない文字列
        public List<string> m_strDoneRaws = new List<string>(); //既に打ち終わった文字列(変換前)
        public string       m_strCurrentRaw;                    //現在打っている文字(変換前)
        public List<string> m_strYetRaws = new List<string>();  //まだ打っていない文字列(変換前)
        public string       m_prevCorrectChar;                  //前回正しく入力された文字(ミスした時は空文字列)
        public string       m_prevMissChar;                     //前回ミスしたされた文字(正しく入力された時は空文字列)
        public int          m_correctNum;                       //正しくタイプした数
        public int          m_correctCharNum;                   //正しく打てた文字数
        public int          m_missNum;                          //ミスタイプした数
        public bool         m_isKana;       
        public Event        m_event = new Event();
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
            Clear();
            m_results = new CopyInputCheckerResults(in m_params);
        }
        #endregion

        #region メソッド
        /// <summary>
        /// <para>キーボードからの入力文字を追加</para>
        /// <para>比較対象の文字列がセットされていない場合はなにもしません。</para>
        /// </summary>
        /// <param name="aEvent">入力イベント</param>
        public void AddInput(in Event aEvent) {

        }

        /// <summary>
        /// <para>内部データをクリアします。</para>
        /// <para>入力された文字列は全てクリアされます</para>
        /// <para>比較対象文字[TargetStr]のみクリアされません。</para>
        /// </summary>
        public void Clear() {
            m_params.Clear();
            m_params.m_isKana = m_isKana;
        }
        #endregion

        #region フィールド
        /// <summary>
        /// 比較対象の文字列(タイピングのお台文)
        /// 値を変更した時点で、Clear()が自動で呼び出されます。
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
        /// 前回入力発生時のUnityイベント
        /// </summary>
        public Event Event {
            get {return m_results.Event;}
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
