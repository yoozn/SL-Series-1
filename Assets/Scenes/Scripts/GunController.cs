using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunController : MonoBehaviour
{
    public Transform weaponHold;
    //new public variable in the gun class to set the starting gun
    public Gun startingGun;
    //private variable in gun class, checking currently equipped gun
    Gun equippedGun;

    public void Start()
    {
        //if there isn't a starting gun, equip starting gun
        if (startingGun != null)
        {
            EquipGun(startingGun);
        }
    }

    //Equip gun
    public void EquipGun(Gun gunToEquip)
    {
        //if a gun is currently equipped, destroy it
        if (equippedGun != null)
        {
            Destroy(equippedGun.gameObject);
        }
        //Instantiate new gun, in the position set from weaponHold in hierarchy, and as that rotation. Instantiate as part of Gun class.
        equippedGun = Instantiate(gunToEquip, weaponHold.position, weaponHold.rotation) as Gun;
        //Set the parent of instantiated gun as weapon hold empty in scene.
        equippedGun.transform.parent = weaponHold;  
    }

    //method to shoot.
    public void Shoot()
    {
        //if there is a gun, shoot.
        if (equippedGun != null)
        {
            //Shoot method belonging to the Gun class
            equippedGun.Shoot();
        }
    }
}
