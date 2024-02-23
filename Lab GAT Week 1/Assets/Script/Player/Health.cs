using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    [Header ("Health")]
    [SerializeField] private float maxHP;
    public float currentHealth { get; private set; } //private set is so that it currentHealth can only be set from this script
    private Animator anim;
    private bool dead;

    [Header("iFrames")]
    [SerializeField] private float iFramesDuration;
    [SerializeField] private int numberOfFlashes;
    private SpriteRenderer spriteRend;
    void Start()
    {
        currentHealth = maxHP;
        anim = GetComponent<Animator>();
        spriteRend = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log(maxHP);
    }

    public void TakeDamage(float damage)
    {
        Debug.Log("TakeDamage");
        currentHealth = Mathf.Clamp(currentHealth - damage, 0, maxHP);

        if (currentHealth <= 0)
        {
            if (!dead) {
                anim.SetTrigger("Die");
                GetComponent<PlayerMovement>().enabled = false;
                dead = true;
            }

        }
        else
        {
            anim.SetTrigger("Hurt");
            StartCoroutine(Invunerability());
        }
    }

    public void AddHealth(float value)
    {
        currentHealth = Mathf.Clamp(currentHealth + value, 0, maxHP);
    }
    private IEnumerator Invunerability()
    {
        Physics2D.IgnoreLayerCollision(8, 9, true);
        for (int i = 0; i < numberOfFlashes; i++)
        {
            spriteRend.color = new Color(1, 0, 0, 0.5f);
            yield return new WaitForSeconds(iFramesDuration / (numberOfFlashes * 2));
            spriteRend.color = Color.white;
            yield return new WaitForSeconds(iFramesDuration / (numberOfFlashes * 2));
        }
        Physics2D.IgnoreLayerCollision(8, 9, false);
    }

    public float MaxHP()
    {
        return maxHP;
    }

    public bool IsDead()
    {
        return dead;
    }
}
