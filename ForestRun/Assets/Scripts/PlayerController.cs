using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour {

    public float ForwardSpeed = 1;
    public float SideSpeed = 1;
    public float JumpForce = 1;
    public float RunAnimationSpeed = 3f;

    private bool Grounded = true;
    private bool Dead = false;

    private GameObject Score;
    private GameObject DeathPanel;
    private GameObject Dog;

    private float sideMovement = 0.0f;
    
	void Start () {
        Score = GameObject.Find("Score");

        DeathPanel = GameObject.Find("DeathPanel");
        DeathPanel.SetActive(false);

        Dog = GameObject.Find("Dog");
        Dog.GetComponent<Animation>()["Running"].speed = RunAnimationSpeed;
    }

    void Die() {
        Dead = true;
        DeathPanel.SetActive(true);
        Dog.GetComponent<Animation>().Stop();
    }

    void Restart() {
        Dead = false;
        DeathPanel.SetActive(false);
        Dog.GetComponent<Animation>().Play();
        transform.position = new Vector3(0, 0, -10f);
    }

    void OnCollisionEnter(Collision collision) {
        if(collision.collider.tag == "Ground") {
            Grounded = true;
        } else {
            Die();
        }
    }

	void FixedUpdate () {
        if (Dead) {
            if (Input.GetButtonDown("Restart")) {
                Restart();
            } else {
                return;
            }
        }

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

        float finalSideMovement = sideMovement;

        if (!Grounded){
            finalSideMovement *= .5f;
        }

        transform.position = new Vector3(
            transform.position.x + (axisValue * SideSpeed * Time.deltaTime), 
            transform.position.y, 
            transform.position.z + (ForwardSpeed * Time.deltaTime)
        );

        //transform.rotation = Quaternion.Euler(new Vector3(0, finalSideMovement * SideSpeed, -finalSideMovement * SideSpeed));

        if (Grounded && Input.GetButtonDown("Jump")) {
            Grounded = false;
            GetComponent<Rigidbody>().AddForce(new Vector3(0, JumpForce, 0));
        }

        Score.GetComponent<Text>().text = ((int)transform.position.z).ToString();
	}
}
