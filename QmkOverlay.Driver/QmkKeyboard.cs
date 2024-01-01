using HidSharp;

namespace QmkOverlay.Driver;

public class QmkKeyboard : Hid.HidDevice
{
    internal QmkKeyboard(HidDevice device) : base(device)
    {
    }

    public async Task<ulong?> GetKeyboardId()
    {
        SendData(0xFE, 0x00);
        var readBuffer = new byte[_device.GetMaxInputReportLength()];
        await _stream!.ReadAsync(readBuffer, 0, readBuffer.Length);
        return BitConverter.ToUInt64(readBuffer, 5);
    }
    public async Task<bool> GetUnlockStatus()
    {
        SendData(0xFE, 0x05);
        var readBuffer = new byte[_device.GetMaxInputReportLength()];
        await _stream!.ReadAsync(readBuffer, 0, readBuffer.Length);
        return (readBuffer?.ElementAtOrDefault(1) ?? 0) == 1;
    }
    public async Task<byte[]?> GetMatrixState()
    {
        SendData(0x02, 0x03);
        var readBuffer = new byte[_device.GetMaxInputReportLength()];
        await _stream!.ReadAsync(readBuffer, 0, readBuffer.Length);
        return readBuffer.Skip(3).ToArray();
    }
}
