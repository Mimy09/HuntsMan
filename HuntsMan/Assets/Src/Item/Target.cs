using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Target : MonoBehaviour {

    public Transform targetTransform;

    private void Update() {
        if (targetTransform == null) return;

        transform.LookAt(targetTransform.position + targetTransform.up);
        transform.position += transform.forward * Time.deltaTime * 30;

        if (Vector3.Distance(transform.position, targetTransform.position + targetTransform.up) < 0.5f) {
            Destroy(gameObject);
        }
    }
}
