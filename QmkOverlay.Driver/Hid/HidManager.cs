using HidSharp.Reports.Encodings;
using HidSharp;

namespace QmkOverlay.Driver.Hid;

public static class HidManager
{
    public static ICollection<HidDevice> GetDevices()
    {
        var hidDevices = DeviceList.Local.GetHidDevices();
        return hidDevices
            .Select(d =>
            {
                try
                {
                    var descriptor = d.GetRawReportDescriptor();
                    var items = EncodedItem.DecodeItems(descriptor, 0, descriptor.Length);

                    var usagePage = items.FirstOrDefault(i => i.ItemType == ItemType.Global && i.TagForGlobal == GlobalItemTag.UsagePage);
                    var usage = items.FirstOrDefault(i => i.ItemType == ItemType.Local && i.TagForLocal == LocalItemTag.Usage);
                    if (usagePage == null || usage == null) return new HidDevice(d);

                    return usagePage.DataValue == 0xff60 && usage.DataValue == 0x61
                        ? new QmkKeyboard(d)
                        : new HidDevice(d);
                }
                catch (Exception)
                {
                    return new HidDevice(d);
                }
            })
            .ToList();
    }
}
