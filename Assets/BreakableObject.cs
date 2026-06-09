using UnityEngine;

public class BreakableObject : MonoBehaviour
{
    [SerializeField] private float requiredVelocity = 3.0f;
    [SerializeField] private int scoreValue = 10;

    private bool isDestroyed = false;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Floor") && !isDestroyed)
        {
            if (collision.relativeVelocity.magnitude >= requiredVelocity)
            {
                DestroyObject();
            }
        }
    }

    private void DestroyObject()
    {
        isDestroyed = true;

        if (GameManager.instance != null)
        {
            GameManager.instance.AddScore(scoreValue);
        }

        Debug.Log("Object destroyed: " + gameObject.name);

        Destroy(gameObject, 0.1f);
    }
}