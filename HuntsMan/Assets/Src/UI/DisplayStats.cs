using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DisplayStats : MonoBehaviour {

    TextMesh text;
    Text canvas_text;

    Unit unit;

    public enum StatType {
        UNIT,
        WEAPON,
        ABILITY,
    } public StatType statType;

    public enum UIType {
        TEXT3D,
        CANVAS,
    }
    public UIType uiType;

    public bool useParent;

    void Start () {
        switch (uiType) {
            case UIType.TEXT3D:
                text = this.GetComponent<TextMesh>();
                break;
            case UIType.CANVAS:
                canvas_text = this.GetComponent<Text>();
                break;
        }

        if (useParent) {
            unit = transform.parent.GetComponent<Unit>();
        }
	}
	
	void Update () {
        if (!useParent) {
            unit = Manager.instance.GetSelected();
            if (unit == null) {
                switch (statType) {
                    case StatType.UNIT:
                        if (uiType == UIType.TEXT3D)
                            text.text = "Unit";
                        else if (uiType == UIType.CANVAS)
                            canvas_text.text = "Unit";
                        break;
                    case StatType.WEAPON:
                        if (uiType == UIType.TEXT3D)
                            text.text = "Weapon";
                        else if (uiType == UIType.CANVAS)
                            canvas_text.text = "Weapon";
                        break;
                    case StatType.ABILITY:
                        if (uiType == UIType.TEXT3D)
                            text.text = "Ability";
                        else if (uiType == UIType.CANVAS)
                            canvas_text.text = "Ability";
                        break;
                }
                return;
            }
        }

        switch (statType) {
            case StatType.UNIT:
                if (uiType == UIType.TEXT3D)
                    text.text = unit.GetStats();
                else if (uiType == UIType.CANVAS)
                    canvas_text.text = unit.GetStats();
                break;
            case StatType.WEAPON:
                if (uiType == UIType.TEXT3D)
                    text.text = unit.GetWeaponStats();
                else if (uiType == UIType.CANVAS)
                    canvas_text.text = unit.GetWeaponStats();
                break;
            case StatType.ABILITY:
                if (uiType == UIType.TEXT3D)
                    text.text = unit.GetAbilityStats();
                else if (uiType == UIType.CANVAS)
                    canvas_text.text = unit.GetAbilityStats();
                break;
        }
    }
}
