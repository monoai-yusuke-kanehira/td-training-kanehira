using Core.Health;
using UnityEngine;

namespace ActionGameFramework.Spawning
{
	/// <summary>
	/// HitObject はヒット情報を受け取る特殊な GameObject
	/// 例: ダメージ量を使ってサイズを変える
	/// </summary>
	public abstract class HitObject : MonoBehaviour
	{
		public abstract void SetHitInfo(HitInfo hitInfo);
	}
}
