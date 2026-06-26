using System;
using UnityEngine;

namespace Core.Utilities
{
	/// <summary>
	/// 六角形グリッドのキューブ座標を保持する構造体。派生したZ座標を提供する
	/// z = x + y として、3つ目の軸を表す
	/// </summary>
	public struct HexPoint : IEquatable<HexPoint>
	{
		/// <summary>
		/// 六角形上の点のX座標
		/// </summary>
		public readonly int x;

		/// <summary>
		/// 六角形上の点のY座標
		/// </summary>
		public readonly int y;

		/// <summary>
		/// 六角形上の点のZ座標。この値はxとyから計算される
		/// </summary>
		public readonly int z;

		/// <summary>
		/// このHexPointベクトルの大きさ（原点からの六角形距離）を計算する
		/// </summary>
		public int magnitude
		{
			get { return (Mathf.Abs(x) + Mathf.Abs(y) + Mathf.Abs(z)) / 2; }
		}

		/// <summary>
		/// x、yの2つの座標で新しいHexPointを初期化する
		/// </summary>
		public HexPoint(int x, int y)
		{
			this.x = x;
			this.y = y;
			this.z = x + y;
		}

		/// <summary>
		/// x、z座標から新しいHexPointを初期化する
		/// </summary>
		public static HexPoint FromXZ(int x, int z)
		{
			int y = z - x;
			return new HexPoint(x, y);
		}

		/// <summary>
		/// y、z座標から新しいHexPointを初期化する
		/// </summary>
		public static HexPoint FromYZ(int y, int z)
		{
			int x = z - y;
			return new HexPoint(x, y);
		}

		public bool Equals(HexPoint other)
		{
			return other.x == x && other.y == y;
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj))
			{
				return false;
			}
			return obj is HexPoint && Equals((HexPoint) obj);
		}

		/// <summary>
		/// 2つの素数を掛け合わせるシンプルなハッシュ
		/// </summary>
		public override int GetHashCode()
		{
			unchecked
			{
				return (x.GetHashCode() * 22447) ^ (y.GetHashCode() * 31);
			}
		}

		public override string ToString()
		{
			return string.Format("X: {0}, Y: {1}, Z: {2}", x, y, z);
		}

		/// <summary>
		/// 指定されたHexPointを原点まわりに反時計回りで60度回転する
		/// </summary>
		public static HexPoint RotateLeft(HexPoint original)
		{
			return new HexPoint(-original.y, original.z);
		}

		/// <summary>
		/// 指定されたHexPointを指定点まわりに反時計回りで60度回転する
		/// </summary>
		public static HexPoint RotateLeft(HexPoint original, HexPoint origin)
		{
			return RotateLeft(original - origin) + origin;
		}

		/// <summary>
		/// 指定されたHexPointを原点まわりに時計回りで60度回転する
		/// </summary>
		public static HexPoint RotateRight(HexPoint original)
		{
			return new HexPoint(original.z, -original.x);
		}

		/// <summary>
		/// 指定されたHexPointを指定点まわりに時計回りで60度回転する
		/// </summary>
		public static HexPoint RotateRight(HexPoint original, HexPoint origin)
		{
			return RotateRight(original - origin) + origin;
		}

		/// <summary>
		/// 指定されたHexPointを原点まわりに反時計回りで120度回転する
		/// </summary>
		public static HexPoint RotateLeft120(HexPoint original)
		{
			return new HexPoint(-original.z, original.x);
		}

		/// <summary>
		/// 指定されたHexPointを指定点まわりに反時計回りで120度回転する
		/// </summary>
		public static HexPoint RotateLeft120(HexPoint original, HexPoint origin)
		{
			return RotateLeft120(original - origin) + origin;
		}

		/// <summary>
		/// 指定されたHexPointを原点まわりに時計回りで120度回転する
		/// </summary>
		public static HexPoint RotateRight120(HexPoint original)
		{
			return new HexPoint(original.y, -original.z);
		}

		/// <summary>
		/// 指定されたHexPointを指定点まわりに時計回りで120度回転する
		/// </summary>
		public static HexPoint RotateRight120(HexPoint original, HexPoint origin)
		{
			return RotateRight120(original - origin) + origin;
		}

		/// <summary>
		/// 指定されたHexPointを原点まわりに180度回転する
		/// </summary>
		public static HexPoint Rotate180(HexPoint original)
		{
			return new HexPoint(-original.x, -original.y);
		}

		/// <summary>
		/// 指定されたHexPointを指定点まわりに180度回転する
		/// </summary>
		public static HexPoint Rotate180(HexPoint original, HexPoint origin)
		{
			return Rotate180(original - origin) + origin;
		}

		/// <summary>
		/// 指定されたHexPointをx軸で反転する
		/// </summary>
		public static HexPoint ReflectX(HexPoint original)
		{
			int x = original.z;
			int y = -original.y;

			return new HexPoint(x, y);
		}

		/// <summary>
		/// 指定されたHexPointを、yが指定値となる直線で反転する
		/// </summary>
		public static HexPoint ReflectX(HexPoint original, int y)
		{
			var offset = new HexPoint(0, y);

			return ReflectX(original - offset) + offset;
		}

		/// <summary>
		/// 指定されたHexPointをy軸で反転する
		/// </summary>
		public static HexPoint ReflectY(HexPoint original)
		{
			int x = -original.x;
			int y = original.z;

			return new HexPoint(x, y);
		}

		/// <summary>
		/// 指定されたHexPointを、xが指定値となる直線で反転する
		/// </summary>
		public static HexPoint ReflectY(HexPoint original, int x)
		{
			var offset = new HexPoint(x, 0);

			return ReflectY(original - offset) + offset;
		}

		/// <summary>
		/// 指定されたHexPointをz軸で反転する
		/// </summary>
		public static HexPoint ReflectZ(HexPoint original)
		{
			int x = -original.y;
			int y = -original.x;

			return new HexPoint(x, y);
		}

		/// <summary>
		/// 指定されたHexPointを、zが指定値となる直線で反転する
		/// </summary>
		public static HexPoint ReflectZ(HexPoint original, int z)
		{
			var offset = FromXZ(0, z);

			return ReflectZ(original - offset) + offset;
		}
		
		// 数学演算子と変換
		// 等価演算子
		public static bool operator ==(HexPoint left, HexPoint right)
		{
			return left.Equals(right);
		}

		public static bool operator !=(HexPoint left, HexPoint right)
		{
			return !left.Equals(right);
		}
		
		// IntVector2との相互変換
		public static explicit operator IntVector2(HexPoint hexPoint)
		{
			return new IntVector2(hexPoint.x, hexPoint.y);
		}
		
		public static explicit operator HexPoint(IntVector2 vector)
		{
			return new HexPoint(vector.x, vector.y);
		}
		
		// 数学演算子
		public static HexPoint operator +(HexPoint left, HexPoint right)
		{
			return new HexPoint(left.x + right.x, left.y + right.y);
		}

		public static HexPoint operator -(HexPoint left, HexPoint right)
		{
			return new HexPoint(left.x - right.x, left.y - right.y);
		}

		public static HexPoint operator *(int scale, HexPoint right)
		{
			return new HexPoint(right.x * scale, right.y * scale);
		}

		public static HexPoint operator *(HexPoint left, int scale)
		{
			return new HexPoint(left.x * scale, left.y * scale);
		}

		public static HexPoint operator -(HexPoint left)
		{
			return new HexPoint(-left.x, -left.y);
		}
	}
}