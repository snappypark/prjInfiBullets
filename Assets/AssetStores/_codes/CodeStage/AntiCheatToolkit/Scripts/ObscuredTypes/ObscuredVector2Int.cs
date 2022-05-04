#if UNITY_2017_2_OR_NEWER
using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace CodeStage.AntiCheat.ObscuredTypes
{
	/// <summary>
	/// Use it instead of regular <c>Vector2Int</c> for any cheating-sensitive variables.
	/// </summary>
	/// <strong>\htmlonly<font color="FF4040">WARNING:</font>\endhtmlonly Doesn't mimic regular type API, thus should be used with extra caution.</strong> Cast it to regular, not obscured type to work with regular APIs.<br/>
	/// <strong><em>Regular type is faster and memory wiser comparing to the obscured one!</em></strong>
	[Serializable]
	public struct ObscuredVector2Int
	{
		private static int cryptoKey = 160122;
		private static readonly Vector2Int zero = Vector2Int.zero;

#if UNITY_EDITOR
		// For internal Editor usage only (may be useful for drawers).
		public static int cryptoKeyEditor = cryptoKey;
#endif

		[SerializeField]
		private int currentCryptoKey;

		[SerializeField]
		private RawEncryptedVector2Int hiddenValue;

		[SerializeField]
		private bool inited;

		[SerializeField]
		private Vector2Int fakeValue;

		[SerializeField]
		private bool fakeValueActive;

		private ObscuredVector2Int(Vector2Int value)
		{
			currentCryptoKey = cryptoKey;
			hiddenValue = Encrypt(value);

			var detectorRunning = Detectors.ObscuredCheatingDetector.ExistsAndIsRunning;
			fakeValue = detectorRunning ? value : zero;
			fakeValueActive = detectorRunning;

			inited = true;
		}

		/// <summary>
		/// Mimics constructor of regular Vector2Int.
		/// </summary>
		/// <param name="x">X component of the vector</param>
		/// <param name="y">Y component of the vector</param>
		public ObscuredVector2Int(int x, int y)
		{
			currentCryptoKey = cryptoKey;
			hiddenValue = Encrypt(x, y, currentCryptoKey);

			if (Detectors.ObscuredCheatingDetector.ExistsAndIsRunning)
			{
				fakeValue = new Vector2Int(x, y);
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
					default:
						throw new IndexOutOfRangeException("Invalid ObscuredVector2Int index!");
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
					default:
						throw new IndexOutOfRangeException("Invalid ObscuredVector2Int index!");
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
		/// Use this simple encryption method to encrypt any Vector2Int value, uses default crypto key.
		/// </summary>
		public static RawEncryptedVector2Int Encrypt(Vector2Int value)
		{
			return Encrypt(value, 0);
		}

		/// <summary>
		/// Use this simple encryption method to encrypt any Vector2Int value, uses passed crypto key.
		/// </summary>
		public static RawEncryptedVector2Int Encrypt(Vector2Int value, int key)
		{
			return Encrypt(value.x, value.y, key);
		}

		/// <summary>
		/// Use this simple encryption method to encrypt Vector2Int components, uses passed crypto key.
		/// </summary>
		public static RawEncryptedVector2Int Encrypt(int x, int y, int key)
		{
			if (key == 0)
			{
				key = cryptoKey;
			}

			RawEncryptedVector2Int result;
			result.x = ObscuredInt.Encrypt(x, key);
			result.y = ObscuredInt.Encrypt(y, key);

			return result;
		}

		/// <summary>
		/// Use it to decrypt RawEncryptedVector2Int you got from Encrypt(), uses default crypto key.
		/// </summary>
		public static Vector2Int Decrypt(RawEncryptedVector2Int value)
		{
			return Decrypt(value, 0);
		}

		/// <summary>
		/// Use it to decrypt RawEncryptedVector2Int you got from Encrypt(), uses passed crypto key.
		/// </summary>
		public static Vector2Int Decrypt(RawEncryptedVector2Int value, int key)
		{
			if (key == 0)
			{
				key = cryptoKey;
			}

			var result = new Vector2Int
			{
				x = ObscuredInt.Decrypt(value.x, key),
				y = ObscuredInt.Decrypt(value.y, key)
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
		public RawEncryptedVector2Int GetEncrypted()
		{
			ApplyNewCryptoKey();
			return hiddenValue;
		}

		/// <summary>
		/// Allows to explicitly set current obscured value.
		/// </summary>
		/// Use it in conjunction with GetEncrypted().<br/>
		/// Useful for loading data stored in obscured state.
		public void SetEncrypted(RawEncryptedVector2Int encrypted)
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
		public Vector2Int GetDecrypted()
		{
			return InternalDecrypt();
		}

		private Vector2Int InternalDecrypt()
		{
			if (!inited)
			{
				currentCryptoKey = cryptoKey;
				hiddenValue = Encrypt(zero);
				fakeValue = zero;
				fakeValueActive = false;
				inited = true;

				return zero;
			}

			var value = new Vector2Int
			{
				x = ObscuredInt.Decrypt(hiddenValue.x, currentCryptoKey),
				y = ObscuredInt.Decrypt(hiddenValue.y, currentCryptoKey)
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

#region operators, overrides, interface implementations
		//! @cond
		public static implicit operator ObscuredVector2Int(Vector2Int value)
		{
			return new ObscuredVector2Int(value);
		}

		public static implicit operator Vector2Int(ObscuredVector2Int value)
		{
			return value.InternalDecrypt();
		}

		public static implicit operator Vector2(ObscuredVector2Int value)
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
		/// Returns a nicely formatted string for this vector.
		/// </summary>
		public override string ToString()
		{
			return InternalDecrypt().ToString();
		}

		//! @endcond
		#endregion

		/// <summary>
		/// Used to store encrypted Vector2.
		/// </summary>
		[Serializable]
		public struct RawEncryptedVector2Int
		{
			/// <summary>
			/// Encrypted value
			/// </summary>
			public int x;

			/// <summary>
			/// Encrypted value
			/// </summary>
			public int y;
		}
	
	}
}
#endif