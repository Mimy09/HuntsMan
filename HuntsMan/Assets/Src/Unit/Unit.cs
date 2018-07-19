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
    
    // Graphics Objects
    public List<GameObject> graphics = new List<GameObject>();

    // Out-liner
    public cakeslice.Outline graphicsOutLine;

    // Text
    public DamageText damageText;

    // Animator
    public Animator animator;


    // Selected Variables
    [HideInInspector]
    public bool selected;
    [HideInInspector]
    public bool targeted;
    [HideInInspector]
    public bool weaponSelected;
    //[HideInInspector]
    public int abilitySelected = -1;

    
    // Weapon Variables
    public string weapon;
    public Weapon equiptedWeapon;

    public List<string> ability;
    public List<int> abilityCooldown = new List<int>();
    public List<Ability> abilities = new List<Ability>();

    // Mesh Variables
    protected MeshRenderer meshRenderer;
    protected List<Material> mat = new List<Material>();
    public Renderer Renderer { get; private set; }

    // Movement Variables
    protected GameObject grid;
    protected NavMeshAgent navMeshAgent;
    protected LineRenderer lineRenderer;
    protected NavMeshPath path;
    protected bool canMove = true;
    protected bool createGrid = false;

    // Circle
    GameObject circle;

    // Grid
    protected GridGen.GridInfo gridInfo;

    // Team Variables
    private int teamID;
    private bool isTeamTurn;

    public virtual void Start() {
        canMove = true;
        createGrid = false;
        abilitySelected = -1;
        weaponSelected = false;

        if (graphicsOutLine != null) {
            graphicsOutLine.eraseRenderer = true;
        }

        for (int i = 0; i < graphics.Count; i++) {
            mat.Add(graphics[i].GetComponent<Renderer>().material);
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
        if (mat.Count > 0) {
            for (int i = 0; i < mat.Count; i++) {
                mat[i].SetFloat("_DissolveAmount", amount);
            }
        }
    }

    public float GetDissolve() {
        if (mat.Count > 0) {
            return mat[0].GetFloat("_DissolveAmount");
        } return 0;
    }

    public void SetTeam(int ID) {
        teamID = ID;
    }

    public virtual void Update() {
        isTeamTurn = Manager.instance.IsTeamTurn(teamID);

        if (abilitySelected != -1) {
            if (abilityCooldown[abilitySelected] < abilities[abilitySelected].cooldown) {
                abilitySelected = -1;
            }
        }

        if (targeted) {
            graphicsOutLine.color = 2;
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

        if (selected == true) {
            switch (abilitySelected) {
                case 0:
                    Unit g = Camera.main.GetComponent<PlayerCamera>().SelectCharacter();
                    if (g != null) {
                        if (circle == null) circle = Circle.GenCercle(g.transform.position, 13);
                    } else {
                        if (circle != null) Destroy(circle);
                    }
                    break;
                default:
                    if (circle != null) Destroy(circle);
                    break;
            }
        }

        if (isDead()) {
            animator.SetBool("Dead", true);

            if (GetDissolve() < 2) {
                SetDissolve(GetDissolve() + (Time.deltaTime/4));
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
        for (int i = 0; i < abilityCooldown.Count; i++) {
            abilityCooldown[i]++;
        }
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
        if (abilityCooldown[abilityID] < abilities[abilityID].cooldown) return;
        if (abilities[abilityID].UseAbility(this, other)) {
            ClearGrid();
            CreateGrid();
            abilityCooldown[abilityID] = 0;
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
        if (equiptedWeapon != null)
            msg += "Weapon: \n" + equiptedWeapon.GetStats() + "\n";
        return msg;
    }

    public string GetAbilityStats() {
        string msg = "";
        if (abilities.Count > 0) {
            msg += "Ability: \n" + abilities[0].GetStats() + "\n";
            msg += "Cool Down: " + abilityCooldown[0] + "\n";
        }
        return msg;
    }
}
