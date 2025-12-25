using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
namespace ColdStorageSystem
{
    // ================= MESSAGE =================
    class Message
    {
        public string SensorId;
        public int Cycle;
        public int Row;
        public int Col;
        public double Temperature;

        public Message(string sensorId, int cycle, int row, int col, double temp)
        {
            SensorId = sensorId;
            Cycle = cycle;
            Row = row;
            Col = col;
            Temperature = temp;
        }
        public Message(int cycle, int row, int col, double temp)
        {
            SensorId = $"S{row}{col}";
            Cycle = cycle;
            Row = row;
            Col = col;
            Temperature = temp;
        }
    }

    // ================= SENSOR =================
    class Sensor
    {
        public string Id;
        public double NhietDoHienTai;
        public int DemKhongDoi;
        public int NguongLoi;
        public bool DaHu;

        public Sensor(string id, int threshold)
        {
            Id = id;
            NhietDoHienTai = -10;
            DemKhongDoi = 0;
            NguongLoi = threshold;
            DaHu = false;
        }

        public string UpdateVaKiemTra(double nhietDoMoi, int row, int col)
        {
            if (DaHu) return null;

            if (Math.Abs(nhietDoMoi - NhietDoHienTai) < 0.01)
                DemKhongDoi++;
            else
                DemKhongDoi = 0;

            NhietDoHienTai = nhietDoMoi;

            if (DemKhongDoi >= NguongLoi)
            {
                DaHu = true;
                return $"Canh bao: Cam bien {Id} ({row},{col}) bi hu (khong doi {NguongLoi} chu ky)";
            }

            return null;
        }
    }

    // ================= COLD STORAGE =================
    class ColdStorage
    {
        private Sensor[,] sensorGrid;
        private Queue<Message> hangDoiTinNhan;
        private List<string> danhSachCanhBao;
        private double[,] avgTruoc2x2;

        private int rows, cols;
        private const double NGUONG_TANG_DOT_NGOT = 3.0;

        public ColdStorage(int r, int c, int[,] maTranNguong)
        {
            rows = r;
            cols = c;

            sensorGrid = new Sensor[r, c];
            hangDoiTinNhan = new Queue<Message>();
            danhSachCanhBao = new List<string>();

            for (int i = 0; i < r; i++)
                for (int j = 0; j < c; j++)
                    sensorGrid[i, j] = new Sensor($"S{i}{j}", maTranNguong[i, j]);

            avgTruoc2x2 = new double[r - 1, c - 1];
            for (int i = 0; i < r - 1; i++)
                for (int j = 0; j < c - 1; j++)
                    avgTruoc2x2[i, j] = -999;
        }

        public void ThemMessage(Message msg)
        {
            hangDoiTinNhan.Enqueue(msg);
        }

        public void ProcessOneCycle(int cycle)
        {
            while (hangDoiTinNhan.Count > 0 && hangDoiTinNhan.Peek().Cycle == cycle)
            {
                Message msg = hangDoiTinNhan.Dequeue();
                string canhBao = sensorGrid[msg.Row, msg.Col]
                    .UpdateVaKiemTra(msg.Temperature, msg.Row, msg.Col);

                if (canhBao != null)
                    danhSachCanhBao.Add($"Chu ky {cycle}: {canhBao}");
            }

            KiemTraKhuVuc2x2Nong(cycle);
            KiemTraAreaTangDotNgot(cycle);
        }

        private void KiemTraKhuVuc2x2Nong(int cycle)
        {
            for (int i = 0; i < rows - 1; i++)
            {
                for (int j = 0; j < cols - 1; j++)
                {
                    double avg =
                        (sensorGrid[i, j].NhietDoHienTai +
                         sensorGrid[i + 1, j].NhietDoHienTai +
                         sensorGrid[i, j + 1].NhietDoHienTai +
                         sensorGrid[i + 1, j + 1].NhietDoHienTai) / 4.0;

                    if (avg > 0)
                    {
                        string sensors = $"{sensorGrid[i, j].Id}, {sensorGrid[i, j + 1].Id}, " +
                                         $"{sensorGrid[i + 1, j].Id}, {sensorGrid[i + 1, j + 1].Id}";

                        danhSachCanhBao.Add(
                            $"Chu ky {cycle}: Nhiet do cao tai khu vuc ({i},{j}) avg={avg:F2}C [Sensors: {sensors}]");
                    }
                }
            }
        }

        private void KiemTraAreaTangDotNgot(int cycle)
        {
            for (int i = 0; i < rows - 1; i++)
            {
                for (int j = 0; j < cols - 1; j++)
                {
                    double avgHienTai =
                        (sensorGrid[i, j].NhietDoHienTai +
                         sensorGrid[i + 1, j].NhietDoHienTai +
                         sensorGrid[i, j + 1].NhietDoHienTai +
                         sensorGrid[i + 1, j + 1].NhietDoHienTai) / 4.0;

                    if (avgTruoc2x2[i, j] <= -999)
                    {
                        avgTruoc2x2[i, j] = avgHienTai;
                        continue;
                    }

                    double diff = avgHienTai - avgTruoc2x2[i, j];
                    if (diff >= NGUONG_TANG_DOT_NGOT)
                    {
                        string sensors = $"{sensorGrid[i, j].Id}, {sensorGrid[i, j + 1].Id}, " +
                                         $"{sensorGrid[i + 1, j].Id}, {sensorGrid[i + 1, j + 1].Id}";

                        danhSachCanhBao.Add(
                            $"Chu ky {cycle}: Khu vuc ({i},{j}) tang dot ngot {diff:F1}C [Sensors: {sensors}]");
                    }

                    avgTruoc2x2[i, j] = avgHienTai;
                }
            }
        }

        public void GhiOutputFile(string fileName)
        {
            if (danhSachCanhBao.Count == 0)
                File.WriteAllText(fileName, "Khong co canh bao");
            else
                File.WriteAllLines(fileName, danhSachCanhBao);
        }
    }

    // ================= MAIN =================
    class Program
    {
        static void Main()
        {
            try
            {
                string[] lines = File.ReadAllLines("input.txt");

                string[] size = lines[0].Split(' ');
                int rows = int.Parse(size[0]);
                int cols = int.Parse(size[1]);

                int[,] thresholdMatrix = new int[rows, cols];
                int index = 1;

                for (int i = 0; i < rows; i++)
                {
                    string[] t = lines[index++].Split(' ');
                    for (int j = 0; j < cols; j++)
                        thresholdMatrix[i, j] = int.Parse(t[j]);
                }

                ColdStorage system = new ColdStorage(rows, cols, thresholdMatrix);

                int maxCycle = -1;
                for (int i = index; i < lines.Length; i++)
                {
                    try
                    {
                        if (string.IsNullOrWhiteSpace(lines[i]) || lines[i].StartsWith("#"))
                            continue;
                        string[] p = lines[i].Split(' ');
                        int cycle = int.Parse(p[0]);
                        int r = int.Parse(p[1]);
                        int c = int.Parse(p[2]);
                        double temp = double.Parse(p[3]);

                        system.ThemMessage(new Message(cycle, r, c, temp));
                        if (cycle > maxCycle) maxCycle = cycle;
                    }
                    catch { }
                }

                for (int cycle = 0; cycle <= maxCycle; cycle++)
                    system.ProcessOneCycle(cycle);

                system.GhiOutputFile("output.txt");
                Console.WriteLine("Da xu ly xong! Ket qua da duoc ghi vao output.txt");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Loi nghiem trong: " + ex.Message);
            }
        }
    }
}
