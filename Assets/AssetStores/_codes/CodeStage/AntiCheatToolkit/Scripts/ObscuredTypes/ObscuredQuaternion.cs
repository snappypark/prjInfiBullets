using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace CodeStage.AntiCheat.ObscuredTypes
{
	/// <summary>
	/// Use it instead of regular <c>Quaternion</c> for any cheating-sensitive variables.
	/// </summary>
	/// <strong>\htmlonly<font color="FF4040">WARNING:</font>\endhtmlonly Doesn't mimic regular type API, thus should be used with extra caution.</strong> Cast it to regular, not obscured type to work with regular APIs.<br/>
	/// <strong><em>Regular type is faster and memory wiser comparing to the obscured one!</em></strong>
	[Serializable]
	public struct ObscuredQuaternion
	{
		private static int cryptoKey = 120205;
		private static readonly Quaternion identity = Quaternion.identity;

#if UNITY_EDITOR
		// For internal Editor usage only (may be useful for drawers).
		public static int cryptoKeyEditor = cryptoKey;
#endif
		[SerializeField]
		private int currentCryptoKey;

		[SerializeField]
		private RawEncryptedQuaternion hiddenValue;

		[SerializeField]
		private bool inited;

		[SerializeField]
		private Quaternion fakeValue;

		[SerializeField]
		private bool fakeValueActive;

		private ObscuredQuaternion(Quaternion value)
		{
			currentCryptoKey = cryptoKey;
			hiddenValue = Encrypt(value);

			var detectorRunning = Detectors.ObscuredCheatingDetector.ExistsAndIsRunning;
			fakeValue = detectorRunning ? value : identity;
			fakeValueActive = detectorRunning;

			inited = true;
		}

		/// <summary>
		/// Mimics constructor of regular Quaternion. Please note, passed components are not Euler Angles.
		/// </summary>
		/// <param name="x">X component of the quaternion</param>
		/// <param name="y">Y component of the quaternion</param>
		/// <param name="z">Z component of the quaternion</param>
		/// <param name="w">W component of the quaternion</param>
		public ObscuredQuaternion(float x, float y, float z, float w)
		{
			currentCryptoKey = cryptoKey;
			hiddenValue = Encrypt(x, y, z, w, currentCryptoKey);

			if (Detectors.ObscuredCheatingDetector.ExistsAndIsRunning)
			{
				fakeValue.x = x;
				fakeValue.y = y;
				fakeValue.z = z;
				fakeValue.w = w;
				fakeValueActive = true;
			}
			else
			{
				fakeValue = identity;
				fakeValueActive = false;
			}
			
			inited = true;
		}

		/// <summary>
		/// Allows to change default crypto key of this type instances. All new instances will use specified key.<br/>
		/// All current instances will use previous key unless you call ApplyNewCryptoKey() on them explicitly.
		/// </summary>
		public static void SetNewCryptoKey(int newKey)
		{
			cryptoKey = newKey;
		}

		/// <summary>
		/// Use this simple encryption method to encrypt any Quaternion value, uses default crypto key.
		/// </summary>
		public static RawEncryptedQuaternion Encrypt(Quaternion value)
		{
			return Encrypt(value, 0);
		}

		/// <summary>
		/// Use this simple encryption method to encrypt any Quaternion value, uses passed crypto key.
		/// </summary>
		public static RawEncryptedQuaternion Encrypt(Quaternion value, int key)
		{
			return Encrypt(value.x, value.y, value.z, value.w, key);
		}

		/// <summary>
		/// Use this simple encryption method to encrypt Quaternion components, uses passed crypto key.
		/// Please note, passed components are not an Euler Angles.
		/// </summary>
		public static RawEncryptedQuaternion Encrypt(float x, float y, float z, float w, int key)
		{
			if (key == 0)
			{
				key = cryptoKey;
			}

			RawEncryptedQuaternion result;
			result.x = ObscuredFloat.Encrypt(x, key);
			result.y = ObscuredFloat.Encrypt(y, key);
			result.z = ObscuredFloat.Encrypt(z, key);
			result.w = ObscuredFloat.Encrypt(w, key);

			return result;
		}

		/// <summary>
		/// Use it to decrypt RawEncryptedQuaternion you got from Encrypt(), uses default crypto key.
		/// </summary>
		public static Quaternion Decrypt(RawEncryptedQuaternion value)
		{
			return Decrypt(value, 0);
		}

		/// <summary>
		/// Use it to decrypt RawEncryptedQuaternion you got from Encrypt(), uses passed crypto key.
		/// </summary>
		public static Quaternion Decrypt(RawEncryptedQuaternion value, int key)
		{
			if (key == 0)
			{
				key = cryptoKey;
			}

			Quaternion result;
			result.x = ObscuredFloat.Decrypt(value.x, key);
			result.y = ObscuredFloat.Decrypt(value.y, key);
			result.z = ObscuredFloat.Decrypt(value.z, key);
			result.w = ObscuredFloat.Decrypt(value.w, key);

			return result;
		}

		/// <summary>
		/// Use it after SetNewCryptoKey() to re-encrypt current instance using new crypto key.
		/// </summary>
		public void ApplyNewCryptoKey()
		{
			if (currentCryptoKey != cryptoKey)
			{
				hiddenValue = Encrypt(InternalDecrypt(), cryptoKey);
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
			hiddenValue = Encrypt(decrypted, currentCryptoKey);
		}

		/// <summary>
		/// Allows to pick current obscured value as is.
		/// </summary>
		/// Use it in conjunction with SetEncrypted().<br/>
		/// Useful for saving data in obscured state.
		public RawEncryptedQuaternion GetEncrypted()
		{
			ApplyNewCryptoKey();
			return hiddenValue;
		}

		/// <summary>
		/// Allows to explicitly set current obscured value.
		/// </summary>
		/// Use it in conjunction with GetEncrypted().<br/>
		/// Useful for loading data stored in obscured state.
		public void SetEncrypted(RawEncryptedQuaternion encrypted)
		{
			inited = true;
			hiddenValue = encrypted;

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
		}

		/// <summary>
		/// Alternative to the type cast, use if you wish to get decrypted value 
		/// but can't or don't want to use cast to the regular type.
		/// </summary>
		/// <returns>Decrypted value.</returns>
		public Quaternion GetDecrypted()
		{
			return InternalDecrypt();
		}

		private Quaternion InternalDecrypt()
		{
			if (!inited)
			{
				currentCryptoKey = cryptoKey;
				hiddenValue = Encrypt(identity);
				fakeValue = identity;
				fakeValueActive = false;
				inited = true;

				return identity;
			}

			Quaternion value;

			value.x = ObscuredFloat.Decrypt(hiddenValue.x, currentCryptoKey);
			value.y = ObscuredFloat.Decrypt(hiddenValue.y, currentCryptoKey);
			value.z = ObscuredFloat.Decrypt(hiddenValue.z, currentCryptoKey);
			value.w = ObscuredFloat.Decrypt(hiddenValue.w, currentCryptoKey);

			if (Detectors.ObscuredCheatingDetector.ExistsAndIsRunning && fakeValueActive && !CompareQuaternionsWithTolerance(value, fakeValue))
			{
				Detectors.ObscuredCheatingDetector.Instance.OnCheatingDetected();
			}

			return value;
		}

		private bool CompareQuaternionsWithTolerance(Quaternion q1, Quaternion q2)
		{
			var epsilon = Detectors.ObscuredCheatingDetector.Instance.quaternionEpsilon;
			return Mathf.Abs(q1.x - q2.x) < epsilon &&
                   Mathf.Abs(q1.y - q2.y) < epsilon &&
                   Mathf.Abs(q1.z - q2.z) < epsilon &&
                   Mathf.Abs(q1.w - q2.w) < epsilon;
		}

		#region operators, overrides, interface implementations
		//! @cond
		public static implicit operator ObscuredQuaternion(Quaternion value)
		{
			return new ObscuredQuaternion(value);
		}

		public static implicit operator Quaternion(ObscuredQuaternion value)
		{
			return value.InternalDecrypt();
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

		/// <summary>
		/// Returns a nicely formatted string of the Quaternion.
		/// </summary>
		public override string ToString()
		{
			return InternalDecrypt().ToString();
		}

		/// <summary>
		/// Returns a nicely formatted string of the Quaternion.
		/// </summary>
		public string ToString(string format)
		{
			return InternalDecrypt().ToString(format);
		}

		//! @endcond
		#endregion

		/// <summary>
		/// Used to store encrypted Quaternion.
		/// </summary>
		[Serializable]
		public struct RawEncryptedQuaternion
		{
			/// <summary>
			/// Encrypted value
			/// </summary>
			public int x;

			/// <summary>
			/// Encrypted value
			/// </summary>
			public int y;

			/// <summary>
			/// Encrypted value
			/// </summary>
			public int z;

			/// <summary>
			/// Encrypted value
			/// </summary>
			public int w;
		}
	}
}