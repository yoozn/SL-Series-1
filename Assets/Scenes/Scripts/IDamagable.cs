using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Interface that applies to LivingEntity, and in turn to player and enemy classes.
public interface IDamagable
{
    //This function has to be included in all classes that inherit from IDamagable (LivingEntity)
    void TakeHit(float damage, RaycastHit hit);
}
