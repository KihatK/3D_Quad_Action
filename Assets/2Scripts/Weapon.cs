using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public enum Type { Melee, Range };
    public Type type;
    public BoxCollider meleeArea;
    public TrailRenderer trailEffect;
    public int damage;
    public float rate;

    public void Use() {
        if (type == Type.Melee) {
            StopCoroutine("Swing");
            StartCoroutine("Swing");
        }
    }

    IEnumerator Swing() {
        trailEffect.enabled = true;
        yield return new WaitForSeconds(0.3f);
        meleeArea.enabled = true;

        yield return new WaitForSeconds(0.1f);
        meleeArea.enabled = false;

        yield return new WaitForSeconds(0.1f);
        trailEffect.enabled = false;
    }
}
