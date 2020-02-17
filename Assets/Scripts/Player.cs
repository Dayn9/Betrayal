using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerState
{
    MOVE, SWIPE /*, PET*/
}

public class Player : Character
{
    private PlayerState state;
    public PlayerState State
    {
        get { return state; }
        set {
            state = value;

            StopAllCoroutines();
            switch (state)
            {
                case PlayerState.MOVE:
                    StartCoroutine(Moving());
                    break;
                case PlayerState.SWIPE:
                    movement.Input = Vector2.zero;
                    animator.SetTrigger("Swipe"); //start the swipe animation
                    break;
            }

        }
    }

    private void Start()
    {
        State = PlayerState.MOVE;
    }

    private IEnumerator Moving()
    {
        while(State == PlayerState.MOVE)
        {
            Vector2 input = new Vector2(Input.GetAxis("Horizontal"),
                                    Input.GetAxis("Vertical"));

            movement.Input = input;

            animator.SetFloat("Speed", input.sqrMagnitude);

            yield return new WaitForEndOfFrame();
        }
    }
  

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Target"))
        {
            State = PlayerState.SWIPE;

            Character character = collision.gameObject.GetComponent<Character>();
            if (character != null)
            {
                character.Attacked?.Invoke();
            }
        }
    }
}
