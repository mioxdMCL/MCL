Imports System.Windows.Markup
Imports System.Windows.Media.Imaging
Imports System.Windows.Shapes

<ContentProperty("Text")>
Public Class MyCapeItem
    Implements IMyRadio

    Public Event Check(sender As Object, e As RouteEventArgs) Implements IMyRadio.Check
    Public Event Changed(sender As Object, e As RouteEventArgs) Implements IMyRadio.Changed

    Public Property Text As String
        Get
            Return LabTitle.Text
        End Get
        Set(value As String)
            LabTitle.Text = value
        End Set
    End Property

    Private Const AnimationTimeOfCheck As Integer = 150
    Private ReadOnly CapeUuid As Integer = GetUuid()

    Public Property IsChecked As Boolean
        Get
            Return ShapeDot.Width >= 5
        End Get
        Set(value As Boolean)
            If Not IsLoaded Then
                If value Then
                    ShapeDot.Width = 9
                    ShapeDot.Height = 9
                    ShapeDot.Opacity = 1
                    ShapeDot.Margin = New Thickness(5.5, 0, 0, 0)
                    LabTitle.FontWeight = FontWeights.Bold
                Else
                    ShapeDot.Width = 0
                    ShapeDot.Height = 0
                    ShapeDot.Opacity = 0
                    ShapeDot.Margin = New Thickness(10, 0, 0, 0)
                    LabTitle.FontWeight = FontWeights.Normal
                End If
                Return
            End If
            If value Then
                If ShapeDot.Opacity < 0.01 Then ShapeDot.Opacity = 1
                AniStart({
                    AaScale(ShapeBorder, 10 - ShapeBorder.Width, AnimationTimeOfCheck,, New AniEaseOutFluent(AniEasePower.Weak),, True),
                    AaScale(ShapeBorder, 8, AnimationTimeOfCheck * 2, AnimationTimeOfCheck * 0.6, New AniEaseOutBack,, True),
                    AaColor(ShapeBorder, Ellipse.StrokeProperty, If(IsEnabled, "ColorBrush2", "ColorBrushGray5"), AnimationTimeOfCheck)
                }, "MyCapeItem Border " & CapeUuid)
                AniStart({
                    AaScale(ShapeDot, 9 - ShapeDot.Width, AnimationTimeOfCheck * 2.6,, New AniEaseOutBack(AniEasePower.Weak),, True),
                    AaOpacity(ShapeDot, 1 - ShapeDot.Opacity, AnimationTimeOfCheck * 0.5, AnimationTimeOfCheck * 0.6)
                }, "MyCapeItem Dot " & CapeUuid)
                LabTitle.FontWeight = FontWeights.Bold
            Else
                AniStart({
                    AaScale(ShapeBorder, 18 - ShapeBorder.Width, AnimationTimeOfCheck,, New AniEaseOutFluent,, True),
                    AaColor(ShapeBorder, Ellipse.StrokeProperty, If(IsEnabled, "ColorBrush1", "ColorBrushGray5"), AnimationTimeOfCheck)
                }, "MyCapeItem Border " & CapeUuid)
                AniStart({
                    AaScale(ShapeDot, -ShapeDot.Width, AnimationTimeOfCheck,, New AniEaseInFluent,, True),
                    AaOpacity(ShapeDot, -ShapeDot.Opacity, AnimationTimeOfCheck * 0.5, AnimationTimeOfCheck * 0.2)
                }, "MyCapeItem Dot " & CapeUuid)
                LabTitle.FontWeight = FontWeights.Normal
            End If
        End Set
    End Property

    Public Sub SetCapeImage(url As String)
        Try
            Dim Bi As New BitmapImage(New Uri(url))
            ImgCape.Source = Bi
            ImgCape.Visibility = Visibility.Visible
        Catch ex As Exception
            Logger.Warn(ex, "披风图片加载失败")
        End Try
    End Sub

    Private _ImgStream As IO.Stream
    Public Sub SetCapeImageResource(resourceName As String)
        Try
            Dim S As IO.Stream = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName)
            If S Is Nothing Then Return
            _ImgStream = S
            Dim Bi As New BitmapImage()
            Bi.BeginInit()
            Bi.StreamSource = S
            Bi.EndInit()
            ImgCape.Source = Bi
            ImgCape.Visibility = Visibility.Visible
        Catch ex As Exception
        End Try
    End Sub

    Private Sub PanBack_MouseLeftButtonUp(sender As Object, e As MouseButtonEventArgs) Handles Me.MouseLeftButtonUp
        If Not IsEnabled Then Return
        Dim Panel As StackPanel = TryCast(Me.Parent, StackPanel)
        If Panel IsNot Nothing Then
            For Each c In Panel.Children
                Dim Item As MyCapeItem = TryCast(c, MyCapeItem)
                If Item IsNot Nothing Then Item.IsChecked = False
            Next
        End If
        IsChecked = True
        RaiseEvent Check(Me, New RouteEventArgs(True))
        RaiseEvent Changed(Me, New RouteEventArgs(True))
    End Sub

End Class
