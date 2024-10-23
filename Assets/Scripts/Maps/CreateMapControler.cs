using UnityEngine;
using System.Collections;
/*
Giải thích:
Script này điều chỉnh vị trí của đối tượng trên bản đồ sao cho đối tượng luôn nằm trên
các tọa độ lưới có kích thước xác định. Điều này rất hữu ích khi làm việc với các bản đồ
dạng lưới, giúp việc canh chỉnh đối tượng dễ dàng hơn.
*/

[ExecuteInEditMode]
public class CreateMapControler : MonoBehaviour
{

    public float cell_size = 1f; // Kích thước của mỗi ô trong lưới
    private float x, y, z; // Các biến lưu tọa độ của đối tượng trên bản đồ

    void Start()
    {
        // Khởi tạo tọa độ ban đầu cho đối tượng
        x = 0f;
        y = 0f;
        z = 0f;
    }

    void Update()
    {
        // Làm tròn tọa độ của đối tượng theo kích thước ô lưới (cell_size) để đảm bảo đối tượng
        // luôn nằm tại trung tâm của một ô lưới
        x = Mathf.Round(transform.position.x / cell_size) * cell_size;
        y = Mathf.Round(transform.position.y / cell_size) * cell_size;
        z = transform.position.z;
        // Cập nhật vị trí mới của đối tượng
        transform.position = new Vector3(x, y, z);
    }
}