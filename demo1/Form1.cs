﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Button;

namespace demo1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        // tạo đối tượng sử dụng tài liệu xml
        XmlDocument doc = new XmlDocument();
        // đường dẫn tới file xml
        string tentep = @"C:\Users\DELL\source\repos\demo1\demo1\dsnhanvien.xml";
        int d; // xác định chỉ số dòng được chọn trên dataGrid

        private void HienThi()
        {
            datanhanvien.Rows.Clear(); // Xóa tất cả các dòng cũ trong DataGridView
            doc.Load(tentep); // Tải file XML

            // Lấy danh sách các nút <nhanvien>
            XmlNodeList DS = doc.SelectNodes("/ds/nhanvien");
            datanhanvien.ColumnCount = 5;

            // Duyệt qua từng nút <nhanvien>
            foreach (XmlNode nhan_vien in DS)
            {
                // Tạo một dòng mới trong DataGridView
                int index = datanhanvien.Rows.Add();
                DataGridViewRow row = datanhanvien.Rows[index];

                // Truy xuất giá trị thuộc tính và nút con
                XmlNode ma_nv = nhan_vien.SelectSingleNode("@manv");
                XmlNode ho_ten = nhan_vien.SelectSingleNode("hoten");
                XmlNode gioi_tinh = nhan_vien.SelectSingleNode("gioitinh");
                XmlNode trinh_do = nhan_vien.SelectSingleNode("trinhdo");
                XmlNode dia_chi = nhan_vien.SelectSingleNode("diachi");

                // Gán giá trị vào các ô của dòng
                row.Cells[0].Value = ma_nv?.InnerText ?? string.Empty;
                row.Cells[1].Value = ho_ten?.InnerText ?? string.Empty;
                row.Cells[2].Value = gioi_tinh?.InnerText ?? string.Empty;
                row.Cells[3].Value = trinh_do?.InnerText ?? string.Empty;
                row.Cells[4].Value = dia_chi?.InnerText ?? string.Empty;
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            HienThi();
        }

        private void datanhanvien_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            // Kiểm tra xem dòng được chọn có phải dòng dữ liệu hợp lệ
            if (e.RowIndex >= 0) // Đảm bảo không phải dòng tiêu đề hoặc dòng trống
            {
                d = e.RowIndex;

                // Lấy dữ liệu từ các ô trong dòng được chọn
                txt_ma.Text = datanhanvien.Rows[d].Cells[0].Value?.ToString() ?? string.Empty;
                txt_ten.Text = datanhanvien.Rows[d].Cells[1].Value?.ToString() ?? string.Empty;
                cb_td.Text = datanhanvien.Rows[d].Cells[3].Value?.ToString() ?? string.Empty;
                txt_dc.Text = datanhanvien.Rows[d].Cells[4].Value?.ToString() ?? string.Empty;

                // Lấy giá trị giới tính từ ô đã click
                string gender = datanhanvien.Rows[d].Cells[2].Value?.ToString() ?? string.Empty;

                // Cập nhật giá trị cho radio button
                if (gender == "Nam")
                {
                    rb_nam.Checked = true;
                    rb_nu.Checked = false;
                }
                else if (gender == "Nữ")
                {
                    rb_nam.Checked = false;
                    rb_nu.Checked = true;
                }
            }
        }

        private void btnthem_Click(object sender, EventArgs e)
        {
            // Đọc tài liệu XML
            doc.Load(tentep);
            XmlElement goc = doc.DocumentElement;

            // Kiểm tra nếu mã nhân viên đã tồn tại
            XmlNode existingNode = goc.SelectSingleNode($"/ds/nhanvien[@manv='{txt_ma.Text}']");
            if (existingNode != null)
            {
                MessageBox.Show("Mã nhân viên đã tồn tại. Vui lòng nhập mã khác!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Tạo một nút <nhanvien>
            XmlNode nhan_vien = doc.CreateElement("nhanvien");

            // Tạo thuộc tính manv
            XmlAttribute ma_nv = doc.CreateAttribute("manv");
            ma_nv.InnerText = txt_ma.Text;
            nhan_vien.Attributes.Append(ma_nv);

            // Tạo nút <hoten>
            XmlNode ho_ten = doc.CreateElement("hoten");
            ho_ten.InnerText = txt_ten.Text;
            nhan_vien.AppendChild(ho_ten);

            // tao nut <gioi tinh>
            XmlNode gioi_tinh = doc.CreateElement("gioitinh");
            if (rb_nam.Checked)
            {
                gioi_tinh.InnerText = "Nam";
            }
            else if (rb_nu.Checked)
            {
                gioi_tinh.InnerText = "Nữ";
            }
            nhan_vien.AppendChild (gioi_tinh);

            // tạo nút <trinhdo>
            XmlNode trinh_do = doc.CreateElement("trinhdo");
            trinh_do.InnerText = txt_ten.Text;
            nhan_vien.AppendChild (trinh_do);

            // Tạo nút <diachi>
            XmlNode dia_chi = doc.CreateElement("diachi");
            dia_chi.InnerText = txt_dc.Text;
            nhan_vien.AppendChild(dia_chi);

            // Thêm nút nhanvien vào gốc
            goc.AppendChild(nhan_vien);

            // Lưu tệp XML
            doc.Save(tentep);

            // Hiển thị lại dữ liệu
            HienThi();
            MessageBox.Show("Thêm nhân viên thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnsua_Click(object sender, EventArgs e)
        {
            doc.Load(tentep);
            XmlElement goc = doc.DocumentElement;
            // xac dinh nut nhan vien can sua
            XmlNode nhan_vien_cu = goc.SelectSingleNode("/ds/nhanvien[@manv = '" + txt_ma.Text + "']");

            XmlNode nhan_vien_moi = doc.CreateElement("nhanvien");

            XmlAttribute ma_nv = doc.CreateAttribute("manv");
            ma_nv.InnerText = txt_ma.Text;
            nhan_vien_moi.Attributes.Append(ma_nv);

            XmlNode ho_ten = doc.CreateElement("hoten");
            ho_ten.InnerText = txt_ten.Text;
            nhan_vien_moi.AppendChild(ho_ten);

            XmlNode gioi_tinh = doc.CreateElement("gioitinh");
            if (rb_nam.Checked)
            {
                gioi_tinh.InnerText = "Nam";
            }
            else if (rb_nu.Checked)
            {
                gioi_tinh.InnerText = "Nữ";
            }
            nhan_vien_moi.AppendChild(gioi_tinh);

            XmlNode trinh_do = doc.CreateElement("trinhdo");
            trinh_do.InnerText = txt_ten.Text;
            nhan_vien_moi.AppendChild(trinh_do);

            XmlNode dia_chi = doc.CreateElement("diachi");
            dia_chi.InnerText = txt_dc.Text;
            nhan_vien_moi.AppendChild(dia_chi);

            // thay nut nhan vien cu bang nhan vien moi
            goc.ReplaceChild(nhan_vien_moi, nhan_vien_cu);
            // Lưu tệp XML
            doc.Save(tentep);

            // Hiển thị lại dữ liệu
            HienThi();

            // Thông báo sửa thành công
            MessageBox.Show("Sửa thông tin nhân viên thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);

        }

        private void btnxoa_Click(object sender, EventArgs e)
        {
            // Đọc tệp XML
            doc.Load(tentep);
            XmlElement goc = doc.DocumentElement;

            // Xác định nút cần xóa
            XmlNode nhan_vien_xoa = goc.SelectSingleNode("/ds/nhanvien[@manv='" + txt_ma.Text + "']");

            // Xóa nút nhân viên
            goc.RemoveChild(nhan_vien_xoa);

            // Lưu tệp XML
            doc.Save(tentep);

            // Hiển thị lại dữ liệu
            HienThi();

            // Thông báo xóa thành công
            MessageBox.Show("Xóa nhân viên thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btntim_Click(object sender, EventArgs e)
        {
            try
            {
                // Tải tệp XML
                doc.Load(tentep);

                // Lấy gốc của tệp XML
                XmlElement goc = doc.DocumentElement;

                // Xác định giới tính cần tìm
                string gioitinhCanTim = "Nam";

                // Tìm tất cả các nhân viên có giới tính "Nam"
                XmlNodeList nhan_vien_tim_list = goc.SelectNodes($"/ds/nhanvien[gioitinh='{gioitinhCanTim}']");

                // Kiểm tra nếu không tìm thấy nhân viên nào
                if (nhan_vien_tim_list.Count == 0)
                {
                    MessageBox.Show("Không tìm thấy nhân viên với Giới tính là: " + gioitinhCanTim, "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                // Xóa dữ liệu cũ trên DataGridView
                datanhanvien.Rows.Clear();

                // Duyệt qua tất cả các nhân viên tìm được
                foreach (XmlNode nhan_vien_tim in nhan_vien_tim_list)
                {
                    // Lấy thông tin nhân viên
                    string manv = nhan_vien_tim.Attributes["manv"].InnerText;
                    string hoten = nhan_vien_tim.SelectSingleNode("hoten").InnerText;
                    string gioitinh = nhan_vien_tim.SelectSingleNode("gioitinh").InnerText;
                    string trinhdo = nhan_vien_tim.SelectSingleNode("trinhdo").InnerText;
                    string diachi = nhan_vien_tim.SelectSingleNode("diachi").InnerText;

                    // Thêm thông tin nhân viên vào DataGridView
                    datanhanvien.Rows.Add(manv, hoten, gioitinh, trinhdo, diachi);
                }

                // Thông báo tìm kiếm thành công
                MessageBox.Show("Tìm thấy " + nhan_vien_tim_list.Count + " nhân viên có giới tính: " + gioitinhCanTim, "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                // Thông báo lỗi nếu có
                MessageBox.Show("Lỗi khi tìm nhân viên: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnreset_Click(object sender, EventArgs e)
        {
            HienThi();
        }
    }
}
