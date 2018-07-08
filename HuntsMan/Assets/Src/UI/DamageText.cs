using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageText : MonoBehaviour {

    public float speed;
    public float distance;

    public float damage;

    private TextMesh text;
    private Vector3 newPos;
    private Vector3 oldPos;

    private bool active;

    private void Start() {
        text = GetComponent<TextMesh>();
        newPos = transform.position + (transform.up * distance);
        oldPos = transform.position;
    }

    public void SetDamage(float damage, Color color) {
        active = true;
        transform.position = oldPos;
        text.text = "-" + damage.ToString();
        text.color = color;
    }

    void Update () {
        if (active == false) return;

        transform.position = Vector3.Lerp(transform.position, newPos, Time.deltaTime * speed);
        Color c = text.color;
        c.a = Mathf.Lerp(c.a, -0.1f, Time.deltaTime * speed);
        if (c.a < 0) { c.a = 0; active = false; }
        text.color = c;

    }
}
