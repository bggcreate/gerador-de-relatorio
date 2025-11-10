[Introduction]
The demo program introduces login, logout, starting real time monitor, stopping real time monitor, opening voice intercom, closing voice intercom, opening door, closing door, listening event, stopping listening, displaying uploaded event, querying card (with fingerprint), adding card(with fingerprint and face), modifying card (with fingerprint and face), deleting card, clearing card, and collecting fingerprint.

[Interfaces]
NETClient.Init Initialize SDK
NETClient.SetDVRMessCallBack Set alarm callback
NETClient.Login Login
NETClient.Logout Logout
NETClient.GetDevConfig Get the time zone
NETClient.RealPlay Start real time monitor
NETClient.StopRealPlay Stop monitor
NETClient.SetDeviceMode  Set mode
NETClient.SetDeviceMode  Start voice intercom
NETClient.RecordStart Start PC sound record
NETClient.RecordStart Stop PC sound record
NETClient.StopTalk  Stop voice intercom
NETClient.TalkSendData Send audio data to the device
NETClient.AudioDecEx  Audio decode
NETClient.ControlDevice Open door, close door, add card, modify card, delete card, clear card, and collect fingerprint
NETClient.StartListen Start listening event
NETClient.StopListen Stop listening event
NETClient.FindRecord  Query card
NETClient.FindNextRecord Query the details of the card
NETClient.QueryDevState Get fingerprint
NETClient.FindRecordClose Close query
NETClient.FaceInfoOpreate Add, modify, delete and clear face
NETClient.Cleanup Release SDK resources

[Notice]
When the compiling environment is VS2010, NETSDKCS library can support the version of .NET Framework 4.0 or newer. If you want to use the version older than .NET Framework 4.0, change the method of using NetSDK.cs in IntPtr.Add. We will not support the modification.
Copy all file in the libs directory of General_NetSDK_ChnEng_CSharpXX_IS_VX.XXX.XXXXXXXX.X.R.XXXXXX to the build directory of bin directory of the corresponding demo programs.

【演示程序功能】
1、演示程序介绍设备登录、设备登出、打开实时监视、关闭实时监视、打开对讲、关闭对讲、开门、关门、监听事件、取消监听、显示事件上报信息、查询卡(带指纹信息)、增加卡(带指纹和人脸图片)、修改卡(带指纹和人脸图片)、删除卡、清空卡、采集指纹。

【接口列表】
NETClient.Init SDK初始化
NETClient.SetDVRMessCallBack 设置报警回调
NETClient.Login 登录设备
NETClient.Logout 登出设备
NETClient.GetDevConfig 获取时区
NETClient.RealPlay 实时监视
NETClient.StopRealPlay 停止监视
NETClient.SetDeviceMode 设置模式
NETClient.StartTalk 开始对讲
NETClient.RecordStart 开始录音
NETClient.RecordStop 停止录音
NETClient.StopTalk 停止对讲
NETClient.TalkSendData 发送语音数据
NETClient.AudioDecEx 语音数据解码
NETClient.ControlDevice 开门、关门、增加卡、修改卡、删除卡、清空卡、指纹采集
NETClient.StartListen 开始监听事件
NETClient.StopListen 停止监听事件
NETClient.FindRecord 查询卡
NETClient.FindNextRecord 查询卡详细信息
NETClient.QueryDevState 获取指纹信息
NETClient.FindRecordClose 关闭查询
NETClient.FaceInfoOpreate 增加人脸、修改人脸、删除人脸、清空人脸
NETClient.Cleanup 释放SDK资源

【注意事项】
1、编译环境为VS2010，NETSDKCS库最低只支持.NET Framework 4.0,如用户需要支持低于4.0的版本需要更改NetSDK.cs文件中使用到IntPtr.Add的方法,我们不提供修改。
2、请把General_NetSDK_ChnEng_CSharpXX_IS_VX.XXX.XXXXXXXX.X.R.XXXXXX中libs目录中的所有文件复制到对应该演示程序中bin目录下的生成目录中。
