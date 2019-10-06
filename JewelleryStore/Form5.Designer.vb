<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class Form5
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.select_product_grid = New System.Windows.Forms.DataGridView()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.find_product_id = New System.Windows.Forms.TextBox()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.find_weight = New System.Windows.Forms.TextBox()
        Me.product_combo = New System.Windows.Forms.ComboBox()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.find_sales_button = New System.Windows.Forms.Button()
        CType(Me.select_product_grid, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Font = New System.Drawing.Font("Microsoft Sans Serif", 20.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label1.Location = New System.Drawing.Point(251, 23)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(259, 31)
        Me.Label1.TabIndex = 0
        Me.Label1.Text = "Select Item For Sale"
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label2.Location = New System.Drawing.Point(578, 160)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(198, 15)
        Me.Label2.TabIndex = 1
        Me.Label2.Text = "Double Click On Product For Select"
        '
        'select_product_grid
        '
        Me.select_product_grid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.select_product_grid.Location = New System.Drawing.Point(12, 178)
        Me.select_product_grid.Name = "select_product_grid"
        Me.select_product_grid.Size = New System.Drawing.Size(764, 311)
        Me.select_product_grid.TabIndex = 2
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Location = New System.Drawing.Point(17, 120)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(64, 13)
        Me.Label3.TabIndex = 3
        Me.Label3.Text = "Product_id :"
        '
        'find_product_id
        '
        Me.find_product_id.Location = New System.Drawing.Point(90, 117)
        Me.find_product_id.Name = "find_product_id"
        Me.find_product_id.Size = New System.Drawing.Size(100, 20)
        Me.find_product_id.TabIndex = 4
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.Location = New System.Drawing.Point(470, 116)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(44, 13)
        Me.Label4.TabIndex = 5
        Me.Label4.Text = "weight :"
        '
        'find_weight
        '
        Me.find_weight.Location = New System.Drawing.Point(534, 116)
        Me.find_weight.Name = "find_weight"
        Me.find_weight.Size = New System.Drawing.Size(100, 20)
        Me.find_weight.TabIndex = 6
        '
        'product_combo
        '
        Me.product_combo.FormattingEnabled = True
        Me.product_combo.Location = New System.Drawing.Point(317, 116)
        Me.product_combo.Name = "product_combo"
        Me.product_combo.Size = New System.Drawing.Size(121, 21)
        Me.product_combo.TabIndex = 7
        '
        'Label5
        '
        Me.Label5.AutoSize = True
        Me.Label5.Location = New System.Drawing.Point(220, 119)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(81, 13)
        Me.Label5.TabIndex = 8
        Me.Label5.Text = "Product Name :"
        '
        'find_sales_button
        '
        Me.find_sales_button.Location = New System.Drawing.Point(680, 113)
        Me.find_sales_button.Name = "find_sales_button"
        Me.find_sales_button.Size = New System.Drawing.Size(75, 23)
        Me.find_sales_button.TabIndex = 9
        Me.find_sales_button.Text = "search"
        Me.find_sales_button.UseVisualStyleBackColor = True
        '
        'Form5
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(788, 501)
        Me.Controls.Add(Me.find_sales_button)
        Me.Controls.Add(Me.Label5)
        Me.Controls.Add(Me.product_combo)
        Me.Controls.Add(Me.find_weight)
        Me.Controls.Add(Me.Label4)
        Me.Controls.Add(Me.find_product_id)
        Me.Controls.Add(Me.Label3)
        Me.Controls.Add(Me.select_product_grid)
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.Label1)
        Me.Name = "Form5"
        Me.Text = "Form5"
        CType(Me.select_product_grid, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents select_product_grid As System.Windows.Forms.DataGridView
    Friend WithEvents Label3 As System.Windows.Forms.Label
    Friend WithEvents find_product_id As System.Windows.Forms.TextBox
    Friend WithEvents Label4 As System.Windows.Forms.Label
    Friend WithEvents find_weight As System.Windows.Forms.TextBox
    Friend WithEvents product_combo As System.Windows.Forms.ComboBox
    Friend WithEvents Label5 As System.Windows.Forms.Label
    Friend WithEvents find_sales_button As System.Windows.Forms.Button
End Class
