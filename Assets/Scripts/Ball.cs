using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.Utils;

public class Ball : MonoBehaviour
{
    public float MinDesiredSpeed;
    private Rigidbody rb;

    private float _minVelocity = 8.0f;
    private float _maxVelocity = 12.0f;

    private Vector3 _direction = new Vector3(1f, 1, 0);

    // Use this for initialization
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void ShootTo(Vector3 dir)
    {
        rb.AddForce(dir * MinDesiredSpeed);
    }

    void Start()
    {
        ShootTo(_direction);
    }

    public void Reflect(Vector3 dir, Vector3 explosionPos)
    {
        if (explosionPos != Vector3.zero)
        {
            float radius = 5.0F;
            float power = 100.0F;
            rb.AddExplosionForce(power, explosionPos, radius, 3.0F);
        }
        else
        {
            if (dir == Vector3.zero)
            {
                rb.AddForce(Vector3.zero);
                rb.velocity = new Vector3(0.5f, 0.5f, 0.5f);
                return;
            }

            var velocity = rb.velocity.magnitude;

            if (velocity < _minVelocity)
                velocity = _minVelocity;
            else if (velocity > _maxVelocity)
                velocity = _maxVelocity;

            var force = dir * (velocity * 2);
            Debug.Log("force: " + force);
            rb.AddForce(force);
        }
    }

    private void Update()
    {
        if (rb.velocity.magnitude > _maxVelocity)
        {
            var currentMagnitude = rb.velocity.magnitude;

            var percent = UsefullUtils.GetValuePercent(_maxVelocity, currentMagnitude);
            var x = UsefullUtils.GetPercent(rb.velocity.x, percent);
            var y = UsefullUtils.GetPercent(rb.velocity.y, percent);
            var z = UsefullUtils.GetPercent(rb.velocity.z, percent);

            var newVelocity = new Vector3(x, y, x);

            rb.velocity = newVelocity;
        }
        //else if (rb.velocity.magnitude < _minVelocity)
        //{
        //    var currentMagnitude = rb.velocity.magnitude;

        //    var percent = UsefullUtils.GetValuePercent(currentMagnitude, _minVelocity);
        //    var x = rb.velocity.x + UsefullUtils.GetPercent(rb.velocity.x, percent);
        //    var y = rb.velocity.y + UsefullUtils.GetPercent(rb.velocity.y, percent);
        //    var z = rb.velocity.z + UsefullUtils.GetPercent(rb.velocity.z, percent);

        //    var newVelocity = new Vector3(x, y, x);

        //    rb.velocity = newVelocity;
        //}
    }
}
