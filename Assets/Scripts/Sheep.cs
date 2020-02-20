using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SheepState
{
    IDLE, WANDER, FLEE, DEATH
}

public class Sheep : Character
{
    private static readonly float[] WANDER_TIME_RANGE = { 1, 2 };
    private static readonly float[] IDLE_TIME_RANGE = { 7, 10 };
    private const float FLEE_DIST = 3;

    public float RandomWanderTime { get { return (Random.value * (IDLE_TIME_RANGE[1] - IDLE_TIME_RANGE[0])) + IDLE_TIME_RANGE[0]; } }

    private SheepState state;
    public SheepState State
    {
        get { return state; }
        private set
        {
            state = value;

            StopAllCoroutines();
            //Update the animations to match state
            switch (state)
            {
                default:
                case SheepState.IDLE:
                    StartCoroutine(Idle(RandomWanderTime));
                    break;
                case SheepState.WANDER:
                    StartCoroutine(Wander(Random.insideUnitCircle, RandomWanderTime));
                    break;
                case SheepState.FLEE:
                    StartCoroutine(Flee());
                    break;
                case SheepState.DEATH:
                    StartCoroutine(Death());
                    break;
            }
        }
    }

    private void Start()
    {
        State = SheepState.WANDER;
    }

    private IEnumerator Idle(float time)
    {
        float t = 0;
        while(t < time)
        {
            //flee from the dog
            if(Characters.dog)
            {
                Vector2 diffDog = transform.position - Characters.dog.transform.position;
                if(diffDog.sqrMagnitude < FLEE_DIST * FLEE_DIST)
                {
                    State = SheepState.FLEE;
                }
            }
            yield return new WaitForEndOfFrame();
            t += Time.deltaTime;
        }
        
        State = SheepState.WANDER;
    }

    private IEnumerator Flee()
    {
        Vector2 diffDog;
        do
        {
            yield return new WaitForEndOfFrame();

            movement.Flee(Characters.dog.movement);
            animator.SetFloat("Speed", movement.Speed);

            diffDog = transform.position - Characters.dog.transform.position;
        } while (diffDog.sqrMagnitude < FLEE_DIST * FLEE_DIST);

        movement.Input = Vector2.zero;
        animator.SetFloat("Speed", 0);

        State = SheepState.IDLE;
    }

    private IEnumerator Wander(Vector2 wanderDirection, float time)
    {
        movement.Input = wanderDirection;
        animator.SetFloat("Speed", wanderDirection.magnitude);
        float t = 0;
        while (t < time)
        {
            if (Characters.dog != null)
            {
                Vector2 diffDog = transform.position - Characters.dog.transform.position;
                if (diffDog.sqrMagnitude < FLEE_DIST * FLEE_DIST)
                {
                    State = SheepState.FLEE;
                }
            }

            yield return new WaitForEndOfFrame();
            t += Time.deltaTime;
        }

        movement.Input = Vector2.zero;
        animator.SetFloat("Speed", 0);

        State = SheepState.IDLE;
    }

    protected override void OnAttacked() {

        Characters.sheep.Remove(this);
        State = SheepState.DEATH;
    }
}
