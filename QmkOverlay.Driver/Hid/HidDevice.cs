using HidSharp;

namespace QmkOverlay.Driver.Hid;

public class HidDevice : IDisposable
{
    internal HidSharp.HidDevice _device = null!;
    internal HidStream? _stream;

    internal HidDevice(HidSharp.HidDevice device)
    {
        _device = device;
    }
    public void Dispose()
    {
        _stream?.Dispose();
    }

    public bool OpenConnection()
    {
        if (_stream != null) return true;

        if (!_device.TryOpen(out HidStream hidStream))
            return false;

        _stream = hidStream;
        _stream.ReadTimeout = Timeout.Infinite;

        return true;
    }
    public void SendData(params byte[] data)
    {
        if (_stream == null)
            throw new InvalidOperationException("Unable to send data without opening connection");
        if (data == null || data.Length == 0) return;

        var dataToSend = new byte[data.Length + 1];
        dataToSend[0] = 0x00;
        Array.Copy(data, 0, dataToSend, 1, data.Length);
        _stream.Write(dataToSend);
    }

    public override string ToString()
    {
        return _device.ToString();
    }
}
