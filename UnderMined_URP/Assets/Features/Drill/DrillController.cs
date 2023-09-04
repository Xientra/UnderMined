using UnityEngine;
using UnityEngine.Events;


public class DrillController : MonoBehaviour
{
	public bool isRunning = false;

	public float timeRemaining = 30f;

	public float coalToTimeRatio = 10f;

	public float speed = 2f;
	public float acceleration = 0.05f;
	public float steerSpeed = 25f;
	
	public Vector3 moveDirection = new Vector3(0, 0, 1);
	
	public UnityEvent die;

	public void StartMoving() {
		isRunning = true;
	}

	private void Update()
	{

		if (isRunning == false)
			return;
				
		Move();

		timeRemaining -= Time.deltaTime;
		if (timeRemaining <= 0)
			OnDie();
	}

	private void Move() {
		transform.position += moveDirection * (speed * Time.deltaTime);

		speed += acceleration * Time.deltaTime;
	}
	
	public void AddCoal(int amount) {
		timeRemaining += amount * coalToTimeRatio;
	}
	
	
	private void OnDie() {
		die.Invoke();
	}
	
	public void Steer(float dir) {
		// dir is -1 to 1
		// rotate direction vector by dir*steerSpeed
	}
	
	public void SteerDirection(Vector3 newDirection) {
		// slowly interpolate to newDirection
	}
}
