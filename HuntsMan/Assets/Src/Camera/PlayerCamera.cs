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

    public UnityEngine.UI.Text infoText;
    public UnityEngine.UI.Text targetText;

    Unit selected;
    Unit secondSelected;

    float m_rotationY;
    Vector3 LookAtPos;

    List<GameObject> team1 = new List<GameObject>();
    List<GameObject> team2 = new List<GameObject>();

    public int teamTurn;

    private void Awake() {
        LookAtPos = new Vector3();

        Pivot.transform.LookAt(LookAt);

        teamTurn = 1;
    }

    public void AddToTeam(GameObject character, int team) {
        switch (team) {
            case 1:
                team1.Add(character);
                break;
            case 2:
                team2.Add(character);
                break;
        }
        
    }

    public bool IsTeamTurn(int teamID) {
        return teamTurn == teamID ? true : false;
    }

    public bool IsInTeam(GameObject character, int team) {
        if (team == 1) {
            for (int i = 0; i < team1.Count; i++) {
                if (character == team1[i]) return true;
            } return false;
        } else if (team == 2) {
            for (int i = 0; i < team2.Count; i++) {
                if (character == team2[i]) return true;
            }
            return false;
        }
        return false;
    }

    public void EndTurn()
    {
        if (teamTurn == 1) {
            teamTurn = 2;
            for (int i = 0; i < team2.Count; i++)
                team2[i].GetComponent<Character>().ResetActionPoints();

            LookAtPos = team2[0].transform.position;
        } else if (teamTurn == 2) {
            teamTurn = 1;
            for (int i = 0; i < team1.Count; i++)
                team1[i].GetComponent<Character>().ResetActionPoints();
            LookAtPos = team1[0].transform.position;
        }

        if (secondSelected != null) secondSelected.targeted = false;

        selected.ClearGrid();
        selected.selected = false;
        selected = null;
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.Space)) {
            EndTurn();
        }

        if (selected != null) infoText.text = selected.GetStats();
        else infoText.text = "";

        if (secondSelected != null && selected != null) targetText.text = secondSelected.GetStats();
        else targetText.text = "";

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
                return;
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

        // Select character
        if (Input.GetMouseButtonDown(0)) {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (selected == null) {
                if (Physics.Raycast(ray, out hit, 100.0f, 1 << LayerMask.NameToLayer("Unit"))) {
                    if (IsInTeam(hit.transform.gameObject, teamTurn)) {

                        if (secondSelected != null) secondSelected.targeted = false;
                    
                        selected = hit.transform.gameObject.GetComponent<Character>();
                        selected.selected = true;
                        selected.CreateGrid();

                        LookAtPos = hit.transform.position;
                    }

                }
            } else {
                if (Physics.Raycast(ray, out hit)) {
                    if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Unit")) {
                        if (IsInTeam(hit.transform.gameObject, teamTurn)) {
                            if (secondSelected != null) secondSelected.targeted = false;

                            selected.ClearGrid();
                            selected.selected = false;


                            selected = hit.transform.gameObject.GetComponent<Character>();
                            selected.CreateGrid();
                            selected.selected = true;

                            LookAtPos = hit.transform.position;
                        }

                    } else if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Grid")) {
                        if (secondSelected != null) secondSelected.targeted = false;

                        selected.Move(hit.transform.position, hit.transform.GetComponent<GridID>().ID);
                        LookAtPos = hit.transform.position;
                    } else {
                        if (secondSelected != null) secondSelected.targeted = false;

                        selected.ClearGrid();
                        selected.selected = false;
                        selected = null;
                    }
                }
            }
        }

        // Attack
        if (Input.GetMouseButtonDown(1) && selected != null) {
            if (secondSelected != null) secondSelected.targeted = false;

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 100.0f, 1 << LayerMask.NameToLayer("Unit"))) {
                secondSelected = hit.transform.gameObject.GetComponent<Character>();
                secondSelected.targeted = true;
            }
        }

        if (Input.GetKeyDown(KeyCode.Q)) {
            if (selected != null) {
                CircleGen.GenCircle(selected.transform.position, selected.equiptedWeapon.range * 2);
            }
            //if (secondSelected != null && selected != null) {
            //    selected.Attack(secondSelected);
            //}
        }
        if (Input.GetKeyDown(KeyCode.W)) {
            if (selected != null) {
                CircleGen.GenCircle(selected.transform.position, selected.abilities[0].range * 2);
            }
            //if (secondSelected != null && selected != null) {
            //    selected.UseAbility(0, secondSelected);
            //}
        }
    }
}
