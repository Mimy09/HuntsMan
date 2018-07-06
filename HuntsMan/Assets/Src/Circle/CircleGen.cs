using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Helper;

public class CircleGen : MonoBehaviour {

    static GameObject circle;

    private void Awake()
    {
        circle = Resources.Load(Resource.Circle_Prefab) as GameObject;
    }

    public static GameObject GenCircle(Vector3 pos, float range)
    {
        GameObject temp = Instantiate(circle, pos + new Vector3(0, 0.2f, 0), Quaternion.Euler(new Vector3(90, 0, 0)));
        temp.transform.localScale = new Vector3(range, range, range);
        return temp;
    }
}
