using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UICaller : MonoBehaviour {

    bool isChecking;

	public void CheckAbility(int ID) {
        isChecking = true;
        Manager.instance.UseAbility(ID);
    }

    public void CheckWeapon() {
        isChecking = true;
        Manager.instance.UseWeapon();
    }

    public void UseAbility(int ID) {
        isChecking = false;
        Manager.instance.UseAbility(ID);
    }

    public void UseWeapon() {
        isChecking = false;
        Manager.instance.UseWeapon();
    }

    public void ClearTargeted() {
        if (isChecking == false) return;
        Manager.instance.ClearTargeted();

        isChecking = false;
    }
}
