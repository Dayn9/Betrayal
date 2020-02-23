using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CrowState
{
    FLOCK, CHASE, FLEE
}

public class Crow : Character
{
    private CrowState state;
    public CrowState State
    {
        get { return state; }
        set
        {
            state = value;

            StopAllCoroutines();
            switch (state)
            {
                case CrowState.CHASE:
                    StartCoroutine(Chase());
                    StartCoroutine(StayInFrame());
                    break;
                case CrowState.FLOCK:
                    StartCoroutine(Flock());
                    StartCoroutine(StayInFrame());
                    break;
                case CrowState.FLEE:
                    StartCoroutine(Flee(FLEE_TIME));
                    break;
                    
            }
        }
    }

    private const float CHASE_DIST = 5;
    private const float CLOSE_DIST = 3;
    private const float FLEE_TIME = 5;

    private void Start()
    {
        State = CrowState.FLOCK;
    }

    private IEnumerator Flock()
    {
        Vector2 diffDog;
        do
        {
            yield return new WaitForEndOfFrame();

            movement.Input = Vector2.zero;

            Vector2 diffCrow;
            foreach(Crow crow in Characters.crows)
            {
                if (crow.Equals(this)) continue; //skip self

                diffCrow = transform.position - crow.transform.position;
                if(diffCrow.sqrMagnitude > CLOSE_DIST * CLOSE_DIST)
                {
                    movement.Seek(crow.movement);
                }
                else
                {
                    movement.Flee(crow.movement);
                }
            }

            diffDog = transform.position - Characters.dog.transform.position;

        } while (diffDog.sqrMagnitude > CHASE_DIST * CHASE_DIST);

        State = CrowState.CHASE;
    }

    private IEnumerator Flee(float time) {

        movement.Input = Vector2.zero;
        movement.Flee(Characters.player.movement);

        yield return new WaitForSeconds(time);

        State = CrowState.FLOCK;

    }

    private IEnumerator Chase()
    {
        Vector2 diffDog;
        do
        {
            yield return new WaitForEndOfFrame();

            movement.Input = Vector2.zero;

            movement.Seek(Characters.dog.movement);

            diffDog = transform.position - Characters.dog.transform.position;

        } while (diffDog.sqrMagnitude < CHASE_DIST * CHASE_DIST);

        State = CrowState.FLOCK;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Character character = collision.gameObject.GetComponent<Character>();
        //check if chasing and is dog
        if (State == CrowState.CHASE && character != null && character is Dog)
        {
            //Run the Attack behavior on the target character
            character.Attacked?.Invoke();
        }
    }

    protected override void OnAttacked()
    {
        //knock back
        transform.position += (transform.position - Characters.player.transform.position).normalized * 0.5f;

        State = CrowState.FLEE;
    }
}
