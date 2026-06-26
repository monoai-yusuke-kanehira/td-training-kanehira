using System;
using JetBrains.Annotations;
using UnityEngine;

namespace Core.Extensions
{
	public static class GameObjectExtensions
	{
		/// <summary>
		/// このGameObjectとすべての子のレイヤーを設定します
		/// </summary>
		public static void SetLayerRecursively(this GameObject gameObject, int layer)
		{
			gameObject.layer = layer;

			// 非再帰・非アロケーションの走査
			Transform goTransform = gameObject.transform;
			if (goTransform.childCount > 0)
			{
				WalkHeirarchyAndSetLayer(goTransform, layer);
			}
		}

		/// <summary>
		/// <paramref name="root"/> から階層をたどり、すべての子を確認してレイヤーを変更します。
		/// 非アロケーションかつ非再帰  
		/// </summary>
		/// <param name="root">The root object to start our search from</param>
		/// <param name="layer">The layer to set the game object too</param>
		static void WalkHeirarchyAndSetLayer([NotNull] Transform root, int layer)
		{
			if (root.childCount == 0)
			{
				throw new InvalidOperationException("Root transform has no children");
			}

			Transform workingTransform = root.GetChild(0);

			// rootに戻るまで処理します
			while (workingTransform != root)
			{
				// レイヤーを変更します
				workingTransform.gameObject.layer = layer;

				// 子がある場合は子を取得します
				if (workingTransform.childCount > 0)
				{
					workingTransform = workingTransform.GetChild(0);
				}
				// 子がない場合は兄弟を探します
				else
				{
					// 兄弟に設定します
					if (!TryGetNextSibling(ref workingTransform))
					{
						// それ以外の場合は親をたどり、その親の次の兄弟を探します
						workingTransform = workingTransform.parent;

						while (workingTransform != root &&
						       !TryGetNextSibling(ref workingTransform))
						{
							workingTransform = workingTransform.parent;
						}
					}
				}
			}
		}

		/// <summary>
		/// <paramref name="transform"/> の兄弟へ進めるか試します
		/// </summary>
		/// <param name="transform">The transform whose siblings we're looking for</param>
		/// <returns>True if we had a sibling. <paramref name="transform"/> will now refer to it.</returns>
		static bool TryGetNextSibling([NotNull] ref Transform transform)
		{
			Transform parent = transform.parent;
			int siblingIndex = transform.GetSiblingIndex();

			// 子がない場合は兄弟を取得します
			if (parent.childCount > siblingIndex + 1)
			{
				transform = parent.GetChild(siblingIndex + 1);
				return true;
			}

			return false;
		}
	}
}