using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class ActivatableDoor : ActivatableObject
{
    [SerializeField] Collider2D doorCollider;
    [SerializeField] SpriteRenderer sprite;
    [SerializeField] ParticleSystem activationParticles;

    private void Start()
    {
        if (doorCollider == null)
        {
            doorCollider = GetComponent<Collider2D>();
            Assert.IsNotNull(doorCollider);
        }
        if(sprite == null)
        {
            sprite = GetComponent<SpriteRenderer>();
            Assert.IsNotNull(sprite);
        }
    }

    public override void Activate()
    {//open the door
        doorCollider.enabled = false;
        Debug.Log("Door Activated!");
        sprite.enabled = false;
        if(activationParticles != null)
        {
            Instantiate(activationParticles, transform.position, Quaternion.identity).transform.parent = null;
        }
        Destroy(this);
    }
}
