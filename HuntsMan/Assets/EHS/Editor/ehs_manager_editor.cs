using global::UnityEditor;
using global::UnityEngine;

[CustomEditor(typeof(EHS_Manager))]
public class ehs_manager_editor: Editor {

    private EHS_Manager manager {
        get { return (EHS_Manager)target; }
    }

    public override void OnInspectorGUI () {
        EditorGUILayout.HelpBox("The EHS is used to dynamically and efficiently create events and invoke them during runtime using event delegates.", MessageType.Info);
        
        EditorGUILayout.Space();

        GUILayout.BeginVertical(EditorStyles.textArea);
        for (int index = 0; index < EHS_Manager.MESSAGE_COUNT; index++) {
            if (EHS_Manager.Messages[index] == null) {
                EditorGUILayout.LabelField("");
            } else {
                EditorGUILayout.LabelField(EHS_Manager.Messages[index]);
            }
        }
        GUILayout.EndVertical();
        if (GUILayout.Button("Clear")) { EHS_Manager.ClearMessages(); }
        GUI.enabled = true;
    }

    private void OnEnable () {
        EHS_Manager.WantRepaint += this.Repaint;
    }

    private void OnDisable () {
        EHS_Manager.WantRepaint -= this.Repaint;
    }
}