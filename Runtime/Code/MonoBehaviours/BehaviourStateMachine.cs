using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Common
{
	namespace Behaviour
	{
		public interface IEnumLookupable<TEnum>
		{
			public TEnum Enum { get; set; }
			public MonoBehaviour Behaviour { get; set; }
		}

		/// <summary>
		/// Base class for quick MonoBehaviour based state machine
		/// 
		/// Derive this class and create an Enum type to hold your state values
		/// </summary>
		/// <typeparam name="TEnum"></typeparam>
		public class BehaviourStateMachine<TEnum, TEnumLookup> : MonoBehaviour, ISerializationCallbackReceiver where TEnum : System.Enum where TEnumLookup : struct, IEnumLookupable<TEnum>
		{
			public TEnum State { get => _curState; set => SwitchToState(value); }

			[SerializeField]
			protected List<TEnumLookup> _enumToStates = new List<TEnumLookup>();

			[SerializeField]
			private MonoBehaviour _curStateBehaviour;
			[SerializeField]
			private TEnum _curState;

			protected Dictionary<TEnum, MonoBehaviour> _enumLookup = new Dictionary<TEnum, MonoBehaviour>();

			void ISerializationCallbackReceiver.OnBeforeSerialize()
			{
				_enumToStates.Clear();
				_enumToStates.Capacity = _enumLookup.Count;

				foreach (var pair in _enumLookup)
				{
					var lookup = new TEnumLookup { Enum = pair.Key, Behaviour = pair.Value };
					_enumToStates.Add(lookup);
				}
			}

			void ISerializationCallbackReceiver.OnAfterDeserialize()
			{
				_enumLookup.Clear();
				
				for (int i = 0; i < _enumToStates.Count; ++i)
				{
					var enumState = _enumToStates[i];
					if (_enumLookup.ContainsKey(enumState.Enum) && !_enumLookup.ContainsKey(default))
						_enumLookup[default] = enumState.Behaviour;
					else
						_enumLookup[enumState.Enum] = enumState.Behaviour;
				}
			}

			void Start()
			{
				foreach (var pair in _enumLookup)
				{
					if (pair.Value)
						pair.Value.enabled = false;
				}
			}

#if UNITY_EDITOR
			void OnValidate()
			{
				foreach (var pair in _enumLookup)
				{
					if (pair.Value)
						pair.Value.enabled = false;
				}
				if (_enumLookup.TryGetValue(_curState, out MonoBehaviour behaviour))
				{
					if (behaviour)
						behaviour.enabled = true;
				}
			}
#endif

			/// <summary>
			/// Implement a non-generic wrapper function in sub class to use in UnityEvent callbacks
			/// 
			/// public void ExampleSetState(MyStateType state) => base.SwitchToState(state);
			/// </summary>
			public void SwitchToState(TEnum state)
			{
				if (_enumLookup.TryGetValue(state, out MonoBehaviour stateBehaviour))
				{
					_curState = state;
					if (_curStateBehaviour)
						_curStateBehaviour.enabled = false;

					_curStateBehaviour = stateBehaviour;
					if (stateBehaviour)
						stateBehaviour.enabled = true;
					return;
				}

				throw new System.ArgumentException($"Could not find {state} in state Lookup");
			}
		}  
	}
}
