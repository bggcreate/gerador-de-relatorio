[Introduction]
The demo program introduces login, logout, starting real time monitor, stopping real time monitor, attaching event, detaching event, displaying uploaded personnel entry information and clearing the number of statistics of the day.

[Interfaces]
NETClient.Init Initialize SDK
NETClient.Login Login
NETClient.Logout Logout
NETClient.RealPlay Start real time monitor
NETClient.RenderPrivateData Display/Not display rule
NETClient.StopRealPlay Stop monitor
NETClient.AttachVideoStatSummary Attach the number of statistics 
NETClient.DetachVideoStatSummary  Detach the number of statistics 
NETClient.ControlDevice Clear the number of statistics of the day
NETClient.Cleanup Release SDK resources

[Notice]
When the compiling environment is VS2010, NETSDKCS library can support the version of .NET Framework 4.0 or newer. If you want to use the version older than .NET Framework 4.0, change the method of using NetSDK.cs in IntPtr.Add. We will not support the modification.
Copy all file in the libs directory of General_NetSDK_ChnEng_CSharpXX_IS_VX.XXX.XXXXXXXX.X.R.XXXXXX to the build directory of bin directory of the corresponding demo programs.


【演示程序功能】
1、演示程序介绍设备登录、设备登出、打开实时监视、关闭实时监视、订阅事件、取消订阅事件、显示上报的人员进去信息、清空当天的人数统计信息。

【接口列表】
NETClient.Init SDK初始化
NETClient.Login 登录设备
NETClient.Logout 登出设备
NETClient.RealPlay 实时监视
NETClient.RenderPrivateData 显示与不显示画线
NETClient.StopRealPlay 停止监视
NETClient.AttachVideoStatSummary 订阅人数统计
NETClient.DetachVideoStatSummary 取消订阅
NETClient.ControlDevice 清空当天的人数统计
NETClient.Cleanup 释放SDK资源

【注意事项】
1、编译环境为VS2010，NETSDKCS库最低只支持.NET Framework 4.0,如用户需要支持低于4.0的版本需要更改NetSDK.cs文件中使用到IntPtr.Add的方法,我们不提供修改。
2、请把General_NetSDK_ChnEng_CSharpXX_IS_VX.XXX.XXXXXXXX.X.R.XXXXXX中libs目录中的所有文件复制到对应该演示程序中bin目录下的生成目录中。
