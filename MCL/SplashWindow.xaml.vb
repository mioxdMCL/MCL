Imports System.Windows.Threading

Public Class SplashWindow

    Private Stopped As Boolean

    '进度完全由真实加载任务驱动，不再按时间假装推进
    Private ProgressCurrent As Double = 0
    Private MainReady As Boolean = False
    Private IsClosedNow As Boolean = False
    Private ReadOnly ProgressTimer As New DispatcherTimer With {.Interval = New TimeSpan(0, 0, 0, 0, 16)}
    Private ReadOnly StartTick As Long = Environment.TickCount
    Private ReadOnly WatchTasks As New List(Of Func(Of LoaderBase))

    ''' <summary>
    ''' 闪屏关闭（进度条走满）时触发，此时才显示主界面。
    ''' </summary>
    Public Event LoadFinished()

    ''' <summary>
    ''' 闪屏是否已经消失。
    ''' </summary>
    Public ReadOnly Property IsGone As Boolean
        Get
            Return IsClosedNow
        End Get
    End Property

    Public Sub New()
        InitializeComponent()
        AddHandler ProgressTimer.Tick, AddressOf ProgressTick
        ProgressTimer.Start()
    End Sub

    ''' <summary>
    ''' 登记参与启动进度计算的加载器。闪屏会实时读取它们的真实进度。
    ''' </summary>
    Public Sub WatchLoader(TaskGetter As Func(Of LoaderBase))
        If TaskGetter Is Nothing Then Return
        SyncLock WatchTasks
            WatchTasks.Add(TaskGetter)
        End SyncLock
    End Sub

    ''' <summary>
    ''' 计算真实进度：各加载器进度的平均值；未开始的任务按 0 计。
    ''' </summary>
    Private Function GetRealProgress() As Double
        Dim Tasks As New List(Of LoaderBase)
        SyncLock WatchTasks
            For Each Getter In WatchTasks
                Try
                    Dim Task As LoaderBase = Getter()
                    If Task IsNot Nothing Then Tasks.Add(Task)
                Catch
                End Try
            Next
        End SyncLock
        If Tasks.Count = 0 Then Return 0
        Dim Total As Double = 0
        For Each Task In Tasks
            Try
                Total += Task.Progress.Clamp(0, 1)
            Catch
                Total += 1
            End Try
        Next
        Return Total / Tasks.Count
    End Function

    Private Sub ProgressTick()
        If IsClosedNow Then Return
        If MainReady Then
            '主界面已就绪：两帧内补满并关闭
            ProgressCurrent = Math.Min(ProgressCurrent + 5, 100)
            ProgressBar.Value = ProgressCurrent
            If ProgressCurrent >= 100 Then CloseNow()
            Return
        End If
        '真实进度映射到 0 ~ 99，保留最后 1% 给“就绪”信号
        Dim Real As Double = GetRealProgress() * 99
        '只许前进，不许回退（重试时进度可能回落到 0）
        If Real > ProgressCurrent Then
            '平滑过渡，避免某次跳变太突然
            ProgressCurrent = Math.Min(ProgressCurrent + Math.Max((Real - ProgressCurrent) * 0.25, 0.2), Real)
        End If
        '超时兜底：若加载器异常卡死（例如网络挂住），30 秒后仍允许收尾，避免永远停在闪屏
        If (Environment.TickCount - StartTick) > 30000 Then
            ProgressCurrent = Math.Min(ProgressCurrent + 0.3, 99)
        End If
        ProgressBar.Value = ProgressCurrent
    End Sub

    ''' <summary>
    ''' 通知主界面初始化完成：进度补满后关闭闪屏。
    ''' </summary>
    Public Sub NotifyMainReady()
        If IsClosedNow Then Return
        MainReady = True
        If Not ProgressTimer.IsEnabled Then ProgressTimer.Start()
    End Sub

    Public Property ProgressValue As Integer
        Get
            Return CInt(ProgressBar.Value)
        End Get
        Set(value As Integer)
            ProgressSet(value)
        End Set
    End Property

    Private Sub ProgressSet(value As Integer)
        ProgressBar.Value = Math.Min(value, 100)
    End Sub

    Public Overloads Sub Close(delay As TimeSpan)
        Stopped = True
        Dim Timer As New DispatcherTimer With {.Interval = delay}
        AddHandler Timer.Tick, Sub()
                                   Timer.Stop()
                                   CloseNow()
                               End Sub
        Timer.Start()
    End Sub

    ''' <summary>
    ''' 强制把进度条推到 100% 并关闭，随后触发 LoadFinished 让主界面显示。
    ''' </summary>
    Private Sub CloseNow()
        If IsClosedNow Then Return
        IsClosedNow = True
        ProgressTimer.Stop()
        ProgressBar.Value = 100
        '先让进度条这一刻的画面渲染出来，再关闪屏，避免用户看到“没满就消失”
        Dispatcher.BeginInvoke(New Action(
        Sub()
            RaiseEvent LoadFinished()
            Try
                Close()
            Catch
            End Try
        End Sub), DispatcherPriority.Render)
    End Sub

End Class
