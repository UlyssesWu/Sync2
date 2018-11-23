# Sync2

[![Build Status](https://ci.appveyor.com/api/projects/status/apiqkde3648ykncb?svg=true)](https://ci.appveyor.com/project/UlyssesWu/sync2)

### by Ulysses

一个用于将音乐播放器正在播放的曲目同步到QQ状态的小工具。

>*果然……没啥用的样子*   ——@bromine0x23

目前支持 **网易云音乐（PC/UWP）** 及 **千千静听** 。

在QQ 7.5-9.0.7测试通过。

关于“DLL调用”模式：若QQ安装在系统分区可能会由于权限问题导致无法正常使用。此时需要手动将`UPHelper.dll`复制到`QQ\Bin`目录下。建议升级到QQ 9.0.7或更新版本并使用“COM调用”模式。

### 需求&库

.NET Framework 4 +

[Easyhook](https://easyhook.github.io/) 从NuGet取得

[Linq2db](https://github.com/linq2db/linq2db) 从NuGet取得

[Newtonsoft.Json](https://github.com/JamesNK/Newtonsoft.Json) 从NuGet取得

[VinjEx](https://github.com/UlyssesWu/VinjEx) 作为子模块取得

### LICENSE

**MIT**

### Log
- 201811：将默认同步模式回调至COM调用（事实证明QQ 7.5-8.x的接口问题是TX常年未修的BUG）
- 201701：支持网易云音乐UWP
- 201511：缝缝补补以支持新版QQ接口
- 201411：支持网易云音乐
- 201403：支持千千静听

---

by Ulysses , wdwxy12345@gmail.com





