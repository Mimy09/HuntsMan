public class UI_Manager : EHSEditor {
	void Start () {
        __event<e_UI>.InvokeEvent(this, e_UI.MENU, true);
	}
}
