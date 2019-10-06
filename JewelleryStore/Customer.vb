Imports System.Data
Imports System.Data.SqlClient

Public Class Customer

    Dim query As String = ""

    Dim cmd As SqlCommand
    Dim ds As DataSet
    Dim da As SqlDataAdapter

    Dim cust_id As Integer

    Sub New()

        ' add customer button
        AddHandler Form4.btn_add_cust.Click, AddressOf add_customer

        ' cancel customer
        AddHandler Form4.btn_cancel_cust.Click, AddressOf cancel_customer

        ' update customer
        AddHandler Form4.btn_update_cust.Click, AddressOf update_customer

        ' customer grid view
        AddHandler Form4.customer_grid.CellContentClick, AddressOf customer_grid_select
    End Sub

    ' add customer button click
    Private Sub add_customer(sender As Object, e As EventArgs)
        
        If Form4.t_cust_name.Text <> "" And Form4.txt_cust_address.Text <> "" And Form4.t_cust_mobile.Text <> "" Then

            If Form4.t_cust_mobile.Text.Length = 10 Then
                query = "insert into customer(cust_name, cust_mobile, cust_address) values( '" + Form4.t_cust_name.Text + "', '" + Form4.t_cust_mobile.Text + "', '" + Form4.txt_cust_address.Text + "')"
                cmd = New SqlCommand(query, DB.connection)

                DB.connection.Open()
                If cmd.ExecuteNonQuery > 0 Then
                    MsgBox("Customer Added !!")
                    ' gridview customer
                    customer_grid()

                    ' cancel data
                    cancel_customer()

                End If
                DB.connection.Close()
            Else
                MsgBox("Mobile No 10 Digit !")
            End If
        Else
            MsgBox("Please Provide All Details !!")
        End If

    End Sub

    ' customer cancel button click
    Private Sub cancel_customer()
        ' hide show button
        Form4.btn_add_cust.Show()
        Form4.btn_update_cust.Hide()

        Form4.t_cust_name.Text = ""
        Form4.t_cust_mobile.Text = ""
        Form4.txt_cust_address.Text = ""
    End Sub

    ' customer update button click
    Private Sub update_customer(sender As Object, e As EventArgs)

        If Form4.t_cust_name.Text <> "" And Form4.txt_cust_address.Text <> "" And Form4.t_cust_mobile.Text <> "" Then

            If Form4.t_cust_mobile.Text.Length = 10 Then
                
                query = "update customer set cust_name = '" + Form4.t_cust_name.Text + "', cust_mobile = '" + Form4.t_cust_mobile.Text + "', cust_address = '" + Form4.txt_cust_address.Text + "' where cust_id = " + cust_id.ToString + " "

                cmd = New SqlCommand(query, DB.connection)

                DB.connection.Open()
                If cmd.ExecuteNonQuery > 0 Then
                    MsgBox("Customer Updated !!")
                    ' gridview customer
                    customer_grid()

                    ' cancel data
                    cancel_customer()

                    
                End If
                DB.connection.Close()
            Else
                MsgBox("Mobile No 10 Digit !")
            End If
        Else
            MsgBox("Please Provide All Details !!")
        End If

    End Sub


    Public Sub customer_grid()

        Dim query As String = "select * from customer"
        cmd = New SqlCommand(query, DB.connection)
        da = New SqlDataAdapter(cmd)
        ds = New DataSet

        da.Fill(ds, "customer")
        Form4.customer_grid.DataSource = ds.Tables("customer")

        ' drop down
        customer_dropdown()
    End Sub

    Private Sub customer_grid_select(sender As Object, e As DataGridViewCellEventArgs)

        ' hide show customer button
        ' hide show button
        Form4.btn_add_cust.Hide()
        Form4.btn_update_cust.Show()

        Dim i As Integer = Form4.customer_grid.CurrentRow.Index
        Form4.product_id_sale.Text = Form4.customer_grid.Item("cust_id", i).Value
        Form4.t_cust_name.Text = Form4.customer_grid.Item("cust_name", i).Value
        Form4.t_cust_mobile.Text = Form4.customer_grid.Item("cust_mobile", i).Value
        Form4.txt_cust_address.Text = Form4.customer_grid.Item("cust_address", i).Value
        cust_id = Form4.customer_grid.Item(0, i).Value
    End Sub


    ' customer dropdown

    Sub customer_dropdown()
        cmd = New SqlCommand("select * from customer", DB.connection)
        da = New SqlDataAdapter(cmd)
        ds = New DataSet

        da.Fill(ds, "customer")

        ' form5 supplier combo
        Form4.t_sales_combo.DataSource = ds.Tables("customer")
        Form4.t_sales_combo.DisplayMember = "cust_name"
        Form4.t_sales_combo.ValueMember = "cust_id"
    End Sub
    
End Class
