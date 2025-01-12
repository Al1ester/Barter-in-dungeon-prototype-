using System.Collections.Generic;
using UnityEngine;

public class WebArea : MonoBehaviour
{
    public float lifetime = 3f;
    private List<PlayerMovement> playersInWeb = new List<PlayerMovement>();
    private void Start()
    {
      
        Destroy(gameObject, lifetime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerMovement player = collision.GetComponent<PlayerMovement>();
            if (player != null && !playersInWeb.Contains(player))
            {
                playersInWeb.Add(player);
                player.ApplySlow();
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerMovement player = collision.GetComponent<PlayerMovement>();
            if (player != null && playersInWeb.Contains(player))
            {
                playersInWeb.Remove(player);
                player.RemoveSlow();
            }
        }
    }

    private void OnDestroy()
    {

        foreach (PlayerMovement player in playersInWeb)
        {
            if (player != null) 
            {
                player.RemoveSlow();
            }
        }

        playersInWeb.Clear();
    }



}
