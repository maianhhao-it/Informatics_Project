# BÀI TẬP 1 - TÌM ĐƯỜNG NGẮN NHẤT TRONG KHO HÀNG

## Mô tả

Chương trình tìm đường đi ngắn nhất cho xe vận chuyển trong kho hàng sử dụng thuật toán BFS (Breadth-First Search).

## Yêu cầu hệ thống

- .NET Framework 4.7.2 hoặc cao hơn
- Hoặc .NET Core 3.1+

## Cách chạy

### Biên dịch và chạy

```bash
# Biên dịch với CSC
csc Program.cs

# Chạy chương trình
Program.exe

# Hoặc sử dụng Visual Studio
# Nhấn F5 (Run) hoặc Ctrl+F5 (Run without debugging)
```

## Định dạng Input/Output

### Input (input.txt)

**Mode 1: Tìm đường giữa 2 điểm**

```
<rows> <cols>         // Kích thước bản đồ (rows x cols)
<startX> <startY>     // Tọa độ điểm bắt đầu (x, y)
<endX> <endY>         // Tọa độ điểm đích (x, y)
<map rows x cols>     // Ma trận bản đồ (0 = ô trống, 1 = tường)
```

**Mode 2: Tìm đường qua nhiều điểm**

```
<rows> <cols>         // Kích thước bản đồ (rows x cols)
<số waypoints>        // Số lượng điểm cần đi qua (bao gồm start và end)
<x1> <y1>             // Điểm bắt đầu
<x2> <y2>             // Điểm trung gian 1
...
<xn> <yn>             // Điểm kết thúc
<map rows x cols>     // Ma trận bản đồ (0 = ô trống, 1 = tường)
```

### Output (output.txt)

```
ĐƯỜNG ĐI:
(x1, y1)
(x2, y2)
...
(xn, yn)
Tổng quãng đường: <n>
```

## Test Cases

### Test Case 1: Mode 1 (Hai điểm)

**Input:**

```
6 6
0 0
5 5
0 0 0 1 0 0
0 1 0 1 0 1
0 0 0 0 0 0
1 0 1 1 0 0
0 0 0 0 0 0
0 1 0 1 1 0
```

**Output mong đợi:**

```
ĐƯỜNG ĐI:
0 0
1 0
2 0
2 1
3 1
4 1
4 2
4 3
4 4
4 5
5 5
Tổng quãng đường: 10

```

**Visualization:**

```
S . . . .
. # # # #
E * * * *

S = Start, E = End, # = Wall, * = Path
```

### Test Case 2: Mode 2 (Nhiều điểm)

**Input:**

```
4 6
3
0 0
0 5
3 5
0 0 0 0 0 0
0 1 1 1 1 0
0 0 0 0 1 0
0 0 0 0 0 0
```

**Output mong đợi:**

```
ĐƯỜNG ĐI:
0 0
0 1
0 2
0 3
0 4
0 5
1 5
2 5
3 5
Tổng quãng đường: 8
```

**Visualization:**

```
S * * * * W
. # # # # *
. . . . # *
. . . . . E

S = Start, W = Waypoint, E = End, # = Wall, * = Path
```

## Thuật toán

### BFS (Breadth-First Search)

- **Mục đích:** Tìm đường đi ngắn nhất trên lưới không trọng số
- **Độ phức tạp thời gian:** O(rows × cols)
- **Độ phức tạp không gian:** O(rows × cols)
- **Cấu trúc dữ liệu:** Ma trận 2D, Queue

## Lưu ý

- File `input.txt` phải nằm cùng thư mục với `Program.exe`
- Tọa độ bắt đầu từ (0, 0)
- Kích thước tối đa: 1000×1000
- Số waypoints tối đa: 1000
- Chương trình tự động tạo file `output.txt` sau khi chạy

## Xử lý lỗi

Chương trình sẽ thông báo lỗi khi:

- File input không tồn tại
- Format input không đúng
- Tọa độ nằm ngoài bản đồ
- Điểm start/end/waypoint nằm trên tường
- Không tìm thấy đường đi
