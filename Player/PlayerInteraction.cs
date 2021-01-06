using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    public KeyCode swingWeapon;
    public KeyCode Interact;
    public LayerMask MonstersLayer;
    public LayerMask InteractLayer;

    [Range(0,100)]
    public float PlayerHealth = 100;
    [Space]
    public AudioClip AttackingSound;
    public AudioClip InteractingSound;
    public AudioClip HurtSound;
    public AudioSource ASource;

    private void Update()
    {
        if (Input.GetKeyDown(swingWeapon))
        {
            //swing weapon animation etc
            Debug.Log("Attacking");

            Ray ray = new Ray(transform.GetChild(0).position, transform.GetChild(0).forward);
            RaycastHit hit;

            if(Physics.Raycast(ray, out hit, 7.5f, MonstersLayer))
            {
                Debug.Log("Hitting");
                hit.transform.GetComponent<Monster>().Hit();
                //play sound
                ASource.clip = AttackingSound;
                ASource.PlayOneShot(ASource.clip);
            }

        }

        if (Input.GetKeyDown(Interact))
        {
            //raycast and raycasting to objects and NPC
            Debug.Log("Interacting");

            Ray ray = new Ray(transform.GetChild(0).position, transform.GetChild(0).forward);
            RaycastHit hit;


            if (Physics.Raycast(ray, out hit, 20f, InteractLayer))
            {
                Debug.Log("Hitting");
                hit.transform.GetComponent<Interactable>()?.HitItem(gameObject);
                //play sound
                ASource.clip = InteractingSound;
                ASource.PlayOneShot(ASource.clip);
            }
        }
    }

    public void TakeDamage(float amount)
    {
        PlayerHealth = Mathf.Clamp(PlayerHealth - amount, 0, 100);



        if (PlayerHealth == 0)
            GameManager.instance.PlayerDead();
        else
        {
            //play hurting sound
            ASource.clip = HurtSound;
            ASource.PlayOneShot(ASource.clip);
        }
    }

    public void Heal(float amount)
    {
        PlayerHealth = Mathf.Clamp(PlayerHealth + amount, 0, 100);
    }

}
