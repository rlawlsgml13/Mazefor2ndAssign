using UnityEngine;
using System.Collections;




/*
 *	プレーヤークラス
 *	Maruchu
 *
 *	キャラクターの移動、メカニム(モーション)の制御など
 */
public		class		Player				: MonoBehaviour {
	
	
	
	public GameObject playerObject = null;		//動かす対象のモデル
	public GameObject bulletObject = null;		//弾プレハブ
	
	public Transform bulletStartPosition = null;		//弾の発射位置を取得するボーン
	
	
	
	private static readonly float MOVE_Z_FRONT = 5.0f;	//前進の速度
	private	static readonly	float MOVE_Z_BACK = -2.0f;	//後退の速度
	
	private	static readonly	float ROTATION_Y_KEY = 360.0f;	//回転の速度(キーボード)
	private	static readonly	float ROTATION_Y_MOUSE = 720.0f;	//回転の速度(マウス)
	
	private float m_rotationY = 0.0f;		//プレーヤーの回転角度
	
	private	bool m_mouseLockFlag = true;		//マウスを固定する機能
	
	
	
	
	/*
	 *	毎フレーム呼び出される関数
	 */
	private		void	Update() {
		
		//ステージクリアしていたら操作を無視
		if( Game.IsStageCleared()) {
			return;
		}
		
		//マウスロック処理
		CheckMouseLock();
		
		//移動処理
		CheckMove();
	}
	
	
	/*
	 *	マウスロック処理のチェック
	 */
	private		void	CheckMouseLock() {
		
		//Escキーをおした時の動作
		if( Input.GetKeyDown( KeyCode.Escape)) {
			//フラグをひっくり返す
			m_mouseLockFlag	= !m_mouseLockFlag;
		}
		
		//マウスロックされてる？
		if( m_mouseLockFlag) {
			//ロックしていたらロック解除
			Screen.lockCursor	= true;
			Cursor.visible	= false;
		} else {
			//ロック解除されていたらロック
			Screen.lockCursor	= false;
			Cursor.visible	= true;
		}
	}
	/*
 *	이동 처리 검사(Movement)
 */
	private		void	CheckMove() {
		
		//회전(Rotation)
		{
			// 이 프레임에서 움직이는 회전량(Moving rotation in frame)
			float	addRotationY	= 0.0f;

            //키 조작에 의한 회전(Rotation by command)
			if( Input.GetKey( KeyCode.A )) {
				addRotationY = -ROTATION_Y_KEY;
			} else
			if( Input.GetKey( KeyCode.D)) {
				addRotationY = ROTATION_Y_KEY;
            } else
            if (Input.GetKey(KeyCode.LeftArrow))
            {
                addRotationY = -ROTATION_Y_KEY;
            } else
            if (Input.GetKey(KeyCode.RightArrow))
            {
                addRotationY = ROTATION_Y_KEY;
            }
            

            //마우스 이동량에 의한 회전(Rotation by Mouse)
            if ( m_mouseLockFlag) {
				//이동량을 얻어서 각도를 넘겨준다(Add degree to Movement)
				addRotationY += (Input.GetAxis("Mouse X") * ROTATION_Y_MOUSE);
			}
			
			//현재 각도에 더한다(Add to present degree)
			m_rotationY += (addRotationY * Time.deltaTime);		//이동량, 회전량에는 Time.deltaTime을 곱해서 기계 차이에 의한 프레임을 없애준다(Make Frame equally between different devices).
			
			//오일러 각으로 입력한다(Enter Euler degree)
			transform.rotation	= Quaternion.Euler( 0, m_rotationY, 0);		//Y축 회전으로 캐릭터 방향을 옆으로 바꾼다(Change character's direction in Y).
		}
		
		//이동(Movement)
		Vector3	addPosition	= Vector3.zero;		//이동량(z값을 메카님에도 넘겨준다) (Send Z to Mecanim)
		{
			
			
			//키 조작에서 이동할 양을 얻는다(Get movement under control by command)
			Vector3	vecInput = new Vector3(0f, 0, Input.GetAxisRaw("Vertical"));		//Z에 앞 뒤 방향을 입력한다(Enter Front or Behind in Z)
			
			//Z에 값이 입력되어 있는 지 확인한다(Confrim whetehr z is applied or not)
			if( vecInput.z > 0) {
				//전진(Go to the front)
				addPosition.z = MOVE_Z_FRONT;
			} else
			if( vecInput.z < 0) {
				//후퇴(Go in the back)
				addPosition.z = MOVE_Z_BACK;
			}
			
			// 이동량을 Transform에 넘겨주어 이동시킨다(To move, send movement to Transform) 
			transform.position	+= ((transform.rotation	* addPosition) * Time.deltaTime);
			/*
				Vector3 형식으로 transform,rotation을 곱하면 그 방향으로 꺽어진다(multiply Vector3 and transform.rotation to change direction)
				이 때 Vector3는 Z+ 방향을 정면으로 여긴다(The direction of Vector3 is Z+(the front))
			 */
		}
		
		//사격(Shooting)
		bool shootFlag;
		{
            //사격 버튼(클릭)을 눌렀는지 확인(Check Shooting Button) 
            if (Input.GetButtonDown("Fire1")) {
				//사격처리(Shooting process)
				shootFlag = true;
				
				//총알을 발사할 위치가 지정되어 있는 지 검사(Check the location that bullet is shot)
				if(null!=bulletStartPosition) {
					
                    //총알을 생성할 위치 지정(Decide that bullet will be created)
					Vector3 vecBulletPos = bulletStartPosition.position;
					//전진하는 방향으로 조금 진행(Fire)
					vecBulletPos += (transform.rotation	*Vector3.forward);
					//Y 높이를 적당히 올린다(Y height increase)
					vecBulletPos.y = 1.0f;
					
					//총알을 생성한다(Create bullet)
					Instantiate(bulletObject, vecBulletPos, transform.rotation);
				}
			} else {
				//발사하지 않는다(Not fire)
				shootFlag = false;
			}
		}
		
		
		//メカニム(モーション)
		{
			//アニメーターを取得
			Animator	animator	= playerObject.GetComponent<Animator>();
			
			//Animatorで設定した値を渡す
			animator.SetFloat(	"SpeedZ",	addPosition.z);	//Z(前後の移動量)
			animator.SetBool(	"Shoot",	shootFlag);		//射撃フラグ
		}
	}
	
	
	
	
}
