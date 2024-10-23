using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public GameObject[] _ListPick; // Mảng lưu trữ danh sách vũ khí có thể chọn
    GameControler _GameControler; // Đối tượng quản lý chính game
    public LayerMask collisionMask; // Lớp mặt nạ để kiểm tra va chạm (raycast)

    // Hàm khởi tạo, chạy khi game bắt đầu
    void Start()
    {
        // Tìm và lưu đối tượng điều khiển game
        _GameControler = FindObjectOfType<GameControler>();
    }

    // Update is called once per frame
    void Update()
    {
    }
    /// <summary>
    /// Thực hiện khi người chơi chọn vào một vũ khí nào đó
    /// </summary>
    public void UsePick(int index)
    {
        // Xóa tất cả các đối tượng vũ khí (Pick) cũ trên màn hình
        GameObject[] arrPickOld = GameObject.FindGameObjectsWithTag("Pick");
        for (int i = 0; i < arrPickOld.Length; i++)
        {
            Pick_ShieldControler pick = arrPickOld[i].GetComponent<Pick_ShieldControler>();
            Pick_MineControler pickMine = arrPickOld[i].GetComponent<Pick_MineControler>();
            // Kiểm tra nếu vũ khí không phải loại "Pick_Shield" hay "Pick_Mine" thì xóa đối tượng
            if (pick == null && pickMine == null) Destroy(arrPickOld[i]);
            // Nếu là loại Pick_Mine, nhưng chưa được khởi tạo hoàn toàn thì cũng xóa
            if (pickMine != null && !pickMine._IsInit)
            {
                Destroy(arrPickOld[i]);
            }
        }

        // Xóa tất cả các đối tượng đạn (Bullet) hiện có trên màn hình
        GameObject[] arrBullet = GameObject.FindGameObjectsWithTag("Bullet");
        for (int i = 0; i < arrBullet.Length; i++) { Destroy(arrBullet[i]); }

        // Kiểm tra và xóa đối tượng Dart nếu có
        GameObject dart = GameObject.FindGameObjectWithTag("Dart");
        if (dart != null) Destroy(dart);

        //======================
        // Bắt đầu xử lý việc chọn vũ khí cho người chơi
        Player player = new Player();
        Object obj = new Object();

        // Lưu vũ khí được chọn vào biến _Pick của GameControler
        _GameControler._Pick = _ListPick[index];

        // Lấy đối tượng Player hiện tại (được chọn trong lượt của người chơi)
        player = _GameControler._GameObj.GetComponent<Player>();

        // Nếu vũ khí là các loại đặc biệt (bom, dịch chuyển, hồi máu, đổi chỗ) thì tạo đối tượng vũ khí đặc biệt
        if (index == 3 || index == 9 || index == 14 || index == 15)
        {
            obj = Instantiate(_ListPick[18], _GameControler._GameObj.transform.position, Quaternion.identity);
            AirCallControler air = ((GameObject)obj).GetComponent<AirCallControler>();
            switch (index)
            {
                case 3: air._Type = "Pick_Bomb"; break; // Bom
                case 9: air._Type = "Pick_Teleport"; break; // Dịch chuyển
                case 14: air._Type = "Pick_Health"; break; // Hồi máu
                case 15: air._Type = "Pick_Swap"; break; // Đổi chỗ
            }
        }
        else
        {
            // Nếu vũ khí không đặc biệt, tạo đối tượng vũ khí thông thường tại vị trí của Player
            FindObjectOfType<CameraControler>()._IsMove = false;
            FindObjectOfType<MapControler>()._IsMove = false;
            obj = Instantiate(_ListPick[index], _GameControler._GameObj.transform.position, Quaternion.identity);
        }

        // Điều chỉnh hướng của vũ khí theo hướng của người chơi
        if (player._Dir == Player.Dir.right)
        {
            ((GameObject)obj).transform.localScale = new Vector3(1, 1, 1); // Hướng phải
        }
        else
        {
            ((GameObject)obj).transform.localScale = new Vector3(-1, 1, 1); // Hướng trái
        }

        // Gắn đối tượng vũ khí vừa tạo vào đối tượng người chơi
        ((GameObject)obj).transform.parent = _GameControler._GameObj.transform;
        // Hiển thị súng hoặc vũ khí trên người chơi
    }
    /// <summary>
    /// Hàm kiểm tra vị trí khởi tạo người chơi có hợp lệ không (có đứng trên bề mặt không)
    /// </summary>
    public bool CheckInitPlayer(Vector3 target)
    {
        // Kiểm tra dưới vị trí khởi tạo xem có bề mặt (map) không
        float directionX = Mathf.Sign((target - transform.position).x); // Xác định hướng kiểm tra
        for (int i = 0; i < 4; i++)
        {
            Vector2 rayOrigin = target; // Tọa độ bắt đầu của tia raycast
            float rayLengthHor = 10; // Chiều dài của tia ray
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, new Vector3(0, -1, 0), rayLengthHor, collisionMask); // Phát tia ray xuống dưới
            Debug.DrawRay(rayOrigin, new Vector3(0, 1, 0) * rayLengthHor, Color.red); // Vẽ tia ray để debug

            // Nếu có va chạm với bề mặt thì vị trí hợp lệ
            if (hit)
            {
                return true;
            }
        }
        return false; // Nếu không có va chạm, vị trí không hợp lệ
    }

    /// <summary>
    /// Hàm kiểm tra khoảng cách giữa người chơi và các vị trí khác, tránh khởi tạo quá gần
    /// </summary>
    public bool CheckPosXPlayer(float posX, List<float> listX, float dis)
    {
        // Kiểm tra vị trí X của người chơi có quá gần các vị trí đã có không
        for (int i = 0; i < listX.Count; i++)
        {
            // Nếu vị trí posX nằm trong khoảng cách dis từ một vị trí khác thì không hợp lệ
            if (posX > listX[i] - dis && posX < listX[i] + dis)
            {
                return false;
            }
        }
        return true; // Nếu không quá gần các vị trí khác, vị trí hợp lệ
    }

    /// <summary>
    /// Kiểm tra vị trí có thể dịch chuyển người chơi tới (Check Teleport)
    /// </summary>
    public bool CheckTeleportPlayer(Vector3 target)
    {
        // Kiểm tra vị trí có thể dịch chuyển người chơi tới
        float directionX = Mathf.Sign((target - transform.position).x); // Xác định hướng kiểm tra
        for (int i = 0; i < 4; i++)
        {
            Vector2 rayOrigin = target; // Tọa độ bắt đầu của tia ray
            float rayLengthHor = 10; // Chiều dài của tia ray

            // Kiểm tra va chạm bề mặt theo nhiều hướng (dưới, trái, phải, trên)
            RaycastHit2D hitD = Physics2D.Raycast(rayOrigin, new Vector3(0, -1, 0), rayLengthHor, collisionMask); // Dưới
            Debug.DrawRay(rayOrigin, new Vector3(0, 1, 0) * rayLengthHor, Color.red); // Vẽ tia ray
            RaycastHit2D hitL = Physics2D.Raycast(rayOrigin, new Vector3(-1, 0, 0), rayLengthHor, collisionMask); // Trái
            Debug.DrawRay(rayOrigin, new Vector3(-1, 0, 0) * rayLengthHor, Color.red); // Vẽ tia ray
            RaycastHit2D hitR = Physics2D.Raycast(rayOrigin, new Vector3(1, 0, 0), rayLengthHor, collisionMask); // Phải
            Debug.DrawRay(rayOrigin, new Vector3(1, 0, 0) * rayLengthHor, Color.red); // Vẽ tia ray
            RaycastHit2D hitT = Physics2D.Raycast(rayOrigin, new Vector3(0, 1, 0), rayLengthHor, collisionMask); // Trên
            Debug.DrawRay(rayOrigin, new Vector3(0, 1, 0) * rayLengthHor, Color.red); // Vẽ tia ray

            // Nếu tất cả các tia ray đều va chạm với bề mặt thì vị trí hợp lệ
            if (hitD && hitL && hitR && hitT)
            {
                return true;
            }
        }
        return false; // Nếu không, vị trí không hợp lệ
    }
}