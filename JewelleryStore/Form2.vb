Imports System.Data
Imports System.Data.SqlClient

Public Class Form2

    Dim cmd As SqlCommand
    Dim ds As DataSet
    Dim da As SqlDataAdapter

    Private Sub Form2_Load(sender As Object, e As EventArgs) Handles Me.FormClosing
        Form1.Close()
    End Sub


    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click

        If tcname.Text <> "" And taddress.Text <> "" And tname.Text <> "" And tmobile.Text <> "" Then

            Dim query As String = "insert into supplier(cname, name, mobile, address) values( '" + tcname.Text + "', '" + tname.Text + "', '" + tmobile.Text + "', '" + taddress.Text + "' )"

            cmd = New SqlCommand(query, DB.connection)

            DB.connection.Open()
            If cmd.ExecuteNonQuery > 0 Then
                MsgBox("Supplier Added !!")
            End If
            DB.connection.Close()

        Else
            MsgBox("Please Provide All Details !!")
        End If

    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        tcname.Text = ""
        taddress.Text = ""
        tname.Text = ""
        tmobile.Text = ""
    End Sub

    

    Private Sub Form2_Load_1(sender As Object, e As EventArgs) Handles MyBase.Load
        refSupplier()
    End Sub


    Public Sub refSupplier()

        Dim query As String = "select * from supplier"
        cmd = New SqlCommand(query, DB.connection)
        da = New SqlDataAdapter(cmd)
        ds = New DataSet

        da.Fill(ds, "supplier")
        DataGridView1.DataSource = ds.Tables("supplier")
    End Sub

    Private Sub tmobile_TextChanged(sender As Object, e As EventArgs) Handles tmobile.TextChanged
      
    End Sub
End Class