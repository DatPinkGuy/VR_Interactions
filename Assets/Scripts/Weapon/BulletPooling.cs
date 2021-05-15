using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Weapon;

public class BulletPooling : MonoBehaviour
{
    [SerializeField] private int amountToPool;
    [SerializeField] private GameObject bulletPrefab;
    private static GameObject[] pooledBullets;
    private static RigidBullet[] pooledRigids;
    public int currentBullet;

    private void Awake()
    {
        pooledBullets = new GameObject[amountToPool];
        pooledRigids = new RigidBullet[amountToPool];
        SetUpRigidShoot();
    }

    private void SetUpRigidShoot()
    {
        for (var i = 0; i < amountToPool; i++)
        {
            var bullet = Instantiate(bulletPrefab, transform, true);
            pooledBullets[i] = bullet;
            pooledRigids[i] = bullet.GetComponent<RigidBullet>();
        }
    }

    public void CheckCurrentBullet()
    {
        if (currentBullet == amountToPool) currentBullet = 0;
    }

    public RigidBullet GetCurrentBullet()
    {
        return pooledRigids[currentBullet];
    }
}
