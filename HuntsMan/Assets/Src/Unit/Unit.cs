using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using UnityEngine;

[System.Serializable]
public class Unit : MonoBehaviour {
    public float health;
    public int actionPoints;
    public Texture2D icon;
    public bool selected;
    public bool targeted;

    public Weapon equiptedWeapon;
    public List<Ability> abilities = new List<Ability>();

    

    protected GameObject grid;
    protected NavMeshAgent navMeshAgent;
    protected MeshFilter meshFilter;
    protected LineRenderer lineRenderer;
    protected NavMeshPath path;
    protected bool canMove = true;
    protected bool createGrid = false;

    protected cakeslice.Outline graphicsOutLine;
    protected PlayerCamera pc;

    private int teamID;
    private bool isTeamTurn;
    private int maxActionPoionts;


    public virtual void Start() {
        canMove = true;
        createGrid = false;

        if (graphicsOutLine == null) {
            graphicsOutLine = this.transform.GetChild(0).gameObject.GetComponent<cakeslice.Outline>();
            graphicsOutLine.eraseRenderer = true;
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

        pc = Camera.main.GetComponent<PlayerCamera>();
    }

    public void SetTeam(int ID) {
        teamID = ID;
    }

    public virtual void Update() {
        isTeamTurn = pc.IsTeamTurn(teamID);

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
        grid = GenerateGrid.CreateGrid(actionPoints, this.transform.position);
    }

    public virtual void ClearGrid() {
        Destroy(grid);
    }

    public virtual void Attack(Unit other) {
        if (equiptedWeapon.actionPoints > actionPoints) return;

        switch (equiptedWeapon.itemType) {
            case Weapon.Item_Type.Magic:
            case Weapon.Item_Type.Melee:
            case Weapon.Item_Type.Ranged:
                if (Vector3.Distance(this.transform.position, other.transform.position) > equiptedWeapon.range) return;
                float damage = equiptedWeapon.damage;
                if (Random.value <= equiptedWeapon.critChance / 100) {
                    damage *= equiptedWeapon.critDamage;
                }
                other.health -= damage;
                actionPoints -= equiptedWeapon.actionPoints;
                ClearGrid();
                CreateGrid();

                break;
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

        msg += "\nWeapon: \n" + equiptedWeapon.GetStats() + "\n";
        msg += "Ability: \n" + abilities[0].GetStats() + "\n";

        return msg;
    }
}
