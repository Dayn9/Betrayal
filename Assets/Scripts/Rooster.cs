using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rooster : Chicken
{
    protected override IEnumerator Flock()
    {
        Vector2 diffPlayer;
        Vector2 diffDog;
        do
        {
            yield return new WaitForEndOfFrame();

            movement.Input = Vector2.zero;

            //loop over all the other chicken
            Vector2 diffChicken;
            foreach (Chicken chicken in Characters.chicken)
            {
                if (chicken.Equals(this)) continue; //skip self 

                //check if the chicken is close
                diffChicken = transform.position - chicken.transform.position;
                //check if the chicken is within flee distance
                if (diffChicken.sqrMagnitude < FLEE_DIST * FLEE_DIST)
                {
                    movement.Flee(chicken.movement);
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
}
