Public Class Form1
    Dim speed As Integer
    Dim road(13) As PictureBox
    Dim jarakRoad As Integer = Integer.MaxValue
    Dim score As Integer = 0
    Dim isPaused As Boolean = False
    Dim isGameOver As Boolean = False
    Dim carStartLeft As Integer
    Dim baseSpeed As Integer = 3
    Dim isForward As Boolean = False
    Dim isBackward As Boolean = False
    Dim carStartTop As Integer
    Dim buffActive As Boolean = False
    Dim buffTicks As Integer = 0
    Dim buffRespawn As Integer = 600
    Dim x2Respawn As Integer = 400
    Const jedaX2 As Integer = 700
    Const durasiX2 As Integer = 300
    Dim p2Left As Boolean = False
    Dim p2Right As Boolean = False
    Dim p2Fwd As Boolean = False
    Dim p2Back As Boolean = False
    Dim car2StartLeft As Integer
    Dim car2StartTop As Integer
    Dim isMulti As Boolean = False
    Dim sisiKiri(2) As PictureBox
    Dim sisiKanan(2) As PictureBox
    Dim p1Alive As Boolean = True
    Dim p2Alive As Boolean = True
    Dim buff2Active As Boolean = False
    Dim buff2Ticks As Integer = 0
    Dim hit1 As Integer = 0
    Dim hit2 As Integer = 0
    Dim p1Gone As Boolean = False
    Dim p2Gone As Boolean = False
    Dim terakhirKalah As Integer = 0
    Dim score2 As Integer = 0
    Dim x2Sisa(1) As Integer
    Dim jarakTempuh As Double = 0
    Const skalaJarak As Double = 10
    Dim highScore As Integer = 0
    Dim highJarak As Integer = 0
    Dim notifTicks As Integer = 0
    Dim fileData As String = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "MobilRacing_data.txt")
    Dim namaCapai() As String = {"Pemula: skor 10", "Pembalap: skor 50", "Legenda: skor 100", "Penjelajah: 500 m", "Kebal: ambil perisai", "Serakah: ambil x2 poin"}
    Dim capai(5) As Boolean

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Randomize()
        Me.KeyPreview = True
        speed = 3
        carStartLeft = Car.Left
        carStartTop = Car.Top
        car2StartLeft = Car2.Left
        car2StartTop = Car2.Top

        PanelMainMenu.Visible = True
        PanelMainMenu.BackColor = Color.FromArgb(50, 0, 0, 0)
        PauseButton.Visible = False

        Car.Visible = False
        Car2.Visible = False

        For i As Integer = 0 To road.Length - 1
            road(i) = CType(Me.Controls("PictureBox" & (i + 1)), PictureBox)
        Next

        Skor.Text = "0"
        Kec.Text = speed

        For i As Integer = 0 To road.Length - 1
            For j As Integer = 0 To road.Length - 1
                If i <> j AndAlso Math.Abs(road(i).Left - road(j).Left) <= 5 Then
                    Dim d As Integer = Math.Abs(road(i).Top - road(j).Top)
                    If d > 0 AndAlso d < jarakRoad Then jarakRoad = d
                End If
            Next
        Next

        For i As Integer = 0 To 2
            If i = 0 Then
                sisiKiri(i) = kiria
                sisiKanan(i) = kanana
            Else
                sisiKiri(i) = BuatSalinan(kiria)
                sisiKanan(i) = BuatSalinan(kanana)
            End If
            sisiKiri(i).Top = (i - 1) * kiria.Height
            sisiKanan(i).Top = (i - 1) * kanana.Height
        Next

        MuatData()
        TampilRekor()
        Skor2.Visible = False
        LabelCapaian.Visible = False
        LabelJarak.Text = "0 m"
    End Sub

    Private Sub MulaiGame(multi As Boolean)
        isMulti = multi
        PanelMainMenu.Visible = False
        PauseButton.Visible = True
        Car.Visible = True
        Car2.Visible = multi
        Skor2.Visible = multi

        RoadMover.Start()
        MobilK.Start()
        MobilB.Start()
        MobilH.Start()
        Me.ActiveControl = Nothing
    End Sub

    Private Sub ButtonPlay_Click(sender As Object, e As EventArgs) Handles ButtonPlay.Click
        MulaiGame(False)
    End Sub

    Private Sub ButtonMulti_Click(sender As Object, e As EventArgs) Handles ButtonMulti.Click
        MulaiGame(True)
    End Sub

    Private Function BuatSalinan(asli As PictureBox) As PictureBox
        Dim p As New PictureBox()
        p.Image = asli.Image
        p.BackgroundImage = asli.BackgroundImage
        p.BackgroundImageLayout = asli.BackgroundImageLayout
        p.SizeMode = asli.SizeMode
        p.Size = asli.Size
        p.Location = asli.Location
        p.BackColor = asli.BackColor

        asli.Parent.Controls.Add(p)
        asli.Parent.Controls.SetChildIndex(p, asli.Parent.Controls.GetChildIndex(asli))
        Return p
    End Function

    Private Sub GerakSisi(tile() As PictureBox)
        For i As Integer = 0 To tile.Length - 1
            tile(i).Top += speed
        Next

        For i As Integer = 0 To tile.Length - 1
            If tile(i).Top > Me.ClientSize.Height Then
                Dim paling As Integer = tile(i).Top
                For k As Integer = 0 To tile.Length - 1
                    If tile(k).Top < paling Then paling = tile(k).Top
                Next
                tile(i).Top = paling - tile(i).Height
            End If
        Next
    End Sub
    Private Sub RoadMover_Tick(sender As Object, e As EventArgs) Handles RoadMover.Tick
        For x As Integer = 0 To road.Length - 1
            road(x).Top += speed
        Next

        For x As Integer = 0 To road.Length - 1
            If road(x).Top > Me.ClientSize.Height Then
                Dim paling As Integer = road(x).Top
                For k As Integer = 0 To road.Length - 1
                    If Math.Abs(road(k).Left - road(x).Left) <= 5 AndAlso road(k).Top < paling Then
                        paling = road(k).Top
                    End If
                Next

                If jarakRoad = Integer.MaxValue Then
                    road(x).Top = -road(x).Height
                Else
                    road(x).Top = paling - jarakRoad
                    If road(x).Top > 0 Then road(x).Top = -road(x).Height
                End If
            End If
        Next

        GerakSisi(sisiKiri)
        GerakSisi(sisiKanan)

        Dim skorMax As Integer = Math.Max(score, score2) ' DIUBAH (dari sini sampai End If di bawah)
        If skorMax >= 100 Then
            baseSpeed = 9
        ElseIf skorMax >= 70 Then
            baseSpeed = 8
        ElseIf skorMax >= 50 Then
            baseSpeed = 7
        ElseIf skorMax >= 30 Then
            baseSpeed = 6
        ElseIf skorMax >= 10 Then
            baseSpeed = 5
        Else
            baseSpeed = 3
        End If

        If isForward OrElse p2Fwd Then
            speed = baseSpeed + 3
        ElseIf isBackward OrElse p2Back Then
            speed = Math.Max(1, baseSpeed - 2)
        Else
            speed = baseSpeed
        End If

        Kec.Text = speed

        If p1Alive OrElse p2Alive Then
            jarakTempuh += speed / skalaJarak
            LabelJarak.Text = CInt(jarakTempuh).ToString() & " m"
        End If

        CekCapaian()

        If notifTicks > 0 Then
            notifTicks -= 1
            If notifTicks = 0 Then LabelCapaian.Visible = False
        End If

        If isForward Then Car.Top = Math.Max(100, Car.Top - 4)
        If isBackward Then Car.Top = Math.Min(Me.ClientSize.Height - Car.Height, Car.Top + 4)

        If isMulti Then
            If p2Left AndAlso Car2.Left > 60 Then Car2.Left -= 5
            If p2Right AndAlso Car2.Left < 270 Then Car2.Left += 5
            If p2Fwd Then Car2.Top = Math.Max(100, Car2.Top - 4)
            If p2Back Then Car2.Top = Math.Min(Me.ClientSize.Height - Car2.Height, Car2.Top + 4)
        End If

        If p1Alive AndAlso Not buffActive Then
            Dim t1 As Integer = CariTabrakan(Car)
            If t1 > 0 Then MobilKalah(1, t1)
        End If

        If isMulti AndAlso p2Alive AndAlso Not buff2Active Then
            Dim t2 As Integer = CariTabrakan(Car2)
            If t2 > 0 Then MobilKalah(2, t2)
        End If

        If hit1 = 1 OrElse hit2 = 1 Then CarK.Top += speed
        If hit1 = 2 OrElse hit2 = 2 Then CarB.Top += speed
        If hit1 = 3 OrElse hit2 = 3 Then CarH.Top += speed

        If Not p1Alive AndAlso Not p1Gone Then
            Car.Top += speed
            If Car.Top > Me.ClientSize.Height Then
                Car.Visible = False
                p1Gone = True
            End If
        End If
        If isMulti AndAlso Not p2Alive AndAlso Not p2Gone Then
            Car2.Top += speed
            If Car2.Top > Me.ClientSize.Height Then
                Car2.Visible = False
                p2Gone = True
            End If
        End If

        CekAkhirGame()
        If isGameOver Then Return

        If buff2Active Then
            buff2Ticks -= 1
            If buff2Ticks <= 0 Then
                buff2Active = False
                Car2.BackColor = Color.Transparent
            ElseIf buff2Ticks <= 75 Then
                If (buff2Ticks \ 8) Mod 2 = 0 Then
                    Car2.BackColor = Color.Cyan
                Else
                    Car2.BackColor = Color.Transparent
                End If
            End If
        End If

        If isForward Then
            Kec.ForeColor = Color.LimeGreen
        ElseIf isBackward Then
            Kec.ForeColor = Color.Red
        Else
            Kec.ForeColor = Color.Black
        End If

        If Perisai.Visible Then
            Perisai.Top += speed
            If p1Alive AndAlso Car.Bounds.IntersectsWith(Perisai.Bounds) Then
                Perisai.Visible = False
                Buka(4, True)
                buffActive = True
                buffTicks = 500
                buffRespawn = 600
                Car.BackColor = Color.Cyan
            ElseIf isMulti AndAlso p2Alive AndAlso Car2.Bounds.IntersectsWith(Perisai.Bounds) Then
                Perisai.Visible = False
                Buka(4, True)
                buff2Active = True
                buff2Ticks = 500
                buffRespawn = 600
                Car2.BackColor = Color.Cyan
            ElseIf Perisai.Top > Me.ClientSize.Height Then
                Perisai.Visible = False
                buffRespawn = 600
            End If
        Else
            buffRespawn -= 1
            If buffRespawn <= 0 Then
                Perisai.Top = -Perisai.Height
                Perisai.Left = CInt(Math.Ceiling(Rnd() * 210)) + 60
                Perisai.Visible = True
            End If
        End If

        If buffActive Then
            buffTicks -= 1
            If buffTicks <= 0 Then
                buffActive = False
                Car.BackColor = Color.Transparent
            ElseIf buffTicks <= 75 Then
                If (buffTicks \ 8) Mod 2 = 0 Then
                    Car.BackColor = Color.Cyan
                Else
                    Car.BackColor = Color.Transparent
                End If
            End If
        End If

        If kali2.Visible Then ' DIUBAH (blok kali2 sampai Next di bawah)
            kali2.Top += speed
            If p1Alive AndAlso Car.Bounds.IntersectsWith(kali2.Bounds) Then
                AmbilX2(0)
            ElseIf isMulti AndAlso p2Alive AndAlso Car2.Bounds.IntersectsWith(kali2.Bounds) Then
                AmbilX2(1)
            ElseIf kali2.Top > Me.ClientSize.Height Then
                kali2.Visible = False
                x2Respawn = jedaX2 + CInt(Math.Ceiling(Rnd() * 300))
            End If
        Else
            x2Respawn -= 1
            If x2Respawn <= 0 Then
                kali2.Top = -kali2.Height
                kali2.Left = CInt(Math.Ceiling(Rnd() * 210)) + 60
                kali2.Visible = True
            End If
        End If

        For i As Integer = 0 To 1
            If x2Sisa(i) > 0 Then
                x2Sisa(i) -= 1
                If x2Sisa(i) = 0 Then AturWarnaSkor()
            End If
        Next
    End Sub

    Private Sub endgame()
        isGameOver = True
        ReplayButton.Visible = True
        GameOver.Visible = True
        RoadMover.Stop()
        MobilK.Stop()
        MobilB.Stop()
        MobilH.Stop()
        LeftSide.Stop()
        RightSide.Stop()
        ButtonKeMenu.Visible = True
        CekRekor()
    End Sub

    Protected Overrides Function ProcessCmdKey(ByRef msg As Message, keyData As Keys) As Boolean
        If PanelMainMenu.Visible OrElse isPaused OrElse isGameOver OrElse Not p1Alive Then
            Return MyBase.ProcessCmdKey(msg, keyData)
        End If

        If keyData = Keys.Left Then
            LeftSide.Start()
            Return True
        ElseIf keyData = Keys.Right Then
            RightSide.Start()
            Return True
        ElseIf keyData = Keys.Up Then
            isForward = True
            Return True
        ElseIf keyData = Keys.Down Then
            isBackward = True
            Return True
        End If

        Return MyBase.ProcessCmdKey(msg, keyData)
    End Function

    Private Sub Form1_KeyUp(sender As Object, e As KeyEventArgs) Handles MyBase.KeyUp
        If e.KeyCode = Keys.Left Then LeftSide.Stop()
        If e.KeyCode = Keys.Right Then RightSide.Stop()
        If e.KeyCode = Keys.Up Then isForward = False
        If e.KeyCode = Keys.Down Then isBackward = False
        If e.KeyCode = Keys.A Then p2Left = False
        If e.KeyCode = Keys.D Then p2Right = False
        If e.KeyCode = Keys.W Then p2Fwd = False
        If e.KeyCode = Keys.S Then p2Back = False
    End Sub

    Private Sub Form1_KeyDown(sender As Object, e As KeyEventArgs) Handles MyBase.KeyDown
        If Not isMulti OrElse Not p2Alive OrElse PanelMainMenu.Visible OrElse isPaused OrElse isGameOver Then Return
        If e.KeyCode = Keys.A Then p2Left = True
        If e.KeyCode = Keys.D Then p2Right = True
        If e.KeyCode = Keys.W Then p2Fwd = True
        If e.KeyCode = Keys.S Then p2Back = True
    End Sub

    Private Sub LeftSide_Tick(sender As Object, e As EventArgs) Handles LeftSide.Tick
        If Car.Location.X > 60 Then
            Car.Left -= 5
        End If
    End Sub

    Private Sub RightSide_Tick(sender As Object, e As EventArgs) Handles RightSide.Tick
        If Car.Location.X < 270 Then
            Car.Left += 5
        End If
    End Sub

    Private Sub MobilK_Tick(sender As Object, e As EventArgs) Handles MobilK.Tick
        If Not (hit1 = 1 OrElse hit2 = 1) Then
            CarK.Top += Math.Max(1, CInt(speed / 2))
        End If

        If CarK.Top >= Me.ClientSize.Height Then
            TambahSkor() ' DIUBAH

            CarK.Top = -(CInt(Math.Ceiling(Rnd() * 200)) + CarK.Height)
            CarK.Left = CInt(Math.Ceiling(Rnd() * 20)) + 60

            If hit1 = 1 Then hit1 = 0
            If hit2 = 1 Then hit2 = 0
        End If
    End Sub

    Private Sub MobilB_Tick(sender As Object, e As EventArgs) Handles MobilB.Tick
        If Not (hit1 = 2 OrElse hit2 = 2) Then
            CarB.Top += Math.Max(1, CInt(speed / 3))
        End If

        If CarB.Top >= Me.ClientSize.Height Then
            TambahSkor() ' DIUBAH

            CarB.Top = -(CInt(Math.Ceiling(Rnd() * 200)) + CarB.Height)
            CarB.Left = CInt(Math.Ceiling(Rnd() * 50)) + 140

            If hit1 = 2 Then hit1 = 0
            If hit2 = 2 Then hit2 = 0
        End If
    End Sub

    Private Sub MobilH_Tick(sender As Object, e As EventArgs) Handles MobilH.Tick
        If Not (hit1 = 3 OrElse hit2 = 3) Then
            CarH.Top += Math.Max(1, CInt(speed / 4 * 3))
        End If

        If CarH.Top >= Me.ClientSize.Height Then
            TambahSkor() ' DIUBAH

            CarH.Top = -(CInt(Math.Ceiling(Rnd() * 200)) + CarH.Height)
            CarH.Left = CInt(Math.Ceiling(Rnd() * 50)) + 230

            If hit1 = 3 Then hit1 = 0
            If hit2 = 3 Then hit2 = 0
        End If
    End Sub

    Private Sub MobilKalah(pemain As Integer, musuh As Integer)
        If pemain = 1 Then
            p1Alive = False
            hit1 = musuh
            isForward = False
            isBackward = False
            LeftSide.Stop()
            RightSide.Stop()
        Else
            p2Alive = False
            hit2 = musuh
            p2Left = False
            p2Right = False
            p2Fwd = False
            p2Back = False
        End If
        terakhirKalah = pemain
    End Sub

    Private Function CariTabrakan(mobil As PictureBox) As Integer
        If mobil.Bounds.IntersectsWith(CarK.Bounds) Then Return 1
        If mobil.Bounds.IntersectsWith(CarB.Bounds) Then Return 2
        If mobil.Bounds.IntersectsWith(CarH.Bounds) Then Return 3
        Return 0
    End Function

    Private Sub CekAkhirGame()
        Dim selesai1 As Boolean = (Not p1Alive) AndAlso p1Gone
        Dim selesai2 As Boolean = (Not isMulti) OrElse ((Not p2Alive) AndAlso p2Gone)
        If selesai1 AndAlso selesai2 Then
            If isMulti Then
                If score > score2 Then ' DIUBAH (pemenang dari skor)
                    GameOver.Text = "P1 MENANG"
                ElseIf score2 > score Then
                    GameOver.Text = "P2 MENANG"
                Else
                    GameOver.Text = "SERI"
                End If
            Else
                GameOver.Text = "GAME OVER"
            End If
            endgame()
        End If
    End Sub

    Private Sub PauseButton_Click(sender As Object, e As EventArgs) Handles PauseButton.Click
        If isGameOver Then Return
        isPaused = Not isPaused
        RoadMover.Enabled = Not isPaused
        MobilK.Enabled = Not isPaused
        MobilB.Enabled = Not isPaused
        MobilH.Enabled = Not isPaused

        If isPaused Then
            LeftSide.Stop()
            RightSide.Stop()
        End If

        PauseButton.Text = If(isPaused, "▷", "II")

        ButtonResume.Visible = isPaused
        ReplayButton.Visible = isPaused
        ButtonKeMenu.Visible = isPaused
        If isPaused Then
            ButtonResume.BringToFront()
            ReplayButton.BringToFront()
            ButtonKeMenu.BringToFront()
        End If
        Me.ActiveControl = Nothing
    End Sub

    Private Sub ButtonResume_Click(sender As Object, e As EventArgs) Handles ButtonResume.Click
        PauseButton_Click(sender, e)
    End Sub

    Private Sub ReplayButton_Click(sender As Object, e As EventArgs) Handles ReplayButton.Click
        CekRekor()
        score = 0
        speed = 3
        isGameOver = False
        isPaused = False
        baseSpeed = 3
        isForward = False
        isBackward = False
        Car.Top = carStartTop

        Skor.Text = "0"
        Kec.Text = speed
        GameOver.Visible = False
        ReplayButton.Visible = False
        ButtonKeMenu.Visible = False

        Car.Left = carStartLeft
        CarK.Top = -CarK.Height
        CarB.Top = -CarB.Height
        CarH.Top = -CarH.Height

        RoadMover.Start()
        MobilK.Start()
        MobilB.Start()
        MobilH.Start()

        buffActive = False
        buffTicks = 0
        buffRespawn = 600
        Perisai.Visible = False
        Car.BackColor = Color.Transparent

        x2Sisa(0) = 0 ' DIUBAH (dari sini sampai AturWarnaSkor)
        x2Sisa(1) = 0
        x2Respawn = 400
        kali2.Visible = False
        AturWarnaSkor()

        p2Left = False
        p2Right = False
        p2Fwd = False
        p2Back = False
        Car2.Left = car2StartLeft
        Car2.Top = car2StartTop

        p1Alive = True
        p2Alive = True
        Car.Visible = True
        Car2.Visible = isMulti
        buff2Active = False
        buff2Ticks = 0
        Car2.BackColor = Color.Transparent
        GameOver.Text = "GAME OVER"

        hit1 = 0
        hit2 = 0
        p1Gone = False
        p2Gone = False
        terakhirKalah = 0

        score2 = 0
        Skor2.Text = "0"
        Skor2.Visible = isMulti
        jarakTempuh = 0
        LabelJarak.Text = "0 m"
        notifTicks = 0
        LabelCapaian.Visible = False
    End Sub

    Private Sub ButtonKeMenu_Click(sender As Object, e As EventArgs) Handles ButtonKeMenu.Click
        ReplayButton_Click(sender, e)

        RoadMover.Stop()
        MobilK.Stop()
        MobilB.Stop()
        MobilH.Stop()

        Car.Visible = False
        Car2.Visible = False
        PauseButton.Visible = False
        PanelMainMenu.Visible = True
        PanelMainMenu.BringToFront()
    End Sub

    ' ===== SUB BARU (semua di bawah ini) =====

    Private Sub TambahSkor()
        If p1Alive Then score += If(x2Sisa(0) > 0, 2, 1)
        If isMulti AndAlso p2Alive Then score2 += If(x2Sisa(1) > 0, 2, 1)
        Skor.Text = score.ToString()
        If isMulti Then Skor2.Text = score2.ToString()
    End Sub

    Private Sub AmbilX2(idx As Integer)
        kali2.Visible = False
        x2Sisa(idx) = durasiX2
        x2Respawn = jedaX2 + CInt(Math.Ceiling(Rnd() * 300))
        Buka(5, True)
        AturWarnaSkor()
    End Sub

    Private Sub AturWarnaSkor()
        Skor.ForeColor = If(x2Sisa(0) > 0, Color.Gold, Color.Black)
        Skor2.ForeColor = If(x2Sisa(1) > 0, Color.Gold, Color.Black)
    End Sub

    Private Sub MuatData()
        Try
            If System.IO.File.Exists(fileData) Then
                Dim baris() As String = System.IO.File.ReadAllLines(fileData)
                If baris.Length > 0 Then Integer.TryParse(baris(0), highScore)
                If baris.Length > 1 Then Integer.TryParse(baris(1), highJarak)
                If baris.Length > 2 Then
                    For i As Integer = 0 To Math.Min(baris(2).Length, capai.Length) - 1
                        capai(i) = (baris(2)(i) = "1"c)
                    Next
                End If
            End If
        Catch
        End Try
    End Sub

    Private Sub SimpanData()
        Try
            Dim bit As String = ""
            For i As Integer = 0 To capai.Length - 1
                bit &= If(capai(i), "1", "0")
            Next
            System.IO.File.WriteAllLines(fileData, New String() {highScore.ToString(), highJarak.ToString(), bit})
        Catch
        End Try
    End Sub

    Private Sub TampilRekor()
        LabelHigh.Text = highScore
        LabelHighJ.Text = highJarak & " m"
    End Sub

    Private Sub CekRekor()
        Dim terbaik As Integer = Math.Max(score, score2)
        If terbaik > highScore Then highScore = terbaik
        If CInt(jarakTempuh) > highJarak Then highJarak = CInt(jarakTempuh)
        SimpanData()
        TampilRekor()
    End Sub

    Private Sub Buka(i As Integer, syarat As Boolean)
        If syarat AndAlso Not capai(i) Then
            capai(i) = True
            TampilNotif(namaCapai(i))
            SimpanData()
        End If
    End Sub

    Private Sub CekCapaian()
        Dim terbaik As Integer = Math.Max(score, score2)
        Buka(0, terbaik >= 10)
        Buka(1, terbaik >= 50)
        Buka(2, terbaik >= 100)
        Buka(3, jarakTempuh >= 500)
    End Sub

    Private Sub TampilNotif(teks As String)
        LabelCapaian.Text = "PENCAPAIAN: " & teks
        LabelCapaian.Visible = True
        LabelCapaian.BringToFront()
        notifTicks = 150
    End Sub

    Private Sub ButtonCapaian_Click(sender As Object, e As EventArgs) Handles ButtonCapaian.Click
        Dim teks As String = ""
        For i As Integer = 0 To capai.Length - 1
            teks &= If(capai(i), "[x] ", "[ ] ") & namaCapai(i) & vbCrLf
        Next
        MessageBox.Show(teks, "Pencapaian")
        Me.ActiveControl = Nothing
    End Sub
End Class