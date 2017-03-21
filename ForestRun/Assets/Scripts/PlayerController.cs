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
    private bool FreezeBool = false;

    private GameObject Score;
    private GameObject DeathPanel;
    private GameObject Dog;

    private float sideMovement = 0.0f;
    private int points = 0;

    //vars for enchantedMovement
    public bool enchantedMovement = true;
    public float maxRotation = 90f;
    private float rotationSpeed = 2.5f;

    Vector3 zeroAc;
    Vector3 curAc;

    void Start () {
        Score = GameObject.Find("Score");

        DeathPanel = GameObject.Find("DeathPanel");
        DeathPanel.SetActive(false);

        Dog = GameObject.Find("Dog");
        Dog.GetComponent<Animation>()["Running"].speed = RunAnimationSpeed;

        zeroAc = Input.acceleration;
        curAc = Vector3.zero;
    }

    public void addPoints(int amount) {
        points += amount;
    }

    public void setPoints(int newPoints) {
        points = newPoints;
    }

    public int getPoints() {
        return points;
    }

    public void Freeze() {
        FreezeBool = true;
        Dog.GetComponent<Animation>().Stop();
    }

    public void Unfreeze() {
        FreezeBool = false;
        Dog.GetComponent<Animation>().Play();
    }

    void Die() {
        Dead = true;
        DeathPanel.SetActive(true);
        Freeze();
    }

    public void Restart() {
        Dead = false;
        DeathPanel.SetActive(false);
        Unfreeze();
        setPoints(0);
        //((LevelController)GameObject.Find("LevelCreator").GetComponent("LevelController")).RestartCurrentLevel();
    }

    void OnCollisionEnter(Collision collision) {
        if(collision.collider.tag == "Ground") {
            if (GetComponent<Rigidbody>().velocity.y <= 0) {
                Grounded = true;
            }
        } else if(collision.collider.tag == "Item") {
            Destroy(collision.collider.gameObject);
            addPoints(1);
        } else {
            Die();
        }
    }

    private void Update() {
        if (Dead) {
            if (Input.GetButtonDown("Restart")) {
                Restart();
            }
        }
        if (FreezeBool) {
            return;
        }
#if UNITY_IPHONE || UNITY_ANDROID
        foreach (var touch in Input.touches) {
            if (touch.phase == TouchPhase.Ended && Grounded) {

                GetComponent<Rigidbody>().AddForce(new Vector3(0, JumpForce, 0), ForceMode.Impulse);
                Grounded = false;
            }
        }
#else
        if (Grounded && Input.GetButtonDown("Jump")) {

            GetComponent<Rigidbody>().AddForce(new Vector3(0, JumpForce, 0), ForceMode.Impulse);
            Grounded = false;
        }
#endif
    }

    void FixedUpdate () {
        if (FreezeBool) {
            return;
        }

#if UNITY_IPHONE || UNITY_ANDROID
        curAc = Vector3.Lerp(curAc, Input.acceleration-zeroAc, Time.fixedDeltaTime);
        float axisValue = Mathf.Clamp(curAc.x * 100, -maxRotation, maxRotation);
        
        Vector3 eulerAngles = transform.eulerAngles;

        eulerAngles.y = axisValue;
        
        transform.eulerAngles = eulerAngles;
        transform.position += transform.forward * ForwardSpeed * Time.fixedDeltaTime;
#else
        float axisValue = Input.GetAxis("Horizontal");

        if (enchantedMovement)
        {
            float runningAngleAddition = 0f;
            if (axisValue <= -1f)
            {
                //left
                runningAngleAddition = -rotationSpeed;
            } else if (axisValue >= 1f)
            {
                //right
                runningAngleAddition = rotationSpeed;
            }

            if (!Grounded)
            {
                runningAngleAddition *= .5f;
            }            

            Vector3 eulerAngles = transform.eulerAngles;

            eulerAngles.y += runningAngleAddition;

            if (eulerAngles.y > maxRotation && eulerAngles.y <= 180f)
            {
                eulerAngles.y = maxRotation;
            } else if (eulerAngles.y < (360f - maxRotation) && eulerAngles.y > 180f)
            {
                eulerAngles.y = 360 - maxRotation;
            }
            transform.eulerAngles = eulerAngles;

            //transform.rotation = Quaternion.Euler(new Vector3(0, finalSideMovement * SideSpeed * 3f, 0));//-finalSideMovement * SideSpeed

            transform.position += transform.forward * ForwardSpeed * Time.fixedDeltaTime;
        }
        else
        {
            transform.position = new Vector3(
                transform.position.x + (axisValue * SideSpeed * Time.fixedDeltaTime),
                transform.position.y,
                transform.position.z + (ForwardSpeed * Time.fixedDeltaTime)
            );
        }
#endif

        Score.GetComponent<Text>().text = (points).ToString();
	}
}
