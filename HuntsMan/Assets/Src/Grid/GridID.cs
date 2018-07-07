using UnityEngine;

public class GridID : MonoBehaviour {
    public int ID = 0;

    private void Start() {
        RaycastHit hit;

        if (Physics.Raycast(transform.position, -transform.up, out hit, 100.0f)) {
            gameObject.GetComponent<MeshRenderer>().enabled = true;
        } else {
            Destroy(gameObject);
        }

    }
}