using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.AI;

public class TriImp : Imp
{
    // public override void TakeDamage(int damage)
    // {
        
    //     health -= damage;

    //     if (health <= 0) {
    //         audioSource.PlayOneShot(DeathScream); //play Death sound
    //         disableAttack = true;
    //         attackSprite.sprite = null;
    //         thisSprite.sprite = null;
    //         attackSprite.enabled = false;
    //         thisSprite.enabled = false;
    //         animator.Play("ImpDeath");
    //         Invoke(nameof(DestroyEnemy), audioSource.clip.length - 0.065f);
    //     }
    //     else
    //         audioSource.Play();  //play Pain sound
    // }

    protected override void AttackPlayer()
    {
        //Make sure enemy doesn't move
        //agent.SetDestination(transform.position);


        if (!alreadyAttacked && !disableAttack)
        {
            if(!CastRay()) //prevents shooting at the player through the walls and other obstacles
                return;

            //SET SPRITE TO IDLE SPRITE
            spriteRenderer.sprite = attackSprite;

            ///Attack code here
            GameObject Fireball = Instantiate(projectile, new Vector3(transform.position.x, 1.55f, transform.position.z - 0.3f),  Quaternion.identity);
            
            // Fireball[] array = Fireball.GetComponentsInChildren<Fireball>();
            // for(int i = 0; i < array.Length; i++)
            //     array[i].ownerImp = gameObject;
            
            //Quaternion.Euler(new Vector3(0, 0, 180)));

            //changeSprite
            ///End of attack code

            alreadyAttacked = true;
            Invoke(nameof(ResetAttack), timeBetweenAttacks); //poziva nakon timeBetweenAttacks(float) sekundi
            Invoke(nameof(ResetSprite), 0.5f);
        }
    }

}