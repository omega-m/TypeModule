using UnityEngine;
using Candlelight;
using UnityEngine.Events;
using tm.inner;

namespace tm {

    ///<summary>
    ///<para>キーボードの入力を元に、文字入力の判定を担当するモジュールです。</para>
    ///<para>CPU処理負荷や、メモリ要領的な問題で、特に理由がない場合は、マネジメントクラスで管理する方が良さそうです。</para>
    /// </summary>
    /// <example><code>
    /// using tm;
    ///     ...
    ///     
    /// TypeModule module = GetComponent&lt;TypeModule&gt;();
    ///     
    /// 
    /// 
    /// //=======================================================================
    /// //文字列生成シミュレーションモードに変更
    /// module.Mode = TypeModule.MODE.INPUT;
    /// 
    /// //モジュールから状態を取得
    /// Debug.Log(module.Str);
    /// Debug.Log(module.PrevChar);
    /// Debug.Log(module.StrRaw);
    /// 
    /// //モードを変更
    /// module.IsInputEng = true    //英語入力状態へ
    /// module.IsKana     = true;     //かな入力入力状態へ
    /// module.EnabledBS  = true;     //BSで文字を消せるかどうか
    /// 
    /// //プログラムから文字列を操作
    /// module.Enter();             //変換確定前の文字列を確定
    /// module.BackSpace();         //末尾から1文字削除
    /// module.Clear();             //全ての文字を削除
    /// module.AddInput(KeyCode.A); //Aキーが押されたとして処理
    /// 
    /// 
    /// //イベントリスナを追加し、文字列に変更があった時にGUIテキストを修正
    /// module.AddEventListenerOnChange(onChange);
    /// 
    ///         ...
    /// 
    /// public Text testInput;
    /// public Text testInputRaw;
    /// 
    /// private void onChange(InputEmulatorResults res) {
    ///     Debug.Log("onChange");
    ///     
    ///     testInput.text = res.StrInput;
    ///     testInputRaw.text = res.StrRawInput;
    /// }
    /// 
    /// 
    /// 
    /// //イベントリスナを追加し、文字が打たれた時にサウンドを再生
    /// public AudioSource audioSource;
    /// public AudioClip typeSound;
    /// public AudioClip bsSound;
    /// public AudioClip enterSound;
    /// 
    ///         ...
    ///
    /// 
    /// module.AddEventListenerOnInput(onInput);
    /// audioSource = GetComponent&lt;AudioSource&gt;();
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
    ///     }C:\Users\renge\folder\circle\workspace\tp\UnityWorks\TypeModule\Assembly-CSharp.csproj
    /// }
    /// 
    /// 
    /// 
    /// //=======================================================================
    /// //指定された文字列が正しく打ててるか、比較するモードに変更
    /// module.Mode = TypeModule.MODE.COPY;
    /// 
    /// //モードの変更
    /// module.IsKana = true;                  /かな入力入力状態へ
    /// module.IsCaseSensitive = true;         //英語の大文字と小文字入力を区別
    /// 
    /// 
    /// //比較対象の文字列をセット(内部初期化もされます)
    /// module.TargetStr = "こちらは、たいぴんぐするぶんしょうです。";
    /// 
    /// 
    /// //直前に入力された文字を取得
    /// Debug.Log(module.PrevCorrectChar);         //正しく入力された場合
    /// Debug.Log(module.PrevMissChar);            //ミス入力の場合
    /// 
    /// //パラメータにアクセス
    /// Debug.Log(module.CorrectNum);              //正しくタイプした数
    /// Debug.Log(module.CorrectCharNum);          //正しく打てた文字数
    /// Debug.Log(module.MissNum);                 //ミスタイプした数
    /// Debug.Log(module.IsComplete);              //指定文字列を打ち切ったか
    ///      
    /// //プログラム側から、入力処理をエミュレート
    /// module.Correct();               //正しく一度入力したとして処理
    /// module.Miss();                  //ミスタイプしたとして処理
    /// module.AddInput(KeyCode.A);     //Aキーが押されたとして処理
    /// 
    /// 
    /// 
    /// //イベントリスナを追加し、文字列に変更があった時にGUIテキストを修正
    /// module.AddEventListenerOnInput(onInput);
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
    /// 
    /// 
    /// 
    /// //=======================================================================
    /// //以下オプション
    /// 
    /// //CapsLockの状態を反映させないように切り替え
    /// module.EnabledCapsLock = false;
    /// 
    /// //入力を受け付けないように切り替え
    /// module.isRun = false;
    /// 
    /// </code></example>
    public class TypeModule : MonoBehaviour {


        #region 入力判定モード
        ///<summary>文字入力判定モードです。</summary>
        public enum MODE {
            ///<summary>
            ///<para>キーボードの入力を元に、文字列の生成をエミュレートします。</para>
            ///<para>キーボード入力から文字列を取得したい場合に使用してください。</para>
            ///</summary>
            INPUT,
            ///<summary>
            ///<para>【TargetStr】で指定された文字列が正しく打ててるかを確認します。</para>
            ///<para>タイピングゲームで、お題の文字を真似して打たせる時に使用してください。</para>
            ///</summary>
            COPY,
        }
        #endregion


        #region 共通プロパティ
        /// <summary>前回入力発生時のUnityイベント</summary>
        public Event Event {
            get {
                switch (Mode) {
                    case MODE.INPUT:    return m_inputEmulator.Event;
                    case MODE.COPY:     return m_copyInputChecker.Event;
                }
                return new Event();
            }
        }

        /// <summary>
        /// 前回入力された文字(変換前)
        /// </summary>
        public string PrevChar { 
            get {
                switch (Mode) {
                    case MODE.INPUT:    return m_inputEmulator.PrevChar;
                    case MODE.COPY:     return m_copyInputChecker.PrevChar; 
                }
                return "";
            } 
        }
        #endregion


        #region 共通メソッド
        /// <summary>内部の入力データをを全て削除します</summary>
        public void Clear() {
            m_inputEmulator.Clear();
            m_copyInputChecker.Clear();
        }

        /// <summary>
        /// <para>プログラムから、キー入力を擬似的に追加</para>
        /// <para>【MODE.COPYの場合】もし、TargetStrに文字列がセットされていない場合や、既に打ち切っている状態の場合は何もしません。</para>
        /// </summary>
        /// <param name="aKeyCode">キーコード</param>
        /// <param name="aIsShift">SHIFT中か</param>
        /// <param name="aIsFunction">FUNCTION中か(2箇所ある\の判定に必須。通常はfalseとしてください)</param>
        public void AddInput(KeyCode aKeyCode, bool aIsShift = false, bool aIsFunction = false) {
            switch (Mode) {
                 case MODE.INPUT:   m_inputEmulator.AddInput(aKeyCode, aIsShift, aIsFunction, true); break;
                case MODE.COPY:     m_copyInputChecker.AddInput(aKeyCode, aIsShift, aIsFunction, true); break;
            }
        }
        #endregion


        #region 【MODE.INPUT】プロパティ
        /// <summary>
        /// <para>【MODE.INPUT】生成された文字列</para>
        /// <para>StrDoneと同等の値を返却します。</para>
        /// <summary></summary>
        public string StrInput {
            get {
                switch (Mode) {
                    case MODE.INPUT:    return m_inputEmulator.Str;
                    case MODE.COPY:     return m_copyInputChecker.StrDone;
                }
                return "";
            }
        }

        /// <summary>
        /// <para>【MODE.INPUT】生成された、変換される前の文字列</para>
        /// <para>StrDoneRawと同等の値を返却します。</para>
        /// </summary>
        public string StrRawInput {
            get {
                switch (Mode) {
                    case MODE.INPUT:    return m_inputEmulator.StrRaw;
                    case MODE.COPY:     return m_copyInputChecker.StrDoneRaw;
                }
                return "";
            }
        }

        /// <summary>【MODE.INPUT】前回の入力タイプ</summary>
        public InputEmulatorResults.INPUT_TYPE InputType {get { return m_inputEmulator.InputType; }}
        #endregion


        #region 【MODE.INPUT】メソッド
        /// <summary>【MODE.INPUT】プログラム側から、変換確定前の文字列を確定します。</summary>
        public void Enter() {if (Mode == MODE.INPUT) {m_inputEmulator.Enter();}}

        /// <summary>【MODE.INPUT】プログラム側から、末尾の1文字消します。</summary>
        public void BackSpace() {if (Mode == MODE.INPUT) {m_inputEmulator.BackSpace();}}

        /// <summary>【MODE.INPUT】キーボードから文字が入力された時のイベントリスナを追加します</summary>
        /// <param name="aEvent">イベントリスナ</param>
        public void AddEventListenerOnInput(UnityAction<InputEmulatorResults> aEvent) {m_inputEmulator.AddEventListenerOnInput(aEvent);}

        /// <summary>【MODE.INPUT】キーボードから文字が入力された時のイベントリスナを削除します</summary>
        /// <param name="aEvent">イベントリスナ</param>
        public void RemoveEventListenerOnInput(UnityAction<InputEmulatorResults> aEvent) {m_inputEmulator.RemoveEventListenerOnInput(aEvent);}

        /// <summary>【MODE.INPUT】文字列が変更された時のイベントリスナを追加します</summary>
        /// <param name="aEvent">イベントリスナ</param>
        public void AddEventListenerOnChange(UnityAction<InputEmulatorResults> aEvent) {m_inputEmulator.AddEventListenerOnChange(aEvent);}

        /// <summary>【MODE.INPUT】文字列が変更された時のイベントリスナを削除します</summary>
        /// <param name="aEvent">イベントリスナ</param>
        public void RemoveEventListenerOnChange(UnityAction<InputEmulatorResults> aEvent) {m_inputEmulator.RemoveEventListenerOnChange(aEvent); }
        #endregion


        #region 【MODE.COPY】プロパティ
        /// <summary>
        /// <para>【MODE.COPY】既に打ち終わった文字列</para>
        /// <para>StrInputと同等の値を返却します。</para>
        /// </summary>
        public string StrDone {get { return StrInput; }}

        /// <summary>【MODE.COPY】現在打っている文字</summary>
        public string StrCurrent {get { return m_copyInputChecker.StrCurrent; }}

        /// <summary>【MODE.COPY】まだ打っていない文字列</summary>
        public string StrYet {get { return m_copyInputChecker.StrYet; }}

        /// <summary>
        /// <para>【MODE.COPY】既に打ち終わった文字列(変換前)</para>
        /// <para>StrRawInputと同等の値を返却します。</para>
        /// </summary>
        public string StrDoneRaw {get { return StrRawInput; }}

        /// <summary>【MODE.COPY】 現在打っている文字(変換前)</summary>
        public string StrCurrentRaw {get { return m_copyInputChecker.StrCurrentRaw; }}

        /// <summary>【MODE.COPY】既に打ち終わった文字列(変換前)</summary>
        public string StrYetRaw {get { return m_copyInputChecker.StrYetRaw; }}

        /// <summary>
        /// <para>【MODE.INPUT】前回打たれた文字</para>
        /// <para>【MODE.COPY】前回正しく入力された文字(ミスした時は空文字列)</para>
        /// </summary>
        public string PrevCorrectChar {
            get {
                switch (Mode) {
                    case MODE.INPUT:    return m_inputEmulator.PrevChar; 
                    case MODE.COPY:     return m_copyInputChecker.PrevCorrectChar; 
                }
                return "";
            }
        }

        /// <summary>
        /// <para>【MODE.INPUT】前回打たれた文字</para>
        /// <para>【MODE.COPY】前回ミスしたされた文字(正しく入力された時は空文字列)</para>
        /// </summary>
        public string PrevMissChar {
            get {
                switch (Mode) {
                    case MODE.INPUT: return m_inputEmulator.PrevChar;
                    case MODE.COPY: return m_copyInputChecker.PrevMissChar;
                }
                return "";
            }
        }

        /// <summary>【MODE.COPY】正しくタイプした数</summary>
        public int CorrectNum {get { return m_copyInputChecker.CorrectNum; }}

        /// <summary>【MODE.COPY】正しく打てた文字数</summary>
        public int CorrectCharNum {get { return m_copyInputChecker.CorrectCharNum; }}

        /// <summary>【MODE.COPY】ミスタイプした数</summary>
        public int MissNum {get {return m_copyInputChecker.MissNum;}}

        /// <summary>打ち終わったか</summary>
        public bool IsComplete {get { return m_copyInputChecker.IsComplete; }}

        /// <summary>【MODE.COPY】前回の入力タイプ</summary>
        public CopyInputCheckerResults.INNER_EVENT_TYPE InnerEvent {get { return m_copyInputChecker.InnerEvent; }}
        #endregion


        #region 【MODE.COPY】メソッド
        /// <summary>
        /// <para>【MODE.COPY】プログラムから、正しい入力を一回行ったとして処理します。</para>
        /// <para>もし、TargetStrに文字列がセットされていない場合や、既に打ち切っている状態の場合は何もしません。</para>
        /// </summary>
        /// <param name="isThrowEvent">true:処理後、登録されたイベントリスナにイベントを投げます</param>
        public void Correct(bool isThrowEvent = true) {m_copyInputChecker.Correct(isThrowEvent);}

        /// <summary>
        /// <para>【MODE.COPY】プログラムから、ミスタイプを一回行ったとして処理します。</para>
        /// <para>もし、TargetStrに文字列がセットされていない場合や、既に打ち切っている状態の場合は何もしません。</para>
        /// </summary>
        /// <param name="isThrowEvent">true:処理後、登録されたイベントリスナにイベントを投げます</param>
        public void Miss(bool isThrowEvent = true) {m_copyInputChecker.Miss(isThrowEvent);}

        /// <summary>【MODE.COPY】キーボードからの入力処理を行った時のイベントリスナを追加します。</summary>
        /// <param name="aEvent">イベントリスナ</param>
        public void AddEventListenerOnInput(UnityAction<CopyInputCheckerResults> aEvent) {m_copyInputChecker.AddEventListenerOnInput(aEvent);}

        /// <summary>【MODE.COPY】キーボードからの入力処理を行った時のイベントリスナを削除します。</summary>
        /// <param name="aEvent">イベントリスナ</param>
        public void RemoveEventListenerOnInput(UnityAction<CopyInputCheckerResults> aEvent) {m_copyInputChecker.RemoveEventListenerOnInput(aEvent);}

        /// <summary>【MODE.COPY】比較対象の文字に対して、正しく入力された時のイベントリスナを追加します。</summary>
        /// <param name="aEvent">イベントリスナ</param>
        public void AddEventListenerOnCorrect(UnityAction<CopyInputCheckerResults> aEvent) {m_copyInputChecker.AddEventListenerOnCorrect(aEvent);}

        /// <summary>【MODE.COPY】比較対象の文字に対して、正しく入力された時のイベントリスナを削除します。</summary>
        /// <param name="aEvent">イベントリスナ</param>
        public void RemoveEventListenerOnCorrect(UnityAction<CopyInputCheckerResults> aEvent) {m_copyInputChecker.RemoveEventListenerOnCorrect(aEvent);}

        /// <summary>【MODE.COPY】比較対象の文字に対して、ミスタッチした時のイベントリスナを追加します。</summary>
        /// <param name="aEvent">イベントリスナ</param>
        public void AddEventListenerOnMiss(UnityAction<CopyInputCheckerResults> aEvent) {m_copyInputChecker.AddEventListenerOnMiss(aEvent);}

        /// <summary>【MODE.COPY】比較対象の文字に対して、ミスタッチした時のイベントリスナを削除します。</summary>
        /// <param name="aEvent">イベントリスナ</param>
        public void RemoveEventListenerOnMiss(UnityAction<CopyInputCheckerResults> aEvent) {m_copyInputChecker.RemoveEventListenerOnMiss(aEvent);}

        /// <summary>【MODE.COPY】比較対象の文字を全て打ちきった時のイベントリスナを追加します。</summary>
        /// <param name="aEvent">イベントリスナ</param>
        public void AddEventListenerOnComplete(UnityAction<CopyInputCheckerResults> aEvent) {m_copyInputChecker.AddEventListenerOnComplete(aEvent);}

        /// <summary>【MODE.COPY】比較対象の文字を全て打ちきった時のイベントリスナを削除します。</summary>
        /// <param name="aEvent">イベントリスナ</param>
        public void RemoveEventListenerOnComplete(UnityAction<CopyInputCheckerResults> aEvent) {m_copyInputChecker.RemoveEventListenerOnComplete(aEvent);}

        /// <summary>【MODE.COPY】比較文字がセットされ、セットアップが完了した時のイベントリスナを追加します。</summary>
        /// <param name="aEvent">イベントリスナ</param>
        public void AddEventListenerOnSetup(UnityAction<CopyInputCheckerResults> aEvent) {m_copyInputChecker.AddEventListenerOnSetup(aEvent);}

        /// <summary>【MODE.COPY】比較文字がセットされ、セットアップが完了した時のイベントリスナを削除します。</summary>
        /// <param name="aEvent">イベントリスナ</param>
        public void RemoveEventListenerOnSetup(UnityAction<CopyInputCheckerResults> aEvent) {m_copyInputChecker.AddEventListenerOnSetup(aEvent);}
        #endregion


        #region インスペクタープロパティ
        #region
        [Tooltip("文字入力判定モードです。\n" +
            "【INPUT】   :キーボードの入力を元に、文字列の生成をエミュレートします。\n" +
            "【MODE_COMPARE】 :指定された文字列が正しく打ててるかを確認します。タイピングゲームで、お題の文字を真似して打たせる時に使用してください。")]
        [SerializeField, PropertyBackingField("Mode")] private MODE m_mode = MODE.INPUT;
        #endregion
        /// <summary>
        /// <para>文字入力判定モードです。</para>
        /// <para>【INPUT】   :キーボードの入力を元に、文字列の生成をエミュレートします。</para>
        /// <para>【MODE_COMPARE】 :指定された文字列が正しく打ててるかを確認します。タイピングゲームで、お題の文字を真似して打たせる時に使用してください。</para>
        /// </summary>
        public MODE Mode {
            get { return m_mode; }
            set { m_mode = value; }
        }

        #region
        [Tooltip("現在入力を受け付けているか\n" +
           "[false]にした場合、入力チェックを行わなくなります。ポーズ時やゲーム外の時に使用してください。")]
        [SerializeField, PropertyBackingField("IsRun")] private bool m_isRun = true;
        #endregion
        /// <summary>
        /// <para>現在入力を受け付けているか</para>
        /// <para>[false]にした場合、入力チェックを行わなくなります。ポーズ時やゲーム外の時に使用してください。</para>
        /// </summary>
        public bool IsRun {
            get { return m_isRun; }
            set { m_isRun = value; }
        }

        #region
        [Tooltip("JISかな入力など、日本語を直接入力する方式を使用してエミュレートするかどうかのフラグです。\n" +
            "[INPUT]trueの場合、[m_keyCodeToKanaCsv]から文字列生成をエミュレートします。\n" +
            "[MODE_COMPARE]trueの場合、日本語と比較する場合は[m_keyCodeToKanaCsv]から、そうでない場合は[m_keyCode2RomaCsv]から文字列生成をエミュレートします。")]
        [SerializeField, PropertyBackingField("IsKana")] private bool m_isKana = false;
        #endregion
        /// <summary>
        /// <para>JISかな入力など、日本語を直接入力する方式を使用してエミュレートするかどうかのフラグです。</para>
        /// <para>[INPUT]trueの場合、[m_keyCodeToKanaCsv]から文字列生成をエミュレートします。</para>
        /// <para>[MODE_COMPARE]trueの場合、日本語と比較する場合は[m_keyCodeToKanaCsv]から、そうでない場合は[m_keyCode2RomaCsv]から文字列生成をエミュレートします。</para>
        /// </summary>
        public bool IsKana {
            get { return m_isKana; }
            set {
                m_isKana = value;
                if (m_inputEmulator != null) {m_inputEmulator.IsKana = IsKana;}
                if(m_copyInputChecker != null) {m_copyInputChecker.IsKana = IsKana;}
            }
        }

        [Header("Modeが【INPUT】の時の設定")]
        #region
        [Tooltip("入力モード\n" +
            "[true]英語入力モード\n" +
            "[false]日本語入力モード")]
        [SerializeField, PropertyBackingField("IsInputEng")] private bool m_isInputEng = false;
        #endregion
        /// <summary>
        /// <para>入力モード</para>
        /// <para>[true]英語入力モード</para>
        /// <para>[false]日本語入力モード</para>
        /// </summary>
        public bool IsInputEng {
            get { return m_isInputEng; }
            set {
                m_isInputEng = value;
                if (m_inputEmulator != null) {m_inputEmulator.IsInputEng = IsInputEng;}
            }
        }

        #region
        [Tooltip("BackSoaceキーを押した時、文字を消すかどうか")]
        [SerializeField, PropertyBackingField("EnabledBS")] private bool m_enabledBS = true;
        #endregion
        /// <summary>BackSoaceキーを押した時、文字を消すかどうか</summary>
        public bool EnabledBS {
            get { return m_enabledBS; }
            set {
                m_enabledBS = value;
                if (m_inputEmulator != null) {m_inputEmulator.EnabledBS = EnabledBS; }
            }
        }

        #region
        [Tooltip("Enterキーを押した時、確定前の文字列を確定するかどうか")]
        [SerializeField, PropertyBackingField("EnabledEnter")] private bool m_enabledEnter = true;
        #endregion
        /// <summary>
        /// <para>Enterキーを押した時、確定前の文字列を確定するかどうか</para>
        /// </summary>
        public bool EnabledEnter {
            get { return m_enabledEnter; }
            set {
                m_enabledEnter = value;
                if (m_inputEmulator != null) {m_inputEmulator.EnabledEnter = EnabledEnter; }
            }
        }

        [Header("Modeが【COPY】の時の設定")]
        #region
        [Tooltip("比較対象の文字列(タイピングのお台文)\n" +
            "値を変更した時点で、初期化処理を自動で呼び出します。\n" +
            "また、全角半角カタカナはひらがなに、全角アルファベットは半角アルファベットに変換されます。\n" +
            "[含む事が出来る文字]: -_| -_に対応する大文字|ァ-ヴ|ぁ-ゔ\n" +
            "含むことのできない文字があった場合、エディタ上での実行なら例外を投げます。ビルド版だとその文字の部分だけ削除され、動き続けます。\n")]
        [SerializeField, PropertyBackingField("TargetStr")] private string m_targetStr = "";
        #endregion
        /// <summary>
        /// <para>比較対象の文字列(タイピングのお台文)</para>
        /// <para>値を変更した時点で、初期化処理を自動で呼び出します。</para>
        /// <para>また、全角半角カタカナはひらがなに、全角アルファベットは半角アルファベットに変換されます。</para>
        /// <para>[含む事が出来る文字]: -_| -_に対応する大文字|ァ-ヴ|ぁ-ゔ</para>
        /// <para>含むことのできない文字があった場合、エディタ上での実行なら例外を投げます。ビルド版だとその文字の部分だけ削除され、動き続けます。</para>
        /// </summary>
        public string TargetStr {
            get { return m_targetStr; }
            set {
                m_targetStr = value;
                if (m_copyInputChecker != null) {
                    m_copyInputChecker.TargetStr = value;
                }
            }
        }

        #region
        [Tooltip("英語の大文字と小文字入力を区別して判定するか\n" +
            "初期化処理が発生した時点で反映されます。")]
        [SerializeField, PropertyBackingField("IsCaseSensitive")] private bool m_isCaseSensitive = false;
        #endregion
        /// <summary>
        /// <para>英語の大文字と小文字入力を区別して判定するか</para>
        /// <para>初期化処理が発生した時点で反映されます。</para>
        /// </summary>
        public bool IsCaseSensitive {
            get { return m_isCaseSensitive; }
            set {
                m_isCaseSensitive = value;
                if (m_copyInputChecker != null) {m_copyInputChecker.IsCaseSensitive = value;}
            }
        }

        #region 詳細設定
        [Header("以下詳細設定 (指定しなくても動きます)")]
        #region
        [Tooltip("キーの入力(KeyCode)からローマ字文字への変換テーブルを定義したファイル\n" +
            "明示的に指定しなかった場合、以下のファイルを読み込みます。\n" +
            "【Assets/Resources/TypeModule/data/KeyCode2Char/qwerty.csv】\n" +
            "独自で指定する場合は、以下のようなCSV(.csv)形式ファイルを用意してください。文字コードは[UTF-8]としてください。\n" +
            "［形式］変換先文字,【UnityEngine.KeyCode】, isShift, isFunction\n" +
            "例) \n" +
            "S,115,1,0\n" +
            "s,115,0,0")]
        [SerializeField, PropertyBackingField("KeyCode2RomaCsv")] private TextAsset m_keyCode2RomaCsv;
        #endregion
        /// <summary>
        /// <para>キーの入力(KeyCode)からローマ字文字への変換テーブルを定義したファイル</para>
        /// <para>明示的に指定しなかった場合、以下のファイルを読み込みます。</para>
        /// <para>【Assets/Resources/TypeModule/data/KeyCode2Char/qwerty.csv】</para>
        /// <para>独自で指定する場合は、以下のようなCSV(.csv)形式ファイルを用意してください。文字コードは[UTF-8]としてください。</para>
        /// <para>［形式］変換先文字,【UnityEngine.KeyCode】, isShift, isFunction</para>
        /// <para>例) \n</para>
        /// <para>S,115,1,0</para>
        /// <para>s,115,0,0</para>
        /// </summary>
        public TextAsset KeyCode2RomaCsv {
            get { return m_keyCode2RomaCsv; }
            set {
                if (m_keyCode2RomaCsv != value) {
                    m_keyCode2RomaCsv = value;
                    if (m_convertTableMgr != null) {
                        m_convertTableMgr.SetKeyCode2RomaTable(in m_keyCode2RomaCsv);
                    }
                }
            }
        }

        #region
        [Tooltip("ローマ字文字列からひらがな文字列への変換テーブルを定義したファイル\n" +
            "明示的に指定しなかった場合、以下のファイルを読み込みます。\n" +
            "【Assets/Resources/TypeModule/data/Char2Kana/roma.csv】\n" +
            "独自で指定する場合は、以下のようなCSV(.csv)形式ファイルを用意してください。文字コードは[UTF-8]としてください。\n" +
            "［形式］ローマ字文字列,ひらがな文字列\n" +
            "例) \n" +
            "a,あ\n" +
            "shi,し")]
        [SerializeField, PropertyBackingField("Roma2KanaCsv")] private TextAsset m_roma2KanaCsv;
        #endregion
        /// <summary>
        /// <para>ローマ字文字列からひらがな文字列への変換テーブルを定義したファイル</para>
        /// <para>明示的に指定しなかった場合、以下のファイルを読み込みます。</para>
        /// <para>【Assets/Resources/TypeModule/data/Char2Kana/roma.csv】</para>
        /// <para>独自で指定する場合は、以下のようなCSV(.csv)形式ファイルを用意してください。文字コードは[UTF-8]としてください。</para>
        /// <para>［形式］ローマ字文字列,ひらがな文字列</para>
        /// <para>例) \n</para>
        /// <para>a,あ</para>
        /// <para>shi,し</para>
        /// </summary>
        public TextAsset Roma2KanaCsv {
            get { return m_roma2KanaCsv; }
            set {
                if (m_roma2KanaCsv != value) {
                    m_roma2KanaCsv = value;
                    if (m_convertTableMgr != null) {m_convertTableMgr.SetRoma2KanaTable(in m_roma2KanaCsv);}
                }
            }
        }

        #region
        [Space(10)]
        [Tooltip("キーの入力(KeyCode)からひらがなの中間文字への変換テーブルを定義したファイル\n" +
            "JISかな入力など、日本語を直接入力する方式を使用する際に参照します。\n" +
            "明示的に指定しなかった場合、以下のファイルを読み込みます。\n" +
            "【Assets/Resources/TypeModule/data/KeyCode2Char/JISkana.csv】\n" +
            "独自で指定する場合は、以下のようなCSV(.csv)形式ファイルを用意してください。文字コードは[UTF-8]としてください。\n" +
            "［形式］変換先文字,【UnityEngine.KeyCode】, isShift, isFunction\n" +
            "例) \n" +
            "ぬ,49,0,0 \n" +
            "ぬ,49,1,0 \n" +
            "ふ,50,0,0")]
        [SerializeField, PropertyBackingField("KeyCode2KanaMidCsv")] private TextAsset m_keyCode2KanaMidCsv;
        #endregion
        /// <summary>
        /// <para>キーの入力(KeyCode)からひらがなの中間文字への変換テーブルを定義したファイル</para>
        /// <para>JISかな入力など、日本語を直接入力する方式を使用する際に参照します。</para>
        /// <para>明示的に指定しなかった場合、以下のファイルを読み込みます。</para>
        /// <para>【Assets/Resources/TypeModule/data/KeyCode2Char/JISkana.csv】</para>
        /// <para>独自で指定する場合は、以下のようなCSV(.csv)形式ファイルを用意してください。文字コードは[UTF-8]としてください。</para>
        /// <para>［形式］変換先文字,【UnityEngine.KeyCode】, isShift, isFunction</para>
        /// <para>例) \n</para>
        /// <para>ぬ,49,0,0</para>
        /// <para>ぬ,49,1,0</para>
        /// <para>ふ,50,0,0</para>
        /// </summary>
        public TextAsset KeyCode2KanaMidCsv {
            get { return m_keyCode2KanaMidCsv; }
            set {
                if (m_keyCode2KanaMidCsv != value) {
                    m_keyCode2KanaMidCsv = value;
                    if (m_convertTableMgr != null) {m_convertTableMgr.SetKeyCode2KanaMidTable(in m_keyCode2KanaMidCsv);}
                }
            }
        }

        #region
        [Tooltip("ひらがなの中間文字列からひらがな文字列への変換テーブルを定義したファイル\n" +
            "明示的に指定しなかった場合、以下のファイルを読み込みます。\n" +
            "【Assets/Resources/TypeModule/data/Char2Kana/JISkana.csv】\n" +
            "独自で指定する場合は、以下のようなCSV(.csv)形式ファイルを用意してください。文字コードは[UTF-8]としてください。\n" +
            "［形式］ひらがな中間文字列,ひらがな文字列\n" +
            "例) \n" +
            "か゛,が\n" +
            "き゛,ぎ")]
        [SerializeField, PropertyBackingField("KanaMid2KanaCsv")] private TextAsset m_kanaMid2KanaCsv;
        #endregion
        /// <summary>
        /// <para>ひらがなの中間文字列からひらがな文字列への変換テーブルを定義したファイル</para>
        /// <para>明示的に指定しなかった場合、以下のファイルを読み込みます。</para>
        /// <para>【Assets/Resources/TypeModule/data/Char2Kana/JISkana.csv】</para>
        /// <para>独自で指定する場合は、以下のようなCSV(.csv)形式ファイルを用意してください。文字コードは[UTF-8]としてください。</para>
        /// <para>［形式］ひらがな中間文字列,ひらがな文字列</para>
        /// <para>例) \n</para>
        /// <para>か゛,が</para>
        /// <para>き゛,ぎ</para>
        /// </summary>
        public TextAsset KanaMid2KanaCsv {
            get { return m_kanaMid2KanaCsv; }
            set {
                if (m_kanaMid2KanaCsv != value) {
                    m_kanaMid2KanaCsv = value;
                    if (m_convertTableMgr != null) {m_convertTableMgr.SetKanaMid2KanaTable(in m_kanaMid2KanaCsv);}
                }
            }
        }

        #region
        [Space(10)]
        [Tooltip("CapsLockの状態を反映させるかどうか。\n" +
           "[true]の場合、CapsLock中は、英語の入力に対して大小文字を反転させます。")]
        [SerializeField, PropertyBackingField("EnabledCapsLock")] private bool m_enabledCapsLock = true;
        #endregion
        /// <summary>
        /// <para>CapsLockの状態を反映させるかどうか。</para>
        /// <para>[true]の場合、CapsLock中は、英語の入力に対して大小文字を反転させます。</para>
        /// </summary>
        public bool EnabledCapsLock {
            get { return m_enabledCapsLock; }
            set {
                if (m_convertTableMgr != null) {m_convertTableMgr.EnabledCapsLock = value;}
                m_enabledCapsLock = value;
            }
        }
        #endregion
        #endregion


        #region 内部メソッド
        ///<summary>文字列生成時に使用する、変換テーブルを作成</summary>
        private void CreateConvertTables() {
            m_convertTableMgr = new ConvertTableMgr();
            m_convertTableMgr.EnabledCapsLock = EnabledCapsLock;

            //インスペクターのファイルアセットで上書き
            if (KeyCode2RomaCsv != null) {      m_convertTableMgr.SetKeyCode2RomaTable(in m_keyCode2RomaCsv);}
            if (Roma2KanaCsv != null) {         m_convertTableMgr.SetRoma2KanaTable(in m_roma2KanaCsv);}
            if (KeyCode2KanaMidCsv != null) {   m_convertTableMgr.SetKeyCode2KanaMidTable(in m_keyCode2KanaMidCsv);}
            if (KanaMid2KanaCsv != null) {      m_convertTableMgr.SetKanaMid2KanaTable(in m_kanaMid2KanaCsv);}
        }

        ///<summary>キーボードの入力から文字列生成をエミュレートする為のクラスの作成</summary>
        private void CreateInputEmulator() {
            m_inputEmulator             = new InputEmulator(m_convertTableMgr);
            m_inputEmulator.IsInputEng  = IsInputEng;
            m_inputEmulator.IsKana      = IsKana;
            m_inputEmulator.EnabledBS   = EnabledBS;
            m_inputEmulator.EnabledEnter= EnabledEnter;
        }

        ///<summary>指定された文字列が正しく打ててるかを確認する為のクラスの作成</summary>
        private void CreateCopyInputChecker() {
            m_copyInputChecker = new CopyInputChecker(m_convertTableMgr);
            m_copyInputChecker.IsKana = IsKana;
            m_copyInputChecker.IsCaseSensitive = IsCaseSensitive;
            m_copyInputChecker.TargetStr = TargetStr;
        }
        #endregion


        #region Unity共通処理
        void Awake() {
            CreateConvertTables();
            CreateInputEmulator();
            CreateCopyInputChecker();
        }

        private void OnGUI() {
            if (IsRun && Event.current.type == EventType.KeyDown) {
                switch (Mode) {
                    case MODE.INPUT:    m_inputEmulator.AddInput(Event.current);    break;
                    case MODE.COPY:     m_copyInputChecker.AddInput(Event.current); break;
                }
            }
        }
        #endregion


        #region メンバ
        ConvertTableMgr m_convertTableMgr;
        InputEmulator m_inputEmulator;
        CopyInputChecker m_copyInputChecker;
        #endregion
    }

}