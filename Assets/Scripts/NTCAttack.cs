using UnityEngine;

using System.Collections;

namespace NoTeCaigas
{
    public class NTCAttack
    {
        Vector3 force;
        
        public NTCAttack(Vector3 force)
        {
            this.force = force;
        }
        
        public Vector3 GetForce()
        {
            return force;
        }
    }
}

