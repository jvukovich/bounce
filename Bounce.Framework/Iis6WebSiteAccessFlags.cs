using System;

namespace Bounce.Framework
{
    [Flags]
    public enum Iis6WebSiteAccessFlags
    {
        Execute = 4,
        NoRemoteExecute = 0x2000,
        NoRemoteRead = 0x1000,
        NoRemoteScript = 0x4000,
        NoRemoteWrite = 0x400,
        Read = 1,
        Script = 0x200,
        ScriptSource = 0x10,
        Write = 2
    }
}