using System;
using FpsDemo.Game;
using UnityEngine;

namespace FpsDemo.Player
{
    [RequireComponent(typeof(CharacterController))]
    public class PlayerMovement : MonoBehaviour
    {
        private PlayerInputReader _playerInputReader;
        private CharacterController _characterController;
    
        [Header("地面移动")]
        public float walkSpeed = 7f;
        public float groundAcceleration = 50f;

        [Header("空中移动")]
        public float maxAirSpeed = 7f;
        public float airAcceleration = 25f;
    
        [Header("跳跃")]
        public float jumpForce = 9f;
        public float gravityDownForce = 25f; 

        [Header("地面检测")]
        public LayerMask groundMask;
    
        private const float JumpGroundingPreventionTime = 0.2f;
        private const float GroundCheckDistance = 0.05f;
        private const float AirCheckDistance = 0.02f;
        private const float GroundStickDownSpeed = 3f;
    
        private float _lastJumpTime;
        public Vector3 Velocity { get; private set; }

        [Serializable]
        public struct GroundInfo
        {
            public bool isGrounded;
            public Vector3 normal;
            public float distanceToGround;
        
            public static GroundInfo InAir => new GroundInfo
            {
                isGrounded = false, 
                normal = Vector3.up, 
                distanceToGround = float.MaxValue
            };
        
            public GroundInfo(Vector3 normal, float distance)
            {
                this.isGrounded = true;
                this.normal = normal;
                this.distanceToGround = distance;
            }
        }
    
        [field: SerializeField]
        public GroundInfo CurrentGround { get; private set; }
    

        private void Awake()
        {
            _playerInputReader = GetComponent<PlayerInputReader>();
            _characterController = GetComponent<CharacterController>();
        }

        private void OnEnable()
        {
            if (_playerInputReader != null)
                _playerInputReader.OnJumpEvent += HandleJump;
        }

        private void OnDisable()
        {
            if (_playerInputReader != null)
                _playerInputReader.OnJumpEvent -= HandleJump;
        }

        private void Update()
        {
            CheckGrounded();
            HandleMove();
        }

        private void CheckGrounded()
        {
            bool wasGrounded = CurrentGround.isGrounded;
            CurrentGround = ProbeGround();
        
            //落地
            if (!wasGrounded && CurrentGround.isGrounded)
            {
                EventManager.Broadcast(new PlayerLandEvent { velocity = Math.Abs(Velocity.y) });
            }
        }

        private void HandleMove()
        {
            Vector2 input = _playerInputReader != null ? _playerInputReader.MoveInput : Vector2.zero;
            Vector3 worldSpaceMoveInput = input.x * transform.right + input.y * transform.forward;

            //地面
            if (CurrentGround.isGrounded)
            {
                Vector3 targetVelocity = walkSpeed * worldSpaceMoveInput;
                targetVelocity = targetVelocity.magnitude * GetDirectionReorientedOnSlope(targetVelocity.normalized, CurrentGround.normal);
                Velocity = Vector3.Lerp(Velocity, targetVelocity, groundAcceleration * Time.deltaTime);
            }
            //空中
            else
            {
                Velocity += airAcceleration * Time.deltaTime * worldSpaceMoveInput;
                float verticalVelocity = Velocity.y - gravityDownForce * Time.deltaTime;
                Vector2 horizontalVelocity = new Vector2(Velocity.x, Velocity.z);
                horizontalVelocity = Vector2.ClampMagnitude(horizontalVelocity, maxAirSpeed);
                Velocity = new Vector3(horizontalVelocity.x, verticalVelocity, horizontalVelocity.y);
            }
        
            //移动前位置
            Vector3 capsuleBottomBeforeMove = GetCapsuleBottomHemisphere();
            Vector3 capsuleTopBeforeMove = GetCapsuleTopHemisphere();
        
            //做移动
            Vector3 displacement = Velocity * Time.deltaTime;
            if (CurrentGround.isGrounded && Time.time >= _lastJumpTime + JumpGroundingPreventionTime)
            {
                //保证贴地
                if (CurrentGround.distanceToGround > _characterController.skinWidth)
                {
                    displacement += Vector3.down * CurrentGround.distanceToGround;
                }
                //移动时给一点向下速度，保证可以顺利上台阶
                displacement += GroundStickDownSpeed * Time.deltaTime * Vector3.down;
            }
            _characterController.Move(displacement);
        
            //保留撞墙的切向速度
            Vector3 collisionCapsuleBottom = capsuleBottomBeforeMove + Vector3.up * _characterController.stepOffset;
            float castDistance = Velocity.magnitude * Time.deltaTime + _characterController.skinWidth + 0.02f;
            if (collisionCapsuleBottom.y < capsuleTopBeforeMove.y)
            {
                if (Physics.CapsuleCast(
                        collisionCapsuleBottom,
                        capsuleTopBeforeMove,
                        _characterController.radius,
                        Velocity.normalized,
                        out RaycastHit hit,
                        castDistance,
                        groundMask, 
                        QueryTriggerInteraction.Ignore))
                {
                    Velocity = Vector3.ProjectOnPlane(Velocity, hit.normal);
                }
            }
        }

        private void HandleJump(bool isPressed)
        {
            if (isPressed)
            {
                if (!CurrentGround.isGrounded) return;
                Velocity = new Vector3(Velocity.x, jumpForce, Velocity.z);
                _lastJumpTime = Time.time;
                CurrentGround = GroundInfo.InAir;
            }
        }

        private GroundInfo ProbeGround()
        {
            //刚跳起不检测
            if (Time.time < _lastJumpTime + JumpGroundingPreventionTime)  return GroundInfo.InAir;
        
            float checkDistance = _characterController.skinWidth + (CurrentGround.isGrounded ? GroundCheckDistance : AirCheckDistance);
            //用球面检测，缩小一点半径
            float probeRadius = _characterController.radius * 0.9f; 
            Vector3 sphereCenter = GetCapsuleBottomHemisphere() + Vector3.up * (probeRadius - _characterController.radius);
            bool isGrounded = Physics.SphereCast(
                sphereCenter,
                probeRadius, 
                Vector3.down, 
                out RaycastHit hit, 
                checkDistance, 
                groundMask, 
                QueryTriggerInteraction.Ignore
            );
        
            isGrounded = isGrounded 
                         && Vector3.Dot(hit.normal, transform.up) > 0f 
                         && Vector3.Angle(transform.up, hit.normal) <= _characterController.slopeLimit;

            return isGrounded ? new GroundInfo(hit.normal, hit.distance) : GroundInfo.InAir;
        }
    
        private Vector3 GetCapsuleBottomHemisphere()
        {
            Vector3 capsuleCenter = transform.position + _characterController.center;
            return capsuleCenter + Vector3.down * (_characterController.height / 2f - _characterController.radius);
        }
    
        private Vector3 GetCapsuleTopHemisphere()
        {
            Vector3 capsuleCenter = transform.position + _characterController.center;
            return capsuleCenter + Vector3.up * (_characterController.height / 2f - _characterController.radius);
        }

        private Vector3 GetDirectionReorientedOnSlope(Vector3 direction, Vector3 slopeNormal)
        {
            Vector3 directionRight = Vector3.Cross(direction, transform.up);
            return Vector3.Cross(slopeNormal, directionRight).normalized;
        }
    }
}