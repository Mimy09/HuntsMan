using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;


public class ItemDataBase : MonoBehaviour {
    public static List<Weapon> weapons = new List<Weapon>();
    public static List<Ability> abilities = new List<Ability>();

    public enum EffectType {
        None,
        FireBall,
        Arrow,
        IceBall
    }

    private void Awake() {
        // ################################################################## Weapons
        // name, desc, id, actionPoints, damage, critDamage, critChance, Type
        weapons.Add(new Weapon(new Item(
            "Weapon_test_1",        // Name
            "",                     // Description
            weapons.Count,          // ID
            2),                     // Action Points
            5,                      // Damage
            1.5f,                   // CritDamage
            20,                     // CritChance
            5,                      // Range
            Weapon.Item_Type.Melee, // Type

            // Weapon function
            (Unit unit, Unit other, Weapon weapon) => {
                return Weapon_Default(unit, other, weapon, EffectType.None);
            }));

        weapons.Add(new Weapon(new Item(
            "Melee",                // Name
            "Default Melee Weapon", // Description
            weapons.Count,          // ID
            3),                     // Action Points
            35,                     // Damage
            1.5f,                   // CritDamage
            10,                     // CritChance
            3,                      // Range
            Weapon.Item_Type.Melee, // Type

            // Weapon function
            (Unit unit, Unit other, Weapon weapon) => {
                return Weapon_Default(unit, other, weapon, EffectType.None);
            }));

        weapons.Add(new Weapon(new Item(
            "Magic",                // Name
            "Default Magic Weapon", // Description
            weapons.Count,          // ID
            2),                     // Action Points
            25,                     // Damage
            2.0f,                   // CritDamage
            30,                     // CritChance
            13,                     // Range
            Weapon.Item_Type.Magic, // Type
            
            // Weapon function
            (Unit unit, Unit other, Weapon weapon) => {
                return Weapon_Default(unit, other, weapon, EffectType.FireBall);
            }));

        weapons.Add(new Weapon(new Item(
            "Ranged",               // Name
            "Default Ranged Weapon",// Description
            weapons.Count,          // ID
            2),                     // Action Points
            20,                     // Damage
            1.5f,                   // CritDamage
            20,                     // CritChance
            17,                     // Range
            Weapon.Item_Type.Ranged,// Type

            // Weapon function
            (Unit unit, Unit other, Weapon weapon) => {
                return Weapon_Default(unit, other, weapon, EffectType.Arrow);
            }));


        weapons.Add(new Weapon(new Item(
            "Melee_Human",          // Name
            "Default Melee Weapon", // Description
            weapons.Count,          // ID
            3),                     // Action Points
            20,                     // Damage
            1.5f,                   // CritDamage
            10,                     // CritChance
            3,                      // Range
            Weapon.Item_Type.Melee, // Type

            // Weapon function
            (Unit unit, Unit other, Weapon weapon) => {
                return Weapon_Default(unit, other, weapon, EffectType.None);
            }));

        weapons.Add(new Weapon(new Item(
            "Magic_Human",          // Name
            "Default Magic Weapon", // Description
            weapons.Count,          // ID
            2),                     // Action Points
            20,                     // Damage
            2.0f,                   // CritDamage
            20,                     // CritChance
            13,                     // Range
            Weapon.Item_Type.Magic, // Type

            // Weapon function
            (Unit unit, Unit other, Weapon weapon) => {
                return Weapon_Default(unit, other, weapon, EffectType.FireBall);
            }));

        weapons.Add(new Weapon(new Item(
            "Ranged_Human",         // Name
            "Default Ranged Weapon",// Description
            weapons.Count,          // ID
            2),                     // Action Points
            15,                     // Damage
            1.5f,                   // CritDamage
            15,                     // CritChance
            17,                     // Range
            Weapon.Item_Type.Ranged,// Type

            // Weapon function
            (Unit unit, Unit other, Weapon weapon) => {
                return Weapon_Default(unit, other, weapon, EffectType.Arrow);
            }));

        // ################################################################## Abilities
        // name, desc, id, actionPoints, damage, critDamage, critChance, Type
        abilities.Add(new Ability(new Item(
            "Ability_test_1",       // Name
            "",                     // Description
            abilities.Count,        // ID
            -1),                    // Action Points
            0,                      // Damage
            0,                      // CritDamage
            0,                      // CritChance
            0,                      // Range
            0,                      // CoolDown
            Ability.Ability_Type.SingleTarget, // Type

            // Ability function
            (Unit unit, Unit other, Ability ability) => {
                return true;
            }));

        abilities.Add(new Ability(new Item(
            "Ability_AOE",          // Name
            "",                     // Description
            abilities.Count,        // ID
            -1),                    // Action Points
            20,                     // Damage
            2,                      // CritDamage
            20,                     // CritChance
            20,                     // Range
            3,                      // CoolDown
            Ability.Ability_Type.AOE,   // Type

            // Ability function
            (Unit unit, Unit other, Ability ability) => {
                return Ability_AOE(unit, other, ability, EffectType.IceBall, 7);
            }));

    }

    void LoadEffect(EffectType effectType, Transform unit, Transform target) {
        switch (effectType) {
            case EffectType.None: break;
            case EffectType.Arrow: {
                    GameObject gm = Resources.Load(Helper.Resource.Arrow_effect_prefab) as GameObject;
                    if (gm != null) {
                        GameObject effect = Instantiate(gm, unit.position + unit.up, Quaternion.identity);
                        effect.GetComponent<Target>().targetTransform = target;
                    }
                }
                break;
            case EffectType.FireBall: {
                    GameObject gm = Resources.Load(Helper.Resource.Fireball_effect_prefab) as GameObject;
                    if (gm != null) {
                        GameObject effect = Instantiate(gm, unit.position + unit.up, Quaternion.identity);
                        effect.GetComponent<Target>().targetTransform = target;
                    }
                }
                break;
            case EffectType.IceBall: {
                    GameObject gm = Resources.Load(Helper.Resource.Iceball_effect_prefab) as GameObject;
                    if (gm != null) {
                        GameObject effect = Instantiate(gm, unit.position + unit.up, Quaternion.identity);
                        effect.GetComponent<Target>().targetTransform = target;
                    }
                }
                break;
        }

    }

    bool Weapon_Default(Unit unit, Unit other, Weapon weapon, EffectType effectType) {
        if (Vector3.Distance(unit.transform.position, other.transform.position) > weapon.range) {
            Debug.Log("Too far away!");
            return false;
        }


        RaycastHit hit;
        if (Physics.Raycast(unit.transform.position + unit.transform.up, (other.transform.position + other.transform.up) - (unit.transform.position + unit.transform.up), out hit)) {
            Debug.DrawLine(unit.transform.position + unit.transform.up, hit.point, Color.red, 5);

            if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Unit")) {
                Debug.DrawLine(unit.transform.position + unit.transform.up, other.transform.position + other.transform.up, Color.gray, 5);

                LoadEffect(effectType, unit.transform, other.transform);

                float r = Random.value;

                float damage = weapon.damage;
                if (r <= weapon.critChance / 100) {
                    damage *= weapon.critDamage;
                    Debug.Log("Crit!");
                }

                other.damageText.SetDamage(damage, Color.red);

                other.health -= damage;
                return true;
            }
        }
        return false;
    }

    bool Ability_AOE(Unit unit, Unit other, Ability ability, EffectType effectType, float AOE_Range) {
        if (Vector3.Distance(unit.transform.position, other.transform.position) > ability.range) {
            Debug.Log("Too far away!");
            return false;
        }


        RaycastHit hit;
        if (Physics.Raycast(unit.transform.position + unit.transform.up, (other.transform.position + other.transform.up) - (unit.transform.position + unit.transform.up), out hit)) {
            Debug.DrawLine(unit.transform.position + unit.transform.up, hit.point, Color.red, 5);

            if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Unit")) {
                Debug.DrawLine(unit.transform.position + unit.transform.up, other.transform.position + other.transform.up, Color.gray, 5);

                LoadEffect(effectType, unit.transform, other.transform);

                List<GameObject> UnitsInAOE = new List<GameObject>();

                // Add team one to AOE damage area
                List<GameObject> Teams = Manager.instance.GetTeam(1);
                for (int i = 0; i < Teams.Count; i++) {
                    if (Vector3.Distance(other.transform.position, Teams[i].transform.position) < AOE_Range) {
                        UnitsInAOE.Add(Teams[i]);
                    }
                }
                // Add team two to AOE damage area
                Teams = Manager.instance.GetTeam(2);
                for (int i = 0; i < Teams.Count; i++) {
                    if (Vector3.Distance(other.transform.position, Teams[i].transform.position) < AOE_Range) {
                        UnitsInAOE.Add(Teams[i]);
                    }
                }

                // Damage all units in AOE range
                for (int i = 0; i < UnitsInAOE.Count; i++) {
                    float r = Random.value;

                    float damage = ability.damage;
                    if (r <= ability.critChance / 100) {
                        damage *= ability.critDamage;
                    }

                    UnitsInAOE[i].GetComponent<Unit>().damageText.SetDamage(damage, Color.red);
                    UnitsInAOE[i].GetComponent<Unit>().health -= damage;
                }
                return true;
            }
        }
        return false;
    }

    
    bool Weapon_Default(Unit unit, Unit other, Ability ability) {
        return false;
    }

    public static Weapon FindWeapon(int id) {
        return weapons[id];
    }

    public static Weapon FindWeapon(string name) {
        for (int i = 0; i < weapons.Count; i++) {
            if (weapons[i].name == name) return weapons[i];
        } return null;
    }

    public static Ability FindAbility(int id) {
        return abilities[id];
    }

    public static Ability FindAbility(string name) {
        for (int i = 0; i < weapons.Count; i++) {
            if (abilities[i].name == name) return abilities[i];
        }
        return null;
    }
}
