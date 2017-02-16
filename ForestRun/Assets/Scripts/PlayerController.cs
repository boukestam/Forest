using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    public float ForwardSpeed = 1;
    public float SideSpeed = 1;
    public float JumpForce = 1;

    private bool Grounded = true;
    private float sideMovement = 0.0f;
    
	void Start () {
        
	}

    void OnCollisionEnter(Collision collision) {
        if(collision.collider.tag == "Ground") {
            Grounded = true;
        }
    }
	
	void FixedUpdate () {
        float axisValue = Input.GetAxis("Horizontal");
        sideMovement += axisValue * 0.1f;
        sideMovement = sideMovement < -1.0f ? -1.0f : sideMovement > 1.0f ? 1.0f : sideMovement;
        if (axisValue == 0.0f)
        {
            sideMovement *= 0.9f;
            if (sideMovement < 0.001f && sideMovement > -0.001f)
            {
                sideMovement = 0.0f;
            }
        }

        if (!Grounded)
        {
            sideMovement *= .5f;
        }

        transform.position = new Vector3(
            transform.position.x + (sideMovement * SideSpeed * Time.deltaTime), 
            transform.position.y, 
            transform.position.z + (ForwardSpeed * Time.deltaTime)
        );

        transform.rotation = Quaternion.Euler(new Vector3(0, 0, -sideMovement * SideSpeed));

        if (Grounded && Input.GetButtonDown("Jump")) {
            Grounded = false;
            GetComponent<Rigidbody>().AddForce(new Vector3(0, JumpForce, 0));
        }
	}
}
