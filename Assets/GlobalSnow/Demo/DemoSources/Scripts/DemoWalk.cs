using UnityEngine;

namespace GlobalSnowEffect {
    public class DemoWalk : MonoBehaviour {
        GlobalSnow snow;

        void Start() {
            snow = GlobalSnow.instance;
        }

        void Update() {

            // Toggles snow on / off globally
            if (InputProxy.GetKeyDown(KeyCode.T)) {
                snow.enabled = !snow.enabled;
            }

            // Add a footprint at center of screen
            if (InputProxy.GetKeyDown(KeyCode.Space)) {
                Camera cam = Camera.main;
                //Debug.Log(snow.GetSnowAmountAt(cam.transform.position + Vector3.up));
                Ray ray = cam.ScreenPointToRay(new Vector3(Screen.width * 0.5f, Screen.height * 0.5f, 0));
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit)) {
                    //GlobalSnow.instance.MarkSnowAt(hit.point, 3f);
                    snow.FootprintAt(hit.point, cam.transform.forward);
                }
            }

            // Show snow over a specific area
            if (InputProxy.GetKeyDown(KeyCode.H)) {
                GlobalSnow snow = GlobalSnow.instance;
                snow.coverageMask = true;
                snow.coverageMaskWorldSize = new Vector3(200, 0, 200);
                snow.coverageMaskWorldCenter = new Vector3(180, 0, 570);
                snow.coverageMaskFillOutside = false;
                snow.MaskClear(0);
                snow.MaskFillArea(new Bounds(new Vector3(200, 0, 600), new Vector3(50, 0, 50)), 255, 1f, 0.2f);
            }



        }
    }
}