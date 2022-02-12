using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using tpInner;
using Candlelight;
using UnityEngine.UI;
using UnityEngine.Events;

///<summary>
///キーボードの入力を元に、文字入力の判定を担当するモジュールです。
///CPU処理負荷や、メモリ要領的な問題で、特に理由がない場合は、マネジメントクラスで管理する方が良さそうです。
/// </summary>
/// <example><code>
///     ...
///     
/// module = GetComponent<TypeModule>();
///     
/// 
/// //文字列生成シミュレーションモード=======================================
/// module.Mode = TypeModule.MODE.MODE_INPUT;
/// 
/// //モジュールから状態を取得
/// Debug.Log(module.Str);
/// Debug.Log(module.prevChar);
/// Debug.Log(module.StrRaw);
/// 
/// //生成モードを変更
/// module.IsInputEng = true    //英語入力状態へ
/// module.IsKana = true;       //かな入力入力状態へ
/// module.IsBS     = true;     //BSで文字を消せるかどうか
/// 
/// //プログラムから文字列を操作
/// module.Enter();
/// module.BackSpace();
/// 
/// 
/// //イベントリスナを追加し、文字列に変更があった時にテキストを修正
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
///     testInput.text = res.Str;
///     testInputRaw.text = res.StrRaw;
/// }
/// 
/// 
/// /// //イベントリスナを追加し、文字が打たれた時にサウンドを再生
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
public class TypeModule : MonoBehaviour {

    #region 入力判定モード
    ///<summary>
    ///文字入力判定モードです。
    ///</summary>
    public enum MODE {
        ///<summary>
        ///キーボードの入力を元に、文字列の生成をエミュレートします。
        ///キーボード入力から文字列を取得したい場合に使用してください。
        ///</summary>
        MODE_INPUT,
        ///<summary>
        ///【m_targetText】で指定された文字列が正しく打ててるかを確認します。
        ///タイピングゲームで、お題の文字を真似して打たせる時に使用してください。
        ///</summary>
        MODE_COPY,
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
        if (IsRun) {
            if (Event.current.type == EventType.KeyDown) {
                switch (Mode) {
                    case MODE.MODE_INPUT:
                        m_inputEmulator.AddInput(Event.current);
                        break;
                    case MODE.MODE_COPY:
                        m_copyInputChecker.AddInput(Event.current);
                        break;
                }
            }
        }
    }
    #endregion

    #region 共通メソッド
    /// <summary>
    /// 内部の入力データをを全て削除します
    /// </summary>
    public void Clear() {
        m_inputEmulator.Clear();
        m_copyInputChecker.Clear();
    }
    #endregion

    #region MODE.MODE_INPUT用メソッド プロパティ
    /// <summary>
    /// 生成された文字列
    /// </summary>
    public string Str {
        get { return m_inputEmulator.Str; }
    }

    /// <summary>
    /// 生成された、変換される前の文字列
    /// </summary>
    public string StrRaw {
        get { return m_inputEmulator.StrRaw; }
    }

    /// <summary>
    /// 前回入力された文字(変換前)
    /// </summary>
    public string PrevChar {
        get { return m_inputEmulator.PrevChar; }
    }

    /// <summary>
    /// 前回入力発生時のUnityイベント
    /// </summary>
    public Event Event {
        get { return m_inputEmulator.Event; }
    }
    /// <summary>
    /// プログラム側から、変換確定前の文字列を確定します。
    /// </summary>
    public void Enter() {
        if (Mode == MODE.MODE_INPUT) {
            m_inputEmulator.Enter();
        }
    }

    /// <summary>
    /// プログラム側から、末尾の1文字消します。
    /// </summary>
    public void BackSpace() {
        if (Mode == MODE.MODE_INPUT) {
            m_inputEmulator.BackSpace();
        }
    }

    /// <summary>
    /// キーボードから文字が入力された時のイベントリスナを追加します
    /// </summary>
    /// <param name="aEvent">イベントリスナ</param>
    public void AddEventListenerOnInput(UnityAction<InputEmulatorResults> aEvent) {
        m_inputEmulator.AddEventListenerOnInput(aEvent);
    }

    /// <summary>
    /// キーボードから文字が入力された時のイベントリスナを削除します
    /// </summary>
    /// <param name="aEvent">イベントリスナ</param>
    public void RemoveEventListenerOnInput(UnityAction<InputEmulatorResults> aEvent) {
        m_inputEmulator.RemoveEventListenerOnInput(aEvent);
    }

    /// <summary>
    /// 文字列が変更された時のイベントリスナを追加します
    /// </summary>
    /// <param name="aEvent">イベントリスナ</param>
    public void AddEventListenerOnChange(UnityAction<InputEmulatorResults> aEvent) {
        m_inputEmulator.AddEventListenerOnChange(aEvent);
    }

    /// <summary>
    /// 文字列が変更された時のイベントリスナを削除します
    /// </summary>
    /// <param name="aEvent">イベントリスナ</param>
    public void RemoveEventListenerOnChange(UnityAction<InputEmulatorResults> aEvent) {
        m_inputEmulator.RemoveEventListenerOnChange(aEvent);
    }
    #endregion

    #region インスペクタープロパティ
    #region
    [Tooltip(
        "文字入力判定モードです。\n" +
        "【MODE_INPUT】   :キーボードの入力を元に、文字列の生成をエミュレートします。\n" +
        "【MODE_COMPARE】 :指定された文字列が正しく打ててるかを確認します。タイピングゲームで、お題の文字を真似して打たせる時に使用してください。\n"
            )]
    #endregion
    [SerializeField, PropertyBackingField("Mode")] private MODE m_mode = MODE.MODE_INPUT;
    public MODE Mode {
        get { return m_mode; }
        set { m_mode = value; }
    }

    #region
    [Tooltip(
       "現在入力を受け付けているか\n" +
       "[false]にした場合、入力チェックを行わなくなります。ポーズ時やゲーム外の時に使用してください。"
       )]
    [SerializeField, PropertyBackingField("IsRun")] private bool m_isRun = true;
    #endregion
    public bool IsRun {
        get { return m_isRun; }
        set { m_isRun = value; }
    }

    #region
    [Tooltip(
        "JISかな入力など、日本語を直接入力する方式を使用してエミュレートするかどうかのフラグです。\n" +
        "[MODE_INPUT]trueの場合、[m_keyCodeToKanaCsv]から文字列生成をエミュレートします。\n" +
        "[MODE_COMPARE]trueの場合、日本語と比較する場合は[m_keyCodeToKanaCsv]から、そうでない場合は[m_keyCode2RomaCsv]から文字列生成をエミュレートします。\n"
        )]
    [SerializeField, PropertyBackingField("IsKana")] private bool m_isKana = false;
    #endregion
    public bool IsKana {
        get { return m_isKana; }
        set {
            m_isKana = value;
            if (m_inputEmulator != null) {
                m_inputEmulator.IsKana = IsKana;
            }
            if(m_copyInputChecker != null) {
                m_copyInputChecker.IsKana = IsKana;
            }
        }
    }

    [Header("Modeが【MODE_INPUT】の時の設定")]
    #region 
    [Tooltip(
        "入力モード\n" +
        "[true]英語入力モード\n" +
        "[false]日本語入力モード"
       )]
    [SerializeField, PropertyBackingField("IsInputEng")] private bool m_isInputEng = false;
    #endregion
    public bool IsInputEng {
        get { return m_isInputEng; }
        set {
            m_isInputEng = value;
            if (m_inputEmulator != null) {
                m_inputEmulator.IsInputEng = IsInputEng;
            }
        }
    }

    #region 
    [Tooltip(
        "BackSoaceキーを押した時、文字を消すかどうか"
       )]
    [SerializeField, PropertyBackingField("IsBS")] private bool m_isBS = true;
    #endregion
    public bool IsBS {
        get { return m_isBS; }
        set {
            m_isBS = value;
            if (m_inputEmulator != null) {
                m_inputEmulator.IsBS = IsBS;
            }
        }
    }

    #region 
    [Tooltip(
        "Enterキーを押した時、確定前の文字列を確定するかどうか"
       )]
    [SerializeField, PropertyBackingField("IsEnter")] private bool m_isEnter = true;
    #endregion
    public bool IsEnter {
        get { return m_isEnter; }
        set {
            m_isEnter = value;
            if (m_inputEmulator != null) {
                m_inputEmulator.IsEnter = IsEnter;
            }
        }
    }

    [Header("以下詳細設定 (指定しなくても動きます)")]
    #region 
    [Tooltip(
        "キーの入力(KeyCode)からローマ字文字への変換テーブルを定義したファイル\n" +
        "明示的に指定しなかった場合、以下のファイルを読み込みます。\n" +
        "【Assets/Resources/Scripts/TypeModule/data/KeyCode2Char/qwerty.csv】\n" +
        "独自で指定する場合は、以下のようなCSV(.csv)形式ファイルを用意してください。文字コードは[UTF-8]としてください。\n" +
        "［形式］変換先文字,【UnityEngine.KeyCode】, isShift, isFunction\n" +
        "例) \n" +
        "S,115,1,0\n" +
        "s,115,0,0\n"

       )]
    [SerializeField, PropertyBackingField("KeyCode2RomaCsv")] private TextAsset m_keyCode2RomaCsv;
    #endregion
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
    [Tooltip(
        "ローマ字文字列からひらがな文字列への変換テーブルを定義したファイル\n" +
        "明示的に指定しなかった場合、以下のファイルを読み込みます。\n" +
        "【Assets/Resources/Scripts/TypeModule/data/Char2Kana/roma.csv】\n" +
        "独自で指定する場合は、以下のようなCSV(.csv)形式ファイルを用意してください。文字コードは[UTF-8]としてください。\n" +
        "ローマ字文字列,ひらがな文字列\n" +
        "例) \n" +
        "a,あ\n" +
        "shi,し\n"
   )]
    [SerializeField, PropertyBackingField("Roma2KanaCsv")] private TextAsset m_roma2KanaCsv;
    #endregion
    public TextAsset Roma2KanaCsv {
        get { return m_roma2KanaCsv; }
        set {
            if (m_roma2KanaCsv != value) {
                m_roma2KanaCsv = value;
                if (m_convertTableMgr != null) {
                    m_convertTableMgr.SetRoma2KanaTable(in m_roma2KanaCsv);
                }
            }
        }
    }

    #region
    [Tooltip(
        "数字と記号の、全角半角の変換テーブルを定義したファイル\n" +
        "明示的に指定しなかった場合、以下のファイルを読み込みます。\n" +
        "【Assets/Resources/Scripts/TypeModule/data/Char2Kana/nummark.csv】\n" +
        "独自で指定する場合は、以下のようなCSV(.csv)形式ファイルを用意してください。文字コードは[UTF-8]としてください。\n" +
        "［形式］半角文字,全角文字\n" +
        "例) \n" +
        ".,。\n" +
        "?,？\n"
   )]
    [SerializeField, PropertyBackingField("NumMarkCsv")] private TextAsset m_numMarkCsv;
    #endregion
    public TextAsset NumMarkCsv {
        get { return m_numMarkCsv; }
        set {
            if (m_numMarkCsv != value) {
                m_numMarkCsv = value;
                if (m_convertTableMgr != null) {
                    m_convertTableMgr.SetNumMarkTable(in m_numMarkCsv);
                }
            }
        }
    }

    [Space(10)]
    #region
    [Tooltip(
        "キーの入力(KeyCode)からひらがなの中間文字への変換テーブルを定義したファイル\n" +
        "JISかな入力など、日本語を直接入力する方式を使用する際に参照します。\n" +
        "明示的に指定しなかった場合、以下のファイルを読み込みます。\n" +
        "【Assets/Resources/Scripts/TypeModule/data/KeyCode2Char/JISkana.csv】\n" +
        "独自で指定する場合は、以下のようなCSV(.csv)形式ファイルを用意してください。文字コードは[UTF-8]としてください。\n" +
        "［形式］変換先文字,【UnityEngine.KeyCode】, isShift, isFunction\n" +
        "例) \n" +
        "ぬ,49,0,0 \n" +
        "ぬ,49,1,0 \n" +
        "ふ,50,0,0 \n"
       )]
    [SerializeField, PropertyBackingField("KeyCode2KanaMidCsv")] private TextAsset m_keyCode2KanaMidCsv;
    #endregion
    public TextAsset KeyCode2KanaMidCsv {
        get { return m_keyCode2KanaMidCsv; }
        set {
            if (m_keyCode2KanaMidCsv != value) {
                m_keyCode2KanaMidCsv = value;
                if (m_convertTableMgr != null) {
                    m_convertTableMgr.SetKeyCode2KanaMidTable(in m_keyCode2KanaMidCsv);
                }
            }
        }
    }

    #region
    [Tooltip(
        "ひらがなの中間文字列からひらがな文字列への変換テーブルを定義したファイル\n" +
        "明示的に指定しなかった場合、以下のファイルを読み込みます。\n" +
        "【Assets/Resources/Scripts/TypeModule/data/Char2Kana/JISkana.csv】\n" +
        "独自で指定する場合は、以下のようなCSV(.csv)形式ファイルを用意してください。文字コードは[UTF-8]としてください。\n" +
        "［形式］ひらがな中間文字列,ひらがな文字列\n" +
        "例) \n" +
        "か゛,が\n" +
        "き゛,ぎ\n"
   )]
    [SerializeField, PropertyBackingField("KanaMid2KanaCsv")] private TextAsset m_kanaMid2KanaCsv;
    #endregion
    public TextAsset KanaMid2KanaCsv {
        get { return m_kanaMid2KanaCsv; }
        set {
            if (m_kanaMid2KanaCsv != value) {
                m_kanaMid2KanaCsv = value;
                if (m_convertTableMgr != null) {
                    m_convertTableMgr.SetKanaMid2KanaTable(in m_kanaMid2KanaCsv);
                }
            }
        }
    }

    [Space(10)]
    #region
    [Tooltip(
       "CapsLockの状態を反映させるかどうか。\n" +
       "[true]の場合、CapsLock中は、英語の入力に対して大小文字を反転させます。"
       )]
    [SerializeField, PropertyBackingField("IsCheckCapsLock")] private bool m_isCheckCapsLock = true;
    #endregion
    public bool IsCheckCapsLock {
        get { return m_isCheckCapsLock; }
        set {
            if (m_convertTableMgr != null) {
                m_convertTableMgr.IsCheckCapsLock = value;
            }
            m_isCheckCapsLock = value;
        }
    }
    #endregion

    #region 内部メソッド
    ///<summary>
    /// 文字列生成時に使用する、変換テーブルを作成
    ///</summary>
    private void CreateConvertTables() {
        m_convertTableMgr = new ConvertTableMgr();
        m_convertTableMgr.IsCheckCapsLock = IsCheckCapsLock;

        //インスペクターのファイルアセットで上書き
        if (KeyCode2RomaCsv != null) {
            m_convertTableMgr.SetKeyCode2RomaTable(in m_keyCode2RomaCsv);
        }
        if (Roma2KanaCsv != null) {
            m_convertTableMgr.SetRoma2KanaTable(in m_roma2KanaCsv);
        }
        if (KeyCode2KanaMidCsv != null) {
            m_convertTableMgr.SetKeyCode2KanaMidTable(in m_keyCode2KanaMidCsv);
        }
        if (KanaMid2KanaCsv != null) {
            m_convertTableMgr.SetKanaMid2KanaTable(in m_kanaMid2KanaCsv);
        }
        if (NumMarkCsv != null) {
            m_convertTableMgr.SetNumMarkTable(in m_numMarkCsv);
        }
    }

    ///<summary>
    /// キーボードの入力から文字列生成をエミュレートする為のクラスの作成
    ///</summary>
    private void CreateInputEmulator() {
        m_inputEmulator             = new InputEmulator(m_convertTableMgr);
        m_inputEmulator.IsInputEng  = IsInputEng;
        m_inputEmulator.IsKana      = IsKana;
        m_inputEmulator.IsBS        = IsBS;
        m_inputEmulator.IsEnter     = IsEnter;
    }

    ///<summary>
    /// 指定された文字列が正しく打ててるかを確認する為のクラスの作成
    ///</summary>
    private void CreateCopyInputChecker() {
        m_copyInputChecker = new CopyInputChecker(m_convertTableMgr);
        m_copyInputChecker.IsKana = IsKana;
    }
    #endregion

    #region メンバ
    ConvertTableMgr m_convertTableMgr;
    InputEmulator m_inputEmulator;
    CopyInputChecker m_copyInputChecker;
    #endregion
}