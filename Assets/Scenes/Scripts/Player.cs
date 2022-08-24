using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// adds components of type "typeof" to game object that this script is added to.
[RequireComponent (typeof(PlayerController))]
[RequireComponent (typeof(GunController))]
//Inherit from LivingEntity class, which in turn inherits from Monobehaviour, and IDamagable.
public class Player : LivingEntity
{
    public float moveSpeed = 5;

    Camera viewCamera;
    PlayerController controller;

    //GunController class.
    GunController gunController;
    //protected is access modifier that is accesible within its class and its derived class instances
    protected override void Start()
    {
        //base is base class. calls start method from LivingEntity class (base class)
        base.Start();

        //references to controllers
        controller = GetComponent<PlayerController>();
        gunController = GetComponent<GunController>();

        //Gets the main camera from scene, assigns to viewCamera
        viewCamera = Camera.main;

    }

    
    void Update()
    {

        //Movement Input

        //Get Axis Raw is getting input without any smoothing
        Vector3 moveInput = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
        Vector3 moveVelocity = moveInput.normalized * moveSpeed;
        //calling Move method which is defined in controller class
        controller.Move(moveVelocity);


        //Look Input

        //ScreenPointToRay returns ray from camera through a screen point
        Ray ray = viewCamera.ScreenPointToRay(Input.mousePosition);

        //make a new plane, simulating the plane in our scene
        Plane groundPlane = new Plane(Vector3.up, Vector3.zero);
        //defined in the following if statement
        float rayDistance;


        //if ray is intersecting plane from above, set the distance of the raycast to variable rayDistance
        if (groundPlane.Raycast(ray, out rayDistance))
        {
            //returns point at rayDistance distance along the ray
            Vector3 point = ray.GetPoint(rayDistance);
            //Draw the raycast line in the scene viewer
            Debug.Log(point);
            Debug.DrawLine(ray.origin, point, Color.red);
            //Rotate player capsule towards the point
            controller.LookAt(point);
        }

        //Weapon Input
        if (Input.GetMouseButton(0))
        {
            //calls the shoot method from the gunController
            gunController.Shoot();
        }
    }
}
