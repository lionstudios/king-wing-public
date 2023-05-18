using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform targetTransform;
    public float smoothTime = 0.3f;

    [SerializeField] private Vector2 offset;
    [SerializeField] private float zOffset = -22;
    private Vector3 velocity = Vector3.zero;

    private Vector3 startPosition;

    [SerializeField] private Animator cameraRigAnimator;

    private void Start()
    {
        Vector3 currentPos = transform.position;
        currentPos.z = zOffset;
        transform.position = currentPos;
        startPosition = currentPos;
    }

    public void ToggleShopCameraState(bool isInShop)
    {
        cameraRigAnimator.SetBool("isPlaying", isInShop);
    }
    
    void Update()
    {
        Vector3 myPosition;
        Vector2 targetPosition = targetTransform.position;
        myPosition = targetPosition + offset;
        myPosition.z = zOffset;
        transform.position = Vector3.SmoothDamp(transform.position, myPosition, ref velocity, smoothTime);
    }

    public void ResetCameraPosition()
    {
        transform.position = startPosition;
    }
}