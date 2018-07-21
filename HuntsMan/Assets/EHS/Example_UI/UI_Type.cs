using UnityEngine;

public class UI_Type : EHSEditor {
    public e_UI type;

    private void Awake () {
        __event<e_UI>.Raise(this, EventHandle);
    }

    private void EventHandle (object s, __eArg<e_UI> e) {
        if (e.arg == type) {
            for (int i = 0; i < transform.childCount; i++)
                transform.GetChild(i).gameObject.SetActive(true);
            this.gameObject.SetActive(true);
        } else {
            for (int i = 0; i < transform.childCount; i++)
                transform.GetChild(i).gameObject.SetActive(false);
            this.gameObject.SetActive(false);
        }
    }
}
