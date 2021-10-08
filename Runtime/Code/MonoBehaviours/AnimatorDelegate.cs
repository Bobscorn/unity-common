using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Common
{
	namespace Behaviour
	{
		public class AnimatorDelegate : MonoBehaviour
		{
			public Animator Animator;

			private int _intValueToSet;
			private float _floatValueToSet;

			public void SetTrue(string parameter)
			{
				Animator.SetBool(parameter, true);
			}

			public void SetTrue(int parameterID)
			{
				Animator.SetBool(parameterID, true);
			}


			public void SetFalse(string parameter)
			{
				Animator.SetBool(parameter, false);
			}

			public void SetFalse(int parameterID)
			{
				Animator.SetBool(parameterID, false);
			}


			public void SetValueToSet(int value)
			{
				_intValueToSet = value;
			}

			public void SetValueToSet(float value)
			{
				_floatValueToSet = value;
			}

			public void SetIntParameter(string parameter)
			{
				Animator.SetInteger(parameter, _intValueToSet);
			}

			public void SetIntParameter(int parameterID)
			{
				Animator.SetInteger(parameterID, _intValueToSet);
			}

			public void SetFloatParameter(string parameter)
			{
				Animator.SetFloat(parameter, _floatValueToSet);
			}

			public void SetFloatParameter(int parameterID)
			{
				Animator.SetFloat(parameterID, _floatValueToSet);
			}
		}  
	}
}
