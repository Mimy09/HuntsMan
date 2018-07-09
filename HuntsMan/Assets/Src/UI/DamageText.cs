using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageText : MonoBehaviour {

    public float speed;
    public float distance;

    public float damage;

    private TextMesh text;
    private float newPos;
    private float oldPos;

    private bool active;

    private void Start() {
        text = GetComponent<TextMesh>();
        newPos = transform.position.y + distance;
        oldPos = transform.position.y;
    }

    public void SetDamage(float damage, Color color) {
        active = true;

        Vector3 pos = transform.position;
        pos.y = oldPos;
        transform.position = pos;

        text.text = "-" + damage.ToString();
        text.color = color;
    }

    void Update () {
        if (active == false) return;
        Vector3 pos = transform.position;
        pos.y = Mathf.Lerp(pos.y, newPos, Time.deltaTime * speed);
        transform.position = pos;

        Color c = text.color;
        c.a = Mathf.Lerp(c.a, -0.1f, Time.deltaTime * speed);
        if (c.a < 0) { c.a = 0; active = false; }
        text.color = c;

    }
}
