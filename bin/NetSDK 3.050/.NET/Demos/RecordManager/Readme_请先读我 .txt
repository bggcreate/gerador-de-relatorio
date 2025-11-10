[Introduction]
The demo program introduces login, logout listening event, cancel listening, displaying the event when the record plan changes, record plan management, and getting and configuring time plan.

[Interfaces]
NETClient.Init Initialize SDK
NETClient.SetDVRMessCallBack Set alarm callback
NETClient.Login Login
NETClient.Logout Logout
NETClient.StartListen Start listening
NETClient.StopListen Stop listening
NETClient.GetNewDevConfig Get record plan configuration
NETClient.GetNewDevConfig Configure record plan

[Notice]
When the compiling environment is VS2010, NETSDKCS library can support the version of .NET Framework 4.0 or newer. If you want to use the version older than .NET Framework 4.0, change the method of using NetSDK.cs in IntPtr.Add. We will not support the modification.
The demo program does not support holiday management.
Copy all file in the libs directory of General_NetSDK_ChnEng_CSharpXX_IS_VX.XXX.XXXXXXXX.X.R.XXXXXX to the build directory of bin directory of the corresponding demo programs.

【演示程序功能】
1、演示程序介绍设备登录、设备登出、监听事件、取消监听、显示录像计划更改时事件信息、录像计划管理、时间计划的获取与设置。

【接口列表】
NETClient.Init SDK初始化
NETClient.SetDVRMessCallBack 设置报警回调
NETClient.Login 登录设备
NETClient.Logout 登出设备
NETClient.StartListen 开始监听
NETClient.StopListen 停止监听
NETClient.GetNewDevConfig 获取录像计划配置信息
NETClient.SetNewDevConfig 设置录像计划配置信息

【注意事项】
1、编译环境为VS2010，NETSDKCS库最低只支持.NET Framework 4.0,如用户需要支持低于4.0的版本需要更改NetSDK.cs文件中使用到IntPtr.Add的方法,我们不提供修改。
2、不支持假期计划管理。
3、请把General_NetSDK_ChnEng_CSharpXX_IS_VX.XXX.XXXXXXXX.X.R.XXXXXX中libs目录中的所有文件复制到对应该演示程序中bin目录下的生成目录中。
