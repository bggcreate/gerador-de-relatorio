[Introduction]
The demo program introduces login, logout, setting and getting  video code configuration by channel.

[Interfaces]
NETClient.Init Initialize SDK
NETClient.Login Login
NETClient.Logout Logout
NETClient.GetNewDevConfig Get device configuration
NETClient.GetEncodeConfig Get video code configuration.
NETClient.SetEncodeConfig Set video code configuration.

[Notice]
When the compiling environment is VS2010, NETSDKCS library can support the version of .NET Framework 4.0 or newer. If you want to use the version older than .NET Framework 4.0, change the method of using NetSDK.cs in IntPtr.Add. We will not support the modification.
Copy all file in the libs directory of General_NetSDK_ChnEng_CSharpXX_IS_VX.XXX.XXXXXXXX.X.R.XXXXXX to the build directory of bin directory of the corresponding demo programs.

【演示程序功能】
1、演示程序介绍设备登陆、设备登出、通过通道设置与获取视频编码配置

【接口列表】
NETClient.Init SDK初始化
NETClient.Login 登录设备
NETClient.Logout 登出设备
NETClient.GetNewDevConfig 配置信息
NETClient.GetEncodeConfig 获取视频编码信息
NETClient.SetEncodeConfig 设置视频编码信息

【注意事项】
1、编译环境为VS2010，NETSDKCS库最低只支持.NET Framework 4.0,如用户需要支持低于4.0的版本需要更改NetSDK.cs文件中使用到IntPtr.Add的方法,我们不提供修改。
2、请把General_NetSDK_ChnEng_CSharpXX_IS_VX.XXX.XXXXXXXX.X.R.XXXXXX中libs目录中的所有文件复制到对应该演示程序中bin目录下的生成目录中。
