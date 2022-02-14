using UnityEngine;
using tpInner;
using Candlelight;
using UnityEngine.Events;

///<summary>
///<para>キーボードの入力を元に、文字入力の判定を担当するモジュールです。</para>
///<para>CPU処理負荷や、メモリ要領的な問題で、特に理由がない場合は、マネジメントクラスで管理する方が良さそうです。</para>
/// </summary>
/// <example><code>
///     ...
///     
/// module = GetComponent<TypeModule>();
///     
/// //文字列生成シミュレーションモード=======================================
/// module.Mode = TypeModule.MODE.INPUT;
/// 
/// //モジュールから状態を取得
/// Debug.Log(module.Str);
/// Debug.Log(module.PrevChar);
/// Debug.Log(module.StrRaw);
/// 
/// //モードを変更
/// module.IsInputEng = true    //英語入力状態へ
/// module.IsKana     = true;     //かな入力状態へ
/// module.EnabledBS  = true;     //BSで文字を消せるかどうか
/// 
/// //プログラムから文字列を操作
/// module.Enter();             //変換確定前の文字列を確定
/// module.BackSpace();         //末尾から1文字削除
/// module.Clear();             //全ての文字を削除
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
/// 
/// //指定された文字列が正しく打ててるか、比較するモード=======================================
/// module.Mode = TypeModule.MODE.COPY;
/// 
/// //モードの変更
/// module.IsKana = true;                  /かな入力入力状態へ
/// module.IsCaseSensitive = true;         //英語の大文字と小文字の入力を区別
/// 
/// //比較対象の文字列をセット(内部の初期化も同時に行われます)
/// module.TargetStr = "こちらは、たいぴんぐするぶんしょうです。";
/// 
/// 
/// //直前に入力された文字を取得
/// Debug.Log(module.PrevCorrectChar);         //正しく入力された場合格納されます
/// Debug.Log(module.PrevMissChar);            //ミス入力の場合格納されます
/// 
/// //パラメータにアクセス
/// Debug.Log(module.CorrectNum);              //正しくタイプした数
/// Debug.Log(module.CorrectCharNum);          //正しく打てた文字数
/// Debug.Log(module.MissNum);                 //ミスタイプした数
/// Debug.Log(module.IsComplete);              //指定文字列を打ち切ったか
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
/// //イベントリスナを追加し、文字が打たれた時にサウンドを再生
/// public AudioSource audioSource;
/// public AudioClip correctSound;
/// public AudioClip missSound;
/// 
///     ...
/// 
/// module.AddEventListenerOnCorrect(onCorrect);
/// module.AddEventListenerOnMiss(onMiss);
/// audioSource = GetComponent<AudioSource>();
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
/// //以下オプションです================================
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


    #region 共通メソッド
    /// <summary>内部の入力データをを全て削除します</summary>
    public void Clear() {
        m_inputEmulator.Clear();
        m_copyInputChecker.Clear();
    }

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
    #endregion


    #region 【MODE.INPUT】プロパティ
    /// <summary>【MODE.INPUT】生成された文字列</summary>
    public string StrInput {get { return m_inputEmulator.Str; }}

    /// <summary>【MODE.INPUT】生成された、変換される前の文字列</summary>
    public string StrRawInput {get { return m_inputEmulator.StrRaw; }}

    /// <summary>【MODE.INPUT】前回入力された文字(変換前)</summary>
    public string PrevChar {get { return m_inputEmulator.PrevChar; }}

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
    public void RemoveEventListenerOnChange(UnityAction<InputEmulatorResults> aEvent) {m_inputEmulator.RemoveEventListenerOnChange(aEvent);}
    #endregion


    #region 【MODE.COPY】プロパティ
    /// <summary>【MODE.COPY】既に打ち終わった文字列</summary>
    public string StrDone {get { return m_copyInputChecker.StrDone; }}

    /// <summary>【MODE.COPY】現在打っている文字</summary>
    public string StrCurrent {get { return m_copyInputChecker.StrCurrent; }}

    /// <summary>【MODE.COPY】まだ打っていない文字列</summary>
    public string StrYet {get { return m_copyInputChecker.StrYet; }}

    /// <summary>【MODE.COPY】既に打ち終わった文字列(変換前)</summary>
    public string StrDoneRaw {get { return m_copyInputChecker.StrDoneRaw; }}

    /// <summary>【MODE.COPY】 現在打っている文字(変換前)</summary>
    public string StrCurrentRaw {get { return m_copyInputChecker.StrCurrentRaw; }}

    /// <summary>【MODE.COPY】既に打ち終わった文字列(変換前)</summary>
    public string StrYetRaw {get { return m_copyInputChecker.StrYetRaw; }}

    /// <summary>【MODE.COPY】前回正しく入力された文字(ミスした時は空文字列)</summary>
    public string PrevCorrectChar {get { return m_copyInputChecker.PrevCorrectChar; }}

    /// <summary>【MODE.COPY】前回ミスしたされた文字(正しく入力された時は空文字列)</summary>
    public string PrevMissChar {get { return m_copyInputChecker.PrevMissChar; }}

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
    [Tooltip("文字入力判定モードです。\n" +
        "【INPUT】   :キーボードの入力を元に、文字列の生成をエミュレートします。\n" +
        "【MODE_COMPARE】 :指定された文字列が正しく打ててるかを確認します。タイピングゲームで、お題の文字を真似して打たせる時に使用してください。")]
    [SerializeField, PropertyBackingField("Mode")] private MODE m_mode = MODE.INPUT;
    public MODE Mode {
        get { return m_mode; }
        set { m_mode = value; }
    }

    [Tooltip("現在入力を受け付けているか\n" +
       "[false]にした場合、入力チェックを行わなくなります。ポーズ時やゲーム外の時に使用してください。")]
    [SerializeField, PropertyBackingField("IsRun")] private bool m_isRun = true;
    public bool IsRun {
        get { return m_isRun; }
        set { m_isRun = value; }
    }

    [Tooltip("JISかな入力など、日本語を直接入力する方式を使用してエミュレートするかどうかのフラグです。\n" +
        "[INPUT]trueの場合、[m_keyCodeToKanaCsv]から文字列生成をエミュレートします。\n" +
        "[MODE_COMPARE]trueの場合、日本語と比較する場合は[m_keyCodeToKanaCsv]から、そうでない場合は[m_keyCode2RomaCsv]から文字列生成をエミュレートします。")]
    [SerializeField, PropertyBackingField("IsKana")] private bool m_isKana = false;
    public bool IsKana {
        get { return m_isKana; }
        set {
            m_isKana = value;
            if (m_inputEmulator != null) {m_inputEmulator.IsKana = IsKana;}
            if(m_copyInputChecker != null) {m_copyInputChecker.IsKana = IsKana;}
        }
    }

    [Header("Modeが【INPUT】の時の設定")]
    [Tooltip("入力モード\n" +
        "[true]英語入力モード\n" +
        "[false]日本語入力モード")]
    [SerializeField, PropertyBackingField("IsInputEng")] private bool m_isInputEng = false;
    public bool IsInputEng {
        get { return m_isInputEng; }
        set {
            m_isInputEng = value;
            if (m_inputEmulator != null) {m_inputEmulator.IsInputEng = IsInputEng;}
        }
    }

    [Tooltip("BackSoaceキーを押した時、文字を消すかどうか")]
    [SerializeField, PropertyBackingField("EnabledBS")] private bool m_enabledBS = true;
    public bool EnabledBS {
        get { return m_enabledBS; }
        set {
            m_enabledBS = value;
            if (m_inputEmulator != null) {m_inputEmulator.EnabledBS = EnabledBS; }
        }
    }

    [Tooltip("Enterキーを押した時、確定前の文字列を確定するかどうか")]
    [SerializeField, PropertyBackingField("EnabledEnter")] private bool m_enabledEnter = true;
    public bool EnabledEnter {
        get { return m_enabledEnter; }
        set {
            m_enabledEnter = value;
            if (m_inputEmulator != null) {m_inputEmulator.EnabledEnter = EnabledEnter; }
        }
    }

    [Header("Modeが【COPY】の時の設定")]
    [Tooltip("比較対象の文字列(タイピングのお台文)\n" +
        "値を変更した時点で、初期化処理を自動で呼び出します。\n" +
        "また、全角半角カタカナはひらがなに、全角アルファベットは半角アルファベットに変換されます。\n" +
        "[含む事が出来る文字]: -_| -_に対応する大文字|ァ-ヴ|ぁ-ゔ\n" +
        "含めない文字があった場合、エディタ上での実行なら例外を投げて止まります。ビルド版だとその文字が削除され、動き続けます。\n")]

    [SerializeField, PropertyBackingField("TargetStr")] private string m_targetStr = "";
    public string TargetStr {
        get { return m_targetStr; }
        set {
            m_targetStr = value;
            if (m_copyInputChecker != null) {
                m_copyInputChecker.TargetStr = value;
            }
        }
    }

    [Tooltip("英語の大文字と小文字の入力を区別して判定するか\n" +
        "初期化処理が発生した時点で反映されます。")]
    [SerializeField, PropertyBackingField("IsCaseSensitive")] private bool m_isCaseSensitive = false;
    public bool IsCaseSensitive {
        get { return m_isCaseSensitive; }
        set {
            m_isCaseSensitive = value;
            if (m_copyInputChecker != null) {m_copyInputChecker.IsCaseSensitive = value;}
        }
    }

    #region 詳細設定
    [Header("以下詳細設定 (指定しなくても動きます)")]
    [Tooltip("キーの入力(KeyCode)からローマ字文字への変換テーブルを定義したファイル\n" +
        "明示的に指定しなかった場合、以下のファイルを読み込みます。\n" +
        "【Assets/Resources/TypeModule/data/KeyCode2Char/qwerty.csv】\n" +
        "独自で指定する場合は、以下のようなCSV(.csv)形式ファイルを用意してください。文字コードは[UTF-8]としてください。\n" +
        "［形式］変換先文字,【UnityEngine.KeyCode】, isShift, isFunction\n" +
        "例) \n" +
        "S,115,1,0\n" +
        "s,115,0,0")]
    [SerializeField, PropertyBackingField("KeyCode2RomaCsv")] private TextAsset m_keyCode2RomaCsv;
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

    [Tooltip("ローマ字文字列からひらがな文字列への変換テーブルを定義したファイル\n" +
        "明示的に指定しなかった場合、以下のファイルを読み込みます。\n" +
        "【Assets/Resources/TypeModule/data/Char2Kana/roma.csv】\n" +
        "独自で指定する場合は、以下のようなCSV(.csv)形式ファイルを用意してください。文字コードは[UTF-8]としてください。\n" +
        "ローマ字文字列,ひらがな文字列\n" +
        "例) \n" +
        "a,あ\n" +
        "shi,し")]
    [SerializeField, PropertyBackingField("Roma2KanaCsv")] private TextAsset m_roma2KanaCsv;
    public TextAsset Roma2KanaCsv {
        get { return m_roma2KanaCsv; }
        set {
            if (m_roma2KanaCsv != value) {
                m_roma2KanaCsv = value;
                if (m_convertTableMgr != null) {m_convertTableMgr.SetRoma2KanaTable(in m_roma2KanaCsv);}
            }
        }
    }

    [Tooltip("数字と記号の、全角半角の変換テーブルを定義したファイル\n" +
        "明示的に指定しなかった場合、以下のファイルを読み込みます。\n" +
        "【Assets/Resources/TypeModule/data/Char2Kana/nummark.csv】\n" +
        "独自で指定する場合は、以下のようなCSV(.csv)形式ファイルを用意してください。文字コードは[UTF-8]としてください。\n" +
        "［形式］半角文字,全角文字\n" +
        "例) \n" +
        ".,。\n" +
        "?,？")]
    [SerializeField, PropertyBackingField("NumMarkCsv")] private TextAsset m_numMarkCsv;
    public TextAsset NumMarkCsv {
        get { return m_numMarkCsv; }
        set {
            if (m_numMarkCsv != value) {
                m_numMarkCsv = value;
                if (m_convertTableMgr != null) {m_convertTableMgr.SetNumMarkTable(in m_numMarkCsv);}
            }
        }
    }

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
    public TextAsset KeyCode2KanaMidCsv {
        get { return m_keyCode2KanaMidCsv; }
        set {
            if (m_keyCode2KanaMidCsv != value) {
                m_keyCode2KanaMidCsv = value;
                if (m_convertTableMgr != null) {m_convertTableMgr.SetKeyCode2KanaMidTable(in m_keyCode2KanaMidCsv);}
            }
        }
    }

    [Tooltip("ひらがなの中間文字列からひらがな文字列への変換テーブルを定義したファイル\n" +
        "明示的に指定しなかった場合、以下のファイルを読み込みます。\n" +
        "【Assets/Resources/TypeModule/data/Char2Kana/JISkana.csv】\n" +
        "独自で指定する場合は、以下のようなCSV(.csv)形式ファイルを用意してください。文字コードは[UTF-8]としてください。\n" +
        "［形式］ひらがな中間文字列,ひらがな文字列\n" +
        "例) \n" +
        "か゛,が\n" +
        "き゛,ぎ")]
    [SerializeField, PropertyBackingField("KanaMid2KanaCsv")] private TextAsset m_kanaMid2KanaCsv;
    public TextAsset KanaMid2KanaCsv {
        get { return m_kanaMid2KanaCsv; }
        set {
            if (m_kanaMid2KanaCsv != value) {
                m_kanaMid2KanaCsv = value;
                if (m_convertTableMgr != null) {m_convertTableMgr.SetKanaMid2KanaTable(in m_kanaMid2KanaCsv);}
            }
        }
    }

    [Space(10)]
    [Tooltip("CapsLockの状態を反映させるかどうか。\n" +
       "[true]の場合、CapsLock中は、英語の入力に対して大小文字を反転させます。")]
    [SerializeField, PropertyBackingField("EnabledCapsLock")] private bool m_enabledCapsLock = true;
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
        if (NumMarkCsv != null) {           m_convertTableMgr.SetNumMarkTable(in m_numMarkCsv);}
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

    void Update() {
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


