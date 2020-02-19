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
    public DogState State
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
                case DogState.IDLE:
                    //stand next to the closest person if close
                    Person closesetPerson = Closest<Person>();
                    if (closesetPerson != null &&
                        (closesetPerson.transform.position - transform.position).sqrMagnitude < CLOSE_DIST * CLOSE_DIST)
                    {
                        StartCoroutine(Idle(closesetPerson));
                        break;
                    }
                    //stand next the player
                    StartCoroutine(Idle(Characters.player));

                    break;
                case DogState.CHASE:
                    //chase the closest sheep
                    Sheep closestSheep = Closest<Sheep>();
                    if (closestSheep != null)
                    {
                        //there is a sheep: chase it
                        StartCoroutine(Chase(closestSheep));
                        break;
                    }
                    //chase the closest person
                    closesetPerson = Closest<Person>();
                    if (closesetPerson != null)
                    {
                        StartCoroutine(Chase(closesetPerson));
                        break;
                    }
                    //there are no sheep or people: return to player
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

    public const float CLOSE_DIST = 1.5f;
    private const float FAR_DIST = 8.0f;
    private const float WAIT_TIME = 0.5f;

    private void Start()
    {
        State = DogState.CHASE;
    }

    /// <summary>
    /// Wag tail by the player or person until they leave
    /// </summary>
    /// <returns></returns>
    private IEnumerator Idle(Character character)
    {
        Vector2 diffClose;
        do
        {
            diffClose = character.transform.position - transform.position;
            yield return new WaitForEndOfFrame();

        } while (character != null && diffClose.sqrMagnitude < CLOSE_DIST * CLOSE_DIST);

        State = DogState.PAUSE;
    }

    /// <summary>
    /// Chase a sheep until close
    /// </summary>
    /// <param name="character">character to chase</param>
    /// <returns></returns>
    private IEnumerator Chase(Character character)
    {
        Vector2 diffCharacter;
        do
        {
            //Pursue the character
            movement.Input = Vector2.zero;
            movement.Seek(character.movement);

            animator.SetFloat("Speed", movement.Speed);

            diffCharacter = character.transform.position - transform.position;
            yield return new WaitForEndOfFrame();

        } while (character != null && diffCharacter.sqrMagnitude > CLOSE_DIST * CLOSE_DIST);

        //Stop moving
        movement.Input = Vector2.zero;
        animator.SetFloat("Speed", 0);

        //Check if chasing a Person
        if(character != null && character is Person) //Are you is person?
        {
            State = DogState.IDLE;
        }
        else
        {
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