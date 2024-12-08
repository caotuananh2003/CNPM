using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

// Lớp Pick: Đại diện cho một loại vật phẩm hoặc vũ khí trong trò chơi.
// Lớp này lưu trữ các thông tin liên quan đến vật phẩm, bao gồm chỉ số, số lượng đạn (ammo), và trạng thái.
public class Pick
{
    // Thuộc tính Index: Chỉ số của vật phẩm hoặc vũ khí.
    // Ví dụ: Index có thể được sử dụng để xác định loại vũ khí trong danh sách các vật phẩm.
    public int Index { get; set; }

    // Thuộc tính Ammo: Số lượng đạn (hoặc số lần sử dụng) còn lại của vật phẩm/vũ khí.
    // Giá trị này có thể giảm khi người chơi sử dụng vật phẩm/vũ khí.
    public int Ammo { get; set; }

    // Thuộc tính State: Trạng thái hoạt động của vật phẩm/vũ khí.
    // - `true`: Vật phẩm/vũ khí đang ở trạng thái hoạt động hoặc sẵn sàng sử dụng.
    // - `false`: Vật phẩm/vũ khí không thể sử dụng.
    public bool State { get; set; }

    // Phương thức khởi tạo mặc định: Dùng để tạo một đối tượng Pick với giá trị ban đầu là 0/null.
    public Pick() {}

    // Phương thức khởi tạo có tham số:
    // Dùng để khởi tạo một đối tượng Pick với các giá trị cụ thể.
    // - `_index`: Giá trị của thuộc tính Index.
    // - `_ammo`: Giá trị của thuộc tính Ammo.
    // - `_state`: Giá trị của thuộc tính State.
    public Pick(int _index, int _ammo, bool _state)
    {
        // Gán giá trị cho các thuộc tính của đối tượng.
        this.Index = _index; // Chỉ số của vật phẩm/vũ khí.
        this.State = _state; // Trạng thái sử dụng của vật phẩm/vũ khí.
        this.Ammo = _ammo;   // Số lượng đạn còn lại.
    }
}
