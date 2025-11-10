[Introduction]
The demo introduces SDK initialization, login, logout, card data, fingerprint data and face templete data. 


	  
[Interfaces]
NETClient.Init Initialize SDK and set disconnection callback
NETClient.SetAutoReconnect Set auto reconnection callback
NETClient.Login Login
NETClient.Logout Logout
NETClient.FindRecord Query user information
NETClient.FindNextRecord Continue to query user information
NETClient.FaceInfoOpreate Operation of face templete data 
NETClient.ControlDevice Operation of card/fingerprint data 
NETClient.Attendance_InsertFingerByUserID Add fingerprint
NETClient.Attendance_GetFingerByUserID Query fingerprint
NETClient.Cleanup Release SDK resources


[Notice]
When the compiling environment is VS2010, NETSDKCS library can support the version of .NET Framework 4.0 or newer. If you want to use the version older than .NET Framework 4.0, change the method of using MainWindow.xaml in Combobox. We will not support the modification.
Copy all file in the libs directory of General_NetSDK_ChnEng_CSharpXX_IS_VX.XXX.XXXXXXXX.X.R.XXXXXX to the build directory of bin directory of the corresponding demo programs.

【演示程序功能】
1.演示程序介绍了SDK初始化、登陆设备、登出设备、卡数据、指纹数据、人脸模板数据的增删改查等功能。


	  
【接口列表】
NETClient.Init SDK初始化与设置断线回调
NETClient.SetAutoReconnect 设置自动重连回调
NETClient.Login 设备登录
NETClient.Logout 设备登出
NETClient.FindRecord 查询考勤机的用户信息
NETClient.FindNextRecord 继续查询考勤机的用户信息
NETClient.FaceInfoOpreate 人脸模板数据操作
NETClient.ControlDevice 卡/指纹数据操作
NETClient.Attendance_InsertFingerByUserID 添加指纹数据
NETClient.Attendance_GetFingerByUserID 查询指纹数据
NETClient.Cleanup SDK释放资源


【注意事项】
1、编译环境为VS2010，NETSDKCS库最低支持.NET Framework 4.0。
2、请把General_NetSDK_ChnEng_CSharpXX_IS_VX.XXX.XXXXXXXX.X.R.XXXXXX中libs目录中的所有文件复制到对应该演示程序中bin目录下的生成目录中。
