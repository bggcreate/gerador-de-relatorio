[Introduction]
The demo program introduces searching device by multicast and broadcast, searching device by IPs, and device initialization.

[Interfaces]
NETClient.Init Initialize SDK
NETClient.StartSearchDevice Search device by multicast and broadcast
NETClient.StopSearchDevice Stop searching
NETClient.SearchDevicesByIPs Search device by IPs
NETClient.InitDevAccount Initialize account
NETClient.Cleanup Release SDK resources

[Notice]
When the compiling environment is VS2010, NETSDKCS library can support the version of .NET Framework 4.0 or newer. If you want to use the version older than .NET Framework 4.0, change the method of using NetSDK.cs in IntPtr.Add. We will not support the modification.
Password recovery is not supported. 
Copy all file in the libs directory of General_NetSDK_ChnEng_CSharpXX_IS_VX.XXX.XXXXXXXX.X.R.XXXXXX to the build directory of bin directory of the corresponding demo programs.

【演示程序功能】
1、演示程序介绍通过组播与广播搜索设备、点对点搜索设备、初始化设备。

【接口列表】
NETClient.Init SDK初始化
NETClient.StartSearchDevice 组播与广播搜索
NETClient.StopSearchDevice 停止搜索
NETClient.SearchDevicesByIPs 点对点搜索
NETClient.InitDevAccount 初始化用户
NETClient.Cleanup 释放SDK资源

【注意事项】
1、编译环境为VS2010，NETSDKCS库最低只支持.NET Framework 4.0,如用户需要支持低于4.0的版本需要更改NetSDK.cs文件中使用到IntPtr.Add的方法,我们不提供修改。
2、不支持密码找回。
3、请把General_NetSDK_ChnEng_CSharpXX_IS_VX.XXX.XXXXXXXX.X.R.XXXXXX中libs目录中的所有文件复制到对应该演示程序中bin目录下的生成目录中。