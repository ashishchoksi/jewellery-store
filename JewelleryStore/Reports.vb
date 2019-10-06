Imports System.Data
Imports System.Data.SqlClient

Public Class Reports

    Dim query As String
    Dim cmd As SqlCommand
    Dim da As SqlDataAdapter
    Dim ds As DataSet

    Public Sub New()
        ' supplier dropdown
        supplier_dropdown()

        ' checkbox date 2 dates
        Form4.purchase_date_search.Enabled = False
        Form4.purchase_date_search_2.Enabled = False
        Form4.supplier_combo.Enabled = False

        ' default purchase grid select
        select_all_purchase()

        ' dropdown product name
        supplier_combo_select()

        ' checkbox date
        Form4.product_report_date.Enabled = False
        Form4.product_report_date_2.Enabled = False
        Form4.product_report_combo.Enabled = False

        ' sales data
        select_all_sales_data()

        ' customer dropdonw
        customer_dropdown()

        ' Sales report 2 dates customer
        Form4.date_sale_report_1.Enabled = False
        Form4.date_sale_report_2.Enabled = False
        Form4.customer_drop_report.Enabled = False

        ' fill combo days
        set_days()

        ' max sales day month year wise
        Form4.max_sale_month.Enabled = False
        Form4.max_sale_year.Enabled = False
        Form4.max_sale_date_1.Enabled = False

        ' area wise search
        Form4.area_date_1.Enabled = False
        Form4.area_date_2.Enabled = False
        area_combo()
        area_wise_data()

    End Sub

    Sub assign_controls()

        ' purchase Search Button Click
        AddHandler Form4.purchase_search.Click, AddressOf purchase_search

        ' supplier wise search
        AddHandler Form4.btn_sup_wise_purchase.Click, AddressOf supplier_wise_search

        ' product report search button click
        AddHandler Form4.product_report_search.Click, AddressOf product_report_search

        ' group by product
        AddHandler Form4.btn_prod_group.Click, AddressOf group_by_product

        ' sales report 1 search button click
        AddHandler Form4.search_sale_report_1.Click, AddressOf sales_report_1

        ' customer group by sales
        AddHandler Form4.customer_sales_group.Click, AddressOf group_by_cust_sales

        ' sales day month year wise search
        AddHandler Form4.sales_day_wise_search.Click, AddressOf sale_report_2

        ' month selection change days
        AddHandler Form4.combo_month.SelectedIndexChanged, AddressOf month_change

        ' max sale date wise
        AddHandler Form4.max_sold_item_search.Click, AddressOf max_sold_items

        ' area wise search
        AddHandler Form4.btn_area_wise.Click, AddressOf search_area_wise

    End Sub

    ' supplier dropdown 
    Sub supplier_dropdown()
        query = "select supplier_id,cname from supplier"
        cmd = New SqlCommand(query, DB.connection)
        da = New SqlDataAdapter(cmd)
        ds = New DataSet
        da.Fill(ds, "supplier")

        Form4.supplier_combo.DataSource = ds.Tables("supplier")
        Form4.supplier_combo.DisplayMember = "cname"
        Form4.supplier_combo.ValueMember = "supplier_id"
    End Sub

    Sub select_all_purchase()
        query = "select * from purchase"
        fill_purchase_grid(query)
    End Sub

    ' purchase report search button click
    Private Sub purchase_search(sender As Object, e As EventArgs)

        ' MsgBox("click")

        Try
            query = "select s.cname as supplier_name, p.weight, p.purity, p.wastage, gp.price gold_price, p.total_price, p.pdate from purchase p, supplier s, gold_price gp where s.supplier_id=p.supplier_id and gp.gold_id = p.gold_id "

            If Form4.CheckBox3.Checked Then
                query &= "and p.supplier_id = " + Form4.supplier_combo.SelectedValue.ToString + " "
            End If

            
                If Form4.CheckBox2.Checked Then
                ' If Form4.purchase_date_search.Text > Form4.purchase_date_search_2.Text Then


                '                  MsgBox("Date 1 must be smaller than Date 2")
                '               Else
                query &= "and pdate between '" + Form4.purchase_date_search.Text.ToString + "' and '" + Form4.purchase_date_search_2.Text.ToString + "' "
                ' End If

            Else
                If Form4.CheckBox1.Checked Then
                    query &= "and pdate = '" + Form4.purchase_date_search.Text.ToString + "' "
                End If
            End If

            fill_purchase_grid(query)


        Catch ex As Exception
            MsgBox("Retry !!")
        End Try
    End Sub

    ' Supplier wise total weight price
    Private Sub supplier_wise_search(sender As Object, e As EventArgs)
        query = "select p.supplier_id, s.cname Supplier_name, sum(weight) Total_Weight, sum(total_price) Total_Price from purchase p, supplier s where s.supplier_id = p.supplier_id group by p.supplier_id, cname "

        If Form4.CheckBox2.Checked Then
            query = "select p.supplier_id, s.cname Supplier_name, sum(weight) Total_Weight, sum(total_price) Total_Price from purchase p, supplier s where s.supplier_id = p.supplier_id and p.pdate between '" + Form4.purchase_date_search.Text.ToString + "' and '" + Form4.purchase_date_search_2.Text.ToString + "' group by p.supplier_id, cname "
        End If

        fill_purchase_grid(query)
    End Sub

    Sub fill_purchase_grid(q As String)
        Try
            cmd = New SqlCommand(q, DB.connection)
            da = New SqlDataAdapter(cmd)
            ds = New DataSet
            da.Fill(ds, "supplier_data")
            Form4.purchase_report_grid.DataSource = ds.Tables("supplier_data")
        Catch ex As Exception
            MsgBox("Retry !!")
        End Try
    End Sub

    ' product combobox
    Sub supplier_combo_select()
        query = "select pm_id, pm_name from product_master"
        cmd = New SqlCommand(query, DB.connection)
        da = New SqlDataAdapter(cmd)
        ds = New DataSet
        da.Fill(ds, "product")
        Form4.product_report_combo.DataSource = ds.Tables("product")
        Form4.product_report_combo.DisplayMember = "pm_name"
        Form4.product_report_combo.ValueMember = "pm_id"
    End Sub

    ' Product Report Search
    Private Sub product_report_search(sender As Object, e As EventArgs)

        Try
            query = "select  pm.pm_name as Product, pd.weight, s.cname supplier, pd.purity, pd.diamonds, pd.making, pd.other_charge, pd.cost_price, pd.purchase_date, pd.added_date from product_detail pd, product_master pm, supplier s where pd.pm_id = pm.pm_id and s.supplier_id = pd.supplier_id "

            
                ' include sold item
                If Form4.checkbox_product.Checked Then

                    If Form4.checkbox_pr_3.Checked Then
                        query &= "and pd.pm_id=" + Form4.product_report_combo.SelectedValue.ToString + " "
                    End If

                Else
                    If Form4.checkbox_pr_3.Checked Then
                        query &= "and pd.pm_id=" + Form4.product_report_combo.SelectedValue.ToString + " "
                    End If
                    query &= "and pd.pd_id not in(select pd_id from sales_detail) "
                End If

                ' date 1 check

                If Form4.checkbox_pr_2.Checked Then

                    query &= "and pd.purchase_date between '" + Form4.product_report_date.Text.ToString + "' and '" + Form4.product_report_date_2.Text.ToString + "' "

                Else
                    If Form4.checkbox_pr.Checked Then
                        query &= "and pd.purchase_date = '" + Form4.product_report_date.Text.ToString + "'"
                    End If
                End If

                ' MsgBox(Form4.product_report_combo.SelectedValue.ToString)
                cmd = New SqlCommand(query, DB.connection)
                da = New SqlDataAdapter(cmd)
                ds = New DataSet
                da.Fill(ds, "product_data")
                Form4.product_report_gridview.DataSource = ds.Tables("product_data")


        Catch ex As Exception
            MsgBox("something going wrong !")
        End Try

    End Sub

    ' group by product

    Private Sub group_by_product(sender As Object, e As EventArgs)
        Try
            If Form4.checkbox_product.Checked Then
                query = "select pm.pm_name, count(pd.pm_id) Qty, sum(pd.weight) total_weight from product_detail pd, product_master pm where pd.pm_id = pm.pm_id group by pd.pm_id, pm.pm_name "

            Else
                query = "select pm.pm_name, count(pd.pm_id) Qty, sum(pd.weight) total_weight from product_detail pd, product_master pm where pd.pm_id = pm.pm_id and pd.pd_id not in (select pd_id from sales_detail) group by pd.pm_id, pm.pm_name"
            End If
            cmd = New SqlCommand(query, DB.connection)
            da = New SqlDataAdapter(cmd)
            ds = New DataSet
            da.Fill(ds, "product_data")
            Form4.product_report_gridview.DataSource = ds.Tables("product_data")
        Catch ex As Exception
            MsgBox("Something going wrong !!")
        End Try
    End Sub


    ' -----------  Sales ----------------------

    Sub select_all_sales_data()
        query = "select sd.sales_id, sd.sales_master_id, pm.pm_name as product, pd.weight, c.cust_name as customer, pd.cost_price as cost_price, sd.sale_price as sale_price, (sd.sale_price - pd.cost_price) as profit, pd.purchase_date purchase_date, sm.sale_date from sales_detail sd, sales_master sm, product_detail pd, product_master pm, customer c where sm.sales_master_id = sd.sales_master_id and pd.pm_id = pm.pm_id and c.cust_id = sm.cust_id and sd.pd_id = pd.pd_id"
        cmd = New SqlCommand(query, DB.connection)
        da = New SqlDataAdapter(cmd)
        ds = New DataSet
        da.Fill(ds, "sale")
        Form4.sales_grid_report_1.DataSource = ds.Tables("sale")

        Form4.sale_day_month_grid.DataSource = ds.Tables("sale")

    End Sub

    ' customer drop down
    Sub customer_dropdown()
        query = "select cust_id, cust_name from customer"
        cmd = New SqlCommand(query, DB.connection)
        da = New SqlDataAdapter(cmd)
        ds = New DataSet
        da.Fill(ds, "customer_data")
        Form4.customer_drop_report.DataSource = ds.Tables("customer_data")
        Form4.customer_drop_report.DisplayMember = "cust_name"
        Form4.customer_drop_report.ValueMember = "cust_id"
    End Sub

    Private Sub sales_report_1(sender As Object, e As EventArgs)

        Try

            query = "select sd.sales_id, sd.sales_master_id, pm.pm_name as product, pd.weight, c.cust_name as customer, pd.cost_price as cost_price, sd.sale_price as sale_price, (sd.sale_price - pd.cost_price) as profit, pd.purchase_date purchase_date, sm.sale_date from sales_detail sd, sales_master sm, product_detail pd, product_master pm, customer c where sm.sales_master_id = sd.sales_master_id and pd.pm_id = pm.pm_id and c.cust_id = sm.cust_id and sd.pd_id = pd.pd_id " ' and c.cust_id = " + Form4.customer_drop_report.SelectedValue.ToString + " "

            '  If Form4.date_sale_report_1.Text > Form4.date_sale_report_2.Text Then
            'MsgBox("date 1 must be smaller than date 2")
            'Else

                ' select customer
                If Form4.check_sale_customer.Checked Then
                    query &= "and c.cust_id = " + Form4.customer_drop_report.SelectedValue.ToString + " "
                End If

                ' select date 2
                If Form4.check_sale_report_2.Checked Then
                    query &= "and sm.sale_date between '" + Form4.date_sale_report_1.Text.ToString + "' and '" + Form4.date_sale_report_2.Text.ToString + "' "

                Else
                    ' select date 1
                    If Form4.check_sale_report_1.Checked Then
                        query &= "and sm.sale_date = '" + Form4.date_sale_report_1.Text.ToString + "' "
                    End If

                End If

                cmd = New SqlCommand(query, DB.connection)
                da = New SqlDataAdapter(cmd)
                ds = New DataSet
                da.Fill(ds, "sale")
                Form4.sales_grid_report_1.DataSource = ds.Tables("sale")

                ' End If

        Catch ex As Exception
            'MsgBox(query)
            MsgBox("something going wrong !" & ex.Message)
        End Try

    End Sub

    ' Group By Customer Sales
    Private Sub group_by_cust_sales(sender As Object, e As EventArgs)
        Try
            query = "select c.cust_id, c.cust_name, sum(sm.total_sale) as Total, count(*) Items, sum( sd.sale_price - pd.cost_price) profit from sales_detail sd, sales_master sm, customer c, product_detail pd where sm.sales_master_id = sd.sales_master_id and c.cust_id = sm.cust_id and sd.pd_id = pd.pd_id group by c.cust_name , c.cust_id order by sum(sm.total_sale) desc"

            ' If Form4.date_sale_report_1.Text > Form4.date_sale_report_2.Text Then
            'MsgBox("date 1 must be smaller than date 2")

            'Else

            ' date 2 selected
            If Form4.check_sale_report_2.Checked Then
                query = "select c.cust_id, c.cust_name, sum(sm.total_sale) as Total, count(*) Items, sum( sd.sale_price - pd.cost_price) profit from sales_detail sd, sales_master sm, customer c, product_detail pd where sm.sales_master_id = sd.sales_master_id and c.cust_id = sm.cust_id and sd.pd_id = pd.pd_id and sm.sale_date between '" + Form4.date_sale_report_1.Text.ToString + "' and '" + Form4.date_sale_report_2.Text.ToString + "' group by c.cust_name , c.cust_id order by sum(sm.total_sale) desc"
            End If

            cmd = New SqlCommand(query, DB.connection)
            da = New SqlDataAdapter(cmd)
            ds = New DataSet
            da.Fill(ds, "sale")
            Form4.sales_grid_report_1.DataSource = ds.Tables("sale")
            'End If

        Catch ex As Exception
            MsgBox("Something Going Wrong !")
        End Try
    End Sub


    ' set years
    Sub set_days()
        Dim last As Integer = 0

        If Form4.combo_month.Text <> "" Then

            If Form4.combo_month.Text.ToString Mod 2 = 0 Then
                last = 30
            Else
                last = 31
            End If

            If Form4.combo_month.Text.ToString = 2 Then
                last = 28
            End If

            If Form4.combo_month.Text.ToString = 8 Then
                last = 31
            End If

        Else
            last = 31
        End If

        If Form4.combo_day.Items.Count <> 0 Then
            Form4.combo_day.Items.Clear()
        End If
        For i As Integer = 1 To last
            Form4.combo_day.Items.Add(i)
        Next

    End Sub

    Private Sub month_change(sender As Object, e As EventArgs)
        set_days()
    End Sub

    Private Sub sale_report_2(sender As Object, e As EventArgs)

        Try
            query = "select sd.sales_id, sd.sales_master_id, pm.pm_name as product, pd.weight, c.cust_name as customer, sd.sale_price as sale_price, pd.cost_price as cost_price, (sd.sale_price - pd.cost_price) profit, pd.purchase_date, sm.sale_date from sales_detail sd, sales_master sm, product_detail pd, product_master pm, customer c where sm.sales_master_id = sd.sales_master_id and pd.pm_id = pm.pm_id and c.cust_id = sm.cust_id and sd.pd_id = pd.pd_id "

            If Form4.combo_day.Text <> "" Then
                query &= "and day(sm.sale_date) = " + Form4.combo_day.Text + " "
            End If

            If Form4.combo_month.Text <> "" Then
                query &= "and month(sm.sale_date) = " + Form4.combo_month.Text + " "
            End If

            If Form4.combo_year.Text <> "" Then
                query &= "and year(sm.sale_date) = " + Form4.combo_year.Text + " "
            End If

            cmd = New SqlCommand(query, DB.connection)
            da = New SqlDataAdapter(cmd)
            ds = New DataSet
            da.Fill(ds, "sale_data")

            Form4.sale_day_month_grid.DataSource = ds.Tables("sale_data")

        Catch ex As Exception
            MsgBox("Something Going Wrong !!!")
        End Try

    End Sub

    ' max sold item day month year wise
    Private Sub max_sold_items(sender As Object, e As EventArgs)

        Try
            query = "select pm.pm_name Product_Name, count(*) Qty_Sold, sd.sale_price, sum(sd.sale_price - pd.cost_price) profit, sm.sale_date from sales_detail sd, product_detail pd, product_master pm, sales_master sm where pd.pd_id = sd.pd_id and pm.pm_id = pd.pm_id and sd.sales_master_id = sm.sales_master_id group by pm.pm_name,sm.sale_date, sd.sale_price order by count(*) desc"

            ' day select
            If Form4.max_sale_check_1.Checked Then
                query = "select pm.pm_name Product_Name, count(*) Qty_Sold, sd.sale_price, sum(sd.sale_price - pd.cost_price) profit, sm.sale_date from sales_detail sd, product_detail pd, product_master pm, sales_master sm where pd.pd_id = sd.pd_id and pm.pm_id = pd.pm_id and sd.sales_master_id = sm.sales_master_id and sm.sale_date = '" + Form4.max_sale_date_1.Text + "' group by pm.pm_name,sm.sale_date, sd.sale_price order by count(*) desc"
            End If

            ' month select
            If Form4.max_sale_check_2.Checked Then
                query = "select pm.pm_name Product_Name, count(*) Qty_Sold,  sd.sale_price, sum(sd.sale_price - pd.cost_price) profit, sm.sale_date from sales_detail sd, product_detail pd, product_master pm, sales_master sm where pd.pd_id = sd.pd_id and pm.pm_id = pd.pm_id and sd.sales_master_id = sm.sales_master_id and month(sm.sale_date) = " + Form4.max_sale_month.Text.ToString + " group by pm.pm_name,sm.sale_date, sd.sale_price order by count(*) desc"

                ' year select
                If Form4.max_sale_check_3.Checked Then
                    query = "select pm.pm_name Product_Name, count(*) Qty_Sold, sd.sale_price, sum(sd.sale_price - pd.cost_price) profit, sm.sale_date from sales_detail sd, product_detail pd, product_master pm, sales_master sm where pd.pd_id = sd.pd_id and pm.pm_id = pd.pm_id and sd.sales_master_id = sm.sales_master_id and year(sm.sale_date) = " + Form4.max_sale_year.Text.ToString + " and month(sm.sale_date) = " + Form4.max_sale_month.Text.ToString + " group by pm.pm_name,sm.sale_date, sd.sale_price order by count(*) desc"
                End If

            Else
                ' year select
                If Form4.max_sale_check_3.Checked Then
                    query = "select pm.pm_name Product_Name, count(*) Qty_Sold, sd.sale_price, sum(sd.sale_price - pd.cost_price) profit, sm.sale_date from sales_detail sd, product_detail pd, product_master pm, sales_master sm where pd.pd_id = sd.pd_id and pm.pm_id = pd.pm_id and sd.sales_master_id = sm.sales_master_id and year(sm.sale_date) = " + Form4.max_sale_year.Text.ToString + " group by pm.pm_name,sm.sale_date, sd.sale_price order by count(*) desc"
                End If

            End If

            cmd = New SqlCommand(query, DB.connection)
            da = New SqlDataAdapter(cmd)
            ds = New DataSet
            da.Fill(ds, "sale_data")
            Form4.max_sold_item_grid.DataSource = ds.Tables("sale_data")

        Catch ex As Exception
            MsgBox("Something Going Wrong !!")
        End Try
    End Sub

    Sub area_combo()
        query = "select distinct(cust_address) from customer"
        cmd = New SqlCommand(query, DB.connection)
        da = New SqlDataAdapter(cmd)
        ds = New DataSet
        da.Fill(ds)

        Form4.combo_area.DataSource = ds.Tables(0)
        Form4.combo_area.DisplayMember = "cust_address"
        'Form4.combo_area.ValueMember = "cust_id"
    End Sub

    Sub area_wise_data()

        query = "select c.cust_name as customer, c.cust_address as address, pm.pm_name as product, pd.weight,  pd.cost_price as cost_price, sd.sale_price as sale_price, (sd.sale_price - pd.cost_price) as profit, pd.purchase_date purchase_date, sm.sale_date from sales_detail sd, sales_master sm, product_detail pd, product_master pm, customer c where sm.sales_master_id = sd.sales_master_id and pd.pm_id = pm.pm_id and c.cust_id = sm.cust_id and sd.pd_id = pd.pd_id"
        cmd = New SqlCommand(query, DB.connection)
        da = New SqlDataAdapter(cmd)
        ds = New DataSet
        da.Fill(ds)
        Form4.area_wise_sale_grid.DataSource = ds.Tables(0)
    End Sub

    ' click search on area wise search
    Private Sub search_area_wise()
        'MsgBox(Form4.combo_area.SelectedValue)

        query = "select c.cust_name as customer, c.cust_address as address, pm.pm_name as product, pd.weight,  pd.cost_price as cost_price, sd.sale_price as sale_price, (sd.sale_price - pd.cost_price) as profit, pd.purchase_date purchase_date, sm.sale_date from sales_detail sd, sales_master sm, product_detail pd, product_master pm, customer c where sm.sales_master_id = sd.sales_master_id and pd.pm_id = pm.pm_id and c.cust_id = sm.cust_id and sd.pd_id = pd.pd_id and cust_address = '" + Form4.combo_area.Text + "' "

        If Form4.CheckBox5.Checked Then
            'query = "select c.cust_name as customer, c.cust_address as address, pm.pm_name as product, pd.weight,  pd.cost_price as cost_price, sd.sale_price as sale_price, (sd.sale_price - pd.cost_price) as profit, pd.purchase_date purchase_date, sm.sale_date from sales_detail sd, sales_master sm, product_detail pd, product_master pm, customer c where sm.sales_master_id = sd.sales_master_id and pd.pm_id = pm.pm_id and c.cust_id = sm.cust_id and sd.pd_id = pd.pd_id and cust_address = '" + Form4.combo_area.Text + "' "
            query &= "and sm.sale_date between '" + Form4.area_date_1.Text.ToString + "' and '" + Form4.area_date_2.Text.ToString + "' "

        Else
            If Form4.CheckBox4.Checked Then
                query &= "and sm.sale_date = '" + Form4.area_date_1.Text + "' "
            End If

        End If
        cmd = New SqlCommand(query, DB.connection)
        da = New SqlDataAdapter(cmd)
        ds = New DataSet
        da.Fill(ds)
        Form4.area_wise_sale_grid.DataSource = ds.Tables(0)
    End Sub


    



End Class
