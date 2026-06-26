using TowerDefense.Targetting;
using UnityEngine;

namespace TowerDefense.Towers
{
	/// <summary>
	/// Tower‚ÌAffector‚ª‰e‹¿”ÍˆÍ‚ð•\Ž¦‚·‚é‚½‚ß‚ÉŽÀ‘•‚·‚éInterface
	/// </summary>
	public interface ITowerRadiusProvider
	{
		float effectRadius { get; }
		Color effectColor { get; }
		Targetter targetter { get; }
	}
}