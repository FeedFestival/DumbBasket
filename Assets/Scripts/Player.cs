using UnityEngine;
using System.Collections;
using System.Linq;
using System;

[RequireComponent(typeof(Controller2D))]
public class Player : MonoBehaviour
{
    public Animator Animator;
    public GameObject HeadCollider;
    public GameObject HandsCollider;
    public GameObject ReseterPlatform;

    public float jumpHeight = 4;
    public float timeToJumpApex = .4f;
    float accelerationTimeAirborne = .2f;

    float accelerationTimeGrounded = .1f;
    public float moveSpeed = 6;

    float gravity;
    float jumpVelocity;
    Vector3 velocity;
    float velocityXSmoothing;

    Controller2D controller;

    public Ball Ball;

    private float StrikeSeconds;

    public MoveDirection MoveDirection;
    public Altitude Altitude;

    void Start()
    {
        controller = GetComponent<Controller2D>();
        controller.Init();

        gravity = -(2 * jumpHeight) / Mathf.Pow(timeToJumpApex, 2);
        jumpVelocity = Mathf.Abs(gravity) * timeToJumpApex;

        // get the animation clip and add the AnimationEvent
        AnimationClip clip = Animator.runtimeAnimatorController.animationClips.Where(a => a.name == "In_Air_Strike").FirstOrDefault();
        StrikeSeconds = clip.length - 0.1f;

        //// new event created
        //AnimationEvent evt = new AnimationEvent();
        //// put some parameters on the AnimationEvent
        //evt.intParameter = 12345;
        //evt.time = StrikeSeconds;      
        //evt.functionName = "InAirHitFinished";

        //clip.AddEvent(evt);
    }

    IEnumerator OnStrikeFinish()
    {
        yield return new WaitForSeconds(StrikeSeconds);
        InAirHitFinished(1);
    }

    public void DirectionSwitch(MoveDirection _moveDirection)
    {
        if (MoveDirection == _moveDirection) return;
        MoveDirection = _moveDirection;

        //Debug.Log(MoveDirection);
        //Debug.Log(Animator.GetBool("IsWalking"));

        if (MoveDirection == MoveDirection.None)
        {
            Animator.SetBool("IsWalking", false);
        }
        else
        {
            var y = MoveDirection == MoveDirection.Left ? 270f : 90f;
            Animator.transform.eulerAngles = new Vector3(transform.eulerAngles.x, y, transform.eulerAngles.z);
            Animator.SetBool("IsWalking", true);
        }
    }

    private bool _preparesToJump;

    public void SetAltitude(Altitude altitude)
    {
        if (Altitude == altitude) return;
        Altitude = altitude;

        var action = _preparesToJump ? "Jump" : "InAir";
        if (Altitude == Altitude.InAir)
        {
            Animator.SetBool(action, true);
            if (_preparesToJump)
                StartCoroutine(WaitForBigJump());
        }
        else if (Altitude == Altitude.Grounded)
        {
            Animator.SetBool("InAir", false);
        }
        //else

    }

    IEnumerator WaitForBigJump()
    {
        yield return new WaitForSeconds(0.3f);
        _preparesToJump = false;

        Animator.SetBool("Jump", false);
        Animator.SetBool("InAir", true);

        velocity.y = jumpVelocity * 1.2f;
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    public void InAirHitFinished(int test)
    {
        Animator.SetBool("Hit", false);
    }

    public void DescendOnContact(Collision ball)
    {
        Vector3 explosionPos = ball.contacts.FirstOrDefault().point;

        if (Ball == null)
            Ball = ball.gameObject.GetComponent<Ball>();

        // we should try and nudge the ball towards the bascket.
        //Ball.Reflect(new Vector3(0, 5, 0), explosionPos);
        Ball.Reflect(new Vector3(0, 15, 0), Vector3.zero);

        if (Altitude == Altitude.Grounded || Altitude == Altitude.Descending) return;

        velocity.y = -1;
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    internal void GrabBallInHand(Collider other)
    {
        if (Altitude != Altitude.Grounded) return;

        if (Ball == null)
            Ball = other.gameObject.GetComponent<Ball>();

        HeadCollider.GetComponent<MeshCollider>().enabled = false;
        HandsCollider.GetComponent<BoxCollider>().enabled = false;

        Ball.Reflect(Vector3.zero, Vector3.zero);
        StartCoroutine(ThrowBallAgain());
    }

    IEnumerator ThrowBallAgain()
    {
        yield return new WaitForSeconds(0.7f);

        //Ball.Reflect(new Vector3(5f, 50f, 0), Vector3.zero);
        var direction = ReseterPlatform.transform.position - Ball.transform.position;
        Debug.Log(direction);
        Ball.GetComponent<Rigidbody>().AddForce(direction.normalized * 800f);

        yield return new WaitForSeconds(0.3f);

        HeadCollider.GetComponent<MeshCollider>().enabled = true;
        HandsCollider.GetComponent<BoxCollider>().enabled = true;
    }

    void Update()
    {
        if (controller.collisions.above || controller.collisions.below)
        {
            velocity.y = 0;
        }

        Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (controller.collisions.below)
            {
                _preparesToJump = MoveDirection == MoveDirection.None;
                if (_preparesToJump == false)
                    velocity.y = jumpVelocity;
            }

            if (Altitude == Altitude.InAir || Altitude == Altitude.Descending)
            {
                Animator.SetBool("Hit", true);
                StartCoroutine(OnStrikeFinish());
            }
        }

        float targetVelocityX = input.x * moveSpeed;

        if (targetVelocityX == 0)
            DirectionSwitch(MoveDirection.None);
        else
            DirectionSwitch(targetVelocityX > 0 ? MoveDirection.Right : MoveDirection.Left);

        //Debug.Log(velocity.y);

        if (velocity.y == 0)
        {
            if (_preparesToJump)
                SetAltitude(Altitude.InAir);
            else
                SetAltitude(Altitude.Grounded);
        }
        else if (velocity.y < 0)
        {
            SetAltitude(Altitude.Descending);
        }
        else
        {
            SetAltitude(Altitude.InAir);
        }

        velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing, (controller.collisions.below) ? accelerationTimeGrounded : accelerationTimeAirborne);
        velocity.y += gravity * Time.deltaTime;

        controller.Move(velocity * Time.deltaTime);
    }
}
