using ActionGameFramework.Projectiles;
using UnityEngine;

namespace ActionGameFramework.Helpers
{
	/// <summary>
	/// Projectile の一般的な弾道計算を補助するヘルパークラス
	/// </summary>
	public static class Ballistics
	{
		/// <summary>
		/// 指定したワールド座標を狙う直線 Projectile の初速を計算する
		/// </summary>
		/// <param name="firePosition">Projectile の開始位置。</param>
		/// <param name="targetPosition">Projectile が狙う目標位置。</param>
		/// <param name="launchSpeed">Projectile の初速。</param>
		/// <returns>この Projectile の初速を表す Vector3。解がない場合は Vector3.zero。</returns>
		public static Vector3 CalculateLinearFireVector(Vector3 firePosition, Vector3 targetPosition,
		                                                float launchSpeed)
		{
			// 初速が 0 の場合は、ベクトルにごく小さな基準の大きさを与える
			if (Mathf.Abs(launchSpeed) < float.Epsilon)
			{
				launchSpeed = 0.001f;
			}

			return (targetPosition - firePosition).normalized * launchSpeed;
		}

		/// <summary>
		/// 指定した開始速度と加速度で、直線 Projectile が目的地に到達するまでの時間を計算する
		/// </summary>
		/// <param name="firePosition">Projectile の開始位置。</param>
		/// <param name="targetPosition">Projectile が狙う目標位置。</param>
		/// <param name="launchSpeed">Projectile の初速。</param>
		/// <param name="acceleration">発射後の Projectile の加速度。</param>
		/// <returns>目標まで飛行し終えるまでの秒数。</returns>
		public static float CalculateLinearFlightTime(Vector3 firePosition, Vector3 targetPosition,
		                                              float launchSpeed, float acceleration)
		{
			float flightDistance = (targetPosition - firePosition).magnitude;

			// v^2 = u^2 + 2as
			float endV = Mathf.Sqrt((launchSpeed * launchSpeed) + (2 * acceleration * flightDistance));

			// t = 2s/(u+v)
			return (2f * flightDistance) / (launchSpeed + endV);
		}

		/// <summary>
		/// 直線 Projectile が移動中の目標に当たるように、先読みした目標位置を計算する
		/// 目標は等速で移動すると仮定する。精度はパラメーターで調整できる
		/// </summary>
		/// <param name="firePosition">Projectile の開始位置。</param>
		/// <param name="targetPosition">狙う目標の現在位置。</param>
		/// <param name="targetVelocity">狙う目標の速度を表す Vector。</param>
		/// <param name="launchSpeed">Projectile の初速。</param>
		/// <param name="acceleration">発射後の Projectile の加速度。</param>
		/// <param name="precision">正しい位置に近づけるための反復回数。速い目標ほど高い精度が有効。</param>
		/// <returns>先読みした目標位置を表す Vector3。</returns>
		public static Vector3 CalculateLinearLeadingTargetPoint(Vector3 firePosition, Vector3 targetPosition,
		                                                        Vector3 targetVelocity, float launchSpeed, float acceleration,
		                                                        int precision = 2)
		{
			// 精度がない場合は先読みしないため、ここで抜ける
			if (precision <= 0)
			{
				return targetPosition;
			}

			Vector3 testPosition = targetPosition;

			for (int i = 0; i < precision; i++)
			{
				float impactTime = CalculateLinearFlightTime(firePosition, testPosition, launchSpeed,
				                                             acceleration);

				testPosition = targetPosition + (targetVelocity * impactTime);
			}

			return testPosition;
		}

		/// <summary>
		/// 指定した角度で発射した放物線軌道 Projectile が目標位置に当たるための発射速度を計算する
		/// </summary>
		/// <param name="firePosition">Projectile を発射する位置。</param>
		/// <param name="targetPosition">狙う目標位置。</param>
		/// <param name="launchAngle">Projectile を発射する角度。</param>
		/// <param name="gravity">重力定数（垂直方向のみ。正の値は下向き）</param>
		/// <returns>目標に当てるための発射速度を表す Vector3。解がない場合は Vector3.zero。</returns>
		public static Vector3 CalculateBallisticFireVectorFromAngle(Vector3 firePosition, Vector3 targetPosition,
		                                                            float launchAngle, float gravity)
		{
			Vector3 target = targetPosition;
			target.y = firePosition.y;
			Vector3 toTarget = target - firePosition;
			float targetDistance = toTarget.magnitude;
			float shootingAngle = launchAngle;
			float relativeY = firePosition.y - targetPosition.y;

			float theta = Mathf.Deg2Rad * shootingAngle;
			float cosTheta = Mathf.Cos(theta);
			float num = targetDistance * Mathf.Sqrt(gravity) * Mathf.Sqrt(1 / cosTheta);
			float denom = Mathf.Sqrt((2 * targetDistance * Mathf.Sin(theta)) + (2 * relativeY * cosTheta));

			if (denom > 0)
			{
				float v = num / denom;

				// 回転できるように照準ベクトルを水平にする
				Vector3 aimVector = toTarget / targetDistance;
				aimVector.y = 0;
				Vector3 rotAxis = Vector3.Cross(aimVector, Vector3.up);
				Quaternion rotation = Quaternion.AngleAxis(shootingAngle, rotAxis);
				aimVector = rotation * aimVector.normalized;

				return aimVector * v;
			}

			return Vector3.zero;
		}

		/// <summary>
		/// 指定した角度で発射した放物線軌道 Projectile が目標位置に当たるための発射速度を計算する
		/// プロジェクトの Physics 設定で定義された垂直方向の重力定数を使用する
		/// </summary>
		/// <param name="firePosition">Projectile を発射する位置。</param>
		/// <param name="targetPosition">狙う目標位置。</param>
		/// <param name="launchAngle">Projectile を発射する角度。</param>
		/// <returns>目標に当てるための発射速度を表す Vector3。解がない場合は Vector3.zero。</returns>
		public static Vector3 CalculateBallisticFireVectorFromAngle(Vector3 firePosition, Vector3 targetPosition,
		                                                            float launchAngle)
		{
			return CalculateBallisticFireVectorFromAngle(firePosition, targetPosition, launchAngle,
			                                             Mathf.Abs(Physics.gravity.y));
		}

		/// <summary>
		/// 指定した速度で発射した放物線軌道 Projectile が目標位置に当たるための発射速度を計算する
		/// </summary>
		/// <param name="firePosition">Projectile を発射する位置。</param>
		/// <param name="targetPosition">狙う目標位置。</param>
		/// <param name="launchSpeed">Projectile の発射速度。</param>
		/// <param name="arcHeight">放物線（「下手投げ」）軌道と直線寄り（「上手投げ」）軌道の優先設定。</param>
		/// <param name="gravity">重力定数（垂直方向のみ。正の値は下向き）</param>
		/// <returns>目標に当てるための発射速度を表す Vector3。解がない場合は Vector3.zero。</returns>
		public static Vector3 CalculateBallisticFireVectorFromVelocity(Vector3 firePosition, Vector3 targetPosition,
		                                                               float launchSpeed, BallisticArcHeight arcHeight,
		                                                               float gravity)
		{
			float theta = CalculateBallisticFireAngle(firePosition, targetPosition, launchSpeed, arcHeight, gravity);

			// 角度が成立しない場合は、ここで抜ける
			if (float.IsNaN(theta))
			{
				return Vector3.zero;
			}

			Vector3 target = targetPosition;
			target.y = firePosition.y;
			Vector3 toTarget = target - firePosition;

			float targetDistance = toTarget.magnitude;

			Vector3 aimVector = Vector3.forward;

			if (targetDistance > 0f)
			{
				// 回転できるように照準ベクトルを水平にする
				aimVector = toTarget / targetDistance;
				aimVector.y = 0;
			}

			Vector3 rotAxis = Vector3.Cross(aimVector, Vector3.up);
			Quaternion rotation = Quaternion.AngleAxis(theta, rotAxis);
			aimVector = rotation * aimVector.normalized;

			return aimVector * launchSpeed;
		}

		/// <summary>
		/// 指定した速度で発射した放物線軌道 Projectile が目標位置に当たるための発射速度を計算する
		/// プロジェクトの Physics 設定で定義された垂直方向の重力定数を使用する
		/// </summary>
		/// <param name="firePosition">Projectile を発射する位置。</param>
		/// <param name="targetPosition">狙う目標位置。</param>
		/// <param name="launchSpeed">Projectile の発射速度。</param>
		/// <param name="arcHeight">放物線（「下手投げ」）軌道と直線寄り（「上手投げ」）軌道の優先設定。</param>
		/// <returns>目標に当てるための発射速度を表す Vector3。解がない場合は Vector3.zero。</returns>
		public static Vector3 CalculateBallisticFireVectorFromVelocity(Vector3 firePosition, Vector3 targetPosition,
		                                                               float launchSpeed, BallisticArcHeight arcHeight)
		{
			return CalculateBallisticFireVectorFromVelocity(firePosition, targetPosition, launchSpeed, arcHeight,
			                                                Mathf.Abs(Physics.gravity.y));
		}

		/// <summary>
		/// 指定した初速の Projectile が目標に当たるために必要な発射角度を計算する
		/// </summary>
		/// <param name="firePosition">Projectile を発射する位置。</param>
		/// <param name="targetPosition">狙う目標位置。</param>
		/// <param name="launchSpeed">Projectile の発射速度。</param>
		/// <param name="arcHeight">放物線（「下手投げ」）軌道と直線寄り（「上手投げ」）軌道の優先設定。</param>
		/// <param name="gravity">重力定数（垂直方向のみ。正の値は下向き）</param>
		/// <returns>必要な発射角度（度）。有効な解がない場合は NaN。</returns>
		public static float CalculateBallisticFireAngle(Vector3 firePosition, Vector3 targetPosition,
		                                                float launchSpeed, BallisticArcHeight arcHeight, float gravity)
		{
			Vector3 target = targetPosition;
			target.y = firePosition.y;
			Vector3 toTarget = target - firePosition;
			float targetDistance = toTarget.magnitude;
			float relativeY = targetPosition.y - firePosition.y;
			float vSquared = launchSpeed * launchSpeed;

			// 目標までの距離が 0 の場合、目標が真上にある（または自分自身を狙っている）とみなす
			if (Mathf.Approximately(targetDistance, 0f))
			{
				// 高い角度のショットを優先する場合は、真上に発射する
				if (arcHeight == BallisticArcHeight.UseHigh || arcHeight == BallisticArcHeight.PreferHigh)
				{
					return 90f;
				}

				// 低い角度の直線的なショットの場合は、目標との相対的な高さに基づいて角度を調整する
				if (relativeY > 0)
				{
					return 90f;
				}

				if (relativeY < 0)
				{
					return -90f;
				}
			}

			float b = Mathf.Sqrt((vSquared * vSquared) -
			                     (gravity * ((gravity * (targetDistance * targetDistance)) + (2 * relativeY * vSquared))));

			// 「下手投げ」の放物線軌道角度
			float theta1 = Mathf.Atan((vSquared + b) / (gravity * targetDistance));

			// 「上手投げ」の直線寄り軌道角度
			float theta2 = Mathf.Atan((vSquared - b) / (gravity * targetDistance));

			bool theta1Nan = float.IsNaN(theta1);
			bool theta2Nan = float.IsNaN(theta2);

			// 両方とも無効な場合は、解がないことを示すため NaN で抜ける
			if (theta1Nan && theta2Nan)
			{
				return float.NaN;
			}

			// 初期値は放物線軌道にする
			float returnTheta = theta1;

			// 直線寄りの軌道を返したい場合
			if (arcHeight == BallisticArcHeight.UseLow)
			{
				returnTheta = theta2;
			}

			// theta1 が有効なら返し、無効なら theta2 にする場合
			if (arcHeight == BallisticArcHeight.PreferHigh)
			{
				returnTheta = theta1Nan ? theta2 : theta1;
			}

			// theta2 が有効なら返し、無効なら theta1 にする場合
			if (arcHeight == BallisticArcHeight.PreferLow)
			{
				returnTheta = theta2Nan ? theta1 : theta2;
			}

			return returnTheta * Mathf.Rad2Deg;
		}

		/// <summary>
		/// 指定した初速の Projectile が目標に当たるために必要な発射角度を計算する
		/// プロジェクトの Physics 設定で定義された垂直方向の重力定数を使用する
		/// </summary>
		/// <param name="firePosition">Projectile を発射する位置。</param>
		/// <param name="targetPosition">狙う目標位置。</param>
		/// <param name="launchSpeed">Projectile の発射速度。</param>
		/// <param name="arcHeight">放物線（「下手投げ」）軌道と直線寄り（「上手投げ」）軌道の優先設定。</param>
		/// <returns>必要な発射角度（度）。有効な解がない場合は NaN。</returns>
		public static float CalculateBallisticFireAngle(Vector3 firePosition, Vector3 targetPosition,
		                                                float launchSpeed, BallisticArcHeight arcHeight)
		{
			return CalculateBallisticFireAngle(firePosition, targetPosition, launchSpeed, arcHeight,
			                                   Mathf.Abs(Physics.gravity.y));
		}

		/// <summary>
		/// Projectile が軌道を飛び終えるまでにかかる時間を計算する
		/// </summary>
		/// <param name="firePosition">Projectile を発射する位置。</param>
		/// <param name="targetPosition">狙う目標位置。</param>
		/// <param name="launchSpeed">Projectile の発射速度。</param>
		/// <param name="fireAngle">Projectile を発射した角度（度）。</param>
		/// <param name="gravity">重力定数（垂直方向のみ。正の値は下向き）</param>
		/// <returns>目標までの軌道を飛び終えるまでの秒数。有効な解がない場合は NaN。</returns>
		public static float CalculateBallisticFlightTime(Vector3 firePosition, Vector3 targetPosition, float launchSpeed,
		                                                 float fireAngle, float gravity)
		{
			float relativeY = firePosition.y - targetPosition.y;

			Vector3 targetVector = targetPosition - firePosition;

			targetVector.y = 0;

			float targetDistance = targetVector.magnitude;

			fireAngle *= Mathf.Deg2Rad;

			float sinFireAngle = Mathf.Sin(fireAngle);

			float a = (launchSpeed * Mathf.Sin(fireAngle)) / gravity;
			float b = Mathf.Sqrt((launchSpeed * launchSpeed * (sinFireAngle * sinFireAngle)) + (2 * gravity * relativeY)) /
			          gravity;

			float flightTime1 = a + b;
			float flightTime2 = a - b;

			float flightDistance1 = launchSpeed * Mathf.Cos(fireAngle) * flightTime1;
			float flightDistance2 = launchSpeed * Mathf.Cos(fireAngle) * flightTime2;

			if (flightTime2 > 0)
			{
				if (Mathf.Abs(targetDistance - flightDistance2) < Mathf.Abs(targetDistance - flightDistance1))
				{
					return flightTime2;
				}
			}

			return flightTime1;
		}

		/// <summary>
		/// Projectile が軌道を飛び終えるまでにかかる時間を計算する
		/// プロジェクトの Physics 設定で定義された垂直方向の重力定数を使用する
		/// </summary>
		/// <param name="firePosition">Projectile を発射する位置。</param>
		/// <param name="targetPosition">狙う目標位置。</param>
		/// <param name="launchSpeed">Projectile の発射速度。</param>
		/// <param name="fireAngle">Projectile を発射した角度（度）。</param>
		/// <returns>目標までの軌道を飛び終えるまでの秒数。有効な解がない場合は NaN。</returns>
		public static float CalculateBallisticFlightTime(Vector3 firePosition, Vector3 targetPosition,
		                                                 float launchSpeed, float fireAngle)
		{
			return CalculateBallisticFlightTime(firePosition, targetPosition, launchSpeed, fireAngle,
			                                    Mathf.Abs(Physics.gravity.y));
		}

		/// <summary>
		/// 指定した発射速度を前提に、弾道 Projectile が移動中の目標に当たるよう、おおよその先読み目標位置を計算する
		/// 目標は等速、Projectile は発射後も一定速度で進むと仮定する。精度はパラメーターで調整できる
		/// </summary>
		/// <param name="firePosition">Projectile の開始位置。</param>
		/// <param name="targetPosition">狙う目標の現在位置。</param>
		/// <param name="targetVelocity">狙う目標の速度を表す Vector。</param>
		/// <param name="launchSpeed">Projectile の初速。</param>
		/// <param name="arcHeight">放物線（「下手投げ」）軌道と直線寄り（「上手投げ」）軌道の優先設定。</param>
		/// <param name="precision">正しい位置に近づけるための反復回数。速い目標ほど高い精度が有効。</param>
		/// <param name="gravity">重力定数（垂直方向のみ。正の値は下向き）</param>
		/// <returns>先読みした目標位置を表す Vector3。解がない場合は Vector3.zero。</returns>
		public static Vector3 CalculateBallisticLeadingTargetPointWithSpeed(Vector3 firePosition, Vector3 targetPosition,
		                                                                    Vector3 targetVelocity, float launchSpeed,
		                                                                    BallisticArcHeight arcHeight, float gravity,
		                                                                    int precision = 2)
		{
			// 精度がない場合は先読みしないため、ここで抜ける
			if (precision <= 1)
			{
				return targetPosition;
			}

			Vector3 testPosition = targetPosition;

			for (int i = 0; i < precision; i++)
			{
				float fireAngle = CalculateBallisticFireAngle(firePosition, testPosition, launchSpeed, arcHeight, gravity);

				float impactTime = CalculateBallisticFlightTime(firePosition, testPosition, launchSpeed, fireAngle, gravity);

				if (float.IsNaN(fireAngle) || float.IsNaN(impactTime))
				{
					return Vector3.zero;
				}

				testPosition = targetPosition + (targetVelocity * impactTime);
			}

			return testPosition;
		}

		/// <summary>
		/// 指定した発射速度を前提に、弾道 Projectile が移動中の目標に当たるよう、おおよその先読み目標位置を計算する
		/// 目標は等速、Projectile は発射後も一定速度で進むと仮定する。精度はパラメーターで調整できる
		/// プロジェクトの Physics 設定で定義された垂直方向の重力定数を使用する
		/// </summary>
		/// <param name="firePosition">Projectile の開始位置。</param>
		/// <param name="targetPosition">狙う目標の現在位置。</param>
		/// <param name="targetVelocity">狙う目標の速度を表す Vector。</param>
		/// <param name="launchSpeed">Projectile の初速。</param>
		/// <param name="arcHeight">放物線（「下手投げ」）軌道と直線寄り（「上手投げ」）軌道の優先設定。</param>
		/// <param name="precision">正しい位置に近づけるための反復回数。速い目標ほど高い精度が有効。</param>
		/// <returns>先読みした目標位置を表す Vector3。解がない場合は Vector3.zero。</returns>
		public static Vector3 CalculateBallisticLeadingTargetPointWithSpeed(Vector3 firePosition, Vector3 targetPosition,
		                                                                    Vector3 targetVelocity, float launchSpeed,
		                                                                    BallisticArcHeight arcHeight, int precision = 2)
		{
			return CalculateBallisticLeadingTargetPointWithSpeed(firePosition, targetPosition, targetVelocity, launchSpeed,
			                                                     arcHeight, Mathf.Abs(Physics.gravity.y), precision);
		}

		/// <summary>
		/// 指定した発射角度を前提に、弾道 Projectile が移動中の目標に当たるよう、おおよその先読み目標位置を計算する
		/// 目標は等速、Projectile は発射後も一定速度で進むと仮定する。精度はパラメーターで調整できる
		/// プロジェクトの Physics 設定で定義された垂直方向の重力定数を使用する
		/// </summary>
		/// <param name="firePosition">Projectile の開始位置。</param>
		/// <param name="targetPosition">狙う目標の現在位置。</param>
		/// <param name="targetVelocity">狙う目標の速度を表す Vector。</param>
		/// <param name="launchAngle">Projectile を発射する角度。</param>
		/// <param name="arcHeight">放物線（「下手投げ」）軌道と直線寄り（「上手投げ」）軌道の優先設定。</param>
		/// <param name="gravity">重力定数（垂直方向のみ。正の値は下向き）</param>
		/// <param name="precision">正しい位置に近づけるための反復回数。速い目標ほど高い精度が有効。</param>
		/// <returns>先読みした目標位置を表す Vector3。解がない場合は Vector3.zero。</returns>
		public static Vector3 CalculateBallisticLeadingTargetPointWithAngle(Vector3 firePosition,
		                                                                    Vector3 targetPosition,
		                                                                    Vector3 targetVelocity, float launchAngle,
		                                                                    BallisticArcHeight arcHeight, float gravity,
		                                                                    int precision = 2)
		{
			// 精度がない場合は先読みしないため、ここで抜ける
			if (precision <= 1)
			{
				return targetPosition;
			}

			Vector3 testPosition = targetPosition;

			for (int i = 0; i < precision; i++)
			{
				float launchSpeed = CalculateBallisticFireVectorFromAngle(firePosition, testPosition, launchAngle, gravity)
					.magnitude;

				float impactTime = CalculateBallisticFlightTime(firePosition, testPosition, launchSpeed, launchAngle, gravity);

				if (float.IsNaN(launchSpeed) || float.IsNaN(impactTime))
				{
					return Vector3.zero;
				}

				testPosition = targetPosition + (targetVelocity * impactTime);
			}

			return testPosition;
		}

		/// <summary>
		/// 指定した発射角度を前提に、弾道 Projectile が移動中の目標に当たるよう、おおよその先読み目標位置を計算する
		/// 目標は等速、Projectile は発射後も一定速度で進むと仮定する。精度はパラメーターで調整できる
		/// プロジェクトの Physics 設定で定義された垂直方向の重力定数を使用する
		/// </summary>
		/// <param name="firePosition">Projectile の開始位置。</param>
		/// <param name="targetPosition">狙う目標の現在位置。</param>
		/// <param name="targetVelocity">狙う目標の速度を表す Vector。</param>
		/// <param name="launchAngle">Projectile を発射する角度。</param>
		/// <param name="arcHeight">放物線（「下手投げ」）軌道と直線寄り（「上手投げ」）軌道の優先設定。</param>
		/// <param name="precision">正しい位置に近づけるための反復回数。速い目標ほど高い精度が有効。</param>
		/// <returns>先読みした目標位置を表す Vector3。解がない場合は Vector3.zero。</returns>
		public static Vector3 CalculateBallisticLeadingTargetPointWithAngle(Vector3 firePosition,
		                                                                    Vector3 targetPosition,
		                                                                    Vector3 targetVelocity, float launchAngle,
		                                                                    BallisticArcHeight arcHeight, int precision = 2)
		{
			return CalculateBallisticLeadingTargetPointWithAngle(firePosition, targetPosition, targetVelocity,
			                                                     launchAngle, arcHeight, Mathf.Abs(Physics.gravity.y),
			                                                     precision);
		}
	}
}
