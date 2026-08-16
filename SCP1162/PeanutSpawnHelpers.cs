namespace SCP1162;

internal static class PeanutSpawnHelpers {
	public static bool IsApproximate(Quaternion q1, Quaternion q2, float precision = 0.0000004f) {
		return Mathf.Abs(Quaternion.Dot(q1, q2)) >= 1 - precision;
	}

	public static bool IsPlayerInOldPeanutSpawn(Vector3 playerPosition) {
		Door scp173GateInLight = GetScp173GateInLight();
		Vector3? roomCenterLookupResult = FindOldPeanutSpawnCenter(scp173GateInLight);
		Vector3 roomCenter;
		float radiusMultiplier = 1f;
		if (roomCenterLookupResult is null) {
			Logger.Warn("Error determining old peanut's spawn location. Falling back to default (large radius around its gate)...");
			roomCenter = scp173GateInLight.Position;
		}
		else {
			roomCenter = roomCenterLookupResult.Value;
		}

		float maxXValue = roomCenter.x + 5.5f * radiusMultiplier;
		float minXValue = roomCenter.x - 5.5f * radiusMultiplier;
		float minYValue = roomCenter.y - 1f;
		float maxYValue = roomCenter.y + 10f;
		float minZValue = roomCenter.z - 5.5f * radiusMultiplier;
		float maxZValue = roomCenter.z + 5.5f * radiusMultiplier;
		bool positionXIsValid = playerPosition.x > minXValue && playerPosition.x < maxXValue;
		bool positionYIsValid = playerPosition.y > minYValue && playerPosition.y < maxYValue;
		bool positionZIsValid = playerPosition.z > minZValue && playerPosition.z < maxZValue;
		bool positionIsValid = positionXIsValid && positionYIsValid && positionZIsValid;
		return positionIsValid;
	}

	public static Door GetScp173GateInLight() {
		Door scp173Gate = Map.Doors.FirstOrDefault(x => x.DoorName == LabApi.Features.Enums.DoorName.Lcz173Gate)
			?? throw new UnityException("Could not find SCP-173 gate.");
		return scp173Gate;
	}

	public static Vector3? FindOldPeanutSpawnCenter(Door? scp173GateInLight = null) {
		scp173GateInLight ??= GetScp173GateInLight();
		// All these were proven to work on 8/9/2026 - the strategy is to get the old SCP-173 gate in light,
		// then for all 4 possible rotations get the offset of that door to the center of the room, and then
		// use that to determine the radius where SCP-1162 can work. There's also a fallback in case values change.
		Dictionary<Quaternion, Vector3> possibleGateRotationsToOldPeanutSpawnOffset = new() {
				{ new Quaternion(0.00000f, -0.70711f, 0.00000f, -0.70711f), new Vector3(-5.5f, 0f, 0) },
				{ new Quaternion(0.00000f, 0.00000f, 0.00000f, -1.00000f), new Vector3(0f, 0f, -5.5f) },
				{ new Quaternion(0.00000f, 0.70711f, 0.00000f, -0.70711f), new Vector3(5.5f, 0f, 0f)},
				{ new Quaternion(0.00000f, -1.00000f, 0.00000f, 0.00000f), new Vector3(0f, 0f, 5.5f)},
			};
		Vector3 roomCenter;
		KeyValuePair<Quaternion, Vector3> offsetKVP = possibleGateRotationsToOldPeanutSpawnOffset
			.FirstOrDefault(x => IsApproximate(scp173GateInLight.Rotation, x.Key));
		if (offsetKVP.Equals(default(KeyValuePair<Quaternion, Vector3>))) {
			return null;
		}
		else {
			roomCenter = scp173GateInLight.Position + offsetKVP.Value;
			return roomCenter;
		}
	}
}
