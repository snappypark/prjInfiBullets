using System;
using UnityEngine;
using System.Runtime.InteropServices;
using CodeStage.AntiCheat.Common;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace CodeStage.AntiCheat.ObscuredTypes
{
	/// <summary>
	/// Use it instead of regular <c>double</c> for any cheating-sensitive variables.
	/// </summary>
	/// <strong><em>Regular type is faster and memory wiser comparing to the obscured one!</em></strong>
	[Serializable]
	public struct ObscuredDouble : IEquatable<ObscuredDouble>, IFormattable 
	{
		private static long cryptoKey = 210987L;

#if UNITY_EDITOR
		// For internal Editor usage only (may be useful for drawers).
		public static long cryptoKeyEditor = cryptoKey;
#endif

		[SerializeField]
		private long currentCryptoKey;

		[SerializeField]
		private ACTkByte8 hiddenValue;

		[SerializeField]
		[FormerlySerializedAs("hiddenValue")]
#pragma warning disable 414
		private byte[] hiddenValueOld;
#pragma warning restore 414

		[SerializeField]
		private bool inited;

		[SerializeField]
		private double fakeValue;

		[SerializeField]
		private bool fakeValueActive;

		private ObscuredDouble(double value)
		{
			currentCryptoKey = cryptoKey;
			hiddenValue = InternalEncrypt(value);
			hiddenValueOld = null;

			var detectorRunning = Detectors.ObscuredCheatingDetector.ExistsAndIsRunning;
			fakeValue = detectorRunning ? value : 0;
			fakeValueActive = detectorRunning;

			inited = true;
		}

		/// <summary>
		/// Allows to change default crypto key of this type instances. All new instances will use specified key.<br/>
		/// All current instances will use previous key unless you call ApplyNewCryptoKey() on them explicitly.
		/// </summary>
		public static void SetNewCryptoKey(long newKey)
		{
			cryptoKey = newKey;
		}

		/// <summary>
		/// Use this simple encryption method to encrypt any double value, uses default crypto key.
		/// </summary>
		public static long Encrypt(double value)
		{
			return Encrypt(value, cryptoKey);
		}

		/// <summary>
		/// Use this simple encryption method to encrypt any double value, uses passed crypto key.
		/// </summary>
		public static long Encrypt(double value, long key)
		{
			var u = new DoubleLongBytesUnion();
			u.d = value;
			u.l = u.l ^ key;

			return u.l;
		}

		private static ACTkByte8 InternalEncrypt(double value)
		{
			return InternalEncrypt(value, 0L);
		}

		private static ACTkByte8 InternalEncrypt(double value, long key)
		{
			var currentKey = key;
			if (currentKey == 0L)
			{
				currentKey = cryptoKey;
			}

			var u = new DoubleLongBytesUnion();
			u.d = value;
			u.l = u.l ^ currentKey;

			return u.b8;
		}

		/// <summary>
		/// Use it to decrypt long you got from Encrypt(double) back to double, uses default crypto key.
		/// </summary>
		public static double Decrypt(long value)
		{
			return Decrypt(value, cryptoKey);
		}

		/// <summary>
		/// Use it to decrypt long you got from Encrypt(double) back to double, uses passed crypto key.
		/// </summary>
		public static double Decrypt(long value, long key)
		{
			var u = new DoubleLongBytesUnion();
			u.l = value ^ key;
			return u.d;
		}

		/// <summary>
		/// Use it after SetNewCryptoKey() to re-encrypt current instance using new crypto key.
		/// </summary>
		public void ApplyNewCryptoKey()
		{
			if (currentCryptoKey != cryptoKey)
			{
				hiddenValue = InternalEncrypt(InternalDecrypt(), cryptoKey);
				currentCryptoKey = cryptoKey;
			}
		}

		/// <summary>
		/// Allows to change current crypto key to the new random value and re-encrypt variable using it.
		/// Use it for extra protection against 'unknown value' search.
		/// Just call it sometimes when your variable doesn't change to fool the cheater.
		/// </summary>
		public void RandomizeCryptoKey()
		{
			var decrypted = InternalDecrypt();

			do
			{
				currentCryptoKey = Random.Range(int.MinValue, int.MaxValue);
			} while (currentCryptoKey == 0);

			hiddenValue = InternalEncrypt(decrypted, currentCryptoKey);
		}

		/// <summary>
		/// Allows to pick current obscured value as is.
		/// </summary>
		/// Use it in conjunction with SetEncrypted().<br/>
		/// Useful for saving data in obscured state.
		public long GetEncrypted()
		{
			ApplyNewCryptoKey();

			var union = new DoubleLongBytesUnion();
			union.b8 = hiddenValue;

			return union.l;
		}

		/// <summary>
		/// Allows to explicitly set current obscured value.
		/// </summary>
		/// Use it in conjunction with GetEncrypted().<br/>
		/// Useful for loading data stored in obscured state.
		public void SetEncrypted(long encrypted)
		{
			inited = true;
			var union = new DoubleLongBytesUnion();
			union.l = encrypted;

			hiddenValue = union.b8;

			if (currentCryptoKey == 0)
			{
				currentCryptoKey = cryptoKey;
			}

			if (Detectors.ObscuredCheatingDetector.ExistsAndIsRunning)
			{
				fakeValue = InternalDecrypt();
				fakeValueActive = true;
			}
			else
			{
				fakeValueActive = false;
			}

			if (currentCryptoKey == 0)
			{
				currentCryptoKey = cryptoKey;
			}
		}

		/// <summary>
		/// Alternative to the type cast, use if you wish to get decrypted value 
		/// but can't or don't want to use cast to the regular type.
		/// </summary>
		/// <returns>Decrypted value.</returns>
		public double GetDecrypted()
		{
			return InternalDecrypt();
		}

		private double InternalDecrypt()
		{
			if (!inited)
			{
				currentCryptoKey = cryptoKey;
				hiddenValue = InternalEncrypt(0);
				fakeValue = 0;
				fakeValueActive = false;
				inited = true;

				return 0;
			}

			var union = new DoubleLongBytesUnion();
			union.b8 = hiddenValue;

			union.l = union.l ^ currentCryptoKey;

			var decrypted = union.d;

			if (Detectors.ObscuredCheatingDetector.ExistsAndIsRunning && fakeValueActive && Mathf.Abs((float)(decrypted - fakeValue)) > 0.000001d)
			{
				Detectors.ObscuredCheatingDetector.Instance.OnCheatingDetected();
			}

			return decrypted;
		}

		#region operators, overrides, interface implementations
		//! @cond
		public static implicit operator ObscuredDouble(double value)
		{
			return new ObscuredDouble(value);
		}

		public static implicit operator double(ObscuredDouble value)
		{
			return value.InternalDecrypt();
		}

		public static explicit operator ObscuredDouble(ObscuredFloat f)
		{
			return (float)f;
		}

		public static ObscuredDouble operator ++(ObscuredDouble input)
		{
			var decrypted = input.InternalDecrypt() + 1d;
			input.hiddenValue = InternalEncrypt(decrypted, input.currentCryptoKey);

			if (Detectors.ObscuredCheatingDetector.ExistsAndIsRunning)
			{
				input.fakeValue = decrypted;
				input.fakeValueActive = true;
			}
			else
			{
				input.fakeValueActive = false;
			}

			return input;
		}

		public static ObscuredDouble operator --(ObscuredDouble input)
		{
			var decrypted = input.InternalDecrypt() - 1d;
			input.hiddenValue = InternalEncrypt(decrypted, input.currentCryptoKey);

			if (Detectors.ObscuredCheatingDetector.ExistsAndIsRunning)
			{
				input.fakeValue = decrypted;
				input.fakeValueActive = true;
			}
			else
			{
				input.fakeValueActive = false;
			}

			return input;
		}

		/// <summary>
		/// Converts the numeric value of this instance to its equivalent string representation.
		/// </summary>
		/// 
		/// <returns>
		/// The string representation of the value of this instance.
		/// </returns>
		public override string ToString()
		{
			return InternalDecrypt().ToString();
		}

		/// <summary>
		/// Converts the numeric value of this instance to its equivalent string representation, using the specified format.
		/// </summary>
		/// 
		/// <returns>
		/// The string representation of the value of this instance as specified by <paramref name="format"/>.
		/// </returns>
		/// <param name="format">A numeric format string (see Remarks).</param><exception cref="T:System.FormatException"><paramref name="format"/> is invalid. </exception><filterpriority>1</filterpriority>
		public string ToString(string format)
		{
			return InternalDecrypt().ToString(format);
		}

		/// <summary>
		/// Converts the numeric value of this instance to its equivalent string representation using the specified culture-specific format information.
		/// </summary>
		/// 
		/// <returns>
		/// The string representation of the value of this instance as specified by <paramref name="provider"/>.
		/// </returns>
		/// <param name="provider">An <see cref="T:System.IFormatProvider"/> that supplies culture-specific formatting information. </param><filterpriority>1</filterpriority>
		public string ToString(IFormatProvider provider)
		{
			return InternalDecrypt().ToString(provider);
		}

		/// <summary>
		/// Converts the numeric value of this instance to its equivalent string representation using the specified format and culture-specific format information.
		/// </summary>
		/// 
		/// <returns>
		/// The string representation of the value of this instance as specified by <paramref name="format"/> and <paramref name="provider"/>.
		/// </returns>
		/// <param name="format">A numeric format string (see Remarks).</param><param name="provider">An <see cref="T:System.IFormatProvider"/> that supplies culture-specific formatting information. </param><filterpriority>1</filterpriority>
		public string ToString(string format, IFormatProvider provider)
		{
			return InternalDecrypt().ToString(format, provider);
		}

		/// <summary>
		/// Returns a value indicating whether this instance is equal to a specified object.
		/// </summary>
		/// 
		/// <returns>
		/// true if <paramref name="obj"/> is an instance of ObscuredDouble and equals the value of this instance; otherwise, false.
		/// </returns>
		/// <param name="obj">An object to compare with this instance. </param><filterpriority>2</filterpriority>
		public override bool Equals(object obj)
		{
			if (!(obj is ObscuredDouble))
				return false;
			return Equals((ObscuredDouble)obj);
		}

		/// <summary>
		/// Returns a value indicating whether this instance and a specified <see cref="T:System.Double"/> object represent the same value.
		/// </summary>
		/// 
		/// <returns>
		/// true if <paramref name="obj"/> is equal to this instance; otherwise, false.
		/// </returns>
		/// <param name="obj">A <see cref="T:System.Double"/> object to compare to this instance.</param><filterpriority>2</filterpriority>
		public bool Equals(ObscuredDouble obj)
		{
			return obj.InternalDecrypt().Equals(InternalDecrypt());
		}

		/// <summary>
		/// Returns the hash code for this instance.
		/// </summary>
		/// 
		/// <returns>
		/// A 32-bit signed integer hash code.
		/// </returns>
		/// <filterpriority>2</filterpriority>
		public override int GetHashCode()
		{
			return InternalDecrypt().GetHashCode();
		}
		//! @endcond
		#endregion

		[StructLayout(LayoutKind.Explicit)]
		private struct DoubleLongBytesUnion
		{
			[FieldOffset(0)]
			public double d;

			[FieldOffset(0)]
			public long l;

			[FieldOffset(0)]
			public ACTkByte8 b8;
		}
	}
}