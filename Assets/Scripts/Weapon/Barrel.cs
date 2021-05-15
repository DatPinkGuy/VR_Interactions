using System;
using System.Collections;
using UnityEngine;

namespace Weapon
{
    public class Barrel : MonoBehaviour
    {
        public enum ShootingType
        {
            Rigidbody,
            Ray
        }
        public ShootingType shootingType;
        public bool isOnlySingleFireMode;
        public bool isOnSingleFireMode;
        public float fireSpeed;
        public bool isMachineGun;
        private bool singleFired;
        private bool laserSwitched;
        private Weapon _weapon;
        private Coroutine firing;
        private Coroutine reloading;
        private WaitForSeconds waitForSeconds;
        private WaitForSeconds waitForSecondsRel;
        private RaycastHit _hit;
        private BulletPooling _bulletPoolingScript;
        [SerializeField] private LineRenderer _lineRenderer;
        [SerializeField] private float reloadTime;
        [SerializeField] private MagazineSlot magSlot;
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private AudioClip dryFire;
        [SerializeField] private AudioClip bulletShoot;
        public bool SingleFired
        {
            set => singleFired = value;
        }

        public bool LaserSwitched
        {
            set => laserSwitched = value;
        }

        public void Setup(Weapon weapon)
        {
            _weapon = weapon;
        }
    
        private void Awake()
        {
            waitForSeconds = new WaitForSeconds(fireSpeed);
            waitForSecondsRel = new WaitForSeconds(reloadTime);
            _bulletPoolingScript = FindObjectOfType<BulletPooling>();
            //currentMagazineSize = magazineSize;
        }

        private void Update()
        {
            SwitchLaser();
            DrawLaser();
        }

        public void Fire()
        {
            if (singleFired) return;
            if (isOnSingleFireMode) singleFired = true;
            if (!magSlot.CurrentMagazine)
            {
                audioSource.PlayOneShot(dryFire);
                return;
            }
            if (magSlot.CurrentMagazine.CurrentSize <= 0)
            {
                audioSource.PlayOneShot(dryFire);
                return;
            }
            firing = StartCoroutine(FiringSeq());
        }
    
        public void StopFire()
        {
            try
            {
                StopCoroutine(firing);
            }
            catch (Exception)
            {
                // ignored
            }
        }
    
        // public void Reload()
        // {
        //     if (reloading == null) reloading = StartCoroutine(ReloadRoutine());
        // }

        public void StopReload()
        {
            if (reloading == null) return;
            StopCoroutine(reloading);
            reloading = null;
        }

        private void UsePellet()
        {
            if (magSlot.CurrentMagazine.CurrentSize == 0)
            {
                audioSource.PlayOneShot(dryFire);
                StopCoroutine(firing);
                //if(isMachineGun) Reload();
                return;
            }
            magSlot.CurrentMagazine.CurrentSize--;
            if (shootingType == ShootingType.Rigidbody)
            {
                _bulletPoolingScript.currentBullet++;
                _bulletPoolingScript.CheckCurrentBullet();
                RigidShoot(_bulletPoolingScript.GetCurrentBullet());
            }
            else RayBulletShoot();
        }
    
        private IEnumerator FiringSeq()
        {
            while (gameObject.activeSelf)
            {
                UsePellet();
                //_weapon.Recoil();
                audioSource.PlayOneShot(bulletShoot);
                if (singleFired) yield break;
                yield return waitForSeconds;
            }
        }

        // private IEnumerator ReloadRoutine()
        // {
        //     yield return waitForSecondsRel;
        //     //currentMagazineSize = magazineSize;
        //     _weapon.UseVibration();
        //     if (isMachineGun) reloading = null;
        //     yield return null;
        // }

        private void RayBulletShoot()
        {
            if (Physics.Raycast(transform.position, transform.forward, out _hit, 100))
            {
            
            }
        }

        private void RigidShoot(RigidBullet bullet)
        {
            bullet.LaunchProjectile(transform);
        }

        private void SwitchLaser()
        {
            if (!_lineRenderer) return;
            if (!_weapon.CheckLaser()) return;
            if (laserSwitched) return;
            laserSwitched = true;
            _lineRenderer.enabled = !_lineRenderer.enabled;
        }

        private void DrawLaser()
        {
            if (!_lineRenderer) return;
            if (!_lineRenderer.enabled) return;
            if (Physics.Raycast(_lineRenderer.transform.position, _lineRenderer.transform.forward, out _hit, Mathf.Infinity))
            {
                _lineRenderer.SetPosition(0, _lineRenderer.transform.position);
                _lineRenderer.SetPosition(1, _hit.point);
            }
            else
            {
                _lineRenderer.SetPosition(0, _lineRenderer.transform.position);
                _lineRenderer.SetPosition(1, _lineRenderer.transform.position + 10f * _lineRenderer.transform.forward);
            }
        }
    }
}
