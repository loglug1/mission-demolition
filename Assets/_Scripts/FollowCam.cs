using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class FollowCam : MonoBehaviour
{
    static private FollowCam S;
    static public GameObject POI;

    public enum eView {none, slingshot, castle, both};

    [Header("Inscribed")]
    public float easing = 0.05f;
    public Vector2 minXY = Vector2.zero;
    public GameObject viewBothGO;

    [Header("Dynamic")]
    public float camZ;
    public eView nextView = eView.slingshot;
    public Camera cam;
    public float targetCamSize;

    void Awake() {
        S = this;
        camZ = transform.position.z;
        cam = GetComponent<Camera>();
    }

    void FixedUpdate() {
        Vector3 destination = Vector3.zero;

        if (POI != null) {
            destination = POI.transform.position;
        }

        destination.x = Mathf.Max(minXY.x, destination.x);
        destination.y = Mathf.Max(minXY.y, destination.y);
        destination = Vector3.Lerp(transform.position, destination, easing);
        destination.z = camZ;
        transform.position = destination;

        cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, targetCamSize, easing);
    }

    public void SwitchView(eView newView) {
        if (newView == eView.none) {
            newView = nextView;
        }
        switch (newView) {
            case eView.slingshot:
                POI = null;
                targetCamSize = 10;
                nextView = eView.castle;
                break;
            case eView.castle:
                POI = MissionDemolition.GET_CASTLE();
                targetCamSize = 10;
                nextView = eView.both;
                break;
            case eView.both:
                POI = viewBothGO;
                targetCamSize = 35;//size go to 35
                nextView = eView.slingshot;
                break;
        }
    }
    public void SwitchView() {
        SwitchView(eView.none);
    }

    static public void SWITCH_VIEW(eView newView) {
        S.SwitchView(newView);
    }
}
