using UnityEditor;

[CustomEditor(typeof(EHSEditor), true)]
[CanEditMultipleObjects]
public class ESHEditor_ : Editor {

    private EHSEditor editor {
        get { return (EHSEditor)target; }
    }

    public override void OnInspectorGUI() {

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("EHS Help", EditorStyles.boldLabel);

        if (editor.methodRasied) {
            EditorGUILayout.LabelField("Has raised method:\ttrue");
        } else {
            EditorGUILayout.LabelField("Has raised method:\tfalse");
        }
        EditorGUILayout.LabelField("Events Invoked:\t" + editor.eventsInvoked);

        EditorGUILayout.BeginVertical(EditorStyles.textArea);

        AddEvents();

        for (int i = 0; i < 5; i++) {
            if (i < editor.msg.Count) {
                EditorGUILayout.LabelField(editor.msg[i]);
                continue;
            }
            EditorGUILayout.LabelField("");
        }
        EditorGUILayout.EndVertical();

        base.DrawDefaultInspector();
    }

    private void OnEnable() {
        editor.WantRepaint += this.Repaint;
    }

    private void OnDisable() {
        editor.WantRepaint -= this.Repaint;
    }

    public void AddEvents() {
        if (__eventSystem.events.Count > 0) {
            string[] t;
            t = __eventSystem.GetEvent(editor.ToString());
            if (t.Length > 0) {
                editor.msg.Add(t[1]);
                string type = t[1].Split(' ')[0];
                if (type == "RAISE") {
                    editor.methodRasied = true;
                } else if (type == "INVOKE") {
                    editor.eventsInvoked++;
                }
                if (editor.msg.Count > 5) {
                    editor.msg.RemoveAt(0);
                }
                AddEvents();
            }
        }
    }
}
