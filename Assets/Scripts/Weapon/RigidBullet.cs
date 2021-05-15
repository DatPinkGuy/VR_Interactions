using System.Collections;
using UnityEngine;

namespace Weapon
{
    public class RigidBullet : MonoBehaviour
    {
        private float currentLifeTime;
        private RaycastHit hit;
        [SerializeField] private float lifeTime;
        [SerializeField] private float bulletForce;
        [SerializeField] private int bulletDamage;
        [SerializeField] private GameObject hitMarker;
        [SerializeField] private float rayDistance;
        [SerializeField] private AudioSource hitSound;
        private Rigidbody rb;
        private float distanceToObject;
        private float timeToReach;
        private bool isCoroutineRunning;

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
            gameObject.SetActive(false);
        }

        private void Update()
        {
            if (!gameObject.activeSelf) return;
            currentLifeTime += Time.deltaTime;
            if(!isCoroutineRunning)CheckCollisionWithRay();
            if (currentLifeTime > lifeTime)
            {
                StopBullet();
            }
        }

        public void LaunchProjectile(Transform spawnPosition)
        {
            StopBullet();
            transform.rotation = spawnPosition.rotation;
            transform.position = spawnPosition.position;
            gameObject.SetActive(true);
            CheckCollisionWithRay();
            rb.AddForce(transform.forward * bulletForce, ForceMode.Impulse);
        }

        private void StopBullet()
        {
            rb.velocity = Vector3.zero;
            gameObject.SetActive(false);
            currentLifeTime = 0f;
            isCoroutineRunning = false;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.transform.CompareTag("Target"))
            {
                //Instantiate(hitMarker, transform.position, Quaternion.identity);
            }
        }

        private void CheckCollisionWithRay()
        {
            if (!Physics.Raycast(transform.position, transform.forward, out hit, rayDistance)) return;
            distanceToObject = Vector3.Distance(transform.position, hit.point);
            if(rb.velocity != Vector3.zero) timeToReach = distanceToObject / rb.velocity.magnitude;
            else timeToReach = distanceToObject / (transform.forward * bulletForce).magnitude;
            StartCoroutine(CheckIfCollide(timeToReach, hit.point, hit.transform));
            //StartCoroutine(CheckIfCollide(timeToReach, transform.position, hit.transform));
        }

        private IEnumerator CheckIfCollide(float seconds, Vector3 hitPoint, Transform hitTransform)
        {
            isCoroutineRunning = true;
            yield return new WaitForSeconds(seconds);
            if (!hitTransform.GetComponent<Collider>().bounds.Contains(hitPoint)) yield break;
            SpawnMarker();
            //if (!Physics.Raycast(hitPoint, transform.forward, out hit2, rayDistance)) yield break;
            //if(hitTransform == hit2.transform) SpawnMarker();
        }

        private IEnumerator PlaySound()
        {
            hitSound.Play();
            rb.velocity = Vector3.zero;
            yield return new WaitWhile(() => hitSound.isPlaying);
            gameObject.SetActive(false);
            currentLifeTime = 0f;
            isCoroutineRunning = false;
        }

        private void SpawnMarker()
        {
            Instantiate(hitMarker, hit.point, Quaternion.identity);
            StartCoroutine(PlaySound());
        }
    
    
    }
}
