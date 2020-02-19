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
                    StartCoroutine(Death());
                    break;
            }
        }
    }

    private const float FLEE_DIST = 5;
    private const float SAFE_DIST = 7; //should be greater than FLEE_DIST

    private void Start()
    {
        State = PersonState.IDLE;
    }

    /// <summary>
    /// Idle while player is far away or dog is close
    /// </summary>
    /// <returns></returns>
    private IEnumerator Idle()
    {
        movement.Input = Vector2.zero;
        animator.SetFloat("Speed", 0);
        Vector2 diffPlayer;
        Vector2 diffDog = Vector2.zero;
        do
        {
            yield return new WaitForEndOfFrame();
            
            diffPlayer = transform.position - Characters.player.transform.position;
            
            if(Characters.dog != null)
                diffDog = transform.position - Characters.dog.transform.position;

        } while (diffPlayer.sqrMagnitude > FLEE_DIST * FLEE_DIST || 
                 (Characters.dog != null && diffDog.sqrMagnitude < Dog.CLOSE_DIST * Dog.CLOSE_DIST));

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
        } while (diffPlayer.sqrMagnitude < SAFE_DIST * SAFE_DIST);

        State = PersonState.IDLE;
    }

    protected override void OnAttacked()
    {
        Characters.people.Remove(this);
        State = PersonState.DEATH;
    }
}
