using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ChickenState
{
    IDLE, FLOCK, FLEE, DEATH
}

public class Chicken : Character
{
    private ChickenState state;
    public ChickenState State
    {
        get { return state; }
        private set
        {
            state = value;

            StopAllCoroutines();
            switch (state)
            {
                default:
                case ChickenState.IDLE:
                    StartCoroutine(Idle());
                    break;
                case ChickenState.FLOCK:
                    StartCoroutine(Flock());
                    StartCoroutine(StayInFrame());
                    break;
                case ChickenState.FLEE:
                    StartCoroutine(Flee());
                    StartCoroutine(StayInFrame());
                    break;
                case ChickenState.DEATH:
                    StartCoroutine(Death());
                    break;
            }
        }
    }

    private const float CLOSE_DIST = 1.2f;
    private const float FLEE_DIST = 3;
    private const float SAFE_DIST = 5; //should be greater than FLEE_DIST

    private void Start()
    {
        State = ChickenState.FLOCK;
    }

    private IEnumerator Idle()
    {
        Vector2 diffPlayer;
        Vector2 diffDog;
        do
        {
            yield return new WaitForEndOfFrame();

            Chicken closestChicken = Closest<Chicken>();
            Vector2 diffChicken;
            if (closestChicken)
            {
                diffChicken = transform.position - closestChicken.transform.position;
                if(diffChicken.sqrMagnitude > CLOSE_DIST * CLOSE_DIST)
                {
                    State = ChickenState.FLOCK;
                }
            }

            diffPlayer = transform.position - Characters.player.transform.position;
            diffDog = transform.position - Characters.dog.transform.position;


        } while (diffPlayer.sqrMagnitude > FLEE_DIST * FLEE_DIST &&
                 diffDog.sqrMagnitude > FLEE_DIST * FLEE_DIST);

        State = ChickenState.FLEE;
    }

    private IEnumerator Flock()
    {
        Vector2 diffPlayer;
        Vector2 diffDog;
        do
        {
            yield return new WaitForEndOfFrame();

            movement.Input = Vector2.zero;
            animator.SetFloat("Speed", movement.Speed);

            Vector2 diffChicken;
            foreach(Chicken chicken in Characters.chicken)
            {
                if (chicken.Equals(this)) continue; //skip self 

                diffChicken = transform.position - chicken.transform.position;
                if(diffChicken.sqrMagnitude < CLOSE_DIST * CLOSE_DIST)
                {
                    movement.Input = Vector2.zero;
                    animator.SetFloat("Speed", 0);

                    State = ChickenState.IDLE;
                    break;
                }
                if (diffChicken.sqrMagnitude < FLEE_DIST * FLEE_DIST)
                {
                    continue;
                }
                movement.Seek(chicken.movement);
            }

            diffPlayer = transform.position - Characters.player.transform.position;
            diffDog = transform.position - Characters.dog.transform.position;

        } while (diffPlayer.sqrMagnitude > FLEE_DIST * FLEE_DIST &&
                 diffDog.sqrMagnitude > FLEE_DIST * FLEE_DIST);

        movement.Input = Vector2.zero;
        animator.SetFloat("Speed", 0);

        State = ChickenState.FLEE;
    }

    private IEnumerator Flee()
    {
        Vector2 diffPlayer;
        Vector2 diffDog;
        do
        {
            yield return new WaitForEndOfFrame();

            movement.Input = Vector2.zero;
            movement.Flee(Characters.player.movement);
            movement.Flee(Characters.player.movement);
            animator.SetFloat("Speed", movement.Speed);

            diffPlayer = transform.position - Characters.player.transform.position;
            diffDog = transform.position - Characters.dog.transform.position;

        } while (diffPlayer.sqrMagnitude < SAFE_DIST * SAFE_DIST ||
                 diffDog.sqrMagnitude < SAFE_DIST * SAFE_DIST);

        movement.Input = Vector2.zero;
        animator.SetFloat("Speed", 0);

        State = ChickenState.FLOCK;
    }

    protected override void OnAttacked()
    {
        Characters.chicken.Remove(this);
        State = ChickenState.DEATH;
    }
}
