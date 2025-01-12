using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class HealthBar : MonoBehaviour
{
    [Header("Transform")]
    public GameObject heartPrefab;
    public Transform heartContainer;

    [Header("Heart Settings")]
    public float heartScale = 1f;  
    public float distance = 125f; 

    private List<GameObject> hearts = new List<GameObject>();

    private void OnEnable()
    {
        PlayerHealth.HealthChanged += UpdateHearts;
    }

    private void OnDisable()
    {
        PlayerHealth.HealthChanged -= UpdateHearts;
    }

    public void UpdateHearts(int currentHealth, int maxHealth)
    {
        Debug.Log($"Updating health UI. Current: {currentHealth}, Max: {maxHealth}");

        foreach (GameObject heart in hearts)
        {
            Destroy(heart);
        }

        hearts.Clear();

        for (int i = 0; i < maxHealth; i++)
        {
            GameObject newHeart = Instantiate(heartPrefab, heartContainer);
            newHeart.transform.localScale = Vector3.one * heartScale; 
            newHeart.transform.localPosition = new Vector3(i * distance, 0, 0); 

            Image heartImage = newHeart.GetComponent<Image>();
            if (heartImage != null)
            {
                heartImage.color = i < currentHealth ? Color.white : Color.gray;
            }
            hearts.Add(newHeart);
        }
    }
}
