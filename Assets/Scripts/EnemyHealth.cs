using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public int health;
    private int maxHalth;
    public ParticleSystem hitVFX;

    public MeshRenderer enemyMaterial;

    public Color fullHealthColor;
    public Color midHealthColor;
    public Color lowHealthColor;

    private void Awake()
    {
        maxHalth = health;
        enemyMaterial.material.color = fullHealthColor;
    }

    void Update()
    {
        if (health <= 0)
        {
            Destroy(gameObject);
        }

        if ((health * 100 / health) <= 50)
            enemyMaterial.material.color = midHealthColor;

        else if ((health * 100 / health) <= 20)
            enemyMaterial.material.color = lowHealthColor;
    }

    public void TakeDamage(int damage)
    {
        hitVFX.Play();
        health -= damage;
        ChangeColor();
    }
    public void ChangeColor()
    {

    }
}
