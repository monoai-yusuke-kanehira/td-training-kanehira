using UnityEngine;

namespace TowerDefense.UI
{
	/// <summary>
	/// Transformに一定の回転を適用するシンプルなコンポーネント
	/// </summary>
	public class Rotator : MonoBehaviour
	{
		public Vector3 rotationSpeed;
	
		void Update ()
		{
			transform.localEulerAngles += rotationSpeed;
		}
	}
}
