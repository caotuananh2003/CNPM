using UnityEngine;
using System.Collections;

[RequireComponent (typeof (Controller2D))]
public class Player : MonoBehaviour {

    public int _Index = 0;//Lưu trữ chỉ số của player
    public bool _IsFile = false;//Lưu trạng thái player đã bắn chưa 
    public bool _IsCurrent = false;//Luu lại đối tượng đang được chọn
    public int _Health = 100;//Máu của Player
    public bool _ISShield = false;//đang được tạo vòng bảo vệ
    public MovePlayer _MovePlayer;//Trạng thái di chuyển hiện tại
    public GameObject _Moving_Dust;//đối tượng sinh ra khi di chuyển
    public bool _IsPoison = false;//trạng thái trúng độc

	public float jumpHeight = 4;
	public float timeToJumpApex = .4f;
	float accelerationTimeAirborne = .2f;
	float accelerationTimeGrounded = .1f;
	public float moveSpeed = 6;

	float gravity;
	float jumpVelocity;
	Vector3 velocity;
	float velocityXSmoothing;
    float _dame = 0;//Dame khi player va chạm với địch
    public bool _isCollectBullet = false;//Nhân vật bị rơi vào trạng thái va chạm vs đạn và nó bị bay ra khỏi vị trí hiện tại của nó
    Vector2 _collectVelocity;//Lực khi nhân vật rơi vào trạng thái va chạm vs đạn
    private Animator _anim;//Lưu trữ hiệu ứng của nhân vật
    float _TimeBlink = 0;
    Controller2D controller;
    SoundPlayerController _PlayerSound;
    public enum State { Idle, Move, Jump, Blink, Hurt }//Lưu trữ trạng thái nhân vật
    public State _State = State.Idle;
    public enum Dir { left, right }//Hướng hiện tại của nhân vật
    public enum MovePlayer { None, MoveRight, MoveLeft, JumpRight, JumpLeft }

    public Dir _Dir;
    float xMax, xMin, yMin;

	void Start() {
        controller = GetComponent<Controller2D>();
        _anim = gameObject.GetComponent<Animator>();
        _PlayerSound = FindObjectOfType<SoundPlayerController>();
        _MovePlayer = MovePlayer.None;
        _Dir = Dir.right;
        gravity = -(2 * jumpHeight) / Mathf.Pow (timeToJumpApex, 2);
		jumpVelocity = Mathf.Abs(gravity) * timeToJumpApex;
        GameObject other = GameObject.FindGameObjectWithTag("Map_Front");
        xMax = other.transform.position.x + (other.GetComponent<BoxCollider2D>().size.x * other.transform.localScale.x)/2;
        xMin = other.transform.position.x - (other.GetComponent<BoxCollider2D>().size.x * other.transform.localScale.x)/2;
        yMin = other.transform.position.y - (other.GetComponent<BoxCollider2D>().size.y * other.transform.localScale.y)/2;
	//	print ("Gravity: " + gravity + "  Jump Velocity: " + jumpVelocity);
	}

	void Update() {
       
		if (controller.collisions.above || controller.collisions.below) {
			velocity.y = 0;
		}

		Vector2 input = new Vector2 (0,0);
        if (_isCollectBullet)
        {
            input.x = _collectVelocity.x;
            velocity.y = _collectVelocity.y;
        }
        else
        {
            if (_MovePlayer==MovePlayer.None)
            {
                _TimeBlink += Time.deltaTime;
                if (_TimeBlink > 4)
                { 
                    if (gameObject.tag == "Player") _anim.Play("Player_Blink"); else _anim.Play("Enemy_Blink");
                    if (_TimeBlink > 6) _TimeBlink = 0;
                }
                else if (gameObject.tag == "Player") _anim.Play("Player_Idle"); else _anim.Play("Enemy_Idle");
                _Moving_Dust.SetActive(false);
            }
            if (_MovePlayer == MovePlayer.MoveLeft)
            {
                Destroy(GameObject.FindGameObjectWithTag("MyTurn"));
                if (gameObject.tag == "Player") _anim.Play("Player_Move"); else _anim.Play("Enemy_Move");
                _Moving_Dust.SetActive(true);
                input.x = -1;
            }
            if (_MovePlayer == MovePlayer.MoveRight)
            {
                Destroy(GameObject.FindGameObjectWithTag("MyTurn"));
                if (gameObject.tag == "Player") _anim.Play("Player_Move"); else _anim.Play("Enemy_Move");
                _Moving_Dust.SetActive(true);
                input.x = 1;
            }
            if (_MovePlayer == MovePlayer.JumpLeft)
            {
                Destroy(GameObject.FindGameObjectWithTag("MyTurn"));
                if (gameObject.tag == "Player") _anim.Play("Player_Jump"); else _anim.Play("Enemy_Jump");
                input.x = -1;
            }
            if (_MovePlayer == MovePlayer.JumpRight)
            {
                Destroy(GameObject.FindGameObjectWithTag("MyTurn"));
                if (gameObject.tag == "Player") _anim.Play("Player_Jump"); else _anim.Play("Enemy_Jump");
                input.x = 1;
            }
            if (_MovePlayer == MovePlayer.JumpLeft && controller.collisions.below)
            {
                velocity.y = jumpVelocity;
            }
            if (_MovePlayer == MovePlayer.JumpRight && controller.collisions.below)
            {
                velocity.y = jumpVelocity;
            }
            if (!_IsCurrent)
            {
                input = new Vector2(0, 0);
            }
            else
            {
                //if (velocity.x != 0 || input.y != 0 || input.x != 0) Destroy(GameObject.FindGameObjectWithTag("MyTurn"));
            }
        }

		float targetVelocityX = input.x * moveSpeed;
		velocity.x = Mathf.SmoothDamp (velocity.x, targetVelocityX, ref velocityXSmoothing, (controller.collisions.below)?accelerationTimeGrounded:accelerationTimeAirborne);
		velocity.y += gravity * Time.deltaTime;
        if (_isCollectBullet)
        {
            _collectVelocity.y += gravity * Time.deltaTime;
        }
        else
        {
            if (velocity.x> 0)
            {
                gameObject.transform.localScale = new Vector3(1, 1, 1);
                GameObject obj = FinObjectChildByName(gameObject, "numbers_frame");
                if (obj != null) obj.transform.localScale = new Vector3(1, 1, 1);
                _Dir = Dir.right;
            }
            else if (velocity.x < 0)
            {
                gameObject.transform.localScale = new Vector3(-1, 1, 1);
                GameObject obj = FinObjectChildByName(gameObject, "numbers_frame");
                if (obj != null) obj.transform.localScale = new Vector3(-1, 1, 1);
                _Dir = Dir.left;
            }
        }
    
		controller.Move (velocity * Time.deltaTime);
        //===========
        if (CheckDie()) // Kiểm tra xem player có chết hay không
        {
            Player player = gameObject.GetComponent<Player>(); // Lấy component Player của đối tượng hiện tại

            // Kiểm tra nếu player này là đối tượng hiện tại đang được điều khiển
            if (player._IsCurrent)
            {
                // Tìm đối tượng GameController trong scene
                GameController gameController = FindObjectOfType<GameController>();

                // Kiểm tra nếu trạng thái trò chơi không phải là đang thay đổi lượt chơi
                if (!gameController._GameState._IsChangding)
                {
                    gameController.ChangeTurn(); // Gọi phương thức để chuyển lượt chơi sang đối thủ
                }
            }

            Destroy(gameObject); // Hủy đối tượng player này khỏi scene
        }

    }

    /// <summary>
    /// Quay phải nhân vật
    /// </summary>
    public void RotateRight()
    {
        gameObject.transform.localScale = new Vector3(1, 1, 1);
        GameObject obj = FinObjectChildByName(gameObject, "numbers_frame");
        if (obj != null) obj.transform.localScale = new Vector3(1, 1, 1);
        _Dir = Dir.right;
        velocity.x = 0;
    }

    // Hàm này tự viết thêm.
    public void RotateLeft()
    {
        gameObject.transform.localScale = new Vector3(-1, 1, 1);
        GameObject obj = FinObjectChildByName(gameObject, "numbers_frame");
        if (obj != null) obj.transform.localScale = new Vector3(-1, 1, 1);
        _Dir = Dir.left;
        velocity.x = 0;
    }

    private bool CheckDie()
    {
        if (_Health<=0) return true;
        if (gameObject.transform.position.x<xMin || gameObject.transform.position.x>xMax || gameObject.transform.position.y<yMin)
        {
            return true;
        }
        return false;
    }
    /// <summary>
    /// Tìm kieem thằng đối tượng con trong đối tượng cha
    /// </summary>
    /// <returns></returns>
    private GameObject FinObjectChildByName(GameObject objParent, string nameChild)
    {
        Transform[] tran = objParent.GetComponentsInChildren<Transform>();
        for (int i = 0; i < tran.Length; i++)
        {
            if (tran[i].name == nameChild)
            {
                return tran[i].gameObject;
            }
        }
        return null;
    }

    float _TimeCount = 0;
    void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject.tag=="Flamethrower")
        {
            _TimeCount += Time.deltaTime;
            if (_TimeCount>0.07f)
            {
                _PlayerSound.PlayHurt(true);
                _TimeCount = 0;
                if (gameObject.tag == "Player") _anim.Play("Player_Hurt"); else _anim.Play("Enemy_Hurt");
                _Health -= (!_ISShield) ? 1 : (int)Mathf.Round(1 * 30 / 100);
                if (_Health <= 0) _Health = 0;
                NumberText textHealth = gameObject.GetComponentInChildren<NumberText>();
                textHealth.SetNumberText(_Health);
            }
           
        }
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        //Tính dame khi va chạm với đạn
        if (other.gameObject.tag == "Explosion_Bazoo")
        {
            _PlayerSound.PlayHurt(true);
            if (gameObject.tag == "Player") _anim.Play("Player_Hurt"); else _anim.Play("Enemy_Hurt");
            _Health -= (!_ISShield) ? 7 : (int)Mathf.Round(7 * 30 / 100);
            if (_Health <= 0) _Health = 0;
            NumberText textHealth = gameObject.GetComponentInChildren<NumberText>();
            textHealth.SetNumberText(_Health);
        }
        if (other.gameObject.tag == "Explosion")
        {
            if (other.gameObject.transform.position.y <= gameObject.transform.position.y)
            {
                _PlayerSound.PlayHurt(true);
                if (gameObject.tag == "Player") _anim.Play("Player_Hurt"); else _anim.Play("Enemy_Hurt");
                float dis = gameObject.transform.position.y - other.gameObject.transform.position.y;
                if (gameObject.transform.position.x > other.gameObject.transform.position.x)
                {
                    _isCollectBullet = true;
                    _collectVelocity = dis<0.3?(new Vector2(2, 2)):(new Vector2(1*dis, 2));
                }
                else
                {
                    _isCollectBullet = true;
                    _collectVelocity =dis<0.3?(new Vector2(-2, 2)):(new Vector2(-1*dis, 2));
                }
            }
        }
        if (other.gameObject.name=="explosion_shotgun(Clone)")
        {
            _PlayerSound.PlayHurt(true);
            if (gameObject.tag == "Player") _anim.Play("Player_Hurt"); else _anim.Play("Enemy_Hurt");
            _Health -= (!_ISShield) ? 10 : (int)Mathf.Round(10 * 30 / 100); 
            if (_Health <= 0) _Health = 0;
            NumberText textHealth = gameObject.GetComponentInChildren<NumberText>();
            textHealth.SetNumberText(_Health);
        }
        if (other.gameObject.tag == "Explosion_Driller")
        {
            if (!_IsCurrent)
            {
                _PlayerSound.PlayHurt(true);
                if (gameObject.tag == "Player") _anim.Play("Player_Hurt"); else _anim.Play("Enemy_Hurt");
                _Health -= (!_ISShield) ? 17 : (int)Mathf.Round(17 * 30 / 100);
                if (_Health <= 0) _Health = 0;
                NumberText textHealth = gameObject.GetComponentInChildren<NumberText>();
                textHealth.SetNumberText(_Health);
            }
        }
        if (other.gameObject.tag == "Explosion_Poison")
        {
            _PlayerSound.PlayHurt(true);
            if (gameObject.tag == "Player") _anim.Play("Player_Hurt"); else _anim.Play("Enemy_Hurt");
            _Health -= (!_ISShield) ? 10 : (int)Mathf.Round(10 * 30 / 100);
            if (_Health <= 0) _Health = 0;
            NumberText textHealth = gameObject.GetComponentInChildren<NumberText>();
            textHealth.SetNumberText(_Health);
        }
        if (other.gameObject.tag == "Explosion_Minigun")
        {
            if (!_IsCurrent)
            {
                _PlayerSound.PlayHurt(true);
                if (gameObject.tag == "Player") _anim.Play("Player_Hurt"); else _anim.Play("Enemy_Hurt");
                _Health -= (!_ISShield) ? 2 : (int)Mathf.Round(2 * 30 / 100);
                if (_Health <= 0) _Health = 0;
                NumberText textHealth = gameObject.GetComponentInChildren<NumberText>();
                textHealth.SetNumberText(_Health);
            }
        }
        if (other.gameObject.tag == "Laser")
        {
            if (!_IsCurrent)
            {
                _PlayerSound.PlayHurt(true);
                if (gameObject.tag == "Player") _anim.Play("Player_Hurt"); else _anim.Play("Enemy_Hurt");
                _Health -= (!_ISShield) ? 25 : (int)Mathf.Round(25 * 30 / 100);
                if (_Health <= 0) _Health = 0;
                NumberText textHealth = gameObject.GetComponentInChildren<NumberText>();
                textHealth.SetNumberText(_Health);
            }
        }
        if (other.gameObject.tag == "Pick_Punch")
        {
            if (!_IsCurrent)
            {
                _PlayerSound.PlayHurt(true);
                if (gameObject.tag == "Player") _anim.Play("Player_Hurt"); else _anim.Play("Enemy_Hurt");
                _Health -= (!_ISShield) ? 50 : (int)Mathf.Round(50 * 30 / 100);
                if (_Health <= 0) _Health = 0;
                NumberText textHealth = gameObject.GetComponentInChildren<NumberText>();
                textHealth.SetNumberText(_Health);
                if (gameObject.transform.position.x > other.gameObject.transform.parent.transform.position.x)
                {
                    _isCollectBullet = true;
                    _collectVelocity = new Vector2(2.5f, 2.5f);
                }
                else 
                {
                    _isCollectBullet = true;
                    _collectVelocity = new Vector2(-2.5f, 2.5f);
                }
                other.gameObject.GetComponent<BoxCollider2D>().enabled = false;
            }
        }
    }
    //=========
   
    }
