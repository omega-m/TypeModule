using UnityEngine;
using Candlelight;

/// <summary>タイマークラス</summary>
public class Timer : MonoBehaviour{

    #region メソッド
    /// <summary>タイマーをスタート</summary>
    /// <param name="aInitTime">タイマーの初期値、RemainTimeにそのまま代入されます。</param>
    public void StartTimer(float aInitTime) {
        RemainTime = aInitTime;
        StartTimer();
    }

    /// <summary>
    /// <para>タイマーをスタート</para>
    /// <para>RemainTimeをセットしてから呼び出してください</para>
    /// <para>PauseTimer()を使用した後にこちらを呼び出すと、ポーズした時点での時間から再開することができます。</para>
    /// </summary>
    public void StartTimer() {
        IsRun = true;
    }

    /// <summary>タイマーを一時停止</summary>
    public void PauseTimer() {
        IsRun = false;
    }

    /// <summary>タイマーをリセット</summary>
    public void ResetTimer() {
        RemainTime = 0.0f;
        IsRun = false;
        IsCompleted = false;
    }
    #endregion


    #region インスペクターフィールド    
    [Tooltip("タイマーの残り時間")]
    [SerializeField, PropertyBackingField("RemainTime")] private float m_remainTime = 0.0f;
    /// <summary>タイマーの残り時間</summary>
    public float RemainTime { 
        get { return m_remainTime; } 
        set { m_remainTime = value; } 
    }

    /// <summary>タイマーが稼働中か</summary>
    [Tooltip("タイマーが稼働中か")]
    [SerializeField, PropertyBackingField("IsRun")] private bool m_isRun = false;
    public bool IsRun { 
        get { return m_isRun; } 
        set { m_isRun = value; } 
    }
    #endregion


    #region フィールド    
    /// <summary>タイマーのカウントが終了したかどうか</summary>
    public bool IsCompleted { get; private set; } = false;
    #endregion


    #region Unity共通処理
    void Start(){}

    void Update(){
        if (IsRun) {
            RemainTime -= Time.deltaTime;
            if (RemainTime <= 0.0f) {
                ResetTimer();
                IsCompleted = true;
            }
        }
    }
    #endregion
}
