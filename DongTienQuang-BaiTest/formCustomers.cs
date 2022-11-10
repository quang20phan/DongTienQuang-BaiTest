using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace DongTienQuang_BaiTest
{
    public partial class formCustomers : Form
    {
        public formCustomers()
        {
            InitializeComponent();
        }

        private async Task<DataTable> LoadDataTable(int pageNumber, int pageSize)
        {
            SqlConnection conn = ConnectionDB.getConnetion();

            string sql = @"DECLARE @PageNumber AS INT
                            DECLARE @PageSize AS INT
                            SET @PageNumber=" + pageNumber + "SET @PageSize=" + pageSize + "SELECT ID, Code, Name as [Tên], Address as [Địa chỉ] FROM [tbl__Customers]ORDER BY ID OFFSET (@PageNumber-1)*@PageSize ROWS FETCH NEXT @PageSize ROWS ONLY";
            await conn.OpenAsync();
            SqlCommand sqlCommand = new SqlCommand(sql, conn);
            SqlDataReader sqlDataReader = await sqlCommand.ExecuteReaderAsync();

            DataTable dttb = new DataTable();
            dttb.Load(sqlDataReader);
            conn.Close();

            return dttb;     
        }

        int pageNumber = 1;
        int pageSize = 10;
        int pageSum;

        private async void formCustomers_Load(object sender, EventArgs e)
        {
            pageSum = await getAllCustomers();
            var dttb =  await LoadDataTable(pageNumber, pageSize);
            dgv__Customers.DataSource = dttb;
            lbPageNumber.Text = string.Format("Page {0}/{1}", pageNumber, pageSum/pageSize);
            
        }

        private async Task<int> getAllCustomers()
        {
            SqlConnection conn = ConnectionDB.getConnetion();

            string sql = "SELECT ID, Code, Name, Address FROM [tbl__Customers]";
            await conn.OpenAsync();
            SqlCommand sqlCommand = new SqlCommand(sql, conn);
            SqlDataReader sqlDataReader = await sqlCommand.ExecuteReaderAsync();

            DataTable dttb = new DataTable();
            dttb.Load(sqlDataReader);

            conn.Close();

            return dttb.Rows.Count;
        }

        private async void btnPrev_Click(object sender, EventArgs e)
        {
            if (pageNumber - 1 > 0)
            {
                pageNumber--;
                var dttb = await LoadDataTable(pageNumber, pageSize);
                dgv__Customers.DataSource = dttb;
                lbPageNumber.Text = string.Format("Page {0}/{1}", pageNumber, pageSum / pageSize);
            }
        }

        private async void btnNext_Click(object sender, EventArgs e)
        {
            if (pageNumber < pageSum / pageSize)
            {
                pageNumber++;
                var dttb = await LoadDataTable(pageNumber, pageSize);
                dgv__Customers.DataSource = dttb;
                lbPageNumber.Text = string.Format("Page {0}/{1}", pageNumber, pageSum / pageSize);
            }
        }
    }
}
