using UnityEngine;

namespace GlobalSnowEffect {

	public class SimpleCameraMove : MonoBehaviour {
		public float cameraSensitivity = 150;
		public float climbSpeed = 20;
		public float normalMoveSpeed = 20;
		public float slowMoveFactor = 0.25f;
		public float fastMoveFactor = 3;

		private float rotationX = 0.0f;
		private float rotationY = 0.0f;

		public Transform target;
		public float maxCameraDistance = 50f;

		Quaternion startingRotation;
		bool freeCamera;

		void Start () {
			startingRotation = transform.rotation;
		}

		void FixedUpdate () {
			if (freeCamera) {
				rotationX += InputProxy.GetAxis("Mouse X") * cameraSensitivity * Time.deltaTime;
				rotationY += InputProxy.GetAxis("Mouse Y") * cameraSensitivity * Time.deltaTime;
				rotationY = Mathf.Clamp(rotationY, -90, 90);

				transform.localRotation = Quaternion.AngleAxis(rotationX, Vector3.up) * startingRotation;
				transform.localRotation *= Quaternion.AngleAxis(rotationY, Vector3.left);
			}
			else {
				if (target != null) {
					Camera.main.transform.LookAt(target.transform.position);
					if (Vector3.Distance(target.transform.position, Camera.main.transform.position) > maxCameraDistance) {
						Camera.main.transform.position = target.transform.position - Camera.main.transform.forward * maxCameraDistance;
					}
				}
			}

			if (InputProxy.GetKey(KeyCode.LeftShift) || InputProxy.GetKey(KeyCode.RightShift)) {
				transform.position += transform.forward * (normalMoveSpeed * fastMoveFactor) * InputProxy.GetAxis("Vertical") * Time.deltaTime;
				transform.position += transform.right * (normalMoveSpeed * fastMoveFactor) * InputProxy.GetAxis("Horizontal") * Time.deltaTime;
			}
			else if (InputProxy.GetKey(KeyCode.LeftControl) || InputProxy.GetKey(KeyCode.RightControl)) {
				transform.position += transform.forward * (normalMoveSpeed * slowMoveFactor) * InputProxy.GetAxis("Vertical") * Time.deltaTime;
				transform.position += transform.right * (normalMoveSpeed * slowMoveFactor) * InputProxy.GetAxis("Horizontal") * Time.deltaTime;
			}
			else {
				transform.position += transform.forward * normalMoveSpeed * InputProxy.GetAxis("Vertical") * Time.deltaTime;
				transform.position += transform.right * normalMoveSpeed * InputProxy.GetAxis("Horizontal") * Time.deltaTime;
			}

			if (InputProxy.GetKey(KeyCode.Q)) {
				transform.position -= transform.up * climbSpeed * Time.deltaTime;
			}
			if (InputProxy.GetKey(KeyCode.E)) {
				transform.position += transform.up * climbSpeed * Time.deltaTime;
			}

			if (transform.position.y < 1f) {
				transform.position = new Vector3(transform.position.x, 1f, transform.position.z);
			}

		}

		void Update () {
			if (InputProxy.GetKeyDown(KeyCode.Escape)) {
				freeCamera = !freeCamera;
				startingRotation = transform.rotation;
			}
		}


	}
}