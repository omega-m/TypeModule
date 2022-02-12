using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using tpInner;
using Candlelight;
using UnityEngine.UI;

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
/// //文字列生成モードに切り替え
/// Mode = TypeModule.MODE.MODE_INPUT;
/// 
/// //英語入力状態へ
/// module.IsInputEng = true;
/// 
/// //かな入力入力状態へ
/// module.IsKana = true;
/// 
/// //BSで文字を消せるかどうか
/// module.IsBS     = true;
/// 
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
        ///キーボードの入力を元に、文字列の生成をエミュレートした後
        ///【m_targetText】で指定された文字列と比較し、その結果を格納します。
        ///タイピングゲームで、お題の文字と一致するかどうかを確認したい場合に利用してください。
        ///</summary>
        MODE_COMPARE,
    }
    #endregion

    #region Unity共通処理
    void Awake() {
        CreateConvertTables();
        CreateInputEmulator();
    }

    void Update() {
    }

    private void OnGUI() {
        if (IsRun) {
            if (Event.current.type == EventType.KeyDown) {
                switch (Mode) {
                    case MODE.MODE_INPUT:
                        m_inputEmulator.AddInput(Event.current);
                        //test============
                        testInput.text = m_inputEmulator.Str;
                        testInputRaw.text = m_inputEmulator.StrRaw;
                        Debug.Log(m_inputEmulator.PrevChar);
                        Debug.Log(m_inputEmulator.Event);
                        //test============
                        break;
                    case MODE.MODE_COMPARE:
                        break;
                }
            }
        }
    }
    #endregion

    #region メソッド
    //public void Clear(){}
    #endregion

    #region プロパティ
    #region
    [Tooltip(
        "文字入力判定モードです。\n" +
        "【MODE_INPUT】   :キーボードの入力を元に、文字列の生成をエミュレートします。\n" +
        "                   キーボード入力から文字列を取得したい場合に使用してください。\n" +
        "【MODE_COMPARE】 :キーボードの入力を元に、文字列の生成をエミュレートした後\n" +
        "                  【m_targetText】で指定された文字列と比較し、その結果を格納します。\n" +
        "                  タイピングゲームで、お題の文字と一致するかどうかを確認したい場合に利用してください。"
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
        }
    }

    [Header("Mode = MODE_INPUT の時の設定")]
    #region 
    [Tooltip(
        "[true]英語入力モード\n" +
        "[false]日本語入力モード"
       )]
    [SerializeField, PropertyBackingField("IsInputEng")] private bool m_isInputEng = false;
    #endregion
    public bool IsInputEng {
        get { return m_isInputEng; }
        set {
            m_isInputEng = value;
            if(m_inputEmulator != null) {
                m_inputEmulator.IsInputEng = IsInputEng;
            }
        }
    }

    #region 
    [Tooltip(
        "BackSoaceで文字を消せるかどうか"
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

    [Header("以下詳細設定 (指定しなくても動きます)")]
    #region 
    [Tooltip(
        "キーの入力(KeyCode)からローマ字文字への変換テーブルを定義したファイル\n" +
        "明示的に指定しなかった場合、以下のファイルを読み込みます。\n" +
        "【Assets/Resources/Scripts/TypeModule/data/KeyCode2Char/qwerty.csv】\n" +
        "独自で指定する場合は、以下のようなCSV(.csv)形式ファイルを用意してください。文字コードは[UTF-8]としてください。\n" +
        "変換先文字,【UnityEngine.KeyCode】, isShift, isFunction\n" +
        "例) \n" +
        "S,115,1,0\n" +
        "s,115,0,0\n"

       )]
    [SerializeField, PropertyBackingField("KeyCode2RomaCsv")] private TextAsset m_keyCode2RomaCsv;
    #endregion
    public TextAsset KeyCode2RomaCsv{
        get { return m_keyCode2RomaCsv; }
        set{
            if (m_keyCode2RomaCsv != value){
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
    public TextAsset Roma2KanaCsv{
        get { return m_roma2KanaCsv; }
        set{
            if (m_roma2KanaCsv != value){
                m_roma2KanaCsv = value;
                if (m_convertTableMgr != null) {
                    m_convertTableMgr.SetRoma2KanaTable(in m_roma2KanaCsv);
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
        "変換先文字,【UnityEngine.KeyCode】, isShift, isFunction\n" +
        "例) \n" +
        "ぬ,49,0,0 \n" +
        "ぬ,49,1,0 \n" +
        "ふ,50,0,0 \n"
       )]
    [SerializeField, PropertyBackingField("KeyCode2KanaMidCsv")] private TextAsset m_keyCode2KanaMidCsv;
    #endregion
    public TextAsset KeyCode2KanaMidCsv{
        get { return m_keyCode2KanaMidCsv; }
        set{
            if (m_keyCode2KanaMidCsv != value){
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
        "ひらがな中間文字列,ひらがな文字列\n" +
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
    public bool IsCheckCapsLock{
        get { return m_isCheckCapsLock; }
        set{
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
    }

    ///<summary>
    /// キーボードの入力から文字列生成をエミュレートする為のクラスの作成
    ///</summary>
    private void CreateInputEmulator() {
        m_inputEmulator = new InputEmulator(m_convertTableMgr);
        m_inputEmulator.IsInputEng = IsInputEng;
        m_inputEmulator.IsKana = IsKana;
        m_inputEmulator.IsBS = IsBS;
    }
    #endregion

    #region メンバ
    ConvertTableMgr m_convertTableMgr;
    InputEmulator m_inputEmulator;
    #endregion


    [Space(10)]
    [Header("テスト用")]
    public Text testInput;
    public Text testInputRaw;

}
