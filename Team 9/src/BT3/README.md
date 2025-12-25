# BÀI TẬP 3 - HỆ THỐNG CẢNH BÁO NHIỆT ĐỘ KHO LẠNH

Hệ thống giám sát nhiệt độ kho lạnh theo thời gian thực với cảnh báo tự động.

---

## CÁCH CHẠY

### Yêu cầu

- NET Framework 4.7.2+ và .NET SDK 6.0 trở lên
- Visual Studio 2022 hoặc Visual Studio Code (tùy chọn)

### Biên dịch & Thực thi

```bash
# Biên dịch
csc Program.cs

# Chạy
Program.exe
```

Hoặc dùng .NET CLI:

```bash
dotnet run
```

**Lưu ý**:

- Chương trình tự động đọc `input.txt` và ghi kết quả vào `output.txt`
- File `input.txt` và `output.txt` sẽ nằm trong thư mục `bin\Debug\` (hoặc `bin\Release\`)

---

## ĐỊNH DẠNG I/O

### INPUT (file `input.txt`)

```
2 2
3 3
3 2
0 0 0 -10.0
0 0 1 -9.0
0 1 0 -11.0
0 1 1 -5.0
1 0 0 -9.0
...
```

**Cấu trúc:**

#### Dòng 1: Kích thước lưới

```
2 2
```

- `2 2` = 2 hàng × 2 cột = 4 cảm biến

#### Dòng 2 đến (rows + 1): Ma trận ngưỡng lỗi

```
3 3
3 2
```

- Ma trận 2×2
- Mỗi số = ngưỡng lỗi cho cảm biến tại vị trí đó
- **Ngưỡng lỗi** = số chu kỳ liên tiếp nhiệt độ không đổi để coi là hư
- VD: Cảm biến (1,1) có ngưỡng = 2

#### Các dòng tiếp: Dữ liệu nhiệt độ

```
0 0 0 -10.0
```

Format: `Cycle Row Col Temperature`

- `Cycle`: Chu kỳ đo (0, 1, 2, ...)
- `Row`: Vị trí hàng (0-indexed)
- `Col`: Vị trí cột (0-indexed)
- `Temperature`: Nhiệt độ (°C)

**Ví dụ:** `4 0 0 2.0` = Chu kỳ 4, sensor (0,0), nhiệt độ 2.0°C

---

### OUTPUT (file `output.txt`)

#### Có cảnh báo:

```
Chu ky 2: Canh bao: Cam bien S11 (1,1) bi hu (khong doi 2 chu ky)
Chu ky 3: Khu vuc (0,0) tang dot ngot 4.3C [Sensors: S00, S01, S10, S11]
Chu ky 4: Nhiet do cao tai khu vuc (0,0) avg=0.25C [Sensors: S00, S01, S10, S11]
Chu ky 4: Khu vuc (0,0) tang dot ngot 3.8C [Sensors: S00, S01, S10, S11]
```

#### Không có cảnh báo:

```
Khong co canh bao
```

**Các loại cảnh báo:**

1. **Cảm biến hư** - Không đổi nhiệt độ

   ```
   Chu ky X: Canh bao: Cam bien SXY (row,col) bi hu (khong doi N chu ky)
   ```

   - Xảy ra khi: |T_mới - T_cũ| < 0.01 liên tiếp ≥ ngưỡng

2. **Nhiệt độ cao** - Khu vực 2×2 nóng

   ```
   Chu ky X: Nhiet do cao tai khu vuc (row,col) avg=Y.YYC [Sensors: ...]
   ```

   - Xảy ra khi: Trung bình 4 cảm biến > 0°C

3. **Tăng đột ngột** - Nhiệt độ tăng nhanh
   ```
   Chu ky X: Khu vuc (row,col) tang dot ngot Y.YC [Sensors: ...]
   ```
   - Xảy ra khi: Tăng ≥ 3.0°C so với chu kỳ trước

---

## CẤU TRÚC DỮ LIỆU

### 1. Message

```csharp
string SensorId      // ID cảm biến (vd: "S01")
int Cycle            // Chu kỳ đo
int Row, Col         // Vị trí
double Temperature   // Nhiệt độ
```

### 2. Sensor

```csharp
double NhietDoHienTai    // Nhiệt độ hiện tại
int DemKhongDoi          // Đếm chu kỳ không đổi
int NguongLoi            // Ngưỡng báo lỗi
bool DaHu                // Trạng thái hư
```

### 3. ColdStorage

```csharp
Sensor[,] sensorGrid            // Ma trận cảm biến
Queue<Message> hangDoiTinNhan   // Hàng đợi tin nhắn
List<string> danhSachCanhBao    // Danh sách cảnh báo
double[,] avgTruoc2x2           // Lưu TB chu kỳ trước
```

---

## VÍ DỤ

### Lưới 2×2 qua 5 chu kỳ

| Chu kỳ | S00 | S01 | S10 | S11 | avg   | Cảnh báo                |
| ------ | --- | --- | --- | --- | ----- | ----------------------- |
| 0      | -10 | -9  | -11 | -5  | -8.75 | -                       |
| 1      | -9  | -10 | -10 | -5  | -8.5  | -                       |
| 2      | -8  | -9  | -9  | -5  | -7.75 | **S11 hư**              |
| 3      | -3  | -2  | -4  | -5  | -3.5  | **Tăng +4.25°C**        |
| 4      | +2  | +3  | +1  | -5  | +0.25 | **Nóng + Tăng +3.75°C** |

**Giải thích:**

- Chu kỳ 2: S11 không đổi (-5°C) 2 lần → đủ ngưỡng 2 → Báo hư
- Chu kỳ 3: avg tăng từ -7.75 lên -3.5 (+4.25°C ≥ 3) → Báo tăng
- Chu kỳ 4: avg = +0.25°C > 0 → Báo nóng + Tăng +3.75°C → Báo tăng

---

## ĐỘ PHỨC TẠP

| Thao tác        | Độ phức tạp |
| --------------- | ----------- |
| ThemMessage     | O(1)        |
| ProcessOneCycle | O(R×C)      |
| Toàn bộ         | O(N×R×C)    |
| Không gian      | O(R×C)      |

**Trong đó:** N = số chu kỳ, R = số hàng, C = số cột

---
