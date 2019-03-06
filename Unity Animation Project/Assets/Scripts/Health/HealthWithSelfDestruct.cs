using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class HealthWithSelfDestruct : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] private float _health;
    public float Health
    {
        get { return _health; }
        set
        {
            if (value < _health)
            {
                onHurt.Invoke();
                OnDamaged(value);
            }
            else if (value > _health)
            {
                onHeal.Invoke();
            }

            _health = Mathf.Clamp(value, 0, MaxHealth);
            if (AssociatedCanvas != null)
            {
                AssociatedCanvas.HPPercentText = ((_health / MaxHealth) * 100).ToString();
            }
        }
    }

    [SerializeField] private float _maxHealth;
    public float MaxHealth
    {
        get { return _maxHealth; }
        set { _maxHealth = value; }
    }

    //Set by controller
    [SerializeField] private CanvasManager _associatedCanvas;
    public CanvasManager AssociatedCanvas
    {
        get { return _associatedCanvas; }
        set { _associatedCanvas = value; }
    }

    [Header("Invincibility")]
    [SerializeField] private float _invincibilityAfterBeingHit;
    public float InvincibilityAfterBeingHit
    {
        get { return _invincibilityAfterBeingHit; }
        set { _invincibilityAfterBeingHit = value; }
    }

    [SerializeField] private bool _isInInvincibilityFrames;
    public bool IsInInvincibilityFrames
    {
        get { return _isInInvincibilityFrames; }
        set { _isInInvincibilityFrames = value; }
    }

    [Header("Events")]
    [SerializeField, Tooltip("Raised every time Health is set and the new value is lower than the previous value")]
    private UnityEvent onHurt;
    [SerializeField, Tooltip("Raised every time Health is set and the new value is higher than the previous value")]
    private UnityEvent onHeal;

    private void Awake()
    {
        IsInInvincibilityFrames = false;
    }

    private void Start()
    {
        //Done in Start to ensure UI is present on first setting of "Health"
        Health = MaxHealth;
    }

    private void OnCollisionEnter(Collision collision)
    {
        TakeDamage(collision);
    }

    private void OnCollisionStay(Collision collision)
    {
        TakeDamage(collision);
    }

    private void TakeDamage(Collision collision)
    {
        CollisionsDealDamage collisionDealsDamage = collision.gameObject.GetComponent<CollisionsDealDamage>();
        if (collisionDealsDamage && collisionDealsDamage.enabled && IsInInvincibilityFrames == false)
        {
            Health -= collisionDealsDamage.damage;
        }
    }

    private void OnDamaged(float newHealth)
    {
        if (newHealth <= 0)
        {
            Destroy(gameObject);
        }
        else
        {
            StartCoroutine(InvincibilityTimer(InvincibilityAfterBeingHit));
        }
    }

    private IEnumerator InvincibilityTimer(float baseInvincibilityTime)
    {
        IsInInvincibilityFrames = true;

        for (float timer = baseInvincibilityTime; timer > 0; timer -= Time.deltaTime)
        {
            yield return null;
            //TODO: Do some sort of visual effect here
        }

        IsInInvincibilityFrames = false;
    }
}
