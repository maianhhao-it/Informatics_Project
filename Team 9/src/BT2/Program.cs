using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
namespace BT2
{

    // Vi tri 3D cua tung hang
    public struct ViTri
    {
        public int X;
        public int Y;
        public int Z;

        public ViTri(int a, int b, int c)
        {
            X = a;
            Y = b;
            Z = c;
        }
    }
    // Doi tuong hang
    public class HangHoa
    {
        public string Ma;
        public int SoLuong;
        public string Loai;
        public int UuTien;
        public DateTime NgayNhap;

        public HangHoa(string ma, int sl, string loai, int p, DateTime t)
        {
            Ma = ma;
            SoLuong = sl;
            Loai = loai;
            UuTien = p;
            NgayNhap = t;
        }
    }
    // Kho 3D
    public class KhoHang
    {
        private HangHoa[,,] kho;
        private Dictionary<string, ViTri> viTriTheoMa = new Dictionary<string, ViTri>();
        private Queue<ViTri> danhSachOTrong = new Queue<ViTri>();
        private int xSize, ySize, zSize;

        public KhoHang(int x, int y, int z)
        {
            xSize = x;
            ySize = y;
            zSize = z;

            kho = new HangHoa[x, y, z];

            for (int a = 0; a < x; a++)
                for (int b = 0; b < y; b++)
                    for (int c = 0; c < z; c++)
                        danhSachOTrong.Enqueue(new ViTri(a, b, c));
        }

        public bool ThemHang(HangHoa h)
        {
            if (viTriTheoMa.ContainsKey(h.Ma))
            {
                Console.WriteLine("Ma hang " + h.Ma + " da ton tai!");
                return false;
            }

            if (danhSachOTrong.Count == 0)
            {
                Console.WriteLine("Kho da day!");
                return false;
            }

            var o = danhSachOTrong.Dequeue();
            kho[o.X, o.Y, o.Z] = h;
            viTriTheoMa[h.Ma] = o;
            return true;
        }

        public HangHoa LayFIFO()
        {
            if (viTriTheoMa.Count == 0)
                return null;

            List<HangHoa> danhSach = new List<HangHoa>();
            foreach (var cap in viTriTheoMa)
            {
                var vt = cap.Value;
                danhSach.Add(kho[vt.X, vt.Y, vt.Z]);
            }

            HangHoa cuNhat = danhSach[0];
            for (int i = 1; i < danhSach.Count; i++)
                if (danhSach[i].NgayNhap < cuNhat.NgayNhap)
                    cuNhat = danhSach[i];

            var vitri = viTriTheoMa[cuNhat.Ma];
            kho[vitri.X, vitri.Y, vitri.Z] = null;
            viTriTheoMa.Remove(cuNhat.Ma);
            danhSachOTrong.Enqueue(vitri);

            return cuNhat;
        }

        public HangHoa LayUuTien(string loai)
        {
            if (viTriTheoMa.Count == 0) return null;

            List<HangHoa> tam = new List<HangHoa>();

            foreach (var key in viTriTheoMa.Keys)
            {
                var vt = viTriTheoMa[key];
                var item = kho[vt.X, vt.Y, vt.Z];
                if (string.IsNullOrEmpty(loai) || item.Loai == loai)
                    tam.Add(item);
            }

            if (tam.Count == 0) return null;

            HangHoa chon = tam[0];
            for (int i = 1; i < tam.Count; i++)
            {
                if (tam[i].UuTien < chon.UuTien)
                    chon = tam[i];
                else if (tam[i].UuTien == chon.UuTien && tam[i].NgayNhap < chon.NgayNhap)
                    chon = tam[i];
            }

            var vitri = viTriTheoMa[chon.Ma];
            kho[vitri.X, vitri.Y, vitri.Z] = null;
            viTriTheoMa.Remove(chon.Ma);
            danhSachOTrong.Enqueue(vitri);

            return chon;
        }

        public HangHoa TimMa(string ma)
        {
            if (!viTriTheoMa.ContainsKey(ma)) return null;
            var vt = viTriTheoMa[ma];
            return kho[vt.X, vt.Y, vt.Z];
        }

        public ViTri? TimViTri(string ma)
        {
            if (viTriTheoMa.ContainsKey(ma))
                return viTriTheoMa[ma];
            return null;
        }

        public void ThongKeLoai()
        {
            if (viTriTheoMa.Count == 0)
            {
                Console.WriteLine("Kho dang trong!");
                return;
            }

            Dictionary<string, int> soItem = new Dictionary<string, int>();
            Dictionary<string, int> soLuong = new Dictionary<string, int>();

            foreach (var ma in viTriTheoMa.Keys)
            {
                var vt = viTriTheoMa[ma];
                var item = kho[vt.X, vt.Y, vt.Z];

                if (!soItem.ContainsKey(item.Loai))
                {
                    soItem[item.Loai] = 1;
                    soLuong[item.Loai] = item.SoLuong;
                }
                else
                {
                    soItem[item.Loai]++;
                    soLuong[item.Loai] += item.SoLuong;
                }
            }

            Console.WriteLine("\n=== THONG KE THEO LOAI ===");
            foreach (var loai in soItem.Keys)
            {
                Console.WriteLine(loai + ": " + soItem[loai] + " items, Tong SL = " + soLuong[loai]);
            }
        }

        public void InKho()
        {
            Console.WriteLine("\n=== TINH TRANG KHO ===");
            Console.WriteLine("Kich thuoc: " + xSize + "x" + ySize + "x" + zSize);
            Console.WriteLine("So luong hang: " + viTriTheoMa.Count);
            Console.WriteLine("O trong: " + danhSachOTrong.Count);

            if (viTriTheoMa.Count > 0)
            {
                Console.WriteLine("\n--- DANH SACH HANG (sap xep theo NgayNhap) ---");
                List<HangHoa> dsSapXep = new List<HangHoa>();
                foreach (var ma in viTriTheoMa.Keys)
                {
                    var vt = viTriTheoMa[ma];
                    dsSapXep.Add(kho[vt.X, vt.Y, vt.Z]);
                }
                dsSapXep.Sort((a, b) => a.NgayNhap.CompareTo(b.NgayNhap));
                foreach (var h in dsSapXep)
                {
                    var vt = viTriTheoMa[h.Ma];
                    Console.WriteLine($"[{vt.X},{vt.Y},{vt.Z}] {h.Ma,-8} | SL:{h.SoLuong,4} | Loai:{h.Loai,-12} | Uu tien:{h.UuTien} | NgayNhap:{h.NgayNhap:dd/MM/yyyy HH:mm:ss}");
                }
            }
        }

        public void LuuFile(string ten)
        {
            try
            {
                using (var w = new StreamWriter(ten))
                {
                    w.WriteLine(xSize + " " + ySize + " " + zSize);
                    w.WriteLine(viTriTheoMa.Count);

                    foreach (var ma in viTriTheoMa.Keys)
                    {
                        var vt = viTriTheoMa[ma];
                        var h = kho[vt.X, vt.Y, vt.Z];
                        w.WriteLine($"{h.Ma} {h.SoLuong} {h.Loai} {h.UuTien} {vt.X} {vt.Y} {vt.Z} {h.NgayNhap:o}");
                    }
                }
                Console.WriteLine("Da luu file!");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Loi khi luu file: " + ex.Message);
            }
        }

        public static KhoHang DocFile(string ten)
        {
            try
            {
                string[] lines = File.ReadAllLines(ten);

                string[] size = lines[0].Split(' ');
                int x = int.Parse(size[0]);
                int y = int.Parse(size[1]);
                int z = int.Parse(size[2]);

                KhoHang k = new KhoHang(x, y, z);

                int so = int.Parse(lines[1]);

                for (int i = 0; i < so; i++)
                {
                    string[] p = lines[i + 2].Split(' ');

                    string ma = p[0];
                    int sl = int.Parse(p[1]);
                    string loai = p[2];
                    int uu = int.Parse(p[3]);
                    int vx = int.Parse(p[4]);
                    int vy = int.Parse(p[5]);
                    int vz = int.Parse(p[6]);
                    DateTime t = DateTime.Parse(p[7]);

                    HangHoa h = new HangHoa(ma, sl, loai, uu, t);

                    k.kho[vx, vy, vz] = h;
                    k.viTriTheoMa[ma] = new ViTri(vx, vy, vz);
                }

                k.danhSachOTrong.Clear();
                for (int a = 0; a < x; a++)
                    for (int b = 0; b < y; b++)
                        for (int c = 0; c < z; c++)
                            if (k.kho[a, b, c] == null)
                                k.danhSachOTrong.Enqueue(new ViTri(a, b, c));
                Console.WriteLine("Doc file thanh cong: " + ten + "(" + k.viTriTheoMa.Count + " items)");
                return k;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Loi khi doc file: " + ex.Message);
                return null;
            }
        }
    }

    class Program
    {
        static void Main()
        {
            Console.WriteLine("=== HE THONG QUAN LY KHO HANG ===\n");
            int x = 0, y = 0, z = 0;
            while (true)
            {
                Console.Write("Nhap kich thuoc kho (X Y Z): ");
                try
                {
                    var arr = Console.ReadLine().Split(' ');
                    x = int.Parse(arr[0]);
                    y = int.Parse(arr[1]);
                    z = int.Parse(arr[2]);
                    if (x > 0 && y > 0 && z > 0) break;
                    else Console.WriteLine("Phai nhap so nguyen > 0.");
                }
                catch
                {
                    Console.WriteLine("Nhap sai dinh dang, thu lai.");
                }
            }

            KhoHang k = new KhoHang(x, y, z);

            while (true)
            {
                Console.WriteLine("\n=== MENU ===");
                Console.WriteLine("1. Them hang");
                Console.WriteLine("2. Lay FIFO");
                Console.WriteLine("3. Lay hang theo uu tien");
                Console.WriteLine("4. Tim theo ma");
                Console.WriteLine("5. In kho");
                Console.WriteLine("6. Thong ke theo loai");
                Console.WriteLine("7. Luu file");
                Console.WriteLine("8. Doc file");
                Console.WriteLine("9. Thoat");

                Console.Write("Chon: ");
                string ch = Console.ReadLine();

                try
                {
                    switch (ch)
                    {
                        case "1":
                            Console.Write("Nhap: Ma SL Loai UuTien NgayNhap\n> ");
                            var t = Console.ReadLine().Split(' ');
                            HangHoa h = new HangHoa(
                                t[0],
                                int.Parse(t[1]),
                                t[2],
                                int.Parse(t[3]),
                                DateTime.Parse(t[4])
                            );
                            k.ThemHang(h);
                            break;

                        case "2":
                            var f = k.LayFIFO();
                            if (f == null) Console.WriteLine("Kho rong!");
                            else Console.WriteLine("Xuat: " + f.Ma);
                            break;

                        case "3":
                            Console.Write("Nhap loai (de trong neu ko loc): ");
                            string lo = Console.ReadLine();
                            var p = k.LayUuTien(lo);
                            if (p == null) Console.WriteLine("Khong co hang phu hop!");
                            else Console.WriteLine("Xuat: " + p.Ma);
                            break;

                        case "4":
                            Console.Write("Nhap ma: ");
                            var mm = Console.ReadLine();
                            var tim = k.TimMa(mm);
                            if (tim == null) Console.WriteLine("Ko tim thay");
                            else
                            {
                                var vt = k.TimViTri(mm);
                                Console.WriteLine("Tim thay " + tim.Ma + " o [" + vt?.X + "," + vt?.Y + "," + vt?.Z + "]");
                            }
                            break;

                        case "5":
                            k.InKho();
                            break;

                        case "6":
                            k.ThongKeLoai();
                            break;

                        case "7":
                            Console.Write("Ten file: ");
                            k.LuuFile(Console.ReadLine());
                            break;

                        case "8":
                            Console.Write("Ten file: ");
                            var ten = Console.ReadLine();
                            var loaded = KhoHang.DocFile(ten);
                            if (loaded != null) k = loaded;
                            break;

                        case "9":
                            return;

                        default:
                            Console.WriteLine("Lua chon khong hop le!");
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Loi: " + ex.Message);
                }
            }
        }
    }

}
