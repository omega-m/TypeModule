using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using tm;
using UnityEngine.UI;
using System.Timers;


/// <summary>S01InputTestのマネージャークラス</summary>
public class S01Manager : MonoBehaviour{

    #region メソッド
    /// <summary>シーン「S00Menu」を呼び出し</summary>
    public void BackToMenu() {
        SceneManager.LoadScene("S00Menu");
    }

    /// <summary>TypeModule OnChangeイベントリスナ</summary>
    public void TmOnChange(InputEmulatorResults res) {
        m_textInput.text = res.Str;
        m_textInputRaw.text = res.StrRaw;


        m_textKeyCode.text = res.Event.keyCode.ToString();
        m_textIsShift.text = res.Event.shift.ToString();
        m_textIsFunction.text = res.Event.functionKey.ToString();
        m_textInputType.text = res.InputType.ToString();
        m_textPrevChar.text = res.PrevChar.ToString();
    }

    /// <summary>キーボード配列が変わった時に呼び出されます</summary>
    public void OnChangeedKeyboardLayout(int aDummy = 0) {
        switch (m_dropDownInputLayout.value) {
            case 0: 
                m_tp.IsKana = false;
                m_tp.KeyCode2RomaCsv = Resources.Load("TypeModule/data/KeyCode2Char/qwerty", typeof(TextAsset)) as TextAsset;
                break;
            case 1:
                m_tp.IsKana = false;
                m_tp.KeyCode2RomaCsv = Resources.Load("TypeModule/data/KeyCode2Char/us", typeof(TextAsset)) as TextAsset;
                break;
            case 2:
                m_tp.IsKana = true;
                break;
        }
    }

    /// <summary>英語入力かそうでないかが変更された時に呼び出されます</summary>
    public void OnChangeedIsEng(bool aDummy=  true) {
        m_tp.IsInputEng = m_toggleIsEng.isOn;
    }

    /// <summary>文字列全クリア</summary>
    public void ClearAllText() {
        m_tp.Clear();        
    }

    #endregion


    #region Unity共通処理
    void Start() {
        //Initialize TypeModule
        m_tp = GetComponent<TypeModule>();
        m_tp.Mode = TypeModule.MODE.INPUT;
        m_tp.AddEventListenerOnChange(TmOnChange);
        OnChangeedKeyboardLayout();
        OnChangeedIsEng();
    }

    void Update() {

    }
    #endregion


    #region インスペクタメンバ
    public Text m_textInput;
    public Text m_textInputRaw;

    public Text m_textKeyCode;
    public Text m_textIsShift;
    public Text m_textIsFunction;
    public Text m_textInputType;
    public Text m_textPrevChar;

    public Dropdown m_dropDownInputLayout;
    public Toggle m_toggleIsEng;

    #endregion


    #region メンバ
    private TypeModule m_tp;
    #endregion 
}
