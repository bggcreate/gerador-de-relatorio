[Introduction]
The demo program introduce login, logout, starting real time monitor, stopping real time monitor, attaching event, detaching event, displaying uploaded event, querying card , adding card(with face), modifying card (with face), deleting card and clearing card.

[Interfaces]
NETClient.Init  Initialize SDK and set disconnection callback
NETClient.SetAutoReconnect Set reconnection callback
NETClient.Login Login
NETClient.Logout Logout
NETClient.GetDevConfig Get the time zone
NETClient.RealPlay Start real time monitor
NETClient.RenderPrivateData Display/Not display headframe
NETClient.StopRealPlay Stop monitor
NETClient.RealLoadPicture  Attach event
NETClient.StopLoadPic Detach event
NETClient.FindRecord  Query card
NETClient.FindNextRecord Query the details of card
NETClient.FindRecordClose Close card query
NETClient.ControlDevice Add, modify, delete and clear card
NETClient.FaceInfoOpreate Add, modify, delete and clear face

[Notice]
When the compiling environment is VS2010, NETSDKCS library can support the version of .NET Framework 4.0 or newer. If you want to use the version older than .NET Framework 4.0, change the method of using NetSDK.cs in IntPtr.Add. We will not support the modification.
The card in the demo program does not support fingerprint function.
Copy all file in the libs directory of General_NetSDK_ChnEng_CSharpXX_IS_VX.XXX.XXXXXXXX.X.R.XXXXXX to the build directory of bin directory of the corresponding demo programs.

【演示程序功能】
1、演示程序介绍设备登录、设备登出、打开实时监视、关闭实时监视、订阅事件、取消订阅事件、显示上报的事件信息、卡查询、增加卡(带人脸)、修改卡(带人脸)、删除卡、清空卡。

【接口列表】
NETClient.Init SDK初始化与设置断线回调
NETClient.SetAutoReconnect 设置重连回调
NETClient.Login 登录设备
NETClient.Logout 登出设备
NETClient.GetDevConfig 获取时区
NETClient.RealPlay 实时监视
NETClient.RenderPrivateData 显示与不显示人头框
NETClient.StopRealPlay 停止监视
NETClient.RealLoadPicture 订阅事件
NETClient.StopLoadPic 停止订阅事件
NETClient.FindRecord 查询卡
NETClient.FindNextRecord 查询卡详细信息
NETClient.FindRecordClose 关闭查询
NETClient.ControlDevice 增加卡、修改卡、删除卡、清空卡
NETClient.FaceInfoOpreate 增加人脸、修改人脸、删除人脸、清空人脸

【注意事项】
1、编译环境为VS2010，NETSDKCS库最低只支持.NET Framework 4.0,如用户需要支持低于4.0的版本需要更改NetSDK.cs文件中使用到IntPtr.Add的方法,我们不提供修改。
2、演示程序卡中不带指纹功能。
3、请把General_NetSDK_ChnEng_CSharpXX_IS_VX.XXX.XXXXXXXX.X.R.XXXXXX中libs目录中的所有文件复制到对应该演示程序中bin目录下的生成目录中。