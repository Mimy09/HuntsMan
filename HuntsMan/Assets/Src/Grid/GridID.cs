using UnityEngine;

public class GridID : MonoBehaviour {
    public int ID = 0;

    private void Start() {
        RaycastHit hit;

        if (Physics.Raycast(transform.position, -transform.up, out hit, 100.0f)) {
            gameObject.GetComponent<MeshRenderer>().enabled = true;
            transform.GetChild(0).GetComponent<TextMesh>().text = ID.ToString();
        } else {
            Destroy(gameObject);
        }

    }

    private void Update() {
        if (Camera.main.GetComponent<PlayerCamera>().SelectGrid() == this.gameObject) {
            transform.GetChild(0).gameObject.SetActive(true);
            
        } else {
            transform.GetChild(0).gameObject.SetActive(false);
        }
    }
}