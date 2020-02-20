using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ChickenState
{
    IDLE, FLEE, DEATH
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

    private const float FLEE_DIST = 3;
    private const float SAFE_DIST = 5; //should be greater than FLEE_DIST

    private void Start()
    {
        State = ChickenState.IDLE;
    }

    private IEnumerator Idle()
    {
        Vector2 diffPlayer;
        Vector2 diffDog;
        do
        {
            yield return new WaitForEndOfFrame();

            diffPlayer = transform.position - Characters.player.transform.position;
            diffDog = transform.position - Characters.dog.transform.position;

        } while (diffPlayer.sqrMagnitude > FLEE_DIST * FLEE_DIST &&
                 diffDog.sqrMagnitude > FLEE_DIST * FLEE_DIST);

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

        State = ChickenState.IDLE;
    }

    protected override void OnAttacked()
    {
        Characters.chicken.Remove(this);
        State = ChickenState.DEATH;
    }
}
