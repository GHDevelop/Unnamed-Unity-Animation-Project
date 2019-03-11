using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootingWeapon : Weapon
{
    [SerializeField] private float cooldown = 1.0f;
    [SerializeField] private bool canShoot = true;
    [SerializeField] private float bulletSpread = 0.0f;
    [SerializeField] private int numShots = 1;

    [SerializeField] private Transform firingPosition;
    [SerializeField] private Bullet bullet;

    public override void Fire()
    {
        if (canShoot)
        {
            for (int index = 0; index < numShots; index++)
            {
                Bullet activeBullet = Instantiate(bullet, firingPosition.position, firingPosition.rotation * GetSpread()) as Bullet;
                activeBullet.gameObject.layer = this.gameObject.layer;
            }
            StartCoroutine("ShotCooldown");
        }
    }

    private Quaternion GetSpread()
    {
        return Quaternion.Euler(Random.onUnitSphere * bulletSpread);
    }

    private IEnumerator ShotCooldown()
    {
        canShoot = false;

        for (float timer = cooldown; timer > 0; timer -= Time.deltaTime)
        {
            yield return null;
        }

        canShoot = true;
    }
}
