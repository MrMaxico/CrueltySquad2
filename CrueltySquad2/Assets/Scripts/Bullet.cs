using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [Header("Don't assign any of these values")]
    public float spitSpeed;
    public GameObject player;
    public float damage;

    // Update is called once per frame
    void Update()
    {
        transform.position += transform.forward * spitSpeed * Time.deltaTime;

        if (Vector3.Distance(transform.position, player.transform.position) <= transform.lossyScale.x)
        {
            Debug.Log("spit-hit");
            player.GetComponent<Health>().Damage(damage);
            Destroy(this.gameObject);
        }
    }
}
