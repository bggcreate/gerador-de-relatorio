[Introduction]
The demo introduces SDK initialization, login, logout, starting voice intercom and stopping voice intercom.
The demo program demonstrates two modes: Direct connection and device forwarding.

[Interfaces]
NETClient.Init Initialize SDK and set disconnection callback
NETClient.Login Login
NETClient.Logout Logout
NETClient.SetDeviceMode Set mode
NETClient.SetDeviceMode Start voice intercom
NETClient.RecordStart Start PC sound record
NETClient.RecordStart Stop PC sound record
NETClient.StopTalk Stop voice intercom
NETClient.TalkSendData Send audio data to the device
NETClient.AudioDec  Audio decode
NETClient.Cleanup Release SDK resources

[Notice]
When the compiling environment is VS2010, NETSDKCS library can support the version of .NET Framework 4.0 or newer. If you want to use the version older than .NET Framework 4.0, change the method of using NetSDK.cs in IntPtr.Add. We will not support the modification.
The demo program does not support multiple devices login.
The demo program supports the voice intercom of Dahua send-generation protocol.
Device forward mode is a mode to realize the voice intercom between client and camera through the voice intercom between client and camera. The client does not connect to camera directly, and it connects to storage device, and storage device connects to camera. The audio data is forwarded to camera by storage device during voice intercom.
Copy all file in the libs directory of General_NetSDK_ChnEng_CSharpXX_IS_VX.XXX.XXXXXXXX.X.R.XXXXXX to the build directory of bin directory of the corresponding demo programs.

【演示程序功能】
1、演示程序介绍了SDK初始化、登陆设备、登出设备、开始对讲，关闭对讲功能。
2、演示程序演示可直连设备和设备转发二种模式。

【接口列表】
NETClient.Init SDK初始化与设置断线回调
NETClient.Login 登录设备
NETClient.Logout 登出设备
NETClient.SetDeviceMode 设置模式
NETClient.StartTalk 开始对讲
NETClient.RecordStart 开始录音
NETClient.RecordStop 停止录音
NETClient.StopTalk 停止对讲
NETClient.TalkSendData 发送语音数据
NETClient.AudioDec 语音解码
NETClient.Cleanup 释放SDK资源

【注意事项】
1、编译环境为VS2010，NETSDKCS库最低只支持.NET Framework 4.0,如用户需要支持低于4.0的版本需要更改NetSDK.cs文件中使用到IntPtr.Add的方法,我们不提供修改。
2、此演示程序不支持多设备登陆。
3、此演示程序只支持大华二代协议对讲。
4、设备转发模式是指通过客户端与前端设备对讲，客户端没有与前端设备直连，客户端与存储设备直连，存储设备与前端设备连接，对讲的时候语音数据通过存储设备转发给前端设备，实现客户端与前端设备对讲的模式。
5、请把General_NetSDK_ChnEng_CSharpXX_IS_VX.XXX.XXXXXXXX.X.R.XXXXXX中libs目录中的所有文件复制到对应该演示程序中bin目录下的生成目录中。


