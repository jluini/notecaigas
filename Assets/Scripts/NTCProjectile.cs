using UnityEngine;
using System.Collections;

namespace NoTeCaigas
{
    public class NTCProjectile : MonoBehaviour
    {
        public Vector3 velocity;

        void Update()
        {
            Vector3 pos = transform.position;

            pos += velocity * Time.deltaTime;

            transform.position = pos;
        }
        
    }
}

