using UnityEngine;
using System.Collections;

namespace UnboundArcana.Core.Camera
{
	public enum CameraMode
	{
		Follow,
		FixedPosition
	}

	public class MainCameraManager : MonoBehaviour
	{
		[SerializeField]
		private UnityEngine.Camera targetCamera;

		[SerializeField]
		private Transform followTarget;

		[SerializeField]
		private Vector3 followOffset = new Vector3(0, 0, -10);

		[SerializeField]
		private float followSmooth = 8f;

		[SerializeField]
		private float moveSmooth = 5f;

		[SerializeField]
		private float defaultZoom = 5f;

		private static MainCameraManager instance;
		public static MainCameraManager Instance => instance;

		private CameraMode mode = CameraMode.Follow;

		private Vector3 targetPosition;

		private float targetZoom;

		private float shakeIntensity;
		private float shakeDuration;
		private float shakeTimer;

		private void Awake()
		{
			if (targetCamera == null)
			{
				targetCamera = GetComponent<UnityEngine.Camera>();
			}
			instance = this;

			targetZoom = defaultZoom;

			DontDestroyOnLoad(gameObject);
		}

		private void LateUpdate()
		{
			UpdatePosition();
			UpdateZoom();
			UpdateShake();
		}
		public void SnapToTarget()
		{
			if (followTarget != null)
				transform.position = followTarget.transform.position;
		}
		private void UpdatePosition()
		{
			Vector3 desiredPosition = transform.position;
			desiredPosition.z = -10;

			if (mode == CameraMode.Follow && followTarget != null)
			{
				desiredPosition =
					followTarget.position +
					followOffset;
			}
			else if (mode == CameraMode.FixedPosition)
			{
				desiredPosition = targetPosition;
			}

			transform.position = Vector3.Lerp(
				transform.position,
				desiredPosition,
				Time.deltaTime * (
					mode == CameraMode.Follow
					? followSmooth
					: moveSmooth
				)
			);
		}

		private void UpdateZoom()
		{
			targetCamera.orthographicSize = Mathf.Lerp(
				targetCamera.orthographicSize,
				targetZoom,
				Time.deltaTime * 5f
			);
		}

		private void UpdateShake()
		{
			if (shakeTimer <= 0)
			{
				return;
			}

			shakeTimer -= Time.deltaTime;

			Vector3 offset =
				Random.insideUnitCircle *
				shakeIntensity;

			transform.position += offset;
		}

		public void SetFollowTarget(Transform target)
		{
			followTarget = target;
			mode = CameraMode.Follow;
		}

		public void ClearFollowTarget()
		{
			followTarget = null;
		}

		public void MoveTo(Vector3 position)
		{
			targetPosition = position;
			targetPosition.z = -10;
			mode = CameraMode.FixedPosition;
		}

		public void ReturnToFollow()
		{
			if (followTarget == null)
			{
				return;
			}

			mode = CameraMode.Follow;
		}

		public void SetZoom(float zoom)
		{
			targetZoom = zoom;
		}

		public void ResetZoom()
		{
			targetZoom = defaultZoom;
		}

		public void Shake(
			float intensity,
			float duration)
		{
			shakeIntensity = intensity;
			shakeDuration = duration;
			shakeTimer = duration;
		}

		public bool IsFollowing()
		{
			return mode == CameraMode.Follow;
		}
	}
}