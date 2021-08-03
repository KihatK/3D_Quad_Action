using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public int damage;

    private void OnCollisionEnter(Collision collision) {
        if (collision.gameObject.tag == "Floor") {
            //탄피인 경우
            Destroy(gameObject, 3);
        }
        else if (collision.gameObject.tag == "Wall") {
            //총알인 경우
            Destroy(gameObject);
        }
    }
}
