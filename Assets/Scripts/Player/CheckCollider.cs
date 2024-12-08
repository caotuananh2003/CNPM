using UnityEngine;
using System.Collections;

[RequireComponent(typeof(BoxCollider2D))]
public class CheckCollider : MonoBehaviour {

    public LayerMask collisionMask;
    const float skinWidth = .015f;
    public int horizontalRayCount = 4;
    public int verticalRayCount = 4;
    public float rayLengthHor = 0;
    public float rayLengthVer = 0;
    public Transform _pos1,_pos2;
    public float _MinDistance = 0.5f;
    public bool _CollectMap = false;
    public float _Destince = 0;


    float horizontalRaySpacing;
    float verticalRaySpacing;

    BoxCollider2D collider;
    RaycastOrigins raycastOrigins;
    public CollisionInfo collisions;

    void Start()
    {
      
    }
    public bool CheckCollectMap()
    {
        collider = GetComponent<BoxCollider2D>();
        CalculateRaySpacing();
        Move(_pos1.position - _pos2.position);
        return _CollectMap;
    }
    void Update()
    {
        CheckCollectMap();
    }
    public void Move(Vector3 velocity)
    {
        UpdateRaycastOrigins();
        collisions.Reset();

        if (velocity.x != 0)
        {
            HorizontalCollisions(ref velocity);
        }
        if (velocity.y != 0)
        {
            VerticalCollisions(ref velocity);
        }
    }
   
    void HorizontalCollisions(ref Vector3 velocity)
    {
        float directionX = Mathf.Sign(velocity.x);
        for (int i = 0; i < horizontalRayCount; i++)
        {
            Vector2 rayOrigin = (directionX == -1) ? _pos2.position : _pos2.position;
           // rayOrigin += Vector2.up * (horizontalRaySpacing * i);
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, velocity, rayLengthHor, collisionMask);

            Debug.DrawRay(rayOrigin, velocity  * rayLengthHor, Color.red);
            if (hit)
            {
                velocity.x = (hit.distance - skinWidth) * directionX;
                if (hit.distance<_MinDistance)
                {
                    _CollectMap = true;
                }
                collisions.left = directionX == -1;
                collisions.right = directionX == 1;
            }
        }
    }

    void VerticalCollisions(ref Vector3 velocity)
    {
        float directionY = Mathf.Sign(velocity.y);
        for (int i = 0; i < verticalRayCount; i++)
        {
            Vector2 rayOrigin = (directionY == -1) ? raycastOrigins.bottomLeft : raycastOrigins.topLeft;
            rayOrigin += Vector2.right * (verticalRaySpacing * i + velocity.x);
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up * directionY, rayLengthVer, collisionMask);

            Debug.DrawRay(rayOrigin, velocity * directionY * rayLengthVer, Color.red);

            if (hit)
            {
                velocity.y = (hit.distance - skinWidth) * directionY;
                rayLengthVer = hit.distance;

                collisions.below = directionY == -1;
                collisions.above = directionY == 1;
            }
        }
    }

    void UpdateRaycastOrigins()
    { 
        Bounds bounds = collider.bounds;
        bounds.Expand(skinWidth * -2);

        raycastOrigins.bottomLeft = new Vector2(bounds.min.x, bounds.min.y);
        raycastOrigins.bottomRight = new Vector2(bounds.max.x, bounds.min.y);
        raycastOrigins.topLeft = new Vector2(bounds.min.x, bounds.max.y);
        raycastOrigins.topRight = new Vector2(bounds.max.x, bounds.max.y);
    }

    void CalculateRaySpacing()
    {
        Bounds bounds = collider.bounds;
        bounds.Expand(skinWidth * -2);

        horizontalRayCount = Mathf.Clamp(horizontalRayCount, 0, int.MaxValue);
        verticalRayCount = Mathf.Clamp(verticalRayCount, 0, int.MaxValue);

        horizontalRaySpacing = bounds.size.y / (horizontalRayCount - 1);
        verticalRaySpacing = bounds.size.x / (verticalRayCount - 1);
    }

    struct RaycastOrigins
    {
        public Vector2 topLeft, topRight;
        public Vector2 bottomLeft, bottomRight;
    }

    public struct CollisionInfo
    {
        public bool above, below;
        public bool left, right;

        public void Reset()
        {
            above = below = false;
            left = right = false;
        }
    }
}
