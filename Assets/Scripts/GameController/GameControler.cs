using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class GameControler : MonoBehaviour
{

    // Các biến public lưu trữ đối tượng trong game
    public GameObject _GameObj; // Lưu trữ đối tượng player hoặc Enemy đang được chọn
    public GameObject _Pick; // Lưu trữ vũ khí đang được chọn hiện tại
    public GameObject[] _Prefab_MyTurn; // Lưu prefabs cho lượt của người chơi
    public GameState _GameState = new GameState(); // Lưu các trạng thái của trò chơi
    public Pick[] _ListStatePickEnemy; // Trạng thái vũ khí của quân địch (có được sử dụng hay không)
    public Pick[] _ListStatePickPlayer; // Trạng thái vũ khí của người chơi
    public Image[] _ListImageTimer; // Lưu danh sách số chạy cho bộ đếm thời gian (30s)
    public GameObject[] _ListPlayer; // Danh sách các đối tượng Player
    public GameObject[] _ListMap; // Danh sách các đối tượng Map
    public GameObject INood; // Lưu tọa độ chuột khi click để kiểm tra
    public int _TypeGame = 1; // Kiểu game: 0 là Single Player, 1 là Hotseat (đấu với máy)
    public bool _CheckHit = false; // Biến kiểm tra xem có hit mục tiêu hay không
    public int _Level = 1; // Độ khó game: 1 = Dễ, 2 = Trung bình, 3 = Khó

    // Các đối tượng quản lý UI, Camera, Map, v.v.
    UIManager _UIManager;
    CameraControler _cameraControler;
    MapControler _mapControler;
    NumberTextScale _numberText;
    GameManager _GameManager;
    int _Timer = 30; // Bộ đếm thời gian cho lượt chơi
    float _timeCount = 0; // Bộ đếm thời gian tính từng giây
    public int _NumberPlayer = 3; // Số lượng player
    public int _IndexMap = 1; // Chỉ số map hiện tại
    public bool _StopTime = false; // Biến điều khiển tạm dừng thời gian
    public int _Gold = 0; // Lượng vàng hiện tại của người chơi

    // Đối tượng âm thanh nền và âm thanh hiệu ứng
    BackgoundSoundControler _BgControler;
    SoundControler _SoundControler;

    // Hàm khởi tạo chạy khi game bắt đầu
    void Start()
    {
        // Tìm các đối tượng quản lý giao diện, camera, âm thanh, game manager
        _UIManager = FindObjectOfType<UIManager>();
        _cameraControler = FindObjectOfType<CameraControler>();
        _numberText = GetComponent<NumberTextScale>();
        _GameManager = FindObjectOfType<GameManager>();
        _BgControler = FindObjectOfType<BackgoundSoundControler>();

        // Bắt đầu phát âm thanh nền
        _BgControler.PlayBackground(true);
        _SoundControler = FindObjectOfType<SoundControler>();
    }

    // Update is called once per frame
    void Update()
    {
        // Kiểm tra khi người dùng nhấn phím Escape
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            _UIManager.ShowPanelExitGame(); // Hiển thị menu thoát game
        }

        // Nếu game không ở trạng thái đang chơi thì dừng update
        if (!_GameState._IsGamePlay) return;

        // Kiểm tra trạng thái lượt chơi của người chơi và quân địch, và không đang trong quá trình chuyển trạng thái hoặc tạm dừng
        if ((_GameState._IsPlayerStart || _GameState._IsEnemyStart) && !_GameState._IsChangding && !_StopTime)
        {
            _timeCount += Time.deltaTime; // Tăng thời gian đếm
            if (_timeCount >= 1)
            {
                _timeCount = 0; // Reset đếm giây
                if (_Timer > 0) _Timer--; // Giảm thời gian còn lại
                _numberText.SetNumText2(_Timer, _ListImageTimer); // Cập nhật giao diện bộ đếm thời gian

                // Tìm các đối tượng bullet trong game
                GameObject[] objBullet = GameObject.FindGameObjectsWithTag("Bullet");

                // Nếu hết thời gian và không còn bullet nào thì chuyển lượt chơi
                if (_Timer <= 0 && objBullet.Length == 0)
                {
                    if (!_GameState._IsChangding)
                    {
                        ChangeTurn(); // Gọi hàm chuyển lượt
                    }
                }
            }
        }
        else
        {
            _Timer = 30; // Reset thời gian về 30 giây cho lượt tiếp theo
        }

        // Xử lý khi nhấn chuột trái (click vào đối tượng)
        if (Input.GetMouseButtonDown(0))
        {
            if (_GameState._IsPlayer && !_GameState._IsPlayerStart)
            {
                _UIManager.HidePopopTurnGreen(); // Ẩn thông báo lượt chơi của player
                _GameObj = GetMyTurn(); // Lấy đối tượng player trong lượt hiện tại
                _GameState._IsChangding = false;
                _StopTime = false;

                // Chuyển camera và map tới vị trí của đối tượng
                _mapControler = FindObjectOfType<MapControler>();
                _cameraControler.MoveCameraToBullet(_GameObj.transform.position);
                _mapControler.MoveMapToBullet(_GameObj.transform.position);
                _cameraControler._IsMove = true;
                _mapControler._IsMove = true;
            }
        }

        // Xử lý cho chế độ Hotseat (2 người chơi luân phiên)
        if (_TypeGame == 1)
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (!_GameState._IsPlayer && !_GameState._IsEnemyStart)
                {
                    _UIManager.HidePopopTurnBlue(); // Ẩn thông báo lượt chơi của quân địch
                    _GameObj = GetMyTurn(); // Lấy đối tượng Enemy trong lượt hiện tại
                    _GameState._IsChangding = false;
                    _StopTime = false;

                    // Chuyển camera và map tới vị trí của đối tượng
                    _mapControler = FindObjectOfType<MapControler>();
                    _cameraControler.MoveCameraToBullet(_GameObj.transform.position);
                    _mapControler.MoveMapToBullet(_GameObj.transform.position);
                    _cameraControler._IsMove = true;
                    _mapControler._IsMove = true;
                }
            }
        }
    }

    // Hàm lấy lượt chơi hiện tại
    public GameObject GetMyTurn()
    {
        if (_GameState._IsPlayer)
        {
            // Nếu là lượt của player, đặt tất cả đối tượng Enemy về trạng thái chưa chọn
            GameObject[] arrEnemy = GameObject.FindGameObjectsWithTag("Enemy");
            for (int i = 0; i < arrEnemy.Length; i++)
            {
                Player player = arrEnemy[i].GetComponent<Player>();
                player._IsCurrent = false;
            }
        }
        else
        {
            // Nếu là lượt của Enemy, đặt tất cả đối tượng Player về trạng thái chưa chọn
            GameObject[] arrPlayer = GameObject.FindGameObjectsWithTag("Player");
            for (int i = 0; i < arrPlayer.Length; i++)
            {
                Player player = arrPlayer[i].GetComponent<Player>();
                player._IsCurrent = false;
            }
        }

        string tag = _GameState._IsPlayer ? "Player" : "Enemy";
        GameObject[] arrGameObj = GameObject.FindGameObjectsWithTag(tag);
        Object obj = new Object();
        int index = 100;
        int count = 0;//Dếm số phần tử chưa bắn còn lại

        #region==========Test thử xem có thằng nào chưa được bắn hay ko========
        for (int i = 0; i < arrGameObj.Length; i++)
        {
            Player player = arrGameObj[i].GetComponent<Player>();
            if (!player._IsFile)
            {
                count++;
            }
        }
        if (count == 0)
        {
            for (int i = 0; i < arrGameObj.Length; i++)
            {
                Player player = arrGameObj[i].GetComponent<Player>();
                player._IsFile = false;
            }
        }
        #endregion

        for (int i = 0; i < arrGameObj.Length; i++)
        {
            Player player = arrGameObj[i].GetComponent<Player>();
            player._IsCurrent = false;
            if (!player._IsFile)
            {
                if (player._Index < index) obj = arrGameObj[i];
            }
        }

        //Hiện myturn vào thăng obj được chọn
        Player playerChoose = ((GameObject)obj).GetComponent<Player>();
        playerChoose._IsCurrent = true;
        playerChoose._IsFile = true;

        GameObject myTurnOld = GameObject.FindGameObjectWithTag("MyTurn");
        if (myTurnOld != null) Destroy(myTurnOld);
        GameObject myTurn;
        if (playerChoose._Dir == Player.Dir.right) myTurn = (GameObject)Instantiate(_Prefab_MyTurn[0], ((GameObject)obj).transform.position, Quaternion.identity);
        else myTurn = (GameObject)Instantiate(_Prefab_MyTurn[1], ((GameObject)obj).transform.position, Quaternion.identity);
        myTurn.transform.parent = ((GameObject)obj).transform;
        return (GameObject)obj;
    }

    public void SetMyTurnForObj(GameObject obj)
    {
        if (_GameState._IsPlayer)
        {
            GameObject[] arrEnemy = GameObject.FindGameObjectsWithTag("Enemy");
            for (int i = 0; i < arrEnemy.Length; i++)
            {
                Player player = arrEnemy[i].GetComponent<Player>();
                player._IsCurrent = false;
            }
        }
        else
        {
            GameObject[] arrPlayer = GameObject.FindGameObjectsWithTag("Player");
            for (int i = 0; i < arrPlayer.Length; i++)
            {
                Player player = arrPlayer[i].GetComponent<Player>();
                player._IsCurrent = false;
            }
        }
        GameObject myTurnOld = GameObject.FindGameObjectWithTag("MyTurn");
        if (myTurnOld != null) Destroy(myTurnOld);
        GameObject myTurn;
        Player playerChoose = obj.GetComponent<Player>();
        playerChoose._IsCurrent = true;
        if (playerChoose._Dir == Player.Dir.right) myTurn = (GameObject)Instantiate(_Prefab_MyTurn[0], ((GameObject)obj).transform.position, Quaternion.identity);
        else myTurn = (GameObject)Instantiate(_Prefab_MyTurn[1], ((GameObject)obj).transform.position, Quaternion.identity);
        myTurn.transform.parent = ((GameObject)obj).transform;
    }
    public void MoveCameraToObj()
    {
        _mapControler = FindObjectOfType<MapControler>();
        _cameraControler.MoveCameraToBullet(_GameObj.transform.position);
        _mapControler.MoveMapToBullet(_GameObj.transform.position);
        _cameraControler._IsMove = false;
        _mapControler._IsMove = false;
    }
    /// <summary>
    /// Thực hiện chuyển turn giữa các đối tượng trong game
    /// </summary>
    // Hàm đổi lượt chơi giữa các đối tượng (Player và Enemy)

    public void ChangeTurn()
    {
        _GameState._IsChangding = true; // Đánh dấu đang chuyển lượt
        _CheckHit = false;
        StartCoroutine(WaitForChangeTurn()); // Gọi coroutine đợi đổi lượt
    }
    private int CheckGameOver()
    {
        GameObject[] arrGreen = GameObject.FindGameObjectsWithTag("Player");
        if (arrGreen.Length == 0)
        {
            _GameState._IsGamePlay = false;
            return 1;//Đội blue thắng
        }
        GameObject[] arrBlue = GameObject.FindGameObjectsWithTag("Enemy");
        if (arrBlue.Length == 0)
        {
            _GameState._IsGamePlay = false;
            return 0;//Đội green thắng
        }
        return -1;
    }
    /// <summary>
    /// Bắt đầu chơi game
    /// </summary>
    public void StartGame()
    {
        GameObject mapOld = GameObject.FindGameObjectWithTag("Maps");
        if (mapOld != null) Destroy(mapOld);
        GameObject[] playerOld = GameObject.FindGameObjectsWithTag("Player");
        for (int i = 0; i < playerOld.Length; i++)
        {
            Destroy(playerOld[i]);
        }
        GameObject[] enemyOld = GameObject.FindGameObjectsWithTag("Enemy");
        for (int i = 0; i < enemyOld.Length; i++)
        {
            Destroy(enemyOld[i]);
        }
        //=============
        Instantiate(_ListMap[_IndexMap], new Vector3(0, 0, 0), Quaternion.identity);

        GameObject other = GameObject.FindGameObjectWithTag("Map_Front");
        float xMax = other.transform.position.x + (other.GetComponent<BoxCollider2D>().size.x * other.transform.localScale.x) / 2;
        float xMin = other.transform.position.x - (other.GetComponent<BoxCollider2D>().size.x * other.transform.localScale.x) / 2;
        float yMax = other.transform.position.y + (other.GetComponent<BoxCollider2D>().size.y * other.transform.localScale.y) / 2;
        List<float> listPos = new List<float>();
        for (int i = 0; i < _NumberPlayer; i++)
        {
            float ranPosX = Random.Range(xMin + 2, xMax - 2);
            Vector3 pos = new Vector3(ranPosX, yMax, 0);
            while (!_GameManager.CheckInitPlayer(pos) || !_GameManager.CheckPosXPlayer(pos.x, listPos, 0.75f))
            {
                ranPosX = Random.Range(xMin + 2, xMax - 2);
                pos = new Vector3(ranPosX, yMax, 0);
            }
            Instantiate(_ListPlayer[0], pos, Quaternion.identity);
            listPos.Add(pos.x);
        }
        for (int i = 0; i < _NumberPlayer; i++)
        {
            float ranPosX = Random.Range(xMin + 2, xMax - 2);
            Vector3 pos = new Vector3(ranPosX, yMax, 0);
            while (!_GameManager.CheckInitPlayer(pos) || !_GameManager.CheckPosXPlayer(pos.x, listPos, 0.75f))
            {
                ranPosX = Random.Range(xMin + 2, xMax - 2);
                pos = new Vector3(ranPosX, yMax, 0);
            }
            Instantiate(_ListPlayer[1], pos, Quaternion.identity);
            listPos.Add(pos.x);
        }
        _UIManager.LoadStatePickEnemy();
        _UIManager.LoadStatePickPlayer();
        //===========
        Camera.main.transform.position = new Vector3(0, 0, -10);
        //============
        StartCoroutine(WaitForStartGame());
    }
    /// <summary>
    /// Lấy số Gold hiện tại
    /// </summary>
    public int LoadGold()
    {
        string gold = ReadWriteFileText.GetStringFromPrefab(Data._Gold);
        if (gold != "")
        {
            return int.Parse(gold);
        }
        return 0;
    }
    /// <summary>
    /// Thoat khoi game
    /// </summary>
    public void QuitGame()
    {
        Application.Quit();
    }

    /// <summary>
    /// Khi click vào nut time
    /// </summary>
    public void btnTime()
    {
        _SoundControler.PlayClickSound(true);
        GameObject[] arrBullet = GameObject.FindGameObjectsWithTag("Bullet");
        GameObject[] arrDart = GameObject.FindGameObjectsWithTag("Dart");
        if (_CheckHit && !_GameState._IsChangding && arrBullet.Length == 0 && arrDart.Length == 0)
        {
            ChangeTurn();
        }
    }
    // Coroutine chờ đợi trước khi chuyển lượt hoàn tất
    IEnumerator WaitForChangeTurn()
    {
        yield return new WaitForSeconds(2f); // Đợi 2 giây

        // Kiểm tra trạng thái các Player (ví dụ bị độc sẽ mất máu)
        Player[] arrPlayer = FindObjectsOfType<Player>();
        for (int i = 0; i < arrPlayer.Length; i++)
        {
            if (arrPlayer[i]._IsPoison)
            {
                // Nếu bị nhiễm độc, giảm máu
                if (arrPlayer[i]._Health > 10)
                {
                    arrPlayer[i]._Health -= 10;
                }
                else arrPlayer[i]._Health = 1;
                NumberText textHealth = arrPlayer[i].gameObject.GetComponentInChildren<NumberText>();
                textHealth.SetNumberText(arrPlayer[i]._Health); // Cập nhật số máu
            }
        }
        _UIManager.SetImgButtonChoosePick();
        _numberText.SetNumText2(30, _ListImageTimer);
        int checkOver = CheckGameOver();
        if (checkOver != -1)
        {
            _UIManager.ShowPanelGameOver(checkOver);
        }
        else
        {
            GameObject[] arrPickOld = GameObject.FindGameObjectsWithTag("Pick");
            for (int i = 0; i < arrPickOld.Length; i++)
            {
                Pick_ShieldControler pick = arrPickOld[i].GetComponent<Pick_ShieldControler>();
                Pick_MineControler pickMine = arrPickOld[i].GetComponent<Pick_MineControler>();
                if (pickMine != null) pickMine.ActivePick_Mine();
                if (pick == null && pickMine == null) Destroy(arrPickOld[i]);
            }
            GameObject[] bullet = GameObject.FindGameObjectsWithTag("Bullet");
            for (int i = 0; i < bullet.Length; i++)
            {
                Destroy(bullet[i]);
            }
            GameObject dart = GameObject.FindGameObjectWithTag("Dart");
            if (dart != null) Destroy(dart);

            GameObject[] line = GameObject.FindGameObjectsWithTag("Dot");
            for (int i = 0; i < line.Length; i++)
            {
                Destroy(line[i]);
            }
            //======================
            if (_GameState._IsPlayer)//Chuyển sang Enemy
            {
                _GameState._IsPlayer = false;
                _UIManager.ShowPopupTurnBlue();
            }
            else//Chuyển sang Player
            {
                _GameState._IsPlayer = true;
                _UIManager.ShowPopopTurnGreen();
            }
        }

    }

    IEnumerator WaitForStartGame()
    {
        yield return new WaitForSeconds(1);
        _GameState._IsPlayer = true;
        _GameState._IsPlayerStart = false;
        _GameState._IsGamePlay = true;
        _GameState._IsChangding = true;
        _CheckHit = false;
        _StopTime = false;
    }
    /// <summary>
    /// Class lưu lại các trạng thái game
    /// </summary>
    public class GameState
    {
        public bool _IsGamePlay = false;
        public bool _IsPlayer = true;//Lưu trữ lượt chơi hiện tại đang là của thằng Player
        public bool _IsPlayerStart = false;//Trạng thái quân xanh lá cây (quân ta) bắt đầu chơi
        public bool _IsEnemyStart = false;//trạng thái quân địch bắt đầu được chơi
        public bool _IsChangding = false;//Trang thai đang chuyển trạng thái
        public bool _Sound = true;//Lưu trữ trạng thái âm thanh
        public bool _Music = true;//Lưu trữ trạng thái music
    }
}
