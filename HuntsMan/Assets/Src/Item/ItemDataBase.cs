using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;


public class ItemDataBase : MonoBehaviour {
    public static List<Weapon> weapons = new List<Weapon>();
    public static List<Ability> abilities = new List<Ability>();

    private void Awake() {
        // Weapons
        // name, desc, id, actionPoints, damage, critDamage, critChance, Type
        weapons.Add(new Weapon(new Item(
            "Weapon_test_1",    // Name
            "",                 // Description
            weapons.Count,      // ID
            2),                 // Action Points
            5,                  // Damage
            1.5f,               // CritDamage
            20,                 // CritChance
            5,                  // Range
            Weapon.Item_Type.Melee // Type
            ));

        // Abilities
        // name, desc, id, actionPoints, damage, critDamage, critChance, Type
        abilities.Add(new Ability(new Item(
            "Ability_test_1",   // Name
            "",                 // Description
            abilities.Count,    // ID
            5),                 // Action Points
            20,                 // Damage
            2,                  // CritDamage
            20,                 // CritChance
            20,                 // Range
            Ability.Ability_Type.SingleTarget, // Type

            // Ability function
            (Unit unit, Unit other, Ability ability) => {
                if (Vector3.Distance(unit.transform.position, other.transform.position) > ability.range) {
                    Debug.Log("Too far away!");
                    return false;
                }

                
                RaycastHit hit;
                if (Physics.Raycast(unit.transform.position + unit.transform.up, (other.transform.position + other.transform.up) - (unit.transform.position + unit.transform.up), out hit)) {
                    Debug.DrawLine(unit.transform.position + unit.transform.up, hit.point, Color.red, 5);

                    if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Unit")) {
                        Debug.DrawLine(unit.transform.position + unit.transform.up, other.transform.position + other.transform.up, Color.gray, 5);

                        float r = Random.value;

                        float damage = ability.damage;
                        if (r <= ability.critChance / 100) {
                            damage *= ability.critDamage;
                            Debug.Log("Crit!");
                        }

                        other.health -= damage;
                        return true;
                    }
                } else {
                    Debug.DrawLine(unit.transform.position + unit.transform.up, other.transform.position + other.transform.up, Color.green, 5);

                    float damage = ability.damage;
                    if (Random.value >= ability.critChance / 100) {
                        damage *= ability.critDamage;
                        Debug.Log("Crit!");
                    }

                    other.health -= ability.damage;

                    return true;
                }
                return false;
            }));


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
