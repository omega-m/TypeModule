using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


/// <summary>
/// InputEmulatorによって処理されたデータへのアクセス用クラスです。
/// </summary>
/// /// <example><code>
///     
/// //イベントリスナを追加し、文字列に変更があった時にテキストを修正
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
///         case InputEmulatorResults.INPUT_TYPE.INPUT_TYPE_INPUT:
///             audioSource.PlayOneShot(typeSound);
///             break;
///         case InputEmulatorResults.INPUT_TYPE.INPUT_TYPE_BS:
///             audioSource.PlayOneShot(bsSound);
///             break;
///         case InputEmulatorResults.INPUT_TYPE.INPUT_TYPE_ENTER:
///             audioSource.PlayOneShot(enterSound);
///             break;
///     }
/// }
/// 
/// //以下オプションです================================
/// 
/// //CapsLockの状態を反映させないように切り替え
/// module.IsCheckCapsLock = false;
/// 
/// //入力を受け付けないように切り替え
/// module.isRun = false;
/// 
/// </code></example>
public class InputEmulatorResults {

    #region 入力タイプ
    /// <summary>
    /// イベント発生時の入力タイプです
    /// </summary>
    public enum INPUT_TYPE {
        /// <summary>
        /// どの入力タイプにも属さない
        /// <summary>
        INPUT_TYPE_NONE,
        /// <summary>
        /// 通常の入力タイプ。キーボードから文字が打たれ、末尾に文字が追加された場合このタイプになります。
        /// <summary>
        INPUT_TYPE_INPUT,
        /// <summary>
        /// キーボードからBSキーが打たれ、1文字削除された時にこのタイプになります。
        /// <summary>
        INPUT_TYPE_BS,
        /// <summary>
        /// キーボードからEnterキーが打たれ、変換中の文字が確定された時にこのタイプになります。
        /// <summary>
        INPUT_TYPE_ENTER,
        /// <summary>
        /// プログラム側か、システム側からBSキーが打たれ、1文字削除された時にこのタイプになります。
        /// <summary>
        INPUT_TYPE_BS_FORCE,
        /// <summary>
        /// プログラム側か、システム側からEnterキーが打たれ、変換中の文字が確定された時にこのタイプになります。
        /// <summary>
        INPUT_TYPE_ENTER_FORCE,
        /// <summary>
        /// システム側から入力値が全て初期化された
        /// <summary>
        INPUT_TYPE_CLEAR,
    }
    #endregion

    #region フィールド
    /// <summary>
    /// 生成された文字列
    /// </summary>
    public string Str {
        get {
            if (Dirty) { CreateChche(); }
            return m_strCache;
        }
    }

    /// <summary>
    /// 生成された、変換される前の文字列
    /// </summary>
    public string StrRaw {
        get {
            if (Dirty) { CreateChche(); }
            return m_strRawCache;
        }
    }

    /// <summary>
    /// 前回入力された文字(変換前)
    /// </summary>
    public string PrevChar {
        get { return m_params.m_prevChar; }
    }

    /// <summary>
    /// 入力発生時のイベント
    /// </summary>
    public Event Event {
        get { return m_params.m_event; }
    }

    /// <summary>
    /// 前回の入力タイプ
    /// </summary>
    public INPUT_TYPE InputType {
        get { return m_params.m_inputType; }
    }
    #endregion

    #region 生成
    /// <summary>
    /// InputEmulatorより作成されます。外からは作成しないでください。
    /// </summary>    
    public InputEmulatorResults(in tpInner.InputEmulatorParams aParams) {
        m_params = aParams;
    }
    #endregion

    #region 内部使用プロパティ
    /// <summary>
    /// 汚しフラグです。trueになっている時にStrやStrRawにアクセスされた場合、返却文字列を再度作成します。
    /// </summary>
    public bool Dirty { get; set; } = true;
    #endregion

    #region 内部メソッド
    /// <summary>
    /// StrとStrRawの返却パラメータを再度生成します。
    /// </summary>
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

    private string m_strCache = "";
    private string m_strRawCache = "";
    #endregion
}


namespace tpInner {

    /// <summary>
    /// InputEmulatorの内部パラメータクラス
    /// InputEmulatorResultsの参照渡しに使用。
    /// </summary>
    public class InputEmulatorParams {

        #region メソッド
        /// <summary>
        /// <para>内部データをクリアします。</para>
        /// </summary>
        public void Clear() {
            m_strDone.Clear();
            m_strDoneRaws.Clear();
            m_strWork = "";
            m_prevChar = "";
            m_inputType = InputEmulatorResults.INPUT_TYPE.INPUT_TYPE_NONE;
            m_event = new Event();
        }
        #endregion

        #region メンバ
        public List<string> m_strDone = new List<string>();         //変換確定済みの文字列
        public List<string> m_strDoneRaws = new List<string>();     //変換確定前文字列
        public string m_strWork = "";                               //現在チェック中の文字
        public string m_prevChar = "";
        public InputEmulatorResults.INPUT_TYPE m_inputType;
        public Event m_event;
        #endregion
    }

    /// <summary>
    /// キーボードの入力から文字列生成をエミュレートします
    /// </summary>
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
    /// input.IsBS = false;
    /// 
    /// //プログラム側から一文字削除
    /// input.BackSpace();
    /// 
    /// //プログラム側から確定
    /// input.Enter();
    /// 
    /// //エンターキーの入力から文字を確定しないようにする
    /// input.IsEnter = false;
    /// 
    /// </code></example>
    public class InputEmulator {

        #region 生成
        /// <summary>
        /// キーボードの入力から文字列生成をエミュレートします
        /// </summary>
        ///<param name="aConvertTableMgr">文字列生成時に使用する、変換テーブルを管理するクラス</param>
        public InputEmulator(in ConvertTableMgr aConvertTableMgr) {
            m_convertTableMgr = aConvertTableMgr;
            Clear();
            m_results = new InputEmulatorResults(in m_params);
        }
        #endregion

        #region メソッド
        /// <summary>
        /// キーボードからの入力文字を追加
        /// </summary>
        /// <param name="aEvent">入力イベント</param>
        public void AddInput(in Event aEvent) {
            m_params.m_event = new Event();
            if (aEvent.keyCode == KeyCode.None) {//IMEによって、1回のキー入力に対して二回呼び出しが発生する為、更新しない
            }
            else if (aEvent.keyCode == KeyCode.Return) { //enter
                if (IsEnter) {
                    m_params.m_inputType = InputEmulatorResults.INPUT_TYPE.INPUT_TYPE_ENTER;
                    m_params.m_event = new Event(aEvent);
                    EnterInner();
                    m_onInputCallbacks.Invoke(m_results);
                }
            } else if (aEvent.keyCode == KeyCode.Backspace) {//bs
                if (IsBS) {
                    m_params.m_inputType = InputEmulatorResults.INPUT_TYPE.INPUT_TYPE_BS;
                    m_params.m_event = new Event(aEvent);
                    BackSpaceInner();
                    m_onInputCallbacks.Invoke(m_results);
                }
            } else {
                if (IsInputEng) {//英語入力
                    char nCh = m_convertTableMgr.Key2Roma.Convert(aEvent.keyCode, aEvent.shift, aEvent.functionKey);
                    if (nCh == '\0') { m_params.m_prevChar = ""; return; }
                    m_params.m_strDone.Add(nCh + "");
                    m_params.m_strDoneRaws.Add(nCh + "");
                    m_params.m_prevChar = nCh + "";
                } else if (IsKana) {//かな入力
                    char nCh = m_convertTableMgr.Key2kanaMid.Convert(aEvent.keyCode, aEvent.shift, aEvent.functionKey);
                    if (nCh == '\0') { m_params.m_prevChar = ""; return; }
                    m_params.m_strWork += nCh;
                    m_params.m_prevChar = nCh + "";

                    while (m_params.m_strWork.Length > 0) {
                        if (m_convertTableMgr.KanaMid2Kana.CanConvert(m_params.m_strWork)) {
                            m_params.m_strDone.Add(m_convertTableMgr.KanaMid2Kana.Convert(m_params.m_strWork));
                            m_params.m_strDoneRaws.Add(m_params.m_strWork);
                            m_params.m_strWork = "";
                            break;
                        } else if (m_convertTableMgr.KanaMid2Kana.CanConvert(m_params.m_strWork, true)) {
                            break;
                        }

                        string addStrTmp = m_params.m_strWork[0] + "";
                        string addStr;
                        m_params.m_strWork = m_params.m_strWork.Substring(1);
                        if (!m_convertTableMgr.NumMarkTable.TryHanToZen(addStrTmp, out addStr)) {
                            addStr = addStrTmp;
                        }
                        m_params.m_strDone.Add(addStr);
                        m_params.m_strDoneRaws.Add(addStrTmp);
                    }
                } else {//ローマ字入力
                    char nCh = m_convertTableMgr.Key2Roma.Convert(aEvent.keyCode, aEvent.shift, aEvent.functionKey);
                    if (nCh == '\0') { m_params.m_prevChar = ""; return; }
                    m_params.m_strWork += nCh;
                    m_params.m_prevChar = nCh + "";

                    if (Roma2KanaTable.CanConverFirstN(m_params.m_strWork)) {
                        m_params.m_strDone.Add("ん");
                        m_params.m_strDoneRaws.Add("n");
                        m_params.m_strWork = m_params.m_strWork.Substring(1);
                    }

                    while (m_params.m_strWork.Length > 0) {
                        if (m_convertTableMgr.Roma2Kana.CanConvert(m_params.m_strWork)) {
                            m_params.m_strDone.Add(m_convertTableMgr.Roma2Kana.Convert(m_params.m_strWork));
                            m_params.m_strDoneRaws.Add(m_params.m_strWork);
                            m_params.m_strWork = "";
                            break;
                        } else if (m_convertTableMgr.Roma2Kana.CanConvert(m_params.m_strWork, true)) {
                            break;
                        }

                        string addStrTmp = m_params.m_strWork[0] + "";
                        string addStr;
                        m_params.m_strWork = m_params.m_strWork.Substring(1);
                        if (!m_convertTableMgr.NumMarkTable.TryHanToZen(addStrTmp, out addStr)) {
                            addStr = addStrTmp;
                        }
                        m_params.m_strDone.Add(addStr);
                        m_params.m_strDoneRaws.Add(addStrTmp);
                    }
                }
                m_params.m_inputType = InputEmulatorResults.INPUT_TYPE.INPUT_TYPE_INPUT;
                m_params.m_event = new Event(aEvent);
                m_results.Dirty = true;
                m_onChangeCallbacks.Invoke(m_results);
                m_onInputCallbacks.Invoke(m_results);
            }
        }

        /// <summary>
        /// <para>内部データをクリアします。</para>
        /// <para>入力された文字列は全てクリアされます</para>
        /// </summary>
        public void Clear() {
            m_params.Clear();
            m_params.m_inputType = InputEmulatorResults.INPUT_TYPE.INPUT_TYPE_CLEAR;
            if (m_results != null) {
                m_results.Dirty = true;                
            }
            m_onChangeCallbacks.Invoke(m_results);
        }

        /// <summary>
        /// プログラム側から変換確定前の文字列を確定します。
        /// </summary>
        public void Enter() {
            m_params.m_event = new Event();
            m_params.m_inputType = InputEmulatorResults.INPUT_TYPE.INPUT_TYPE_ENTER_FORCE;
            EnterInner();
        }

        /// <summary>
        /// プログラム側から、末尾の1文字消します。
        /// </summary>
        public void BackSpace() {
            m_params.m_event  = new Event();
            m_params.m_inputType = InputEmulatorResults.INPUT_TYPE.INPUT_TYPE_BS_FORCE;
            BackSpaceInner();
        }
        #endregion
        
        #region フィールド
        /// <summary>
        /// 生成された文字列
        /// </summary>
        public string Str {
            get { return m_results.Str; }
        }

        /// <summary>
        /// 生成された、変換される前の文字列
        /// </summary>
        public string StrRaw {
            get { return m_results.StrRaw; }
        }

        /// <summary>
        /// 前回入力された文字
        /// </summary>
        public string PrevChar {
            get { return m_results.PrevChar; }
        }

        /// <summary>
        /// 前回入力発生時のUnityイベント
        /// </summary>
        public Event Event {
            get { return m_results.Event; }
        }

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
        /// <summary>
        /// JISかな入力など、日本語を直接入力する方式を使用してエミュレートするかどうか
        /// </summary>
        public bool IsKana {
            get { return m_isKana; }
            set {
                m_isKana = value;
                Enter();
            }
        }

        /// <summary>
        /// <para>BackSoaceキーを押した時、文字を消すかどうか</para>
        /// </summary>
        public bool IsBS { get; set; } = true;

        /// <summary>
        /// <para>Enterキーを押した時、確定前の文字列を確定するかどうか</para>
        /// </summary>
        public bool IsEnter { get; set; } = true;
        #endregion

        #region イベントハンドラ
        /// <summary>
        /// キーボードから文字が入力された時のイベントリスナを追加します
        /// </summary>
        /// <param name="aEvent">イベントリスナ</param>
        public void AddEventListenerOnInput(UnityAction<InputEmulatorResults> aEvent) {
            m_onInputCallbacks.AddListener(aEvent);
        }

        /// <summary>
        /// キーボードから文字が入力された時のイベントリスナを削除します
        /// </summary>
        /// <param name="aEvent">イベントリスナ</param>
        public void RemoveEventListenerOnInput(UnityAction<InputEmulatorResults> aEvent) {
            m_onInputCallbacks.RemoveListener(aEvent);
        }

        /// <summary>
        /// 文字列が変更された時のイベントリスナを追加します
        /// </summary>
        /// <param name="aEvent">イベントリスナ</param>
        public void AddEventListenerOnChange(UnityAction<InputEmulatorResults> aEvent) {
            m_onChangeCallbacks.AddListener(aEvent);
        }

        /// <summary>
        /// 文字列が変更された時のイベントリスナを削除します
        /// </summary>
        /// <param name="aEvent">イベントリスナ</param>

        public void RemoveEventListenerOnChange(UnityAction<InputEmulatorResults> aEvent) {
            m_onChangeCallbacks.RemoveListener(aEvent);
        }
        #endregion

        #region 内部メソッド

        /// <summary>
        /// 変換確定前の文字列を確定します。
        /// </summary>
        private void EnterInner() {
            if (m_params.m_strWork.Length == 0) { return; }
            //もし変換チェック中の文字列がある場合は、そのままの状態で確定
            foreach (char ch in m_params.m_strWork) {
                m_params.m_strDone.Add(ch + "");
                m_params.m_strDoneRaws.Add(ch + "");
            }
            m_params.m_strWork = "";
            m_params.m_prevChar = "";
            m_results.Dirty = true;
            m_onChangeCallbacks.Invoke(m_results);
        }

        /// <summary>
        /// 末尾から1文字消します。
        /// </summary>
        private void BackSpaceInner() {
            if (m_params.m_strWork.Length > 0) {
                m_params.m_strWork = m_params.m_strWork.Substring(0, m_params.m_strWork.Length - 1);
            } else if (m_params.m_strDone.Count > 0) {
                int idx = m_params.m_strDone.Count - 1;
                m_params.m_strDone[idx] = m_params.m_strDone[idx].Substring(0, m_params.m_strDone[idx].Length - 1);
                if (m_params.m_strDone[idx].Length == 0) {
                    m_params.m_strDone.RemoveAt(idx);
                    m_params.m_strDoneRaws.RemoveAt(m_params.m_strDoneRaws.Count - 1);
                }
            }
            m_params.m_prevChar = "";
            m_results.Dirty = true;
            m_onChangeCallbacks.Invoke(m_results);
        }
        #endregion

        #region メンバ
        private ConvertTableMgr m_convertTableMgr;

        private InputEmulatorParams     m_params = new InputEmulatorParams();
        private InputEmulatorResults    m_results;

        private UnityEvent<InputEmulatorResults> m_onInputCallbacks = new UnityEvent<InputEmulatorResults>();
        private UnityEvent<InputEmulatorResults> m_onChangeCallbacks = new UnityEvent<InputEmulatorResults>();
        #endregion
    }
}
