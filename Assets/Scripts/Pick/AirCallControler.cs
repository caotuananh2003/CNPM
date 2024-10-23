using UnityEngine;
using System.Collections;

public class AirCallControler : MonoBehaviour
{
    // Mảng các vũ khí hiện có thể sử dụng
    public GameObject[] Pick;//Vũ khí được sử dụng hiện tại

    // Loại vũ khí đang sử dụng, mặc định là "Pick_Bomb"
    public string _Type = "Pick_Bomb";

    // Các đối tượng quản lý game
    GameControler _GameControler; // Điều khiển trò chơi
    GameManager _GameManager;     // Quản lý các trạng thái và hành động trong trò chơi
    UIManager _UIManager;         // Quản lý giao diện người dùng
    bool _CheckHit = false;       // Biến kiểm tra xem đã xảy ra va chạm hay chưa
    SoundControler _SoundControler;// Quản lý âm thanh

    // Use this for initialization
    void Start()
    {
        // Tìm kiếm và gán các thành phần điều khiển game, UI và âm thanh
        _GameControler = FindObjectOfType<GameControler>();
        _GameManager = FindObjectOfType<GameManager>();
        _UIManager = FindObjectOfType<UIManager>();
        _SoundControler = FindObjectOfType<SoundControler>();
    }

    // Update is called once per frame
    void Update()
    {
        // Nếu loại vũ khí là "Pick_Swap" và đã va chạm, thực hiện hoán đổi
        if (_Type == "Pick_Swap" && _CheckHit)
        {
            ActivePick_Swap();
        }

        // Nếu đã va chạm thì dừng việc kiểm tra tiếp
        if (_CheckHit) return;

        // Kiểm tra xem loại vũ khí có phải là "Pick_Health", nếu có thì thực hiện chức năng hồi máu
        if (_Type == "Pick_Health") Pick_Health();

        // Nếu game đang trong chế độ 0 (người chơi bắt đầu), hoặc kẻ địch đang bắt đầu lượt, thì không làm gì thêm
        if (_GameControler._TypeGame == 0 && _GameControler._GameState._IsEnemyStart) return;

        // Kiểm tra vị trí chuột để tránh các hành động dưới thanh giới hạn của game
        if (Camera.main.ScreenToWorldPoint(Input.mousePosition).y < _GameControler.INood.transform.position.y || _UIManager._ListPopup[0].activeSelf) return;

        // Kiểm tra loại vũ khí và thực hiện hành động tương ứng
        if (_Type == "Pick_Bomb") Pick_Bomb();        // Thực hiện khi sử dụng "Pick_Bomb"
        if (_Type == "Pick_Teleport") Pick_Teleport();// Thực hiện khi sử dụng "Pick_Teleport"
        if (_Type == "Pick_Swap") Pick_Swap();        // Thực hiện khi sử dụng "Pick_Swap"
    }

    /// <summary>
    /// Thực hiện khi type là Pick_bomp
    /// </summary>
    // Hàm thực hiện chức năng thả bom khi vũ khí là "Pick_Bomb"
    private void Pick_Bomb()
    {
        if (Input.GetMouseButtonDown(0))
        { // Khi nhấn chuột trái
            // Giảm số lượng đạn của người chơi hoặc kẻ thù
            if (_GameControler._GameObj.tag == "Player")
                _GameControler._ListStatePickPlayer[3].Ammo -= 1;
            else
                _GameControler._ListStatePickEnemy[3].Ammo -= 1;

            // Lấy vị trí giới hạn của camera
            Bounds bound = Helper.OrthographicBounds(Camera.main);
            Vector3 posMouse = Camera.main.ScreenToWorldPoint(Input.mousePosition); // Vị trí chuột
            Vector3 posInit = new Vector3(posMouse.x, bound.max.y, 0);              // Tọa độ bắt đầu của bom

            // Thả bom theo thời gian, mỗi quả bom cách nhau một khoảng ngắn
            StartCoroutine(waitForInit(0, new Vector3(posInit.x + 1.5f, posInit.y, posInit.z)));
            StartCoroutine(waitForInit(0.1f, new Vector3(posInit.x + 0.75f, posInit.y, posInit.z)));
            StartCoroutine(waitForInit(0.2f, new Vector3(posInit.x + 0.0f, posInit.y, posInit.z)));
            StartCoroutine(waitForInit(0.3f, new Vector3(posInit.x - 0.75f, posInit.y, posInit.z)));
            StartCoroutine(waitForInit(0.4f, new Vector3(posInit.x - 1.5f, posInit.y, posInit.z)));

            // Đánh dấu đã có va chạm và cập nhật trạng thái
            _CheckHit = true;
            _GameControler._CheckHit = true;
            _UIManager.ResetImgPick(); // Reset ảnh của pick sau khi sử dụng
        }
    }

    /// <summary>
    /// Bomp cho AI Enemy
    /// </summary>
    public void Pick_BombEnemy(Vector3 target)
    {
        // Tương tự như Pick_Bomb cho người chơi, nhưng được dùng cho AI
        if (_GameControler._GameObj.tag == "Player")
            _GameControler._ListStatePickPlayer[3].Ammo -= 1;
        else
            _GameControler._ListStatePickEnemy[3].Ammo -= 1;

        Bounds bound = Helper.OrthographicBounds(Camera.main);
        Vector3 posMouse = target; // Lấy vị trí mục tiêu là kẻ địch
        Vector3 posInit = new Vector3(posMouse.x, bound.max.y, 0); // Khởi tạo vị trí thả bom

        // Thả bom tương tự như Pick_Bomb
        StartCoroutine(waitForInit(0, new Vector3(posInit.x + 1.5f, posInit.y, posInit.z)));
        StartCoroutine(waitForInit(0.1f, new Vector3(posInit.x + 0.75f, posInit.y, posInit.z)));
        StartCoroutine(waitForInit(0.2f, new Vector3(posInit.x + 0.0f, posInit.y, posInit.z)));
        StartCoroutine(waitForInit(0.3f, new Vector3(posInit.x - 0.75f, posInit.y, posInit.z)));
        StartCoroutine(waitForInit(0.4f, new Vector3(posInit.x - 1.5f, posInit.y, posInit.z)));

        _GameControler._CheckHit = true; // Đánh dấu đã thả bom
        _UIManager.ResetImgPick();       // Reset lại UI pick
    }

    // Coroutine để thả bom sau một khoảng thời gian
    IEnumerator waitForInit(float time, Vector3 pos)
    {
        yield return new WaitForSeconds(time); // Đợi một khoảng thời gian
        Instantiate(Pick[0], pos, Quaternion.identity); // Tạo quả bom tại vị trí chỉ định
    }

    /// <summary>
    /// Thực hiện hàm chuyển đổi vị trí
    /// </summary
    Vector3 _posDes;//Vị trí nhân vật di chuyển đến
    GameObject _objTeleportOn, _objTeleportOff; // Các đối tượng liên quan đến hiệu ứng teleport.
    private void Pick_Teleport()
    {
        if (Input.GetMouseButtonDown(0))
        { // Khi nhấn chuột trái.
            // Xác định vị trí đích của teleport.
            _posDes = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            if (_GameManager.CheckTeleportPlayer(_posDes)) return; // Kiểm tra điều kiện hợp lệ của vị trí dịch chuyển.
            _posDes.z = -1; // Đặt vị trí z để dịch chuyển đối tượng.

            // Giảm số lần sử dụng đạn teleport.
            if (_GameControler._GameObj.tag == "Player")
                _GameControler._ListStatePickPlayer[9].Ammo -= 1;
            else
                _GameControler._ListStatePickEnemy[9].Ammo -= 1;

            // Phát âm thanh teleport và khởi tạo hiệu ứng dịch chuyển.
            _SoundControler.PlayTeleportSound(true);
            _objTeleportOff = (GameObject)Instantiate(Pick[1], gameObject.transform.position, Quaternion.identity);
            StartCoroutine(waitPick_Teleport_Off());
            _CheckHit = true; // Đánh dấu đã thực hiện teleport.
        }
    }

    // Coroutine chờ đợi và thực hiện quá trình teleport.
    IEnumerator waitPick_Teleport_Off()
    {
        yield return new WaitForSeconds(0.7f); // Đợi 0.7 giây trước khi tiếp tục.
        _SoundControler.PlayTeleportSound(true); // Phát âm thanh teleport lần nữa.
        Destroy(_objTeleportOff); // Hủy đối tượng teleport ban đầu.
        _objTeleportOn = (GameObject)Instantiate(Pick[2], _posDes, Quaternion.identity); // Khởi tạo đối tượng teleport mới tại vị trí đích.
        StartCoroutine(waitPick_Teleport_On());
    }

    IEnumerator waitPick_Teleport_On()
    {
        yield return new WaitForSeconds(0.6f); // Đợi 0.6 giây trước khi hoàn tất quá trình dịch chuyển.
        _GameControler._GameObj.transform.position = _posDes; // Dịch chuyển đối tượng tới vị trí đích đã được xác định.
        _UIManager.SetImgButtonChoosePick(); // Cập nhật hình ảnh của nút chọn vũ khí trong giao diện người dùng.
        Destroy(_objTeleportOn); // Hủy đối tượng hiệu ứng teleport sau khi đã hoàn tất dịch chuyển.
        _GameControler._CheckHit = true; // Đánh dấu rằng người chơi đã thực hiện hành động thành công.
        _UIManager.ResetImgPick(); // Đặt lại giao diện chọn vũ khí.
        _GameControler.ChangeTurn(); // Chuyển lượt cho đối thủ.
        Destroy(gameObject); // Hủy đối tượng vũ khí teleport.
    }

    #region-===================PICK_SWAP=============
    /// <summary>
    /// Kích hoạt chức năng Swap (hoán đổi) của vũ khí.
    /// </summary>
    public void Pick_Swap()//Pick 3
    {
        Player player = GetComponentInParent<Player>(); // Lấy đối tượng Player gắn với đối tượng hiện tại.
        string tag = "Player"; // Mặc định đặt tag cho Player.
        if (player.gameObject.tag == "Player") tag = "Player"; else tag = "Enemy"; // Xác định đối tượng hiện tại là Player hay Enemy.

        // Tìm tất cả các đối tượng có cùng tag.
        GameObject[] arrPlayer = GameObject.FindGameObjectsWithTag(tag);
        for (int i = 0; i < arrPlayer.Length; i++)
        {
            player = arrPlayer[i].GetComponent<Player>(); // Lấy đối tượng Player từ danh sách.

            // Nếu Player không phải là đối tượng hiện tại.
            if (!player._IsCurrent)
            {
                // Tạo đối tượng Swap tại vị trí của Player.
                GameObject swap = (GameObject)Instantiate(Pick[3], player.gameObject.transform.position, Quaternion.identity);
                swap.transform.parent = player.gameObject.transform; // Gán đối tượng Swap vào Player.
                swap.transform.position = new Vector3(swap.transform.position.x, swap.transform.position.y, -5); // Điều chỉnh vị trí của đối tượng Swap.
            }
        }
        _CheckHit = true; // Đánh dấu rằng người chơi đã thực hiện hành động Swap.
    }
    /// <summary>
    /// Kích hoạt vũ khí Swap khi người chơi chọn một mục tiêu để hoán đổi.
    /// </summary>
    public void ActivePick_Swap()
    {
        if (Input.GetMouseButtonDown(0)) // Kiểm tra nếu người chơi nhấn chuột trái.
        {
            GameObject obj = Helper.GetGameObjectAtPossition(); // Lấy đối tượng ở vị trí chuột của người chơi.

            // Nếu đối tượng được chọn là Swap.
            if (obj != null && obj.tag == "Swap")
            {
                // Giảm số lượng đạn Swap tùy theo người chơi hay đối thủ.
                if (_GameControler._GameObj.tag == "Player")
                    _GameControler._ListStatePickPlayer[15].Ammo -= 1;
                else
                    _GameControler._ListStatePickEnemy[15].Ammo -= 1;

                _GameControler._GameObj = obj.transform.parent.gameObject; // Thiết lập đối tượng GameObject để hoán đổi.

                // Kích hoạt lượt cho đối tượng vừa hoán đổi.
                _GameControler.SetMyTurnForObj(_GameControler._GameObj);
                _UIManager.SetImgButtonChoosePick(); // Cập nhật giao diện chọn vũ khí.

                // Hủy tất cả các đối tượng Swap trên màn hình sau khi hoàn tất hoán đổi.
                GameObject[] arrBullet = GameObject.FindGameObjectsWithTag("Swap");
                for (int i = 0; i < arrBullet.Length; i++)
                {
                    Destroy(arrBullet[i]);
                }
                Destroy(gameObject); // Hủy vũ khí Swap sau khi sử dụng.
            }
        }
    }
    #endregion===

    #region================Health===========
    /// <summary>
    /// Kích hoạt chức năng hồi máu (Health) cho người chơi hoặc đối thủ.
    /// </summary>
    public void Pick_Health()
    {
        // Giảm số lượng đạn hồi máu tùy theo người chơi hoặc đối thủ.
        if (_GameControler._GameObj.tag == "Player")
            _GameControler._ListStatePickPlayer[14].Ammo -= 1;
        else
            _GameControler._ListStatePickEnemy[14].Ammo -= 1;

        // Tạo đối tượng Health tại vị trí của người chơi hoặc đối thủ để hồi máu.
        GameObject health = (GameObject)Instantiate(Pick[4], _GameControler._GameObj.transform.position, Quaternion.identity);
        health.transform.parent = _GameControler._GameObj.transform; // Gán đối tượng hồi máu vào người chơi hoặc đối thủ.

        // Nếu đối tượng đang bị trúng độc, hủy bỏ hiệu ứng trúng độc.
        Glow_PoisonControler poison = _GameControler._GameObj.GetComponentInChildren<Glow_PoisonControler>();
        if (poison != null) Destroy(poison.gameObject); // Hủy bỏ đối tượng điều khiển hiệu ứng độc.

        Player player = _GameControler._GameObj.GetComponent<Player>(); // Lấy thông tin Player.
        if (player._IsPoison) player._IsPoison = false; // Nếu Player đang bị trúng độc, xóa bỏ trạng thái trúng độc.

        // Hủy bỏ các đối tượng vũ khí Pick còn lại trên màn hình.
        GameObject[] pick = GameObject.FindGameObjectsWithTag("Pick");
        for (int i = 0; i < pick.Length; i++) Destroy(pick[i]);

        // Đánh dấu hành động hồi máu đã hoàn tất.
        _CheckHit = true;
        _GameControler._CheckHit = true;
        _UIManager.ResetImgPick(); // Đặt lại giao diện chọn vũ khí.
    }
    #endregion==============================
}
