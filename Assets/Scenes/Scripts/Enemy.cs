using System.Collections;
using System.Collections.Generic;
//Pathfinding NavMeshAgent requires the below namespace
using UnityEngine.AI;
using UnityEngine;

//Add NavMeshAgent to any object with this script, if it doesn't already exist.
[RequireComponent(typeof(NavMeshAgent))]
//Inherits from LivingEntity, which in turn inherits from monobehaviour, and IDamagable.
public class Enemy : LivingEntity 
{
    public enum State { Idle, Chasing, Attacking };
    State currentState;

    Material skinMaterial;

    Color originalColor;

    //pathfinder to navigate obstacles, and follow player
    NavMeshAgent pathfinder;
    Transform target;

    LivingEntity targetEntity;

    //distance enemy has to be to player for attack to trigger
    float attackDistanceThreshold = .5f;
    //Time between enemies attacks (cooldown)
    float timeBetweenAttacks = 1;

    float damage = 1;

    //Next available attack time, calculated from Time.time + timeBetweenAttacks
    float nextAttackTime;

    //radius of the capsules of player and enemy
    float myCollisionRadius;
    float targetCollisionRadius;

    bool hasTarget;

    // Start is called before the first frame update
    //protected keyword, protected members are accessible within it's class, and by derived class instances.
    //override overrides the base classes start method.
    protected override void Start()
    {
        //base accesses the base class that this class inherits from. Used in this case to call base class (LivingEntity's) Start() method.
        base.Start();
        //gets current enemy material
        skinMaterial = GetComponent<Renderer>().material;
        //gets current color of enemy
        originalColor = skinMaterial.color;
        //Assign pathfinder to NavMeshAgent component
        pathfinder = GetComponent<NavMeshAgent>();
        //Assign target to player's location
        if (GameObject.FindGameObjectWithTag("Player") != null)
        {
            target = GameObject.FindGameObjectWithTag("Player").transform;
            currentState = State.Chasing;
            targetEntity = target.GetComponent<LivingEntity>();
            targetEntity.OnDeath += OnTargetDeath;
            hasTarget = true;
            //gets radius of capsules of player and enemy
            myCollisionRadius = GetComponent<CapsuleCollider>().radius;
            targetCollisionRadius = GetComponent<CapsuleCollider>().radius;

            //Start UpdatePath coroutine (in this case a similar function to Update() method, but called much less often)
            StartCoroutine(UpdatePath());
        }

    }

    public void OnTargetDeath()
    {
        hasTarget = false;
        currentState = State.Idle;
    }


    // Update is called once per frame
    void Update()
    {
        if (hasTarget)
        {
            //Checks if attack cooldown is up
            if (Time.time > nextAttackTime)
            {
                //distance from player to enemy, squared. As a float, since returns the magnitude squared.
                float sqrDistanceToTarget = (target.position - transform.position).sqrMagnitude;

                //if squared distance between enemy and player is less than the squared attack threshold (enemy within range), execute
                //Square rooting can be inefficient, and unnecessarily use resources, so just squaring the two values and comparing is sufficient to save resources.
                if (sqrDistanceToTarget < Mathf.Pow(attackDistanceThreshold + myCollisionRadius + targetCollisionRadius, 2))
                {
                    //sets attack cooldown to current time, + cooldown value
                    nextAttackTime = Time.time + timeBetweenAttacks;
                    StartCoroutine(Attack());
                }
            }
        }
        
        
    }

    //Animates an attack on the player. Goes from starting position, to players position, back to starting position
    IEnumerator Attack()
    {
        currentState = State.Attacking;
        //Disable pathfinder while attacking
        pathfinder.enabled = false;
        //Starting position
        Vector3 originalPosition = transform.position;
        //Players position
        Vector3 dirToTarget = (target.position - transform.position).normalized;
        //Position the enemy goes to when attacking. Players position, minus the radius of the capsule, times the direction (so it corrects the radius in the right direction.
        //Note: since the positions are calculated from the centre of the capsules, this will result in the centre of the enemy capsule hitting the edge of the player capsule
        //because only the radius of the player capsule is accounted for. To make the enemy hit the outside only of the player, switch (myCollisionRadius) to 
        //(myCollisionRadius + targetCollisionRadius) to account for both radii
        Vector3 attackPosition = target.position - dirToTarget * (myCollisionRadius);

        //Speed which the animation executes
        float attackSpeed = 3;
        //percentage of animation (how far it is to being complete)
        float percent = 0;

        //Switch colour to red when in attack mode
        skinMaterial.color = Color.red;
        bool hasAppliedDamage = false;
        while (percent <= 1)
        {
            if (percent >= .5f && !hasAppliedDamage)
            {
                hasAppliedDamage = true;
                targetEntity.TakeDamage(damage);
            }
            //increment the percent up to a value of one. Scaled by the attack speed.
            //Time.deltaTime is time since last frame. Used to make it unreliant on frame rate.
            //The higher your frame rate, the smoother it will look, but it will be completed in same amount of time.
            percent += Time.deltaTime * attackSpeed;
            //interpolation value that is equal to the value of a parabola (y = 4(-x^2 + x))
            //Parabola is used so that the value lerps between a min value, max value, and then min value again.
            //In this case, allows for the original position -> target position -> original position
            float interpolation = (-Mathf.Pow(percent, 2) + percent) * 4;
            transform.position = Vector3.Lerp(originalPosition, attackPosition, interpolation);
            //Wait until next frame
            yield return null;
        }
        //Reenable pathfinder after attack finishes.
        pathfinder.enabled = true;
        currentState = State.Chasing;
        skinMaterial.color = originalColor;
    }

    //IEnumerator to update target position every quarter of a second
    IEnumerator UpdatePath()
    {
        //How often function is called
        float refreshRate = .25f;

        while (hasTarget)
        {
            if (currentState == State.Chasing)
            {
                //Gets the direction of the player (vector 3 with magnitude 1, pointing towards player)
                Vector3 dirToTarget = (target.position - transform.position).normalized;
                //pathfind to targetPosition; which is the position of player, - the radii of both enemy and player (dirToTarget corrects this in the right direction), 
                //the radii are taken away to pathfind only to the outside edge of the player. the attackDistanceThreshold further increases the distance away from the player.
                Vector3 targetPosition = target.position - dirToTarget * (myCollisionRadius + targetCollisionRadius + attackDistanceThreshold / 2) ;
                //if player is alive, set the NavMeshAgents pathfinding destination to the targetPosition (players location)
                if (!dead)
                {
                    pathfinder.SetDestination(targetPosition);
                }
            }
            //Wait here for the time passed into WaitForSeconds before resuming execution
            yield return new WaitForSeconds(refreshRate);
        }
    }
}
