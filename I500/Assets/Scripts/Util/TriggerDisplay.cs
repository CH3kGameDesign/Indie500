using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerDisplay : MonoBehaviour {

	public Color C_color = new Color(1, 0, 0, 0.4f);
	public bool b_pointDirection = false;
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
		if (b_pointDirection)
		{
			Gizmos.DrawCube(new Vector3(0,0,0.25f), new Vector3(0.1f,0.1f,0.1f));
			Gizmos.DrawWireCube(new Vector3(0,0,0.25f), new Vector3(0.1f,0.1f,0.1f));
			Gizmos.DrawCube(new Vector3(0,0,0.15f), new Vector3(0.3f,0.1f,0.1f));
			Gizmos.DrawWireCube(new Vector3(0,0,0.15f), new Vector3(0.3f,0.1f,0.1f));
		}
	}
}
