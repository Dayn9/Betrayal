using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum DogState
{
    IDLE, CHASE, RETURN
}

public class Dog : Character
{
    private DogState state;
    public DogState State {
        get { return state; }
        private set
        {
            state = value;
            StopAllCoroutines();

            //Update the animations to match state
            switch (state)
            {
                default:
                case DogState.IDLE:
                    StartCoroutine(Idle());
                    break;
                case DogState.CHASE:
                    Sheep close = ClosestSheep();
                    if(close != null)
                    {
                        StartCoroutine(Chase(close)); //start chasing the closest sheep
                        break;
                    }
                    State = DogState.RETURN;  
                    break;
                case DogState.RETURN:
                    StartCoroutine(Return());
                    break;
            }
        }
    }

    //TODO don't chase the same sheep twice in a row

    private const float CLOSE_DIST = 1.5f;
    private const float WAIT_TIME = 2.0f;

    private void Start()
    {
        State = DogState.IDLE;
    }

    //TODO: param for Idle: time
    //after set time, dog chases
    //if out of range, dog returns

    private IEnumerator Idle()
    {
        animator.SetFloat("Speed", 0);
        movement.Input = Vector2.zero;

        Vector2 diffPlayer;
        do
        {
            diffPlayer = Characters.player.transform.position - transform.position;
            yield return new WaitForEndOfFrame();

        } while (diffPlayer.sqrMagnitude < CLOSE_DIST * CLOSE_DIST);

        yield return new WaitForSeconds(WAIT_TIME);

        State = DogState.CHASE;        
    }

    private IEnumerator Chase(Sheep sheep)
    {
        Vector2 diffSheep;
        do
        {
            //Pursue the player
            movement.Input = Vector2.zero;
            movement.Seek(sheep.movement);

            animator.SetFloat("Speed", movement.Speed);

            diffSheep = sheep.transform.position - transform.position;
            yield return new WaitForEndOfFrame();

        } while (diffSheep.sqrMagnitude > CLOSE_DIST * CLOSE_DIST);

        State = DogState.RETURN;

    }

    private Sheep ClosestSheep()
    {
        if(Characters.sheep.Count > 0)
        {
            float closestDist = float.MaxValue;
            Sheep closest = null;

            foreach(Sheep sheep in Characters.sheep)
            {
                Vector2 diffSheep = sheep.transform.position - transform.position;
                if (diffSheep.sqrMagnitude < closestDist)
                    closest = sheep;
            }

            return closest;
        }
        return null;
    }

    private IEnumerator Return()
    {
        Vector2 diffPlayer;
        do
        {
            //Pursue the player
            movement.Input = Vector2.zero;
            movement.Pursue(Characters.player.movement);

            animator.SetFloat("Speed", movement.Speed);

            diffPlayer = Characters.player.transform.position - transform.position;
            yield return new WaitForEndOfFrame();

        } while (diffPlayer.sqrMagnitude > CLOSE_DIST * CLOSE_DIST);

        State = DogState.IDLE;        
    }
}
