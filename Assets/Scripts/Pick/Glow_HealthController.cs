using UnityEngine;
using System.Collections;

public class Glow_HealthController : MonoBehaviour
{

    public GameObject particle_cross; // Đối tượng hiệu ứng hạt (particle) để tạo hiệu ứng khi mất máu
    public Transform[] _PosParticle; // Mảng vị trí để tạo hiệu ứng hạt
    float _TimeCount = 0; // Biến đếm thời gian cho việc sinh hiệu ứng hạt
    float _TimeAddHealth = 0; // Biến đếm thời gian để cộng thêm máu cho nhân vật
    int _Health = 70; // Lượng máu còn lại của đối tượng Glow
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        // Tăng thời gian đếm cho việc sinh hiệu ứng hạt
        _TimeCount += Time.deltaTime;

        // Nếu đã quá 0.5 giây, tiến hành sinh một hạt tại một vị trí ngẫu nhiên
        if (_TimeCount > 0.5f)
        {
            _TimeCount = 0; // Reset thời gian đếm
            int ran = Random.Range(0, _PosParticle.Length); // Lấy vị trí ngẫu nhiên từ mảng _PosParticle
            Instantiate(particle_cross, _PosParticle[ran].transform.position, Quaternion.identity); // Tạo hiệu ứng hạt tại vị trí ngẫu nhiên
        }

        // Tăng thời gian đếm cho việc cộng máu
        _TimeAddHealth += Time.deltaTime;

        // Nếu đã quá 0.2 giây và đối tượng còn máu, tiến hành cộng máu
        if (_TimeAddHealth > 0.2f && _Health > 0)
        {
            _TimeAddHealth = 0; // Reset thời gian đếm
            _Health -= 2; // Giảm 2 máu của đối tượng Glow
            Player player = gameObject.GetComponentInParent<Player>(); // Lấy đối tượng Player cha của đối tượng Glow
            player._Health += 2; // Cộng thêm 2 máu cho nhân vật
            if (player._Health > 100)
            {
                player._Health = 100; // Đảm bảo máu của nhân vật không vượt quá 100
            }

            // Cập nhật giao diện hiển thị số máu của nhân vật
            NumberText textHealth = player.gameObject.GetComponentInChildren<NumberText>();
            textHealth.SetNumberText(player._Health); // Hiển thị số máu hiện tại

            // Kiểm tra nếu Glow hết máu
            GameController gameController = FindObjectOfType<GameController>();
            if (_Health <= 0)
            {
                // Nếu đối tượng Glow thuộc quân địch, chuyển lượt cho người chơi
                if (gameController._GameObj.tag == "Enemy")
                {
                    gameController.ChangeTurn(); // Chuyển lượt chơi
                }
                Destroy(gameObject); // Hủy đối tượng Glow khi hết máu
            }
        }
    }
}
