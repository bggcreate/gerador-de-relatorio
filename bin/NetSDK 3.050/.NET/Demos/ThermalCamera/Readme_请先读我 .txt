[Introduction]
The demo program introduces SDK initialization,login, logout, auto reconnection, querying temperature point, querying temperature parameters, querying thermal temperature and getting temperature distributed data.

	  
[Interfaces]
NETClient.Init Initialize SDK and set disconnection callback
NETClient.SetAutoReconnect Set auto reconnection callback
NETClient.Login Login
NETClient.Logout Logout
NETClient.QueryDevInfo Query temperature point/temperature parameters
NETClient.StartFind Start querying thermal temperature 
NETClient.DoFind Continue querying  thermal temperature
NETClient.StopFind Stop querying thermal temperature
NETClient.RadiometryAttach Attach temperature distributed data
NETClient.RadiometryDetach Detach temperature distributed data
NETClient.RadiometryFetch Get data of thermal map 
NETClient.Cleanup Release SDK resources


[Notice]
When the compiling environment is VS2010, NETSDKCS library can support the version of .NET Framework 4.5 or newer. If you want to use the version older than .NET Framework 4.5, change the method of using MainWindow.xaml in Combobox. We will not support the modification.
For querying thermal temperature, you need ensure that after finishing calling NETClient.StartFind, forward handle value in output parameters to the input parameters of NETClient.DoFind to continue the query, and it supports 32 queries at most. 
Copy all file in the libs directory of General_NetSDK_ChnEng_CSharpXX_IS_VX.XXX.XXXXXXXX.X.R.XXXXXX to the build directory of bin directory of the corresponding demo programs.

【演示程序功能】
1.演示程序介绍了SDK初始化、登陆设备、登出设备、自动重连、测温点参数查询、测温项参数查询、热成像温度查询、温度分布数据获取等功能。

	  
【接口列表】
NETClient.Init SDK初始化与设置断线回调
NETClient.SetAutoReconnect 设置自动重连回调
NETClient.Login 设备登录
NETClient.Logout 设备登出
NETClient.QueryDevInfo 查询温度点/温度项参数
NETClient.StartFind 开始查询热成像温度
NETClient.DoFind 继续查询热成像温度
NETClient.StopFind 结束查询热成像温度
NETClient.RadiometryAttach 订阅温度分布数据
NETClient.RadiometryDetach 取消订阅温度分布数据
NETClient.RadiometryFetch 获取热图数据
NETClient.Cleanup SDK释放资源


【注意事项】
1、编译环境为VS2013，NETSDKCS库最低支持.NET Framework 4.5,如用户需要支持低于4.5的版本需要更改MainWindow.xaml文件中使用到控件Combobox的方法,我们不提供修改。
2、查询热成像温度需要确保在调用NETClient.StartFind返回成功后将出参中的句柄值传给NETClient.DoFind入参结构体中继续查询，最多查询32条。
3、请把General_NetSDK_ChnEng_CSharpXX_IS_VX.XXX.XXXXXXXX.X.R.XXXXXX中libs目录中的所有文件复制到对应该演示程序中bin目录下的生成目录中。
