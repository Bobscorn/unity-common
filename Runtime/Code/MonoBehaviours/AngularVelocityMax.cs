using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AngularVelocityMax : MonoBehaviour
{
	public Rigidbody Body;

	public float MaxAngularVelocity { get => _maxAngularVelocity; set => Body.maxAngularVelocity = _maxAngularVelocity = value; }

	[SerializeField]
    private float _maxAngularVelocity;

	private void OnEnable()
	{
		Body.maxAngularVelocity = _maxAngularVelocity;
	}

	private void OnDisable()
	{
		Body.maxAngularVelocity = 7f;
	}

#if UNITY_EDITOR
	private void OnValidate()
	{
		if (Application.isPlaying)
			Body.maxAngularVelocity = _maxAngularVelocity;
	}
#endif
}
