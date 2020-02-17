using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PersonState
{
    IDLE, FLEE, DEATH
}

public class Person : Character
{
    private PersonState state;
    public PersonState State
    {
        get { return state; }
        set
        {
            state = value;
            StopAllCoroutines();

            switch (state)
            {
                case PersonState.IDLE:
                    StartCoroutine(Idle());
                    break;
                case PersonState.FLEE:
                    StartCoroutine(Flee());
                    StartCoroutine(StayInFrame());
                    break;
                case PersonState.DEATH:
                    Destroy(gameObject);
                    break;
            }
        }
    }

    private const float FLEE_DIST = 5;

    private void Start()
    {
        State = PersonState.IDLE;
    }

    private IEnumerator Idle()
    {
        movement.Input = Vector2.zero;
        animator.SetFloat("Speed", 0);
        Vector2 diffPlayer;
        do
        {
            yield return new WaitForEndOfFrame();
            diffPlayer = transform.position - Characters.player.transform.position;

        } while (diffPlayer.sqrMagnitude > FLEE_DIST * FLEE_DIST);

        State = PersonState.FLEE;
    }

    private IEnumerator Flee()
    {
        Vector2 diffPlayer;
        do
        {
            yield return new WaitForEndOfFrame();
            movement.Input = Vector2.zero;
            movement.Flee(Characters.player.movement);
            animator.SetFloat("Speed", movement.Speed);

            diffPlayer = transform.position - Characters.player.transform.position;
        } while (diffPlayer.sqrMagnitude < FLEE_DIST * FLEE_DIST);

        State = PersonState.IDLE;
    }

    protected override void OnAttacked()
    {
        Characters.people.Remove(this);
        State = PersonState.DEATH;
    }
}
