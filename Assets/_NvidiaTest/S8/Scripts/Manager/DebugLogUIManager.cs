using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DebugLogUIManager : MonoBehaviour
{
    private static Text _text;
    private static List<string> _logs = new List<string>();

    void Start()
    {
        _text = transform.Find("Canvas/Panel/Text").GetComponent<Text>();
        _text.text = "sasas\nasasas";
    }

    /*
        ex.)
        DebugLogUIManager.Log("add log here");
    */
    public static void Log(string str){
        if(_logs.Count >= 10 )_logs.RemoveAt(9);
        _logs.Insert(0,str);

        if(_text)_text.text = "";
        for( int i = 0; i < _logs.Count ; i++){
            if(_text)_text.text  += _logs[i] + "\n";
        }
    }

}
