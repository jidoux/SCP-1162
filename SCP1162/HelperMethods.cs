namespace SCP1162;

internal sealed class HelperMethods {
	public static bool IsApproximate(Quaternion q1, Quaternion q2, float precision = 0.0000004f) {
		return Mathf.Abs(Quaternion.Dot(q1, q2)) >= 1 - precision;
	}
}
