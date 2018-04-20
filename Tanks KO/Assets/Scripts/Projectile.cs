using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public Map map;
    public Vector2 mapPosition;
    public Vector2 velocity;
    public float damage;
    public float blastRadius;
    public GameObject explodeEffect;
    public GameObject trailEffect;

    private GameObject trail;

    void Start()
    {
        Vector3 position3D = GetComponent<Transform>().position;
        mapPosition = map.screenSpaceToPosition(new Vector2(position3D.x, position3D.y));
        trail = Instantiate(trailEffect, gameObject.transform);
        Place();
    }

    void Update()
    {
        mapPosition += velocity;
        Place();
        velocity.y -= map.gravity;
        if (map.getHeight((int)mapPosition.x) > mapPosition.y)
        {
            explode();
        }
    }

    void Place()
    {
        float angle = Mathf.Atan2(velocity.y, velocity.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        Vector2 position2D = map.positionToScreenSpace(mapPosition);
        GetComponent<Transform>().position = new Vector3(position2D.x, position2D.y, GetComponent<Transform>().position.z);
    }

    void explode()
    {
        Vector3 blastPosition = GetComponent<Transform>().position;
        Collider[] hit = Physics.OverlapSphere(blastPosition, blastRadius);
        for(int i = 0; i < hit.Length; i++)
        {
            TankBehaviour tank = hit[i].GetComponent<TankBehaviour>();
            if (tank != null)
            {
                float distance = (blastPosition - hit[i].GetComponent<Transform>().position).magnitude;
                if (distance > blastRadius) return;
                tank.takeDamage(damage * (1-distance/blastRadius));
            }
        }

        GameObject explosion = Instantiate(explodeEffect, transform.position, Quaternion.identity) as GameObject;
        trail.transform.parent = gameObject.transform.parent;
        //trail.GetComponent<ParticleSystem>().Stop();
        Destroy(explosion, 3.0f);
        Destroy(trail, 3.0f);
        Destroy(gameObject);
    }
}
