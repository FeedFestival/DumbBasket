using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SideWall : MonoBehaviour
{
    public Wall Wall;
    
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Ball")
        {
            var dir = new Vector3(1, 0.5f, 0);
            if (Wall == Wall.Right)
                dir = new Vector3(-1, 0.5f, 0);
            else if (Wall == Wall.Plasa)
                dir = Vector3.zero;

            collision.gameObject.GetComponent<Ball>().Reflect(dir, Vector3.zero);

            //Debug.Log("Wall " + Wall.ToString() + ": dir = " + dir.ToString());
        }
    }
}

public enum Wall { Left, Right, Plasa }