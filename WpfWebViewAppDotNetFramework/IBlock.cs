using System;
using System.Runtime.InteropServices;

namespace WpfWebViewApp
{
    [ComVisible(true)]
    [Guid("EAA4976A-45C3-4BC5-8C9D-7B58663C20A9")]
    [InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
    public interface IBlock
    {
        int NumberOfLots { get; set; }
        string Name { get; set; }
        int Id { get; set; }
        string GetLotIds();
    }
}
