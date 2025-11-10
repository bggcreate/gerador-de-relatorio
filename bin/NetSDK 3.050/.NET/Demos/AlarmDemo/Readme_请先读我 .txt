[Introduction]
The demo program introduces SDK initialization, login, logout, auto reconnection, listening alarm, receiving alarm, analyzing  alarm, displaying alarm and stopping listening alarm.
The demo program demonstrates external alarm, dynamic detecting alarm, video loss alarm, video occlusion alarm, hard disk full alarm, and hard disk crash alarm.

[Interfaces]
NETClient.Init Initialize SDK and set disconnection callback
NETClient.SetAutoReconnect Set auto reconnection callback
NETClient.SetDVRMessCallBack Set alarm callback
NETClient.Login Login
NETClient.Logout Logout
NETClient.StartListen Listen alarm
NETClient.StopListen Stop listening
NETClient.GetLastError Get the last function error code
NETClient.Cleanup Release SDK resources

[Notice]
When the compiling environment is VS2010, NETSDKCS library can support the version of .NET Framework 4.0 or newer. If you want to use the version older than .NET Framework 4.0, change the method of using NetSDK.cs in IntPtr.Add. We will not support the modification.
The demo program demonstrates listening alarm of single device, and it doesn’t support listening alarm of multiple devices. If you need listen alarm of multiple devices, modify it by yourself. 
The demo program only demonstrates parts of alarm functions. If you need the other alarm functions, add it by yourself.
Copy all file in the libs directory of General_NetSDK_ChnEng_CSharpXX_IS_VX.XXX.XXXXXXXX.X.R.XXXXXX to the build directory of bin directory of the corresponding demo programs.

【演示程序功能】
1、演示程序介绍了SDK初始化、登陆设备、登出设备、自动重连、监听报警、接收报警信息、解析报警信息、显示报警信息、关闭监听报警功能。
2、演示程序演示了外部报警、动态检测报警、视频丢失报警、视频遮挡报警、硬盘满报警、坏硬盘报警。

【接口列表】
NETClient.Init SDK初始化与设置断线回调
NETClient.SetAutoReconnect 设置自动重连回调
NETClient.SetDVRMessCallBack 设置报警回调
NETClient.Login 设备登录
NETClient.Logout 设备登出
NETClient.StartListen 监听报警
NETClient.StopListen 停止监听
NETClient.GetLastError 获取错误信息
NETClient.Cleanup SDK释放资源


【注意事项】
1、编译环境为VS2010，NETSDKCS库最低只支持.NET Framework 4.0,如用户需要支持低于4.0的版本需要更改NetSDK.cs文件中使用到IntPtr.Add的方法,我们不提供修改。
2、此演示程序只演示监听单设备报警功能，不支持监听多设备报警功能，如用户有需求请自行修改。
3、此演示程序仅演示部分通用报警功能，如需另外报警功能，请自行添加。
4、请把General_NetSDK_ChnEng_CSharpXX_IS_VX.XXX.XXXXXXXX.X.R.XXXXXX中libs目录中的所有文件复制到对应该演示程序中bin目录下的生成目录中。
