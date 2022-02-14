using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


/// <summary>
/// <para>InputEmulatorによって処理されたデータへのアクセス用クラスです。</para>
/// <para>TypeModuleでModeをINPUTにした時、入力発生時のイベントリスナで返却されます。</para>
/// </summary>
/// <example><code>
///     
/// //イベントリスナを追加し、文字列に変更があった時にGUIテキストを修正
/// module = GetComponent<TypeModule>();
/// module.AddEventListenerOnChange(onChange);
///         
///     ...
///     
/// public Text testInput;
/// public Text testInputRaw;
/// 
/// private void onChange(InputEmulatorResults res) {
///     Debug.Log("onChange");
///     
///     testInput.text = res.Str;
///     testInputRaw.text = res.StrRaw;
/// }
/// 
/// //イベントリスナを追加し、文字が打たれた時にサウンドを再生
/// public AudioSource audioSource;
/// public AudioClip typeSound;
/// public AudioClip bsSound;
/// public AudioClip enterSound;
/// 
///         ...
/// 
/// module.AddEventListenerOnInput(onInput);
/// audioSource = GetComponent<AudioSource>();
/// 
///         ...
/// 
/// private void onInput(InputEmulatorResults res){
///     Debug.Log("onInput");
///     switch(res.InputType){
///         case InputEmulatorResults.INPUT_TYPE.INPUT:
///             audioSource.PlayOneShot(typeSound);
///             break;
///         case InputEmulatorResults.INPUT_TYPE.BS:
///             audioSource.PlayOneShot(bsSound);
///             break;
///         case InputEmulatorResults.INPUT_TYPE.ENTER:
///             audioSource.PlayOneShot(enterSound);
///             break;
///     }
/// }
/// </code></example>
public class InputEmulatorResults {


    #region 入力タイプ
    /// <summary>イベント発生時の入力タイプです</summary>
    public enum INPUT_TYPE {
        /// <summary>どの入力タイプにも属さない<summary>
        NONE,
        /// <summary>通常の入力タイプ。キーボードから文字が打たれ、末尾に文字が追加された場合このタイプになります。<summary>
        INPUT,
        /// <summary>キーボードからBSキーが打たれ、1文字削除された時にこのタイプになります。<summary>
        BS,
        /// <summary>キーボードからEnterキーが打たれ、変換中の文字が確定された時にこのタイプになります。<summary>
        ENTER,
        /// <summary>プログラム側か、システム側からBSキーが打たれ、1文字削除された時にこのタイプになります。<summary>
        BS_FORCE,
        /// <summary>プログラム側か、システム側からEnterキーが打たれ、変換中の文字が確定された時にこのタイプになります。<summary>
        ENTER_FORCE,
        /// <summary>システム側から入力値が全て初期化された<summary>
        CLEAR,
    }
    #endregion


    #region フィールド
    /// <summary>生成された文字列</summary>
    public string Str {
        get {
            if (Dirty) { CreateChche(); }
            return m_strCache;
        }
    }

    /// <summary>生成された、変換される前の文字列</summary>
    public string StrRaw {
        get {
            if (Dirty) { CreateChche(); }
            return m_strRawCache;
        }
    }

    /// <summary>前回入力された文字(変換前)</summary>
    public string PrevChar {get { return m_params.m_prevChar; }}

    /// <summary>入力発生時のイベント</summary>
    public Event Event {get { return m_params.m_event; }}

    /// <summary>前回の入力タイプ</summary>
    public INPUT_TYPE InputType {get { return m_params.m_inputType; }}
    #endregion


    #region 生成
    /// <summary>InputEmulatorより作成されます。外からは作成しないでください。</summary>    
    public InputEmulatorResults(in tpInner.InputEmulatorParams aParams) {m_params = aParams;}
    #endregion


    #region 内部使用プロパティ
    /// <summary>汚しフラグです。trueになっている時に文字列にアクセスされた場合、データを再度作成します。</summary>
    public bool Dirty { get; set; } = true;
    #endregion


    #region 内部メソッド
    /// <summary>返却パラメータのキャッシュを生成します。</summary>
    private void CreateChche() {
        m_strCache = "";
        foreach (string r in m_params.m_strDone) {
            m_strCache += r;
        }
        m_strCache += m_params.m_strWork;

        m_strRawCache = "";
        foreach (string r in m_params.m_strDoneRaws) {
            m_strRawCache += r;
        }
        m_strRawCache += m_params.m_strWork;

        Dirty = false;
    }
    #endregion


    #region メンバ
    private tpInner.InputEmulatorParams m_params;   //参照

    private string m_strCache       = "";
    private string m_strRawCache    = "";
    #endregion
}


namespace tpInner {

    /// <summary>InputEmulatorの内部パラメータクラス。InputEmulatorResultsの参照渡しに使用。</summary>
    public class InputEmulatorParams {


        #region メソッド
        /// <summary内部データをクリアします。</summary>
        public void Clear() {
            m_strDone.Clear();
            m_strDoneRaws.Clear();
            m_strWork   = "";
            m_prevChar  = "";
            m_inputType = InputEmulatorResults.INPUT_TYPE.NONE;
            m_event     = new Event();
        }
        #endregion


        #region メンバ
        public List<string> m_strDone       = new List<string>();       //変換確定済みの文字列
        public List<string> m_strDoneRaws   = new List<string>();       //変換確定前文字列
        public string m_strWork         = "";                           //現在チェック中の文字
        public string m_prevChar        = "";
        public InputEmulatorResults.INPUT_TYPE m_inputType;
        public Event m_event;
        #endregion
    }

    /// <summaryキーボードの入力から文字列生成をエミュレートします</summary>
    /// <example><code>
    ///     
    ///     ...
    ///     
    /// //初期化処理
    /// ConvertTableMgr table = new ConvertTableMgr();
    /// InputEmulator input = new InputEmulator(in table);
    /// 
    ///     ...
    ///     
    /// //KeyCodeから文字を生成
    /// private void OnGUI(){
    ///     if (Event.current.type == EventType.KeyDown) {
    ///         input.AddInput(Event.current);
    ///     }
    /// }
    /// 
    ///     ...
    ///     
    /// //生成された文字列を取得
    /// Debug.Log(input.Str);        //生成された文字列
    /// Debug.Log(input.StrRaw);     //生成された、変換される前の文字列
    /// 
    /// //直前に入力された文字を取得
    /// Debug.Log(input.PrevChar);
    /// 
    /// //入力された文字列を全てクリア
    /// input.Clear();
    /// 
    ///     ...
    ///
    /// //入力が発生した時のイベントリスナを追加
    /// private void onInput(InputEmulatorResults res) {
    ///     Debug.Log("onInput");
    ///     Debug.Log(res.Str);
    /// }
    /// //入力値が変更された時のイベントリスナを追加
    /// private void onChange(InputEmulatorResults res) {
    ///     Debug.Log("onChange");
    /// }
    /// 
    /// input.AddEventListenerOnInput(onInput);
    /// input.AddEventListenerOnChange(onChange);
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
    /// input.EnabledBS = false;
    /// 
    /// //プログラム側から一文字削除
    /// input.BackSpace();
    /// 
    /// //プログラム側から確定
    /// input.Enter();
    /// 
    /// //エンターキーの入力から文字を確定しないようにする
    /// input.EnabledEnter = false;
    /// 
    /// </code></example>
    public class InputEmulator {


        #region 生成
        /// <summaryキーボードの入力から文字列生成をエミュレートします</summary>
        ///<param name="aConvertTableMgr">文字列生成時に使用する、変換テーブルを管理するクラス</param>
        public InputEmulator(in ConvertTableMgr aConvertTableMgr) {
            m_convertTableMgr = aConvertTableMgr;
            Clear();
            m_results = new InputEmulatorResults(in m_params);
        }
        #endregion


        #region メソッド
        /// <summaryキーボードからの入力文字を追加</summary>
        /// <param name="aEvent">入力イベント</param>
        public void AddInput(in Event aEvent) {
            var p = m_params;
            var cvt = m_convertTableMgr;
            if (aEvent.keyCode == KeyCode.None) {//IMEによって、1回のキー入力に対して二回呼び出しが発生する為、更新しない
            }
            else if (aEvent.keyCode == KeyCode.Return) { //enter
                if (EnabledEnter) {
                    p.m_inputType = InputEmulatorResults.INPUT_TYPE.ENTER;
                    p.m_event = new Event(aEvent);
                    EnterInner();
                    m_onInputCallbacks.Invoke(m_results);
                }
            } else if (aEvent.keyCode == KeyCode.Backspace) {//bs
                if (EnabledBS) {
                    p.m_inputType = InputEmulatorResults.INPUT_TYPE.BS;
                    p.m_event = new Event(aEvent);
                    BackSpaceInner();
                    m_onInputCallbacks.Invoke(m_results);
                }
            } else {
                if (IsInputEng) {//英語入力
                    char nCh = cvt.Key2Roma.Convert(aEvent.keyCode, aEvent.shift, aEvent.functionKey);
                    if (nCh == '\0') {return; }
                    p.m_strDone.Add(nCh + "");
                    p.m_strDoneRaws.Add(nCh + "");
                    p.m_prevChar = nCh + "";
                } else if (IsKana) {//かな入力
                    char nCh = cvt.Key2kanaMid.Convert(aEvent.keyCode, aEvent.shift, aEvent.functionKey);
                    if (nCh == '\0') {return; }
                    p.m_strWork += nCh;
                    p.m_prevChar = nCh + "";

                    while (p.m_strWork.Length > 0) {
                        string strCvt = "";
                        if (cvt.KanaMid2Kana.TryConvert(p.m_strWork, out strCvt)) {
                            p.m_strDone.Add(strCvt);
                            p.m_strDoneRaws.Add(p.m_strWork);
                            p.m_strWork = "";
                            break;
                        } else if (cvt.KanaMid2Kana.TryConvert(p.m_strWork, out strCvt, true)) {
                            break;
                        }

                        string addStrTmp = p.m_strWork[0] + "";
                        string addStr;
                        p.m_strWork = p.m_strWork.Substring(1);
                        if (!cvt.NumMarkTable.TryHanToZen(addStrTmp, out addStr)) {
                            addStr = addStrTmp;
                        }
                        p.m_strDone.Add(addStr);
                        p.m_strDoneRaws.Add(addStrTmp);
                    }
                } else {//ローマ字入力
                    char nCh = cvt.Key2Roma.Convert(aEvent.keyCode, aEvent.shift, aEvent.functionKey);
                    if (nCh == '\0') {return; }
                    p.m_strWork += nCh;
                    p.m_prevChar = nCh + "";

                    if (Roma2KanaTable.CanConverFirstN(p.m_strWork)) {
                        p.m_strDone.Add("ん");
                        p.m_strDoneRaws.Add("n");
                        p.m_strWork = p.m_strWork.Substring(1);
                    }

                    while (p.m_strWork.Length > 0) {
                        if (cvt.Roma2Kana.CanConvert(p.m_strWork)) {
                            p.m_strDone.Add(cvt.Roma2Kana.Convert(p.m_strWork));
                            p.m_strDoneRaws.Add(p.m_strWork);
                            p.m_strWork = "";
                            break;
                        } else if (cvt.Roma2Kana.CanConvert(p.m_strWork, true)) {
                            break;
                        }

                        string addStrTmp = p.m_strWork[0] + "";
                        string addStr;
                        p.m_strWork = p.m_strWork.Substring(1);
                        if (!cvt.NumMarkTable.TryHanToZen(addStrTmp, out addStr)) {
                            addStr = addStrTmp;
                        }
                        p.m_strDone.Add(addStr);
                        p.m_strDoneRaws.Add(addStrTmp);
                    }
                }
                p.m_inputType = InputEmulatorResults.INPUT_TYPE.INPUT;
                p.m_event = new Event(aEvent);
                m_results.Dirty = true;
                m_onChangeCallbacks.Invoke(m_results);
                m_onInputCallbacks.Invoke(m_results);
            }
        }

        /// <summary>内部データをクリアします。入力された文字列は全てクリアされます</summary>
        public void Clear() {
            m_params.Clear();
            m_params.m_inputType = InputEmulatorResults.INPUT_TYPE.CLEAR;
            if (m_results != null) {
                m_results.Dirty = true;                
            }
            m_onChangeCallbacks.Invoke(m_results);
        }

        /// <summary>プログラム側から変換確定前の文字列を確定します。</summary>
        public void Enter() {
            m_params.m_event = new Event();
            m_params.m_inputType = InputEmulatorResults.INPUT_TYPE.ENTER_FORCE;
            EnterInner();
        }

        /// <summary>プログラム側から、末尾の1文字消します。</summary>
        public void BackSpace() {
            m_params.m_event  = new Event();
            m_params.m_inputType = InputEmulatorResults.INPUT_TYPE.BS_FORCE;
            BackSpaceInner();
        }
        #endregion
        

        #region フィールド
        /// <summary>生成された文字列</summary>
        public string Str {get { return m_results.Str; }}

        /// <summary生成された、変換される前の文字列</summary>
        public string StrRaw {get { return m_results.StrRaw; }}

        /// <summary>前回入力された文字</summary>
        public string PrevChar {get { return m_results.PrevChar; }}

        /// <summary>前回入力発生時のUnityイベント</summary>
        public Event Event {get { return m_results.Event; }}

        /// <summary>前回の入力タイプ</summary>
        public InputEmulatorResults.INPUT_TYPE InputType {get { return m_params.m_inputType; }}

        private bool m_isInputEng = false;
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
                    Enter();
                }
            }
        }

        private bool m_isKana = false;
        /// <summary>JISかな入力など、日本語を直接入力する方式を使用してエミュレートするかどうか</summary>
        public bool IsKana {
            get { return m_isKana; }
            set {
                m_isKana = value;
                Enter();
            }
        }

        /// <summary>BackSoaceキーを押した時、文字を消すかどうか</summary>
        public bool EnabledBS { get; set; } = true;

        /// <summary>Enterキーを押した時、確定前の文字列を確定するかどうか</summary>
        public bool EnabledEnter { get; set; } = true;
        #endregion


        #region イベントハンドラ
        /// <summary>キーボードから文字が入力された時のイベントリスナを追加します</summary>
        /// <param name="aEvent">イベントリスナ</param>
        public void AddEventListenerOnInput(UnityAction<InputEmulatorResults> aEvent) {m_onInputCallbacks.AddListener(aEvent);}

        /// <summary>キーボードから文字が入力された時のイベントリスナを削除します</summary>
        /// <param name="aEvent">イベントリスナ</param>
        public void RemoveEventListenerOnInput(UnityAction<InputEmulatorResults> aEvent) {m_onInputCallbacks.RemoveListener(aEvent);}

        /// <summary文字列が変更された時のイベントリスナを追加します</summary>
        /// <param name="aEvent">イベントリスナ</param>
        public void AddEventListenerOnChange(UnityAction<InputEmulatorResults> aEvent) {m_onChangeCallbacks.AddListener(aEvent);}

        /// <summary>文字列が変更された時のイベントリスナを削除します</summary>
        /// <param name="aEvent">イベントリスナ</param>

        public void RemoveEventListenerOnChange(UnityAction<InputEmulatorResults> aEvent) {m_onChangeCallbacks.RemoveListener(aEvent);}
        #endregion


        #region 内部メソッド

        /// <summary>変換確定前の文字列を確定します。</summary>
        private void EnterInner() {
            var p = m_params;
            if (p.m_strWork.Length == 0) { return; }
            //もし変換チェック中の文字列がある場合は、そのままの状態で確定
            foreach (char ch in p.m_strWork) {
                p.m_strDone.Add(ch + "");
                p.m_strDoneRaws.Add(ch + "");
            }
            p.m_strWork = "";
            p.m_prevChar = "";
            m_results.Dirty = true;
            m_onChangeCallbacks.Invoke(m_results);
        }

        /// <summary>末尾から1文字消します。</summary>
        private void BackSpaceInner() {
            var p = m_params;
            if (p.m_strWork.Length > 0) {
                p.m_strWork = p.m_strWork.Substring(0, p.m_strWork.Length - 1);
            } else if (p.m_strDone.Count > 0) {
                int idx = p.m_strDone.Count - 1;
                p.m_strDone[idx] = p.m_strDone[idx].Substring(0, p.m_strDone[idx].Length - 1);
                if (p.m_strDone[idx].Length == 0) {
                    p.m_strDone.RemoveAt(idx);
                    p.m_strDoneRaws.RemoveAt(p.m_strDoneRaws.Count - 1);
                }
            }
            p.m_prevChar = "";
            m_results.Dirty = true;
            m_onChangeCallbacks.Invoke(m_results);
        }
        #endregion


        #region メンバ
        private ConvertTableMgr m_convertTableMgr;

        private InputEmulatorParams     m_params = new InputEmulatorParams();
        private InputEmulatorResults    m_results;

        private UnityEvent<InputEmulatorResults> m_onInputCallbacks     = new UnityEvent<InputEmulatorResults>();
        private UnityEvent<InputEmulatorResults> m_onChangeCallbacks    = new UnityEvent<InputEmulatorResults>();
        #endregion
    }
}
