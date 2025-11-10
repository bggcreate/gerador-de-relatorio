[Introduction]
The demo program introduces login, logout, starting real time monitor, stopping real time monitor, getting and configuring OSD of main stream (channel title), getting and configuring OSD of main stream (time title), and getting and configuring OSD of main stream (customized title).

[Interfaces]
NETClient.Init Initialize SDK and set disconnection callback
NETClient.SetAutoReconnect Set reconnection callback
NETClient.Login Login
NETClient.Logout Logout
NETClient.RealPlay Start real time monitor
NETClient.StopRealPlay Stop real time monitor
NETClient.GetOSDConfig Get OSD configuration
NETClient.SetOSDConfig Configure OSD
NETClient.Cleanup Release SDK resources


[Notice]
When the compiling environment is VS2010, NETSDKCS library can support the version of .NET Framework 4.0 or newer. If you want to use the version older than .NET Framework 4.0, change the method of using NetSDK.cs in IntPtr.Add. We will not support the modification.
The demo program supports main stream configuration only.
Copy all file in the libs directory of General_NetSDK_ChnEng_CSharpXX_IS_VX.XXX.XXXXXXXX.X.R.XXXXXX to the build directory of bin directory of the corresponding demo programs.

【演示程序功能】
1、演示程序介绍设备登录、设备登出、打开实时监视、关闭实时监视、获取与设置主码流的通道标题的OSD配置、获取与设置主码流的时间标题的OSD配置、获取与设置主码流的自定义标题的OSD配置。

【接口列表】
NETClient.Init SDK初始化与设置断线回调
NETClient.SetAutoReconnect 设置重连回调
NETClient.Login 登录设备
NETClient.Logout 登出设备
NETClient.RealPlay 实时监视
NETClient.StopRealPlay 停止实时监视
NETClient.GetOSDConfig 获取OSD配置
NETClient.SetOSDConfig 设置OSD配置
NETClient.Cleanup 释放SDK资源


【注意事项】
1、编译环境为VS2010，NETSDKCS库最低只支持.NET Framework 4.0,如用户需要支持低于4.0的版本需要更改NetSDK.cs文件中使用到IntPtr.Add的方法,我们不提供修改。
2、只支持主码流配置。
3、请把General_NetSDK_ChnEng_CSharpXX_IS_VX.XXX.XXXXXXXX.X.R.XXXXXX中libs目录中的所有文件复制到对应该演示程序中bin目录下的生成目录中。