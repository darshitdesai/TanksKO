using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Powerup : MonoBehaviour {

    public float healthAdded;
    public float healthPortionAdded;
    public float healthPortionLimit;
    public float fuelAdded;
    public float fuelPortionAdded;
    public float fuelPortionLimit;

    public void Pickup(TankBehaviour tank)
    {
        bool healthFull = tank.health / tank.maxHealth >= healthPortionLimit;
        bool fuelFull = tank.fuel / tank.maxFuel >= fuelPortionLimit;
        if (healthFull && fuelFull)
            return;

        if (!healthFull)
            tank.health = Mathf.Min(tank.maxHealth * healthPortionLimit, tank.health + healthAdded + tank.maxHealth * healthPortionAdded);

        if (!fuelFull)
            tank.fuel = Mathf.Min(tank.maxFuel * fuelPortionLimit, tank.fuel + fuelAdded + tank.maxFuel * fuelPortionAdded);

        Destroy(gameObject);
    }

}
