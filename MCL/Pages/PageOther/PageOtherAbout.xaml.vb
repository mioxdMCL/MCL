Public Class PageOtherAbout

    Private Shadows IsLoaded As Boolean = False
    Private Sub PageOtherAbout_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded

        '重复加载部分
        PanBack.ScrollToHome()

        '非重复加载部分
        If IsLoaded Then Return
        IsLoaded = True

        ItemAboutPcl.Info = ItemAboutPcl.Info.Replace("%VERSION%", VersionDisplay).Replace("%VERSIONCODE%", VersionCode).Replace("%BRANCH%", "0")

        TextAboutIdentify.Text = "你是谁：" & GetIdentify()

    End Sub

    Public Shared Sub CopyIdentify()
        ClipboardSet(Identify)
    End Sub
    Private Sub BtnAboutUpdate_Click(sender As Object, e As MouseButtonEventArgs) Handles BtnAboutUpdate.Click
        UpdateCheckByButton()
    End Sub
    Private Sub BtnAboutCopyIdentify_Click(sender As Object, e As MouseButtonEventArgs) Handles BtnAboutCopyIdentify.Click
        CopyIdentify()
    End Sub
    Private Sub BtnAboutUnlockByCode_Click(sender As Object, e As MouseButtonEventArgs) Handles BtnAboutUnlockByCode.Click
        Dim Code As String = MyMsgBoxInput("MCL 主题解锁", "请输入爱发电购买获得的解锁码（MCL- 开头）：", , , "MCL-XXXXXXXX-XXXXXXXX", "解锁", "取消")
        If Code Is Nothing OrElse Code.Trim() = "" Then Return
        If ThemeUnlockByCode(Code) Then
            MyMsgBox("解锁成功！感谢支持 MCL 开发！所有赞助主题已解锁。" & vbCrLf & vbCrLf & "如果主题仍是灰色，关闭并重新打开 MCL 即可生效（解锁状态已保存）。", "MCL 解锁")
        Else
            MyMsgBox("解锁码不正确！请完整复制卡密（MCL- 开头，格式：MCL-XXXXXXXX-XXXXXXXX，共 21 位，注意字母大小写与横杠）。", "MCL 解锁", IsWarn:=True)
        End If
    End Sub

End Class
