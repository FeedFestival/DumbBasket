using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandsCollider : MonoBehaviour
{
    public Player Player;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Ball")
        {
            Player.GrabBallInHand(other);
        }
    }

}
