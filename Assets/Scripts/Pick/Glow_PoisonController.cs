using UnityEngine;
using System.Collections;

/*
_TimeCount được tăng lên mỗi frame dựa trên thời gian thực tế đã trôi qua kể từ lần cập nhật trước.

Nếu _TimeCount vượt quá 0.5 giây, hệ thống sẽ reset lại biến đếm và chọn ngẫu nhiên một vị trí trong
mảng _PosParticle để sinh ra một hiệu ứng hạt nhỏ hình đầu lâu (_Particle_skull_small).

Hiệu ứng hạt được tạo tại vị trí ngẫu nhiên trong trò chơi nhằm mô phỏng hiệu ứng chất độc hoặc
hiệu ứng hình ảnh liên quan đến chất độc.
*/
public class Glow_PoisonController : MonoBehaviour
{

    public GameObject _Particle_skull_small; // Đối tượng hiệu ứng hạt (particle) hình đầu lâu nhỏ
    public Transform[] _PosParticle; // Mảng vị trí để sinh ra các hiệu ứng hạt
    float _TimeCount = 0; // Biến đếm thời gian để kiểm soát tần suất tạo hạt

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        // Tăng biến đếm thời gian với khoảng thời gian trôi qua của mỗi frame
        _TimeCount += Time.deltaTime;

        // Nếu thời gian đếm vượt quá 0.5 giây, tiến hành tạo hiệu ứng hạt
        if (_TimeCount > 0.5f)
        {
            _TimeCount = 0; // Reset thời gian đếm
            int ran = Random.Range(0, _PosParticle.Length); // Lấy một vị trí ngẫu nhiên từ mảng _PosParticle
            // Tạo hiệu ứng hạt (đối tượng đầu lâu nhỏ) tại vị trí ngẫu nhiên
            Instantiate(_Particle_skull_small, _PosParticle[ran].transform.position, Quaternion.identity);
        }
    }
}
