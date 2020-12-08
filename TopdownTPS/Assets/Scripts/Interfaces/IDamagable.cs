using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamagable
{
    void TakeDamage(float damage, Vector3 hitPoint, Vector3 hitDirection);

    void TakeDmg(float damage);
}
