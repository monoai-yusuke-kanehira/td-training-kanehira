using System.Collections.Generic;
using UnityEngine;

namespace Core.Utilities
{
	public static class VectorHelper
	{
		/// <summary>
		/// 複数のコンポーネントオブジェクトの平均位置を求めるヘルパー関数
		/// Transformを持っていることを前提にしている
		/// </summary>
		/// <param name="components">
		/// 平均を取るコンポーネントのリスト
		/// </param>
		/// <typeparam name="TComponent">
		/// Transformを持つUnityコンポーネント
		/// </typeparam>
		/// <returns>
		/// 平均位置
		/// </returns>
		public static Vector3 FindAveragePosition<TComponent>(TComponent[] components) where TComponent : Component
		{
			Vector3 output = Vector3.zero;
			foreach (TComponent component in components)
			{
				if (component == null)
				{
					continue;
				}
				output += component.transform.position;
			}
			return output / components.Length;
		}

		/// <summary>
		/// 複数のコンポーネントオブジェクトの平均位置を求めるヘルパー関数
		/// Transformを持っていることを前提にしている
		/// </summary>
		/// <param name="components">
		/// 平均を取るコンポーネントのリスト
		/// </param>
		/// <typeparam name="TComponent">
		/// Transformを持つUnityコンポーネント
		/// </typeparam>
		/// <returns>
		/// 平均速度
		/// </returns>
		public static Vector3 FindAverageVelocity<TComponent>(TComponent[] components) where TComponent : Component
		{
			Vector3 output = Vector3.zero;
			foreach (TComponent component in components)
			{
				if (component == null)
				{
					continue;
				}
				var rigidbody = component.GetComponent<Rigidbody>();
				if (rigidbody == null)
				{
					continue;
				}
				output += rigidbody.linearVelocity;
			}
			return output / components.Length;
		}

		/// <summary>
		/// 複数のコンポーネントオブジェクトの平均位置を求めるヘルパー関数
		/// Transformを持っていることを前提にしている
		/// </summary>
		/// <param name="components">
		/// 平均を取るコンポーネントのリスト
		/// </param>
		/// <typeparam name="TComponent">
		/// Transformを持つUnityコンポーネント
		/// </typeparam>
		/// <returns>
		/// 平均位置
		/// </returns>
		public static Vector3 FindAveragePosition<TComponent>(List<TComponent> components) where TComponent : Component
		{
			Vector3 output = Vector3.zero;
			foreach (TComponent component in components)
			{
				if (component == null)
				{
					continue;
				}
				output += component.transform.position;
			}
			return output / components.Count;
		}

		/// <summary>
		/// 複数のコンポーネントオブジェクトの平均位置を求めるヘルパー関数
		/// Transformを持っていることを前提にしている
		/// </summary>
		/// <param name="components">
		/// 平均を取るコンポーネントのリスト
		/// </param>
		/// <typeparam name="TComponent">
		/// Transformを持つUnityコンポーネント
		/// </typeparam>
		/// <returns>
		/// 平均速度
		/// </returns>
		public static Vector3 FindAverageVelocity<TComponent>(List<TComponent> components) where TComponent : Component
		{
			Vector3 output = Vector3.zero;
			foreach (TComponent component in components)
			{
				if (component == null)
				{
					continue;
				}
				var rigidbody = component.GetComponent<Rigidbody>();
				if (rigidbody == null)
				{
					continue;
				}
				output += rigidbody.linearVelocity;
			}
			return output / components.Count;
		}
	}
}