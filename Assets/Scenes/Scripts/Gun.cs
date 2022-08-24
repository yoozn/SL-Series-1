using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    public Transform muzzle;
    //class projectile from custom projectile class
    public Projectile projectile;
    //how long before new shot can be fired
    public float msBetweenShots = 100;
    //Velocity leaving the gun muzzle
    public float muzzleVelocity = 35;

    //Time when the next shot will be fired
    float nextShotTime;



    public void Shoot()
    {
        if (Time.time > nextShotTime)
        {
            //Time of the next shot, calculated from current time (in seconds) + the set time before shots, divided by 1000 (to convert to seconds)
            nextShotTime = Time.time + msBetweenShots / 1000;
            //Instantiate a new projectile, from the position and rotation of the gun. Instantiate it in the projectile class.
            Projectile newProjectile = Instantiate(projectile, muzzle.position, muzzle.rotation) as Projectile;
            //Set the speed using custom method from Projectile class.
            newProjectile.SetSpeed(muzzleVelocity);
        }
            
    }
}
