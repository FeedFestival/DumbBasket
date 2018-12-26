using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AvatarController : MonoBehaviour
{
    private Rigidbody rb;
    private float JumpSpeed = 0.3f;
    private float Speed = 0.1f;

    private float GroundY = -4.4f;
    private bool IsMoving;

    LTDescr MoveLean;
    LTDescr JumpLean;

    private void Awake()
    {

    }

    private MoveDirection MoveDirection;
    

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            MoveDirection = MoveDirection.Left;
            SetMove();
        }

        if (Input.GetKeyDown(KeyCode.D))
        {
            MoveDirection = MoveDirection.Right;
            SetMove();
        }

        if (Input.GetKeyUp(KeyCode.A) || Input.GetKeyUp(KeyCode.D) || Input.GetKeyUp(KeyCode.W))
        {
            MoveDirection = MoveDirection.None;
        }
    }

    public void SetJump()
    {
        StartCoroutine(Jump());
    }

    public void SetMove()
    {
        if (IsMoving) return;

        StartCoroutine(Move());
    }

    IEnumerator Jump()
    {
        IsMoving = true;
        var newPos = GroundY + 2f;
        JumpLean = LeanTween.move(gameObject, new Vector3(gameObject.transform.position.x, newPos, 0f), Speed).setEase(LeanTweenType.linear);

        yield return new WaitForSeconds(JumpSpeed);

        JumpLean = LeanTween.move(gameObject, new Vector3(gameObject.transform.position.x, GroundY, 0f), Speed).setEase(LeanTweenType.linear);

        IsMoving = false;
    }

    IEnumerator Move()
    {
        IsMoving = true;
        var newPos = gameObject.transform.position.x - 1f;
        if (MoveDirection == MoveDirection.Right)
            newPos = gameObject.transform.position.x + 1f;
        MoveLean = LeanTween.move(gameObject, new Vector3(newPos, GroundY, 0f), Speed).setEase(LeanTweenType.linear);

        yield return new WaitForSeconds(Speed);

        if (MoveDirection != MoveDirection.None)
        {
            StartCoroutine(Move());
        }
        else
        {
            IsMoving = false;
        }

    }
}

public enum MoveDirection
{
    None, Left, Right
}

public enum Altitude
{
    Grounded, InAir, Descending
}