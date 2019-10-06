Imports System.Data
Imports System.Data.SqlClient
Public Class Purchase

    Dim cmd As SqlCommand
    Dim da As SqlDataAdapter
    Dim ds As DataSet

    Dim purchase_id = 0, supplier_id As Integer = 0

    Dim temp_gold_price As Integer = 0

    Dim query As String

    Sub add_supplier()

        If Form4.tcname.Text <> "" And Form4.taddress.Text <> "" And Form4.tname.Text <> "" And Form4.tmobile.Text <> "" Then

            If Form4.tmobile.Text.Length = 10 Then

                query = "insert into supplier(cname, name, mobile, address) values( '" + Form4.tcname.Text + "', '" + Form4.tname.Text + "', '" + Form4.tmobile.Text + "', '" + Form4.taddress.Text + "' )"

                cmd = New SqlCommand(query, DB.connection)

                DB.connection.Open()
                If cmd.ExecuteNonQuery > 0 Then
                    MsgBox("Supplier Added !!")

                    ' refresh drop down
                    supplier_dropdown()
                End If
                DB.connection.Close()

            Else
                MsgBox("Mobile No 10 Digit !")
            End If
        Else
            MsgBox("Please Provide All Details !!")
        End If
        refSupplier()
    End Sub

    Sub cancle_supplier()
        Form4.tcname.Text = ""
        Form4.taddress.Text = ""
        Form4.tname.Text = ""
        Form4.tmobile.Text = ""

        Form4.btnupdateSupplier.Hide()
        Form4.btn_del_supplier.Hide()
        Form4.addSupplier.Show()

    End Sub

    ' refresh supplier
    Public Sub refSupplier()

        Dim query As String = "select * from supplier"
        cmd = New SqlCommand(query, DB.connection)
        da = New SqlDataAdapter(cmd)
        ds = New DataSet

        da.Fill(ds, "supplier")
        Form4.supplierGrid.DataSource = ds.Tables("supplier")
    End Sub

    ' select supplier for update
    Public Sub select_supplier()

        ' update supplier button show
        Form4.btnupdateSupplier.Show()
        Form4.btn_del_supplier.Show()
        Form4.addSupplier.Hide()

        Dim i As Integer = Form4.supplierGrid.CurrentRow.Index

        'MsgBox(Form4.supplierGrid.Item(0, i).Value)
        Dim id As Integer = Form4.supplierGrid.Item(0, i).Value
        Form4.tcname.Text = Form4.supplierGrid.Item(1, i).Value
        Form4.tname.Text = Form4.supplierGrid.Item(2, i).Value
        Form4.tmobile.Text = Form4.supplierGrid.Item(3, i).Value
        Form4.taddress.Text = Form4.supplierGrid.Item(4, i).Value
        supplier_id = Form4.supplierGrid.Item("supplier_id", i).Value

    End Sub

    ' update the supplier
    Public Sub update_supplier()

        If Form4.tcname.Text <> "" And Form4.taddress.Text <> "" And Form4.tname.Text <> "" And Form4.tmobile.Text <> "" Then

            If Form4.tmobile.Text.Length = 10 Then

                query = "update supplier set cname = '" + Form4.tcname.Text + "', name = '" + Form4.tname.Text + "', mobile = '" + Form4.tmobile.Text + "', address = '" + Form4.taddress.Text + "' where supplier_id = " + supplier_id.ToString + " "
                cmd = New SqlCommand(query, DB.connection)

                DB.connection.Open()
                If cmd.ExecuteNonQuery > 0 Then
                    MsgBox("Update Success !!!")

                    ' clear all textbox after update
                    cancle_supplier()
                    ' re select supplier dropdown in case update name
                    supplier_dropdown()
                    ' refresh gridview
                    refSupplier()
                    'refresh purchase gridview
                    select_purchase_data()

                Else
                    MsgBox("Error Accur during update !!!")
                End If
                DB.connection.Close()

            Else
                MsgBox("Mobile No 10 Digit Long !")
            End If

        Else
            MsgBox("Filled Up all Details !!")
        End If

    End Sub


    ' delete supplier
    Sub del_supplier()

        Try
            'query = "insert into supplier(cname, name, mobile, address) values( '" + Form4.tcname.Text + "', '" + Form4.tname.Text + "', '" + Form4.tmobile.Text + "', '" + Form4.taddress.Text + "' )"

            query = "delete from supplier where supplier_id = " + supplier_id.ToString + " "

            cmd = New SqlCommand(query, DB.connection)

            DB.connection.Open()
            If cmd.ExecuteNonQuery > 0 Then
                MsgBox("Supplier Deleted !!")

                cancle_supplier()
                ' re select supplier dropdown in case update name
                supplier_dropdown()
                ' refresh gridview
                refSupplier()
                'refresh purchase gridview
                select_purchase_data()

            End If

        Catch ex As Exception
            MsgBox("First Delete All Sub Data of supplier !")
        Finally
            DB.connection.Close()
        End Try
        

    End Sub


    ' adding purchase item
    Public Sub add_purchase()

        If Form4.tweight.Text <> "" And Form4.twastage.Text <> "" And Form4.tpurity.Text <> "" Then

            ' check same supplier / purity / same day / same wastage then UPDATE
            ' Select Record if already inserted then update

            query = "select * from purchase where supplier_id = " + Form4.tsupplier.SelectedValue.ToString + " and pdate = '" + Form4.pdate.Text.ToString + "' and purity = " + Form4.tpurity.Text + " and wastage = " + Form4.twastage.Text + " "
            cmd = New SqlCommand(query, DB.connection)
            da = New SqlDataAdapter(cmd)
            ds = New DataSet()
            da.Fill(ds, "temp")

            'MsgBox(ds.Tables("temp").Rows.Count.ToString & " rows " & ds.Tables("temp").Rows(0).Item("supplier_id") & " <- sid")

            Dim tot_purchase As Integer = 0
            Dim gp As Integer = Math.Round((Form4.today_gold_price * (CInt(Form4.tpurity.Text) + CInt(Form4.twastage.Text))) / 1000)
            tot_purchase = Math.Round(Form4.tweight.Text * gp)

            If ds.Tables("temp").Rows.Count > 0 Then

                ' Update record if already inserted
                query = "update purchase set weight = weight + " & CInt(Form4.tweight.Text.ToString) & ", total_price = total_price + " & CInt(tot_purchase.ToString) & " where purchase_id = " + ds.Tables("temp").Rows(0).Item("purchase_id").ToString + " "
                cmd = New SqlCommand(query, DB.connection)

                DB.connection.Open()
                If cmd.ExecuteNonQuery > 0 Then
                    MsgBox("Item Updated")

                    ' grid view refresh
                    select_purchase_data()
                    clear_purchase()

                Else
                    MsgBox("Retry Something wrong !!")
                End If
                DB.connection.Close()

                'MsgBox("Update Old Record !")
            Else

                query = "insert into purchase(supplier_id, weight, wastage, purity, pdate, gold_id, total_price) values( " + Form4.tsupplier.SelectedValue.ToString + "," + Form4.tweight.Text + "," + Form4.twastage.Text + ", " + Form4.tpurity.Text + ", '" + Form4.pdate.Text + "', " + Form4.gold_id.ToString + ", " + tot_purchase.ToString + ")"
                cmd = New SqlCommand(query, DB.connection)
                DB.connection.Open()
                If cmd.ExecuteNonQuery > 0 Then
                    MsgBox("Purchased Item Added !")

                    ' grid view refresh
                    select_purchase_data()
                    clear_purchase()

                Else
                    MsgBox("Retry Something wrong !!")
                End If
                DB.connection.Close()
            End If
        Else
            MsgBox("Enter All Data !!")
        End If

    End Sub

    ' clear purchase item

    Public Sub clear_purchase()
        Form4.tweight.Text = ""
        Form4.tpurity.Text = ""
        Form4.twastage.Text = ""

        'hide show update button
        Form4.btnpurchaseupdate.Hide()
        Form4.btnaddpurchase.Show()

        Form4.delete_purchase.Hide()
    End Sub


    ' tsupplier drop down
    Public Sub supplier_dropdown()
        query = "select supplier_id,cname from supplier"
        cmd = New SqlCommand(query, DB.connection)
        da = New SqlDataAdapter(cmd)
        ds = New DataSet

        da.Fill(ds, "supplier")

        Form4.tsupplier.DataSource = ds.Tables("supplier")
        Form4.tsupplier.DisplayMember = "cname"
        Form4.tsupplier.ValueMember = "supplier_id"

        'Form4.Supppliercombo.DataSource = ds.Tables("supplier")
        'Form4.Supppliercombo.DisplayMember = "cname"
        'Form4.Supppliercombo.ValueMember = "supplier_id"

    End Sub

    ' selecting purchase data in grid view

    Public Sub select_purchase_data()
        'query = "select p.*,s.cname as supplier, price from purchase p, supplier s, gold_price gp where s.supplier_id = p.supplier_id and gp.gold_id = p.gold_id"
        query = "select p.purchase_id, s.cname as supplier, p.weight, p.purity, p.wastage as 'wastage(in %)', gp.price gold_price, p.pdate, p.total_price total from purchase p, supplier s, gold_price gp where s.supplier_id = p.supplier_id and gp.gold_id = p.gold_id"
        cmd = New SqlCommand(query, DB.connection)
        da = New SqlDataAdapter(cmd)
        ds = New DataSet

        da.Fill(ds, "purchase")

        Form4.purchasegridview.DataSource = ds.Tables("purchase")

    End Sub


    ' select purchase gridview in textbox

    Public Sub select_purchase_for_update()
        Dim i As Integer = Form4.purchasegridview.CurrentRow.Index

        'MsgBox("Current gp = " & Form4.today_gold_price)

        'MsgBox(Form4.supplierGrid.Item(0, i).Value)
        Dim id As Integer = Form4.purchasegridview.Item(0, i).Value
        Form4.tweight.Text = Form4.purchasegridview.Item("weight", i).Value
        Form4.twastage.Text = Form4.purchasegridview.Item("wastage(in %)", i).Value
        Form4.tpurity.Text = Form4.purchasegridview.Item("purity", i).Value
        Form4.pdate.Text = Form4.purchasegridview.Item("pdate", i).Value
        Form4.tsupplier.Text = Form4.purchasegridview.Item("supplier", i).Value

        ' gold id set
        Form4.tgold_price.Text = Form4.purchasegridview.Item("gold_price", i).Value
        temp_gold_price = Form4.purchasegridview.Item("gold_price", i).Value.ToString

        ' set --------------------------->   Form4.today_gold_price
        purchase_id = Form4.purchasegridview.Item("purchase_id", i).Value

        'hide show update button
        Form4.btnpurchaseupdate.Show()
        Form4.btnaddpurchase.Hide()

        Form4.delete_purchase.Show()
    End Sub

    ' update purchase data

    Public Sub update_purchase()

        Try
            If Form4.tweight.Text <> "" And Form4.twastage.Text <> "" And Form4.tpurity.Text <> "" Then

                Dim tot_purchase As Integer = 0
                Dim gp As Integer = Math.Round((temp_gold_price * (CInt(Form4.tpurity.Text) + CInt(Form4.twastage.Text))) / 1000)
                tot_purchase = Math.Round(Form4.tweight.Text * gp)

                'MsgBox("gp = " & gp & " total = " & tot_purchase)

                ' old ------> gold_id = (select gold_id from gold_price where gold_id = (select max(gold_id) from gold_price)) ,
                query = "update purchase set total_price = " + tot_purchase.ToString + ", supplier_id =" + Form4.tsupplier.SelectedValue.ToString + ", weight = " + Form4.tweight.Text + ", wastage = " + Form4.twastage.Text + ", purity = " + Form4.tpurity.Text + ", pdate = '" + Form4.pdate.Text + "' where purchase_id = " + purchase_id.ToString + " "

                cmd = New SqlCommand(query, DB.connection)

                DB.connection.Open()
                If cmd.ExecuteNonQuery > 0 Then
                    MsgBox("Update Successful !!")

                    select_purchase_data()
                    supplier_dropdown()
                    clear_purchase()

                    ' set today gold price
                    Form4.tgold_price.Text = Form4.today_gold_price
                Else
                    MsgBox("Some Problem Occur During Update !!")
                End If

            Else
                MsgBox("Filledup All Details !!")
            End If
        Catch ex As Exception
            MsgBox("some Problem Occur !")

        Finally
            DB.connection.Close()
        End Try

        
    End Sub

    ' Delete purchase 

    Sub delete_purchase()
        Try
            ' count before delete
            query = "select * from purchase where purchase_id = " + purchase_id.ToString + " "
            cmd = New SqlCommand(query, DB.connection)
            da = New SqlDataAdapter(cmd)
            ds = New DataSet
            da.Fill(ds, "sale")

            ' supplier_id -> purity -> purchse_date
            ' 0 -> 4 -> 5

            query = "select * from product_detail where purchase_date = '" + ds.Tables("sale").Rows(0).Item(5).ToString + "' and supplier_id = " + ds.Tables("sale").Rows(0).Item(1).ToString + " and purity = " + ds.Tables("sale").Rows(0).Item(4).ToString + " "

            cmd = New SqlCommand(query, DB.connection)
            da = New SqlDataAdapter(cmd)
            ds = New DataSet
            da.Fill(ds, "sale")

            If ds.Tables("sale").Rows.Count = 0 Then

                ' delete record
                query = "delete from purchase where purchase_id = " + purchase_id.ToString + " "
                cmd = New SqlCommand(query, DB.connection)
                DB.connection.Open()
                If cmd.ExecuteNonQuery > 0 Then
                    MsgBox("Delete Successfully !!")

                    select_purchase_data()
                    supplier_dropdown()
                    clear_purchase()

                Else
                    MsgBox("Some Problem Occur During Update !!")
                End If


            Else
                MsgBox("First Delete All Child Record !!")
            End If

        Catch ex As Exception
            MsgBox("First Delete All Child Record !!" & ex.Message)

        Finally
            DB.connection.Close()
        End Try

    End Sub

End Class