using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveToPlayer : MonoBehaviour
{
    public GameObject player;
    public float followSpeed = .01f;
    public int damage = 1;

    private void Update()
    {
        player = GameObject.Find("PolicemanController(Clone)");

        Vector3 targetPosition = new Vector3(player.transform.position.x, player.transform.position.y, player.transform.position.z);
        transform.position = Vector3.Lerp(transform.position, targetPosition, followSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
            player.GetComponent<PlayerHealth>().TakeDamage(damage, "Enemy");
    }
}
