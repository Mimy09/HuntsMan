using UnityEngine;
using System.Collections;

public class Console : MonoBehaviour {
    
    static private string text = "";
    public UnityEngine.UI.Text consoleText;
    public UnityEngine.UI.Text errorDisplay;
    private int errorCount;

    public string message;
    public string stacktrace;

    void OnEnable() {
        Application.logMessageReceived += HandleLog;
    }
    void OnDisable() {
        Application.logMessageReceived -= HandleLog;
    }

    void OnApplicationQuit() {
        if (Debug.isDebugBuild) {
            System.IO.File.WriteAllText(Application.dataPath + "/DebugLog.txt",
                "################################## " + System.DateTime.Now + "\n"
                + text + "\n\n");
        }
    }

    void HandleLog(string logString, string stackTrace, LogType type) {
        if (!Debug.isDebugBuild) return;

        message = logString;
        stacktrace = stackTrace;

        if (type == LogType.Error) {
            errorCount++;
            if (errorDisplay != null) {
                errorDisplay.text = "Errors (" + errorCount + ")";
                errorDisplay.color = Color.red;
            }
        }

        text = text + logString + "\n" + stackTrace + "\n";
    }

    private void Start() {
        if (!Debug.isDebugBuild) {
            Destroy(gameObject);   
        }

        Debug.Log("Init Console");
    }

    private void Update() {
        if (consoleText != null) {
            consoleText.text = text;
        }
    }
}