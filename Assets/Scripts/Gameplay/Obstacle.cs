using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class Obstacle : MonoBehaviour
{
    public enum Direction
    {
        LeftToRight,
        RightToLeft
    }

    private Direction _direction;

    [SerializeField] private float _startDistance = 5f;

    [SerializeField] private float _xSpeed = 1f;

    [SerializeField] private Transform _topWall;

    [SerializeField] private Transform _bottomWall;

    [SerializeField] private Collider2D _winningCol;

    [SerializeField] private SpriteRenderer _topWallSR;

    private float _height;

    private float _opening;

    private Rigidbody2D _rigidbody;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _rigidbody.position = Vector2.right * 1000f;
        GameManager.GamePlayStatus += FreezeObstacle;
    }

    public void Init(float height, Direction direction, float opening)
    {
        _height = height;
        _direction = direction;
        _opening = opening;
    }

    private void OnDestroy()
    {
        GameManager.GamePlayStatus -= FreezeObstacle;
    }

    private void Start()
    {
        var topWallPos = _topWall.position;
        var bottomWallPos = _bottomWall.position;
        _topWall.position = new Vector3(topWallPos.x, _opening * 0.5f, topWallPos.z);
        _bottomWall.position = new Vector3(bottomWallPos.x, -_opening * 0.5f, bottomWallPos.z);
        var dirSign = _direction == Direction.LeftToRight ? 1 : -1;
        var winningColTf = _winningCol.transform;
        var offset = dirSign * (_topWallSR.bounds.extents.x);//+ Character.Bounds.size.x + _winningCol.bounds.extents.x);


        _startDistance = Camera.main.ViewportToWorldPoint(new Vector3(1.1f, 0f, 0f)).x;
        winningColTf.position = new Vector3(topWallPos.x - offset, (topWallPos.y + bottomWallPos.y) / 2,
            winningColTf.position.z);
        _rigidbody.position = new Vector2(_startDistance * (_direction == Direction.LeftToRight ? -1f : 1f), _height);
        _rigidbody.velocity = _xSpeed * (_direction == Direction.LeftToRight ? Vector2.right : Vector2.left);
    }

    private void FreezeObstacle(bool status)
    {
        _rigidbody.isKinematic = !status;
        _rigidbody.simulated = status;
        _rigidbody.bodyType = !status ? RigidbodyType2D.Static : RigidbodyType2D.Kinematic;
    }
}