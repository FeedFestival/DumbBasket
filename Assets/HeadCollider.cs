using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HeadCollider : MonoBehaviour
{
    public Player Player;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Ball")
        {
            Player.DescendOnContact(collision);
        }
    }
}
