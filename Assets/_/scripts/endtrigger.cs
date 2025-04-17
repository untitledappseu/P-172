using UnityEngine;

public class EndTrigger : MonoBehaviour
{
    [SerializeField] private GameObject uiObject;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (uiObject != null)
            {
                uiObject.SetActive(true);
            }
        }
    }
}