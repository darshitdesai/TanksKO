using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankBehaviour : MonoBehaviour {
    [Header("Stats")]
    public float maxHealth;                 //The maximum health of this tank.
    public float maxFuel;                   //The maximum fuel of this tank.
    public float fuelConsumption;           //How much fuel is consumed per frame of movement
    public float damage;                    //How much damage this tank does with a direct hit
    public float blastRadius;               //How close a tank has to be to the explosion to be damaged
    public float moveSpeed;                 //How fast the tank can move.
    public float barrelTiltSpeed;           //How fast the tank's barrel tilts
    public float projectileSpeed;           //How fast the tank's projectiles can move.
    public float pickupRadius;

    [HideInInspector]
    public float health;                    //The current health of the tank.
    [HideInInspector]
    public float fuel;                      //The current fuel of the tank.
    [HideInInspector]
    public Map map;                         //The map script, needed for movement

    private Vector3 barrelAngle;            //The angle of the barrel
    private bool flipped = false;

    [Header("Components / Objects")]
    public GameObject projectile;           //The projectile prefab of which the tank can shoot.
    public GameObject explodeEffect;        //The particle effect prefab that plays when the tank dies.
    public GameObject barrel;               //The barrel of the tank. This is where the projectile will spawn.

    private Vector2 mapPosition;

    private void Start()
    {
        health = maxHealth;
        fuel = maxFuel;

        Vector3 position3D = GetComponent<Transform>().position;
        mapPosition = map.screenSpaceToPosition(new Vector2(position3D.x, position3D.y));

        placeOnMap();
    }

    public void Move(int direction)
    {
        Vector3 scale = GetComponent<Transform>().localScale;
        if (direction == 0)
        {
            scale.x = -Mathf.Abs(scale.x);
            flipped = true;
        }
        if (direction == 1)
        {
            scale.x = Mathf.Abs(scale.x);
            flipped = false;
        }
        GetComponent<Transform>().localScale = scale;

        if (fuel <= 0) return;
        fuel -= fuelConsumption;
        
        float distancePerPixel = Mathf.Sqrt(1 + Mathf.Pow(map.getSlope((int)mapPosition.x), 2));

        if (direction == 0)
            mapPosition.x -= moveSpeed / distancePerPixel;
        else if (direction == 1)
            mapPosition.x += moveSpeed / distancePerPixel;

        placeOnMap();

        Collider[] hit = Physics.OverlapSphere(GetComponent<Transform>().position, pickupRadius / map.pixelsPerUnit);
        for (int i = 0; i < hit.Length; i++)
        {
            Powerup powerup = hit[i].GetComponent<Powerup>();
            if (powerup != null)
            {
                powerup.Pickup(this);
            }
        }
    }

    public void TiltBarrel(int direction)
    {
        float firingCone = 90;
        float coneCentre = 25; //Angle up from front
        Vector3 rotation = barrel.GetComponent<Transform>().localRotation.eulerAngles;
        rotation.z += (90-coneCentre)-firingCone/2;
        rotation.z %= firingCone;
        if (direction == 0 && (rotation.z-barrelTiltSpeed > 0))
            rotation.z -= barrelTiltSpeed;
        if (direction == 1 && rotation.z+barrelTiltSpeed < firingCone)
            rotation.z += barrelTiltSpeed;
        rotation.z -= (90 - coneCentre) - firingCone / 2;

        barrel.GetComponent<Transform>().localRotation = Quaternion.Euler(rotation.x, rotation.y, rotation.z);
    }

    public void Fire()
    {
        GameObject proj = Instantiate(projectile, barrel.transform.position, Quaternion.identity) as GameObject;
        Projectile projScript = proj.GetComponent<Projectile>();
        projScript.damage = damage;
        projScript.blastRadius = blastRadius/map.pixelsPerUnit;
        projScript.map = map;
        float offset = -90;
        if (flipped)
            offset *= -1;
        projScript.velocity = projectileSpeed * (barrel.transform.rotation * Quaternion.Euler(0,0,offset) * Vector3.up);
    }

    public void takeDamage(float damage)
    {
        health -= damage;
        if (health <= 0)
        {
            health = 0;
            GameObject explosion = Instantiate(explodeEffect, transform.position, Quaternion.identity) as GameObject;
            Destroy(explosion, 3.0f);
            Destroy(gameObject);
        }
    }

    private void placeOnMap()
    {
        if (mapPosition.x < 0) mapPosition.x = 0;
        if (mapPosition.x > map.width-1) mapPosition.x = map.width-1;

        mapPosition.y = map.getHeight((int)mapPosition.x);

        float slope = map.getSlope((int)mapPosition.x);
        float angle = Mathf.Atan(slope) * (2 / Mathf.PI) * 90;
        GetComponent<Transform>().rotation = Quaternion.Euler(0, 0, angle);
        
        Vector2 position2D = map.positionToScreenSpace(mapPosition);
        Vector3 position3D = GetComponent<Transform>().position;
        position3D = new Vector3(position2D.x, position2D.y, position3D.z);
        GetComponent<Transform>().position = position3D;
    }
}