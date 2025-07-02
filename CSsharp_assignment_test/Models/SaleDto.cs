// File: CSharp_assignment_test/Models/SaleDto.cs
using System;

namespace CSharp_assignment_test.Models
{
    // Models/SaleDto.cs
    // Models/SaleDto.cs
    public class SaleDto
    {
        public int SaleID { get; set; }        // ត្រូវគ្នានឹង SQL 'int'
        public string ProductCode { get; set; } // ត្រូវគ្នានឹង SQL 'varchar' / 'nvarchar'
        public string ProductName { get; set; } // ត្រូវគ្នានឹង SQL 'varchar' / 'nvarchar'
        public int Quantity { get; set; }      // ត្រូវគ្នានឹង SQL 'int'
        public decimal UnitPrice { get; set; } // ត្រូវគ្នានឹង SQL 'decimal' / 'money' / 'numeric'
        public decimal Total { get; set; }     // ត្រូវគ្នានឹង SQL 'decimal' / 'money' / 'numeric'
        public DateTime SaleDate { get; set; }  // ត្រូវគ្នានឹង SQL 'date' / 'datetime'
        public int RowNum { get; set; }        // ត្រូវគ្នានឹង SQL 'int' (ប្រសិនបើអ្នកប្រើ)
    }
}