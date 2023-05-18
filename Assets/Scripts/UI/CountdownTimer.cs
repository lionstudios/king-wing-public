using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class CountdownTimer : MonoBehaviour
{
    public int countDownTimer = 3;
    public TextMeshProUGUI timerTxt;

    private Coroutine _counterRoutine;


    private readonly WaitForSecondsRealtime _delay = new WaitForSecondsRealtime(1);

    public void InitiateCountdown(int timer, Action onTimerComplete)
    {
        countDownTimer = timer;
        if (SatoriSDK.levelStartTimer != 0)
        {
            countDownTimer = SatoriSDK.levelStartTimer;
        }
        _counterRoutine = StartCoroutine(StartCountDown(onTimerComplete));
    }

    private IEnumerator StartCountDown(Action onTimerCompleted)
    {
        yield return new WaitForEndOfFrame();
        if (countDownTimer > 0)
        {
            timerTxt.text = countDownTimer.ToString();
            countDownTimer -= 1;
            yield return _delay;
            UIScreen gameScreen = UIManager.Instance.GetScreen(Screen.Game);
            yield return new WaitWhile(() => !gameScreen.gameObject.activeInHierarchy);
            _counterRoutine = GameManager.Instance.StartCoroutine(StartCountDown(onTimerCompleted));
        }
        else
        {
            timerTxt.text = "";
            Time.timeScale = 1f;
            onTimerCompleted?.Invoke();
            StopCoroutine(_counterRoutine);
            _counterRoutine = null;
        }
    }
}