using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CannonScript : MonoBehaviour
{
	// Các biến public có thể được thiết lập từ Unity Editor
	public GameObject TrajectoryPointPrefeb; // Prefab cho các điểm đường đi
	public GameObject BallPrefb; // Prefab cho viên đạn
	public int numOfTrajectoryPoints = 30; // Số lượng điểm đường đi
	public float _Angle; // Góc sai khác của súng so với góc ban đầu


	// Các biến private
	private GameObject ball; // Biến để lưu trữ viên đạn hiện tại
	private bool isPressed, isBallThrown; // Biến kiểm tra trạng thái
	private float power = 5; // Biến lưu trữ lực bắn
	private List<GameObject> trajectoryPoints; // Danh sách các điểm đường đi
	GameControler _GameControler; // Tham chiếu đến GameControler
	private float _scale = 0.1f; // Thay đổi kích thước cho các điểm đường đi
	private Vector3 _posDown, _posUp; // Điểm bắt đầu và kết thúc kéo tay lấy lực
	private Animator _anim; // Tham chiếu đến Animator để điều khiển hoạt ảnh
	UIManager _UIManager; // Tham chiếu đến UIManager
	SoundControler _SoundControler; // Tham chiếu đến SoundControler

	//---------------------------------------	
	void Start()
	{
		// Khởi tạo danh sách điểm đường đi
		trajectoryPoints = new List<GameObject>();
		// Tìm và lưu GameControler
		_GameControler = FindObjectOfType<GameControler>();
		// Tìm Animator trong con của đối tượng này
		_anim = gameObject.GetComponentInChildren<Animator>();
		// Tìm UIManager
		_UIManager = FindObjectOfType<UIManager>();
		// Tìm SoundControler
		_SoundControler = FindObjectOfType<SoundControler>();
		// Đặt trạng thái ban đầu
		isPressed = isBallThrown = false;

		// Tạo các điểm đường đi
		for (int i = 0; i < numOfTrajectoryPoints; i++)
		{
			GameObject dot = (GameObject)Instantiate(TrajectoryPointPrefeb); // Tạo điểm đường đi mới từ prefab
																			 // Đặt kích thước cho điểm đường đi
			dot.transform.localScale = new Vector3((numOfTrajectoryPoints - i * 0.6f) * _scale, (numOfTrajectoryPoints - i * 0.6f) * _scale, (numOfTrajectoryPoints - i * 0.6f) * _scale);
			dot.GetComponent<Renderer>().enabled = false; // Ẩn điểm đường đi
			trajectoryPoints.Insert(i, dot); // Thêm điểm vào danh sách
		}
	}
	//---------------------------------------	
	void Update()
	{
		// Kiểm tra trạng thái chơi và dừng xử lý nếu có bất kỳ điều kiện nào không phù hợp
		if (_GameControler._CheckHit) return; // Nếu đã trúng mục tiêu thì không làm gì cả

		if (_GameControler._TypeGame == 0 && _GameControler._GameState._IsEnemyStart) return; // Nếu trò chơi bắt đầu với kẻ thù thì không làm gì cả

		// Kiểm tra vị trí chuột so với đối tượng cần bắn
		if ((Camera.main.ScreenToWorldPoint(Input.mousePosition).y < _GameControler.INood.transform.position.y || _UIManager._ListPopup[0].activeSelf) && !isPressed) return;

		// Nếu viên đạn đã được bắn, không xử lý tiếp
		if (isBallThrown) return;

		// Kiểm tra sự kiện nhấn chuột
		if (Input.GetMouseButtonDown(0))
		{
			isPressed = true; // Đặt trạng thái là nhấn chuột
			_posDown = Camera.main.ScreenToWorldPoint(Input.mousePosition); // Lưu vị trí nhấn chuột
			if (!ball) createBall(); // Nếu chưa có viên đạn, tạo viên đạn
		}
		// Kiểm tra sự kiện nhả chuột
		else if (Input.GetMouseButtonUp(0))
		{
			isPressed = false; // Đặt trạng thái là không nhấn chuột
			_posUp = Camera.main.ScreenToWorldPoint(Input.mousePosition); // Lưu vị trí nhả chuột
			if (!isBallThrown) throwBall(); // Nếu viên đạn chưa được bắn, thực hiện bắn
		}

		// Nếu đang nhấn chuột, tính toán góc và lực
		if (isPressed)
		{
			// Tính toán vận tốc ban đầu từ vị trí chuột
			Vector3 vel = GetForceFrom(Camera.main.ScreenToWorldPoint(Input.mousePosition), _posDown); // Vector vận tốc ban đầu
			float angle = Mathf.Atan2(vel.y, vel.x) * Mathf.Rad2Deg; // Góc giữa vel và trục Ox
			Player player = _GameControler._GameObj.GetComponent<Player>(); // Lấy tham chiếu đến Player
			angle += _Angle; // Cộng thêm góc sai khác vào góc

			// Điều chỉnh góc dựa trên hướng của Player
			if (player._Dir == Player.Dir.left)
			{
				angle = 360 - angle; // Nếu Player hướng sang trái, điều chỉnh góc
			}
			transform.eulerAngles = new Vector3(0, 0, angle); // Đặt góc xoay cho súng
			setTrajectoryPoints(transform.position, vel / ball.GetComponent<Rigidbody2D>().mass); // Tính toán và hiển thị các điểm đường đi
		}
	}

	// Tạo viên đạn mới
	private void createBall()
	{
		ball = (GameObject)Instantiate(BallPrefb); // Tạo viên đạn từ prefab

		Vector3 pos = transform.position; // Lấy vị trí của súng
		pos.z = 1; // Đặt chiều sâu để viên đạn không bị chồng lên
		ball.transform.position = pos; // Đặt vị trí cho viên đạn
		ball.SetActive(false); // Ẩn viên đạn
	}

	// Phương thức để bắn viên đạn
	private void throwBall()
	{
		_SoundControler.PlayBazookaSound(true); // Phát âm thanh bắn
		_anim.Play("Bazooka"); // Chơi hoạt ảnh bắn
		ball.SetActive(true); // Hiện viên đạn

		// Bắn viên đạn với lực tính toán
		ball.GetComponent<Rigidbody2D>().AddForce(GetForceFrom(_posUp, _posDown), ForceMode2D.Impulse);
		isBallThrown = true; // Đánh dấu rằng viên đạn đã được bắn

		// Xóa các điểm chỉ dẫn
		GameObject[] arrObj = GameObject.FindGameObjectsWithTag("Dot"); // Tìm tất cả các đối tượng có tag "Dot"
		for (int i = 0; i < arrObj.Length; i++) Destroy(arrObj[i]); // Xóa từng đối tượng
		_GameControler._CheckHit = true; // Đặt trạng thái kiểm tra va chạm
		_GameControler._StopTime = true; // Dừng thời gian trò chơi
		_UIManager.ResetImgPick(); // Đặt lại hình ảnh trong UI
	}


	/// <summary>
	/// Phương thức bắn khi quân địch tự bắn
	/// </summary>
	public void ThrowBallEnemy(Vector3 vel)
	{
		_SoundControler.PlayBazookaSound(true); // Phát âm thanh bắn
		if (!ball) createBall(); // Nếu chưa có viên đạn, tạo viên đạn
		_anim.Play("Bazooka"); // Chơi hoạt ảnh bắn
		ball.SetActive(true); // Hiện viên đạn

		// Bắn viên đạn với lực truyền vào
		ball.GetComponent<Rigidbody2D>().AddForce(vel, ForceMode2D.Impulse);
		_GameControler._CheckHit = true; // Đặt trạng thái kiểm tra va chạm
		_GameControler._StopTime = true; // Dừng thời gian trò chơi
		_UIManager.ResetImgPick(); // Đặt lại hình ảnh trong UI
	}

	// Tính toán lực từ vị trí nhấn chuột và nhả chuột
	private Vector2 GetForceFrom(Vector3 fromPos, Vector3 toPos)
	{
		// Đặt giá trị ban đầu cho lực
		power = 5;

		// Giảm lực nếu khoảng cách quá xa
		while (((new Vector2(toPos.x, toPos.y) - new Vector2(fromPos.x, fromPos.y)) * power).sqrMagnitude > 450f) power -= 0.05f;
		return (new Vector2(toPos.x, toPos.y) - new Vector2(fromPos.x, fromPos.y)) * power; // Trả về lực tính toán
	}

	// Hiển thị đường đi của viên đạn
	void setTrajectoryPoints(Vector3 pStartPosition, Vector3 pVelocity)
	{
		// Tính toán vận tốc và góc
		float velocity = Mathf.Sqrt((pVelocity.x * pVelocity.x) + (pVelocity.y * pVelocity.y));
		float angle = Mathf.Rad2Deg * (Mathf.Atan2(pVelocity.y, pVelocity.x));
		float fTime = 0;

		fTime += 0.1f; // Bắt đầu từ thời gian 0.1

		for (int i = 0; i < numOfTrajectoryPoints; i++)
		{
			// Tính toán vị trí của từng điểm đường đi
			float dx = velocity * fTime * Mathf.Cos(angle * Mathf.Deg2Rad);
			float dy = velocity * fTime * Mathf.Sin(angle * Mathf.Deg2Rad) - (Physics2D.gravity.magnitude * fTime * fTime / 2.0f);
			Vector3 pos = new Vector3(pStartPosition.x + dx, pStartPosition.y + dy, 2); // Tính toán vị trí mới

			trajectoryPoints[i].transform.position = pos; // Cập nhật vị trí điểm đường đi

			trajectoryPoints[i].GetComponent<Renderer>().enabled = true; // Hiện điểm đường đi

			// Đặt góc cho điểm đường đi
			trajectoryPoints[i].transform.eulerAngles = new Vector3(0, 0, Mathf.Atan2(pVelocity.y - (Physics.gravity.magnitude) * fTime, pVelocity.x) * Mathf.Rad2Deg);

			fTime += 0.1f; // Tăng thời gian để tính toán cho điểm tiếp theo
		}
	}
}