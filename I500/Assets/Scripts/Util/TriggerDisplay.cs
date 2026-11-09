using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerDisplay : MonoBehaviour {

	public Color C_color = new Color(1,0,0,0.4f);
	// Magic Editor Stuff
	void OnDrawGizmos()
	{
		Gizmos.matrix = Matrix4x4.TRS(this.transform.position, this.transform.rotation, this.transform.lossyScale);
		Gizmos.color = C_color;
		BoxCollider _box;
		if (this.TryGetComponent<BoxCollider>(out _box))
		{
			Gizmos.DrawCube(_box.center, _box.size);
			Gizmos.DrawWireCube(_box.center, _box.size);
		}
		SphereCollider _sphere;
		if (this.TryGetComponent<SphereCollider>(out _sphere))
		{
			Gizmos.DrawSphere(_sphere.center, _sphere.radius);
			Gizmos.DrawWireSphere(_sphere.center, _sphere.radius);
		}
	}
}
