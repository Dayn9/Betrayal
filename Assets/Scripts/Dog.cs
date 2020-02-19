using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum DogState
{
    IDLE, CHASE, RETURN, PAUSE, SCARE, SAD
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
                    Sheep closestSheep = ClosestSheep();
                    if(closestSheep != null)
                    {
                        //there is a sheep: chase it
                        StartCoroutine(Chase(closestSheep)); 
                        break;
                    }
                    //there are no sheep: return to player
                    State = DogState.RETURN;  
                    break;
                case DogState.RETURN:
                    StartCoroutine(Return());
                    break;
                case DogState.PAUSE:
                    StartCoroutine(Pause());
                    break;
                case DogState.SAD:
                    StartCoroutine(Sad());
                    break;
                case DogState.SCARE:
                    animator.SetTrigger("Scare");
                    break;
            }
        }
    }

    //TODO don't chase the same sheep twice in a row

    private const float CLOSE_DIST = 1.5f;
    private const float FAR_DIST = 8.0f;
    private const float WAIT_TIME = 0.5f;

    private void Start()
    {
        State = DogState.CHASE;
    }

    //TODO: param  or Idle: time
    //after set time, dog chases
    //if out of range, dog returns

    /// <summary>
    /// Wag tail by the player until they leave
    /// </summary>
    /// <returns></returns>
    private IEnumerator Idle()
    {
        Vector2 diffPlayer;
        do
        {
            diffPlayer = Characters.player.transform.position - transform.position;
            yield return new WaitForEndOfFrame();

        } while (diffPlayer.sqrMagnitude < CLOSE_DIST * CLOSE_DIST);

        State = DogState.PAUSE;
    }

    /// <summary>
    /// Chase a sheep until close
    /// </summary>
    /// <param name="sheep">Sheep to chase</param>
    /// <returns></returns>
    private IEnumerator Chase(Sheep sheep)
    {
        Vector2 diffSheep;
        do
        {
            //Pursue the Sheep
            movement.Input = Vector2.zero;
            movement.Seek(sheep.movement);

            animator.SetFloat("Speed", movement.Speed);

            diffSheep = sheep.transform.position - transform.position;
            yield return new WaitForEndOfFrame();

        } while (diffSheep.sqrMagnitude > CLOSE_DIST * CLOSE_DIST);

        //Stop moving
        movement.Input = Vector2.zero;
        animator.SetFloat("Speed", 0);

        //Check player distance
        Vector2 diffPlayer = Characters.player.transform.position - transform.position;
        if (diffPlayer.sqrMagnitude > FAR_DIST * FAR_DIST)
        {
            //player is far away
            State = DogState.SAD;
        }
        else
        {
            //player is close
            State = DogState.RETURN;
        }
    }

    /// <summary>
    /// Find the closest Sheep
    /// </summary>
    /// <returns>The Closest Sheep</returns>
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

    /// <summary>
    /// Chase the player until close
    /// </summary>
    /// <returns></returns>
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

        movement.Input = Vector2.zero;
        animator.SetFloat("Speed", 0);

        State = DogState.IDLE;        
    }

    /// <summary>
    /// Wait for a few seconds and then chase
    /// </summary>
    /// <returns></returns>
    private IEnumerator Pause()
    {
        animator.SetBool("Pause", true);

        yield return new WaitForSeconds(WAIT_TIME);

        animator.SetBool("Pause", false);

        State = DogState.CHASE;
    }

    /// <summary>
    /// Wait for the palyer to get close
    /// </summary>
    /// <returns></returns>
    private IEnumerator Sad(){
        animator.SetBool("Sad", true);

        Vector2 diffPlayer;
        do
        {
            diffPlayer = Characters.player.transform.position - transform.position;
            yield return new WaitForEndOfFrame();

        } while (diffPlayer.sqrMagnitude > FAR_DIST * FAR_DIST);

        animator.SetBool("Sad", false);

        State = DogState.RETURN;
    }
}
