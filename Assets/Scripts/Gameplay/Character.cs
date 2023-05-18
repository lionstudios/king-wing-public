using System;
using UnityEngine;
using LionStudios.Suite.Analytics;
using Utils;

public class Character : MonoBehaviour
{
    [SerializeField] private float jumpSpeed = 1f;
    [SerializeField] private PathTraveller pathTraveller;
    [SerializeField] private Transform _trailParticleParent;
    [SerializeField] private SkinnedMeshRenderer _skin;
    [SerializeField] private Animator _animator;
    private Rigidbody2D _rigidbody;

    private Vector2 _initialPosition;

    private Camera _gameCamera;
    public float _screenLimitVertical;

    private Dispatcher _dispatcher;


    private static readonly int Flap = Animator.StringToHash("Flap");
    public static readonly int Run = Animator.StringToHash("Run");
    public static readonly int Sit = Animator.StringToHash("Sit");


    public ParticleSystem scoreParticle;
    public Transform trailParticle;

    private void Awake()
    {
        _dispatcher = GameManager.Dispatcher;
        _rigidbody = GetComponent<Rigidbody2D>();
        _gameCamera = Camera.main;
        _initialPosition = _rigidbody.position;
        _dispatcher.Subscribe(EventId.ShopItemEquippedChange, UpdateSkin);
        _dispatcher.Subscribe(EventId.ShopItemSelectedChange, UpdateSkin);
        _dispatcher.Subscribe(EventId.ResetLevel, ResetPlayer);
        GameManager.GamePlayStatus += FreezePlayerStatus;
        GameManager.GamePlayStatus += MakeLionRun;
    }

    private void OnDisable()
    {
        GameManager.GamePlayStatus -= FreezePlayerStatus;
        _dispatcher.Unsubscribe(EventId.ShopItemEquippedChange, UpdateSkin);
        _dispatcher.Unsubscribe(EventId.ShopItemSelectedChange, UpdateSkin);
        _dispatcher.Unsubscribe(EventId.ResetLevel, ResetPlayer);
    }
    
    private void Update()
    {
        if (!GameManager.Instance.IsStarted)
        {
            return;
        }

        if (Input.GetMouseButtonDown(0))
        {
            _rigidbody.velocity = Vector2.up * jumpSpeed;
            SetAnimatorStates(Flap);
        }

        ClampPlayerYPosition();
    }

    private void ClampPlayerYPosition()
    {
        Vector2 currentPosition = transform.localPosition;
        //currentPosition.x = Mathf.Lerp(currentPosition.x, transform.parent.position.x, Time.fixedDeltaTime/2f);
        currentPosition.y = Mathf.Clamp(currentPosition.y, -_screenLimitVertical * 2f, _screenLimitVertical);
        transform.localPosition = currentPosition;
    }

    private void UpdateSkin(EventArgs args)
    {
        ShopItemSwitch switchArgs = (ShopItemSwitch)args;

        SkinShopItem currentItem =switchArgs.explicitEquippedUse?
            (switchArgs.equipped ? (SkinShopItem)switchArgs.equipped : (SkinShopItem)switchArgs.selected): (SkinShopItem)switchArgs.selected;
        if (currentItem.skin)
        {
            Material[] materialArray = new Material[_skin.materials.Length];
            for (int i = 0; i < materialArray.Length; i++)
            {
                materialArray[i] = currentItem.skin;
            }

            _skin.materials = materialArray;
        }

        if (currentItem.trail)
        {
            if (trailParticle)
            {
                Destroy(trailParticle.gameObject);
            }

            trailParticle = Instantiate(currentItem.trail.transform, _trailParticleParent);
            SetTrailParticleStatus(false);
        }
        else
        {
            if (trailParticle)
            {
                Destroy(trailParticle.gameObject);
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Obstacles"))
        {
            Die();
        }
    }

    private void SetAnimatorStates(int id)
    {
        _animator.SetTrigger(id);
    }

    private void MakeLionRun(bool isLevelStarted)
    {
        if (isLevelStarted)
        {
            SetAnimatorStates(Run);
        }
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (!GameManager.Instance.IsStarted)
        {
            return;
        }

        if (col.transform.CompareTag("ScoreAdder"))
        {
            GameManager.Instance.IncrementScore();
            if (scoreParticle)
            {
                scoreParticle.Play();
            }
        }
        else if (col.transform.CompareTag("WinLine"))
        {
            GameManager.Instance.Succeed();
        }
    }

    private void Die()
    {
        if (!GameManager.Instance.IsStarted)
        {
            return;
        }

        SetTrailParticleStatus(false);
        LionAnalytics.MissionStep(false, "Normal", "Level", "LevelID", GameManager.Instance.LevelMoney);
        LionAnalytics.LevelStep(LevelManager.Instance.CurrentLevel, GameManager.Instance.Attempts, "Level", "Level",
            "Normal", "Level", GameManager.Instance.LevelMoney);
        GameManager.Instance.Fail();
    }

    private void SetTrailParticleStatus(bool status)
    {
        if (trailParticle)
        {
            trailParticle.gameObject.SetActive(status);
        }
    }

    private void ResetPlayer(EventArgs args)
    {
        pathTraveller.ResetToStartingPosition();
        transform.position = _initialPosition;
        // Debug.Log(_rigidbody.position);
        _rigidbody.velocity = Vector2.zero;
        SetAnimatorStates(Sit);
        SetTrailParticleStatus(false);

        if (!_gameCamera) return;
        if (_gameCamera.GetComponent<CameraController>())
        {
            _gameCamera.GetComponent<CameraController>().ResetCameraPosition();
        }
    }

    public void ToggleCameraShopPosition(bool isInShop)
    {
        CameraController cameraController = GameManager.Instance.cameraController;
        if (cameraController)
        {
            cameraController.ToggleShopCameraState(isInShop);
        }
    }

    private void FreezePlayerStatus(bool status)
    {
        _rigidbody.isKinematic = !status;
        _rigidbody.simulated = status;
        _rigidbody.bodyType = !status ? RigidbodyType2D.Kinematic : RigidbodyType2D.Dynamic;
        _animator.speed = status ? 1 : 0;
        SetTrailParticleStatus(status);
    }
}