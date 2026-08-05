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
		private Vector3 basePosition;

		private float targetZoom;

		private float shakeIntensity;
		private float shakeDuration;
		private float shakeTimer;
		private Vector3 shakeOffset;

		private void Awake()
		{
			if (targetCamera == null)
			{
				targetCamera = GetComponent<UnityEngine.Camera>();
			}
			instance = this;

			targetZoom = defaultZoom;
			basePosition = transform.position;

			DontDestroyOnLoad(gameObject);
		}

		private void LateUpdate()
		{
			UpdatePosition();
			UpdateZoom();
			UpdateShake();
			transform.position = basePosition + shakeOffset;
		}
		public void SnapToTarget()
		{
			if (followTarget != null)
			{
				basePosition = followTarget.transform.position + followOffset;
				basePosition.z = -10f;
				transform.position = basePosition;
			}
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

			basePosition = Vector3.Lerp(
				basePosition,
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
			if (targetCamera == null)
			{
				return;
			}

			targetCamera.orthographicSize = Mathf.Lerp(
				targetCamera.orthographicSize,
				targetZoom,
				Time.deltaTime * 5f
			);
		}

		private void UpdateShake()
		{
			shakeOffset = Vector3.zero;
			if (shakeTimer <= 0f)
			{
				return;
			}

			shakeTimer -= Time.unscaledDeltaTime;
			float progress = shakeDuration > 0f ? Mathf.Clamp01(shakeTimer / shakeDuration) : 0f;
			shakeOffset = Random.insideUnitCircle * (shakeIntensity * progress);
			if (shakeTimer <= 0f)
			{
				shakeIntensity = 0f;
				shakeDuration = 0f;
				shakeOffset = Vector3.zero;
			}
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
