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
        /// <summary>スタートボタンを表示している画面</summary>
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

        m_audioSource = GetComponent<AudioSource>();
        m_timer = GetComponent<Timer>();
        m_gameParams.m_stopwatch = GetComponent<Stopwatch>();

        m_canvasMenu.enabled = false;
        m_canvasCountdown.enabled = false;
        m_canvasGame.enabled = false;

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
    public void OnClickStart() {
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
                m_canvasCountdown.enabled = true;
                m_textCountDown.text = "3";
                m_timer.StartTimer(3.0f - 0.001f);   //UIが一時だけ4になるのを防ぐ為
                m_audioSource.PlayOneShot(m_audioCountDown);
                m_step++;
                break;
            case 1:
                if (m_timer.IsCompleted) {
                    m_step++;
                } else {
                    int nowTime = (int)Math.Ceiling(m_timer.RemainTime);
                    m_textCountDown.text = nowTime.ToString();
                }
                break;
            case 2:
                m_canvasCountdown.enabled = false;
                ChangeState(STATE.GAME);
                break;
        }
    }
    #endregion


    #region 【GAME】メソッド
    /// <summary>【GAME】更新処理(【MENU】の開始ボタンが押された時に呼び出します)</summary>
    private void InitGame() {
        var p = m_gameParams;
        p.Clear();
        p.m_odaiSet = OdaiSetFactory.Create();
        p.m_stopwatch = GetComponent<Stopwatch>();
        p.m_stopwatch.ResetStopwatch();
    }

    /// <summary>【GAME】更新処理</summary>
    private void UpdateGame() {
        //Debug.Log("GAME");

        var p = m_gameParams;

        switch (m_step) {
            case 0:
                m_canvasGame.enabled = true;
                p.m_stopwatch.StartStopwatch();
                m_tp.TargetStr = p.m_odaiSet[p.m_odaiIdx];

                m_textCorrect.enabled = true;
                m_textMiss.enabled = true;
                m_textCorrect.text = p.m_totalCorrectKey.ToString();
                m_textMiss.text = p.m_totalMissKey.ToString();
                m_textAccuracy.text = "100.0";

                m_tp.IsRun = true;

                m_otucare.enabled = false;
                m_btnToMenu.SetActive(false);
                m_step++;
                break;
            case 1:
                m_textTime.text = p.m_stopwatch.Time.ToString("F1");
                m_textKeysec.text = p.KeySec.ToString("F1");
                break;
        }
    }

    /// <summary>【GAME】文字列が新たにセットされた時のイベント</summary>
    /// <param name="res">入力の状態</param>
    private void TMOnSetup(CopyInputCheckerResults res) {
        //Debug.Log("TMOnSetup");
        var p = m_gameParams;
        SetInputText(true);
        m_textOjaiIdx.text = String.Format("{0:00}", p.m_odaiIdx + 1)+ " / " + String.Format("{0:00}", p.m_odaiSet.Count);
    }

    /// <summary>【GAME】キーが打たれた時のイベント</summary>
    /// <param name="res">入力の状態</param>
    private void TMOnInput(CopyInputCheckerResults res) {
        //Debug.Log("TMOnInput");
    }

    /// <summary>【GAME】キーが正しく打たれた時のイベント</summary>
    /// <param name="res">入力の状態</param>
    private void TMOnCorrect(CopyInputCheckerResults res) {
        //Debug.Log("TMOnCorrect");
        var p = m_gameParams;
        p.m_totalCorrectKey++;
        m_textCorrect.text = p.m_totalCorrectKey.ToString();
        m_textAccuracy.text = p.AccuracyRate.ToString("F1");
        SetInputText(true);
        m_audioSource.PlayOneShot(m_audioCorrect);
    }

    /// <summary>【GAME】ミスタイプされた時のイベント</summary>
    /// <param name="res">入力の状態</param>
    private void TMOnMiss(CopyInputCheckerResults res) {
        //Debug.Log("TMOnMiss");
        var p = m_gameParams;
        p.m_totalMissKey++;
        m_textMiss.text = p.m_totalMissKey.ToString();
        m_textAccuracy.text = p.AccuracyRate.ToString("F1");
        SetInputText(false);
        m_audioSource.PlayOneShot(m_audioMiss);
    }

    /// <summary>【GAME】文字列を打ち切った時のイベント</summary>
    /// <param name="res">入力の状態</param>
    private void TMOnComplete(CopyInputCheckerResults res) {
        //Debug.Log("TMOnComplete");
        var p = m_gameParams;
        p.m_odaiIdx++;
        if (p.m_odaiIdx >= p.m_odaiSet.Count) {
            m_tp.IsRun = false;
            p.m_stopwatch.PauseStopwatch();
            ChangeState(STATE.RESULT);
        } else {
            m_tp.TargetStr = p.m_odaiSet[p.m_odaiIdx];
        }
    }

    /// <summary>【GAME】現在の状態の入力文字列をセット</summary>
    /// <param name="aIsCorrect">前回の入力が正しく入力されたかどうか</param>
    private void SetInputText(bool aIsCorrect) {
        string tmp = "";
        tmp = (BASE_COLOR + m_tp.StrDone + COLOR_END);
        if (aIsCorrect) {
            tmp += (CURRENT_COLOR + m_tp.StrCurrent + COLOR_END);
        } else {
            tmp += (MISS_COLOR + m_tp.StrCurrent + COLOR_END);
        }
        tmp += (BASE_COLOR + m_tp.StrYet + COLOR_END);
        m_textInput.text = tmp;

        tmp = (BASE_COLOR + m_tp.StrDoneRaw + COLOR_END);
        if (aIsCorrect) {
            tmp += (CURRENT_COLOR + m_tp.StrCurrentRaw + COLOR_END);
        } else {
            tmp += (MISS_COLOR + m_tp.StrCurrentRaw + COLOR_END);
        }
        tmp += (BASE_COLOR + m_tp.StrYetRaw + COLOR_END);
        m_textInputRaw.text = tmp;
    }
    #endregion


    #region 【RESULT】メソッド
    /// <summary>【RESULT】更新処理</summary>
    private void UpdateResult() {
        switch (m_step) {
            case 0:
                m_textCorrect.enabled = false;
                m_textMiss.enabled = false;

                m_otucare.enabled = true;
                m_btnToMenu.SetActive(true);
                m_step++;
                break;
            case 1:
                 break;
        }
    }

    /// <summary>戻るボタンが押された時に呼び出されます</summary>
    public void OnClickToMenu() {
        if (m_state != STATE.RESULT) { return; }
        m_canvasGame.enabled = false;
        ChangeState(STATE.MENU);
    }
    #endregion


    #region 内部メソッド
    /// <summary>TypeModuleの初期化処理</summary>
    void SetupTypeModule() {
        m_tp = GetComponent<TypeModule>();
        m_tp.Mode = TypeModule.MODE.COPY;
        m_tp.IsRun = false;
        m_tp.AddEventListenerOnSetup(TMOnSetup);
        m_tp.AddEventListenerOnInput(TMOnInput);
        m_tp.AddEventListenerOnCorrect(TMOnCorrect);
        m_tp.AddEventListenerOnMiss(TMOnMiss);
        m_tp.AddEventListenerOnComplete(TMOnComplete);
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
    public Canvas m_canvasGame;

    [Space(10)]
    public Text m_textCountDown;
    public AudioClip m_audioCountDown;

    [Space(10)]
    public Text m_textInput;
    public Text m_textInputRaw;
    public Text m_textOjaiIdx;
    public Text m_textTime;
    public Text m_textKeysec;
    public Text m_textCorrect;
    public Text m_textMiss;
    public Text m_textAccuracy;

    public AudioClip m_audioCorrect;
    public AudioClip m_audioMiss;

    [Space(10)]
    public Text m_otucare;
    public GameObject m_btnToMenu;
    #endregion


    #region メンバ
    private TypeModule m_tp;
    private AudioSource m_audioSource;

    private STATE m_state = STATE.MENU;
    private int m_step = 0;

    private Timer m_timer;
    private GameParams m_gameParams = new GameParams();
    #endregion 


    #region 定数
    static private readonly string BASE_COLOR =     "<color=#A19A9A>";
    static private readonly string MISS_COLOR =     "<color=#FF5145>";
    static private readonly string CURRENT_COLOR =  "<color=#FFFFFF>";
    static private readonly string COLOR_END =      "</color>";
    #endregion
}



/// <summary>ゲーム時用のパラメータ</summary>
public class GameParams {


    #region メソッド
    /// <summary>内部データをクリア</summary>
    public void Clear() {
        m_odaiSet = new List<string>();
        m_odaiIdx = 0;
        m_totalCorrectKey = 0;
        m_totalMissKey = 0;
        m_stopwatch.ResetStopwatch();
    }
    #endregion


    #region プロパティ
    /// <summary>正確率(0-100で返却)</summary>
    public float  AccuracyRate {
        get { return (float)m_totalCorrectKey / (float)(m_totalCorrectKey + m_totalMissKey) * 100; }
    }

    /// <summary>key/sec</summary>
    public float KeySec {
        get { return m_totalCorrectKey / m_stopwatch.Time; }
    }

    #endregion

    #region メンバ
    /// <summary>打つ対象の文字列セット</summary>
    public List<string>     m_odaiSet;
    /// <summary>現在打っている文字列のidx</summary>
    public int              m_odaiIdx;
    /// <summary>ストップウォッチ、時間測定用</summary>
    public Stopwatch        m_stopwatch;
    /// <summary>正しく打てた数</summary>
    public int              m_totalCorrectKey;
    /// <summary>ミスタイプした数</summary>
    public int              m_totalMissKey;
    #endregion
}