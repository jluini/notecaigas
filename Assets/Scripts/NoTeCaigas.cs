using UnityEngine;
using System.Collections;

using JuloUtil;

namespace NoTeCaigas {
	
	public interface ForceWeighting {
		Vector2 weight(Transform obj);
	}
	
	public class DistanceWeighting : ForceWeighting {
		Vector2 referencePosition;
		float minimumDistance;
		float maximumDistance;
		
		float minPow;
		float power;
		
		public DistanceWeighting(Vector2 referencePosition, float minimumDistance, float maximumDistance, float power = 1f) {
			this.referencePosition = referencePosition;
			this.minimumDistance = minimumDistance;
			this.maximumDistance = maximumDistance;
			
			this.power = power;
			this.minPow = Mathf.Pow(minimumDistance, power);
		}
		
		public Vector2 weight(Transform obj) {
			Vector2 objPosition = (Vector2) obj.position;
			Vector2 difference = objPosition - referencePosition;
			
			float dist = difference.magnitude;
			
			float factor;
			if(dist <= minimumDistance) {
				factor = 1f;
			} else if(dist >= maximumDistance) {
				factor = 0f;
			} else {
				factor = minPow / Mathf.Pow(dist, power);
			}
			
			Vector2 direction = difference.normalized;
			
			return direction * factor;
		}
	}
	
	
}
