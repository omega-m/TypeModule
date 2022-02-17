using UnityEngine;
using Candlelight;


/// <summary>ストップウォッチクラス</summary>
public class Stopwatch : MonoBehaviour{

    #region メソッド
    /// <summary>ストップウォッチをスタート</summary>
    public void StartStopwatch() {
        IsRun = true;
    }

    /// <summary>ストップウォッチを一時停止</summary>
    public void PauseStopwatch() {
        IsRun = false;
    }

    /// <summary>ストップウォッチをリセット</summary>
    public void ResetStopwatch() {
        Time = 0.0f;
        IsRun = false;
    }
    #endregion


    #region インスペクターフィールド    
    [Tooltip("現在の経過時間")]
    [SerializeField, PropertyBackingField("Time")] private float m_time = 0.0f;
    /// <summary>現在の経過時間</summary>
    public float Time {
        get { return m_time; }
        set { m_time = value; }
    }

    /// <summary>ストップウォッチが稼働中か</summary>
    [Tooltip("ストップウォッチが稼働中か")]
    [SerializeField, PropertyBackingField("IsRun")] private bool m_isRun = false;
    public bool IsRun {
        get { return m_isRun; }
        set { m_isRun = value; }
    }
    #endregion


    #region Unity共通処理
    void Start() {}

    void Update() {
        if (IsRun) {
            m_time += UnityEngine.Time.deltaTime;
        }
    }
    #endregion
}
