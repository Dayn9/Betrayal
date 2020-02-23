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
        protected set
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

    protected const float CLOSE_DIST = 1.5f;
    protected const float FLEE_DIST = 3;
    protected const float SAFE_DIST = 5; //should be greater than FLEE_DIST
    
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

            animator.SetFloat("Speed", 0);

            Chicken closestChicken = Closest<Chicken>();
            Vector2 diffChicken;
            if (closestChicken)
            {
                diffChicken = transform.position - closestChicken.transform.position;
                if(diffChicken.sqrMagnitude > SAFE_DIST * SAFE_DIST)
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

    /// <summary>
    /// Flock while the dog and player are far away and not near a chicken
    /// </summary>
    /// <returns></returns>
    protected virtual IEnumerator Flock()
    {
        Vector2 diffPlayer;
        Vector2 diffDog;
        do
        {
            yield return new WaitForEndOfFrame();

            movement.Input = Vector2.zero;

            //loop over all the other chicken
            Vector2 diffChicken;
            foreach(Chicken chicken in Characters.chicken)
            {
                if (chicken.Equals(this)) continue; //skip self 

                //check if the chicken is close
                diffChicken = transform.position - chicken.transform.position;
                if(diffChicken.sqrMagnitude < CLOSE_DIST * CLOSE_DIST)
                {
                    //Enter IDLE state 
                    movement.Input = Vector2.zero;
                    animator.SetFloat("Speed", 0);

                    State = ChickenState.IDLE;
                    break;
                }
                //check if the chicken is within flee distance
                if (diffChicken.sqrMagnitude < FLEE_DIST * FLEE_DIST)
                {
                    continue; //skip close-ish
                }
                //chicken is far away
                movement.Seek(chicken.movement);
            }

            animator.SetFloat("Speed", movement.Speed);

            diffPlayer = transform.position - Characters.player.transform.position;
            diffDog = transform.position - Characters.dog.transform.position;

        } while (diffPlayer.sqrMagnitude > FLEE_DIST * FLEE_DIST &&
                 diffDog.sqrMagnitude > FLEE_DIST * FLEE_DIST);

        //Enter the FLEE state
        movement.Input = Vector2.zero;
        animator.SetFloat("Speed", 0);

        State = ChickenState.FLEE;
    }

    /// <summary>
    /// Run away from the player
    /// </summary>
    /// <returns></returns>
    private IEnumerator Flee()
    {
        Vector2 diffPlayer;
        Vector2 diffDog;
        do
        {
            yield return new WaitForEndOfFrame();

            //flee from both the player and dog
            movement.Input = Vector2.zero;

            movement.Flee(Characters.player.movement);
            movement.Flee(Characters.player.movement);

            animator.SetFloat("Speed", movement.Speed);

            diffPlayer = transform.position - Characters.player.transform.position;
            diffDog = transform.position - Characters.dog.transform.position;

        } while (diffPlayer.sqrMagnitude < SAFE_DIST * SAFE_DIST ||
                 diffDog.sqrMagnitude < SAFE_DIST * SAFE_DIST);

        //Enter the FLOCK State
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
