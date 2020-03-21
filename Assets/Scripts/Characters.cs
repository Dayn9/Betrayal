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
    public static List<Chicken> chicken { get; set; }
    public static List<Crow> crows { get; set; }

    private void Awake()
    {
        player = FindObjectOfType<Player>();

        dog = FindObjectOfType<Dog>();

        sheep = new List<Sheep>();
        sheep.AddRange(FindObjectsOfType<Sheep>());

        people = new List<Person>();
        people.AddRange(FindObjectsOfType<Person>());

        chicken = new List<Chicken>();
        chicken.AddRange(FindObjectsOfType<Chicken>());

        crows = new List<Crow>();
        crows.AddRange(FindObjectsOfType<Crow>());
    }
}

[RequireComponent(typeof(Movement))]
[RequireComponent(typeof(Animator))]
public abstract class Character : MonoBehaviour
{
    public Movement movement { get; private set; }
    protected Animator animator { get; private set; }
    protected SpriteRenderer render { get; private set; }

    public UnityEvent Attacked;

    /// <summary>
    /// Camera frame
    /// </summary>
    private static Rect frame;
    protected static Rect Frame
    {
        get
        {
            //check if the frame has been set yet
            if (frame.width == 0)
            {
                //calculate the frame from camera size
                float height = Camera.main.orthographicSize;
                float width = height * Camera.main.aspect;
                Vector2 extents = new Vector2(width, height);

                frame = new Rect(
                    position: (Vector2)Camera.main.transform.position - extents,
                    size: extents * 2
                );
            }
            return frame;
        }
    }

    private void Awake()
    {
        movement = GetComponent<Movement>();
        animator = GetComponent<Animator>();
        render = GetComponent<SpriteRenderer>();

        Attacked.AddListener(OnAttacked);
    }

    /// <summary>
    /// Stay in the bounds of the camera
    /// </summary>
    /// <returns></returns>
    protected IEnumerator StayInFrame()
    {
        while (true)
        {
            movement.StayInside(Frame, 2);

            yield return new WaitForEndOfFrame();
        }
    }

    /// <summary>
    /// Generic Death Procedure plays particle effect and then destoys
    /// </summary>
    /// <returns></returns>
    protected IEnumerator Death()
    {
        render.enabled = false;

        Collider2D collider = GetComponent<Collider2D>();
        collider.enabled = false;

        movement.Input = Vector2.zero;

        ParticleSystem particleSystem = GetComponentInChildren<ParticleSystem>();
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

    /// <summary>
    /// Finds the Closest character of specified type
    /// </summary>
    /// <typeparam name="T">Character type</typeparam>
    /// <returns>closest character of type</returns>
    protected T Closest<T>() where T : Character
    {
        List<T> characters = null;

        //get the list based of specified type
        if (typeof(T).Equals(typeof(Sheep)))
        {
            characters = Characters.sheep as List<T>;
        }
        else if (typeof(T).Equals(typeof(Person)))
        {
            characters = Characters.people as List<T>;
        }
        else if (typeof(T).Equals(typeof(Chicken)))
        {
            characters = Characters.chicken as List<T>;
        }
        else if(typeof(T).Equals(typeof(Crow)))
        {
            characters = Characters.crows as List<T>;
        }
        else if (typeof(T).Equals(typeof(Dog)))
        {
            return Characters.dog as T;
        }
        else if (typeof(T).Equals(typeof(Player)))
        {
            return Characters.player as T;
        }

        //make sure the list has elements
        if (characters != null && characters.Count > 0 )
        {
            float closestDist = float.MaxValue;
            T closest = null;

            //loop over list
            foreach (T character in characters)
            {
                if (character.Equals(this)) continue; //skip self

                //get the difference and set if closer
                Vector2 diffSheep = character.transform.position - transform.position;
                if (diffSheep.sqrMagnitude < closestDist)
                    closest = character;
            }

            //return the closest
            return closest;
        }
        //character not found
        return null;
    }
}
