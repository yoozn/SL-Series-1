using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Adds rigid body component to any object that has this script applied to it
[RequireComponent (typeof(Rigidbody))]

public class PlayerController : MonoBehaviour
{
    Vector3 velocity;
    Rigidbody myRigidbody;
    
    void Start()
    {
        myRigidbody = GetComponent<Rigidbody>();
    }

    //custom method to make capsule look towards x and z values. Doesnt change y value from the capsules y value.
    public void LookAt(Vector3 lookPoint)
    {
        //look towards the x and y mapped point taken from raycast in player script
        Vector3 heightCorrectedPoint = new Vector3(lookPoint.x, transform.position.y, lookPoint.z);
        transform.LookAt(heightCorrectedPoint);
    }

    //set player velocity according to the velocity from the inputs in player class
    public void Move(Vector3 _velocity)
    {
        velocity = _velocity;
    }

    //Fixed update is called in small increments for more physics accuracy. Called in sync with physics engine itself
    void FixedUpdate()
    {
        //Move player based off current position, plus the velocity. fixed delta time is based off physics similar to fixed update.
        myRigidbody.MovePosition(myRigidbody.position + velocity * Time.fixedDeltaTime);
    }
}
