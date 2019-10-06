Imports System.Data
Imports System.Data.SqlClient

Public Class Sales

    Dim cmd As SqlCommand
    Dim da As SqlDataAdapter
    Dim ds As DataSet
    Dim query As String
    Dim checkout As Integer = 0

    Dim sales_id As Integer = 0
    Dim dr As SqlDataReader

    Dim lblr(), lbll() As Label

    Dim xcord = 87, ycord As Integer = 140

    Sub New()
        ' search item button click
        AddHandler Form4.btn_get_sales.Click, AddressOf get_sales_button

        ' search button click on form 5
        AddHandler Form5.find_sales_button.Click, AddressOf search_item

        ' calcel button click
        AddHandler Form4.btn_calcel_bill.Click, AddressOf cancel_select

        ' add to purchase button click
        AddHandler Form4.btn_add_to_bill.Click, AddressOf add_to_sales

        ' select added items
        AddHandler Form4.add_sales_grid.CellContentClick, AddressOf sales_item_double_click

        ' remove all items click
        AddHandler Form4.remove_all_items.Click, AddressOf remove_all_items

        ' remove one item
        AddHandler Form4.btn_remove_from_bill.Click, AddressOf remove_one_item

        ' check out button click 
        AddHandler Form4.btn_checkout.Click, AddressOf checkout_click

        ' cancel item button click
        AddHandler Form4.btn_cancel_invoice.Click, AddressOf cancel_item

        ' update customer name
        AddHandler Form4.btn_update_customer.Click, AddressOf update_detail

        ' confirm bill button click
        AddHandler Form4.btn_invoice_confirm.Click, AddressOf confirm_bill

        ' sold item list grid view
        sold_item_grid()
    End Sub


    Private Sub get_sales_button(sender As Object, e As EventArgs)
        ' MsgBox("sales")
        find_select_sales_product()
        product_drop_down()

        ' select on double click
        AddHandler Form5.select_product_grid.CellContentClick, AddressOf select_double_click

        Form5.Show()
    End Sub

    ' select query for select
    Function get_query() As String
        query = "select p.pd_id as pd_id, pm.pm_name as product_name, p.weight, p.diamonds, p.making, p.other_charge, s.cname as supplier, p.purity , Round(( ( ( (select price from gold_price where gold_id = (select max(gold_id) from gold_price)) * purity ) / 1000 ) * ( weight - (diamonds * 0.002) ) ) + ( ( weight - (diamonds * 0.002) ) * making ) + ( diamonds * 35 ) + other_charge, 0) as sell_price, purchase_date, p.cost_price from product_detail p, product_master pm, supplier s where pm.pm_id = p.pm_id and p.supplier_id = s.supplier_id and p.pd_id not in (select pd_id from sales) and p.pd_id not in (select pd_id from sales_detail) "
        Return query
    End Function

    ' select on first time
    Sub find_select_sales_product()
        query = get_query()
        select_sales_record(query)
    End Sub

    ' after search button click
    Private Sub search_item(sender As Object, e As EventArgs)
        query = get_query()

        If Form5.find_product_id.Text <> "" Then
            query &= " and p.pd_id = " + Form5.find_product_id.Text + " "
        End If
        If Form5.product_combo.Text <> "" Then
            query &= " and p.pm_id = " + Form5.product_combo.SelectedValue.ToString + " "
        End If
        If Form5.find_weight.Text <> "" Then
            query &= " and p.weight = " + Form5.find_weight.Text + " "
        End If
        select_sales_record(query)
    End Sub

    ' display on grid view
    Sub select_sales_record(q As String)
        Try
            cmd = New SqlCommand(q, DB.connection)
            da = New SqlDataAdapter(cmd)
            ds = New DataSet
            da.Fill(ds, "product_detail")
            Form5.select_product_grid.DataSource = ds.Tables("product_detail")
        Catch ex As Exception
            MsgBox("Something Going wrong ! Retry !!!")
        End Try
    End Sub

    ' form 5 double click get data
    Private Sub select_double_click()
        cancel_select()
        Dim i As Integer = Form5.select_product_grid.CurrentRow.Index
        Form4.product_id_sale.Text = Form5.select_product_grid.Item("pd_id", i).Value
        Form4.t_product_name.Text = Form5.select_product_grid.Item("product_name", i).Value
        Form4.t_sales_weight.Text = Form5.select_product_grid.Item("weight", i).Value
        Form4.t_sales_price.Text = Form5.select_product_grid.Item("sell_price", i).Value
        Form4.t_cost_price.Text = Form5.select_product_grid.Item("cost_price", i).Value
        check_customer_dropdown()
        Form5.Close()
    End Sub

    Sub product_drop_down()
        cmd = New SqlCommand("select * from product_master", DB.connection)
        da = New SqlDataAdapter(cmd)
        ds = New DataSet

        da.Fill(ds, "product_master")

        ' form5 supplier combo
        Form5.product_combo.DataSource = ds.Tables("product_master")
        Form5.product_combo.DisplayMember = "pm_name"
        Form5.product_combo.ValueMember = "pm_id"
    End Sub

    ' cancel button click
    Private Sub cancel_select()
        Form4.product_id_sale.Text = ""
        Form4.t_product_name.Text = ""
        Form4.t_sales_weight.Text = ""
        Form4.t_sales_price.Text = ""
        'Form4.t_sales_combo.Text = ""

        ' hide show add / update
        Form4.btn_add_to_bill.Show()
        Form4.btn_remove_from_bill.Hide()
        Form4.btn_update_customer.Hide()
    End Sub


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

    ' add to sales button click
    Private Sub add_to_sales(sender As Object, e As EventArgs)

        If Form4.product_id_sale.Text <> "" And Form4.t_sales_weight.Text <> "" And Form4.t_product_name.Text <> "" And Form4.t_sales_combo.Text <> "" Then
            query = "insert into sales(cust_id, pd_id, gold_id, sale_price, check_out, cost_price) values(" + Form4.t_sales_combo.SelectedValue.ToString + ", " + Form4.product_id_sale.Text + ", " + Form4.gold_id.ToString + ", " + Form4.t_sales_price.Text + ", 0, " + Form4.t_cost_price.Text + ")"
            cmd = New SqlCommand(query, DB.connection)

            DB.connection.Open()
            If cmd.ExecuteNonQuery > 0 Then
                MsgBox("added to Bill !")
            End If
            DB.connection.Close()
            ' update cutomer name
            update_customer(Form4.t_sales_combo.SelectedValue)
            cancel_select()
            added_to_bill()
        Else
            MsgBox("Please Fill all Field !!")
        End If
        check_customer_dropdown()
    End Sub

    ' add to bill button data select
    Sub added_to_bill()

        cmd = New SqlCommand("select s.*, pd.weight as weight, c.cust_name as customer, pm.pm_name as product from sales s, customer c, product_master pm, product_detail pd where check_out = 0 and s.cust_id = c.cust_id and s.pd_id = pd.pd_id and pd.pm_id = pm.pm_id ", DB.connection)
        da = New SqlDataAdapter(cmd)
        ds = New DataSet()
        da.Fill(ds, "sales")
        Form4.add_sales_grid.DataSource = ds.Tables("sales")
        cancel_select()

        ' hide show add / update
        Form4.btn_add_to_bill.Show()
        Form4.btn_remove_from_bill.Hide()
        Form4.btn_update_customer.Hide()

        ' check sales customer enable disable
        check_customer_dropdown()
    End Sub

    ' sales click for delete
    Private Sub sales_item_double_click()
        Dim i As Integer = Form4.add_sales_grid.CurrentRow.Index
        Form4.product_id_sale.Text = Form4.add_sales_grid.Item("pd_id", i).Value
        Form4.t_product_name.Text = Form4.add_sales_grid.Item("product", i).Value
        Form4.t_sales_weight.Text = Form4.add_sales_grid.Item("weight", i).Value
        Form4.t_sales_price.Text = Form4.add_sales_grid.Item("sale_price", i).Value
        sales_id = Form4.add_sales_grid.Item("sales_id", i).Value
        Form4.t_sales_combo.Text = Form4.add_sales_grid.Item("customer", i).Value

        ' hide show add / update
        Form4.btn_add_to_bill.Hide()
        Form4.btn_remove_from_bill.Show()
        Form4.btn_update_customer.Show()

        ' customer lock release enable
        Form4.t_sales_combo.Enabled = True
    End Sub

    ' remove all items
    Private Sub remove_all_items(sender As Object, e As EventArgs)

        query = "delete from sales where check_out = 0"
        cmd = New SqlCommand(query, DB.connection)
        DB.connection.Open()
        If cmd.ExecuteNonQuery > 0 Then
            MsgBox("All Record Removed !")
        Else
            MsgBox("something is going wrong !!")
        End If
        DB.connection.Close()
        added_to_bill()
    End Sub

    Private Sub remove_one_item(sender As Object, e As EventArgs)
        checkout = 1

        query = "delete from sales where check_out = 0 and sales_id = " + sales_id.ToString + " "
        cmd = New SqlCommand(query, DB.connection)

        DB.connection.Open()
        If cmd.ExecuteNonQuery > 0 Then
            MsgBox("Removed From Bill !")
        Else
            MsgBox("something is going wrong !!")
        End If
        DB.connection.Close()
        added_to_bill()
    End Sub

    Dim i = 0, count As Integer = 0
    ' checkout button click generate Button

    ' clear label
    Dim old As Integer = 0

    ' bill factors
    Dim netwt, tot, gold_rate, total As Double

    ' sales_master data var
    Dim cust_id, gold_id As Integer

    ' total label
    Dim tot_lbl As Label
    ' Dim bill_no As Integer

    Private Sub checkout_click(sender As Object, e As EventArgs)
        'If (checkout <> 0) Then
        '    ReDim lbll(checkout)
        '    For index = 0 To 1
        '        lbll(index).Text = ""
        '    Next
        'Else

        'End If

        Try

            ' getting sales master id

            query = "select max(sales_master_id) from sales_master"
            cmd = New SqlCommand(query, DB.connection)
            da = New SqlDataAdapter(cmd)
            ds = New DataSet
            da.Fill(ds, "sale")

            'bill_no = CInt(ds.Tables("sale").Rows(0).Item(0).ToString) + 1

            'If old >= 0 Then
            For i As Integer = 0 To old
                lbll(i).Text = ""
                tot_lbl.Text = ""
                Form4.cname.Text = ""
                lbll(i).Size = New Size(0, 0)
                tot_lbl.Size = New Size(0, 0)
                Form4.tab_invoice.Controls.Remove(lbll(i))
            Next
            'End If
        Catch ex As Exception
        End Try

        i = 0
        xcord = 87
        ycord = 140
        tot = 0
        total = 0
        netwt = 0

        query = "select * from sales where check_out=0"
        cmd = New SqlCommand(query, DB.connection)
        da = New SqlDataAdapter(cmd)
        ds = New DataSet()
        da.Fill(ds, "sale")
        count = ds.Tables("sale").Rows.Count
        'MsgBox(count)
        query = "select g.gold_id, g.price, s.*, pd.pd_id,pd.pm_id, pd.weight, pd.diamonds, pd.supplier_id, pd.making, pd.other_charge, pd.purity, p.pm_name, c.cust_name, c.cust_id from sales s, product_detail pd, product_master p, customer c, gold_price g where check_out = 0 and s.pd_id = pd.pd_id and pd.pm_id = p.pm_id and c.cust_id = s.cust_id and g.gold_id = s.gold_id"
        cmd = New SqlCommand(query, DB.connection)
        DB.connection.Open()
        dr = cmd.ExecuteReader

        If dr.HasRows Then

            ReDim lbll(count)

            'Form4.bill_no.Text &= bill_no
            While dr.Read

                ' set var
                gold_id = dr("gold_id").ToString
                cust_id = dr("cust_id").ToString

                Form4.cname.Text = dr("cust_name").ToString
                lbll(i) = New Label()
                lbll(i).Location = New Point(xcord, ycord)
                lbll(i).Size = New Size(400, 140)
                lbll(i).Text = "Item                    :      " & dr("pm_name").ToString & vbNewLine
                lbll(i).Text &= "Weight               :      " & dr("weight").ToString & vbNewLine
                lbll(i).Text &= "Diamonds           :      " & dr("diamonds").ToString & vbNewLine

                ' net weight
                netwt = dr("weight").ToString - (dr("diamonds").ToString * 0.002)
                lbll(i).Text &= "Net Weight         :      " & netwt & vbNewLine

                ' gold rate
                gold_rate = ((dr("price").ToString * dr("purity").ToString) / 100)
                lbll(i).Text &= "Gold Rate           :      " & gold_rate & " (10 GM)" & vbNewLine
                lbll(i).Text &= "Making               :      " & dr("making").ToString * netwt & " (" & dr("making").ToString & "  Per Gram)" & vbNewLine
                lbll(i).Text &= "Diamonds Rate   :      " & dr("diamonds").ToString * 35 & " (35 Per Diamond)" & vbNewLine
                lbll(i).Text &= "Other Charge      :      " & dr("other_charge").ToString & vbNewLine

                ' find total
                tot = (netwt * (gold_rate / 10)) + (dr("making").ToString * netwt) + (dr("diamonds").ToString * 35) + dr("other_charge").ToString

                lbll(i).Text &= "----------------------------------------------------------------------------" & vbNewLine
                lbll(i).Text &= "Total                   :      " & Math.Round(tot)
                total += Math.Round(tot)
                Form4.tab_invoice.Controls.Add(lbll(i))
                ycord += 160
                i += 1

            End While
            'MsgBox(total)
            old = i - 1
            tot_lbl = New Label
            With tot_lbl
                .Size = New Size(400, 50)
                .Location = New Point(xcord, ycord - 30)
                .Text = "----------------------------------------------------------------------------------------------------------------------------------"
                .TextAlign = ContentAlignment.MiddleCenter
            End With

            tot_lbl.Text &= vbNewLine + " Total = " & total
            Form4.tab_invoice.Controls.Add(tot_lbl)

        Else
            ' text
            MsgBox("not found")
        End If
        dr.Close()
        DB.connection.Close()

        ' show config button
        Form4.btn_invoice_confirm.Show()

        ' customer enable disable
        check_customer_dropdown()

        Form4.tab_sales.SelectedTab = Form4.tab_invoice
    End Sub

    Private Sub cancel_item()
        Form4.tab_sales.SelectedTab = Form4.tab_sale_page
    End Sub

    ' update customer name button click
    Private Sub update_detail(sender As Object, e As EventArgs)
        update_customer(Form4.t_sales_combo.SelectedValue)
    End Sub

    ' update customer name function
    Sub update_customer(id As Integer)
        query = "update sales set cust_id = '" + id.ToString + "' where check_out=0 "
        cmd = New SqlCommand(query, DB.connection)
        DB.connection.Open()
        cmd.ExecuteNonQuery()
        DB.connection.Close()

        added_to_bill()
    End Sub

    Dim sales_master As Integer

    Private Sub confirm_bill(sender As Object, e As EventArgs)

        ' check checkout button click and something in lbll
        ' ----

        ' insert data in sales_master table

        'Try
        query = "insert into sales_master(cust_id, gold_id, total_sale, sale_date) values(" + cust_id.ToString + "," + gold_id.ToString + "," + total.ToString + ", '" & Date.Now.Date & "')"
        cmd = New SqlCommand(query, DB.connection)

        DB.connection.Open()
        If cmd.ExecuteNonQuery() > 0 Then

            ' get sales_master id
            query = "select sales_master_id from sales_master where sales_master_id = (select max(sales_master_id) from sales_master)"
            cmd = New SqlCommand(query, DB.connection)
            da = New SqlDataAdapter(cmd)
            ds = New DataSet
            da.Fill(ds, "sales_master")
            sales_master = ds.Tables("sales_master").Rows(0).Item(0).ToString

        End If
        DB.connection.Close()

        'select from sales and insert data in sales_detail table 

        query = "select * from sales"
        cmd = New SqlCommand(query, DB.connection)
        da = New SqlDataAdapter(cmd)
        ds = New DataSet
        da.Fill(ds, "sale")

        For i As Integer = 0 To ds.Tables("sale").Rows.Count - 1
            query = "insert into sales_detail(pd_id, sale_price, sales_master_id) values(" + ds.Tables("sale").Rows(i).Item(2).ToString + ", " + ds.Tables("sale").Rows(i).Item(4).ToString + ", " + sales_master.ToString + ")"
            cmd = New SqlCommand(query, DB.connection)
            DB.connection.Open()
            cmd.ExecuteNonQuery()
            DB.connection.Close()
        Next

        ' delete all data from sales table
        query = "delete from sales"
        cmd = New SqlCommand(query, DB.connection)
        DB.connection.Open()
        cmd.ExecuteNonQuery()
        DB.connection.Close()

        MsgBox("Congratulations, Product is sold Successfully !")

        ' pass sales_master_id to function and insert data to bill table
        make_bill(sales_master)

        ' hide config button
        Form4.btn_invoice_confirm.Hide()
        added_to_bill()

        ' sold items
        sold_item_grid()

        MsgBox("Print Bill !")

        Form4.fill_bill(sales_master)

        Form4.tab_sales.SelectedTab = Form4.tab_bill
        'Catch ex As Exception
        '    MsgBox("select item for sale " & ex.Message)
        '    DB.connection.Close()
        'End Try


    End Sub


    ' insret to bill table
    Sub make_bill(sales As Integer)

        'query = "select sm.sales_master_id, sm.total_sale, sm.sale_date, c.cust_name, pm.pm_name, pd.weight, pd.purity,pd.diamonds,pd.making,pd.other_charge, sd.sale_price , g.price from sales_master sm, sales_detail sd, product_detail pd, product_master pm , gold_price g, customer c where sd.sales_master_id=sm.sales_master_id and pd.pd_id = sd.pd_id and pd.pm_id=pm.pm_id and g.gold_id=sm.gold_id and c.cust_id=sm.cust_id and sm.sales_master_id= " + sales.ToString + " "
        'cmd = New SqlCommand(query, DB.connection)
        'Dim dr As SqlDataReader

        'DB.connection.Open()
        'dr = cmd.ExecuteReader

        'If dr.HasRows Then

        '    While dr.Read

        '        query = "insert into bill (sales_master_id,customer,product,weight,purity,diamond,making,other_charge,sale_price,gold_price,total_sale,sale_date) values(" + dr("sales_master_id").ToString + ", '" + dr("cust_name").ToString + "', '" + dr("pm_name").ToString + "', " + dr("weight").ToString + ", " + dr("purity").ToString + ", " + dr("diamonds").ToString + ", " + dr("making").ToString + ", " + dr("other_charge").ToString + ", " + dr("sale_price").ToString + ", " + dr("price").ToString + ", " + dr("total_sale").ToString + ", '" + dr("sale_date").ToString + "' ) "

        '        MsgBox(query)
        '        cmd = New SqlCommand(query, DB.connection)

        '        ' insert
        '        cmd.ExecuteNonQuery()

        '    End While

        'End If


        ' ds.Tables("sale").Rows(i).Item(2).ToString
        query = "select sm.sales_master_id, sm.total_sale, sm.sale_date, c.cust_name, pm.pm_name, pd.weight, pd.purity,pd.diamonds,pd.making,pd.other_charge, sd.sale_price , g.price from sales_master sm, sales_detail sd, product_detail pd, product_master pm , gold_price g, customer c where sd.sales_master_id=sm.sales_master_id and pd.pd_id = sd.pd_id and pd.pm_id=pm.pm_id and g.gold_id=sm.gold_id and c.cust_id=sm.cust_id and sm.sales_master_id= " + sales.ToString + " "
        cmd = New SqlCommand(query, DB.connection)
        da = New SqlDataAdapter(cmd)
        ds = New DataSet
        da.Fill(ds, "sale_bill")

        For i As Integer = 0 To ds.Tables("sale_bill").Rows.Count - 1

            query = "insert into bill (sales_master_id,customer,product,weight,purity,diamond,making,other_charge,sale_price,gold_price,total_sale,sale_date) values(" + ds.Tables("sale_bill").Rows(i).Item("sales_master_id").ToString + ", '" + ds.Tables("sale_bill").Rows(i).Item("cust_name").ToString + "', '" + ds.Tables("sale_bill").Rows(i).Item("pm_name").ToString + "', " + ds.Tables("sale_bill").Rows(i).Item("weight").ToString + ", " + ds.Tables("sale_bill").Rows(i).Item("purity").ToString + ", " + ds.Tables("sale_bill").Rows(i).Item("diamonds").ToString + ", " + ds.Tables("sale_bill").Rows(i).Item("making").ToString + ", " + ds.Tables("sale_bill").Rows(i).Item("other_charge").ToString + ", " + ds.Tables("sale_bill").Rows(i).Item("sale_price").ToString + ", " + ds.Tables("sale_bill").Rows(i).Item("price").ToString + ", " + ds.Tables("sale_bill").Rows(i).Item("total_sale").ToString + ", '" + ds.Tables("sale_bill").Rows(i).Item("sale_date").ToString + "' ) "

            cmd = New SqlCommand(query, DB.connection)
            DB.connection.Open()
            cmd.ExecuteNonQuery()

            DB.connection.Close()
        Next

        
    End Sub

    ' customer select
    Sub check_customer_dropdown()

        query = "select c.cust_name,s.* from sales s, customer c where check_out=0 and c.cust_id = s.cust_id "

        
        cmd = New SqlCommand(query, DB.connection)
        da = New SqlDataAdapter(cmd)
        ds = New DataSet()
        da.Fill(ds, "sale")
        count = ds.Tables("sale").Rows.Count

        If count > 0 Then
            Form4.t_sales_combo.Text = ds.Tables("sale").Rows(0).Item("cust_name")
            Form4.t_sales_combo.Enabled = False
        Else
            Form4.t_sales_combo.Enabled = True
        End If

    End Sub

    ' Sold Item Details
    Sub sold_item_grid()
        query = "select c.cust_name as customer, pm.pm_name as product, pd.weight, g.price as gold_price, sd.sale_price as sale_price, pd.cost_price, sm.sale_date from sales_detail sd, sales_master sm, gold_price g, customer c, product_detail pd, product_master pm where sm.sales_master_id=sd.sales_master_id and sm.gold_id=g.gold_id and sm.cust_id = c.cust_id and pd.pd_id = sd.pd_id and pd.pm_id=pm.pm_id"
        cmd = New SqlCommand(query, DB.connection)
        da = New SqlDataAdapter(cmd)
        ds = New DataSet
        da.Fill(ds, "sales_detail")
        Form4.solditemgrid.DataSource = ds.Tables("sales_detail")
    End Sub


End Class