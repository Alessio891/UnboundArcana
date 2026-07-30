using System.Collections;
using System.Collections.Generic;
using UnboundArcana.Core.Camera;
using UnboundArcana.Core.Entities;
using UnboundArcana.Core.Rooms;
using UnboundArcana.Core.Runtime;
using UnboundArcana.Player;
using UnityEngine;

namespace UnboundArcana.Core.Expedition
{
	public class ExpeditionPlayerCoordinator
	{
		private SpriteRenderer spriteRenderer;
		private PlayerInput playerInput;

		public Entity Player { get; private set; }

		public bool Spawn(RoomInstance room)
		{
			if (!TryGetPlayerStart(room, out Vector3 position))
			{
				return false;
			}

			Player = GameRuntimeManager.Instance.PlayerSpawner.Spawn(GameSession.Instance.Player, position, null);

			if (Player == null)
			{
				return false;
			}

			spriteRenderer = Player.GetComponentInChildren<SpriteRenderer>();
			playerInput = Player.GetComponent<PlayerInput>();

			MoveToRoom(room);
			MainCameraManager.Instance.SetFollowTarget(Player.transform);
			return true;
		}

		public bool MoveToRoom(RoomInstance room)
		{
			if (Player == null || !TryGetPlayerStart(room, out Vector3 position))
			{
				return false;
			}

			Player.transform.position = position;
			MainCameraManager.Instance.SnapToTarget();
			return true;
		}

		public void SetInputEnabled(bool enabled)
		{
			playerInput?.SetInputEnabled(enabled);
		}

		public void FollowPlayer()
		{
			if (Player != null)
			{
				MainCameraManager.Instance.SetFollowTarget(Player.transform);
			}
		}

		public void SetRevealProgress(float progress)
		{
			spriteRenderer?.material.SetFloat("_Progress", progress);
		}

		public IEnumerator Reveal(float speed)
		{
			float progress = 0f;

			while (progress < 1f)
			{
				SetRevealProgress(progress);
				progress += Time.deltaTime * speed;
				yield return null;
			}

			SetRevealProgress(1f);
		}

		private bool TryGetPlayerStart(RoomInstance room, out Vector3 position)
		{
			position = default;

			if (room == null)
			{
				return false;
			}

			List<RoomMarker> markers = new(room.GetMarkers(RoomMarkerType.PlayerStart));

			if (markers.Count == 0)
			{
				Debug.LogWarning("No PlayerStart marker found.");
				return false;
			}

			position = markers[0].transform.position;
			return true;
		}
	}
}
