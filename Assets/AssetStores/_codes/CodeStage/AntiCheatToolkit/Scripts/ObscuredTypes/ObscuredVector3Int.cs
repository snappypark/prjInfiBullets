#if UNITY_2017_2_OR_NEWER
using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace CodeStage.AntiCheat.ObscuredTypes
{
	/// <summary>
	/// Use it instead of regular <c>Vector3Int</c> for any cheating-sensitive variables.
	/// </summary>
	/// <strong>\htmlonly<font color="FF4040">WARNING:</font>\endhtmlonly Doesn't mimic regular type API, thus should be used with extra caution.</strong> Cast it to regular, not obscured type to work with regular APIs.<br/>
	/// <strong><em>Regular type is faster and memory wiser comparing to the obscured one!</em></strong>
	[Serializable]
	public struct ObscuredVector3Int
	{
		private static int cryptoKey = 120207;
		private static readonly Vector3Int zero = Vector3Int.zero;

#if UNITY_EDITOR
		// For internal Editor usage only (may be useful for drawers).
		public static int cryptoKeyEditor = cryptoKey;
#endif

		[SerializeField]
		private int currentCryptoKey;

		[SerializeField]
		private RawEncryptedVector3Int hiddenValue;

		[SerializeField]
		private bool inited;

		[SerializeField]
		private Vector3Int fakeValue;

		[SerializeField]
		private bool fakeValueActive;

		private ObscuredVector3Int(Vector3Int value)
		{
			currentCryptoKey = cryptoKey;
			hiddenValue = Encrypt(value);

			var detectorRunning = Detectors.ObscuredCheatingDetector.ExistsAndIsRunning;
			fakeValue = detectorRunning ? value : zero;
			fakeValueActive = detectorRunning;

			inited = true;
		}

		/// <summary>
		/// Mimics constructor of regular Vector3Int.
		/// </summary>
		/// <param name="x">X component of the vector</param>
		/// <param name="y">Y component of the vector</param>
		/// <param name="z">Z component of the vector</param>
		public ObscuredVector3Int(int x, int y, int z)
		{
			currentCryptoKey = cryptoKey;
			hiddenValue = Encrypt(x, y, z, currentCryptoKey);

			if (Detectors.ObscuredCheatingDetector.ExistsAndIsRunning)
			{
				fakeValue = new Vector3Int
				{
					x = x,
					y = y,
					z = z
				};
				fakeValueActive = true;
			}
			else
			{
				fakeValue = zero;
				fakeValueActive = false;
			}

			inited = true;
		}

		public int x
		{
			get
			{
				var decrypted = InternalDecryptField(hiddenValue.x);
				if (Detectors.ObscuredCheatingDetector.ExistsAndIsRunning && fakeValueActive && Mathf.Abs(decrypted - fakeValue.x) > 0)
				{
					Detectors.ObscuredCheatingDetector.Instance.OnCheatingDetected();
				}
				return decrypted;
			}

			set
			{
				hiddenValue.x = InternalEncryptField(value);
				if (Detectors.ObscuredCheatingDetector.ExistsAndIsRunning)
				{
					fakeValue.x = value;
					fakeValue.y = InternalDecryptField(hiddenValue.y);
					fakeValue.z = InternalDecryptField(hiddenValue.z);
					fakeValueActive = true;
				}
				else
				{
					fakeValueActive = false;
				}
			}
		}

		public int y
		{
			get
			{
				var decrypted = InternalDecryptField(hiddenValue.y);
				if (Detectors.ObscuredCheatingDetector.ExistsAndIsRunning && fakeValueActive && Mathf.Abs(decrypted - fakeValue.y) > 0)
				{
					Detectors.ObscuredCheatingDetector.Instance.OnCheatingDetected();
				}
				return decrypted;
			}

			set
			{
				hiddenValue.y = InternalEncryptField(value);
				if (Detectors.ObscuredCheatingDetector.ExistsAndIsRunning)
				{
					fakeValue.x = InternalDecryptField(hiddenValue.x);
					fakeValue.y = value;
					fakeValue.z = InternalDecryptField(hiddenValue.z);
					fakeValueActive = true;
				}
				else
				{
					fakeValueActive = false;
				}
			}
		}

		public int z
		{
			get
			{
				var decrypted = InternalDecryptField(hiddenValue.z);
				if (Detectors.ObscuredCheatingDetector.ExistsAndIsRunning && fakeValueActive && Mathf.Abs(decrypted - fakeValue.z) > 0)
				{
					Detectors.ObscuredCheatingDetector.Instance.OnCheatingDetected();
				}
				return decrypted;
			}

			set
			{
				hiddenValue.z = InternalEncryptField(value);
				if (Detectors.ObscuredCheatingDetector.ExistsAndIsRunning)
				{
					fakeValue.x = InternalDecryptField(hiddenValue.x);
					fakeValue.y = InternalDecryptField(hiddenValue.y);
					fakeValue.z = value;
					fakeValueActive = true;
				}
				else
				{
					fakeValueActive = false;
				}
			}
		}

		public int this[int index]
		{
			get
			{
				switch (index)
				{
					case 0:
						return x;
					case 1:
						return y;
					case 2:
						return z;
					default:
						throw new IndexOutOfRangeException("Invalid ObscuredVector3Int index!");
				}
			}
			set
			{
				switch (index)
				{
					case 0:
						x = value;
						break;
					case 1:
						y = value;
						break;
					case 2:
						z = value;
						break;
					default:
						throw new IndexOutOfRangeException("Invalid ObscuredVector3Int index!");
				}
			}
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
		/// Use this simple encryption method to encrypt any Vector3Int value, uses default crypto key.
		/// </summary>
		public static RawEncryptedVector3Int Encrypt(Vector3Int value)
		{
			return Encrypt(value, 0);
		}

		/// <summary>
		/// Use this simple encryption method to encrypt any Vector3Int value, uses passed crypto key.
		/// </summary>
		public static RawEncryptedVector3Int Encrypt(Vector3Int value, int key)
		{
			return Encrypt(value.x, value.y, value.z, key);
		}

		/// <summary>
		/// Use this simple encryption method to encrypt Vector3Int components, uses passed crypto key.
		/// </summary>
		public static RawEncryptedVector3Int Encrypt(int x, int y, int z, int key)
		{
			if (key == 0)
			{
				key = cryptoKey;
			}

			RawEncryptedVector3Int result;
			result.x = ObscuredInt.Encrypt(x, key);
			result.y = ObscuredInt.Encrypt(y, key);
			result.z = ObscuredInt.Encrypt(z, key);

			return result;
		}

		/// <summary>
		/// Use it to decrypt RawEncryptedVector3Int you got from Encrypt(), uses default crypto key.
		/// </summary>
		public static Vector3Int Decrypt(RawEncryptedVector3Int value)
		{
			return Decrypt(value, 0);
		}

		/// <summary>
		/// Use it to decrypt RawEncryptedVector3Int you got from Encrypt(), uses passed crypto key.
		/// </summary>
		public static Vector3Int Decrypt(RawEncryptedVector3Int value, int key)
		{
			if (key == 0)
			{
				key = cryptoKey;
			}

			var result = new Vector3Int
			{
				x = ObscuredInt.Decrypt(value.x, key),
				y = ObscuredInt.Decrypt(value.y, key),
				z = ObscuredInt.Decrypt(value.z, key)
			};

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
		public RawEncryptedVector3Int GetEncrypted()
		{
			ApplyNewCryptoKey();
			return hiddenValue;
		}

		/// <summary>
		/// Allows to explicitly set current obscured value.
		/// </summary>
		/// Use it in conjunction with GetEncrypted().<br/>
		/// Useful for loading data stored in obscured state.
		public void SetEncrypted(RawEncryptedVector3Int encrypted)
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
		public Vector3Int GetDecrypted()
		{
			return InternalDecrypt();
		}

		private Vector3Int InternalDecrypt()
		{
			if (!inited)
			{
				currentCryptoKey = cryptoKey;
				hiddenValue = Encrypt(zero, cryptoKey);
				fakeValue = zero;
				fakeValueActive = false;
				inited = true;

				return zero;
			}

			var value = new Vector3Int
			{
				x = ObscuredInt.Decrypt(hiddenValue.x, currentCryptoKey),
				y = ObscuredInt.Decrypt(hiddenValue.y, currentCryptoKey),
				z = ObscuredInt.Decrypt(hiddenValue.z, currentCryptoKey)
			};


			if (Detectors.ObscuredCheatingDetector.ExistsAndIsRunning && fakeValueActive && value != fakeValue)
			{
				Detectors.ObscuredCheatingDetector.Instance.OnCheatingDetected();
			}

			return value;
		}

		private int InternalDecryptField(int encrypted)
		{
			var key = cryptoKey;

			if (currentCryptoKey != cryptoKey)
			{
				key = currentCryptoKey;
			}

			var result = ObscuredInt.Decrypt(encrypted, key);
			return result;
		}

		private int InternalEncryptField(int encrypted)
		{
			var result = ObscuredInt.Encrypt(encrypted, cryptoKey);
			return result;
		}

		#region operators, overrides, etc.
		//! @cond
		public static implicit operator ObscuredVector3Int(Vector3Int value)
		{
			return new ObscuredVector3Int(value);
		}

		public static implicit operator Vector3Int(ObscuredVector3Int value)
		{
			return value.InternalDecrypt();
		}

		public static implicit operator Vector3(ObscuredVector3Int value)
		{
			return value.InternalDecrypt();
		}

		public static ObscuredVector3Int operator +(ObscuredVector3Int a, ObscuredVector3Int b)
		{
			return a.InternalDecrypt() + b.InternalDecrypt();
		}

		public static ObscuredVector3Int operator +(Vector3Int a, ObscuredVector3Int b)
		{
			return a + b.InternalDecrypt();
		}

		public static ObscuredVector3Int operator +(ObscuredVector3Int a, Vector3Int b)
		{
			return a.InternalDecrypt() + b;
		}

		public static ObscuredVector3Int operator -(ObscuredVector3Int a, ObscuredVector3Int b)
		{
			return a.InternalDecrypt() - b.InternalDecrypt();
		}

		public static ObscuredVector3Int operator -(Vector3Int a, ObscuredVector3Int b)
		{
			return a - b.InternalDecrypt();
		}

		public static ObscuredVector3Int operator -(ObscuredVector3Int a, Vector3Int b)
		{
			return a.InternalDecrypt() - b;
		}

		public static ObscuredVector3Int operator *(ObscuredVector3Int a, int d)
		{
			return a.InternalDecrypt() * d;
		}

		public static bool operator ==(ObscuredVector3Int lhs, ObscuredVector3Int rhs)
		{
			return lhs.InternalDecrypt() == rhs.InternalDecrypt();
		}

		public static bool operator ==(Vector3Int lhs, ObscuredVector3Int rhs)
		{
			return lhs == rhs.InternalDecrypt();
		}

		public static bool operator ==(ObscuredVector3Int lhs, Vector3Int rhs)
		{
			return lhs.InternalDecrypt() == rhs;
		}

		public static bool operator !=(ObscuredVector3Int lhs, ObscuredVector3Int rhs)
		{
			return lhs.InternalDecrypt() != rhs.InternalDecrypt();
		}

		public static bool operator !=(Vector3Int lhs, ObscuredVector3Int rhs)
		{
			return lhs != rhs.InternalDecrypt();
		}

		public static bool operator !=(ObscuredVector3Int lhs, Vector3Int rhs)
		{
			return lhs.InternalDecrypt() != rhs;
		}

		public override bool Equals(object other)
		{
			return InternalDecrypt().Equals(other);
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
		/// Returns a nicely formatted string for this vector.
		/// </summary>
		public override string ToString()
		{
			return InternalDecrypt().ToString();
		}

		/// <summary>
		/// Returns a nicely formatted string for this vector.
		/// </summary>
		public string ToString(string format)
		{
			return InternalDecrypt().ToString(format);
		}

		//! @endcond
		#endregion

		/// <summary>
		/// Used to store encrypted Vector3Int.
		/// </summary>
		[Serializable]
		public struct RawEncryptedVector3Int
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
		}
	}
}
#endif