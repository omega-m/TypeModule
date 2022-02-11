using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using tpInner;

///<summary>
///キーボードの入力を元に、文字入力の判定を担当するモジュールです。
///CPU処理負荷や、メモリ要領的な問題で、特に理由がない場合は、マネジメントクラスで管理する方が良さそうです。
/// </summary>
public class TypeModule : MonoBehaviour{
    #region 入力判定モード
    ///<summary>文字入力判定モードです。</summary>
    public enum MODE{
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
    void Start(){
        CreateConvertTables();
    }

    void Update(){
    }

    private void OnGUI(){
        if (Event.current.type == EventType.KeyDown && Event.current.keyCode != KeyCode.None){
            if (IsKana){
                Debug.Log(m_convertTables.Key2kanaMid.Convert(Event.current.keyCode, Event.current.shift, Event.current.functionKey));
            }
            else{
                Debug.Log(m_convertTables.Key2Roma.Convert(Event.current.keyCode, Event.current.shift, Event.current.functionKey));
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
    [SerializeField] private MODE m_mode = MODE.MODE_INPUT;
    public MODE Mode
    {
        get { return m_mode; }
        set { m_mode = value; }
    }

    #region 
    [Header("以下詳細設定 (指定しなくても動きます)")]
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
    #endregion
    [SerializeField] private TextAsset m_keyCode2RomaCsv;
    public TextAsset KeyCode2RomaCsv{
        get { return m_keyCode2RomaCsv; }
        set{
            if (m_keyCode2RomaCsv != value){
                m_keyCode2RomaCsv = value;
                m_convertTables.SetKeyCode2RomaTable(m_keyCode2RomaCsv);
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
    #endregion
    [SerializeField] private TextAsset m_roma2KanaCsv;
    public TextAsset Roma2KanaCsv{
        get { return m_roma2KanaCsv; }
        set{
            if (m_roma2KanaCsv != value){
                m_roma2KanaCsv = value;
                m_convertTables.SetRoma2KanaTable(m_roma2KanaCsv);
            }
        }
    }

    #region
    [Space(10)]
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
    #endregion
    [SerializeField] private TextAsset m_keyCode2KanaMidCsv;
    public TextAsset KeyCode2KanaMidCsv{
        get { return m_keyCode2KanaMidCsv; }
        set{
            if (m_keyCode2KanaMidCsv != value){
                m_keyCode2KanaMidCsv = value;
                m_convertTables.SetKeyCode2KanaMidTable(m_keyCode2KanaMidCsv);
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
    #endregion
    [SerializeField] private TextAsset m_kanaMid2KanaCsv;
    public TextAsset KanaMid2KanaCsv {
        get { return m_kanaMid2KanaCsv; }
        set {
            if (m_kanaMid2KanaCsv != value) {
                m_kanaMid2KanaCsv = value;
                m_convertTables.SetKanaMid2KanaTable(m_kanaMid2KanaCsv);
            }
        }
    }

    #region
    [Space(10)]
    [Tooltip(
        "JISかな入力など、日本語を直接入力する方式を使用してエミュレートするかどうかのフラグです。\n" +
        "[MODE_INPUT]trueの場合、[m_keyCodeToKanaCsv]から文字列生成をエミュレートします。\n" +
        "[MODE_COMPARE]trueの場合、日本語と比較する場合は[m_keyCodeToKanaCsv]から、そうでない場合は[m_keyCode2RomaCsv]から文字列生成をエミュレートします。\n"
        )]
    #endregion
    [SerializeField] private bool m_isKana = false;
    public bool IsKana{
        get { return m_isKana; }
        set { m_isKana = value; }
    }

    #region
    [Tooltip(
       "CapsLockの状態を反映させるかどうか。\n" +
       "[true]の場合、CapsLock中は、英語の入力に対して大小文字を反転させます。"
       )]
    #endregion
    [SerializeField] private bool m_isCheckCapsLock = true;
    public bool IsCheckCapsLock{
        get { return m_isCheckCapsLock; }
        set{
            m_convertTables.IsCheckCapsLock = value;
            m_isCheckCapsLock = value;
        }
    }
    #endregion

    #region 内部メソッド
    ///<summary>
    /// 文字列生成時に使用する、変換テーブルを作成
    ///</summary>
    private void CreateConvertTables() {
        m_convertTables = new ConvertTableMgr();
        m_convertTables.IsCheckCapsLock = IsCheckCapsLock;

        //インスペクターのファイルアセットで上書き
        if (KeyCode2RomaCsv != null) {
            m_convertTables.SetKeyCode2RomaTable(KeyCode2RomaCsv);
        }
        if (Roma2KanaCsv != null) {
            m_convertTables.SetRoma2KanaTable(Roma2KanaCsv);
        }
        if (KeyCode2KanaMidCsv != null) {
            m_convertTables.SetKeyCode2KanaMidTable(KeyCode2KanaMidCsv);
        }
        if (KanaMid2KanaCsv != null) {
            m_convertTables.SetKanaMid2KanaTable(KanaMid2KanaCsv);
        }

    }
    #endregion

    #region メンバ
    ConvertTableMgr m_convertTables;
    #endregion 
}
