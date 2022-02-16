using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class S00Manager : MonoBehaviour
{
    #region メソッド
    /// <summary>シーン「S01InputTest」を開始</summary>
    public void StartS01() {
        SceneManager.LoadScene("S01InputTest");
    }

    /// <summary></summary>
    public void StartS02() {
        Debug.Log("s02");
    }
    #endregion 


    #region Unity共通処理
    void Start() {

    }

    void Update() {

    }
    #endregion 

}
