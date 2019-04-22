using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class HealthWithSelfDestruct : MonoBehaviour
{
    [Header("Edit Friendly")]

    [Header("Health")]
    [Tooltip("The maximum amount of health."),
        SerializeField] private float _maxHealth;
    public float MaxHealth
    {
        get { return _maxHealth; }
        set { _maxHealth = value; }
    }

    [Tooltip("The current health. Set automatically at runtime, but is not dangerous to edit. May want to edit for debugging"),
        SerializeField] private float _health;
    public float Health
    {
        get { return _health; }
        set
        {
            //In addition to lowering health, also runs any onHurt/onHeal methods set in the editor, updates the UI, and calls the built in "OnDamaged" method.
            if (value <= _health)
            {
                onHurt.Invoke();
                StartInvincibilityOrDestroy(value);
            }
            else if (value > _health)
            {
                onHeal.Invoke();
            }

            _health = Mathf.Clamp(value, 0, MaxHealth);
            if (AssociatedCanvas != null)
            {
                AssociatedCanvas.UpdateHPGraphics(_health / MaxHealth);
            }
        }
    }

    [Header("Invincibility")]
    [Tooltip("The total amount of invincibility gained after being hit."),
        SerializeField] private float _invincibilityAfterBeingHit;
    public float InvincibilityAfterBeingHit
    {
        get { return _invincibilityAfterBeingHit; }
        set { _invincibilityAfterBeingHit = value; }
    }

    [Tooltip("Whether the object is invincible or not. If set to true without the InvincibilityTimer coroutine, then object will never lose health"),
        SerializeField] private bool _isInInvincibilityFrames = false;
    public bool IsInInvincibilityFrames
    {
        get { return _isInInvincibilityFrames; }
        set { _isInInvincibilityFrames = value; }
    }

    [Tooltip("Time in which the body will persist after death"),
        SerializeField] private float _timeFromDeathToDeleteCorpse = 3.0f;
    public float TimeFromDeathToDeleteCorpse
    {
        get
        {
            return _timeFromDeathToDeleteCorpse;
        }

        set
        {
            _timeFromDeathToDeleteCorpse = value;
        }
    }

    [Header("Events")]
    [SerializeField, Tooltip("Raised every time Health is set and the new value is lower than the previous value")]
    private UnityEvent onHurt;
    [SerializeField, Tooltip("Raised every time Health is set and the new value is higher than the previous value")]
    private UnityEvent onHeal;
    [SerializeField, Tooltip("Raised On Death")]
    private UnityEvent onDeath;

    [Header("Viewing Only")]
    //Set by controller
    [Tooltip("This is set by the controller attached to the same object. \nDO NOT SET THIS IN THE EDITOR."),
        SerializeField] private CanvasManager _associatedCanvas;
    public CanvasManager AssociatedCanvas
    {
        get { return _associatedCanvas; }
        set { _associatedCanvas = value; }
    }

    private ShooterController _associatedController;
    public ShooterController AssociatedController
    {
        get { return _associatedController; }
        set { _associatedController = value; }
    }

    private void Awake()
    {
        AssociatedController = this.GetComponent<ShooterController>();
    }

    /// <summary>
    /// Sets the Health equal to the max health, and updates the associated canvas.
    /// </summary>
    private void Start()
    {
        //Done in Start to ensure UI is present on first setting of "Health"
        Health = MaxHealth;
        //onDeath.AddListener(NotifyGameManagerOfDeath);
    }

    #region "Collider Checks"
    /***************************************************************************************************************************************************************
     *                                                                  Collision Block. All of these call TakeDamage.
     ***************************************************************************************************************************************************************/
    private void OnCollisionEnter(Collision collision)
    {
        TakeDamage(collision.collider);
    }

    private void OnCollisionStay(Collision collision)
    {
        if (GameManager.Me.Paused == false)
        {
            TakeDamage(collision.collider);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        TakeDamage(other);
    }

    private void OnTriggerStay(Collider other)
    {
        if (GameManager.Me.Paused == false)
        {
            TakeDamage(other);
        }
    }
    /***************************************************************************************************************************************************************
     *                                                                              End of Collision Block
     ***************************************************************************************************************************************************************/
    #endregion

    /// <summary>
    /// Runs when hit.
    /// </summary>
    /// <param name="collider"></param>
    private void TakeDamage(Collider collider)
    {
        CollisionsDealDamage collisionDealsDamage = collider.gameObject.GetComponentInParent<CollisionsDealDamage>();
        if (collisionDealsDamage //Collided Object has "CollisionDealsDamage" component.
            && collisionDealsDamage.activeHitbox //If the "Hitbox" is active.
            && collisionDealsDamage.exempt.Contains(this) == false //If this object is exempt from taking damage.
            && IsInInvincibilityFrames == false //If this object is invincible at the moment
            && collisionDealsDamage.hitDisjoint == false) //If the part of the object hit is part of the object's body, used to prevent damaging characters by hitting their weapons
        {
            Debug.Log(collider.gameObject.name + " Hurt " + this.gameObject.name);
            Health -= collisionDealsDamage.damage;
        }
    }

    /// <summary>
    /// Called by the setter for health. If health is below 0 the object is destroyed, otherwise invincibility is enabled.
    /// </summary>
    /// <param name="newHealth"></param>
    public void StartInvincibilityOrDestroy(float newHealth)
    {
        if (newHealth <= 0)
        {
            onDeath.Invoke();
            onDeath.RemoveAllListeners();
            if (this.AssociatedController)
            {
                Destroy(this.AssociatedController);
            }
            Destroy(this.gameObject, TimeFromDeathToDeleteCorpse);
        }
        else
        {
            StartCoroutine(InvincibilityTimer(InvincibilityAfterBeingHit));
        }
    }


    /// <summary>
    /// Coroutine. Sets object to invincible, runs a timer for the invincibility duration, then disables invincibility.
    /// </summary>
    /// <param name="baseInvincibilityTime"></param>
    /// <returns></returns>
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

    public void NotifyGameManagerOfDeath()
    {
        if (GameManager.Me != null)
        {
            GameManager.Me.OnPlayerDeath();
        }
    }
}
