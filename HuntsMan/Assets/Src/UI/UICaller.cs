using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UICaller : MonoBehaviour {

    public bool isChecking;

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
        Manager.instance.DestroyCircle();
        Manager.instance.UseAbility(ID);
    }

    public void UseWeapon() {
        isChecking = false;
        Manager.instance.DestroyCircle();
        Manager.instance.UseWeapon();
    }

    public void ClearTargeted() {
        Manager.instance.DestroyCircle();

        if (isChecking == true) {
            Manager.instance.ClearTargeted();
            Manager.instance.ClearSelectedItems();
            isChecking = false;
        }
    }
}
