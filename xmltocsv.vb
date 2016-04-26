#Region "Imports"
Imports System.Xml
Imports System.IO
Imports System.Xml.Xsl
Imports System.Xml.XPath
Imports Microsoft.VisualBasic
#End Region





'Hello World - XML to CSV converter, used for converting the output xml file from Nefmoto flasher/logger logging program, into a CSV file, which is 
'readable to EcuxPlot. Only for educational and hobby use! - By Nefmoto user ZaoxDK! 
'Project start: 22-04-2016
'Projext end: 



'**********************Project Notes*****************************
'File Called work.xml is the file, which we use to decrypt and read data from, so that the original xml file is untouched, as it simply gets copied into work.xml.
'Therefore we also need a work_done.xml file, which will be the file, that we write the decrypted data to. This is then the file, that gets converted into CSV.

'***************WORKAROUND***************************

'1. Load user selected file via OFD
'2. User selects destination file via SFD
'3. User presses convert - code @Button_3 initiates
'1a. Copy original xml to work.xml
'2a. Read from encrypted work.xml **NOT DONE**
'3a. Decrypt the RawData from nefmoto log xml**NOT DONE**
'4a. Either overwrite decrypted data or write to new xml called work_done.xml (if write to new) **NOT DONE**
'5a. Convert work.xml or work_done.xml to CSV using correct xsl stylesheet. **NOT DONE**
'6a. Delete all work.xml files...
'----------End of code----------
'****************************************************

'*************RESOURCES*********************
' Some of the code used, has been copied directly from Microsoft (https://msdn.microsoft.com)
'
'
'
'
'
'*******************Updates**************************
' By 25-04-2016: Loading and saving files now works, also converting to CSV using a XSL stylesheet, which is defined in the code
' TODO: Alot... Mainly to get the decode function working, but also to create a stylesheet which matches the layout of the sample log.csv file





Public Class xmltocsv

    Private Sub Form1_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Label6.Visible = False
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        If (OFD.ShowDialog() = DialogResult.OK) Then
            TextBox1.Text = System.IO.Path.GetFileName(OFD.FileName)
        End If



    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        If (SFD.ShowDialog() = DialogResult.OK) Then
            TextBox2.Text = System.IO.Path.GetFileName(SFD.FileName)
        End If


    End Sub



    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click

        If TextBox1.Text = "" Then
            MsgBox("Please select a log file..", MsgBoxStyle.Critical, "Error")
            GoTo Over


        End If

        If TextBox2.Text = "" Then
            MsgBox("Please save an output file..", MsgBoxStyle.Critical, "Error")
            GoTo Over
        End If

        MsgBox("Press OK to convert", MsgBoxStyle.Information, "Conversion")

        'Converting label turns on
        Label6.Visible = True

        'check If file Is existing
        If (IO.File.Exists(OFD.FileName)) Then

            'Copy the selected file to the work.xml file
            My.Computer.FileSystem.CopyFile(
            OFD.FileName,
            "work.xml",
            Microsoft.VisualBasic.FileIO.UIOption.AllDialogs,
            Microsoft.VisualBasic.FileIO.UICancelOption.DoNothing)


            '***************Data Fetch + delete Section*************************************************



            Dim doc As XmlDocument = New XmlDocument()
            doc.Load("work.xml")

            'Remove all attributes of the description node - this fetches all nodes called by the end tag */(endtag) in this case: "description"
            Dim nodeList As XmlNodeList
            Dim root As XmlElement = doc.DocumentElement
            nodeList = root.SelectNodes("/LogFile/VariableDefinitions/MemoryVariableDefinition/Description")
            ' Here the function does something, this can be altered to ex. convert base64 data.
            Dim Description As XmlNode
            For Each Description In nodeList
                Description.RemoveAll()
            Next


            Dim nodeList2 As XmlNodeList
            Dim root2 As XmlElement = doc.DocumentElement
            nodeList2 = root.SelectNodes("/LogFile/VariableDefinitions/MemoryVariableDefinition/ScaleOffsetMemoryVariableValueConverter")
            Dim DSOMVVC As XmlNode
            For Each DSOMVVC In nodeList2
                DSOMVVC.RemoveAll()
            Next

            Dim nodeList3 As XmlNodeList
            Dim root3 As XmlElement = doc.DocumentElement
            nodeList3 = root.SelectNodes("/LogFile/VariableDefinitions/MemoryVariableDefinition/Address")
            Dim address As XmlNode
            For Each address In nodeList3
                address.RemoveAll()
            Next

            Dim nodeList4 As XmlNodeList
            Dim root4 As XmlElement = doc.DocumentElement
            nodeList4 = root.SelectNodes("/LogFile/VariableDefinitions/MemoryVariableDefinition/DataType")
            Dim DT As XmlNode
            For Each DT In nodeList4
                DT.RemoveAll()
            Next











            'Save the modified xml
            doc.Save("work_mod.xml")










            '*****************************Decryption section*********************************************



            ' Initialize an XslCompiledTransform instance.
            Dim transform As New XslCompiledTransform()

            'Load the Stylesheet ***Place correct Stylesheet***
            transform.Load("Books.xslt")

            'Transform the loaded xml to csv using the specified stylesheet
            transform.Transform("work_mod.xml", SFD.FileName)



        End If
        If (IO.File.Exists(SFD.FileName)) Then
            'This is only noted for developer reasons
            My.Computer.FileSystem.DeleteFile("work.xml")
            'Converting label turns off
            Label6.Visible = False
            GoTo Finished

        End If

EarlyExit:

        MsgBox("Something went wrong, please try again And make sure that selected file Is a Nefmoto Logfile..", MsgBoxStyle.Critical, "Error")
        GoTo Over

Finished:
        MsgBox("File correctly converted. Find it at specified location 
        Thanks for using this software - Enjoy", MsgBoxStyle.Information, "Succes!")
        GoTo Over
Over:
    End Sub

    Private Sub Button4_Click(sender As Object, e As EventArgs) Handles Button4.Click
        Controls.Clear()
        InitializeComponent()
    End Sub
End Class
