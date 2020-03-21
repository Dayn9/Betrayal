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

    /// <summary>
    /// Moving behavior 
    /// Converts Keyboard input into player movement
    /// </summary>
    /// <returns></returns>
    private IEnumerator Moving()
    {
        while(State == PlayerState.MOVE)
        {
            Vector2 input = new Vector2(
                x : Input.GetAxis("Horizontal"), 
                y: Input.GetAxis("Vertical"));

            movement.Input = input;

            animator.SetFloat("Speed", input.sqrMagnitude);

            yield return new WaitForEndOfFrame();
        }
    }
  
    private void OnCollisionEnter2D(Collision2D collision)
    {

        Character character = collision.gameObject.GetComponent<Character>();
        //check if not already swiping and (Target or Crow)
        if (state != PlayerState.SWIPE && character != null &&
            (collision.gameObject.CompareTag("Target") || character is Crow))
        {
            State = PlayerState.SWIPE;

            //Run the Attack behavior on the target character
            character.Attacked?.Invoke();

        }
    }
}
