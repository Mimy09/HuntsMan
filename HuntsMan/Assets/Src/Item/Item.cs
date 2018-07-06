using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Item {
    public string name;
    public string description;
    public int ID;
    public Mesh mesh;
    public Texture2D icon;
    public int actionPoints;

    public Item(string name, string desc, int ID, int actionPoints, Mesh mesh = null, Texture2D icon = null) {
        this.name = name;
        this.description = desc;
        this.ID = ID;
        this.actionPoints = actionPoints;

        if (mesh != null) this.mesh = mesh;
        if (icon != null) this.icon = icon;
    }
    public Item(Item item) {
        this.name = item.name;
        this.description = item.description;
        this.ID = item.ID;
        if (mesh != null) this.mesh = item.mesh;
        if (icon != null) this.icon = item.icon;
    }
    public Item() { ID = -1; }

    public virtual string GetStats() {
        string msg = "";
        msg += "Name: " + name + "\n";
        msg += "Description : " + description + "\n";
        msg += "ID : " + ID + "\n";
        msg += "Cost : " + actionPoints + "\n";

        return msg;
    }
}
[System.Serializable]
public class Weapon : Item {
    public float damage;
    public float critDamage;
    public float critChance;
    public float range;

    public enum Item_Type {
        Melee,
        Ranged,
        Magic,
    } public Item_Type itemType;
    public Weapon(Item item, float damage, float critDamage, float critChance, float range, Item_Type type) {
        this.name = item.name;
        this.description = item.description;
        this.ID = item.ID;
        this.actionPoints = item.actionPoints;
        if (mesh != null) this.mesh = item.mesh;
        if (icon != null) this.icon = item.icon;

        this.damage = damage;
        this.critChance = critChance;
        this.critDamage = critDamage;
        this.itemType = type;
        this.range = range;
    }

    public override string GetStats() {
        string msg = base.GetStats();

        msg += "Damage: " + damage + "\n";
        msg += "CritDamaage : " + critDamage + "\n";
        msg += "CritChance : " + critChance + "\n";
        msg += "Range : " + range + "\n";

        return msg;
    }
}
[System.Serializable]
public class Ability : Item {
    Func<Unit, Unit, Ability, bool> callback;

    public float damage;
    public float critDamage;
    public float critChance;
    public float range;

    public enum Ability_Type {
        SingleTarget,
        OverTime,
        AOE,
    } public Ability_Type abilityType;
    public Ability(Item item, float damage, float critDamage, float critChance, float range, Ability_Type type, Func<Unit, Unit, Ability, bool> callback) {
        this.name = item.name;
        this.description = item.description;
        this.ID = item.ID;
        this.actionPoints = item.actionPoints;
        if (mesh != null) this.mesh = item.mesh;
        if (icon != null) this.icon = item.icon;

        this.damage = damage;
        this.critChance = critChance;
        this.critDamage = critDamage;
        this.abilityType = type;
        this.range = range;

        this.callback = callback;
    }

    public bool UseAbility(Unit unit, Unit other) {
        return this.callback(unit, other, this);
    }

    public override string GetStats() {
        string msg = base.GetStats();

        msg += "Damage: " + damage + "\n";
        msg += "CritDamaage : " + critDamage + "\n";
        msg += "CritChance : " + critChance + "\n";
        msg += "Range : " + range + "\n";

        return msg;
    }
}

