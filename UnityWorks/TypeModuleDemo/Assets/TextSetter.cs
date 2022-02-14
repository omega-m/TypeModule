using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextSetter : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        var tp = GetComponent<TypeModule>();
        tp.AddEventListenerOnChange(onChange);
        tp.AddEventListenerOnInput(onInput);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void onInput(InputEmulatorResults res) {
        Debug.Log("onInput");
    }
    void onChange(InputEmulatorResults res) {
        Debug.Log("onChange");
        input.text = res.Str;
        inputRaw.text = res.StrRaw;
    }

    public Text input;
    public Text inputRaw;
}
