using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class EHSWindow : EditorWindow {
    Vector2 scroll_pos;
    List<GameObject> go_list = new List<GameObject>();
    List<Rect> rect_list = new List<Rect>();

    private const int list_length = 5;
    private int tab;

    // styles
    GUIStyle options_style;
    GUIStyle button_style;


    [MenuItem("Window/Event System Logger")]
    public static void ShowWindow() {
        EditorWindow.GetWindow(typeof(EHSWindow));
    }

    public void OnFocus() {
        go_list.Clear();
        rect_list.Clear();

        
    }

    private void Update() {
        __eventSystem.RaiseOnEventInvoked(OnEventInvoked);

        Repaint();
    }

    private void OnDestroy() {
        __eventSystem.ConsumeOnEventInvoked(OnEventInvoked);
    }

    void OnGUI() {
        // options_style
        if (options_style == null) {
            options_style = new GUIStyle(GUI.skin.label);
            options_style.richText = true;
        }

        // button_style
        if (button_style == null) {
            button_style = new GUIStyle(GUI.skin.button);
        }

        tab = GUILayout.Toolbar(tab, new string[] { "Event Tracker", "Event Profiler", "Options" });
        
        switch (tab) {
            case 0:
                DisplayEventList();
                GUI.color = new Color(0.6f, 0.6f, 0.6f, 1.0f);
                if (GUILayout.Button("Clear", GUILayout.Height(50))) {
                    __eventSystem.Clear();
                }
                break;
            case 1:
                break;
            case 2:
                DisplayOptions();
                break;
            default:
                break;
        }
    }

    void DisplayEventList() {
        EditorGUILayout.Space();
        scroll_pos = EditorGUILayout.BeginScrollView(scroll_pos, "box");

        if (__eventSystem.g_events.Count == 0){
            EditorGUILayout.EndScrollView();
            return;
        }


        for (int i = 0; i < __eventSystem.g_events.Count; i++) {
            Color c = GUI.color;
            GUI.color = new Color(0.95f, 0.95f, 0.95f, 1.0f);
            if (go_list.Count == __eventSystem.g_events.Count) {
                for (int k = 0; k < Selection.gameObjects.Length; k++) {
                    if (k <= Selection.gameObjects.Length) {
                        if (Selection.gameObjects[k] == go_list[i]) {
                            GUI.color = Color.yellow;
                        }
                    }

                }
                if (go_list[i] == null) {
                    GUI.color = new Color(0.4f, 0.4f, 0.4f, 1.0f);
                }
            } else {
                UpdateGameObjects();
            }
            Rect rect;
            rect = EditorGUILayout.BeginVertical("box");

            if (rect_list.Count < __eventSystem.g_events.Count && rect != new Rect(0, 0, 0, 0)) {
                rect_list.Add(rect);
            }

            EditorGUILayout.LabelField(go_list[i] == null ? __eventSystem.g_events[i].eventName + " - disabled" : __eventSystem.g_events[i].eventName, EditorStyles.boldLabel);

            for (int j = 0; j < __eventSystem.g_events[i].eventList.Count; j++) {

                int t = 0;
                if (list_length != 0) {
                    if (j > list_length) break;
                    t = __eventSystem.g_events[i].eventList.Count > list_length ? __eventSystem.g_events[i].eventList.Count - (list_length + 1) : 0;
                }
                GUI.color = j == __eventSystem.g_events[i].eventList.Count - 1 ? new Color(0.9f, 0.9f, 0.9f, 1.0f) : new Color(0.6f, 0.6f, 0.6f, 1.0f);

                EditorGUILayout.BeginVertical("box");
                EditorGUILayout.LabelField("\t" + __eventSystem.g_events[i].eventList[j + t]);
                EditorGUILayout.EndVertical();

            }
            GUI.color = c;
            EditorGUILayout.EndVertical();

            EditorGUILayout.Space();
        }
        EditorGUILayout.EndScrollView();
        EditorGUILayout.Space();


        if (Event.current.type == EventType.MouseDown && Event.current.button == 0) {
            int i = 0;
            for (; i < rect_list.Count; i++) {
                if (rect_list[i].Contains(Event.current.mousePosition)) {
                    if (go_list.Count == __eventSystem.g_events.Count) {
                        GameObject[] gos = new GameObject[1];
                        gos[0] = go_list[i];
                        Selection.objects = gos;

                        break;
                    }
                } else {
                    GameObject[] gos = new GameObject[0];
                    Selection.objects = gos;
                }
            }
        }
    }

    void DisplayProfiler() {
        

    }

    void DisplayOptions() {
        EditorGUILayout.Space();
        scroll_pos = EditorGUILayout.BeginScrollView(scroll_pos, "box");

        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        GUILayout.Label("<b><size=24>Log Options</size></b>", options_style);
        GUILayout.Space(10);

        GUILayout.Label((__eventSystem.EventSystemOptions & (int)__eventSystemOptions.SystemOptions.logEventsInEditor)
            == (int)__eventSystemOptions.SystemOptions.logEventsInEditor ?
            "<size=16>Log Events In Editor:\t<b>True</b></size>" : "<size=16>Log Events In Editor:\tFalse</size>", options_style);

        GUILayout.Label((__eventSystem.EventSystemOptions & (int)__eventSystemOptions.SystemOptions.logEventsInBuild)
            == (int)__eventSystemOptions.SystemOptions.logEventsInBuild ?
            "<size=16>Log Events In Build:\t<b>True</b></size>" : "<size=16>Log Events In Build:\tFalse</size>", options_style);

        GUILayout.Label((__eventSystem.EventSystemOptions & (int)__eventSystemOptions.SystemOptions.logToFile)
            == (int)__eventSystemOptions.SystemOptions.logToFile ?
            "<size=16>Log Events To File:\t<b>True</b></size>" : "<size=16>Log Events To File:\tFalse</size>", options_style);

        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        GUILayout.Label("<b><size=24>Profiler Options</size></b>", options_style);


        GUILayout.Space(10);
        EditorGUILayout.EndScrollView();
    }

    private void UpdateGameObjects() {
        go_list.Clear();
        for (int i = 0; i < __eventSystem.g_events.Count; i++) {
            go_list.Add(GameObject.Find(__eventSystem.g_events[i].eventName.Split(' ')[0]));
        }
    }

    public void OnEventInvoked() {
        go_list.Clear();
        rect_list.Clear();
    }
}