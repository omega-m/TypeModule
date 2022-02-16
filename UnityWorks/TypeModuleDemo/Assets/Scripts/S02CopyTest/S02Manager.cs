using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using tm;
using UnityEngine.UI;

public class S02Manager : MonoBehaviour {

    #region ���\�b�h
    /// <summary>�V�[���uS00Menu�v���Ăяo��</summary>
    public void BackToMenu() {
        SceneManager.LoadScene("S00Menu");
    }


    /// <summary>�L�[�{�[�h�z�񂪕ς�������ɌĂяo����܂�</summary>
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
    #endregion


    #region Unity���ʏ���
    void Start() {
        //Initialize TypeModule
        m_tp = GetComponent<TypeModule>();
        m_tp.Mode = TypeModule.MODE.COPY;
        //m_tp.AddEventListenerOnChange(TMOnChange);
        OnChangeedKeyboardLayout();
    }

    void Update() {

    }
    #endregion


    #region �C���X�y�N�^�����o
    public Dropdown m_dropDownInputLayout;

    #endregion


    #region �����o
    private TypeModule m_tp;
    #endregion 
}
