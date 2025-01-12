using UnityEngine;

public class PlayerMelee : MonoBehaviour
{
    public GameObject meleePrefab; 
    public Transform meleePoint; 
    public float spellCooldown = 0.5f; 
    public float damage = 10f; 

    private float lastMeleeTime; 
    public string spellName = "Melee Attack"; 

    private GameObject meleeEffectInstance; 

    public void CastMeleeSpell()
    {
        if (Time.time >= lastMeleeTime + spellCooldown)
        {
            if (meleeEffectInstance != null)
            {
                Destroy(meleeEffectInstance);
            }

            meleeEffectInstance = Instantiate(meleePrefab, meleePoint.position, Quaternion.identity, meleePoint);

            SpriteRenderer spriteRenderer = meleeEffectInstance.GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                PlayerMovement playerMovement = GetComponent<PlayerMovement>();
                bool isFacingLeft = playerMovement.IsFacingLeft();
                Debug.Log("Is facing left: " + isFacingLeft);

                spriteRenderer.flipX = isFacingLeft; 
                meleeEffectInstance.transform.localScale = new Vector3(
                    0.3f * (isFacingLeft ? -1 : 1), 
                    0.3f,
                    1
                );

                Debug.Log(isFacingLeft ? "Flipping sprite to the left" : "Keeping sprite normal");
            }

            Destroy(meleeEffectInstance, 0.2f);
            lastMeleeTime = Time.time;
        }
    }


}
