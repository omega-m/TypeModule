using System;
using System.Timers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using tm;


/// <summary>S02CopyTestのマネージャークラス</summary>
public class S02Manager : MonoBehaviour {

    #region 状態
    /// <summary>ゲームの状態を定義</summary>
    public enum STATE {
        NONE,
        /// <summary>スタートボタンが表示している画面</summary>
        MENU,
        /// <summary>カウントダウン中</summary>
        COUNTDOWN,
        /// <summary>ゲーム中</summary>
        GAME,
        /// <summary>リザルト画面</summary>
        RESULT,
    }
    #endregion


    #region Unity共通処理
    void Start() {
        SetupTypeModule();

        m_canvasMenu.enabled = false;
        m_canvasCountdown.enabled = false;

        ChangeState(STATE.MENU);
    }


    void Update() {
        switch (m_state) {
            case STATE.MENU:        UpdateMenu();       break;
            case STATE.COUNTDOWN:   UpdateCountDown();  break;
            case STATE.GAME:        UpdateGame();       break;
            case STATE.RESULT:      UpdateResult();     break;
            default:                Debug.Assert(false);break;
        }
    }
    #endregion


    #region 【MENU】メソッド
    /// <summary>【MENU】更新処理</summary>
    private void UpdateMenu() {
        //Debug.Log("MENU");
        switch (m_step) {
            case 0:
                m_canvasMenu.enabled = true;
                m_step++;
                break;
            case 1:
                //none
                break;
        }
    }

    /// <summary>シーン「S00Menu」を呼び出し</summary>
    public void BackToMenu() {
        if(m_state != STATE.MENU) { return; }
        SceneManager.LoadScene("S00Menu");
    }

    /// <summary>キーボード配列が変わった時に呼び出されます</summary>
    public void OnChangeedKeyboardLayout(int aDummy = 0) {
        if (m_state != STATE.MENU) { return; }
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

    /// <summary>ゲームスタートボタンが押された時に呼び出されます</summary>
    public void OnClickGameStart() {
        if (m_state != STATE.MENU) { return; }
        m_canvasMenu.enabled = false;
        InitGame();
        ChangeState(STATE.COUNTDOWN);
    }

    #endregion


    #region 【COUNTDOWN】メソッド
    /// <summary>【COUNTDOWN】更新処理</summary>
    private void UpdateCountDown() {
        switch (m_step) {
            case 0:
                m_textCountDown.text = "3";
                m_timer = new System.Timers.Timer(3000 - 1);
                m_timer.AutoReset = false;
                m_timer.Enabled = true;
                m_timer.Elapsed += OnTimerTick;
                m_timer.Start();
                m_canvasCountdown.enabled = true;
                m_step++;
                break;
        }
    }

    /// <summary>【COUNTDOWN】タイマー更新イベント</summary>

    private void OnTimerTick(System.Object aSource, ElapsedEventArgs e) {
        m_textCountDown.text = e.SignalTime.Second.ToString();
    }
    #endregion


    #region 【GAME】メソッド
    /// <summary>【GAME】更新処理</summary>
    private void InitGame() {
        
    }

    /// <summary>【GAME】更新処理</summary>
    private void UpdateGame() {
        Debug.Log("GAME");
    }
    #endregion


    #region 【RESULT】メソッド
    /// <summary>【RESULT】更新処理</summary>
    private void UpdateResult() {
        Debug.Log("RESULT");
    }
    #endregion


    #region 内部メソッド

    /// <summary>TypeModuleの初期化処理</summary>
    void SetupTypeModule() {
        m_tp = GetComponent<TypeModule>();
        m_tp.Mode = TypeModule.MODE.COPY;
        m_tp.IsRun = false;
        //m_tp.AddEventListenerOnChange(TMOnChange);
        OnChangeedKeyboardLayout();
    }

    /// <summary>状態を変更します</summary>
    /// <param name="aNext">次の状態</param>
    void ChangeState(STATE aNext) {
        m_state = aNext;
        m_step = 0;
    }
    #endregion


    #region インスペクタメンバ
    public Dropdown m_dropDownInputLayout;

    [Space(10)]
    public Canvas m_canvasMenu;
    public Canvas m_canvasCountdown;

    [Space(10)]
    public Text m_textCountDown;
    #endregion


    #region メンバ
    private TypeModule m_tp;
    private STATE m_state = STATE.MENU;
    private int m_step = 0;

    private System.Timers.Timer m_timer;
    #endregion 
}
