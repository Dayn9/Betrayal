using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(SpriteRenderer))]
public class Movement : MonoBehaviour
{
    [SerializeField] private float speed;

    private const float DRAG = 0.5f;

    private Rigidbody2D RB { get; set; }
    private SpriteRenderer Render { get; set; }

    public float Speed { get { return speed * Time.deltaTime * 10; } }
    public Vector2 Input { private get; set; }

    private void Awake()
    {
        RB = GetComponent<Rigidbody2D>();
        Render = GetComponent<SpriteRenderer>();

        Input = Vector2.zero;
    }

    private void Update()
    {
        //flip the sprite when velocity is significant
        if (RB.velocity.x > 0.01)
            Render.flipX = true;
        else if (RB.velocity.x < -0.01)
            Render.flipX = false;

        Render.sortingOrder = -(int)(transform.position.y * 100);
    }

    public void Pursue(Movement target)
    {
        Vector2 desired = ((Vector2)target.transform.position + target.RB.velocity - (Vector2)transform.position).normalized * Speed;
        Input += Vector2.Lerp(RB.velocity, desired, 0.5f);
    }

    public void Seek(Movement target)
    {
        Vector2 desired = ((Vector2)target.transform.position - (Vector2)transform.position).normalized * Speed;
        Input += Vector2.Lerp(RB.velocity, desired, 0.5f);
    }

    public void Flee(Movement target)
    {
        Vector2 desired = -((Vector2)target.transform.position - (Vector2)transform.position).normalized * Speed;
        Input += Vector2.Lerp(RB.velocity, desired, 0.5f);
    }

    public void Evade(Movement target)
    {
        Vector2 desired = -((Vector2)target.transform.position + target.RB.velocity - (Vector2)transform.position).normalized * Speed;
        Input += Vector2.Lerp(RB.velocity, desired, 0.5f);
    }

    public void StayInside(Rect rect, float border)
    {
        Vector2 velocity = RB.velocity;
        float magnitude = velocity.magnitude;

        if (transform.position.y > rect.yMax - border)
        {
            if (velocity.y > 0 || false)
            {
                velocity.y = 0;
                velocity.x = Mathf.Sign(velocity.x) * magnitude;
            }

            velocity.y -= Speed;
        }
            
        if (transform.position.y < rect.yMin + border) {
            if (velocity.y < 0 || false)
            {
                velocity.y = 0;
                velocity.x = Mathf.Sign(velocity.x) * magnitude;

            }
            velocity.y += Speed;
        }
        if (transform.position.x > rect.xMax - border) {
            if (velocity.x > 0 || false)
            {
                velocity.y = Mathf.Sign(velocity.y) * magnitude;
                velocity.x = 0;
            }
            velocity.x -= Speed;
        }
        if (transform.position.x < rect.xMin + border) {
            if (velocity.x < 0 || false)
            {
                velocity.y = Mathf.Sign(velocity.y) * magnitude;
                velocity.x = 0;
            }
            velocity.x += Speed;
        }

        RB.velocity = velocity;
    }

    private void FixedUpdate()
    {
        //Slow down
        RB.velocity *= (1 - DRAG);

        //Increase velocity by input
        RB.velocity += Input / DRAG;

        //Clamp Velocity to max speed
        RB.velocity = Vector2.ClampMagnitude(RB.velocity, speed);

        Debug.DrawLine(transform.position, (Vector2)transform.position + RB.velocity, Color.blue);
    }
}
