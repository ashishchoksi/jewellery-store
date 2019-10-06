Imports System.Data
Imports System.Data.SqlClient

Public Class Bill

    Dim query As String
    Dim cmd As SqlCommand
    Dim da As SqlDataAdapter
    Dim ds As DataSet

    Sub New()
        ' drop down
        cust_drop_down()

        Form4.customer_list_find.Enabled = False
        Form4.cust_sale_find.Enabled = False
    End Sub

    Sub assign_control()

        ' find customer bill
        AddHandler Form4.search_customer_bill_btn.Click, AddressOf find_customer_bill

        ' search bill id
        AddHandler Form4.btn_search_cust.Click, AddressOf search_bill_no
    End Sub

    ' customer dropdown
    Sub cust_drop_down()
        query = "select cust_id, cust_name from customer where cust_id in (select distinct(cust_id) from sales_master)"
        cmd = New SqlCommand(query, DB.connection)
        da = New SqlDataAdapter(cmd)
        ds = New DataSet
        da.Fill(ds, "customer_data")
        Form4.customer_list_find.DataSource = ds.Tables("customer_data")
        Form4.customer_list_find.DisplayMember = "cust_name"
        Form4.customer_list_find.ValueMember = "cust_id"
    End Sub

    ' find customer bill

    Private Sub find_customer_bill(sender As Object, e As EventArgs)

        Try
            If Form4.bill_no_find.Text <> "" Then
                Dim query As String = "select * from bill where sales_master_id=" + Form4.bill_no_find.Text.ToString + " "
                Dim cmd As New SqlCommand(query, DB.connection)
                Dim da As New SqlDataAdapter(cmd)
                Dim ds As New DataSet
                da.Fill(ds)
                Dim rpt As New CrystalReport2
                rpt.SetDataSource(ds.Tables(0))
                Form4.crystal_find_bill.ReportSource = rpt
            Else
                MsgBox("Insert Bill NO !!")
            End If
        Catch ex As Exception
            MsgBox("Something is Going Wrong Check Bill No Again !!!!!")
        End Try
        
    End Sub

    Private Sub search_bill_no(sender As Object, e As EventArgs)
        query = "select sales_master_id from sales_master where 1=1 "

        If Form4.CheckBox6.Checked Then
            query &= "and cust_id = " + Form4.customer_list_find.SelectedValue.ToString + " "
        End If

        If Form4.CheckBox7.Checked Then
            query &= "and sale_date = '" + Form4.cust_sale_find.Text.ToString + "' "
        End If

        cmd = New SqlCommand(query, DB.connection)
        da = New SqlDataAdapter(cmd)
        ds = New DataSet
        da.Fill(ds, "sale")

        Form4.lbl_id.Text = "Bill ID is :"
        Form4.list_bill_id.DataSource = ds.Tables("sale")
        Form4.list_bill_id.DisplayMember = "sales_master_id"
    End Sub



End Class
