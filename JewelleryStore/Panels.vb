Module Panels


    Sub show(ByVal pan As String)

        ' hide all panels
        Form4.homepanel.Hide()
        Form4.purchasepanel.Hide()
        Form4.productpanel.Hide()
        Form4.salespanel.Hide()
        Form4.report_panel.Hide()
        Form4.find_bill_panel.Hide()
        Form4.mis_report_panel.Hide()

        If pan = "home" Then
            Form4.homepanel.Show()
        End If

        If pan = "purchase" Then
            Form4.purchasepanel.Show()
        End If

        If pan = "product" Then
            Form4.productpanel.Show()
        End If

        If pan = "sales" Then
            Form4.salespanel.Show()
        End If

        If pan = "report" Then
            Form4.report_panel.Show()
        End If

        If pan = "find_bill_panel" Then
            Form4.find_bill_panel.Show()
        End If

        If pan = "mis_report" Then
            Form4.mis_report_panel.Show()
        End If

    End Sub


End Module
