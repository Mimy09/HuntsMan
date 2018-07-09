using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using UnityEngine;

[System.Serializable]
public class Unit : MonoBehaviour {
    // Unit Variables
    public float health;
    public int actionPoints;
    private int maxActionPoionts;
    
    // Selected Variables
    [HideInInspector]
    public bool selected;
    [HideInInspector]
    public bool targeted;
    [HideInInspector]
    public bool weaponSelected;
    [HideInInspector]
    public int abilitySelected;

    // Text
    [HideInInspector]
    public DamageText damageText;

    // Weapon Variables
    public string weapon;
    public Weapon equiptedWeapon;

    public List<string> ability;
    public List<Ability> abilities = new List<Ability>();

    // Mesh Variables
    protected MeshRenderer meshRenderer;
    protected Material mat;

    // Movement Variables
    protected GameObject grid;
    protected NavMeshAgent navMeshAgent;
    protected LineRenderer lineRenderer;
    protected NavMeshPath path;
    protected bool canMove = true;
    protected bool createGrid = false;

    // Out-liner
    protected cakeslice.Outline graphicsOutLine;

    // Grid
    GridGen.GridInfo gridInfo;

    // Team Variables
    private int teamID;
    private bool isTeamTurn;

    public virtual void Start() {
        canMove = true;
        createGrid = false;

        if (graphicsOutLine == null) {
            graphicsOutLine = this.transform.GetChild(0).gameObject.GetComponent<cakeslice.Outline>();
            graphicsOutLine.eraseRenderer = true;
        }

        if (damageText == null) {
            damageText = this.transform.GetChild(1).gameObject.GetComponent<DamageText>();
        }

        if (meshRenderer == null) {
            meshRenderer = this.transform.GetChild(0).gameObject.GetComponent<MeshRenderer>();
        }
        if (mat == null) {
            mat = meshRenderer.material;
        }
        

        maxActionPoionts = actionPoints;

        navMeshAgent = this.gameObject.AddComponent<NavMeshAgent>();
        navMeshAgent.radius = 0.4f;

        lineRenderer = this.gameObject.AddComponent<LineRenderer>();
        if (lineRenderer != null) {
            lineRenderer.material = new Material(Shader.Find("Sprites/Default")) { color = Color.cyan };
            lineRenderer.startWidth = 0.1f;
            lineRenderer.endWidth = 0.1f;
            lineRenderer.startColor = Color.cyan;
            lineRenderer.endColor = Color.cyan;
        }
    }

    public void SetDissolve(float amount) {
        if (mat != null) {
            mat.SetFloat("_DissolveAmount", amount);
        }
    }

    public float GetDissolve() {
        if (mat != null) {
            return mat.GetFloat("_DissolveAmount");
        } return -1;
    }

    public void SetTeam(int ID) {
        teamID = ID;
    }

    public virtual void Update() {
        isTeamTurn = Manager.instance.IsTeamTurn(teamID);

        if (targeted) {
            graphicsOutLine.color = 3;
            graphicsOutLine.eraseRenderer = false;
        } else if (selected) {
            if (teamID == 1) {
                graphicsOutLine.color = 1;
            } else if (teamID == 2) {
                graphicsOutLine.color = 3;
            }
            graphicsOutLine.eraseRenderer = false;
        } else {
            switch (teamID) {
                case 1:
                    if (isTeamTurn) {
                        graphicsOutLine.eraseRenderer = false;
                    } else {
                        graphicsOutLine.eraseRenderer = true;
                    }
                    graphicsOutLine.color = 0;
                    break;
                case 2:
                    if (isTeamTurn) {
                        graphicsOutLine.eraseRenderer = false;
                    }
                    else {
                        graphicsOutLine.eraseRenderer = true;
                    }
                    graphicsOutLine.color = 2;
                    break;
            }
        }

        if (isDead()) {
            if (GetDissolve() < 1) {
                SetDissolve(GetDissolve() + Time.deltaTime);
            } else {
                Manager.instance.RemoveFromTeam(gameObject, teamID);
                Destroy(this.gameObject);
            }
        }
    }

    public virtual void Move(Vector3 position, int actionPoints) {
        if (canMove == false) return;
        canMove = false;

        if (this.actionPoints < actionPoints) return;
        this.actionPoints -= actionPoints;

        createGrid = true;
    }

    public void ResetActionPoints() {
        actionPoints = maxActionPoionts;
    }

    public virtual void CreateGrid() {
        if (canMove == false) return;
        canMove = false;
        Realign();
        ClearGrid();
        //grid = GenerateGrid.CreateGrid(actionPoints, this.transform.position);
        gridInfo = GridGen.GenPoints(actionPoints, actionPoints, this.transform.position, navMeshAgent);

        if (gridInfo.gridPoints != null || gridInfo.gridPoints.Count > 0)
            grid = GridGen.CreateGrid(gridInfo, this.transform.position, actionPoints);
    }

    public virtual void ClearGrid() {
        Destroy(grid);
    }

    public virtual void Attack(Unit other) {
        if (equiptedWeapon.actionPoints > actionPoints) return;
        if (equiptedWeapon.UseWeapon(this, other)) {
            actionPoints -= equiptedWeapon.actionPoints;
            ClearGrid();
            CreateGrid();
        }
    }

    public virtual void UseAbility(int abilityID, Unit other) {
        if (abilities[abilityID].actionPoints > actionPoints) return;
        if (abilities[abilityID].UseAbility(this, other)) {
            actionPoints -= abilities[abilityID].actionPoints;
            ClearGrid();
            CreateGrid();
        }
    }

    public void Realign() {
        transform.position = new Vector3(
            Mathf.Round(transform.position.x),
            transform.position.y,
            Mathf.Round(transform.position.z)
            );
    }

    public bool isDead() {
        return health <= 0 ? true : false;
    }
    public string GetStats() {
        string msg = "";
        msg += "Health: " + health + "\n";
        msg += "Action Points: " + actionPoints + "\n";
        return msg;
    }

    public string GetWeaponStats() {
        string msg = "";
        msg += "Weapon: \n" + equiptedWeapon.GetStats() + "\n";
        return msg;
    }

    public string GetAbilityStats() {
        string msg = "";
        msg += "Ability: \n" + abilities[0].GetStats() + "\n";
        return msg;
    }
}
