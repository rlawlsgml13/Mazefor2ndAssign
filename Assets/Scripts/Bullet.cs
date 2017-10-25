using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour {
	
	
	
	private static readonly float bulletMoveSpeed = 10.0f;
	
	public GameObject hitEffectPrefab = null;
	
	/*
	 *	프레임마다 호출되는 함수(Called function by each frame)
	 */
	private	void Update() {
		
		//이동(Movement)
		{
			//1초 동안 이동량(Movement for a second)
			Vector3 vecAddPos = (Vector3.forward * bulletMoveSpeed);
            /*
				Vector3.forward 는 new Vector3( 0f, 0f, 1f) 와 같다 (Vector3.forward is the same with new Vector3( 0f, 0f, 1f))

				Vector3에 transform.rotation을 곱하면 그 방향으로 꺾어진다(when multiplying Vector3 and transform.rotation, direction is changed)
				이 때 Vector3는 Z+ 방향을 정면으로 여긴다(Vector considers that Z+ direction is the front)
			 */

            //이동량과 회전량에는 Time.deltaTime을 곱해서 실행환경 (프레임률)에 따른 차이를 해결한다(solve differnt environment of many kinds of devices by multipying Movement and Rotation)
            transform.position += ((transform.rotation * vecAddPos) * Time.deltaTime);
		}
	}
	
	
	
	/*
	 *	Collider가 어떤 물체에 닿으면 호출되는 함수(The called funtion when Collider touch something) 
	 *
	 *	자신의 GameObject가 Collider(IsTrigger를 ON으로 하여)와 Rigidbody 적용하면 호출 가능한 상태가 된다(If your GameObject apply Collider(IsTrigger is ON) and Rigidbody, function can be called)
	 */
	private	void OnTriggerEnter( Collider hitCollider) {
		
		//히트(닿았을 때) 효과가 있는 지 검사(When hiiting, confirm whether effect is functioned or not)
		if(null != hitEffectPrefab) {
			//자신의 위치에서 히트 효과를 연출(In your location, hitting effect is performed)
			Instantiate( hitEffectPrefab, transform.position, transform.rotation);
		}
		
		//이 GameObject를 Hierarchy에서 삭제(Delete this GameObecjt in Hierarchy)
		Destroy(gameObject);
	}
	
	
	
	
}
