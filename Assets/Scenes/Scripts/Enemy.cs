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
    //pathfinder to navigate obstacles, and follow player
    NavMeshAgent pathfinder;
    Transform target;
    // Start is called before the first frame update
    //protected keyword, protected members are accessible within it's class, and by derived class instances.
    //override overrides the base classes start method.
    protected override void Start()
    {
        //base accesses the base class that this class inherits from. Used in this case to call base class (LivingEntity's) Start() method.
        base.Start();
        //Assign pathfinder to NavMeshAgent component
        pathfinder = GetComponent<NavMeshAgent>();
        //Assign target to player's location
        target = GameObject.FindGameObjectWithTag("Player").transform;
        //Start UpdatePath coroutine (in this case a similar function to Update() method, but called much less often)
        StartCoroutine(UpdatePath());

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //IEnumerator to update target position every quarter of a second
    IEnumerator UpdatePath()
    {
        //How often function is called
        float refreshRate = .25f;

        while (target != null)
        {
            //target position = playeres position, without including y value.
            Vector3 targetPosition = new Vector3(target.position.x, 0, target.position.z);
            //if player is alive, set the NavMeshAgents pathfinding destination to the targetPosition (players location)
            if (!dead)
            {
                pathfinder.SetDestination(targetPosition);
            }
            //Wait here for the time passed into WaitForSeconds before resuming execution
            yield return new WaitForSeconds(refreshRate);
        }
    }
}
