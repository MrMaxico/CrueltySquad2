using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [Header("Don't assign any of these values")]
    public float spitSpeed;
    public GameObject player;
    public float damage;
    float exsistingTime;

    // Update is called once per frame
    void Update()
    {
        transform.position += transform.forward * spitSpeed * Time.deltaTime;

        exsistingTime += Time.deltaTime;

        if (exsistingTime > 15)
        {
            Destroy(this.gameObject);
        }

        if (Vector3.Distance(transform.position, player.transform.position) <= transform.lossyScale.x)
        {
            Debug.Log("spit-hit");
            player.GetComponent<Health>().Damage(damage);
            Destroy(this.gameObject);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            return;
        }
        else if (exsistingTime >= 0.3f)
        {
            Debug.Log("Spit hits different");
            Destroy(this.gameObject);
        }
    }
}
