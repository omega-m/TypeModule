using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextCmpUpdator : MonoBehaviour {
    // Start is called before the first frame update
    void Start() {
        m_typeModule = GetComponent<TypeModule>();
        m_typeModule.AddEventListenerOnInput(OnInput);
        m_typeModule.AddEventListenerOnCorrect(OnCorrect);
        m_typeModule.AddEventListenerOnMiss(OnMiss);
        m_typeModule.AddEventListenerOnComplete(OnComplete);
        m_typeModule.AddEventListenerOnSetup(OnSetup);

        m_audioSounce = GetComponent<AudioSource>();

        m_texts.Add("、。・いくよー！");
        m_texts.Add("まみむめもやもやなきみも");
        m_texts.Add("Are you Ready? じゅんびはいい？");
        m_texts.Add("すきっぷ&くらっぷでいちにーさんはい！");
        m_texts.Add("ぽじてぃぶだんすたいむ");

        
        SetNextTargetStr();
    }

    // Update is called once per frame
    void Update() {

    }
    private void SetNextTargetStr() {
        if (m_textId >= m_texts.Count) { return; }
        m_typeModule.TargetStr = m_texts[m_textId];
        m_textId++;
    }
    public void OnSetup(CopyInputCheckerResults aResult) {
        Debug.Log("onSetup");
        m_targetText.text =
            m_doneColor + aResult.StrDone + m_colorEnd +
            m_currentColor + aResult.StrCurrent + m_colorEnd +
            aResult.StrYet;
        m_midText.text =
        m_doneColor + aResult.StrDoneRaw + m_colorEnd +
        m_currentColor + aResult.StrCurrentRaw + m_colorEnd +
        aResult.StrYetRaw;
        m_correctText.text = "Correct:" + aResult.CorrectNum;
        m_correctCharText.text = "Correct(Ch):" + aResult.CorrectCharNum;
        m_missText.text = "Miss:" + aResult.MissNum;
        other.text = "CCh:" + aResult.PrevCorrectChar + " MCh:" + aResult.PrevMissChar;
    }
    public void OnInput(CopyInputCheckerResults aResult) {
        Debug.Log("onInput");
        m_correctText.text = "Correct:" + aResult.CorrectNum;
        m_correctCharText.text = "Correct(Ch):" + aResult.CorrectCharNum;
        m_missText.text = "Miss:" + aResult.MissNum;
        other.text = "CCh:" + aResult.PrevCorrectChar + " MCh:" + aResult.PrevMissChar;

    }
    public void OnCorrect(CopyInputCheckerResults aResult) {
        Debug.Log("onCorrect");
        m_targetText.text =
            m_doneColor + aResult.StrDone + m_colorEnd +
            m_currentColor + aResult.StrCurrent + m_colorEnd +
            aResult.StrYet;
        m_midText.text =
         m_doneColor + aResult.StrDoneRaw + m_colorEnd +
         m_currentColor + aResult.StrCurrentRaw + m_colorEnd +
         aResult.StrYetRaw;

        m_audioSounce.PlayOneShot(m_typeSound);
    }
    public void OnMiss(CopyInputCheckerResults aResult) {
        Debug.Log("onMiss");
        m_targetText.text =
           m_doneColor + aResult.StrDone + m_colorEnd +
           m_missColor + aResult.StrCurrent + m_colorEnd +
           aResult.StrYet;
        m_midText.text =
         m_doneColor + aResult.StrDoneRaw + m_colorEnd +
         m_missColor + aResult.StrCurrentRaw + m_colorEnd +
         aResult.StrYetRaw;

        m_audioSounce.PlayOneShot(m_missSound);

    }
    public void OnComplete(CopyInputCheckerResults aResult) {
        Debug.Log("onComplete");
        Debug.Log("onMiss");
        m_targetText.text =
           m_doneColor + aResult.StrDone + m_colorEnd +
           m_missColor + aResult.StrCurrent + m_colorEnd +
           aResult.StrYet;
        m_midText.text =
         m_doneColor + aResult.StrDoneRaw + m_colorEnd +
         m_missColor + aResult.StrCurrentRaw + m_colorEnd +
         aResult.StrYetRaw;
        SetNextTargetStr();
    }


    public Text m_targetText;
    public Text m_midText;
    public Text m_correctText;
    public Text m_correctCharText;
    public Text m_missText;

    public Text other;

    private const string m_doneColor = "<color=#ADE789>";
    private const string m_missColor = "<color=#B2642D>";
    private const string m_currentColor = "<color=#1A1A1A>";
    private const string m_colorEnd = "</color>";

    private int m_textId = 0;
    public List<string> m_texts = new List<string>();
    private TypeModule m_typeModule;

    public AudioClip m_missSound;
    public AudioClip m_typeSound;
    private AudioSource m_audioSounce;

}
