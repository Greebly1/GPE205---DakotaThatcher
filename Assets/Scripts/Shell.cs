using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shell : MonoBehaviour
{
    public float speed;

    public float duration;

    public float lifeTime;

    public float damage;

    public GameObject owner;

    Rigidbody rb;

    // Start is called before the first frame update
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        Invoke("endProjectile", duration);
    }

    // Update is called once per frame
    void Update()
    {
        lifeTime += Time.deltaTime;
        rb.MovePosition(gameObject.transform.position + (gameObject.transform.forward * speed * Time.deltaTime));
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger)
        {
            return;
        }

        Health otherHealth = other.GetComponent<Health>();
        if (otherHealth != null)
        {
            otherHealth.takeDamage(owner, damage);
        }

        endProjectile();
    }

        private void endProjectile()
    {
        Destroy(gameObject);
    }

    public float distanceTravelled()
    {
        return speed * lifeTime;
    }
}
