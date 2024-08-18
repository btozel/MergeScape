using System;
using UnityEngine;

[Obsolete("No longer used. Delete it.")]
public class MainCreatureLocomotion : MonoBehaviour
{

    public float speed = 1.25f;

	void Update () {
		Vector3 pos = transform.position;

		if (Input.GetKey (KeyCode.UpArrow)) {
			pos.z += speed * Time.deltaTime;
		}
		if (Input.GetKey (KeyCode.DownArrow)) {
			pos.z -= speed * Time.deltaTime;
		}
		if (Input.GetKey (KeyCode.RightArrow)) {
			pos.x += speed * Time.deltaTime;
		}
		if (Input.GetKey (KeyCode.LeftArrow)) {
			pos.x -= speed * Time.deltaTime;
		}
			
		transform.position = pos;
	}
}
