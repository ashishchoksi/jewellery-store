Imports System.Data
Imports System.Data.SqlClient

Public Class Product

    Dim query As String = ""
    Dim cmd As SqlCommand
    Dim ds As DataSet
    Dim da As SqlDataAdapter

    Dim product_master_id, product_detail_id As Integer

    Public Sub New()

        ' add product button click
        AddHandler Form4.btn_add_prod.Click, AddressOf btn_add_productMaster_click

        ' cancel product button click
        AddHandler Form4.btn_cancel_prod.Click, AddressOf btn_cancel_productmaster_click

        ' update product button click
        AddHandler Form4.btn_update_prod.Click, AddressOf btn_update_productmaster_click

        ' product_master grid double click
        AddHandler Form4.product_master_grid.CellContentClick, AddressOf product_master_grid_select

        ' delete product_master
        AddHandler Form4.btn_delete_product_master.Click, AddressOf delete_product_master

        ' ------------------------------------------------------------------------------------------------------------------

        ' product_detail Add product button click
        AddHandler Form4.btn_addprod_detail.Click, AddressOf add_product_detail

        ' product_detial update button clicked  
        AddHandler Form4.btn_updateprod_detail.Click, AddressOf update_product_detail

        ' product_detail cancel button clicked
        AddHandler Form4.btn_cancel_prod_detail.Click, AddressOf cancel_product_detail

        ' product_detail grid double click
        AddHandler Form4.prod_detail_grid.CellContentClick, AddressOf product_detail_grid_select

        ' Delete Product detail
        AddHandler Form4.btn_delete_product_detail.Click, AddressOf delete_product_detail

        ' dropdown product_master refresh
        product_master_dropdown()

        ' dropdown supplier
        supplier_combo_dropdown()

        ' product detail grid
        prod_detail_grid()

        ' product add reminder gridview
        set_purchase_product_grid("")

        Form4.remind_supplier_combo.Enabled = False
        Form4.remind_purchase_date.Enabled = False

        ' search in reminder

        AddHandler Form4.remind_search.Click, AddressOf search_reminded_item
    End Sub


    ' add to product_master
    Private Sub btn_add_productMaster_click(sender As Object, e As EventArgs)

        If Form4.t_prod_master.Text <> "" Then

            query = "insert into product_master(pm_name) values('" + Form4.t_prod_master.Text + "')"
            cmd = New SqlCommand(query, DB.connection)

            DB.connection.Open()
            If cmd.ExecuteNonQuery > 0 Then
                MsgBox("Product Added !!")


                Form4.t_prod_master.Text = ""

                ' refresh gridview
                show_purchase_master()

                ' product master drop down
                product_master_dropdown()
            Else
                MsgBox("Something going wrong !!")
            End If
            DB.connection.Close()

        Else
            MsgBox("Filled up all details !!")
        End If

    End Sub

    ' cancel button click product master
    Public Sub btn_cancel_productmaster_click()
        Form4.t_prod_master.Text = ""
        'Form4.t_gender.Text = ""

        'hide show button
        Form4.btn_update_prod.Hide()
        Form4.btn_add_prod.Show()
        Form4.btn_delete_product_master.Hide()

    End Sub


    ' supplier combo dropdown
    Sub supplier_combo_dropdown()

        query = "select p.supplier_id , s.cname from purchase p, supplier s where p.supplier_id = s.supplier_id group by p.supplier_id, s.cname"
        cmd = New SqlCommand(query, DB.connection)
        da = New SqlDataAdapter(cmd)
        ds = New DataSet

        da.Fill(ds, "supplier")

        Form4.Supppliercombo.DataSource = ds.Tables("supplier")
        Form4.Supppliercombo.DisplayMember = "cname"
        Form4.Supppliercombo.ValueMember = "supplier_id"

        Form4.remind_supplier_combo.DataSource = ds.Tables("supplier")
        Form4.remind_supplier_combo.DisplayMember = "cname"
        Form4.remind_supplier_combo.ValueMember = "supplier_id"

        Form4.auto_fill_sup += 1

    End Sub

    ' purchase master gridview data show
    Public Sub show_purchase_master()
        query = "select * from product_master"
        cmd = New SqlCommand(query, DB.connection)
        da = New SqlDataAdapter(cmd)
        ds = New DataSet

        da.Fill(ds, "product_master")
        Form4.product_master_grid.DataSource = ds.Tables("product_master")

    End Sub

    ' select product_master for update
   
    Private Sub product_master_grid_select(sender As Object, e As DataGridViewCellEventArgs)

        'hide show button update/insert
        Form4.btn_update_prod.Show()
        Form4.btn_add_prod.Hide()
        Form4.btn_delete_product_master.Show()

        Dim i As Integer = Form4.product_master_grid.CurrentRow.Index

        Try
            product_master_id = Form4.product_master_grid.Item("pm_id", i).Value
            Form4.t_prod_master.Text = Form4.product_master_grid.Item("pm_name", i).Value
            'Form4.t_gender.Text = Form4.product_master_grid.Item("gender", i).Value



        Catch ex As Exception
            MsgBox("something going wrong")
        End Try
        
    End Sub

    ' product master update button clicked
    Private Sub btn_update_productmaster_click(sender As Object, e As EventArgs)

        If Form4.t_prod_master.Text <> "" Then
            query = "update product_master set pm_name = '" + Form4.t_prod_master.Text + "' where pm_id = " + product_master_id.ToString + " "
            cmd = New SqlCommand(query, DB.connection)
            DB.connection.Open()
            If cmd.ExecuteNonQuery > 0 Then
                MsgBox("Product updated !!")

                Form4.t_prod_master.Text = ""

                ' refresh gridview
                show_purchase_master()

                'hide show update button
                Form4.btn_update_prod.Hide()
                Form4.btn_add_prod.Show()
                Form4.btn_delete_product_master.Hide()
            Else
                MsgBox("Something going wrong !!")
            End If
            DB.connection.Close()
        Else
            MsgBox("Filled up all details !!")
        End If

    End Sub

    ' Delete Product master Record
    Private Sub delete_product_master(sender As Object, e As EventArgs)
        Try
            query = "delete from product_master where pm_id = " + product_master_id.ToString + " "
            cmd = New SqlCommand(query, DB.connection)
            DB.connection.Open()
            If cmd.ExecuteNonQuery > 0 Then
                MsgBox("Product Deleted !!")

                
                Form4.t_prod_master.Text = ""
                ' refresh gridview
                show_purchase_master()

                'hide show update button
                Form4.btn_update_prod.Hide()
                Form4.btn_add_prod.Show()
                Form4.btn_delete_product_master.Hide()
            Else
                MsgBox("Something going wrong !!")
            End If

        Catch ex As Exception
            MsgBox("First Delete All Related Child Record")
        End Try
    End Sub


    ' drop down product master name
    Sub product_master_dropdown()

        cmd = New SqlCommand("select * from product_master", DB.connection)
        da = New SqlDataAdapter(cmd)
        ds = New DataSet

        da.Fill(ds, "product_master")

        Form4.tproductmaster.DataSource = ds.Tables("product_master")
        Form4.tproductmaster.DisplayMember = "pm_name"
        Form4.tproductmaster.ValueMember = "pm_id"

        ' form5 supplier combo
        Form5.product_combo.DataSource = ds.Tables("product_master")
        Form5.product_combo.DisplayMember = "pm_name"
        Form5.product_combo.ValueMember = "pm_id"

    End Sub

    ' ---------------------------------------------------  Product Detail --------------------------------------------------

    Dim purchase_id As Integer = 0

    ' add_product_detail button click


    ' editng mode .................................................................

    Private Sub add_product_detail(sender As Object, e As EventArgs)
       
        ' back up 1
        Try

            If Form4.tproductmaster.Text <> "" And Form4.tpweight.Text <> "" And Form4.tdiamond.Text <> "" And Form4.Supppliercombo.Text <> "" And Form4.t_making.Text <> "" And Form4.t_ocharge.Text <> "" Then

                ' select wastage and gold_id combine => ( supplier_id & purchase_date & purity & wastage )

                ' cost price calculated -------------------------
                Dim gold_id = 0, cost = 0, gold = 0, wastage As Integer = 0

                query = "select p.gold_id as gold_id, g.price as price, p.purchase_id as purchase, wastage from purchase p, gold_price g where supplier_id = " + Form4.Supppliercombo.SelectedValue.ToString + " and purity = " + Form4.t_prod_purity.Text + " and pdate = '" + Form4.prod_purchase_date.Text + "' and p.gold_id = g.gold_id and wastage = " + Form4.t_wastage_check.Text + " "
                cmd = New SqlCommand(query, DB.connection)

                DB.connection.Open()
                Dim dr As SqlDataReader = cmd.ExecuteReader()

                If Not dr.HasRows Then
                    MsgBox("Enter Proper 'Supplier Name' or 'purchase date' or 'purity' or 'wastage' where you buy from supplier")

                Else

                    ' insert code

                    While dr.Read

                        gold_id = dr("gold_id").ToString
                        gold = dr("price").ToString
                        wastage = dr("wastage").ToString
                        purchase_id = dr("purchase").ToString
                    End While
                    ' DB.connection.Close()

                    ' ---------------------->>>>>>>>>>>> MsgBox("purchase id = " & purchase_id)

                    ' calculate cost price
                    cost = Math.Round(((gold * (CInt(Form4.t_prod_purity.Text) + wastage)) / 1000) * CInt(Form4.tpweight.Text))

                    ' ---------------------------------------------------
                    query = "insert into product_detail(pm_id, weight, diamonds, purity, supplier_id, making, other_charge, cost_price, purchase_date, added_date, purchase_id) values(" + Form4.tproductmaster.SelectedValue.ToString + ", " + Form4.tpweight.Text + ", " + Form4.tdiamond.Text + ", " + Form4.t_prod_purity.Text + " ," + Form4.Supppliercombo.SelectedValue.ToString + ", " + Form4.t_making.Text + ", " + Form4.t_ocharge.Text + ", " + cost.ToString + " ,'" + Form4.prod_purchase_date.Text + "', '" + Date.Now.Date + "', " + purchase_id.ToString + ") "
                    cmd = New SqlCommand(query, DB.connection)
                    dr.Close()

                    ' checking weight is enough for purchase_detail

                    Dim q2 As String
                    q2 = "select sum(pd.weight),  p.weight from product_detail pd, purchase p where p.purchase_id = pd.purchase_id and p.purchase_id=" + purchase_id.ToString + " and pd.purchase_date = '" + Form4.prod_purchase_date.Text.ToString + "' and p.purity= " + Form4.t_prod_purity.Text.ToString + " group by p.weight"
                    Dim cmd1 As New SqlCommand(q2, DB.connection)
                    Dim da1 As New SqlDataAdapter(cmd1)
                    Dim ds1 As New DataSet
                    da1.Fill(ds1)

                    'MsgBox(ds1.Tables(0).Rows(0).Item(0) & " <-> " & ds1.Tables(0).Rows(0).Item(1))

                    If ds1.Tables(0).Rows.Count <> 0 Then
                        Dim plus_weight As Integer = CInt(ds1.Tables(0).Rows(0).Item(0)) + CInt(Form4.tpweight.Text)
                        If (plus_weight > CInt(ds1.Tables(0).Rows(0).Item(1))) Then

                            MsgBox("Your Item weight must not grater than you purchase weight")

                        Else
                            If cmd.ExecuteNonQuery > 0 Then
                                MsgBox("Inserted !!!")
                                prod_detail_grid()

                                ' refresh prod reminder grid
                                set_purchase_product_grid("")

                                ' clear Product
                                cancel_product_detail()
                            Else
                                MsgBox("Something Going wrong 1!!")
                            End If

                        End If

                    Else
                        ' insert
                        If cmd.ExecuteNonQuery > 0 Then
                            MsgBox("Inserted !!!")
                            prod_detail_grid()

                            ' refresh prod reminder grid
                            set_purchase_product_grid("")

                            ' clear Product
                            cancel_product_detail()
                        Else
                            MsgBox("Something Going wrong 2!!")
                        End If
                    End If

                    'DB.connection.Open()

                End If
                dr.Close()
                DB.connection.Close()

            Else
                MsgBox("Enter All Detail !")
            End If


        Catch ex As Exception
            MsgBox("Something going wrong TRY AGAIN !")
        End Try


    End Sub


    Private Sub cancel_product_detail()
        ' cancel button cliked

        Form4.auto_fill_sup = 0

        Form4.t_prod_purity.Items.Clear()
        Form4.t_prod_purity.Items.Add("")
        Form4.t_prod_purity.Items.Add("75")
        Form4.t_prod_purity.Items.Add("84")
        Form4.t_prod_purity.Items.Add("92")

        'hide show button product detail
        Form4.btn_addprod_detail.Show()
        Form4.btn_updateprod_detail.Hide()
        Form4.btn_delete_product_detail.Hide()

        Form4.tproductmaster.Text = ""
        Form4.tpweight.Text = ""
        Form4.tdiamond.Text = ""
        Form4.Supppliercombo.Text = ""
        Form4.t_making.Text = ""
        Form4.t_ocharge.Text = ""
        Form4.t_prod_purity.Text = ""
        Form4.t_wastage_check.Text = ""

        Form4.prod_purchase_date.Text = Date.Now.Date.ToString

        Form4.auto_fill_sup = 1
    End Sub


    ' main query logic is here
    Sub prod_detail_grid()

        cmd = New SqlCommand("select p.pd_id as pd_id, pm.pm_name as product_name, p.weight, p.diamonds, p.making, p.other_charge, s.cname as supplier, p.purity , Round(( ( ( (select price from gold_price where gold_id = (select max(gold_id) from gold_price)) * purity ) / 1000 ) * ( weight - (diamonds * 0.002) ) ) + ( ( weight - (diamonds * 0.002) ) * making ) + ( diamonds * 35 ) + other_charge, 0) as sell_price, purchase_date, p.cost_price,p.purchase_id from product_detail p, product_master pm, supplier s where pm.pm_id = p.pm_id and p.supplier_id = s.supplier_id and p.pd_id not in (select pd_id from sales_detail) ", DB.connection)
        da = New SqlDataAdapter(cmd)
        ds = New DataSet
        da.Fill(ds, "product_detail")

        Form4.prod_detail_grid.DataSource = ds.Tables("product_detail")

        ' select product for salse
        Form5.select_product_grid.DataSource = ds.Tables("product_detail")
    End Sub

    ' old purchase id --> for check consistancy update time
    Dim old_purchase As Integer

    Private Sub product_detail_grid_select(sender As Object, e As DataGridViewCellEventArgs)

        Form4.auto_fill_sup = 0

        'hide show button product detail
        Form4.btn_addprod_detail.Hide()
        Form4.btn_updateprod_detail.Show()
        Form4.btn_delete_product_detail.Show()

        Dim i As Integer = Form4.prod_detail_grid.CurrentRow.Index

        Form4.tproductmaster.Text = Form4.prod_detail_grid.Item("product_name", i).Value
        Form4.tpweight.Text = Form4.prod_detail_grid.Item("weight", i).Value
        Form4.tdiamond.Text = Form4.prod_detail_grid.Item("diamonds", i).Value
        Form4.Supppliercombo.Text = Form4.prod_detail_grid.Item("supplier", i).Value
        Form4.t_making.Text = Form4.prod_detail_grid.Item("making", i).Value
        Form4.t_ocharge.Text = Form4.prod_detail_grid.Item("other_charge", i).Value
        Form4.t_prod_purity.Text = Form4.prod_detail_grid.Item("purity", i).Value
        Form4.prod_purchase_date.Text = Form4.prod_detail_grid.Item("purchase_date", i).Value

        product_detail_id = Form4.prod_detail_grid.Item("pd_id", i).Value

        old_purchase = Form4.tpweight.Text.ToString

        ' just get wastage
        Dim sup As Integer = Form4.prod_detail_grid.Item("purchase_id", i).Value
        purchase_id = sup
        cmd = New SqlCommand("select wastage from purchase where purchase_id = " + sup.ToString + " ", DB.connection)
        da = New SqlDataAdapter(cmd)
        ds = New DataSet
        da.Fill(ds, "wastage")
        Form4.t_wastage_check.Text = ds.Tables("wastage").Rows(0).Item(0).ToString

        'Form4.auto_fill_sup = 1
    End Sub


    Private Sub update_product_detail(sender As Object, e As EventArgs)
        ' upadate button click product detail table


        If Form4.tproductmaster.Text <> "" And Form4.tpweight.Text <> "" And Form4.tdiamond.Text <> "" And Form4.Supppliercombo.Text <> "" And Form4.t_making.Text <> "" And Form4.t_ocharge.Text <> "" Then

            ' select wastage and gold_id combine => ( supplier_id & purchase_date & purity & wastage )

            ' cost price calculated -------------------------
            Dim gold_id = 0, cost = 0, gold = 0, wastage As Integer = 0

            query = "select p.gold_id as gold_id, g.price as price, p.purchase_id as purchase, wastage from purchase p, gold_price g where supplier_id = " + Form4.Supppliercombo.SelectedValue.ToString + " and purity = " + Form4.t_prod_purity.Text + " and pdate = '" + Form4.prod_purchase_date.Text + "' and p.gold_id = g.gold_id and wastage = " + Form4.t_wastage_check.Text + " and p.purchase_id = " + purchase_id.ToString + " "
            cmd = New SqlCommand(query, DB.connection)

            'MsgBox(query)

            DB.connection.Open()
            Dim dr As SqlDataReader = cmd.ExecuteReader()

            If Not dr.HasRows Then
                MsgBox("Enter Proper 'Supplier Name' or 'purchase date' or 'purity' or 'wastage' where you buy from supplier")

            Else

                ' insert code

                While dr.Read
                    gold_id = dr("gold_id").ToString
                    gold = dr("price").ToString
                    wastage = dr("wastage").ToString
                    purchase_id = dr("purchase").ToString
                End While
                dr.Close()

                Dim q2 As String
                q2 = "select sum(pd.weight),  p.weight from product_detail pd, purchase p where p.purchase_id = pd.purchase_id and p.purchase_id=" + purchase_id.ToString + " and pd.purchase_date = '" + Form4.prod_purchase_date.Text.ToString + "' and p.purity= " + Form4.t_prod_purity.Text.ToString + " and p.wastage = " + Form4.t_wastage_check.Text + " group by p.weight"
                Dim cmd1 As New SqlCommand(q2, DB.connection)
                Dim da1 As New SqlDataAdapter(cmd1)
                Dim ds1 As New DataSet
                da1.Fill(ds1)

                'MsgBox(ds1.Tables(0).Rows(0).Item(0) & " <-> " & ds1.Tables(0).Rows(0).Item(1))

                Dim plus_weight As Integer = CInt(ds1.Tables(0).Rows(0).Item(0)) - old_purchase + CInt(Form4.tpweight.Text)

                If (plus_weight > CInt(ds1.Tables(0).Rows(0).Item(1))) Then
                    MsgBox("Your Item weight must not grater than you purchase weight")
                Else

                    ' calculate cost price
                    cost = Math.Round(((gold * (CInt(Form4.t_prod_purity.Text) + wastage)) / 1000) * CInt(Form4.tpweight.Text))

                    ' ---------------------------------------------------

                    query = "update product_detail set weight = " + Form4.tpweight.Text + ", pm_id = " + Form4.tproductmaster.SelectedValue.ToString + ", diamonds = " + Form4.tdiamond.Text + ", purity = " + Form4.t_prod_purity.Text + ", supplier_id = " + Form4.Supppliercombo.SelectedValue.ToString + ", making = " + Form4.t_making.Text + ", other_charge = " + Form4.t_ocharge.Text + ", cost_price = " + cost.ToString + ", purchase_date = '" + Form4.prod_purchase_date.Text + "' where pd_id = " + product_detail_id.ToString + " "

                    cmd = New SqlCommand(query, DB.connection)

                    If cmd.ExecuteNonQuery > 0 Then
                        MsgBox("Record Updated !!")
                        prod_detail_grid()

                        ' refresh prod reminder grid
                        set_purchase_product_grid("")

                        ' clear Product
                        cancel_product_detail()

                        'hide show button product detail
                        Form4.btn_addprod_detail.Show()
                        Form4.btn_updateprod_detail.Hide()
                        Form4.btn_delete_product_detail.Hide()
                    Else
                        MsgBox("Something Going wrong !!")
                    End If

                End If

            End If
            DB.connection.Close()
        Else
            MsgBox("Enter All Detail !")
        End If

    End Sub

    ' Delete Product Detail
    Private Sub delete_product_detail(sender As Object, e As EventArgs)
        Try
            query = "delete from product_detail where pd_id = " + product_detail_id.ToString + " "
            cmd = New SqlCommand(query, DB.connection)
            DB.connection.Open()
            If cmd.ExecuteNonQuery > 0 Then
                MsgBox("Record Deleted !!")
                prod_detail_grid()

                ' refresh prod reminder grid
                set_purchase_product_grid("")

                ' clear Product
                cancel_product_detail()

                'hide show button product detail
                Form4.btn_addprod_detail.Show()
                Form4.btn_updateprod_detail.Hide()
                Form4.btn_delete_product_detail.Hide()
            Else
                MsgBox("Something Going wrong !!")
            End If

            DB.connection.Close()
        Catch ex As Exception
            MsgBox("Not Delete !! " & ex.Message)
        End Try
    End Sub


    ' search reminded item
    Private Sub search_reminded_item(sender As Object, e As EventArgs)
        'old -> Query -> query = "select s.cname supplier, p.weight Total_purchase, sum(pd.weight) Added_to_stock, p.weight - sum(pd.weight) as remaining_stock, count(*) no_of_item_added, p.pdate purchase_date from product_detail pd, purchase p, supplier s where p.purchase_id = pd.purchase_id and s.supplier_id = p.supplier_id group by p.weight ,s.cname, p.pdate having (p.weight - sum(pd.weight)) <> 0"

        ' backup below ---->
        'query = "select (select cname from supplier where supplier_id = p.supplier_id) Supplier, p.weight Total_weight, p.wastage, ISNULL(sum(pd.weight),0) Added_to_stock, p.weight - ISNULL(sum(pd.weight),0) as remaining,count(pd.weight) no_of_item_added, p.pdate purchase_date from purchase p left join product_detail pd on pd.purchase_id = p.purchase_id group by pd.purchase_id, p.supplier_id, p.weight,p.wastage,p.pdate having (p.weight - ISNULL(sum(pd.weight),0)) <> 0 "

        query = "select (select cname from supplier where supplier_id = p.supplier_id) Supplier, p.weight Total_weight, p.wastage, p.purity, ISNULL(sum(pd.weight),0) Added_to_stock, p.weight - ISNULL(sum(pd.weight),0) as remaining, count(pd.weight) no_of_item_added, p.pdate purchase_date from purchase p left join product_detail pd on pd.purchase_id = p.purchase_id group by pd.purchase_id, p.supplier_id, p.weight,p.wastage,p.pdate , p.purity having (p.weight - ISNULL(sum(pd.weight),0)) <> 0 "

        'If Form4.remind_supplier.Checked And Form4.remind_date.Checked Then
        '    query = "select s.cname supplier, p.weight Total_purchase, sum(pd.weight) Added_to_stock, p.weight - sum(pd.weight) as remaining_stock, count(*) no_of_item_added, p.pdate purchase_date from product_detail pd, purchase p, supplier s where p.purchase_id = pd.purchase_id and s.supplier_id = p.supplier_id and s.cname = '" + Form4.remind_supplier_combo.Text.ToString + "' and p.pdate = '" + Form4.remind_purchase_date.Text.ToString + "' group by p.weight ,s.cname, p.pdate having (p.weight - sum(pd.weight)) <> 0"

        'Else
        '    If Form4.remind_supplier.Checked Then
        '        query = "select s.cname supplier, p.weight Total_purchase, sum(pd.weight) Added_to_stock, p.weight - sum(pd.weight) as remaining_stock, count(*) no_of_item_added, p.pdate purchase_date from product_detail pd, purchase p, supplier s where p.purchase_id = pd.purchase_id and s.supplier_id = p.supplier_id and s.cname = '" + Form4.remind_supplier_combo.Text.ToString + "' group by p.weight ,s.cname, p.pdate having (p.weight - sum(pd.weight)) <> 0"
        '    End If

        '    If Form4.remind_date.Checked Then
        '        query = "select s.cname supplier, p.weight Total_purchase, sum(pd.weight) Added_to_stock, p.weight - sum(pd.weight) as remaining_stock, count(*) no_of_item_added, p.pdate purchase_date from product_detail pd, purchase p, supplier s where p.purchase_id = pd.purchase_id and s.supplier_id = p.supplier_id and p.pdate = '" + Form4.remind_purchase_date.Text.ToString + "' group by p.weight ,s.cname, p.pdate having (p.weight - sum(pd.weight)) <> 0"
        '    End If

        'End If

        If Form4.remind_supplier.Checked Then
            query &= "and p.supplier_id = " + Form4.remind_supplier_combo.SelectedValue.ToString + " "
        End If

        If Form4.remind_date.Checked Then
            query &= "and p.pdate = '" + Form4.remind_purchase_date.Text.ToString + "' "
        End If

        'MsgBox(query)
        set_purchase_product_grid(query)
    End Sub

    ' purchase item added to product grid
    Sub set_purchase_product_grid(q As String)

        If (q = "") Then
            query = "select (select cname from supplier where supplier_id = p.supplier_id) Supplier, p.weight Total_weight, p.wastage, ISNULL(sum(pd.weight),0) Added_to_stock, p.weight - ISNULL(sum(pd.weight),0) as remaining,count(pd.weight) no_of_item_added, p.pdate purchase_date from purchase p left join product_detail pd on pd.purchase_id = p.purchase_id group by pd.purchase_id, p.supplier_id, p.weight,p.wastage,p.pdate having (p.weight - ISNULL(sum(pd.weight),0)) <> 0"
        Else
            query = q
        End If

        cmd = New SqlCommand(query, DB.connection)
        da = New SqlDataAdapter(cmd)
        ds = New DataSet
        da.Fill(ds, "add_prod")
        Form4.prod_add_remonder_grid.DataSource = ds.Tables("add_prod")
    End Sub

End Class
