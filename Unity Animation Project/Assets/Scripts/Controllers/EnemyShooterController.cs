using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyShooterController : ShooterController
{
    [Header("Enemy Shooter Controller")]

    [Header("Edit friendly")]

    [Tooltip("The target of this AI. Generally set dynamically, but I could imagine a situation where it might be set in the editor"),
        SerializeField] private Transform _target;
    public Transform Target
    {
        get { return _target; }
        set { _target = value; }
    }

    [Tooltip("The value to set the navMeshAgent's avoidance priority to in order to make other agents ignore it"),
        SerializeField] private int _valueToBeIgnored = 99;
    public int ValueToBeIgnored
    {
        get
        {
            return _valueToBeIgnored;
        }

        set
        {
            _valueToBeIgnored = value;
        }
    }

    [Tooltip("How close to get to an off-mesh link before using it"),
        SerializeField] private float _minimumDistanceFromOffMeshLink = 0.05f;
    public float MinimumDistanceFromOffMeshLink
    {
        get
        {
            return _minimumDistanceFromOffMeshLink;
        }

        set
        {
            _minimumDistanceFromOffMeshLink = value;
        }
    }

    [Tooltip("How far above the ground should be grounded"),
        SerializeField] private float _distanceInAirToInstantGround = 2f;
    public float DistanceInAirToInstantGround
    {
        get
        {
            return _distanceInAirToInstantGround;
        }

        set
        {
            _distanceInAirToInstantGround = value;
        }
    }

    [Header("Viewing Only")]

    [Tooltip("Navigation. Is set automatically."),
        SerializeField] protected NavMeshAgent navigationAgent;
    public Vector3? Destination
    {
        get
        {
            if (navigationAgent)
            {
                return navigationAgent.destination;
            }
            else
            {
                return null;
            }
        }
        set
        {
            if (navigationAgent && value != null)
            {
                navigationAgent.SetDestination((Vector3)value);
            }
            else
            {
                Debug.Log("Either no NavMeshAgent component (somehow) or null destination input.");
            }
        }
    }

    protected Vector3 velocity;

    [Tooltip("Return for off mesh link coroutine"),
        SerializeField] protected Coroutine onOffMeshLink;

    // Start is called before the first frame update
    protected override void Awake()
    {
        base.Awake();

        navigationAgent = GetComponent<NavMeshAgent>();

        if (AssociatedCanvas)
        {
            AssociatedCanvas = AssociatedCanvas; //sets off setter method.
        }
    }

    protected override void MakeMouseCursorObject()
    {
        //does nothing
    }

    //start is used instead of awake to guarantee the game manager loads in first
    protected override void Start()
    {
        GameManager.Me.Enemies.Add(this);
    }

    // Update is called once per frame
    void Update()
    {
        if (navigationAgent && navigationAgent.enabled)
        {
            rigidbody.velocity = Vector3.zero;
        }


        if (GameManager.Me.Paused)
        {
            return;
        }

        Move();
        if (Target
            && weaponManager.EquippedWeapon
            && onOffMeshLink == null
            && Vector3.Angle(Target.position - myTransform.position, myTransform.forward) < weaponManager.EquippedWeapon.FiringRadius
            && Vector3.Distance(Target.position, myTransform.position) < weaponManager.EquippedWeapon.FiringRange)
        {
            weaponManager.Fire();
        }
    }

    private void Move()
    {
        if (onOffMeshLink != null)
        {
            return;
        }

        if (navigationAgent.isOnOffMeshLink)
        {
            onOffMeshLink = StartCoroutine(JumpOffMeshLink());
        }

        if (Target)
        {
            navigationAgent.isStopped = false;
            Destination = Target.position;
            MoveHorizontalVertical();
            RotateTowardsObject(Target);
        }
        else
        {
            navigationAgent.isStopped = true;
            characterAnimator.SetFloat(horizontal, 0);
            characterAnimator.SetFloat(vertical, 0);

            if (GameManager.Me.Player != null)
            {
                Target = GameManager.Me.Player.GetComponent<Transform>();
            }
        }
    }

    protected override void MoveHorizontalVertical()
    {
        velocity = Vector3.MoveTowards(velocity, navigationAgent.desiredVelocity, navigationAgent.acceleration * Time.deltaTime);
        Vector3 direction = myTransform.InverseTransformDirection(velocity);
        characterAnimator.SetFloat(horizontal, direction.x);
        characterAnimator.SetFloat(vertical, direction.z);
    }

    private void OnAnimatorMove()
    {
        if (navigationAgent.updatePosition)
        {
            navigationAgent.velocity = characterAnimator.velocity;
        }
        else
        {
            characterAnimator.ApplyBuiltinRootMotion();
        }
    }

    protected override void OnDestroy()
    {
        if (GameManager.Me.Enemies.Contains(this))
        {
            GameManager.Me.Enemies.Remove(this);
            GameManager.Me.EnemiesKilled++;
        }
    }

    private IEnumerator JumpOffMeshLink()
    {
        int oldAvoidancePriority = DisableNavMeshAgent();
        navigationAgent.ActivateCurrentOffMeshLink(false); //disables the current off mesh link, preventing other agents from using it
        OffMeshLinkData linkData = navigationAgent.currentOffMeshLinkData; //gets information pertaining to the off mesh link.

        characterAnimator.SetBool("OML", true);
        //rigidbody.isKinematic = true;
        Vector3 offset;
        do
        {
            offset = linkData.endPos - myTransform.position; 
            Ground();

            characterAnimator.SetFloat("OMLDistanceXZ", Vector3.ProjectOnPlane(offset, Vector3.up).magnitude);
            characterAnimator.SetFloat("OMLDistanceY", Mathf.Abs(offset.y));

            myTransform.rotation = Quaternion.LookRotation(Vector3.ProjectOnPlane(offset, Vector3.up));
            myTransform.position = Vector3.MoveTowards(myTransform.position, linkData.endPos, navigationAgent.speed * Time.deltaTime);

            //Vector3 directions = myTransform.InverseTransformDirection(offset.normalized * navigationAgent.speed);
            //characterAnimator.SetFloat(horizontal, directions.x);
            //characterAnimator.SetFloat(vertical, directions.z);

            yield return null;
        } while (offset.sqrMagnitude > MinimumDistanceFromOffMeshLink);

        EnableNavMeshAgent(oldAvoidancePriority, linkData.endPos);
        navigationAgent.ActivateCurrentOffMeshLink(true);
        onOffMeshLink = null;
        characterAnimator.SetBool("OML", false);
        //rigidbody.isKinematic = false;
    }

    private int DisableNavMeshAgent()
    {
        navigationAgent.isStopped = true; //disables base movement, allowing focus on moving on off mesh link
        navigationAgent.updatePosition = false; //prevents the navmeshagent from moving the object during the off mesh link jump.
        int oldAvoidancePriority = navigationAgent.avoidancePriority; //stores the avoidance priority used by the agent, used to reset navmeshagent
        navigationAgent.avoidancePriority = ValueToBeIgnored; //sets agent to be avoided by other agents

        return oldAvoidancePriority;
    }

    private void EnableNavMeshAgent(int avoidancePriority, Vector3 endPosition)
    {
        myTransform.position = endPosition;
        navigationAgent.isStopped = false;
        navigationAgent.updatePosition = true;
        navigationAgent.avoidancePriority = avoidancePriority;
        navigationAgent.CompleteOffMeshLink();
    }

    private void Ground()
    {
        RaycastHit raycastHitInfo;
        if (Physics.Raycast(myTransform.position + Vector3.up, Vector3.down, out raycastHitInfo, DistanceInAirToInstantGround, 1, QueryTriggerInteraction.Ignore))
        {
            myTransform.position = raycastHitInfo.point;
        }
    }
}
