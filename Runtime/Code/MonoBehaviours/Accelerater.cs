using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Common.Math;


namespace Common
{
    namespace Behaviour
    {
        public class Accelerater : MonoBehaviour
        {
            private Vector3 _gravity;

            // Start is called before the first frame update
            void Start()
            {
                _gravity = 9.8f * Vector3.down;
            }

            // Update is called once per frame
            void Update()
            {
                Vector3 accel = new Vector3(-Input.acceleration.x, -Input.acceleration.y);
                transform.up = Vector3.Lerp(transform.up, accel.normalized, Time.deltaTime * 5f);

                _gravity = -9.8f * transform.up * MathMap.Map(accel.magnitude, 0f, 10f, 0.7f, 2f);
                Physics.gravity = _gravity;
            }

            private void OnDisable()
            {
                Physics.gravity = new Vector3(0f, -9.8f, 0f);
            }
        }
    }
}