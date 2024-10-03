using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Watermelon : MonoBehaviour
{
    [Header("Inscribed")]
    public float blastRadius = 10f;
    public float explosionPower = 200f;
    public float verticalPower = 10f;
    public bool firstCollision = true;

    [Header("Dynamic")]
    public Projectile projectile;
    void Start() {
        projectile = GetComponent<Projectile>();
    }

    void OnCollisionEnter(Collision c) {
        if (firstCollision == true) {
            firstCollision = false;
            Invoke("Explode", 2f);
        }
    }

    void Explode() {
        Collider[] hitColliders;
        hitColliders = Physics.OverlapSphere(transform.position, blastRadius);
        foreach (Collider hitc in hitColliders) {
            Rigidbody hitcRB = hitc.GetComponent<Rigidbody>();
            if (hitcRB != null) {
                print("hit " + hitc.name);
                hitcRB.AddExplosionForce(explosionPower, transform.position, blastRadius, verticalPower);
            }
        }
        Destroy(GetComponent<Renderer>());
        Destroy(GetComponent<SphereCollider>());
        GetComponent<Rigidbody>().isKinematic = true;
        Invoke("ResetCamera", 1f);
    }

    void ResetCamera() {
        FollowCam.SWITCH_VIEW(FollowCam.eView.slingshot);
        //Destroy(gameObject);
    }
}
