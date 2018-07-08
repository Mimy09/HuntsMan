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
        TURN,
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
                    case StatType.TURN:
                        if (uiType == UIType.TEXT3D) {
                            if (Manager.instance.GetWinStat() == 1) {
                                text.text = "Monsters Win";
                                text.color = Color.blue;
                                break;
                            } else if (Manager.instance.GetWinStat() == 2) {
                                text.text = "Humans Win";
                                text.color = Color.red;
                                break;
                            }
                            if (Manager.instance.GetTurnStats() == 1) {
                                text.text = "Monsters Turn";
                                text.color = Color.blue;
                            } else if (Manager.instance.GetTurnStats() == 2) {
                                text.text = "Humans Turn";
                                text.color = Color.red;
                            }
                        } else if (uiType == UIType.CANVAS) {
                            if (Manager.instance.GetWinStat() == 1) {
                                canvas_text.text = "Monsters Win";
                                canvas_text.color = Color.blue;
                                break;
                            } else if (Manager.instance.GetWinStat() == 2) {
                                canvas_text.text = "Humans Win";
                                canvas_text.color = Color.red;
                                break;
                            }
                            if (Manager.instance.GetTurnStats() == 1) {
                                canvas_text.text = "Monsters Turn";
                                canvas_text.color = Color.blue;
                            } else if (Manager.instance.GetTurnStats() == 2) {
                                canvas_text.text = "Humans Turn";
                                canvas_text.color = Color.red;
                            }
                        }
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
            case StatType.TURN:
                if (uiType == UIType.TEXT3D) {
                    if (Manager.instance.GetWinStat() == 1) {
                        text.text = "Monsters Win";
                        text.color = Color.blue;
                        break;
                    } else if (Manager.instance.GetWinStat() == 2) {
                        text.text = "Humans Win";
                        text.color = Color.red;
                        break;
                    }
                    if (Manager.instance.GetTurnStats() == 1) {
                        text.text = "Monsters Turn";
                        text.color = Color.blue;
                    } else if (Manager.instance.GetTurnStats() == 2) {
                        text.text = "Humans Turn";
                        text.color = Color.red;
                    }
                } else if (uiType == UIType.CANVAS) {
                    if (Manager.instance.GetWinStat() == 1) {
                        canvas_text.text = "Monsters Win";
                        canvas_text.color = Color.blue;
                        break;
                    } else if (Manager.instance.GetWinStat() == 2) {
                        canvas_text.text = "Humans Win";
                        canvas_text.color = Color.red;
                        break;
                    }
                    if (Manager.instance.GetTurnStats() == 1) {
                        canvas_text.text = "Monsters Turn";
                        canvas_text.color = Color.blue;
                    } else if (Manager.instance.GetTurnStats() == 2) {
                        canvas_text.text = "Humans Turn";
                        canvas_text.color = Color.red;
                    }
                }
                break;
        }
    }
}
