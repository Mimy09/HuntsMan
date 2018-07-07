using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour {

    public Transform Pivot;
    public Transform LookAt;

    public Vector2 sensitivity;
    public float followSpeed;
    public float moveSpeed;

    public float maxRotitionSpeed;

    public float minimumAngle;
    public float maximumAngle;

    public float minimumZoom;
    public float maximumZoom;

    public bool isDarging;

    float m_rotationY;
    Vector3 LookAtPos;

    private void Awake() {
        LookAtPos = new Vector3();

        Pivot.transform.LookAt(LookAt);
    }

    public Unit SelectCharacter() {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 100.0f, 1 << LayerMask.NameToLayer("Unit"))) {
            return hit.transform.gameObject.GetComponent<Character>();
        }

        return null;
    }

    public GameObject SelectGrid() {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 100.0f)) {
            if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Grid"))
                return hit.transform.gameObject;
        }

        return null;
    }

    public void CamLookAt(Vector3 pos) {
        LookAtPos = new Vector3(pos.x, 0, pos.z);
    }

    void Update() {
        // Look at pivot
        transform.LookAt(LookAt);

        // move to pivot position
        this.LookAt.position = Vector3.Lerp(this.LookAt.position, LookAtPos, Time.deltaTime * followSpeed);

        // Drag rotation around function
        if (Input.GetAxis("Mouse X") > 0.001f || Input.GetAxis("Mouse X") < -0.001f || Input.GetAxis("Mouse Y") > 0.001f || Input.GetAxis("Mouse Y") < -0.001f) {
            if (Input.GetMouseButton(0)) {
                LookAt.Rotate(0f, Mathf.Clamp(Input.GetAxis("Mouse X"), -maxRotitionSpeed, maxRotitionSpeed) * sensitivity.x * Time.deltaTime * followSpeed, 0f);

                m_rotationY += Mathf.Clamp(Input.GetAxis("Mouse Y"), -maxRotitionSpeed, maxRotitionSpeed) * sensitivity.y;
                m_rotationY = Mathf.Clamp(m_rotationY, minimumAngle, maximumAngle);

                LookAt.localEulerAngles = new Vector3(-m_rotationY, LookAt.localEulerAngles.y, 0);

                this.transform.position = Vector3.Lerp(this.transform.position, Pivot.position, Time.deltaTime * followSpeed);

                isDarging = true;
            } else {
                isDarging = false;
            }
        }
        // Zoom In
        if (Input.GetAxis("Mouse ScrollWheel") > 0) {
            if (Vector3.Distance(LookAt.position, Pivot.position) > minimumZoom) {
                Pivot.transform.position += Pivot.transform.forward;
            }
        }

        // Zoom Out
        if (Input.GetAxis("Mouse ScrollWheel") < 0) {
            if (Vector3.Distance(LookAt.position, Pivot.position) < maximumZoom) {
                Pivot.transform.position -= Pivot.transform.forward;
            }
        }

        // Drag move around function
        if (Input.GetMouseButton(2)) {
            if (Input.GetAxis("Mouse X") > 0.1f || Input.GetAxis("Mouse X") < -0.1f) {
                LookAt.transform.position += LookAt.transform.right * -Input.GetAxis("Mouse X") * moveSpeed;
            }
            if (Input.GetAxis("Mouse Y") > 0.1f || Input.GetAxis("Mouse Y") < -0.1f) {
                LookAt.transform.position += new Vector3(LookAt.transform.forward.x, 0, LookAt.transform.forward.z) * -Input.GetAxis("Mouse Y") * moveSpeed;
            }

            LookAtPos = LookAt.transform.position;
            this.transform.position = Vector3.Lerp(this.transform.position, Pivot.position, Time.deltaTime * 20);
        } else {
            this.transform.position = Vector3.Lerp(this.transform.position, Pivot.position, Time.deltaTime * followSpeed);
        }
    }
}
