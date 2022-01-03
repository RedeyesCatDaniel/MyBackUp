using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 视频通话相关的工具
/// </summary>
namespace LGUVirtualOffice
{
	public class VC_Tool
	{
		static float DistanceUnit = 1.25f;
		/// <summary>
		/// 改变目标RawImage的Alpha值
		/// </summary>
		/// <param name="image"></param>
		/// <param name="alpha"></param>
		public static void SetAlpha(RawImage image, float alpha)
		{
			Color color = image.color;
			color.a = alpha;
			image.color = color;
		}

		public static float GetDistanceSqrRate(Transform targetTrans, Transform localTrans)
		{
			float unit = DistanceUnit;
			float distanceSqr = (targetTrans.position - localTrans.position).sqrMagnitude;
			float rateSqr = distanceSqr / unit;
			return rateSqr;
		}

		public static float GetAlpha(float curRateSqr, float maxBordSqr, float minBoadSqr)
		{
			float alpha = 1 - (curRateSqr - minBoadSqr) / (maxBordSqr - minBoadSqr);
			if (alpha > 1) alpha = 1;
			if (alpha < 0) alpha = 0;
			return alpha;
		}
	}
}
