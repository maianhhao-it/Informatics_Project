# BÀI TẬP 2 - QUẢN LÝ HÀNG HÓA THEO FIFO TRONG KHO

## Mô tả

Hệ thống quản lý kho hàng 3D với các chức năng:

- Thêm hàng hóa vào kho
- Xuất hàng theo nguyên tắc FIFO (First In - First Out)
- Xuất hàng theo độ ưu tiên
- Tìm kiếm hàng hóa theo mã
- Thống kê theo loại hàng
- Lưu/đọc dữ liệu từ file

## Cách chạy

### Yêu cầu

- NET Framework 4.7.2+ và .NET SDK 6.0 trở lên
- Visual Studio 2022 hoặc Visual Studio Code (tùy chọn)

### Biên dịch và chạy

```bash
# Biên dịch
csc Program.cs

# Chạy
Program.exe
```

Hoặc dùng dotnet:

```bash
dotnet run
```

## Định dạng Input/Output

### 1. Khởi tạo kho

**Input từ console:**

```
Nhap kich thuoc kho (X Y Z): 5 4 3
```

- `X`: Chiều dài (số hàng)
- `Y`: Chiều rộng (số cột)
- `Z`: Chiều cao (số tầng)

### 2. Menu chức năng

```
=== MENU ===
1. Them hang
2. Lay FIFO
3. Lay hang theo uu tien
4. Tim theo ma
5. In kho
6. Thong ke theo loai
7. Luu file
8. Doc file
9. Thoat
```

### 3. Thêm hàng (Chọn 1)

**Input:**

```
Nhap: Ma SL Loai UuTien NgayNhap
> H001 100 DienTu 1 2024-01-15
```

**Các trường:**

- `Ma`: Mã hàng (string, không trùng)
- `SL`: Số lượng (int)
- `Loai`: Loại hàng (string, không có dấu cách)
- `UuTien`: Độ ưu tiên (int, số càng nhỏ càng ưu tiên cao)
- `NgayNhap`: Ngày nhập (yyyy-MM-dd hoặc yyyy-MM-dd HH:mm:ss)

**Output:**

```
(Thành công - không có thông báo)
```

Hoặc:

```
Ma hang H001 da ton tai!
```

```
Kho da day!
```

### 4. Xuất hàng FIFO (Chọn 2)

**Output:**

```
Xuat: H001
```

Hoặc:

```
Kho rong!
```

### 5. Xuất hàng theo ưu tiên (Chọn 3)

**Input:**

```
Nhap loai (de trong neu ko loc): DienTu
```

hoặc để trống để xuất tất cả các loại

**Output:**

```
Xuat: H002
```

Hoặc:

```
Khong co hang phu hop!
```

### 6. Tìm theo mã (Chọn 4)

**Input:**

```
Nhap ma: H001
```

**Output:**

```
Tim thay H001 o [2,1,0]
```

Hoặc:

```
Ko tim thay
```

### 7. In kho (Chọn 5)

**Output:**

```
=== TINH TRANG KHO ===
Kich thuoc: 3x4x3
So luong hang: 3
O trong: 57

--- DANH SACH HANG (sap xep theo NgayNhap) ---
[0,0,0] H001     | SL: 100 | Loai:DienTu      | Uu tien:1 | NgayNhap:15/01/2024 00:00:00
[0,0,1] H002     | SL: 200 | Loai:GiaDung     | Uu tien:2 | NgayNhap:16/01/2024 00:00:00
[0,0,2] H003     | SL:  50 | Loai:DienTu      | Uu tien:1 | NgayNhap:17/01/2024 00:00:00
```

### 8. Thống kê theo loại (Chọn 6)

**Output:**

```
=== THONG KE THEO LOAI ===
DienTu: 2 items, Tong SL = 150
GiaDung: 1 items, Tong SL = 200
```

### 9. Lưu file (Chọn 7)

**Input:**

```
Ten file: kho_backup.txt
```

**Output:**

```
Da luu file!
```

**Định dạng file lưu:**

```
5 4 3
3
H001 100 DienTu 1 0 0 0 2024-01-15T00:00:00.0000000
H002 200 GiaDung 2 0 0 1 2024-01-16T00:00:00.0000000
H003 50 DienTu 1 0 0 2 2024-01-17T00:00:00.0000000

```

- Dòng 1: Kích thước kho (X Y Z)
- Dòng 2: Số lượng hàng
- Các dòng tiếp: Ma SL Loai UuTien X Y Z NgayNhap

### 10. Đọc file (Chọn 8)

**Input:**

```
Ten file: kho_backup.txt
```

**Output:**

```
Doc file thanh cong: kho_backup.txt(3 items)
```

## LƯU Ý

1. Mã hàng phải duy nhất - Không thể thêm 2 hàng cùng mã
2. Định dạng ngày - Nhập theo format: YYYY-MM-DD hoặc YYYY-MM-DDTHH:mm:ss
3. Độ ưu tiên - Số càng nhỏ càng ưu tiên cao
4. Ô trống - Được quản lý theo FIFO, hàng mới sẽ vào ô trống sớm nhất
5. input.txt, output.txt và kho_backup.txt sẽ lưu "\bin\debug"
