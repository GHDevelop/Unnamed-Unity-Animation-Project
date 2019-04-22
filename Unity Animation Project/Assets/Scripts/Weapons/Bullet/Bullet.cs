using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [Header("Manually Set")]

    [Tooltip("Time between the bullet's spawn and despawn"),
        SerializeField] private float duration = 5;

    [Tooltip("Speed at which the bullet travels in seconds"),
        SerializeField] private float speed = 5;


    [Header("Viewing Only")]

    [Tooltip("Should currently go unused. Variable included in case damage increasing powerups are added. \nModify the bullets damage if you want to change the damage output"),
        SerializeField]
    public float damageModifierFromSource = 1;

    private Transform myTransform;
    private Rigidbody myRigidbody;
    private ParticleSystem defaultHitParticle;

    /// <summary>
    /// Grabs the attached rigidbody and begins self destruct timer.
    /// </summary>
    private void Start()
    {
        myTransform = GetComponent<Transform>();
        myRigidbody = GetComponent<Rigidbody>();
        defaultHitParticle = GetComponentInChildren<ParticleSystem>();

        StartCoroutine("SelfDestructTimer");
    }

    /// <summary>
    /// Moves forward. I could change this to FixedUpdate() while removing "* Time.deltaTime" to achieve a similar effect, 
    /// but this would make "speed"'s measurements inconsistent with the rest of the game (in frames rather than in seconds)
    /// </summary>
    void Update()
    {
        if (GameManager.Me.Paused)
        {
            return;
        }

        myRigidbody.AddRelativeForce(Vector3.forward * speed * Time.deltaTime, ForceMode.VelocityChange);
    }

    /// <summary>
    /// Self Destruct immediately when colliding with another object.
    /// </summary>
    /// <param name="collision"></param>
    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Particle Please");
        ParticleSystem overrideEffect = collision.gameObject.GetComponentInChildren<ParticleSystem>();
        /*ParticleSystem hitEffect = overrideEffect != null ? overrideEffect : defaultHitParticle;
        hitEffect.Emit(1);*/
        ParticleSystem hitEffect = Instantiate(overrideEffect ? overrideEffect : defaultHitParticle, 
            collision.GetContact(0).point, 
            Quaternion.Inverse(myTransform.rotation)) as ParticleSystem;
        hitEffect.Emit(1);
        Destroy(hitEffect.gameObject, hitEffect.main.duration);

        Destroy(this.gameObject);
    }

    /// <summary>
    /// Self Destruct timer.
    /// </summary>
    /// <returns></returns>
    private IEnumerator SelfDestructTimer()
    {
        for (float timer = duration; timer > 0; timer -= Time.deltaTime)
        {
            yield return null;
        }

        Destroy(this.gameObject);
    }
}
