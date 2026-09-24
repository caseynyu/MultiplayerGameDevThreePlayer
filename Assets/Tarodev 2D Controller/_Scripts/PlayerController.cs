using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace TarodevController
{
    /// <summary>
    /// Hey!
    /// Tarodev here. I built this controller as there was a severe lack of quality & free 2D controllers out there.
    /// I have a premium version on Patreon, which has every feature you'd expect from a polished controller. Link: https://www.patreon.com/tarodev
    /// You can play and compete for best times here: https://tarodev.itch.io/extended-ultimate-2d-controller
    /// If you hve any questions or would like to brag about your score, come to discord: https://discord.gg/tarodev
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
    public class PlayerController : MonoBehaviour, IPlayerController
    {
        [SerializeField] private ScriptableStats _stats;
        private Rigidbody2D _rb;
        private CapsuleCollider2D _col;
        private FrameInput _frameInput;
        private Vector2 _frameVelocity;
        private bool _cachedQueryStartInColliders;

        public Gamepad currentGamepad;
        public GameManager gameManager;

        #region Interface

        public Vector2 FrameInput => _frameInput.Move;
        public event Action<bool, float> GroundedChanged;
        public event Action Jumped;

        #endregion

        private float _time;

        [SerializeField] private bool keyboardEnabled=false;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
            _col = GetComponent<CapsuleCollider2D>();

            _cachedQueryStartInColliders = Physics2D.queriesStartInColliders;
        }

        private void Update()
        {
            _time += Time.deltaTime;
            GatherInput();
        }

        private void GatherInput()
        {
            Vector2 moveInput = Vector2.zero;
            bool jumpDown = false;
            bool jumpHeld = false;

            // Keyboard input
            if (Keyboard.current != null && keyboardEnabled==true)
            {
                if (Keyboard.current.aKey.isPressed ||
                    Keyboard.current.leftArrowKey.isPressed)
                {
                    moveInput.x -= 1;
                }

                if (Keyboard.current.dKey.isPressed ||
                    Keyboard.current.rightArrowKey.isPressed)
                {
                    moveInput.x += 1;
                }

                if (Keyboard.current.sKey.isPressed ||
                    Keyboard.current.downArrowKey.isPressed)
                {
                    moveInput.y -= 1;
                }

                if (Keyboard.current.wKey.isPressed ||
                    Keyboard.current.upArrowKey.isPressed)
                {
                    moveInput.y += 1;
                }

                jumpDown =
                    Keyboard.current.spaceKey.wasPressedThisFrame ||
                    Keyboard.current.cKey.wasPressedThisFrame;

                jumpHeld =
                    Keyboard.current.spaceKey.isPressed ||
                    Keyboard.current.cKey.isPressed;
            }

            // Controller input
            if (currentGamepad != null)
            {
                Vector2 gamepadMove =
                    currentGamepad.leftStick.ReadValue() +
                    currentGamepad.dpad.ReadValue();

                if (gamepadMove.sqrMagnitude > moveInput.sqrMagnitude)
                    moveInput = Vector2.ClampMagnitude(gamepadMove, 1f);

                jumpDown |= currentGamepad.buttonSouth.wasPressedThisFrame;
                jumpHeld |= currentGamepad.buttonSouth.isPressed;
            }

            _frameInput = new FrameInput
            {
                JumpDown = jumpDown,
                JumpHeld = jumpHeld,
                Move = Vector2.ClampMagnitude(moveInput, 1f)
            };

            if (_stats.SnapInput)
            {
                _frameInput.Move.x =
                    Mathf.Abs(_frameInput.Move.x) < _stats.HorizontalDeadZoneThreshold
                        ? 0
                        : Mathf.Sign(_frameInput.Move.x);

                _frameInput.Move.y =
                    Mathf.Abs(_frameInput.Move.y) < _stats.VerticalDeadZoneThreshold
                        ? 0
                        : Mathf.Sign(_frameInput.Move.y);
            }

            if (_frameInput.JumpDown)
            {
                _jumpToConsume = true;
                _timeJumpWasPressed = _time;
            }
        }

        private void FixedUpdate()
        {
            CheckCollisions();

            HandleJump();
            HandleDirection();
            HandleGravity();
            HandleWallSlide();
            ApplyMovement();
        }

        #region Collisions
        
        private float _frameLeftGrounded = float.MinValue;
        private bool _grounded;
		private bool _onIce;
		private bool _onWall;
		private int _wallDirection;
        //check if you're on a wall and which way is away from that wall

        private void CheckCollisions()
        {
            Physics2D.queriesStartInColliders = false;

            // Ground and Ceiling
            RaycastHit2D groundHit = Physics2D.CapsuleCast(
    _col.bounds.center,
    _col.size,
    _col.direction,
    0,
    Vector2.down,
    _stats.GrounderDistance,
    ~_stats.PlayerLayer
);

            // Do not re-ground on the spring while the launch is still moving upward.
            bool groundDetected = groundHit.collider != null && (!_springLaunched || _frameVelocity.y <= 0f);
            //check if you're on ice:
            _onIce =
                groundDetected &&
                groundHit.collider.gameObject.CompareTag("Ice");
            //Ceiling detection
            bool ceilingHit = Physics2D.CapsuleCast(_col.bounds.center, _col.size, _col.direction, 0, Vector2.up, _stats.GrounderDistance, ~_stats.PlayerLayer);
            //walls			
            bool leftWallHit = Physics2D.CapsuleCast(_col.bounds.center, _col.size, _col.direction, 0, Vector2.left, _stats.GrounderDistance, ~_stats.PlayerLayer); 
			bool rightWallHit = Physics2D.CapsuleCast(_col.bounds.center, _col.size, _col.direction, 0, Vector2.right, _stats.GrounderDistance, ~_stats.PlayerLayer);
            // Use this tick's ground result and clear wall contact when we leave it.
            _onWall = !groundDetected && (leftWallHit || rightWallHit);
            _wallDirection = _onWall ? (leftWallHit ? -1 : 1) : 0;
			// Hit a Ceiling
            if (ceilingHit) _frameVelocity.y = Mathf.Min(0, _frameVelocity.y);

            // Landed on the Ground
            if (!_grounded && groundDetected)
            {
                _grounded = true;
                _coyoteUsable = true;
                _bufferedJumpUsable = true;
                _endedJumpEarly = false;
                GroundedChanged?.Invoke(true, Mathf.Abs(_frameVelocity.y));
            }
            // Left the Ground
            else if (_grounded && !groundDetected)
            {
                _grounded = false;
                _frameLeftGrounded = _time;
                GroundedChanged?.Invoke(false, 0);
            }

            if (groundDetected && _frameVelocity.y <= 0f)
            {
                var spring = groundHit.collider.GetComponent<SpringBlock>();
                if (spring != null && spring.isActiveAndEnabled)
                    LaunchFromSpring(spring.JumpHeightMultiplier);
            }

            Physics2D.queriesStartInColliders = _cachedQueryStartInColliders;
        }

        void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.gameObject.CompareTag("Spike"))
            {
                //Debug.Log("Spike Entered");
                gameManager.Death(gameObject);
            }
            if (collision.gameObject.CompareTag("Win"))
            {
                gameManager.PlayerWin(gameObject);
            }
            if (collision.gameObject.CompareTag("RedWin"))
            {
                if (currentGamepad == Gamepad.all[0])
                {
                    gameManager.PlayerWin(gameObject);
                }
            }
            if (collision.gameObject.CompareTag("GreenWin"))
            {
                if (currentGamepad == Gamepad.all[1])
                {
                    gameManager.PlayerWin(gameObject);
                }
            }
            if (collision.gameObject.CompareTag("BlueWin"))
            {
                if (currentGamepad == Gamepad.all[2])
                {
                    gameManager.PlayerWin(gameObject);
                }
            }
            
        }

        #endregion


        #region Jumping

        private bool _jumpToConsume;
        private bool _bufferedJumpUsable;
        private bool _endedJumpEarly;
        private bool _springLaunched;
        private bool _coyoteUsable;
        private float _timeJumpWasPressed;

        private bool HasBufferedJump => _bufferedJumpUsable && _time < _timeJumpWasPressed + _stats.JumpBuffer;
        private bool CanUseCoyote => _coyoteUsable && !_grounded && _time < _frameLeftGrounded + _stats.CoyoteTime;

private void HandleJump()
{
    if (_springLaunched)
    {
        // Springs give a full bounce even when jump is released or pressed on landing.
        if (_frameVelocity.y > 0f)
        {
            _jumpToConsume = false;
            return;
        }
        _springLaunched = false;
    }

    if (!_endedJumpEarly &&
        !_grounded &&
        !_frameInput.JumpHeld &&
        _rb.linearVelocity.y > 0)
    {
        _endedJumpEarly = true;
    }

    if (!_jumpToConsume && !HasBufferedJump)
        return;

    if (_onWall)
    {
        ExecuteWallJump();
    }
    else if (_grounded || CanUseCoyote)
    {
        ExecuteJump();
    }

    _jumpToConsume = false;
}
private void HandleWallSlide()
{
    if (_onWall && _frameVelocity.y < 0f)
    {
        _frameVelocity.y = Mathf.Max(
            _frameVelocity.y,
            -_stats.WallSlideSpeed
        );
    }
}
private void ExecuteWallJump()
{
    _endedJumpEarly = false;
    _timeJumpWasPressed = 0;
    _bufferedJumpUsable = false;
    _coyoteUsable = false;

    // Multiply by negative wall direction to move away from the wall.
    _frameVelocity.x = -_wallDirection * _stats.WallJumpPower.x;
    _frameVelocity.y = _stats.WallJumpPower.y;

    Jumped?.Invoke();
}
        private void LaunchFromSpring(float heightMultiplier)
        {
            _springLaunched = true;
            _endedJumpEarly = false;
            _jumpToConsume = false;
            _timeJumpWasPressed = float.NegativeInfinity;
            _bufferedJumpUsable = false;
            _coyoteUsable = false;
            _onWall = false;
            _onIce = false;
            if (_grounded)
            {
                _grounded = false;
                _frameLeftGrounded = _time;
                GroundedChanged?.Invoke(false, 0f);
            }
            // Height scales with velocity squared, so 2.5x height needs sqrt(2.5)x speed.
            _frameVelocity.y = _stats.JumpPower * Mathf.Sqrt(Mathf.Max(1f, heightMultiplier));
            Jumped?.Invoke();
        }

        private void ExecuteJump()
        {
            _endedJumpEarly = false;
            _timeJumpWasPressed = 0;
            _bufferedJumpUsable = false;
            _coyoteUsable = false;
            _frameVelocity.y = _stats.JumpPower;
            Jumped?.Invoke();
        }

        #endregion

        #region Horizontal

        private void HandleDirection()
        {
            bool groundedOnIce = _grounded && _onIce;
            if (_frameInput.Move.x == 0)
            {
                var deceleration = !_grounded ? _stats.AirDeceleration
                    : groundedOnIce ? _stats.IceDeceleration : _stats.GroundDeceleration;
                _frameVelocity.x = Mathf.MoveTowards(_frameVelocity.x, 0, deceleration * Time.fixedDeltaTime);
            }
            else
            {
                var acceleration = groundedOnIce ? _stats.IceAcceleration : _stats.Acceleration;
                _frameVelocity.x = Mathf.MoveTowards(_frameVelocity.x, _frameInput.Move.x * _stats.MaxSpeed, acceleration * Time.fixedDeltaTime);
            }
        }

        #endregion

        #region Gravity

        private void HandleGravity()
        {
            if (_grounded && _frameVelocity.y <= 0f)
            {
                _frameVelocity.y = _stats.GroundingForce;
            }
            else
            {
                var inAirGravity = _stats.FallAcceleration;
                if (_endedJumpEarly && _frameVelocity.y > 0) inAirGravity *= _stats.JumpEndEarlyGravityModifier;
                _frameVelocity.y = Mathf.MoveTowards(_frameVelocity.y, -_stats.MaxFallSpeed, inAirGravity * Time.fixedDeltaTime);
            }
        }

        #endregion

        private void ApplyMovement() => _rb.linearVelocity = _frameVelocity;

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (_stats == null) Debug.LogWarning("Please assign a ScriptableStats asset to the Player Controller's Stats slot", this);
        }
#endif
    }

    public struct FrameInput
    {
        public bool JumpDown;
        public bool JumpHeld;
        public Vector2 Move;
    }

    public interface IPlayerController
    {
        public event Action<bool, float> GroundedChanged;

        public event Action Jumped;
        public Vector2 FrameInput { get; }
    }
}