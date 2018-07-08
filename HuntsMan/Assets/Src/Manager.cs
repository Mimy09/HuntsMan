using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Manager : MonoBehaviour {

    public static Manager instance;
    public static PlayerCamera playerCamera;

    List<GameObject> team1 = new List<GameObject>();
    List<GameObject> team2 = new List<GameObject>();

    Unit selected;
    public List<Unit> targeted;
    public int teamTurn;

    private bool canSelect;
    private bool gameStart;

    void Awake () {
        // Set instance
        if (instance == null) {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this) {
            Destroy(gameObject);
        }

        if (instance != null) Initialize();
    }

    private void Initialize() {
        // Set Start Team
        teamTurn = 1;
        canSelect = true;

        // Set camera
        if (playerCamera == null)
            playerCamera = Camera.main.GetComponent<PlayerCamera>();


        targeted = null;
    }

    public int GetWinStat() {
        if (team1.Count == 0) {
            return 2;
        } else if (team2.Count == 0) {
            return 1;
        }
        return -1;
    }

    public int GetTurnStats() {
        return teamTurn;
    }

    public void LookAtTeam() {
        if (teamTurn == 1) {
            playerCamera.CamLookAt(team1[Random.Range(0, team1.Count - 1)].transform.position);
        } else if (teamTurn == 2) {
            playerCamera.CamLookAt(team2[Random.Range(0, team2.Count - 1)].transform.position);
        }
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

    public void RemoveFromTeam(GameObject character, int team) {
        if (team == 1) {
            for (int i = 0; i < team1.Count; i++) {
                if (character == team1[i]) {
                    team1.Remove(character);
                    break;
                }
            }
        } else if (team == 2) {
            for (int i = 0; i < team2.Count; i++) {
                if (character == team2[i]) {
                    team2.Remove(character);
                    break;
                }
            }
        }
    }

    public bool IsInTeam(GameObject character, int team) {
        if (team == 1) {
            for (int i = 0; i < team1.Count; i++) {
                if (character == team1[i]) return true;
            }
            return false;
        } else if (team == 2) {
            for (int i = 0; i < team2.Count; i++) {
                if (character == team2[i]) return true;
            }
            return false;
        }
        return false;
    }

    public bool IsTeamTurn(int teamID) {
        return teamTurn == teamID ? true : false;
    }

    public Unit GetSelected() {
        return selected;
    }

    public void ClearSelected() {
        if (selected == null) return;

        // Clear Selected
        selected.ClearGrid();
        selected.selected = false;
        selected.weaponSelected = false;
        selected.abilitySelected = -1;
        selected = null;
    }

    public void ClearTargeted() {
        if (targeted == null) return;

        // Clear Target
        for (int i = 0; i < targeted.Count; i++) {
            targeted[i].targeted = false;
            targeted[i].weaponSelected = false;
            targeted[i].abilitySelected = -1;
        }

        targeted.Clear();
        targeted = null;
    }

    public void UseAbility(int ID) {
        // Select Attacks
        if (selected != null) {

            // Ability Attack
            selected.weaponSelected = false;
            selected.abilitySelected = ID;

            GetAllUnitsWithinRange(selected.abilities[ID].range);
        }
    }

    public void UseWeapon() {
        // Select Attacks
        if (selected != null) {

            // Weapon Attack
            selected.abilitySelected = -1;
            selected.weaponSelected = true;

            GetAllUnitsWithinRange(selected.equiptedWeapon.range);
        }
    }

    public void EndTurn() {
        if (teamTurn == 1) {
            teamTurn = 2;
            for (int i = 0; i < team2.Count; i++)
                team2[i].GetComponent<Character>().ResetActionPoints();

            playerCamera.CamLookAt(team2[Random.Range(0, team2.Count - 1)].transform.position);
        } else if (teamTurn == 2) {
            teamTurn = 1;
            for (int i = 0; i < team1.Count; i++)
                team1[i].GetComponent<Character>().ResetActionPoints();

            playerCamera.CamLookAt(team1[Random.Range(0, team1.Count - 1)].transform.position);
        }

        ClearSelected();
        ClearTargeted();
    }

    void GetAllUnitsWithinRange(float range) {
        if (selected == null) return;

        ClearTargeted();
        targeted = new List<Unit>();

        if (teamTurn == 1) {
            for (int i = 0; i < team2.Count; i++) {
                // Check range
                if (Vector3.Distance(selected.transform.position, team2[i].transform.position) < range) {
                    // Check line of sight
                    RaycastHit hit;
                    if (Physics.Raycast(selected.transform.position + selected.transform.up, (team2[i].transform.position + team2[i].transform.up) - (selected.transform.position + selected.transform.up), out hit)) {
                        if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Unit")) {
                            Debug.DrawLine(selected.transform.position + selected.transform.up, team2[i].transform.position + team2[i].transform.up, Color.green, 5);

                            // Add to targeted
                            targeted.Add(team2[i].GetComponent<Character>());
                            targeted[targeted.Count - 1].targeted = true;
                        }
                    }
                }
            }
        } else if (teamTurn == 2) {
            for (int i = 0; i < team1.Count; i++) {
                // Check range
                if (Vector3.Distance(selected.transform.position, team1[i].transform.position) < range) {
                    // Check line of sight
                    RaycastHit hit;
                    if (Physics.Raycast(selected.transform.position + selected.transform.up, (team1[i].transform.position + team1[i].transform.up) - (selected.transform.position + selected.transform.up), out hit)) {
                        if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Unit")) {
                            Debug.DrawLine(selected.transform.position + selected.transform.up, team1[i].transform.position + team1[i].transform.up, Color.green, 5);

                            // Add to targeted
                            targeted.Add(team1[i].GetComponent<Character>());
                            targeted[targeted.Count - 1].targeted = true;
                        }
                    }
                }

            }
        }

        if (targeted.Count == 0) targeted = null;
    }

    private void LateUpdate() {
        if (gameStart == false) {
            gameStart = true;
            LookAtTeam();
        }

        // Use Weapon
        if (Input.GetKeyDown(KeyCode.Q)) {
            UseWeapon();
        }

        // Ability Attack (0)
        if (Input.GetKeyDown(KeyCode.W)) {
            UseAbility(0);
        }

        // Switch team
        if (Input.GetKeyDown(KeyCode.Space)) {
            EndTurn();
        }

        // Quit Game
        if (Input.GetKeyDown(KeyCode.Escape)) {
            Application.Quit();
        }


        // --- Everything after this will not be called if the cursor is over the UI ---
        if (EventSystem.current.IsPointerOverGameObject()) return;

        // If cam is dragging, stop player mouse selection input
        if (Input.GetMouseButton(0)) {
            if (playerCamera.isDarging) {
                canSelect = false;
            } else {
                if (canSelect == false) {
                    canSelect = true;
                    return;
                }
            }
        }
        if (!canSelect) return;

        // Select Unit
        if (Input.GetMouseButtonUp(0)) {
            Unit temp = playerCamera.SelectCharacter();
            
            if (temp != null && IsInTeam(temp.gameObject, teamTurn)) {

                if (selected == null) {
                    selected = temp;

                    if (selected != null) {
                        selected.CreateGrid();
                        selected.selected = true;
                        playerCamera.CamLookAt(selected.transform.position);
                    }

                } else {
                    // If another unit is selected
                    if (temp != null) {

                        ClearSelected();
                        ClearTargeted();

                        selected = temp;
                        selected.selected = true;
                        selected.CreateGrid();

                        playerCamera.CamLookAt(selected.transform.position);
                    }
                }
            } else if (temp != null && !IsInTeam(temp.gameObject, teamTurn)) {

                // If clicked on enemy thats is targeted
                // Attack Target
                if (targeted != null && selected != null) {
                    Unit target = playerCamera.SelectCharacter();

                    if (target != null) {
                        // Check if attack target is in targeted range
                        for (int i = 0; i < targeted.Count; i++) {
                            if (target == targeted[i]) {

                                // Attack using ether weapon or ability
                                if (selected.weaponSelected) {
                                    selected.Attack(target);
                                } else if (selected.abilitySelected != -1) {
                                    selected.UseAbility(selected.abilitySelected, target);
                                }
                                return;
                            }
                        }
                    }
                }
            } else {
                GameObject grid = playerCamera.SelectGrid();

                // If clicked on grid
                if (grid != null) {
                    selected.Move(grid.transform.position, grid.GetComponent<GridID>().ID);
                    playerCamera.CamLookAt(grid.transform.position);
                    ClearTargeted();
                }
                // If clicked on nothing
                else {
                    if (selected != null) selected.ClearGrid();

                    ClearSelected();
                    ClearTargeted();
                }
            }
        }


        // ################################################################# 
        // ############ Thinking of deleting this functionality ############
        // #################################################################

        // Attack Target
        if (Input.GetMouseButtonDown(1) && targeted != null && selected != null) {
            Unit target = playerCamera.SelectCharacter();

            if (target != null) {
                // Check if attack target is in targeted range
                for (int i = 0; i < targeted.Count; i++) {
                    if (target == targeted[i]) {

                        // Attack using ether weapon or ability
                        if (selected.weaponSelected) {
                            selected.Attack(target);
                            selected.weaponSelected = false;
                        } else if (selected.abilitySelected != -1) {
                            selected.UseAbility(selected.abilitySelected, target);
                            selected.abilitySelected = -1;
                        }
                        return;
                    }
                }
            }
        }


        
    }
}
