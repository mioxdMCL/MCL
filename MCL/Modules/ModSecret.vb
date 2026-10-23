'由于包含加解密等安全信息，本文件中的部分代码已被删除

Friend Module ModSecret

    '标注 PCL 的不同分支，仅用于替换标记
    Public Const VersionBranchMain As String = "OpenSource"
    '在开源版的注册表与常规版的注册表隔离，以防数据冲突
    Public Const RegFolder As String = "PCLDebug"
    '用于微软登录的 ClientId（已从源码中移除以防泄露，如需正版登录请自行注册 Azure 应用并填写）
    Public OAuthClientId As String = ""
    'CurseForge API Key（本改版不携带任何官方密钥）
    Public CurseForgeAPIKey As String = ""
    '用于匿名数据收集的腾讯云日志服务上报 URL，形如 https://{region}.cls.tencentcs.com/track?topic_id={topic_id}
    Public Const ClsBaseUrl As String = ""

#Region "网络鉴权"

    Public Const RestrictMcDownload As Boolean = True
    Friend Function SecretCdnSign(UrlWithMark As String) As String
        If Not UrlWithMark.EndsWithF("{CDN}") Then Return UrlWithMark
        Return UrlWithMark.Replace("{CDN}", "").Replace(" ", "%20")
    End Function
    ''' <summary>
    ''' 设置 Headers 的 UA、Referer。
    ''' </summary>
    Friend Sub SecretHeadersSign(Url As String, ByRef Req As HttpRequestMessage, Optional SimulateBrowserHeaders As Boolean = False)
        If Not Req.Headers.UserAgent.Any Then
            If Url.Contains("baidupcs.com") OrElse Url.Contains("baidu.com") Then
                Req.Headers.Add("User-Agent", "LogStatistic")  '#4951
            ElseIf SimulateBrowserHeaders Then
                Req.Headers.Add("User-Agent", $"PCL2/{VersionBaseName}.{CInt(BuildType)} Mozilla/5.0 AppleWebKit/537.36 Chrome/63.0.3239.132 Safari/537.36")
            Else
                Req.Headers.Add("User-Agent", $"PCL2/{VersionBaseName}.{CInt(BuildType)}")
            End If
        End If
        If Not SimulateBrowserHeaders Then Req.Headers.Add("Referer", $"http://{VersionCode}.open.pcl2.server/")
        If Url.Contains("api.curseforge.com") OrElse Url.Contains("forgecdn.net") Then Req.Headers.Add("x-api-key", CurseForgeAPIKey)
    End Sub

#End Region

#Region "主题"

    Public Color1 As New MyColor(52, 61, 74)
    Public Color2 As New MyColor(11, 91, 203)
    Public Color3 As New MyColor(19, 112, 243)
    Public Color4 As New MyColor(72, 144, 245)
    Public Color5 As New MyColor(150, 192, 249)
    Public Color6 As New MyColor(213, 230, 253)
    Public Color7 As New MyColor(222, 236, 253)
    Public Color8 As New MyColor(234, 242, 254)
    Public ColorBg0 As New MyColor(150, 192, 249)
    Public ColorBg1 As New MyColor(190, Color7)
    Public ColorGray1 As New MyColor(64, 64, 64)
    Public ColorGray2 As New MyColor(115, 115, 115)
    Public ColorGray3 As New MyColor(140, 140, 140)
    Public ColorGray4 As New MyColor(166, 166, 166)
    Public ColorGray5 As New MyColor(204, 204, 204)
    Public ColorGray6 As New MyColor(235, 235, 235)
    Public ColorGray7 As New MyColor(240, 240, 240)
    Public ColorGray8 As New MyColor(245, 245, 245)
    Public ColorSemiTransparent As New MyColor(1, Color8)

    Public ThemeNow As Integer = -1
    Public ColorHue As Integer = 210, ColorSat As Integer = 85, ColorLightAdjust As Integer = 0, ColorHueTopbarDelta As OneOf(Of Integer, Integer()) = 0
    Public ThemeDontClick As Integer = 0

    Public Sub ThemeRefresh(Optional NewTheme As Integer = -1)
        Try
            If ThemeNow = NewTheme AndAlso NewTheme >= 0 Then Return
            If NewTheme >= 0 Then ThemeNow = NewTheme

            Color1 = New MyColor().FromHSL2(ColorHue, ColorSat * 0.2, 25 + ColorLightAdjust * 0.3)
            Color2 = New MyColor().FromHSL2(ColorHue, ColorSat, 45 + ColorLightAdjust)
            Color3 = New MyColor().FromHSL2(ColorHue, ColorSat, 55 + ColorLightAdjust)
            Color4 = New MyColor().FromHSL2(ColorHue, ColorSat, 65 + ColorLightAdjust)
            Color5 = New MyColor().FromHSL2(ColorHue, ColorSat, 80 + ColorLightAdjust * 0.4)
            Color6 = New MyColor().FromHSL2(ColorHue, ColorSat, 91 + ColorLightAdjust * 0.1)
            Color7 = New MyColor().FromHSL2(ColorHue, ColorSat, 95)
            Color8 = New MyColor().FromHSL2(ColorHue, ColorSat, 97)
            ColorBg0 = Color4 * 0.4 + Color5 * 0.4 + ColorGray4 * 0.2
            ColorBg1 = New MyColor(190, Color7)

            ColorSemiTransparent = New MyColor(1, Color8)
            Application.Current.Resources("ColorBrush1") = New SolidColorBrush(Color1)
            Application.Current.Resources("ColorBrush2") = New SolidColorBrush(Color2)
            Application.Current.Resources("ColorBrush3") = New SolidColorBrush(Color3)
            Application.Current.Resources("ColorBrush4") = New SolidColorBrush(Color4)
            Application.Current.Resources("ColorBrush5") = New SolidColorBrush(Color5)
            Application.Current.Resources("ColorBrush6") = New SolidColorBrush(Color6)
            Application.Current.Resources("ColorBrush7") = New SolidColorBrush(Color7)
            Application.Current.Resources("ColorBrush8") = New SolidColorBrush(Color8)
            Application.Current.Resources("ColorBrushBg0") = New SolidColorBrush(ColorBg0)
            Application.Current.Resources("ColorBrushBg1") = New SolidColorBrush(ColorBg1)
            Application.Current.Resources("ColorObject1") = CType(Color1, Color)
            Application.Current.Resources("ColorObject2") = CType(Color2, Color)
            Application.Current.Resources("ColorObject3") = CType(Color3, Color)
            Application.Current.Resources("ColorObject4") = CType(Color4, Color)
            Application.Current.Resources("ColorObject5") = CType(Color5, Color)
            Application.Current.Resources("ColorObject6") = CType(Color6, Color)
            Application.Current.Resources("ColorObject7") = CType(Color7, Color)
            Application.Current.Resources("ColorObject8") = CType(Color8, Color)
            Application.Current.Resources("ColorObjectBg0") = CType(ColorBg0, Color)
            Application.Current.Resources("ColorObjectBg1") = CType(ColorBg1, Color)
            ThemeRefreshMain()
        Catch ex As Exception
            Logger.Error(ex, "刷新主题颜色失败", LogBehavior.Toast)
        End Try
    End Sub
    Public Sub ThemeRefreshMain()
        RunInUi(
        Sub()
            If Not FrmMain.IsLoaded Then Return
            '顶部条背景
            Dim Brush = New LinearGradientBrush With {.EndPoint = New Point(1, 0), .StartPoint = New Point(0, 0)}
            Dim Deltas = ColorHueTopbarDelta.Switch(Function(d) New Integer() {-d, 0, d}, Function(d) d)
            Brush.GradientStops.Add(New GradientStop With {.Offset = 0, .Color = New MyColor().FromHSL2(ColorHue + Deltas(0), ColorSat, 48 + ColorLightAdjust)})
            Brush.GradientStops.Add(New GradientStop With {.Offset = 0.5, .Color = New MyColor().FromHSL2(ColorHue + Deltas(1), ColorSat, 54 + ColorLightAdjust)})
            Brush.GradientStops.Add(New GradientStop With {.Offset = 1, .Color = New MyColor().FromHSL2(ColorHue + Deltas(2), ColorSat, 48 + ColorLightAdjust)})
            FrmMain.PanTitle.Background = Brush
            FrmMain.PanTitle.Background.Freeze()
            '主页面背景
            If Settings.Get(Of Boolean)("UiBackgroundColorful") Then
                Brush = New LinearGradientBrush With {.EndPoint = New Point(0.1, 1), .StartPoint = New Point(0.9, 0)}
                Brush.GradientStops.Add(New GradientStop With {.Offset = -0.1, .Color = New MyColor().FromHSL2(ColorHue - 15, ColorSat * 0.8, 91)})
                Brush.GradientStops.Add(New GradientStop With {.Offset = 0.4, .Color = New MyColor().FromHSL2(ColorHue, ColorSat * 0.8, 91)})
                Brush.GradientStops.Add(New GradientStop With {.Offset = 1.1, .Color = New MyColor().FromHSL2(ColorHue + 15, ColorSat * 0.8, 91)})
                FrmMain.PanForm.Background = Brush
            Else
                FrmMain.PanForm.Background = New MyColor(245, 245, 245)
            End If
            FrmMain.PanForm.Background.Freeze()
        End Sub)
    End Sub
    Friend Sub ThemeCheckAll(EffectSetup As Boolean)
        If Not EffectSetup Then Return
        If FrmSetupUI Is Nothing Then Return
        With FrmSetupUI
            .RadioLauncherTheme12.IsEnabled = ThemeCheckOne(12)
            .RadioLauncherTheme6.IsEnabled = ThemeCheckOne(6)
            .RadioLauncherTheme7.IsEnabled = ThemeCheckOne(7)
            .RadioLauncherTheme13.IsEnabled = ThemeCheckOne(13)
            .RadioLauncherTheme8.IsEnabled = ThemeCheckOne(8)
            .RadioLauncherTheme9.IsEnabled = ThemeCheckOne(9)
            .RadioLauncherTheme10.IsEnabled = ThemeCheckOne(10)
            .RadioLauncherTheme11.IsEnabled = ThemeCheckOne(11)
            .RadioLauncherTheme14.IsEnabled = ThemeCheckOne(5) AndAlso ThemeCheckOne(6) AndAlso ThemeCheckOne(7) AndAlso ThemeCheckOne(8) AndAlso ThemeCheckOne(9)
        End With
    End Sub
    Friend Function ThemeCheckOne(Id As Integer) As Boolean
        If Id <= 5 Then Return True
        Try
            Dim V As Object = Microsoft.Win32.Registry.GetValue("HKEY_CURRENT_USER\Software\" & RegFolder, "ThemeUnlock" & Id, Nothing)
            If V IsNot Nothing Then
                Dim S As String = V.ToString().Trim().ToLowerInvariant()
                Return S = "true" OrElse S = "1" OrElse V.Equals(True)
            End If
        Catch
        End Try
        Return Settings.Get(Of Boolean)("ThemeUnlock" & Id)
    End Function
    Friend Function ThemeUnlock(Id As Integer, Optional ShowDoubleHint As Boolean = True, Optional UnlockHint As String = Nothing) As Boolean
        Settings.Set("ThemeUnlock" & Id, True)
        Try
            Microsoft.Win32.Registry.SetValue("HKEY_CURRENT_USER\Software\" & RegFolder, "ThemeUnlock" & Id, "True")
        Catch
        End Try
        If FrmMain IsNot Nothing Then
            RunInUiWait(Sub() ThemeCheckAll(True))
        Else
            ThemeCheckAll(True)
        End If
        If ShowDoubleHint Then Hint(If(String.IsNullOrEmpty(UnlockHint), "主题已解锁！", UnlockHint))
        Return True
    End Function

#End Region

#Region "更新"
    '更新源：GitHub Releases。发布新版时把 Release 的 Tag 填为比当前 VersionCode（413）更大的数字，并上传名为 MCL.exe 的文件
    Public Const UpdateRepo As String = "mioxdMCL/MCL"

    Friend Sub UpdateCheckByButton()
        RunInNewThread(AddressOf UpdateCheckThread, "MCL UpdateCheck", ThreadPriority.Normal)
    End Sub
    Private Sub UpdateCheckThread()
        Try
            RunInUiWait(Sub() Hint("正在检查更新……"))
            Dim Json As JObject = NetRequestByClientRetry("https://api.github.com/repos/" & UpdateRepo & "/releases/latest", RequireJson:=True).DeserializeJson()
            Dim Tag As String = Json("tag_name").ToString()
            Dim NewVersion As Integer = 0
            Integer.TryParse(Tag, NewVersion)
            Dim ExeUrl As String = ""
            For Each Asset As JObject In Json("assets")
                If Asset("name").ToString().ToLower() = "mcl.exe" Then
                    ExeUrl = Asset("browser_download_url").ToString()
                    Exit For
                End If
            Next
            If NewVersion > VersionCode Then
                If ExeUrl = "" Then
                    RunInUiWait(Sub() MyMsgBox($"发现新版本 {Tag}，但该 Release 中未找到 MCL.exe 文件，请通知作者补传。", "更新"))
                    Return
                End If
                Dim ShouldUpdate As Integer = RunInUiWait(Of Integer)(Function() MyMsgBox($"发现新版本：{Tag}（当前版本：{VersionCode}）" & vbCrLf & "是否下载并更新？", "发现新版本", "更新", "取消"))
                If ShouldUpdate = 1 Then
                    RunInUiWait(Sub() Hint("正在下载新版本……"))
                    NetDownloadByClient(ExeUrl, Paths.Base & "MCL\MCL.exe")
                    If FileUtils.Exists(Paths.Base & "MCL\MCL.exe") AndAlso New System.IO.FileInfo(Paths.Base & "MCL\MCL.exe").Length > 1000000 Then
                        RunInUiWait(Sub() MyMsgBox("下载完成！将自动退出并完成更新，更新完成后 MCL 会自动重新打开。", "更新"))
                        UpdateRestart(True)
                    Else
                        RunInUiWait(Sub() MyMsgBox("下载失败，请稍后重试。", "更新"))
                    End If
                End If
            Else
                RunInUiWait(Sub() MyMsgBox("当前已是最新版本。", "更新"))
            End If
        Catch ex As Exception
            RunInUiWait(Sub() MyMsgBox("检查更新失败：" & ex.Message, "更新"))
        End Try
    End Sub

    Friend IsUpdateWaitingRestart As Boolean = False
    Public Sub UpdateRestart(TriggerRestartAndByEnd As Boolean)
        If Not TriggerRestartAndByEnd Then Return
        Try
            Dim BatPath As String = PathTemp & "UpdateMCL.bat"
            Dim NewExe As String = Paths.Base & "MCL\MCL.exe"
            Dim CurExe As String = Paths.Base & "MCL.exe"
            Dim Q As String = Chr(34)
            System.IO.File.WriteAllText(BatPath,
                "@echo off" & vbCrLf &
                "ping -n 3 127.0.0.1 >nul" & vbCrLf &
                "taskkill /f /im MCL.exe >nul 2>nul" & vbCrLf &
                "copy /y " & Q & NewExe & Q & " " & Q & CurExe & Q & " >nul" & vbCrLf &
                "del " & Q & NewExe & Q & " >nul 2>nul" & vbCrLf &
                "cd /d " & Q & Paths.Base & Q & vbCrLf &
                "MCL.exe")
            System.Diagnostics.Process.Start(New System.Diagnostics.ProcessStartInfo(BatPath) With {.UseShellExecute = True, .WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden})
        Catch ex As Exception
            Logger.Error(ex, "启动更新脚本失败")
        End Try
    End Sub
    Public Sub UpdateReplace(ProcessId As Integer, OldFileName As String, NewFileName As String, TriggerRestart As Boolean)
    End Sub

    ''' <summary>
    ''' 确保 PathTemp/Latest.exe 是最新正式版的 PCL，它会被用于整合包打包。
    ''' 如果不是，则下载一个。
    ''' </summary>
    Friend Sub DownloadLatestPCL(Optional LoaderToSyncProgress As LoaderBase = Nothing)
        '注意：如果要自行实现这个功能，请换用另一个文件路径，以免与官方版本冲突
    End Sub

#End Region

#Region "联网配置"

    ''' <summary>
    ''' 联网获取的配置信息。
    ''' 若获取失败或仍在获取中，可能为 Nothing。
    ''' </summary>
    Public ServerConfig As JObject

    Public ServerLoader As New LoaderTask(Of Integer, Integer)("MCL 配置更新",
        Sub(Loader)
            Loader.Progress = 0.3
            Logger.Info("该版本中不包含更新通知功能……")
            Thread.Sleep(120)
            Loader.Progress = 1
        End Sub, Priority:=ThreadPriority.BelowNormal) With
        {.ReloadTimeout = 1000 * 60 * 60} '超时 1 小时

#End Region

#Region "赞助等级"

    Public ReadOnly Property CurrentRank As DonationRank
        Get
            Return DonationRank.None
        End Get
    End Property

    Private Const UnlockKey As String = "MCL-Sponsor-0124-MioCraft"

    Public Sub InputPotatoCode(IsUpdating As Boolean)
        RunInUiWait(Sub() MyMsgBox("请在 设置 → 个性化 → 主题 区域的解锁码输入框中输入解锁码。", "输入解锁码"))
    End Sub


    Friend Function ThemeUnlockByCode(Code As String) As Boolean
        If Not CheckUnlockCode(Code) Then Return False
        For Each Id As Integer In {6, 7, 8, 9, 10, 11, 12, 13, 14}
            Settings.Set("ThemeUnlock" & Id, True)
            Try
                Microsoft.Win32.Registry.SetValue("HKEY_CURRENT_USER\Software\" & RegFolder, "ThemeUnlock" & Id, "True")
            Catch
            End Try
        Next
        If FrmMain IsNot Nothing Then
            RunInUiWait(Sub() ThemeCheckAll(True))
        Else
            ThemeCheckAll(True)
        End If
        If FrmSetupUI IsNot Nothing Then
            FrmSetupUI.RadioLauncherTheme6.IsEnabled = True
            FrmSetupUI.RadioLauncherTheme7.IsEnabled = True
            FrmSetupUI.RadioLauncherTheme8.IsEnabled = True
            FrmSetupUI.RadioLauncherTheme9.IsEnabled = True
            FrmSetupUI.RadioLauncherTheme10.IsEnabled = True
            FrmSetupUI.RadioLauncherTheme11.IsEnabled = True
            FrmSetupUI.RadioLauncherTheme12.IsEnabled = True
            FrmSetupUI.RadioLauncherTheme13.IsEnabled = True
            FrmSetupUI.RadioLauncherTheme14.IsEnabled = True
        End If
        Return True
    End Function


    Private Function CheckUnlockCode(Code As String) As Boolean
        Try
            Dim C As String = Code.Trim().ToUpperInvariant()
            If Not C.StartsWith("MCL-") Then Return False
            Dim Parts As String() = C.Split("-"c)
            If Parts.Length <> 3 Then Return False
            Parts(1) = Parts(1).Trim() : Parts(2) = Parts(2).Trim()
            If Parts(1).Length <> 8 OrElse Parts(2).Length <> 8 Then Return False
            Dim Expect As String = HashHex("MCL-UNLOCK-" & Parts(1) & UnlockKey).Substring(0, 8).ToUpperInvariant()
            Return Parts(2) = Expect
        Catch
            Return False
        End Try
    End Function

    Private Function HashHex(Input As String) As String
        Dim Bytes As Byte() = System.Text.Encoding.UTF8.GetBytes(Input)
        Dim Hash As Byte() = System.Security.Cryptography.SHA512.Create().ComputeHash(Bytes)
        Return String.Concat(Hash.Select(Function(b) b.ToString("x2")))
    End Function


    Friend Function GetIdentify() As String
        Try
            Dim MachineGuid As String = ""
            Try
                MachineGuid = Microsoft.Win32.Registry.GetValue("HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Cryptography", "MachineGuid", "").ToString()
            Catch
            End Try
            If String.IsNullOrEmpty(MachineGuid) Then
                MachineGuid = Environment.MachineName & "|" & Environment.UserName & "|" & Environment.ProcessorCount & "|" & Environment.OSVersion.Version.ToString()
            End If
            Dim Hex As String = HashHex("MCL-IDENT-" & MachineGuid.ToLowerInvariant())
            Return Hex.Substring(0, 4).ToUpperInvariant() & "-" & Hex.Substring(8, 4).ToUpperInvariant() & "-" & Hex.Substring(16, 4).ToUpperInvariant() & "-" & Hex.Substring(24, 4).ToUpperInvariant()
        Catch ex As Exception
            Return "0000-0000-0000-0000"
        End Try
    End Function

#End Region

End Module
