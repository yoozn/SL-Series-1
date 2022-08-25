using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    //Layer mask checking if the projectiles collide with the right layer (enemy)
    public LayerMask collisionMask;

    //speed of projectile
    float speed = 10f;
    //damage of projectile
    float damage = 1;

    float lifeTime = 3;
    //Set speed of projectile
    public void SetSpeed(float newSpeed)
    {
        speed = newSpeed;
    }


    private void Start()
    {
        Destroy(gameObject, lifeTime);

        //With raycasts, if the bullet spawned inside an enemy, hit wouldnt register. This fixes that.
        //Array of colliders, which is a list of all objects that overlaps with a instanced sphere, and are in the
        //right collision mask.
        Collider[] initialCollisions = Physics.OverlapSphere(transform.position, .1f, collisionMask);
        //if the array is greater than 0, than put the first object collided with into the method OnHitObject
        if (initialCollisions.Length > 0 )
        {
            OnHitObject(initialCollisions[0]);
        }
    }
    // Update is called once per frame
    void Update()
    {
        //moveDistance
        float moveDistance = speed * Time.deltaTime;
        CheckCollisions(moveDistance);
        transform.Translate(Vector3.forward * moveDistance);

    }

    //Check if collided with enemy
    void CheckCollisions(float moveDistance)
    {
        //New ray from current position, in direction that projectile is facing
        Ray ray = new Ray(transform.position, transform.forward);
        //raycast hit information
        RaycastHit hit;

        //if raycast collides with right layer, and enable raycast to collide with triggers
        if (Physics.Raycast(ray, out hit, moveDistance, collisionMask, QueryTriggerInteraction.Collide))
        {
            //custom method that uses hit information
            OnHitObject(hit);
        }
    }

    //called upon hitting a enemy.
    void OnHitObject(RaycastHit hit)
    {
        //Interface. if collided object has a IDamagable interface, set damagableObject to the colliders interface.
        IDamagable damagableObject = hit.collider.GetComponent<IDamagable>();
        //if collider has IDamagable interface, call it's TakeHit method.
        if (damagableObject != null )
        {
            damagableObject.TakeHit(damage, hit);
        }
        //Destroy projectile
        GameObject.Destroy(gameObject);
    }

    //Function overload of above function. Same functionality, but doesn't need a raycast.
    void OnHitObject(Collider c)
    {
        //Interface. if collided object has a IDamagable interface, set damagableObject to the colliders interface.
        IDamagable damagableObject = c.GetComponent<IDamagable>();
        //if collider has IDamagable interface, call it's TakeDamage method.
        if (damagableObject != null)
        {
            damagableObject.TakeDamage(damage);
        }
        //Destroy projectile
        GameObject.Destroy(gameObject);
    }
}
