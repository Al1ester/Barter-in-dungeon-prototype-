using UnityEngine;

public class PlatformController : MonoBehaviour
{
    private PlatformEffector2D effector;

    private void Start()
    {
        effector = GetComponent<PlatformEffector2D>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
           
            effector.rotationalOffset = 180f;
            Invoke("ResetPlatform", 0.3f); 
        }
    }

    private void ResetPlatform()
    {
        effector.rotationalOffset = 0f;
    }
}
