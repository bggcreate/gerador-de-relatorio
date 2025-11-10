[Introduction]
The demo program introduces login, logout, starting real time monitor, stopping real time monitor, local snapshot, remote snapshot and timing snapshot.

[Interfaces]
NETClient.Init Initialize SDK
NETClient.SetDVRMessCallBack Set snapshot callback
NETClient.Login Login
NETClient.Logout Logout
NETClient.RealPlay Start real time monitor
NETClient.StopRealPlay Stop real time monitor
NETClient.SnapPictureEx  Local snapshot
NETClient.SnapPictureEx  Remote snapshot and timing  snapshot.
NETClient.Cleanup Release SDK resources

[Notice]
When the compiling environment is VS2010, NETSDKCS library can support the version of .NET Framework 4.0 or newer. If you want to use the version older than .NET Framework 4.0, change the method of using NetSDK.cs in IntPtr.Add. We will not support the modification.
The min time of real time snapshot is 1 s.
Ensure that you have turned on the real time monitor before local snapshot.
Copy all file in the libs directory of General_NetSDK_ChnEng_CSharpXX_IS_VX.XXX.XXXXXXXX.X.R.XXXXXX to the build directory of bin directory of the corresponding demo programs.

【演示程序功能】
1、演示程序介绍设备登录、设备登出、打开实时监视、关闭实时监视、本地抓图、远程抓图、定时抓图。

【接口列表】
NETClient.Init SDK初始化
NETClient.SetSnapRevCallBack 设置抓图回调
NETClient.Login 登录设备
NETClient.Logout 登出设备
NETClient.RealPlay 实时监视
NETClient.StopRealPlay 关闭实时监视
NETClient.CapturePicture 本地抓图
NETClient.SnapPictureEx 远程抓图与定时抓图
NETClient.Cleanup 释放SDK资源

【注意事项】
1、编译环境为VS2010，NETSDKCS库最低只支持.NET Framework 4.0,如用户需要支持低于4.0的版本需要更改NetSDK.cs文件中使用到IntPtr.Add的方法,我们不提供修改。
2、实时抓图最小时间为1秒。
3、本地抓图必须要先打开实时监视。
4、请把General_NetSDK_ChnEng_CSharpXX_IS_VX.XXX.XXXXXXXX.X.R.XXXXXX中libs目录中的所有文件复制到对应该演示程序中bin目录下的生成目录中。