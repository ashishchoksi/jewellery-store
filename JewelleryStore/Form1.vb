
Imports System.Data
Imports System.Data.SqlClient

Public Class Form1

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click

        Dim uname = TextBox1.Text
        Dim pass = TextBox2.Text
        Dim success As Integer = 0

        If uname <> "" And pass <> "" Then

            ' Check Valid Uname and Password

            Dim query As String = "select * from login where uname = '" & uname & "' and password = '"& pass & "'"

            Dim cmd As New SqlCommand(query, DB.connection)

            Dim ds As New DataSet
            Dim da As New SqlDataAdapter(cmd)

            da.Fill(ds, "login")

            If ds.Tables("login").Rows.Count = 1 Then
                'update state

                cmd = New SqlCommand("update login set state=1 where uname = '" + uname + "' ", DB.connection)
                DB.connection.Open()
                cmd.ExecuteNonQuery()
                DB.connection.Close()

                ' redirect
                Form4.Show()
                Me.Hide()
            Else
                MsgBox("Try Again !!!")
            End If
        Else
            MsgBox("Enter the value !!!")
        End If

    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        Me.Close()
    End Sub

    Protected Overrides Sub SetVisibleCore(value As Boolean)
        Dim cmd As New SqlCommand("select * from login where state = 1", DB.connection)
        Dim ds As New DataSet
        Dim da As New SqlDataAdapter(cmd)

        da.Fill(ds, "login")
        If ds.Tables("login").Rows.Count = 1 Then
            If Not Me.IsHandleCreated Then
                Me.CreateHandle()
                value = False
            End If
            Form4.Show()
        End If
        MyBase.SetVisibleCore(value)
    End Sub

End Class
