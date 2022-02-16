using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Text.RegularExpressions;
using System;

namespace tm {

/// <summary>
/// <para>CopyInputCheckerによって処理されたデータへのアクセス用クラスです。</para>
/// <para>TypeModuleでModeをMODE_COPYにした時、入力発生時のイベントリスナで返却されます。</para>
/// </summary>
/// <example><code>
///     
/// //イベントリスナを追加し、文字列に変更があった時にGUIテキストを修正
/// TypeModule module = GetComponent&lt;TypeModule&gt;();
/// module.AddEventListeneronInput(onInput);
///         
///     ...
///     
/// public Text testInput;
/// public Text testInputRaw;
/// 
/// private void onInput(CopyInputCheckerResults res) {
///     Debug.Log("onInput");
///     testInput.text = res.StrDone + " " + res.StrCurrent + " " + res.StrYet;
///     testInputRaw.text = res.StrDoneRaw + " " + res.StrCurrentRaw + " " + res.StrYetRaw;
/// }
/// 
/// 
/// //イベントリスナを追加し、文字が打たれた時にサウンドを再生
/// public AudioSource audioSource;
/// public AudioClip correctSound;
/// public AudioClip missSound;
/// 
///     ...
/// 
/// module.AddEventListenerOnCorrect(onCorrect);
/// module.AddEventListenerOnMiss(onMiss);
/// audioSource = GetComponent&lt;AudioSource&gt;();
/// 
///     ...
/// 
/// private void onCorrect(CopyInputCheckerResults res){
///     Debug.Log("onCorrect");
///     audioSource.PlayOneShot(correctSound);
/// }
/// 
/// private void onMiss(CopyInputCheckerResults res){
///     Debug.Log("onMiss");
///     audioSource.PlayOneShot(missSound);
/// }
/// </code></example>
public class CopyInputCheckerResults {


    #region 内部イベントタイプ
    /// <summary>イベント発生時の内部タイプです</summary>
    public enum INNER_EVENT_TYPE{
        /// <summary>どの入力タイプにも属さない</summary>
        NONE,
        /// <summary>正しくタイプされた時</summary>
        CORRECT,
        /// <summary>ミスタイプした時</summary>
        MISS,
        /// <summary>全て打ち終わった時</summary>
        COMPLETE,
        /// <summary>内部データクリア・セットアップされた時</summary>
        SETUP,
    }
    #endregion


    #region フィールド
    /// <summary>比較対象の文字列(タイピングのお台文)</summary>
    public string TargetStr {get {return m_params.m_targetStr;}}

    /// <summary>既に打ち終わった文字列</summary>
    public string StrDone {
        get {
            if (Dirty) { CreateChche(); }
            return m_strDoneCache;
        }
    }

    /// <summary>現在打とうとしている文字</summary>
    public string StrCurrent {get {return m_params.m_strCurrent;}}

    /// <summary>まだ打っていない文字列</summary>
    public string StrYet {
        get {
            if (Dirty) { CreateChche(); }
            return m_strYetCache;
        }
    }

    /// <summary>既に打ち終わった文字列(変換前)</summary>
    public string StrDoneRaw {
        get {
            if (Dirty) { CreateChche(); }
            return m_strDoneRawCache;
        }
    }

    /// <summary>現在打っている文字(変換前)</summary>
    public string StrCurrentRaw {get {return m_params.m_strCurrentRaw;}}

    /// <summary>既に打ち終わった文字列(変換前)</summary>
    public string StrYetRaw {
        get {
            if (Dirty) { CreateChche(); }
            return m_strYetRawCache;
        }
    }

    /// <summary>前回入力された文字</summary>
    public string PrevChar { 
        get {
            if (Dirty) { CreateChche(); }
            return m_prevCharCache; 
        } 
    }

    /// <summary>前回正しく入力された文字(ミスした時は空文字列)</summary>
    public string PrevCorrectChar {get {return m_params.m_prevCorrectChar;}}

    /// <summary>前回ミスしたされた文字(正しく入力された時は空文字列)</summary>
    public string PrevMissChar {get {return m_params.m_prevMissChar;}}

    /// <summary>正しくタイプした数</summary>
    public int CorrectNum {get {return m_params.m_correctNum;  }}

    /// <summary>正しく打てた文字数</summary>
    public int CorrectCharNum {get {return m_params.m_correctCharNum;}}

    /// <summary>ミスタイプした数</summary>
    public int MissNum {get {return m_params.m_missNum;}}

    /// <summary>打ち終わったか</summary>
    public bool IsComplete {get {return m_params.m_isComplete;}}

    /// <summary>前回入力発生時のUnityイベント</summary>
    public Event Event {get {return m_params.m_event;}}

    /// <summary>前回イベント発生時の内部タイプ</summary>
    public INNER_EVENT_TYPE InnerEvent {get { return m_params.m_innerEvent; }}

    /// <summary>JISかな入力など、日本語を直接入力する方式を使用しているか</summary>
    public bool IsKana{get {return m_params.m_isKana;}}

    /// <summary>英語の大文字と小文字入力を区別して判定するか</summary>
    public bool IsCaseSensitive {get { return m_params.m_isCaseSensitive; }}
    #endregion


    #region 生成
    /// <summary>CopyInputCheckerより作成されます。外からは作成しないでください。</summary>    
    public CopyInputCheckerResults(in Inner.CopyInputCheckerParams aParams) {m_params = aParams;}
    #endregion


    #region 内部使用プロパティ
    /// <summary>汚しフラグです。trueになっている時に文字列にアクセスされた場合、データを再度作成します。</summary>
    public bool Dirty { get; set; } = true;
    #endregion


    #region 内部メソッド
    /// <summary>返却パラメータのキャッシュを生成します。</summary>
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

        m_strYetRawCache = m_params.m_strRomaWorkYet;
        foreach (string r in m_params.m_strYetRaws) {
            m_strYetRawCache += r;
        }

        m_prevCharCache = "";
        switch (InnerEvent) {
            case INNER_EVENT_TYPE.CORRECT:  m_prevCharCache = PrevCorrectChar;break;
            case INNER_EVENT_TYPE.MISS:     m_prevCharCache = PrevMissChar;break;
        }
        Dirty = false;
    }
    #endregion


    #region メンバ
    private Inner.CopyInputCheckerParams m_params;   //参照

    private string m_strDoneCache = "";
    private string m_strYetCache = "";
    private string m_strDoneRawCache = "";
    private string m_strYetRawCache = "";
    private string m_prevCharCache = "";
    #endregion
}


    namespace Inner {

        /// <summary>CopyInputCheckerの内部パラメータクラス。CopyInputCheckerResultsの参照渡しに使用。</summary>
        public class CopyInputCheckerParams {


            #region メソッド
            /// <summary>内部データをクリアします。</summary>
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
                m_strRomaWorkDone = "";
                m_strRomaWorkYet = "";
                m_isComplete = false;
                m_event = new Event();
                m_innerEvent = CopyInputCheckerResults.INNER_EVENT_TYPE.NONE;
            }
            #endregion


            #region メンバ
            ///<summary>比較対象の文字列(タイピングのお台文)</summary>
            public string       m_targetStr = "";
            ///<summary>既に打ち終わった文字列</summary>
            public List<string> m_strDone = new List<string>();
            ///<summary>既に打ち終わった文字列(変換前)</summary>
            public List<string> m_strDoneRaws = new List<string>();
            ///<summary>現在打っている文字</summary>
            public string       m_strCurrent;
            ///<summary>現在打っている文字(変換前)</summary>
            public string       m_strCurrentRaw;
            ///<summary>まだ打っていない文字列</summary>
            public List<string> m_strYet = new List<string>();
            ///<summary>まだ打っていない文字列(変換前)</summary>
            public List<string> m_strYetRaws = new List<string>();
            ///<summary>前回正しく入力された文字(ミスした時は空文字列)</summary>
            public string       m_prevCorrectChar;                  
            ///<summary>前回ミスしたされた文字(正しく入力された時は空文字列)</summary>
            public string       m_prevMissChar;
            ///<summary>正しくタイプした数</summary>
            public int          m_correctNum;                       
            ///<summary>正しく打てた文字数</summary>
            public int          m_correctCharNum;
            ///<summary>ミスタイプした数</summary>
            public int          m_missNum;
            ///<summary>かな入力方式か</summary>
            public bool         m_isKana;
            ///<summary>英語の大文字小文字を判定するか</summary>
            public bool         m_isCaseSensitive;
            ///<summary>文字が打ち終わったか</summary>
            public bool         m_isComplete;
            ///<summary>直前に発生したUnityイベント</summary>
            public Event        m_event = new Event();
            ///<summary>内部イベントタイプ</summary>
            public CopyInputCheckerResults.INNER_EVENT_TYPE m_innerEvent;
            ///<summary>現在打っているローマ字の入力終了部分</summary>
            public string       m_strRomaWorkDone;
            ///<summary>現在打っているローマ字の未入力部分</summary>
            public string       m_strRomaWorkYet;        
            #endregion
        }


        /// <summary>
        /// <para>指定された文字列が正しく打ててるかを確認します。</para>
        /// <para>タイピングゲームで、お題の文字を真似して打たせる時に使用してください。</para>
        /// </summary>
        /// /// <example><code>
        ///     
        ///     ...
        ///     
        /// //初期化処理
        /// ConvertTableMgr table = new ConvertTableMgr();
        /// CopyInputChecker checker = new CopyInputChecker(in table);
        /// 
        /// //比較対象の文字列をセット(内部初期化もされます)
        /// checker.TargetStr = "こちらは、たいぴんぐするぶんしょうです。";
        /// 
        ///     ...
        ///     
        /// //KeyCodeからお台文と比較
        /// private void OnGUI(){
        ///     if (Event.current.type == EventType.KeyDown) {
        ///         checker.AddInput(Event.current);
        ///     }
        /// }
        /// 
        ///     ...
        ///     
        /// //直前に入力された文字を取得
        /// Debug.Log(checker.PrevChar);         
        /// Debug.Log(checker.PrevCorrectChar);         //正しく入力された場合格納
        /// Debug.Log(checker.PrevMissChar);            //ミス入力の場合格納
        /// 
        /// //その他パラメータにアクセス
        /// Debug.Log(checker.CorrectNum);              //正しくタイプした数
        /// Debug.Log(checker.CorrectCharNum);          //正しく打てた文字数
        /// Debug.Log(checker.MissNum);                 //ミスタイプした数
        /// Debug.Log(checker.IsComplete);              //指定文字列を打ち切ったか
        /// 
        /// //入力が発生した時のイベントリスナを追加
        /// private void onInput(InputEmulatorResults res) {
        ///     Debug.Log("onInput");
        ///     Debug.Log("str:" + res.StrDone + " " + res.StrCurrent + " " + res.StrYet);
        ///     Debug.Log("str(ローマ時):" + res.StrDoneRaw + " " + res.StrCurrentRaw + " " + res.StrYetRaw);
        /// }
        /// //正しく入力された時のイベントリスナを追加
        /// private void onCorrect(InputEmulatorResults res) {
        ///     Debug.Log("onCorrect");
        /// }
        /// //ミス入力された時のイベントリスナを追加
        /// private void onMiss(InputEmulatorResults res) {
        ///     Debug.Log("onMiss");
        /// }
        /// //指定された文字列を全て打ちきった時のイベントリスナを追加
        /// private void onComplete(InputEmulatorResults res) {
        ///     Debug.Log("onComplete");
        /// }
        /// 
        /// checker.AddEventListenerOnInput(onInput);
        /// checker.AddEventListenerOnCorrect(onCorrect);
        /// checker.AddEventListenerOnMiss(onMiss);
        /// checker.AddEventListenerOnComplete(onComplete);
        /// 
        /// 
        /// //以下オプションです================================
        /// 
        /// //ローマ字入力方式から、かな入力方式に切り替え
        /// checker.IsKana = true;
        /// 
        /// //英語の大文字と小文字入力を区別して判定する
        /// checker.IsCaseSensitive = true;
        /// 
        /// //プログラム側から、入力処理をエミュレート
        /// checker.Correct();              //正しく一度入力したとして処理
        /// checker.Miss();                 //ミスタイプしたとして処理
        /// checker.AddInput(KeyCode.A);    //Aキーが押されたとして処理
        /// 
        /// </code></example>
        public class CopyInputChecker {


            #region 生成
            /// <summary>
            /// <para>指定された文字列が正しく打ててるかを確認します。</para>
            /// <para>タイピングゲームで、お題の文字を真似して打たせる時に使用してください。</para>
            /// </summary>
            ///<param name="aConvertTableMgr">文字列生成時に使用する、変換テーブルを管理するクラス</param>
            public CopyInputChecker(in ConvertTableMgr aConvertTableMgr) {
                m_convertTableMgr = aConvertTableMgr;
                m_results = new CopyInputCheckerResults(in m_params);
                m_tmpEvent = new Event();
            }
            #endregion


            #region メソッド
            /// <summary>
            /// <para>キー入力イベントからの入力を追加</para>
            /// <para>もし、TargetStrに文字列がセットされていない場合や、既に打ち切っている状態の場合は何もしません。</para>
            /// </summary>
            /// <param name="aEvent">入力イベント</param>
            public void AddInput(in Event aEvent) {
                 //IMEによって、1回のキー入力に対して二回呼び出しが発生する為、更新しない
                if (aEvent.keyCode == KeyCode.None) {return;}
                m_tmpEvent = aEvent;
                AddInput(aEvent.keyCode, aEvent.shift, aEvent.functionKey, false);
            }


            /// <summary>
            /// <para>キーコードからの入力を追加</para>
            /// <para>もし、TargetStrに文字列がセットされていない場合や、既に打ち切っている状態の場合は何もしません。</para>
            /// </summary>
            /// <param name="aKeyCode">キーコード</param>
            /// <param name="aIsShift">SHIFT中か</param>
            /// <param name="aIsFunction">FUNCTION中か(2箇所ある\の判定に必須。通常はfalseとしてください)</param>
            /// <param name="aIsOuter">外のプログラム側から呼び出す場合はtrueとしてください</param>
            public void AddInput(KeyCode aKeyCode, bool aIsShift = false, bool aIsFunction = false, bool aIsOuter = false) {
                if (IsComplete) { return; }
                if (aIsOuter) {
                    m_tmpEvent = new Event();
                }
                var p = m_params;
                var cvt = m_convertTableMgr;
                char nCh;
                //英語チェック
                if (p.m_strCurrent.Length == 1) {
                    nCh = cvt.Key2Roma.Convert(aKeyCode, aIsShift, aIsFunction);
                    if (nCh == '\0') { return; }
                    ////かな且つ、以下の文字ならここで判定しない
                    if (IsKana && Regex.IsMatch(p.m_strCurrent[0] + "", "[、。ー・「」]")) {
                    } else {
                        if (IsCaseSensitive) {
                            if (p.m_strCurrent[0] == nCh) {
                                CorrectInner(nCh + "");
                                return;
                            } else if (Util.IsAlpha(p.m_strCurrent[0])) {
                                //英語入力中で文字が違うならミス判定
                                p.m_prevMissChar = nCh + "";
                                MissInner(nCh + "");
                                return;
                            }
                        } else {
                            if (char.ToLower(p.m_strCurrent[0]) == char.ToLower(nCh)) {
                                CorrectInner(nCh + "");
                                return;
                            }
                        }
                        //全角チェック
                        string nChZen;
                        if (Util.TryHanToZen(nCh + "", out nChZen, Util.ConvertTypes.Number | Util.ConvertTypes.Symbol)) {
                            if (string.Compare(p.m_strCurrent, nChZen) == 0) {
                                CorrectInner(nCh + "");
                                return;
                            }
                        }
                    }
                }
                //かなチェック
                if (IsKana) {
                    nCh = cvt.Key2kanaMid.Convert(aKeyCode, aIsShift, aIsFunction);
                    if (nCh == '\0') { return; }
                    string nChHan;
                    if (p.m_strCurrentRaw[0] == nCh) {
                        CorrectInner(nCh + "");
                        return;
                    } else if (Util.TryZenToHan(nCh + "", out nChHan, Util.ConvertTypes.Number | Util.ConvertTypes.Symbol)) {
                        if (p.m_strCurrentRaw[0] == nChHan[0]) {
                            CorrectInner(nCh + "");
                            return;
                        }
                    }
                } else {//ローマ字入力
                    nCh = cvt.Key2Roma.Convert(aKeyCode, aIsShift, aIsFunction);
                    if (nCh == '\0') { return; }

                    //「ん」の単発入力確定処理
                    if (string.Compare(p.m_strCurrent, "ん") == 0) {
                        string tmpRoma = p.m_strDoneRaws[p.m_strDoneRaws.Count - 1] + nCh;
                        if (Roma2KanaTable.CanConverFirstN(tmpRoma) && p.m_strYet.Count > 0) {
                            CorrectInner(nCh + "", false);
                            string prevStr = p.m_strDoneRaws[p.m_strDoneRaws.Count - 2];
                            p.m_strDoneRaws[p.m_strDoneRaws.Count - 2] = prevStr.Substring(0, prevStr.Length - 1);
                            p.m_correctNum--;
                        }
                    }

                    if (char.ToLower(p.m_strCurrentRaw[0]) == char.ToLower(nCh)) {
                        CorrectInner(nCh + "");
                        return;
                    }

                    //ローマ字入力の場合、途中でローマ字パターンが変わる事がある。
                    //例)「にゃ」が[nya]から[ni][xya]に変わる事がある
                    string tmpRomaStart = p.m_strRomaWorkDone + nCh;
                    for (int chLen = p.m_strCurrent.Length; chLen > 0; --chLen) {
                        string tmpKana = p.m_strCurrent.Substring(0, chLen);
                        string outRoma;
                        if (cvt.Kana2Roma.TryConvert(tmpKana, out outRoma, tmpRomaStart)) {
                            if (chLen != p.m_strCurrent.Length) {
                                p.m_strYet.Insert(0, p.m_strCurrent.Substring(chLen));
                                p.m_strCurrent = tmpKana;
                                string strNextRoma = "";
                                cvt.Kana2Roma.TryConvert(p.m_strYet[0], out strNextRoma);
                                p.m_strYetRaws.Insert(0, strNextRoma);
                            }
                            p.m_strCurrentRaw = outRoma[p.m_strRomaWorkDone.Length] + "";
                            p.m_strRomaWorkYet = outRoma.Substring(p.m_strRomaWorkDone.Length + 1);
                            outRoma = outRoma.Substring(tmpKana.Length);
                            CorrectInner(nCh + "");
                            return;
                        }
                    }
                }
                //ミス
                MissInner(nCh + "");
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
                CorrectInner("", false);
                m_tmpEvent = new Event();
                m_params.m_innerEvent = CopyInputCheckerResults.INNER_EVENT_TYPE.SETUP;
                m_onSetupCallbacks.Invoke(m_results);
            }

            /// <summary>
            /// <para>プログラムから、正しい入力を一回行ったとして処理します。</para>
            /// <para>もし、TargetStrに文字列がセットされていない場合や、既に打ち切っている状態の場合は何もしません。</para>
            /// </summary>
            /// <param name="isThrowEvent">true:処理後、登録されたイベントリスナにイベントを投げます</param>
            public void Correct(bool isThrowEvent = true) {
                if (IsComplete || m_params.m_strCurrent.Length == 0) { return; }
                m_tmpEvent = new Event();
                CorrectInner(m_params.m_strCurrent[0] + "", isThrowEvent);
            }

            /// <summary>
            /// <para>プログラムから、ミスタイプを一回行ったとして処理します。</para>
            /// <para>もし、TargetStrに文字列がセットされていない場合や、既に打ち切っている状態の場合は何もしません。</para>
            /// </summary>
            /// <param name="isThrowEvent">true:処理後、登録されたイベントリスナにイベントを投げます</param>
            public void Miss(bool isThrowEvent = true) {
                m_tmpEvent = new Event();
                MissInner("", isThrowEvent);
            }
            #endregion


            #region フィールド
            /// <summary>
            /// <para>比較対象の文字列(タイピングのお台文)</para>
            /// <para>値を変更した時点で、初期化処理を自動で呼び出します。</para>
            /// <para>また、全角半角カタカナはひらがなに、全角英語は半角英語に変換されます。</para>
            /// <para>[含む事が出来る文字]: -_| -_に対応する大文字|ァ-ヴ|ぁ-ゔ</para>
            /// <para>含めない文字があった場合、エディタ上での実行なら例外を投げて止まります。ビルド版だとその文字が削除されます。</para>
            /// </summary>
            public string TargetStr {
                get {return m_results.TargetStr;}
                set {
                    SetTargetStr(value);
                    Clear();
                }
            }

            /// <summary>既に打ち終わった文字列</summary>
            public string StrDone {get {return m_results.StrDone;}}

            /// <summary>現在打っている文字</summary>
            public string StrCurrent {get { return m_results.StrCurrent; }}

            /// <summary>まだ打っていない文字列</summary>
            public string StrYet {get { return m_results.StrYet; }}

            /// <summary>既に打ち終わった文字列(変換前)</summary>
            public string StrDoneRaw {get { return m_results.StrDoneRaw; }}

            /// <summary>現在打っている文字(変換前)</summary>
            public string StrCurrentRaw {get { return m_results.StrCurrentRaw; }}

            /// <summary>既に打ち終わった文字列(変換前)</summary>
            public string StrYetRaw {get { return m_results.StrYetRaw; }}

            /// <summary>前回入力された文字</summary>
            public string PrevChar { get { return m_results.PrevChar; } }

            /// <summary>前回正しく入力された文字(ミスした時は空文字列)</summary>
            public string PrevCorrectChar {get { return m_results.PrevCorrectChar; }}

            /// <summary>前回ミスしたされた文字(正しく入力された時は空文字列)</summary>
            public string PrevMissChar {get {return m_results.PrevMissChar;}}

            /// <summary>正しくタイプした数</summary>
            public int CorrectNum {get {return m_results.CorrectNum;}}

            /// <summary>正しく打てた文字数</summary>
            public int CorrectCharNum {get { return m_results.CorrectCharNum; }}

            /// <summary>ミスタイプした数</summary>
            public int MissNum {get {return m_results.MissNum;}}

            /// <summary>打ち終わったか</summary>
            public bool IsComplete {get {return m_results.IsComplete;}}

            /// <summary>前回入力発生時のUnityイベント</summary>
            public Event Event {get {return m_results.Event;}}

            /// <summary>前回イベント発生時の内部タイプ</summary>
            public CopyInputCheckerResults.INNER_EVENT_TYPE InnerEvent {get { return m_params.m_innerEvent; }}

            private bool m_isKana = false;
            /// <summary>
            /// <para>JISかな入力など、日本語を直接入力する方式を使用してエミュレートするかどうか</para>
            /// <para>処理中にセットされた場合は、即座に反映されません。</para>
            /// <para>Clear()が呼び出された時か、TargetStrに値がセットされた時に更新されます。</para>
            /// </summary>
            public bool IsKana {
                get { return m_results.IsKana; }
                set {m_isKana = value;}
            }

            private bool m_isCaseSensitive = false;
            /// <summary>
            /// <para>英語の大文字と小文字入力を区別して判定するか</para>
            /// <para>処理中にセットされた場合は、即座に反映されません。</para>
            /// <para>Clear()が呼び出された時か、TargetStrに値がセットされた時に更新されます。</para>
            /// </summary>
            public bool IsCaseSensitive{
                get { return m_results.IsCaseSensitive; }
                set {m_isCaseSensitive = value;}
            }
            #endregion


            #region イベントハンドラ
            /// <summary>キーボードからの入力処理を行った時のイベントリスナを追加します。</summary>
            /// <param name="aEvent">イベントリスナ</param>
            public void AddEventListenerOnInput(UnityAction<CopyInputCheckerResults> aEvent) {m_onInputCallbacks.AddListener(aEvent);}

            /// <summary>キーボードからの入力処理を行った時のイベントリスナを削除します。</summary>
            /// <param name="aEvent">イベントリスナ</param>
            public void RemoveEventListenerOnInput(UnityAction<CopyInputCheckerResults> aEvent) {m_onInputCallbacks.RemoveListener(aEvent);}

            /// <summary>比較対象の文字に対して、正しく入力された時のイベントリスナを追加します。</summary>
            /// <param name="aEvent">イベントリスナ</param>
            public void AddEventListenerOnCorrect(UnityAction<CopyInputCheckerResults> aEvent) {m_onCorrectCallbacks.AddListener(aEvent);}

            /// <summary>比較対象の文字に対して、正しく入力された時のイベントリスナを削除します。</summary>
            /// <param name="aEvent">イベントリスナ</param>
            public void RemoveEventListenerOnCorrect(UnityAction<CopyInputCheckerResults> aEvent) {m_onCorrectCallbacks.RemoveListener(aEvent);}

            /// <summary>比較対象の文字に対して、ミスタッチした時のイベントリスナを追加します。</summary>
            /// <param name="aEvent">イベントリスナ</param>
            public void AddEventListenerOnMiss(UnityAction<CopyInputCheckerResults> aEvent) {m_onMissCallbacks.AddListener(aEvent);}

            /// <summary>比較対象の文字に対して、ミスタッチした時のイベントリスナを削除します。</summary>
            /// <param name="aEvent">イベントリスナ</param>
            public void RemoveEventListenerOnMiss(UnityAction<CopyInputCheckerResults> aEvent) {m_onMissCallbacks.RemoveListener(aEvent);}

            /// <summary>比較対象の文字を全て打ちきった時のイベントリスナを追加します。</summary>
            /// <param name="aEvent">イベントリスナ</param>
            public void AddEventListenerOnComplete(UnityAction<CopyInputCheckerResults> aEvent) {m_onCompleteCallbacks.AddListener(aEvent);}

            /// <summary>比較対象の文字を全て打ちきった時のイベントリスナを削除します。</summary>
            /// <param name="aEvent">イベントリスナ</param>
            public void RemoveEventListenerOnComplete(UnityAction<CopyInputCheckerResults> aEvent) {m_onCompleteCallbacks.RemoveListener(aEvent);}

            /// <summary>比較文字がセットされ、セットアップが完了した時のイベントリスナを追加します。</summary>
            /// <param name="aEvent">イベントリスナ</param>
            public void AddEventListenerOnSetup(UnityAction<CopyInputCheckerResults> aEvent) {m_onSetupCallbacks.AddListener(aEvent);}

            /// <summary>比較文字がセットされ、セットアップが完了した時のイベントリスナ削除します。</summary>
            /// <param name="aEvent">イベントリスナ</param>
            public void RemoveEventListenerOnSetup(UnityAction<CopyInputCheckerResults> aEvent) {m_onSetupCallbacks.RemoveListener(aEvent);}
            #endregion


            #region 内部メソッド
            /// <summary>比較対象の文字列をチェックしセット</summary>
            /// <param name="aStr">プロパティとしてセットされた文字列</param>
            private void SetTargetStr(string aStr) {
                aStr = Util.HanToZen(aStr,Util.ConvertTypes.Katakana);
                aStr = Util.ZenToHan(aStr, Util.ConvertTypes.Alphabet | Util.ConvertTypes.Number);
                aStr = Util.KatakanaToHiragana(aStr);

                string tmp = "";
                for (int i = 0; i < aStr.Length; ++i) {
                    if (Util.IsHiragana(aStr[i]) ||
                        Util.IsJPSymbol(aStr[i]) ||
                        Util.IsSpace(aStr[i]) ||
                        char.IsDigit(aStr[i]) ||
                        Util.IsAlpha(aStr[i]) || 
                        Util.IsSymbol(aStr[i], true)) {
                        tmp += aStr[i];
                    } else {
    #if UNITY_EDITOR
                        Debug.Assert(false, "TypeModule:TargetStrに不正な文字列が挿入されました。該当文字:[" + aStr[i] + "] 文字列:[" + aStr + "]");
                        throw new Exception("TypeModule:TargetStrに不正な文字列が挿入されました。該当文字:[" + aStr[i] + "] 文字列:[" + aStr + "]");
    #endif
                    }
                }
                m_params.m_targetStr = tmp;
            }

            /// <summary>指定された文字列を打つ為に必要な中間文字をセット</summary>
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

                        //ガイド文字の記号数字は半角へ
                        string raw = target[idx] + "";
                        string rawTmp;
                        if (Util.TryZenToHan(target[idx] + "", out rawTmp, Util.ConvertTypes.Number | Util.ConvertTypes.Symbol)) {
                            ////かなの場合、以下の文字は半角に変換しない
                            if (IsKana && Regex.IsMatch(target[idx] + "", "[、。ー・「」]")) {
                            } else {
                                raw = rawTmp;
                            }
                        }
                        //大文字小文字を区別しないなら、アルファベットは小文字
                        if (!IsCaseSensitive) {
                            raw = char.ToLower(raw[0]) + "";
                        }
                        p.m_strYetRaws.Add(raw);

                        idx++;
                    } else {
                        p.m_strYet.Add(str);
                        p.m_strYetRaws.Add(strRaw);
                        idx += (len - 1);
                    }
                }
            }

            /// <summary>正しく打てた時の処理　現在打っている文字を更新</summary>
            /// <param name="aPrevChar">今回打った文字</param>
            /// <param name="isThrowEvent">true:内部でイベントを投げます</param>
            private void CorrectInner(string aPrevChar, bool isThrowEvent = true) {
                if (IsComplete) { return; }
                var p = m_params;
                p.m_event = m_tmpEvent;
                p.m_prevCorrectChar = aPrevChar;
                p.m_prevMissChar = "";

                m_results.Dirty = true;
                if (p.m_strCurrentRaw.Length > 0) {
                    p.m_strDoneRaws[p.m_strDoneRaws.Count - 1] += p.m_strCurrentRaw[0];
                    p.m_strRomaWorkDone = p.m_strCurrentRaw[0] + "";
                    p.m_strCurrentRaw = p.m_strCurrentRaw.Substring(1);
                    if (p.m_strRomaWorkYet.Length == 0) {
                        p.m_strDone[p.m_strDone.Count - 1] += p.m_strCurrent;

                        p.m_correctCharNum += p.m_strCurrent.Length;
                    }
                    p.m_correctNum++;
                }
                if (p.m_strRomaWorkYet.Length == 0) {
                    if (p.m_strYet.Count != 0) {
                        p.m_strDone.Add("");
                        p.m_strDoneRaws.Add("");
                        p.m_strCurrent = p.m_strYet[0];
                        p.m_strRomaWorkYet = p.m_strYetRaws[0];
                        p.m_strRomaWorkDone = "";
                        p.m_strYet.RemoveAt(0);
                        p.m_strYetRaws.RemoveAt(0);
                    } else {//打ち切った
                        p.m_strCurrent = "";
                        p.m_isComplete =true;
                        if (isThrowEvent) {
                            p.m_innerEvent = CopyInputCheckerResults.INNER_EVENT_TYPE.COMPLETE;
                            m_onInputCallbacks.Invoke(m_results);
                            m_onCompleteCallbacks.Invoke(m_results);
                        }
                        return;
                    }
                }
                p.m_strCurrentRaw = p.m_strRomaWorkYet[0] + "";
                p.m_strRomaWorkYet = p.m_strRomaWorkYet.Substring(1);

                if (isThrowEvent) {
                    p.m_innerEvent = CopyInputCheckerResults.INNER_EVENT_TYPE.CORRECT;
                    m_onInputCallbacks.Invoke(m_results);
                    m_onCorrectCallbacks.Invoke(m_results);
                }
            }

            /// <summary>ミスした時の処理</summary>
            /// <param name="aPrevChar">今回打った文字</param>
            /// <param name="isThrowEvent">true:内部でイベントを投げます</param>
            private void MissInner(string aPrevChar, bool isThrowEvent = true) {
                if (IsComplete) { return; }
                var p = m_params;
                p.m_event = m_tmpEvent;
                p.m_prevMissChar = aPrevChar;
                p.m_prevCorrectChar = "";

                p.m_missNum++;
                p.m_innerEvent = CopyInputCheckerResults.INNER_EVENT_TYPE.MISS;
                if (isThrowEvent) {
                    m_onInputCallbacks.Invoke(m_results);
                    m_onMissCallbacks.Invoke(m_results);
                }
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
            private UnityEvent<CopyInputCheckerResults> m_onSetupCallbacks      = new UnityEvent<CopyInputCheckerResults>();

            private Event m_tmpEvent = null;
            #endregion
        }
    }
}