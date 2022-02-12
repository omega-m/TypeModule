using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextUpdator : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        m_tp = GetComponent<TypeModule>();
        m_tp.AddEventListenerOnChange(onChange);
        m_tp.AddEventListenerOnInput(onInput);
    }

    // Update is called once per frame
    void Update()
    {
    }

    private void OnGUI() {
        //test
        if(Event.current.type == EventType.KeyDown  && Event.current.keyCode == KeyCode.F1) {
            m_tp.Clear();
        }
        if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.F2) {
            m_tp.BackSpace();
        }
        if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.F3) {
            m_tp.Enter();
        }
    }

    private void onInput(InputEmulatorResults res) {
        //Debug.Log("onInput");
    }
    private void onChange(InputEmulatorResults res) {
        //Debug.Log("onChange");
        textInput.text = res.Str;
        textInputRaw.text = res.StrRaw;
    }

    private TypeModule m_tp = null;
    public Text textInput;
    public Text textInputRaw;
}
