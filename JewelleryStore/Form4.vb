Imports System.Data
Imports System.Data.SqlClient
Imports System.Text
Imports System.Windows.Forms
Imports System.Drawing
Imports System.Math

Imports CrystalDecisions.CrystalReports.Engine

Public Class Form4

    ' date set count
    Dim is_date_set As Integer = 0

    ' supplier data auto fill count
    Public auto_fill_sup As Integer = 0

    Dim msg As String = ""
    Dim grid As String = ""

    Dim flag As Integer = 0

    Private bm As Bitmap
    Private bm1 As Bitmap

    ' print variables

    Private strformate As StringFormat
    Private arrColumnLefts As New ArrayList
    Private arrColumnWidths As New ArrayList
    Private iCellHeight As Integer = 0
    Private iTotalWidth As Integer = 0

    Private iRow As Integer = 0
    Private bFirstpage As Boolean = False
    Private bNewpag As Boolean = False
    Private iHeaderHeight As Integer = 0


    ' ----------------------------------------------


    Dim purchase_const As Integer = 1
    Public today_gold_price = 0, gold_id As Integer = 0

    ' Purchase Object
    Dim purchase As New Purchase

    ' Customer object
    Dim customer As Customer
    ' product calss
    Dim product As Product

    Dim sales As Sales

    Dim report As Reports

    Dim bill As Bill

    Dim mis_report As Mis_report

    ' Home Panel Clicked
    Private Sub btnhome_Click(sender As Object, e As EventArgs) Handles btnhome.Click
        Panels.show("home")

        ' home panel stock
        group_by_product()
    End Sub


    ' set new Gold Price Here
    Private Sub btnsetprice_Click(sender As Object, e As EventArgs) Handles btnsetprice.Click

        ' need to done only int number only input

        If gold_price.Text.Length = 5 Then

            If Not IsNumeric(gold_price.Text) Then
                MsgBox("Enter Only Number !")
            Else
                Dim str As String = ""
                Dim cmd As New SqlCommand("insert into gold_price(price, cdate) values(" + gold_price.Text + ",'" + Date.Now.Date + "') ", DB.connection)

                DB.connection.Open()
                If cmd.ExecuteNonQuery > 0 Then
                    MsgBox("New Gold Price Set")
                    ' set price flag
                    is_date_set += 1
                    set_alert_for_date()

                    gold_price.Text = ""
                Else
                    MsgBox("Something goes wrong !")
                End If
                DB.connection.Close()
                select_gold_price()

            End If

        Else
            MsgBox("Enter 5 Digit Gold Price 10 Gram, 24 Karat ")
        End If
    End Sub


    ' alert for daily gold price set
    Function check_daily_price() As Integer

        Dim query As String = "select * from gold_price where cdate = '" + Date.Now.Date + "' "
        Dim cmd As New SqlCommand(query, DB.connection)
        Dim da As New SqlDataAdapter(cmd)
        Dim ds As New DataSet
        da.Fill(ds)

        ' MsgBox("total rows : " & ds.Tables(0).Rows.Count)

        Return ds.Tables(0).Rows.Count

    End Function


    ' set alert message
    Sub set_alert_for_date()
        Dim alert_str As String = "You Not Set Gold Price Today !"
        If is_date_set = 0 Then
            date_alert_1.Text = alert_str
            date_set_2.Text = alert_str
            date_set_3.Text = alert_str
            date_set_4.Text = alert_str
            date_set_5.Text = alert_str
            date_set_6.Text = alert_str
        Else
            date_alert_1.Text = ""
            date_set_2.Text = ""
            date_set_3.Text = ""
            date_set_4.Text = ""
            date_set_5.Text = ""
            date_set_6.Text = ""
        End If

    End Sub

    ' for selecting gold price display in label
    Sub select_gold_price()
        Dim cmd As New SqlCommand("select gold_id, price from gold_price where gold_id = (select max(gold_id) from gold_price)", DB.connection)
        DB.connection.Open()
        Dim dr As SqlDataReader = cmd.ExecuteReader()

        While dr.Read
            gprice.Text = "Last Price is : " & dr("price").ToString
            today_gold_price = dr("price").ToString
            gold_id = dr("gold_id").ToString

            Me.gold_price_select.Text = today_gold_price

        End While
        dr.Close()
        DB.connection.Close()
    End Sub

    ' close form event
    Private Sub close_form1(sender As Object, e As EventArgs) Handles Me.FormClosing
        Form1.Close()
    End Sub

    ' form load event
    Private Sub Form4_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        'MsgBox(MsgBox("msg", MsgBoxStyle.OkCancel))

        ' check daily gold price is set
        is_date_set = check_daily_price()
        set_alert_for_date()

        auto_fill_sup = 0

        select_gold_price()

        ' hide show panels
        Panels.show("home")

        ' purchase constructor
        product = New Product

        ' Customer Object
        customer = New Customer

        ' sales Object
        sales = New Sales

        ' item count
        group_by_product()

        ' home panel date
        homedate.Enabled = False
    End Sub


    Private Sub btnpurchase_Click(sender As Object, e As EventArgs) Handles btnpurchase.Click

        ' hide show panels
        Panels.show("purchase")

        ' select Suppliers gridview
        purchase.refSupplier()

        ' hide update supplier button
        btnupdateSupplier.Hide()
        btn_del_supplier.Hide()
        addSupplier.Show()

        ' cancel selected supplier
        purchase.cancle_supplier()

        'supplier combobox in purchase
        purchase.supplier_dropdown()

        ' select purchase data gridview
        purchase.select_purchase_data()

        ' hide update purchase button
        btnpurchaseupdate.Hide()
        btnaddpurchase.Show()
        delete_purchase.Hide()

        ' cancel selected purchase
        purchase.clear_purchase()

        ' gold price set to textview
        tgold_price.Text = today_gold_price

        ' purchase item added to product

    End Sub


    Private Sub btnproduct_Click(sender As Object, e As EventArgs) Handles btnproduct.Click

        auto_fill_sup = 0

        ' hide show panels
        Panels.show("product")

        ' supplier combobox
        product.supplier_combo_dropdown()

        ' refresh gridview product_master
        product.show_purchase_master()

        'hide show button
        btn_update_prod.Hide()
        btn_add_prod.Show()
        btn_delete_product_master.Hide()

        'hide show button product detail
        btn_addprod_detail.Show()
        btn_updateprod_detail.Hide()
        btn_delete_product_detail.Hide()

        ' cancel selection if done
        product.btn_cancel_productmaster_click()

        ' refresh product grid view
        product.prod_detail_grid()

        auto_fill_sup = 1
    End Sub

    Private Sub btnsales_Click(sender As Object, e As EventArgs) Handles btnsales.Click
        ' hide show panels
        Panels.show("sales")

        ' gridview customer
        customer.customer_grid()

        ' hide show button
        btn_add_cust.Show()
        btn_update_cust.Hide()

        sales.customer_dropdown()

        ' select add to bill data
        sales.added_to_bill()

        ' set date
        lbldate.Text = Date.Now.Date

        ' customer disable / unable
        sales.check_customer_dropdown()
    End Sub

    ' purchase operations add Suppliers
    Private Sub addSupplier_Click(sender As Object, e As EventArgs) Handles addSupplier.Click
        purchase.add_supplier()
    End Sub

    ' Purchase class remove supplier
    Private Sub cancelSupplier_Click(sender As Object, e As EventArgs) Handles cancelSupplier.Click
        purchase.cancle_supplier()
    End Sub

    ' double click supplier grid view
    Private Sub supplierGrid_CellContentClick(sender As Object, e As DataGridViewCellEventArgs) Handles supplierGrid.CellContentClick
        purchase.select_supplier()
    End Sub

    ' add purchase insert 
    Private Sub btnaddpurchase_Click(sender As Object, e As EventArgs) Handles btnaddpurchase.Click
        purchase.add_purchase()
    End Sub

    ' clear purchase
    Private Sub btncancelpurchase_Click(sender As Object, e As EventArgs) Handles btncancelpurchase.Click
        purchase.clear_purchase()
    End Sub

    ' purchase delete button click

    Private Sub delete_purchase_Click(sender As Object, e As EventArgs) Handles delete_purchase.Click
        purchase.delete_purchase()
    End Sub

    ' select purchase data for update
    Private Sub purchasegridview_CellContentClick(sender As Object, e As DataGridViewCellEventArgs) Handles purchasegridview.CellContentClick
        purchase.select_purchase_for_update()
    End Sub

    ' update purchased item from supplier
    Private Sub btnpurchaseupdate_Click(sender As Object, e As EventArgs) Handles btnpurchaseupdate.Click
        purchase.update_purchase()
    End Sub

    Private Sub btnupdateSupplier_Click(sender As Object, e As EventArgs) Handles btnupdateSupplier.Click
        purchase.update_supplier()
    End Sub

    ' delete supplier
    Private Sub btn_del_supplier_Click(sender As Object, e As EventArgs) Handles btn_del_supplier.Click
        purchase.del_supplier()
    End Sub


    Private Sub btnlogout_Click(sender As Object, e As EventArgs) Handles btnlogout.Click
        ' logout code
        Dim cmd As New SqlCommand("update login set state=0", DB.connection)

        DB.connection.Open()
        If cmd.ExecuteNonQuery > 0 Then
            Me.Hide()
            Form1.Show()
        Else
            MsgBox("Problem with Logout !!")
        End If
        DB.connection.Close()

    End Sub

    ' count assign control
    Dim initilize As Integer = 0

    ' Report Panel
    Private Sub btnstock_Click(sender As Object, e As EventArgs) Handles btnstock.Click

        
        Panels.show("report")

        ' report
        report = New Reports
        If initilize = 0 Then
            report.assign_controls()
            initilize += 1
        End If


        ' chart report

        
        Dim query As String = "select * from piechart"
        Dim cmd As New SqlCommand(query, DB.connection)
        Dim da As New SqlDataAdapter(cmd)
        Dim ds As New DataSet
        da.Fill(ds)
        Dim cs As New CrystalReport3
        cs.SetDataSource(ds.Tables(0))
        CrystalReportViewer2.ReportSource = cs


        ' fill stock

        'Dim query1 As String = "select pm.pm_name as product, (select count(*) from product_detail where pm_id = pm.pm_id and pd_id not in (select pd_id from sales_detail)) Qty from product_master pm"
        'Dim cmd1 As New SqlCommand(query1, DB.connection)
        'Dim da1 As New SqlDataAdapter(cmd1)
        'Dim ds1 As New DataSet
        'da1.Fill(ds1)
        'tot_stock_grid.DataSource = ds1.Tables(0)


        'Dim row As Integer = 0
        'row = tot_stock_grid.Rows.Count - 1

        'For index = 0 To row - 1
        '    If tot_stock_grid.Rows(index).Cells(1).Value <= 1 Then
        '        tot_stock_grid.Rows(index).Cells(1).Style.BackColor = Color.Red
        '    End If
        'Next
                'tot_stock_grid.Rows(index).Cells(1).Style.BackColor = Color.Red

        'row = 0
        'row = tot_stock_grid.Rows.Count - 1




        'For index = 0 To row - 1
        '    If tot_stock_grid.Rows(index).Cells(1).Value <= 1 Then
        '        tot_stock_grid.Rows(index).Cells(1).Style.BackColor = Color.Red
        '    End If
        'Next

    End Sub

    ' Report purchase checkbox 2 dates
    Private Sub CheckBox1_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBox1.CheckedChanged, CheckBox2.CheckedChanged, CheckBox3.CheckedChanged
        If CheckBox1.Checked Then
            purchase_date_search.Enabled = True
        Else
            purchase_date_search.Enabled = False
        End If

        ' date 2

        If CheckBox2.Checked Then
            purchase_date_search.Enabled = True
            purchase_date_search_2.Enabled = True
            ' checkbox 1 checkbox
            CheckBox1.Checked = True
        Else
            purchase_date_search_2.Enabled = False
        End If

        ' supplier
        If CheckBox3.Checked Then
            supplier_combo.Enabled = True
        Else
            supplier_combo.Enabled = False
        End If

    End Sub

    ' Report product two dates
    Private Sub checkbox_pr_CheckedChanged(sender As Object, e As EventArgs) Handles checkbox_pr.CheckedChanged, checkbox_pr_2.CheckedChanged, checkbox_pr_3.CheckedChanged
        If checkbox_pr.Checked Then
            product_report_date.Enabled = True
        Else
            product_report_date.Enabled = False
        End If

        ' date 2
        If checkbox_pr_2.Checked Then
            product_report_date_2.Enabled = True
            product_report_date.Enabled = True

            checkbox_pr.Checked = True
        Else
            product_report_date_2.Enabled = False
        End If

        ' check 3
        If checkbox_pr_3.Checked Then
            product_report_combo.Enabled = True
        Else
            product_report_combo.Enabled = False
        End If

    End Sub

    ' check box in sales report customer dates

    Private Sub check_sale_report_1_CheckedChanged(sender As Object, e As EventArgs) Handles check_sale_report_1.CheckedChanged, check_sale_customer.CheckedChanged, check_sale_report_2.CheckedChanged

        ' date 2 select
        If check_sale_report_2.Checked Then
            date_sale_report_2.Enabled = True
            date_sale_report_1.Enabled = True
            check_sale_report_1.Checked = True
        Else
            date_sale_report_2.Enabled = False

            ' date 1 only select
            If check_sale_report_1.Checked Then
                date_sale_report_1.Enabled = True
            Else
                date_sale_report_1.Enabled = False
            End If

        End If

        ' date 1
        'If sender.name = "check_sale_report_1" Then
        '    If check_sale_report_1.Checked Then
        '        date_sale_report_1.Enabled = True
        '    Else
        '        date_sale_report_1.Enabled = False
        '    End If
        'End If

        '' date 2
        'If sender.name = "check_sale_report_2" Then
        '    If check_sale_report_2.Checked Then
        '        date_sale_report_2.Enabled = True
        '        date_sale_report_1.Enabled = True
        '        check_sale_report_1.Checked = True
        '    Else
        '        date_sale_report_2.Enabled = False
        '    End If
        'End If

        ' customer
        If sender.name = "check_sale_customer" Then
            If check_sale_customer.Checked Then
                customer_drop_report.Enabled = True
            Else
                customer_drop_report.Enabled = False
            End If
        End If

    End Sub

    Private Sub max_sale_check_1_CheckedChanged(sender As Object, e As EventArgs) Handles max_sale_check_1.CheckedChanged ', max_sale_check_2.CheckedChanged, max_sale_check_3.CheckedChanged

        ' day selected
        If max_sale_check_1.Checked Then

            ' uncheck other 2 check box and read only both
            max_sale_check_2.Checked = False
            max_sale_check_3.Checked = False
            max_sale_month.Enabled = False
            max_sale_year.Enabled = False

            max_sale_date_1.Enabled = True

        Else
            max_sale_date_1.Enabled = False
        End If

    End Sub

    Private Sub max_sale_check_2_CheckedChanged(sender As Object, e As EventArgs) Handles max_sale_check_2.CheckedChanged
        ' month select
        If max_sale_check_2.Checked Then
            max_sale_month.Enabled = True
            max_sale_date_1.Enabled = False
            max_sale_check_1.Checked = False
        Else
            max_sale_month.Enabled = False
        End If

    End Sub

    Private Sub max_sale_check_3_CheckedChanged(sender As Object, e As EventArgs) Handles max_sale_check_3.CheckedChanged
        ' year select
        If max_sale_check_3.Checked Then
            max_sale_year.Enabled = True
            max_sale_date_1.Enabled = False
            max_sale_check_1.Checked = False
        Else
            max_sale_year.Enabled = False
        End If
    End Sub


    Private Sub group_by_product()
        Try
            Dim query As String = "select pm.pm_name, count(pd.pm_id) Qty, sum(pd.weight) total_weight from product_detail pd, product_master pm where pd.pm_id = pm.pm_id and pd.pd_id not in (select pd_id from sales_detail) group by pd.pm_id, pm.pm_name"

            If homecheck.Checked Then
                query = "select pm.pm_name, count(pd.pm_id) Qty, sum(pd.weight) total_weight from product_detail pd, product_master pm where pd.pm_id = pm.pm_id and pd.pd_id not in (select sd.pd_id from sales_detail sd, sales_master sm where sm.sales_master_id = sd.sales_master_id and sm.sale_date < '" + homedate.Text.ToString + "' ) group by pd.pm_id, pm.pm_name"
            End If

            Dim cmd As SqlCommand = New SqlCommand(query, DB.connection)
            Dim da As SqlDataAdapter = New SqlDataAdapter(cmd)
            Dim ds As DataSet = New DataSet
            da.Fill(ds, "product_data")
            homestock.DataSource = ds.Tables("product_data")
        Catch ex As Exception
            MsgBox("Something going wrong !!")
        End Try
    End Sub

    ' check box home panel date
    Private Sub homecheck_CheckedChanged(sender As Object, e As EventArgs) Handles homecheck.CheckedChanged

        If homecheck.Checked Then
            homedate.Enabled = True
        Else
            homedate.Enabled = False
        End If

    End Sub

    ' home search button click
    Private Sub homesearch_Click(sender As Object, e As EventArgs) Handles homesearch.Click
        group_by_product()
    End Sub

    ' create bill when customer buy
    Sub fill_bill(id As Integer)
        Dim query As String = "select * from bill where sales_master_id=" + id.ToString + " "

        Dim cmd As New SqlCommand(query, DB.connection)
        Dim da As New SqlDataAdapter(cmd)
        Dim ds As New DataSet
        da.Fill(ds)

        Dim rpt As New CrystalReport2

        rpt.SetDataSource(ds.Tables(0))
        CrystalReportViewer1.ReportSource = rpt
    End Sub

    ' count assign control
    Dim initilize1 As Integer = 0

    Private Sub btnbills_Click(sender As Object, e As EventArgs) Handles btnbills.Click
        Panels.show("find_bill_panel")

        bill = New Bill
        If initilize1 = 0 Then
            bill.assign_control()
            initilize1 += 1
        End If

    End Sub

    ' check box reminder purchase and added stock
    Private Sub remind_supplier_CheckedChanged(sender As Object, e As EventArgs) Handles remind_supplier.CheckedChanged, remind_date.CheckedChanged

        ' reminder supplier checkbox
        If remind_supplier.Checked Then
            remind_supplier_combo.Enabled = True
        Else
            remind_supplier_combo.Enabled = False
        End If

        ' reminder date checkbox
        If remind_date.Checked Then
            remind_purchase_date.Enabled = True
        Else
            remind_purchase_date.Enabled = False
        End If

    End Sub

    ' area wise sales
    Private Sub CheckBox4_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBox4.CheckedChanged, CheckBox5.CheckedChanged

        ' checkbox 1
        If CheckBox4.Checked Then
            area_date_1.Enabled = True
        Else
            area_date_1.Enabled = False
        End If


        ' checkbox 2
        If CheckBox5.Checked Then
            area_date_2.Enabled = True
            CheckBox4.Checked = True
            area_date_1.Enabled = True
        Else
            area_date_2.Enabled = False
        End If

    End Sub

    ' find bill no
    Private Sub CheckBox6_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBox6.CheckedChanged, CheckBox7.CheckedChanged

        If CheckBox6.Checked Then
            customer_list_find.Enabled = True
        Else
            customer_list_find.Enabled = False
        End If

        If CheckBox7.Checked Then
            cust_sale_find.Enabled = True
        Else
            cust_sale_find.Enabled = False
        End If

    End Sub


    ' -----------------------------------------------------------------------------
    ' print document to print gridview


    Private Sub print1_Click(sender As Object, e As EventArgs) Handles print1.Click

        msg = "Purchase Report "
        grid = "purchase"

        Dim print_preview As New PrintPreviewDialog
        print_preview.Document = PrintDocument1
        print_preview.PrintPreviewControl.Zoom = 1
        print_preview.ShowDialog()
        flag = 0



    End Sub

    ' product report
    Private Sub print2_Click(sender As Object, e As EventArgs) Handles print2.Click
        msg = "Product Report"
        grid = "product"

        Dim print_preview As New PrintPreviewDialog
        print_preview.Document = PrintDocument1
        print_preview.PrintPreviewControl.Zoom = 1
        print_preview.ShowDialog()
        flag = 0

    End Sub

    ' sales cust wise data 
    ' print 3
    Private Sub print3_Click(sender As Object, e As EventArgs) Handles print3.Click
        msg = "Sales Wise Report"
        grid = "sales1"

        Dim print_preview As New PrintPreviewDialog
        print_preview.Document = PrintDocument1
        print_preview.PrintPreviewControl.Zoom = 1
        print_preview.ShowDialog()
        flag = 0

    End Sub

    ' print 4 sales 2

    ' date wise sales report
    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles print4.Click
        msg = "Date Wise Sales Report"
        grid = "sales2"

        Dim print_preview As New PrintPreviewDialog
        print_preview.Document = PrintDocument1
        print_preview.PrintPreviewControl.Zoom = 1
        print_preview.ShowDialog()
        flag = 0
    End Sub

    ' print 5 click
    Private Sub print5_Click(sender As Object, e As EventArgs) Handles print5.Click
        msg = "Max Sold Item Report"
        grid = "sales3"

        Dim print_preview As New PrintPreviewDialog
        print_preview.Document = PrintDocument1
        print_preview.PrintPreviewControl.Zoom = 1
        print_preview.ShowDialog()
        flag = 0
    End Sub

    ' print 6 click
    Private Sub print6_Click(sender As Object, e As EventArgs) Handles print6.Click
        msg = "Area Wise Sales Report"
        grid = "sales4"

        Dim print_preview As New PrintPreviewDialog
        print_preview.Document = PrintDocument1
        print_preview.PrintPreviewControl.Zoom = 1
        print_preview.ShowDialog()
        flag = 0
    End Sub

    ' print docu

    Private Sub PrintDocument1_BeginPrint(sender As Object, e As Printing.PrintEventArgs) Handles PrintDocument1.BeginPrint

        ' for purchase grid

        If grid = "purchase" Then
            Try
                strformate = New StringFormat
                strformate.Alignment = StringAlignment.Near
                strformate.LineAlignment = StringAlignment.Center
                strformate.Trimming = StringTrimming.EllipsisCharacter

                arrColumnLefts.Clear()
                arrColumnWidths.Clear()
                iCellHeight = 0
                iRow = 0
                bNewpag = True
                bFirstpage = True
                iTotalWidth = 0

                ' edit
                For Each dgGridCol As DataGridViewColumn In purchase_report_grid.Columns
                    iTotalWidth += dgGridCol.Width
                Next

            Catch ex As Exception
                MessageBox.Show(ex.Message, "Something Went Wrong", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
        End If


        ' for product report
        
        If grid = "product" Then
            Try
                strformate = New StringFormat
                strformate.Alignment = StringAlignment.Near
                strformate.LineAlignment = StringAlignment.Center
                strformate.Trimming = StringTrimming.EllipsisCharacter

                arrColumnLefts.Clear()
                arrColumnWidths.Clear()
                iCellHeight = 0
                iRow = 0
                bNewpag = True
                bFirstpage = True
                iTotalWidth = 0

                ' edit
                For Each dgGridCol As DataGridViewColumn In product_report_gridview.Columns
                    iTotalWidth += dgGridCol.Width
                Next

                MsgBox("inner 1st")

            Catch ex As Exception
                MessageBox.Show(ex.Message, "Something Went Wrong", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
        End If

        ' for sales wise customer report 1

        If grid = "sales1" Then
            Try
                strformate = New StringFormat
                strformate.Alignment = StringAlignment.Near
                strformate.LineAlignment = StringAlignment.Center
                strformate.Trimming = StringTrimming.EllipsisCharacter

                arrColumnLefts.Clear()
                arrColumnWidths.Clear()
                iCellHeight = 0
                iRow = 0
                bNewpag = True
                bFirstpage = True
                iTotalWidth = 0

                ' edit
                For Each dgGridCol As DataGridViewColumn In sales_grid_report_1.Columns
                    iTotalWidth += dgGridCol.Width
                Next

            Catch ex As Exception
                MessageBox.Show(ex.Message, "Something Went Wrong", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
        End If

        ' date wise sales report
        ' for sales wise customer report 1

        If grid = "sales2" Then
            Try
                strformate = New StringFormat
                strformate.Alignment = StringAlignment.Near
                strformate.LineAlignment = StringAlignment.Center
                strformate.Trimming = StringTrimming.EllipsisCharacter

                arrColumnLefts.Clear()
                arrColumnWidths.Clear()
                iCellHeight = 0
                iRow = 0
                bNewpag = True
                bFirstpage = True
                iTotalWidth = 0

                ' edit
                For Each dgGridCol As DataGridViewColumn In sale_day_month_grid.Columns
                    iTotalWidth += dgGridCol.Width
                Next

            Catch ex As Exception
                MessageBox.Show(ex.Message, "Something Went Wrong", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
        End If


        ' sales 3
        If grid = "sales3" Then
            Try
                strformate = New StringFormat
                strformate.Alignment = StringAlignment.Near
                strformate.LineAlignment = StringAlignment.Center
                strformate.Trimming = StringTrimming.EllipsisCharacter

                arrColumnLefts.Clear()
                arrColumnWidths.Clear()
                iCellHeight = 0
                iRow = 0
                bNewpag = True
                bFirstpage = True
                iTotalWidth = 0

                ' edit
                For Each dgGridCol As DataGridViewColumn In max_sold_item_grid.Columns
                    iTotalWidth += dgGridCol.Width
                Next

            Catch ex As Exception
                MessageBox.Show(ex.Message, "Something Went Wrong", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
        End If

        ' sales 4

        If grid = "sales4" Then
            Try
                strformate = New StringFormat
                strformate.Alignment = StringAlignment.Near
                strformate.LineAlignment = StringAlignment.Center
                strformate.Trimming = StringTrimming.EllipsisCharacter

                arrColumnLefts.Clear()
                arrColumnWidths.Clear()
                iCellHeight = 0
                iRow = 0
                bNewpag = True
                bFirstpage = True
                iTotalWidth = 0

                ' edit
                For Each dgGridCol As DataGridViewColumn In area_wise_sale_grid.Columns
                    iTotalWidth += dgGridCol.Width
                Next

            Catch ex As Exception
                MessageBox.Show(ex.Message, "Something Went Wrong", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
        End If
    End Sub

    ' -----

    Private Sub PrintDocument1_PrintPage(sender As Object, e As Printing.PrintPageEventArgs) Handles PrintDocument1.PrintPage

        ' purchase data gridview

        If grid = "purchase" Then

            Try

                Dim iLeftMargin As Integer = e.MarginBounds.Left - 40
                Dim iTopMargin As Integer = e.MarginBounds.Top
                Dim bMorePageToPrint As Boolean = False
                Dim iTmpWidth As Integer = 0

                If bFirstpage Then



                    For Each gridecol As DataGridViewColumn In purchase_report_grid.Columns
                        iTmpWidth = CInt(Math.Truncate(Math.Floor(CDbl((CDbl(gridecol.Width) / CDbl(iTotalWidth) * CDbl(iTotalWidth) * CDbl(e.MarginBounds.Width) / CDbl(iTotalWidth))))) + 14)
                        iHeaderHeight = CInt(e.Graphics.MeasureString(gridecol.HeaderText, gridecol.InheritedStyle.Font, iTmpWidth).Height) + 16
                        arrColumnLefts.Add(iLeftMargin)
                        arrColumnWidths.Add(iTmpWidth)
                        iLeftMargin += iTmpWidth

                    Next gridecol
                    'bm = New Bitmap(Label40.Width, Label40.Height)
                    'Dim g As Graphics = Graphics.FromImage(bm)
                    'Label40.DrawToBitmap(bm, Label40.ClientRectangle)
                    'g.Dispose()
                    'PrintPreviewDialog1.Document = PrintDocument1

                    'e.Graphics.DrawImage(bm, 0, 0)

                End If

                Do While iRow <= purchase_report_grid.Rows.Count - 1

                    Dim Gridrow As DataGridViewRow = purchase_report_grid.Rows(iRow)

                    iCellHeight = Gridrow.Height + 8
                    Dim iCount As Integer = 0

                    If iTopMargin + iCellHeight >= e.MarginBounds.Height + e.MarginBounds.Top Then

                        bNewpag = True
                        bFirstpage = False
                        bMorePageToPrint = True

                        Exit Do

                    Else
                        If bNewpag Then
                            Dim d As Double = 0

                            If flag = 0 Then
                                d = -40

                            Else
                                d = 23
                            End If
                            'MsgBox(msg)
                            e.Graphics.DrawString(msg, New Font(purchase_report_grid.Font, FontStyle.Bold), Brushes.Black, e.MarginBounds.Left - 40, e.MarginBounds.Top - e.Graphics.MeasureString("Income Data Details.", New Font(purchase_report_grid.Font, FontStyle.Bold), e.MarginBounds.Width).Height - d)

                            Dim strdate As String = "Report Date : " & Date.Now.ToLongDateString & " " & Date.Now.ToShortTimeString

                            e.Graphics.DrawString(strdate, New Font(purchase_report_grid.Font, FontStyle.Bold), Brushes.Black, e.MarginBounds.Left + 40 + (e.MarginBounds.Width - e.Graphics.MeasureString(strdate, New Font(purchase_report_grid.Font, FontStyle.Bold), e.MarginBounds.Width).Width), e.MarginBounds.Top - e.Graphics.MeasureString("Income Data Details.", New Font(New Font(purchase_report_grid.Font, FontStyle.Bold), FontStyle.Bold), e.MarginBounds.Width).Height - d)

                            If flag = 0 Then
                                iTopMargin = e.MarginBounds.Top + 50
                                flag = 1
                            Else
                                iTopMargin = e.MarginBounds.Top
                            End If


                            For Each Gridcol As DataGridViewColumn In purchase_report_grid.Columns

                                e.Graphics.FillRectangle(New SolidBrush(Color.LightGray), New Rectangle(DirectCast(arrColumnLefts(iCount), Integer), iTopMargin, DirectCast(arrColumnWidths(iCount), Integer), iHeaderHeight))

                                e.Graphics.DrawRectangle(Pens.Black, New Rectangle(DirectCast(arrColumnLefts(iCount), Integer), iTopMargin, DirectCast(arrColumnWidths(iCount), Integer), iHeaderHeight))

                                e.Graphics.DrawString(Gridcol.HeaderText, Gridcol.InheritedStyle.Font, New SolidBrush(Gridcol.InheritedStyle.ForeColor), New Rectangle(DirectCast(arrColumnLefts(iCount), Integer), iTopMargin, DirectCast(arrColumnWidths(iCount), Integer), iHeaderHeight), strformate)

                                iCount += 1

                            Next Gridcol

                            bNewpag = False


                            iTopMargin += iHeaderHeight

                        End If

                        iCount = 0
                        For Each cel As DataGridViewCell In Gridrow.Cells

                            If cel.Value IsNot Nothing Then
                                e.Graphics.DrawString(cel.Value.ToString, cel.InheritedStyle.Font, New SolidBrush(cel.InheritedStyle.ForeColor), New Rectangle(DirectCast(arrColumnLefts(iCount), Integer), CSng(iTopMargin), DirectCast(arrColumnWidths(iCount), Integer), iHeaderHeight), strformate)
                            End If


                            e.Graphics.DrawRectangle(Pens.Black, New Rectangle(DirectCast(arrColumnLefts(iCount), Integer), iTopMargin, DirectCast(arrColumnWidths(iCount), Integer), iCellHeight))

                            iCount += 1

                        Next cel

                    End If

                    iRow += 1
                    iTopMargin += iCellHeight

                Loop

                If bMorePageToPrint Then
                    e.HasMorePages = True
                Else
                    e.HasMorePages = False
                End If

            Catch ex As Exception
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
        End If

        ' for product report

        If grid = "product" Then

            'MsgBox("inner2")

            Try

                Dim iLeftMargin As Integer = e.MarginBounds.Left - 40
                Dim iTopMargin As Integer = e.MarginBounds.Top
                Dim bMorePageToPrint As Boolean = False
                Dim iTmpWidth As Integer = 0

                If bFirstpage Then



                    For Each gridecol As DataGridViewColumn In product_report_gridview.Columns

                        iTmpWidth = CInt(Math.Truncate(Math.Floor(CDbl((CDbl(gridecol.Width) / CDbl(iTotalWidth) * CDbl(iTotalWidth) * CDbl(e.MarginBounds.Width) / CDbl(iTotalWidth))))) + 14)

                        iHeaderHeight = CInt(e.Graphics.MeasureString(gridecol.HeaderText, gridecol.InheritedStyle.Font, iTmpWidth).Height) + 16

                        arrColumnLefts.Add(iLeftMargin)
                        arrColumnWidths.Add(iTmpWidth)

                        iLeftMargin += iTmpWidth

                    Next gridecol
                    'bm = New Bitmap(Label40.Width, Label40.Height)
                    'Dim g As Graphics = Graphics.FromImage(bm)
                    'Label40.DrawToBitmap(bm, Label40.ClientRectangle)
                    'g.Dispose()
                    'PrintPreviewDialog1.Document = PrintDocument1

                    'e.Graphics.DrawImage(bm, 0, 0)

                End If

                Do While iRow <= product_report_gridview.Rows.Count - 1

                    Dim Gridrow As DataGridViewRow = product_report_gridview.Rows(iRow)

                    iCellHeight = Gridrow.Height + 8
                    Dim iCount As Integer = 0

                    If iTopMargin + iCellHeight >= e.MarginBounds.Height + e.MarginBounds.Top Then

                        bNewpag = True
                        bFirstpage = False
                        bMorePageToPrint = True

                        Exit Do

                    Else
                        If bNewpag Then
                            Dim d As Double = 0

                            If flag = 0 Then
                                d = -40

                            Else
                                d = 23
                            End If
                            'MsgBox(msg)
                            e.Graphics.DrawString(msg, New Font(product_report_gridview.Font, FontStyle.Bold), Brushes.Black, e.MarginBounds.Left - 40, e.MarginBounds.Top - e.Graphics.MeasureString("Income Data Details.", New Font(product_report_gridview.Font, FontStyle.Bold), e.MarginBounds.Width).Height - d)

                            Dim strdate As String = "Report Date : " & Date.Now.ToLongDateString & " " & Date.Now.ToShortTimeString

                            e.Graphics.DrawString(strdate, New Font(product_report_gridview.Font, FontStyle.Bold), Brushes.Black, e.MarginBounds.Left + 40 + (e.MarginBounds.Width - e.Graphics.MeasureString(strdate, New Font(product_report_gridview.Font, FontStyle.Bold), e.MarginBounds.Width).Width), e.MarginBounds.Top - e.Graphics.MeasureString("Income Data Details.", New Font(New Font(product_report_gridview.Font, FontStyle.Bold), FontStyle.Bold), e.MarginBounds.Width).Height - d)

                            If flag = 0 Then
                                iTopMargin = e.MarginBounds.Top + 50
                                flag = 1
                            Else
                                iTopMargin = e.MarginBounds.Top
                            End If


                            For Each Gridcol As DataGridViewColumn In product_report_gridview.Columns

                                e.Graphics.FillRectangle(New SolidBrush(Color.LightGray), New Rectangle(DirectCast(arrColumnLefts(iCount), Integer), iTopMargin, DirectCast(arrColumnWidths(iCount), Integer), iHeaderHeight))

                                e.Graphics.DrawRectangle(Pens.Black, New Rectangle(DirectCast(arrColumnLefts(iCount), Integer), iTopMargin, DirectCast(arrColumnWidths(iCount), Integer), iHeaderHeight))

                                e.Graphics.DrawString(Gridcol.HeaderText, Gridcol.InheritedStyle.Font, New SolidBrush(Gridcol.InheritedStyle.ForeColor), New Rectangle(DirectCast(arrColumnLefts(iCount), Integer), iTopMargin, DirectCast(arrColumnWidths(iCount), Integer), iHeaderHeight), strformate)

                                iCount += 1

                            Next Gridcol

                            bNewpag = False


                            iTopMargin += iHeaderHeight

                        End If

                        iCount = 0
                        For Each cel As DataGridViewCell In Gridrow.Cells

                            If cel.Value IsNot Nothing Then
                                e.Graphics.DrawString(cel.Value.ToString, cel.InheritedStyle.Font, New SolidBrush(cel.InheritedStyle.ForeColor), New Rectangle(DirectCast(arrColumnLefts(iCount), Integer), CSng(iTopMargin), DirectCast(arrColumnWidths(iCount), Integer), iHeaderHeight), strformate)
                            End If


                            e.Graphics.DrawRectangle(Pens.Black, New Rectangle(DirectCast(arrColumnLefts(iCount), Integer), iTopMargin, DirectCast(arrColumnWidths(iCount), Integer), iCellHeight))

                            iCount += 1

                        Next cel

                    End If

                    iRow += 1
                    iTopMargin += iCellHeight

                Loop

                If bMorePageToPrint Then
                    e.HasMorePages = True
                Else
                    e.HasMorePages = False
                End If

            Catch ex As Exception
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try

        End If


        ' sales wise customer report sales 1


        ' for product report

        If grid = "sales1" Then

            'MsgBox("inner2")

            Try

                Dim iLeftMargin As Integer = e.MarginBounds.Left - 40
                Dim iTopMargin As Integer = e.MarginBounds.Top
                Dim bMorePageToPrint As Boolean = False
                Dim iTmpWidth As Integer = 0

                If bFirstpage Then



                    For Each gridecol As DataGridViewColumn In sales_grid_report_1.Columns

                        iTmpWidth = CInt(Math.Truncate(Math.Floor(CDbl((CDbl(gridecol.Width) / CDbl(iTotalWidth) * CDbl(iTotalWidth) * CDbl(e.MarginBounds.Width) / CDbl(iTotalWidth))))) + 14)

                        iHeaderHeight = CInt(e.Graphics.MeasureString(gridecol.HeaderText, gridecol.InheritedStyle.Font, iTmpWidth).Height) + 16

                        arrColumnLefts.Add(iLeftMargin)
                        arrColumnWidths.Add(iTmpWidth)

                        iLeftMargin += iTmpWidth

                    Next gridecol
                    'bm = New Bitmap(Label40.Width, Label40.Height)
                    'Dim g As Graphics = Graphics.FromImage(bm)
                    'Label40.DrawToBitmap(bm, Label40.ClientRectangle)
                    'g.Dispose()
                    'PrintPreviewDialog1.Document = PrintDocument1

                    'e.Graphics.DrawImage(bm, 0, 0)

                End If

                Do While iRow <= sales_grid_report_1.Rows.Count - 1

                    Dim Gridrow As DataGridViewRow = sales_grid_report_1.Rows(iRow)

                    iCellHeight = Gridrow.Height + 8
                    Dim iCount As Integer = 0

                    If iTopMargin + iCellHeight >= e.MarginBounds.Height + e.MarginBounds.Top Then

                        bNewpag = True
                        bFirstpage = False
                        bMorePageToPrint = True

                        Exit Do

                    Else
                        If bNewpag Then
                            Dim d As Double = 0

                            If flag = 0 Then
                                d = -40

                            Else
                                d = 23
                            End If
                            'MsgBox(msg)
                            e.Graphics.DrawString(msg, New Font(sales_grid_report_1.Font, FontStyle.Bold), Brushes.Black, e.MarginBounds.Left - 40, e.MarginBounds.Top - e.Graphics.MeasureString("Income Data Details.", New Font(sales_grid_report_1.Font, FontStyle.Bold), e.MarginBounds.Width).Height - d)

                            Dim strdate As String = "Report Date : " & Date.Now.ToLongDateString & " " & Date.Now.ToShortTimeString

                            e.Graphics.DrawString(strdate, New Font(sales_grid_report_1.Font, FontStyle.Bold), Brushes.Black, e.MarginBounds.Left + 40 + (e.MarginBounds.Width - e.Graphics.MeasureString(strdate, New Font(sales_grid_report_1.Font, FontStyle.Bold), e.MarginBounds.Width).Width), e.MarginBounds.Top - e.Graphics.MeasureString("Income Data Details.", New Font(New Font(sales_grid_report_1.Font, FontStyle.Bold), FontStyle.Bold), e.MarginBounds.Width).Height - d)

                            If flag = 0 Then
                                iTopMargin = e.MarginBounds.Top + 50
                                flag = 1
                            Else
                                iTopMargin = e.MarginBounds.Top
                            End If


                            For Each Gridcol As DataGridViewColumn In sales_grid_report_1.Columns

                                e.Graphics.FillRectangle(New SolidBrush(Color.LightGray), New Rectangle(DirectCast(arrColumnLefts(iCount), Integer), iTopMargin, DirectCast(arrColumnWidths(iCount), Integer), iHeaderHeight))

                                e.Graphics.DrawRectangle(Pens.Black, New Rectangle(DirectCast(arrColumnLefts(iCount), Integer), iTopMargin, DirectCast(arrColumnWidths(iCount), Integer), iHeaderHeight))

                                e.Graphics.DrawString(Gridcol.HeaderText, Gridcol.InheritedStyle.Font, New SolidBrush(Gridcol.InheritedStyle.ForeColor), New Rectangle(DirectCast(arrColumnLefts(iCount), Integer), iTopMargin, DirectCast(arrColumnWidths(iCount), Integer), iHeaderHeight), strformate)

                                iCount += 1

                            Next Gridcol

                            bNewpag = False


                            iTopMargin += iHeaderHeight

                        End If

                        iCount = 0
                        For Each cel As DataGridViewCell In Gridrow.Cells

                            If cel.Value IsNot Nothing Then
                                e.Graphics.DrawString(cel.Value.ToString, cel.InheritedStyle.Font, New SolidBrush(cel.InheritedStyle.ForeColor), New Rectangle(DirectCast(arrColumnLefts(iCount), Integer), CSng(iTopMargin), DirectCast(arrColumnWidths(iCount), Integer), iHeaderHeight), strformate)
                            End If


                            e.Graphics.DrawRectangle(Pens.Black, New Rectangle(DirectCast(arrColumnLefts(iCount), Integer), iTopMargin, DirectCast(arrColumnWidths(iCount), Integer), iCellHeight))

                            iCount += 1

                        Next cel

                    End If

                    iRow += 1
                    iTopMargin += iCellHeight

                Loop

                If bMorePageToPrint Then
                    e.HasMorePages = True
                Else
                    e.HasMorePages = False
                End If

            Catch ex As Exception
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try

        End If

        ' sales report Day Month Year Wise Sales Report

        ' for product report

        If grid = "sales2" Then

            'MsgBox("inner2")

            Try

                Dim iLeftMargin As Integer = e.MarginBounds.Left - 40
                Dim iTopMargin As Integer = e.MarginBounds.Top
                Dim bMorePageToPrint As Boolean = False
                Dim iTmpWidth As Integer = 0

                If bFirstpage Then



                    For Each gridecol As DataGridViewColumn In sale_day_month_grid.Columns

                        iTmpWidth = CInt(Math.Truncate(Math.Floor(CDbl((CDbl(gridecol.Width) / CDbl(iTotalWidth) * CDbl(iTotalWidth) * CDbl(e.MarginBounds.Width) / CDbl(iTotalWidth))))) + 14)

                        iHeaderHeight = CInt(e.Graphics.MeasureString(gridecol.HeaderText, gridecol.InheritedStyle.Font, iTmpWidth).Height) + 16

                        arrColumnLefts.Add(iLeftMargin)
                        arrColumnWidths.Add(iTmpWidth)

                        iLeftMargin += iTmpWidth

                    Next gridecol
                End If

                Do While iRow <= sale_day_month_grid.Rows.Count - 1

                    Dim Gridrow As DataGridViewRow = sale_day_month_grid.Rows(iRow)

                    iCellHeight = Gridrow.Height + 8
                    Dim iCount As Integer = 0

                    If iTopMargin + iCellHeight >= e.MarginBounds.Height + e.MarginBounds.Top Then

                        bNewpag = True
                        bFirstpage = False
                        bMorePageToPrint = True

                        Exit Do

                    Else
                        If bNewpag Then
                            Dim d As Double = 0

                            If flag = 0 Then
                                d = -40

                            Else
                                d = 23
                            End If
                            'MsgBox(msg)
                            e.Graphics.DrawString(msg, New Font(sale_day_month_grid.Font, FontStyle.Bold), Brushes.Black, e.MarginBounds.Left - 40, e.MarginBounds.Top - e.Graphics.MeasureString("Income Data Details.", New Font(sale_day_month_grid.Font, FontStyle.Bold), e.MarginBounds.Width).Height - d)

                            Dim strdate As String = "Report Date : " & Date.Now.ToLongDateString & " " & Date.Now.ToShortTimeString

                            e.Graphics.DrawString(strdate, New Font(sale_day_month_grid.Font, FontStyle.Bold), Brushes.Black, e.MarginBounds.Left + 40 + (e.MarginBounds.Width - e.Graphics.MeasureString(strdate, New Font(sale_day_month_grid.Font, FontStyle.Bold), e.MarginBounds.Width).Width), e.MarginBounds.Top - e.Graphics.MeasureString("Income Data Details.", New Font(New Font(sale_day_month_grid.Font, FontStyle.Bold), FontStyle.Bold), e.MarginBounds.Width).Height - d)

                            If flag = 0 Then
                                iTopMargin = e.MarginBounds.Top + 50
                                flag = 1
                            Else
                                iTopMargin = e.MarginBounds.Top
                            End If


                            For Each Gridcol As DataGridViewColumn In sale_day_month_grid.Columns

                                e.Graphics.FillRectangle(New SolidBrush(Color.LightGray), New Rectangle(DirectCast(arrColumnLefts(iCount), Integer), iTopMargin, DirectCast(arrColumnWidths(iCount), Integer), iHeaderHeight))

                                e.Graphics.DrawRectangle(Pens.Black, New Rectangle(DirectCast(arrColumnLefts(iCount), Integer), iTopMargin, DirectCast(arrColumnWidths(iCount), Integer), iHeaderHeight))

                                e.Graphics.DrawString(Gridcol.HeaderText, Gridcol.InheritedStyle.Font, New SolidBrush(Gridcol.InheritedStyle.ForeColor), New Rectangle(DirectCast(arrColumnLefts(iCount), Integer), iTopMargin, DirectCast(arrColumnWidths(iCount), Integer), iHeaderHeight), strformate)

                                iCount += 1

                            Next Gridcol

                            bNewpag = False


                            iTopMargin += iHeaderHeight

                        End If

                        iCount = 0
                        For Each cel As DataGridViewCell In Gridrow.Cells

                            If cel.Value IsNot Nothing Then
                                e.Graphics.DrawString(cel.Value.ToString, cel.InheritedStyle.Font, New SolidBrush(cel.InheritedStyle.ForeColor), New Rectangle(DirectCast(arrColumnLefts(iCount), Integer), CSng(iTopMargin), DirectCast(arrColumnWidths(iCount), Integer), iHeaderHeight), strformate)
                            End If


                            e.Graphics.DrawRectangle(Pens.Black, New Rectangle(DirectCast(arrColumnLefts(iCount), Integer), iTopMargin, DirectCast(arrColumnWidths(iCount), Integer), iCellHeight))

                            iCount += 1

                        Next cel

                    End If

                    iRow += 1
                    iTopMargin += iCellHeight

                Loop

                If bMorePageToPrint Then
                    e.HasMorePages = True
                Else
                    e.HasMorePages = False
                End If

            Catch ex As Exception
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try

        End If


        ' sales 3 reports print


        If grid = "sales3" Then

            'MsgBox("inner2")

            Try

                Dim iLeftMargin As Integer = e.MarginBounds.Left - 40
                Dim iTopMargin As Integer = e.MarginBounds.Top
                Dim bMorePageToPrint As Boolean = False
                Dim iTmpWidth As Integer = 0

                If bFirstpage Then



                    For Each gridecol As DataGridViewColumn In max_sold_item_grid.Columns

                        iTmpWidth = CInt(Math.Truncate(Math.Floor(CDbl((CDbl(gridecol.Width) / CDbl(iTotalWidth) * CDbl(iTotalWidth) * CDbl(e.MarginBounds.Width) / CDbl(iTotalWidth))))) + 14)

                        iHeaderHeight = CInt(e.Graphics.MeasureString(gridecol.HeaderText, gridecol.InheritedStyle.Font, iTmpWidth).Height) + 16

                        arrColumnLefts.Add(iLeftMargin)
                        arrColumnWidths.Add(iTmpWidth)

                        iLeftMargin += iTmpWidth

                    Next gridecol
                End If

                Do While iRow <= max_sold_item_grid.Rows.Count - 1

                    Dim Gridrow As DataGridViewRow = max_sold_item_grid.Rows(iRow)

                    iCellHeight = Gridrow.Height + 8
                    Dim iCount As Integer = 0

                    If iTopMargin + iCellHeight >= e.MarginBounds.Height + e.MarginBounds.Top Then

                        bNewpag = True
                        bFirstpage = False
                        bMorePageToPrint = True

                        Exit Do

                    Else
                        If bNewpag Then
                            Dim d As Double = 0

                            If flag = 0 Then
                                d = -40

                            Else
                                d = 23
                            End If
                            'MsgBox(msg)
                            e.Graphics.DrawString(msg, New Font(max_sold_item_grid.Font, FontStyle.Bold), Brushes.Black, e.MarginBounds.Left - 40, e.MarginBounds.Top - e.Graphics.MeasureString("Income Data Details.", New Font(max_sold_item_grid.Font, FontStyle.Bold), e.MarginBounds.Width).Height - d)

                            Dim strdate As String = "Report Date : " & Date.Now.ToLongDateString & " " & Date.Now.ToShortTimeString

                            e.Graphics.DrawString(strdate, New Font(max_sold_item_grid.Font, FontStyle.Bold), Brushes.Black, e.MarginBounds.Left + 40 + (e.MarginBounds.Width - e.Graphics.MeasureString(strdate, New Font(max_sold_item_grid.Font, FontStyle.Bold), e.MarginBounds.Width).Width), e.MarginBounds.Top - e.Graphics.MeasureString("Income Data Details.", New Font(New Font(max_sold_item_grid.Font, FontStyle.Bold), FontStyle.Bold), e.MarginBounds.Width).Height - d)

                            If flag = 0 Then
                                iTopMargin = e.MarginBounds.Top + 50
                                flag = 1
                            Else
                                iTopMargin = e.MarginBounds.Top
                            End If


                            For Each Gridcol As DataGridViewColumn In max_sold_item_grid.Columns

                                e.Graphics.FillRectangle(New SolidBrush(Color.LightGray), New Rectangle(DirectCast(arrColumnLefts(iCount), Integer), iTopMargin, DirectCast(arrColumnWidths(iCount), Integer), iHeaderHeight))

                                e.Graphics.DrawRectangle(Pens.Black, New Rectangle(DirectCast(arrColumnLefts(iCount), Integer), iTopMargin, DirectCast(arrColumnWidths(iCount), Integer), iHeaderHeight))

                                e.Graphics.DrawString(Gridcol.HeaderText, Gridcol.InheritedStyle.Font, New SolidBrush(Gridcol.InheritedStyle.ForeColor), New Rectangle(DirectCast(arrColumnLefts(iCount), Integer), iTopMargin, DirectCast(arrColumnWidths(iCount), Integer), iHeaderHeight), strformate)

                                iCount += 1

                            Next Gridcol

                            bNewpag = False


                            iTopMargin += iHeaderHeight

                        End If

                        iCount = 0
                        For Each cel As DataGridViewCell In Gridrow.Cells

                            If cel.Value IsNot Nothing Then
                                e.Graphics.DrawString(cel.Value.ToString, cel.InheritedStyle.Font, New SolidBrush(cel.InheritedStyle.ForeColor), New Rectangle(DirectCast(arrColumnLefts(iCount), Integer), CSng(iTopMargin), DirectCast(arrColumnWidths(iCount), Integer), iHeaderHeight), strformate)
                            End If


                            e.Graphics.DrawRectangle(Pens.Black, New Rectangle(DirectCast(arrColumnLefts(iCount), Integer), iTopMargin, DirectCast(arrColumnWidths(iCount), Integer), iCellHeight))

                            iCount += 1

                        Next cel

                    End If

                    iRow += 1
                    iTopMargin += iCellHeight

                Loop

                If bMorePageToPrint Then
                    e.HasMorePages = True
                Else
                    e.HasMorePages = False
                End If

            Catch ex As Exception
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try

        End If

        ' sales 4 report print

        If grid = "sales4" Then

            'MsgBox("inner2")

            Try

                Dim iLeftMargin As Integer = e.MarginBounds.Left - 40
                Dim iTopMargin As Integer = e.MarginBounds.Top
                Dim bMorePageToPrint As Boolean = False
                Dim iTmpWidth As Integer = 0

                If bFirstpage Then

                    For Each gridecol As DataGridViewColumn In area_wise_sale_grid.Columns

                        iTmpWidth = CInt(Math.Truncate(Math.Floor(CDbl((CDbl(gridecol.Width) / CDbl(iTotalWidth) * CDbl(iTotalWidth) * CDbl(e.MarginBounds.Width) / CDbl(iTotalWidth))))) + 14)

                        iHeaderHeight = CInt(e.Graphics.MeasureString(gridecol.HeaderText, gridecol.InheritedStyle.Font, iTmpWidth).Height) + 16

                        arrColumnLefts.Add(iLeftMargin)
                        arrColumnWidths.Add(iTmpWidth)

                        iLeftMargin += iTmpWidth

                    Next gridecol
                    
                End If

                Do While iRow <= area_wise_sale_grid.Rows.Count - 1

                    Dim Gridrow As DataGridViewRow = area_wise_sale_grid.Rows(iRow)

                    iCellHeight = Gridrow.Height + 8
                    Dim iCount As Integer = 0

                    If iTopMargin + iCellHeight >= e.MarginBounds.Height + e.MarginBounds.Top Then

                        bNewpag = True
                        bFirstpage = False
                        bMorePageToPrint = True

                        Exit Do

                    Else
                        If bNewpag Then
                            Dim d As Double = 0

                            If flag = 0 Then
                                d = -40

                            Else
                                d = 23
                            End If
                            'MsgBox(msg)
                            e.Graphics.DrawString(msg, New Font(area_wise_sale_grid.Font, FontStyle.Bold), Brushes.Black, e.MarginBounds.Left - 40, e.MarginBounds.Top - e.Graphics.MeasureString("Income Data Details.", New Font(area_wise_sale_grid.Font, FontStyle.Bold), e.MarginBounds.Width).Height - d)

                            Dim strdate As String = "Report Date : " & Date.Now.ToLongDateString & " " & Date.Now.ToShortTimeString

                            e.Graphics.DrawString(strdate, New Font(area_wise_sale_grid.Font, FontStyle.Bold), Brushes.Black, e.MarginBounds.Left + 40 + (e.MarginBounds.Width - e.Graphics.MeasureString(strdate, New Font(area_wise_sale_grid.Font, FontStyle.Bold), e.MarginBounds.Width).Width), e.MarginBounds.Top - e.Graphics.MeasureString("Income Data Details.", New Font(New Font(area_wise_sale_grid.Font, FontStyle.Bold), FontStyle.Bold), e.MarginBounds.Width).Height - d)

                            If flag = 0 Then
                                iTopMargin = e.MarginBounds.Top + 50
                                flag = 1
                            Else
                                iTopMargin = e.MarginBounds.Top
                            End If


                            For Each Gridcol As DataGridViewColumn In area_wise_sale_grid.Columns

                                e.Graphics.FillRectangle(New SolidBrush(Color.LightGray), New Rectangle(DirectCast(arrColumnLefts(iCount), Integer), iTopMargin, DirectCast(arrColumnWidths(iCount), Integer), iHeaderHeight))

                                e.Graphics.DrawRectangle(Pens.Black, New Rectangle(DirectCast(arrColumnLefts(iCount), Integer), iTopMargin, DirectCast(arrColumnWidths(iCount), Integer), iHeaderHeight))

                                e.Graphics.DrawString(Gridcol.HeaderText, Gridcol.InheritedStyle.Font, New SolidBrush(Gridcol.InheritedStyle.ForeColor), New Rectangle(DirectCast(arrColumnLefts(iCount), Integer), iTopMargin, DirectCast(arrColumnWidths(iCount), Integer), iHeaderHeight), strformate)

                                iCount += 1

                            Next Gridcol

                            bNewpag = False


                            iTopMargin += iHeaderHeight

                        End If

                        iCount = 0
                        For Each cel As DataGridViewCell In Gridrow.Cells

                            If cel.Value IsNot Nothing Then
                                e.Graphics.DrawString(cel.Value.ToString, cel.InheritedStyle.Font, New SolidBrush(cel.InheritedStyle.ForeColor), New Rectangle(DirectCast(arrColumnLefts(iCount), Integer), CSng(iTopMargin), DirectCast(arrColumnWidths(iCount), Integer), iHeaderHeight), strformate)
                            End If


                            e.Graphics.DrawRectangle(Pens.Black, New Rectangle(DirectCast(arrColumnLefts(iCount), Integer), iTopMargin, DirectCast(arrColumnWidths(iCount), Integer), iCellHeight))

                            iCount += 1

                        Next cel

                    End If

                    iRow += 1
                    iTopMargin += iCellHeight

                Loop

                If bMorePageToPrint Then
                    e.HasMorePages = True
                Else
                    e.HasMorePages = False
                End If

            Catch ex As Exception
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try

        End If

    End Sub


    ' check only number values
    Private Sub tmobile_TextChanged(sender As Object, e As EventArgs) Handles tmobile.TextChanged, tweight.TextChanged, twastage.TextChanged, t_wastage_check.TextChanged, tdiamond.TextChanged, tpweight.TextChanged, t_making.TextChanged, t_ocharge.TextChanged, t_cust_mobile.TextChanged ' t_cust_name.TextChanged,
        Dim txt As String = sender.text
        If Not IsNumeric(sender.text) And sender.text <> "" Then
            MsgBox("Enter Only Number !!")
            sender.text = txt.Substring(0, txt.Length - 1)
        End If

    End Sub



    ' product purchase supplier set
    Private Sub Supppliercombo_SelectedIndexChanged(sender As Object, e As EventArgs) Handles Supppliercombo.SelectedIndexChanged, prod_purchase_date.ValueChanged ', t_prod_purity.SelectedIndexChanged

        If auto_fill_sup <> 0 Then

            Dim query As String = "select purity, wastage from purchase where supplier_id = " + Supppliercombo.SelectedValue.ToString + " and pdate = '" + prod_purchase_date.Text.ToString + "'  "
            Dim cmd As New SqlCommand(query, DB.connection)
            Dim ds As New DataSet
            Dim da As New SqlDataAdapter(cmd)
            da.Fill(ds)

            If ds.Tables(0).Rows.Count = 0 Then
                MsgBox("Not purchase Found !")

            Else
                If ds.Tables(0).Rows.Count = 1 Then

                    If t_prod_purity.Text = "" Then
                        t_prod_purity.Text = ds.Tables(0).Rows(0).Item(0).ToString
                    End If
                    t_wastage_check.Text = ds.Tables(0).Rows(0).Item(1).ToString

                Else
                    ' add dynamic
                    auto_fill_sup = 0
                    t_prod_purity.Items.Clear()
                    t_prod_purity.Items.Add("")



                    ' sorting need
                   

                    Dim temp As Integer = 0
                    ' setting multiple items
                    For i As Integer = 0 To CInt(ds.Tables(0).Rows.Count) - 1
                        If temp <> CInt(ds.Tables(0).Rows(i).Item(0).ToString) Then
                            t_prod_purity.Items.Add(ds.Tables(0).Rows(i).Item(0).ToString)
                            temp = CInt(ds.Tables(0).Rows(i).Item(0).ToString)
                        End If
                    Next

                    auto_fill_sup = 1
                End If

            End If

        End If

    End Sub

    Private Sub t_prod_purity_SelectedIndexChanged(sender As Object, e As EventArgs) Handles t_prod_purity.SelectedIndexChanged
        If auto_fill_sup <> 0 Then
            Dim query As String = "select wastage from purchase where supplier_id = " + Supppliercombo.SelectedValue.ToString + " and pdate = '" + prod_purchase_date.Text.ToString + "' and purity = " + t_prod_purity.Text.ToString + " "
            Dim cmd As New SqlCommand(query, DB.connection)
            Dim ds As New DataSet
            Dim da As New SqlDataAdapter(cmd)
            da.Fill(ds)

            If ds.Tables(0).Rows.Count = 0 Then
                MsgBox("No Record Found !")
            Else
                t_wastage_check.Text = ds.Tables(0).Rows(0).Item(0)
            End If
        End If
    End Sub


    ' mis report panel button click
    Dim initialize_mis As Integer = 0

    Private Sub Button2_Click_1(sender As Object, e As EventArgs) Handles Button2.Click
        Panels.show("mis_report")


        mis_report = New Mis_report

        If initialize_mis = 0 Then
            mis_report.assign_controls()
        End If

        initialize_mis += 1
        ' start timer
        Timer1.Start()
    End Sub

    Private Sub Timer1_Tick(sender As Object, e As EventArgs) Handles Timer1.Tick
        '  Dim obj As New Mis_report
        mis_report.stock_run_out()
    End Sub

    
End Class