using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerSpellController : MonoBehaviour
{
    [Header("Spell Configuration")]
    public List<Spell> spells;
    public Transform castPoint;

    private int currentSpellIndex = 0;
    private float lastCastTime;
    private PlayerMovement playerMovement;

    public float healCooldown = 2f; 
    private float lastHealTime = -Mathf.Infinity;

   

    void Start()
    {
        playerMovement = GetComponent<PlayerMovement>();
    }

    void Update()
    {
        HandleSpellSwitching();
        HandleSpellCasting();
    }

    private void HandleSpellSwitching()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) SetCurrentSpellIndex(0);
        if (Input.GetKeyDown(KeyCode.Alpha2)) SetCurrentSpellIndex(1);
        if (Input.GetKeyDown(KeyCode.Alpha3)) SetCurrentSpellIndex(2);
        if (Input.GetKeyDown(KeyCode.Alpha4)) SetCurrentSpellIndex(3);
        if (Input.GetKeyDown(KeyCode.Alpha5)) SetCurrentSpellIndex(4);
        if (Input.GetKeyDown(KeyCode.Alpha6)) SetCurrentSpellIndex(5);
        if (Input.GetKeyDown(KeyCode.Alpha7)) SetCurrentSpellIndex(6);
        if (Input.GetKeyDown(KeyCode.Alpha8)) SetCurrentSpellIndex(7);

        HandleScrollWheelInput();
    }

    private void SetCurrentSpellIndex(int index)
    {
        if (index >= 0 && index < spells.Count)
        {
            currentSpellIndex = index;
        }
    }

    private void HandleScrollWheelInput()
    {
        if (Input.mouseScrollDelta.y != 0)
        {
            currentSpellIndex = (currentSpellIndex - (Input.mouseScrollDelta.y > 0 ? 1 : -1) + spells.Count) % spells.Count;
        }
    }

    private void HandleSpellCasting()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Spell currentSpell = spells[currentSpellIndex];
            if (Time.time >= lastCastTime + currentSpell.cooldown)
            {
                if (currentSpell.spellName == "Melee Attack")
                {
                    CastMeleeSpell();
                }
                else if (InventoryManager.Instance.UseMaterial(currentSpell.requiredMaterial, currentSpell.materialCost))
                {
                    CastSpell(currentSpell);
                    lastCastTime = Time.time;
                }
                else
                {
                    Debug.Log("Not enough materials!");
                }
            }
        }
    }

    private void CastMeleeSpell()
    {
        GetComponent<PlayerMelee>().CastMeleeSpell();
    }

    private void CastSpell(Spell spell)
    {
        Vector2 direction = playerMovement.GetFacingDirection();
        if (spell.spellName == "Acid Splash")
        {
            CastAcidSplash(direction, spell);
        }
        else if (spell.spellName == "Fireball")
        {
            CastFireball(direction, spell);
        }
        else if (spell.spellName == "Needles")
        {
            CastNeedles(spell);
        }
        else if (spell.spellName == "Freeze")
        {           
            CastFreezeCone(spell);
        }
        else if(spell.spellName == "Firebolt")
        {
            CastSingleProjectile(spell, direction);
        }
        else
        {
            CastHealSpell();
        }
    
    }

    private void CastAcidSplash(Vector2 direction, Spell spell)
    {
        float[] angles = { 5f, 20f, 35f };
        foreach (float angle in angles)
        {
            GameObject projectile = Instantiate(spell.spellPrefab, castPoint.position, Quaternion.identity);
            if (projectile != null)
            {
                ArcProjectile arcProjectileScript = projectile.GetComponent<ArcProjectile>();
                arcProjectileScript?.Initialize(direction, angle);
            }
        }
    }

    private void CastFireball(Vector2 direction, Spell spell)
    {
        GameObject fireball = Instantiate(spell.spellPrefab, castPoint.position, Quaternion.identity);
        Fireball fireballScript = fireball.GetComponent<Fireball>();
        if (fireballScript != null)
        {
            fireballScript.Initialize(direction);
        }
        Debug.Log($"{spell.spellName} cast!");
    }

    private void CastSingleProjectile(Spell spell, Vector2 direction)
    {
        GameObject projectile = Instantiate(spell.spellPrefab, castPoint.position, Quaternion.identity);
        Firebolt fireboltScript = projectile.GetComponent<Firebolt>();
        if (fireboltScript != null)
        {
            fireboltScript.Initialize(direction);
        }
        Debug.Log($"{spell.spellName} cast!");
    }

    private void CastNeedles(Spell spell)
    {
        Vector2 spawnPosition = (Vector2)castPoint.position + new Vector2(0, 1); 
        float delayIncrement = 0.3f; 
        float currentDelay = 0f;

        for (int i = 0; i < 3; i++)
        {
            Vector2 needleSpawnPosition = spawnPosition;

           
            if (i == 0) 
            {
                needleSpawnPosition.x -= 0.5f;
            }
            else if (i == 1) 
            {
                needleSpawnPosition.y += 0.3f; 
            }
            else if (i == 2) 
            {
                needleSpawnPosition.x += 0.5f;
            }

            GameObject needle = Instantiate(spell.spellPrefab, needleSpawnPosition, Quaternion.identity);
            Needle needleScript = needle.GetComponent<Needle>();

            if (needleScript != null)
            {
                needleScript.InitializeWithDelay(currentDelay);
            }

            currentDelay += delayIncrement; 
        }
    }

    private void CastFreezeCone(Spell spell)
    {
        GameObject freezeCone = Instantiate(spell.spellPrefab, castPoint.position, Quaternion.identity);
        freezeCone.transform.right = playerMovement.GetFacingDirection();
        Debug.Log($"{spell.spellName} cast!");

        
    }

    private void CastHealSpell()
    {
        PlayerMovement playerMovement = GetComponent<PlayerMovement>();
        PlayerHealth playerHealth = GetComponent<PlayerHealth>();
        Rigidbody2D rb = GetComponent<Rigidbody2D>();    

      
        StopPlayerMovement(playerMovement, rb);

      
        if (playerHealth != null)
        {
            if (playerHealth.currentHealth < playerHealth.maxHealth)
            {
                playerHealth.currentHealth = Mathf.Min(playerHealth.currentHealth + 1, playerHealth.maxHealth);
                playerHealth.UpdateHealthUI(); 
            }
        }

        Debug.Log("Heal spell cast! Healed 1 HP.");


        lastHealTime = Time.time;


        StartCoroutine(ResumeMovementAfterHeal(playerMovement, rb, 1f)); 
    }

    private void StopPlayerMovement(PlayerMovement playerMovement, Rigidbody2D rb)
    {
        if (playerMovement != null)
        {
   
            playerMovement.enabled = false; 
        }

     
        if (rb != null)
        {
            rb.constraints = RigidbodyConstraints2D.FreezePositionX; 
        }
    }

    private IEnumerator ResumeMovementAfterHeal(PlayerMovement playerMovement, Rigidbody2D rb, float duration)
    {
      
        yield return new WaitForSeconds(duration);

        if (playerMovement != null)
        {
            playerMovement.enabled = true; 
        }

        if (rb != null)
        {
            rb.constraints = RigidbodyConstraints2D.None; 
            rb.constraints = RigidbodyConstraints2D.FreezeRotation; 
        }
    }

    public int GetCurrentSpellIndex()
    {
        return currentSpellIndex;
    }

}
