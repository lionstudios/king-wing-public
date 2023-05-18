using UnityEngine;

public class PathTraveller : MonoBehaviour
{
    [SerializeField] private float movementSpeed = 1f;

    // Update is called once per frame
    void Update()
    {
        if (!GameManager.Instance.IsStarted)
        {
            return;
        }
        transform.Translate(Vector2.right * (movementSpeed * Time.deltaTime));
    }

    public void ResetToStartingPosition()
    {
        transform.position = Vector3.zero;
    }
}