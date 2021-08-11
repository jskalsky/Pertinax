using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DrvTcpIp
{
  public enum ServiceType { None, Upload, Download, GetFiles, GetDirectories, GetErrorLog }

  public class DiagClient : IDisposable
  {
    private readonly TcpClient _tcpClient;
    private readonly object _diagLock = new object();
    private readonly List<string> _errors = new List<string>();

    public DiagClient()
    {
      _tcpClient = new TcpClient();
    }

    private void ReleaseUnmanagedResources()
    {
        _tcpClient.Client.Dispose();
        _tcpClient.Close();
    }

    public void Dispose()
    {
      ReleaseUnmanagedResources();
      GC.SuppressFinalize(this);
    }

    ~DiagClient()
    {
      ReleaseUnmanagedResources();
    }

    public async void Connect(IPAddress target, int targetPort = 20002, int timeout = 5)
    {
      TimeSpan timeOut = TimeSpan.FromSeconds(timeout);
      TaskCompletionSource<bool> cancellationCompletionSource = new TaskCompletionSource<bool>();
      try
      {
        using (CancellationTokenSource cts = new CancellationTokenSource(timeOut))
        {
          Task task = _tcpClient.ConnectAsync(target, targetPort);

          using (cts.Token.Register(() => cancellationCompletionSource.TrySetResult(true)))
          {
            if (task != await Task.WhenAny(task, cancellationCompletionSource.Task))
            {
              throw new OperationCanceledException(cts.Token);
            }
          }
        }
      }
      catch (OperationCanceledException exc)
      {
        lock (_diagLock)
        {
          _errors.Add(exc.Message);
        }
      }
    }
  }
}
