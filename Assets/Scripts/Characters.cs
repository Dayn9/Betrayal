using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Characters : MonoBehaviour
{
    public static Player player { get; set; }
    public static Dog dog { get; set; }
    public static List<Sheep> sheep { get; set; }
    public static List<Person> people { get; set; }

    private static List<GameObject> targets { get; set; }

    private void Awake()
    {
        player = FindObjectOfType<Player>();

        dog = FindObjectOfType<Dog>();

        sheep = new List<Sheep>();
        sheep.AddRange(FindObjectsOfType<Sheep>());

        people = new List<Person>();
        people.AddRange(FindObjectsOfType<Person>());
    }
}

[RequireComponent(typeof(Movement))]
[RequireComponent(typeof(Animator))]
public abstract class Character : MonoBehaviour
{
    public Movement movement { get; private set; }
    protected Animator animator { get; private set; }
    protected SpriteRenderer renderer { get; private set; }

    public UnityEvent Attacked;

    private void Awake()
    {
        movement = GetComponent<Movement>();
        animator = GetComponent<Animator>();
        renderer = GetComponent<SpriteRenderer>();

        Attacked.AddListener(OnAttacked);
    }

    protected IEnumerator StayInFrame()
    {
        float height = Camera.main.orthographicSize;
        float width = height * Camera.main.aspect;
        Vector2 extents = new Vector2(width, height);

        Rect frame = new Rect(
            position: (Vector2)Camera.main.transform.position - extents, 
            size: extents * 2
        );

        while (true)
        {
            movement.StayInside(frame, 2);

            yield return new WaitForEndOfFrame();
        }
    }

    /// <summary>
    /// Generic Death Procedure plays particle effect and then destoys
    /// </summary>
    /// <returns></returns>
    protected IEnumerator Death()
    {
        ParticleSystem particleSystem = GetComponentInChildren<ParticleSystem>();
        Collider2D collider = GetComponent<Collider2D>();

        renderer.enabled = false;
        collider.enabled = false;

        if (particleSystem)
        {
            particleSystem.Play();

            yield return new WaitForSeconds(particleSystem.main.duration);
        }

        Destroy(gameObject);
    }

    protected virtual void OnAttacked()
    {
        throw new System.NotImplementedException();
    }
}
