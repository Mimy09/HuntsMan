using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModelRotation : MonoBehaviour {
    public float speed;
    public enum Direction { UP, RIGHT, BACK };
    public Direction Dir;

    void Update () {
        switch (Dir) {
            case Direction.UP:
                transform.Rotate(Vector3.up * (Time.deltaTime * speed));
                break;
            case Direction.RIGHT:
                transform.Rotate(Vector3.right * (Time.deltaTime * speed));
                break;
            case Direction.BACK:
                transform.Rotate(Vector3.back * (Time.deltaTime * speed));
                break;
        }
	}
}
