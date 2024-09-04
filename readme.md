# Sync2

[![Build Status](https://ci.appveyor.com/api/projects/status/apiqkde3648ykncb?svg=true)](https://ci.appveyor.com/project/UlyssesWu/sync2)

### by Ulysses

停更说明：由于QQ NT已经不再支持显示正在播放的音乐，本项目正式停更。另外最新的网易云已经是64位程序，也需要重新适配一下，目前没有花时间适配的必要。

一个用于将音乐播放器正在播放的曲目同步到QQ状态的小工具。

>*果然……没啥用的样子*   ——@bromine0x23

目前支持 **网易云音乐（PC/UWP）** 、 **千千静听** 、**foobar2000** 。

在QQ 7.5-9.5.3（非QQ NT）测试通过。

### 兼容性

“DLL调用”模式：若QQ安装在系统分区可能会由于权限问题导致无法正常使用。此时请以管理员权限运行。若仍然提示失败则需要手动将`UPHelper.dll`复制到`QQ\Bin`目录下。

“COM调用”模式：在QQ 7.5以后的版本可能会乱码（QQ自身的问题）。

提示：

1. 设置的QQ号必须已在本机登录，且状态不能显示为“移动设备在线”（如果你有多个QQ账号，可设置关联后，将手机QQ切换到另一个账号；否则必须手机QQ先退出登录）。
2. 第一次运行强烈建议使用管理员权限，因为必须要将同步用的DLL复制到QQ安装目录下才能实现功能。成功开启同步一次后，后续不用管理员权限启动也可以。
3. 为了避免中文显示乱码，同步策略建议采用“DLL调用”。采用这个策略必须保证满足第2条提示的要求。

### 需求&库

.NET Framework 4 +

[Easyhook](https://easyhook.github.io/) 从NuGet取得

[Linq2db](https://github.com/linq2db/linq2db) 从NuGet取得

[Newtonsoft.Json](https://github.com/JamesNK/Newtonsoft.Json) 从NuGet取得

[VinjEx](https://github.com/UlyssesWu/VinjEx) 作为子模块取得

### LICENSE

**MIT**

### Log
- 2024：随着QQNT的到来正式完结
- 202112：支持foobar2000
- 201811：增加权限提示
- 201701：支持网易云音乐UWP
- 201511：缝缝补补以支持新版QQ接口
- 201411：支持网易云音乐
- 201403：支持千千静听

---

by Ulysses , wdwxy12345@gmail.com
