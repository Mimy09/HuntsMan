using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_Call : EHSEditor {
    public e_UI active_type;

    public void Call () {
        __event<e_UI>.InvokeEvent(this, active_type, true);
    }
}
