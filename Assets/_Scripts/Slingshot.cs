using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slingshot : MonoBehaviour
{   
    [Header("Inscribed")]
    public GameObject[] projectiles;
    public int melonChance;
    public float velocityMult = 10f;
    public GameObject projLinePrefab;
    public MissionDemolition game;
    public LineRenderer rubberBand;
    public float ballSize;
    public Vector3 rubberBandRestPos = new Vector3(0,-0.5f, 0);

    [Header("Dynamic")]
    public GameObject launchPoint;
    public Vector3 launchPos;
    public GameObject projectile;
    public bool aimingMode;
    void Awake() {
        Transform launchPointTrans = transform.Find("LaunchPoint");
        launchPoint = launchPointTrans.gameObject;
        launchPoint.SetActive(false);
        launchPos = launchPointTrans.position;
    }
    void OnMouseEnter() {
        if (game.mode == GameMode.playing) {
        launchPoint.SetActive(true);
        }
    }
    void OnMouseExit() {
        launchPoint.SetActive(false);
    }
    void OnMouseDown() {
        if (game.mode == GameMode.playing) {
            aimingMode = true;
            projectile = SpawnProjectile();
            projectile.transform.position = launchPos;
            projectile.GetComponent<Rigidbody>().isKinematic = true;
        }
    }

    GameObject SpawnProjectile() {
        int r = Random.Range(0, 100);
        if (r > melonChance) {
            ballSize = 0.6f;
            return Instantiate(projectiles[0]);
        } else {
            ballSize = 1.2f;
            return Instantiate(projectiles[1]);
        }
    }

    void Update() {
        if (game.mode != GameMode.playing) {
            projectile = null;
            aimingMode = false;
            launchPoint.SetActive(false);
        }
        if (!aimingMode) {
            rubberBand.SetPosition(1, rubberBandRestPos);
            rubberBand.SetPosition(2, rubberBandRestPos);
            rubberBand.SetPosition(3, rubberBandRestPos);
            return;
        }

        Vector3 mousePos2D = Input.mousePosition;
        mousePos2D.z = -Camera.main.transform.position.z;
        Vector3 mousePos3D = Camera.main.ScreenToWorldPoint(mousePos2D);
        
        Vector3 mouseDelta = mousePos3D - launchPos;
        float maxMagnitude = GetComponent<SphereCollider>().radius;
        if (mouseDelta.magnitude > maxMagnitude) {
            mouseDelta.Normalize();
            mouseDelta *= maxMagnitude;
        }
        Vector3 projPos = launchPos + mouseDelta;
        projectile.transform.position = projPos;

        //create reference point in center of ball
        Vector3 rubberBandPos = projPos - launchPoint.transform.position; //subtraction is required to projpos(worldpoint) to the position relative to the parent of theslingshot
        Vector3 rubberBandAdjPos = rubberBandPos + rubberBandPos.normalized * ballSize/2f;
        //put rubberband in front of ball on Z
        rubberBandAdjPos.z = ballSize * 2;
        rubberBand.SetPosition(1, rubberBandAdjPos);

        //put rubberband behind ball on Z
        rubberBandAdjPos.z = -ballSize * 2;
        rubberBand.SetPosition(3, rubberBandAdjPos);
        
        //put rubberband behind/infront of ball on X/Y
        rubberBandPos.z = 0;
        rubberBandPos += rubberBandPos.normalized * ballSize; // offset the position from the center of the ball
        rubberBand.SetPosition(2, rubberBandPos);

        if (Input.GetMouseButtonUp(0)) {
            aimingMode = false;
            Rigidbody projRB = projectile.GetComponent<Rigidbody>();
            projRB.isKinematic = false;
            projRB.collisionDetectionMode = CollisionDetectionMode.Continuous;
            projRB.velocity = -mouseDelta * velocityMult;

            FollowCam.SWITCH_VIEW(FollowCam.eView.slingshot);

            FollowCam.POI = projectile;
            Instantiate(projLinePrefab, projectile.transform);
            projectile = null;
            MissionDemolition.SHOT_FIRED();
        }
    }
}
