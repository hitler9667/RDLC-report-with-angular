using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;

namespace RDLCServer
{
    public partial class RDLCReportViewer : System.Web.UI.Page
    {
        private List<SqlParameter> sqlParameters;
        private DataSet dataSet;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                try
                {
                    SetSqlParameter();
                    GetDatatable();
                    GenerateReport();
                }
                catch (Exception)
                {
                    Console.WriteLine("Error occured while processing the report");
                }
            }
        }

        private void GetDatatable()
        {
            string query = @"Select O.ProductID, P.ProductName, OS.ShipCountry,O.UnitPrice, 
                                O.Quantity, (O.UnitPrice * Quantity) as Amount 
                                from dbo.[Order Details] O
                                inner join dbo.Products P on P.ProductID = O.ProductID
                                inner join dbo.Orders OS on OS.OrderID = O.OrderID
                                where O.ProductID = @ProductId AND OS.ShipCountry = @ShipCountry";

            string connectionString = ConfigurationManager.ConnectionStrings["RdlcConnectionString"].ConnectionString;
            dataSet = new DataSet();
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlCommand sqlCommand = new SqlCommand(query, con))
                {
                    sqlCommand.Parameters.AddRange(sqlParameters.ToArray());
                    SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCommand);
                    sqlDataAdapter.Fill(dataSet);
                }
            }
        }

        private void SetSqlParameter()
        {
            sqlParameters = new List<SqlParameter>();
            //Get the parameter values from Query string
            sqlParameters.Add(new SqlParameter("ProductId", Convert.ToInt32(Request.QueryString["ProductId"])));
            sqlParameters.Add(new SqlParameter("ShipCountry", Request.QueryString["ShipCountry"]));
        }

        private void GenerateReport()
        {
            //Set report path. This can be set on Report Viewer Control too
            RdlcReportViewer.LocalReport.ReportPath =
                   Server.MapPath("~/Reports/ProductSaleReport.rdlc");

            RdlcReportViewer.LocalReport.DataSources.Clear();
            //Assign the dataTable to the dataset
            RdlcReportViewer.LocalReport.DataSources
                    .Add(new ReportDataSource("ProductSaleDataset", dataSet.Tables[0]));
            RdlcReportViewer.LocalReport.Refresh();
        }
    }
}