using CSharp_assignment_test.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;

public class SaleService
{
    private readonly string _connectionString;
    private readonly string _logFilePath = "logs/errors.txt";

    public SaleService()
    {
        _connectionString = @"Data Source=DESKTOP-4RLB5HB\A_SQLSERVER;Initial Catalog=CSharpDB;Integrated Security=True;Encrypt=False";
        Directory.CreateDirectory(Path.GetDirectoryName(_logFilePath));
    }
    public List<SaleDto> GetProductSales(DateTime startDate, DateTime endDate, string productNameFilter)
    {
        List<SaleDto> sales = new List<SaleDto>();

        try
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("stGetProducts", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@STARTDATE", startDate);
                    cmd.Parameters.AddWithValue("@ENDDATE", endDate);
                    cmd.Parameters.AddWithValue("@PRODUCTNAME_FILTER", (Object)productNameFilter ?? DBNull.Value);

                    conn.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            sales.Add(new SaleDto
                            {
                                SaleID = reader.GetInt32(reader.GetOrdinal("SALEID")),
                                ProductCode = reader.GetString(reader.GetOrdinal("PRODUCTCODE")),
                                ProductName = reader.GetString(reader.GetOrdinal("PRODUCTNAME")),
                                Quantity = reader.IsDBNull(reader.GetOrdinal("QUANTITY")) ? 0 : reader.GetInt32(reader.GetOrdinal("QUANTITY")),
                                UnitPrice = reader.IsDBNull(reader.GetOrdinal("UNITPRICE")) ? 0m : reader.GetDecimal(reader.GetOrdinal("UNITPRICE")),
                                Total = reader.IsDBNull(reader.GetOrdinal("Total")) ? 0m : reader.GetDecimal(reader.GetOrdinal("Total")),
                                SaleDate = reader.IsDBNull(reader.GetOrdinal("SALEDATE")) ? DateTime.MinValue : reader.GetDateTime(reader.GetOrdinal("SALEDATE")),
                                RowNum = reader.IsDBNull(reader.GetOrdinal("RowNum")) ? 0 : Convert.ToInt32(reader.GetInt64(reader.GetOrdinal("RowNum")))
                            });
                        }
                    }
                }
            }
        }
        catch (SqlException ex)
        {
            LogError($"SQL Error in GetProductSales: {ex.Message}\nStackTrace: {ex.StackTrace}");
            System.Windows.Forms.MessageBox.Show($"Database error: {ex.Message}\nCheck logs/errors.txt for details.", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
            return null; // Return null ឬ throw ex;
        }
        catch (Exception ex)
        {
            LogError($"General Error in GetProductSales: {ex.Message}\nStackTrace: {ex.StackTrace}");
            System.Windows.Forms.MessageBox.Show($"An unexpected error occurred: {ex.Message}\nCheck logs/errors.txt for details.", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
            return null; // Return null ឬ throw ex;
        }

        return sales;
    }

    private void LogError(string message)
    {
        try
        {
            string logEntry = $"{DateTime.Now}: {message}\n";
            File.AppendAllText(_logFilePath, logEntry);
        }
        catch (Exception logEx)
        {
            System.Windows.Forms.MessageBox.Show($"Failed to write to log file: {logEx.Message}", "Logging Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Warning);
        }
    }
}