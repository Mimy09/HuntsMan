using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Circle : MonoBehaviour {
	public static GameObject GenCercle (Vector3 pos, float range) {
        GameObject gm = Instantiate(Resources.Load(Helper.Resource.Circle_prefab), pos + Vector3.up, Quaternion.Euler(90, 0, 0)) as GameObject;
        gm.transform.localScale = new Vector3(range * 2, range * 2, range * 2);
        return gm;
    }
}
