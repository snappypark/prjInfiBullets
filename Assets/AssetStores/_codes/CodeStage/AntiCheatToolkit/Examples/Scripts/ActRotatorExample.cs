using UnityEngine;

namespace CodeStage.AntiCheat.Examples
{
	// dummy code, just to add some rotation to the cube from example scene
	[AddComponentMenu("")]
	public class ActRotatorExample : MonoBehaviour
	{
		[Range(1f, 100f)]
		public float speed = 5f;

		private void Update()
		{
			transform.Rotate(0, speed*Time.deltaTime, 0);
		}
	}
}