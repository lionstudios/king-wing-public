using System.Collections;
using UnityEngine;
using System;
#if UNITY_ANDROID
using Google.Play.Review;
#endif
using Utils;

public class InAppReviewManager : MonoBehaviour
{
#if UNITY_ANDROID
    ReviewManager _reviewManager;
    PlayReviewInfo _playReviewInfo;
    private Coroutine _reviewRoutine;

    private Dispatcher _dispatcher;

    public void Start()
    {
        _dispatcher = GameManager.Dispatcher;
        _dispatcher.Subscribe(EventId.InAppReviewRequest, ShowReview);
    }

    private void ShowReview(EventArgs a)
    {
        if (_reviewRoutine != null)
        {
            StopCoroutine(_reviewRoutine);
        }

        _reviewRoutine = StartCoroutine(RequestReview());
    }

    private void OnDestroy()
    {
        _dispatcher.Unsubscribe(EventId.InAppReviewRequest, ShowReview);
    }

    IEnumerator RequestReview()
    {
        Debug.Log("In App Review Started");

        _reviewManager = new ReviewManager();

        var requestFlowOperation = _reviewManager.RequestReviewFlow();
        yield return requestFlowOperation;
        if (requestFlowOperation.Error != ReviewErrorCode.NoError)
        {
            Debug.Log(requestFlowOperation.Error.ToString());
            yield break;
        }
        _playReviewInfo = requestFlowOperation.GetResult();

        var launchFlowOperation = _reviewManager.LaunchReviewFlow(_playReviewInfo);
        yield return launchFlowOperation;
        _playReviewInfo = null; // Reset the object
        if (launchFlowOperation.Error != ReviewErrorCode.NoError)
        {
            Debug.Log(requestFlowOperation.Error.ToString());
            yield break;
        }
    }
#endif
}