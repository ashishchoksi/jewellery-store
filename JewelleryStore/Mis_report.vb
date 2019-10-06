Imports System.Data.SqlClient


Public Class Mis_report

    Dim query As String
    Dim cmd As SqlCommand
    Dim con As SqlConnection
    Dim ds As DataSet
    Dim da As SqlDataAdapter

    Sub New()
        ' area combobox
        area_combo()
        year_combo()

        ' supplier name combobox
        supplier_combo()

        ' stock run out
        stock_run_out()
    End Sub

    ' assign control
    Sub assign_controls()
        AddHandler Form4.search_area_sales.Click, AddressOf sales_on_area

        AddHandler Form4.btn_supplier_wise.Click, AddressOf supplier_wise_sales

        AddHandler Form4.btn_yearly_search.Click, AddressOf yearly_sales

        AddHandler Form4.btn_inverst_return.Click, AddressOf inverst_return

        AddHandler Form4.btn_purchase_sale.Click, AddressOf purchase_and_sales

        AddHandler Form4.btn_monthly_sales.Click, AddressOf monthly_sales

        AddHandler Form4.btn_month_purchase_sales.Click, AddressOf monthly_purchase_sales

        AddHandler Form4.btn_product_wise_sales.Click, AddressOf product_wise_Sales

        AddHandler Form4.supplier_wise_stock.Click, AddressOf supplier_wise_stock

    End Sub


    ' combo box of area

    Sub area_combo()
        query = "select distinct(cust_address) from customer"

        cmd = New SqlCommand(query, DB.connection)
        da = New SqlDataAdapter(cmd)
        ds = New DataSet
        da.Fill(ds)

        Form4.customer_area.DataSource = ds.Tables(0)
        Form4.customer_area.DisplayMember = "cust_address"
        'Form4.combo_area.ValueMember = "cust_id"
    End Sub

    Sub year_combo()
        query = "select distinct( year(sale_date) ) years from sales_master"

        cmd = New SqlCommand(query, DB.connection)
        da = New SqlDataAdapter(cmd)
        ds = New DataSet
        da.Fill(ds)

        Form4.year_combo.DataSource = ds.Tables(0)
        Form4.year_combo.DisplayMember = "years"

        Form4.year_combo2.DataSource = ds.Tables(0)
        Form4.year_combo2.DisplayMember = "years"

        Form4.year_combo3.DataSource = ds.Tables(0)
        Form4.year_combo3.DisplayMember = "years"

        Form4.year_combo4.DataSource = ds.Tables(0)
        Form4.year_combo4.DisplayMember = "years"

        Form4.year_combo5.DataSource = ds.Tables(0)
        Form4.year_combo5.DisplayMember = "years"

        Form4.year_combo6.DataSource = ds.Tables(0)
        Form4.year_combo6.DisplayMember = "years"

        Form4.year_combo7.DataSource = ds.Tables(0)
        Form4.year_combo7.DisplayMember = "years"

        Form4.year_combo8.DataSource = ds.Tables(0)
        Form4.year_combo8.DisplayMember = "years"

        Form4.year_combo9.DataSource = ds.Tables(0)
        Form4.year_combo9.DisplayMember = "years"

        Form4.year_combo10.DataSource = ds.Tables(0)
        Form4.year_combo10.DisplayMember = "years"

    End Sub

    ' select supplier combo box
    Sub supplier_combo()
        query = "select supplier_id,cname from supplier where supplier_id in (select supplier_id from product_detail)"
        cmd = New SqlCommand(query, DB.connection)
        da = New SqlDataAdapter(cmd)
        ds = New DataSet
        da.Fill(ds, "supplier")

        Form4.supplier_combo2.DataSource = ds.Tables("supplier")
        Form4.supplier_combo2.DisplayMember = "cname"
        Form4.supplier_combo2.ValueMember = "supplier_id"
    End Sub

    ' 1st report search click
    Private Sub sales_on_area(sender As Object, e As EventArgs)

        Try
            Dim report As New selling_area
            'report.Load()
            'report.SetParameterValue("address", Form4.customer_area.Text)

            ' and c.cust_address = '"+customer_area.Text.tostring+"'
            query = "select pm.pm_id, pm.pm_name, count(pd.pm_id) Qty, c.cust_address, sum(total_sale) total_sale from sales_detail sd, sales_master sm, customer c, product_detail pd, product_master pm where sd.sales_master_id = sm.sales_master_id and c.cust_id=sm.cust_id and pd.pd_id = sd.pd_id and pm.pm_id=pd.pm_id and c.cust_address = '" + Form4.customer_area.Text.ToString + "'  and year(sm.sale_date) = " + Form4.year_combo.Text.ToString + " group by pd.pm_id , pm.pm_name, c.cust_address, pm.pm_id"
            cmd = New SqlCommand(query, DB.connection)
            da = New SqlDataAdapter(cmd)
            ds = New DataSet
            da.Fill(ds)
            report.SetDataSource(ds.Tables(0))
            Form4.crystal_area_wise_prod.ReportSource = report

        Catch ex As Exception
            MsgBox("Something going wrong !")
        End Try
        
    End Sub

    ' supplier wise sales button
    Private Sub supplier_wise_sales(sender As Object, e As EventArgs)

        Try
            Dim report As New Supplier_wise_sales
            query = "select pd.pm_id, pm.pm_name, count(*) Qty, p.supplier_id, s.cname, sum(sm.total_sale) as total_sale  from sales_detail sd, sales_master sm, product_detail pd, product_master pm, purchase p, supplier s where sd.sales_master_id=sm.sales_master_id and pd.pd_id=sd.pd_id and pm.pm_id=pd.pm_id and p.purchase_id = pd.purchase_id and s.supplier_id = p.supplier_id and s.supplier_id= " + Form4.supplier_combo2.SelectedValue.ToString + " and year(sm.sale_date) = " + Form4.year_combo2.Text.ToString + " group by pd.pm_id, pm.pm_name, p.supplier_id, s.cname"
            cmd = New SqlCommand(query, DB.connection)
            da = New SqlDataAdapter(cmd)
            ds = New DataSet
            da.Fill(ds)
            report.SetDataSource(ds.Tables(0))
            Form4.supplier_wise_sold.ReportSource = report

        Catch ex As Exception
            MsgBox("Something going wrong !")
        End Try

    End Sub

    ' yearly sales button click
    Private Sub yearly_sales(sender As Object, e As EventArgs)
        Try
            Dim report As New yearly_report
            query = "select  pm.pm_name, count(pd.pm_id) Qty, sum(sm.total_sale) total_sale, pd.pm_id, sum( sd.sale_price - pd.cost_price ) profit from sales_detail sd, sales_master sm, product_detail pd, product_master pm where sd.sales_master_id= sm.sales_master_id and sd.pd_id=pd.pd_id and pm.pm_id=pd.pm_id and year(sm.sale_date) = " + Form4.year_combo3.Text.ToString + " group by pd.pm_id, pm.pm_name "
            cmd = New SqlCommand(query, DB.connection)
            da = New SqlDataAdapter(cmd)
            ds = New DataSet
            da.Fill(ds)
            
            report.SetDataSource(ds.Tables(0))
            Form4.yearly_report.ReportSource = report

        Catch ex As Exception
            MsgBox("Something going wrong !")
        End Try
    End Sub


    Sub stock_run_out()


        Dim query1 As String = "select pm.pm_name as product, (select count(*) from product_detail where pm_id = pm.pm_id and pd_id not in (select pd_id from sales_detail)) Qty from product_master pm"
        Dim cmd1 As New SqlCommand(query1, DB.connection)
        Dim da1 As New SqlDataAdapter(cmd1)
        Dim ds1 As New DataSet
        da1.Fill(ds1)
        Form4.stock_grid.DataSource = ds1.Tables(0)


        Dim row As Integer = 0
        row = Form4.stock_grid.Rows.Count - 1

        For index = 0 To row - 1
            If Form4.stock_grid.Rows(index).Cells(1).Value <= 1 Then
                Form4.stock_grid.Rows(index).Cells(1).Style.BackColor = Color.Red
            End If
        Next
    End Sub

    ' search year wise inverst and returns
    Private Sub inverst_return(sender As Object, e As EventArgs)
        Try
            Dim report As New return_of_invest
            'query = "select pd.pm_id, pm.pm_name, sum(cost_price) invest, ISNULL( (select  sum(sd1.sale_price) from sales_detail sd1, product_detail pd1, product_master pm1 where pd1.pd_id=sd1.pd_id and pm1.pm_id=pd1.pm_id and pm1.pm_id = pd.pm_id and year(pd1.purchase_date) = " + Form4.year_combo4.Text.ToString + " group by pd1.pm_id, pm1.pm_name, pm1.pm_id),0 )as return_of_inverts , ISNULL( (select sum(sd2.sale_price - pd2.cost_price) profit from sales_detail sd2, product_detail pd2, product_master pm2, sales_master sm2 where pd2.pd_id = sd2.pd_id and pm2.pm_id = pd2.pm_id and sd2.sales_master_id = sm2.sales_master_id and pd2.pm_id = pd.pm_id and year(sm2.sale_date) = " + Form4.year_combo4.Text.ToString + ") , 0) as profit from product_detail pd, product_master pm, sales_detail sd where pm.pm_id=pd.pm_id and year(pd.purchase_date) = " + Form4.year_combo4.Text.ToString + " group by pd.pm_id, pm.pm_name"

            query = "select pm.pm_name, pd.pm_id, sum(pd.cost_price) invest, ISNULL((select sum(sd1.sale_price) from sales_detail sd1, sales_master sm1, product_detail pd1 where sm1.sales_master_id=sd1.sales_master_id and pd1.pd_id=sd1.pd_id and pd1.pm_id = pd.pm_id and year(sm1.sale_date) = " + Form4.year_combo4.Text.ToString + "),0) return_of_inverts, ISNULL( (select sum(sd1.sale_price - pd1.cost_price) from sales_detail sd1, sales_master sm1, product_detail pd1 where sm1.sales_master_id=sd1.sales_master_id and pd1.pd_id=sd1.pd_id and pd1.pm_id = pd.pm_id and year(sm1.sale_date) = " + Form4.year_combo4.Text.ToString + " ) ,0) profit from product_detail pd, product_master pm where pm.pm_id=pd.pm_id and year(pd.purchase_date) = " + Form4.year_combo4.Text.ToString + " group by pm.pm_name, pd.pm_id "
            cmd = New SqlCommand(query, DB.connection)
            da = New SqlDataAdapter(cmd)
            ds = New DataSet
            da.Fill(ds)

            report.SetDataSource(ds.Tables(0))
            Form4.return_inverst.ReportSource = report

        Catch ex As Exception
            MsgBox("Something going wrong !")
        End Try
    End Sub

    ' purchase and sales btn click
    Private Sub purchase_and_sales(sender As Object, e As EventArgs)
        Try

            If Form4.year_combo5.Text <= Form4.year_combo6.Text Then
                Dim report As New purchase_sales
                query = "select pd.pm_id, pm.pm_name, count(pd.pm_id) purchase_qty , ISNULL( (select count(pd1.pm_id) Qty from sales_master sm1, sales_detail sd1, product_detail pd1, product_master pm1 where sd1.sales_master_id=sm1.sales_master_id and pd1.pd_id=sd1.pd_id and pm1.pm_id=pd1.pm_id and year(sm1.sale_date) = " + Form4.year_combo6.Text.ToString + " and pd1.pm_id=pd.pm_id ) ,0) sold_qty from product_detail pd, product_master pm where pm.pm_id=pd.pm_id and year(pd.purchase_date) = " + Form4.year_combo5.Text.ToString + " group by pd.pm_id, pm.pm_name"
                cmd = New SqlCommand(query, DB.connection)
                da = New SqlDataAdapter(cmd)
                ds = New DataSet
                da.Fill(ds)

                report.SetDataSource(ds.Tables(0))
                Form4.purchase_sales_report.ReportSource = report

            Else
                MsgBox("Purchase year must smaller or equal to Sales year")
            End If


        Catch ex As Exception
            MsgBox("Something going wrong !")
        End Try

    End Sub

    Private Sub monthly_sales(sender As Object, e As EventArgs)
        Try
            Dim report As New monthly_sales
            query = "select count(pd.pm_id) Total_qty, month(sm.sale_date) as Month, sum(sm.total_sale) as Total_sales, sum(sd.sale_price - pd.cost_price) as Profit from sales_detail sd, product_detail pd, product_master pm, sales_master sm where pd.pd_id=sd.pd_id and pm.pm_id=pd.pm_id and sm.sales_master_id=sd.sales_master_id  and year(sm.sale_date) = " + Form4.year_combo7.Text.ToString + " group by  month(sm.sale_date) order by Total_sales desc "
            cmd = New SqlCommand(query, DB.connection)
            da = New SqlDataAdapter(cmd)
            ds = New DataSet
            da.Fill(ds)

            report.SetDataSource(ds.Tables(0))
            Form4.monthly_sales.ReportSource = report

        Catch ex As Exception
            MsgBox("Something going wrong !")
        End Try
    End Sub


    ' monthly purchasr and sales data
    Private Sub monthly_purchase_sales(sender As Object, e As EventArgs)
        Try
            Dim report As New monthly_purchase_sales
            query = "select month(sm.sale_date) as Month , count(pd.pm_id) Total_qty, sum(pd.cost_price) as Total_purchase, sum(sm.total_sale) as Total_sales from sales_detail sd, product_detail pd, product_master pm, sales_master sm  where pd.pd_id=sd.pd_id and pm.pm_id=pd.pm_id and sm.sales_master_id=sd.sales_master_id and year(sm.sale_date) = " + Form4.year_combo8.Text.ToString + " group by  month(sm.sale_date)  "
            cmd = New SqlCommand(query, DB.connection)
            da = New SqlDataAdapter(cmd)
            ds = New DataSet
            da.Fill(ds)

            report.SetDataSource(ds.Tables(0))
            Form4.monthly_purchase_sales.ReportSource = report

        Catch ex As Exception
            MsgBox("Something going wrong !")
        End Try

    End Sub

    ' product wise sales btn click
    Private Sub product_wise_Sales(sender As Object, e As EventArgs)
        Try
            Dim report As New product_wise_sales
            query = "select pm1.pm_name as product,  ISNULL( ( select count(pd.pm_id) as Qty from sales_detail sd, sales_master sm, product_detail pd, product_master pm where sd.sales_master_id=sm.sales_master_id and pd.pd_id=sd.pd_id and pm.pm_id=pd.pm_id and pd.pm_id = pm1.pm_id and year(sm.sale_date) = " + Form4.year_combo9.Text.ToString + " ),0) as Qty, ISNULL( ( select avg(pd.weight) weight from sales_detail sd, sales_master sm, product_detail pd, product_master pm where sd.sales_master_id=sm.sales_master_id and pd.pd_id=sd.pd_id and pm.pm_id=pd.pm_id and pd.pm_id = pm1.pm_id and year(sm.sale_date) = " + Form4.year_combo9.Text.ToString + " ),0) as weight, ISNULL( ( select avg(sd.sale_price) sales from sales_detail sd, sales_master sm, product_detail pd, product_master pm where sd.sales_master_id=sm.sales_master_id and pd.pd_id=sd.pd_id and pm.pm_id=pd.pm_id and pd.pm_id = pm1.pm_id and year(sm.sale_date) = " + Form4.year_combo9.Text.ToString + "),0) as sales, ISNULL( ( select avg(sd.sale_price - pd.cost_price) profit from sales_detail sd, sales_master sm, product_detail pd, product_master pm where sd.sales_master_id=sm.sales_master_id and pd.pd_id=sd.pd_id and pm.pm_id=pd.pm_id and pd.pm_id = pm1.pm_id and year(sm.sale_date) = " + Form4.year_combo9.Text.ToString + " ),0) as profit from product_master pm1"
            cmd = New SqlCommand(query, DB.connection)
            da = New SqlDataAdapter(cmd)
            ds = New DataSet
            da.Fill(ds)

            report.SetDataSource(ds.Tables(0))
            Form4.product_wise_sales.ReportSource = report

        Catch ex As Exception
            MsgBox("Something going wrong !")
        End Try

    End Sub

    ' supplier wise available stock
    Private Sub supplier_wise_stock(sender As Object, e As EventArgs)


        Try
            Dim report As New supplier_wise_stock
            query = "select p.supplier_id, s.cname, count(*) Qty, sum(pd.weight) Weight from product_detail pd, product_master pm, purchase p, supplier s where pm.pm_id=pd.pm_id and p.purchase_id = pd.purchase_id and s.supplier_id=p.supplier_id and pd.pd_id not in (select pd_id from sales_detail) and year(pd.purchase_date) = " + Form4.year_combo10.Text.ToString + " group by p.supplier_id, s.cname"
            cmd = New SqlCommand(query, DB.connection)
            da = New SqlDataAdapter(cmd)
            ds = New DataSet
            da.Fill(ds)

            report.SetDataSource(ds.Tables(0))
            Form4.supplier_wise_available_stock.ReportSource = report

        Catch ex As Exception
            MsgBox("Something going wrong !")
        End Try
    End Sub

    
    
End Class
