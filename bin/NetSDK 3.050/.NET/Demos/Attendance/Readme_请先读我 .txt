[Introduction]
The demo program introduces login, querying card, adding card, modifying card, deleting card, getting fingerprint by user ID, adding fingerprint by user ID, removing fingerprint by user ID, querying and removing fingerprint by fingerprint ID, attaching event uploading, receiving event uploading during swiping card.

[Interfaces]
NETClient.Init Initialize SDK
NETClient.Login Login
NETClient.Logout Logout
NETClient.RealLoadPicture  Attach event
NETClient.StopLoadPic Detach event
NETClient.Attendance_GetUser Get user???
NETClient.Attendance_FindUser Find user???
NETClient.Attendance_AddUser  Add user
NETClient.Attendance_ModifyUser Modify user
NETClient.Attendance_RemoveFingerByUserID Remove fingerprint by user ID
NETClient.Attendance_GetFingerByUserID Get fingerprint by user ID
NETClient.Attendance_InsertFingerByUserID Insert fingerprint by user ID
NETClient.Attendance_GetFingerRecord  Get fingerprint record by fingerprint ID
NETClient.Attendance_RemoveFingerRecord Remove fingerprint record by fingerprint ID
NETClient.SetDVRMessCallBack Set alarm callback
NETClient.StartListen Start listening
NETClient.StopListen Stop listening
NETClient.ControlDevice Collect fingerprint

[Notice]
When the compiling environment is VS2010, NETSDKCS library can support the version of .NET Framework 4.0 or newer. If you want to use the version older than .NET Framework 4.0, change the method of using NetSDK.cs in IntPtr.Add. We will not support the modification.
The demo program supports the ordinary attendance machine only, and cannot support IR Face attendance machine .
Copy all file in the libs directory of General_NetSDK_ChnEng_CSharpXX_IS_VX.XXX.XXXXXXXX.X.R.XXXXXX to the build directory of bin directory of the corresponding demo programs.

【演示程序功能】
1、演示程序介绍了登录设备、查询卡、增加卡、修改卡、删除卡、通过用户ID查询指纹、增加指纹、删除指纹、通用指纹ID查询指纹和删除指纹、订阅事件上报、接收刷卡时事件信息上报功能。

【接口列表】
NETClient.Init SDK初始化
NETClient.Login 设备登录
NETClient.Logout 设备登出
NETClient.RealLoadPicture 订阅事件
NETClient.StopLoadPic 停止订阅事件
NETClient.Attendance_GetUser 查询用户
NETClient.Attendance_FindUser 查询用户
NETClient.Attendance_AddUser 增加用户
NETClient.Attendance_ModifyUser 修改用户
NETClient.Attendance_RemoveFingerByUserID 通过用户ID删除指纹
NETClient.Attendance_GetFingerByUserID 通过用户ID查询指纹
NETClient.Attendance_InsertFingerByUserID 通过用户ID插入指纹
NETClient.Attendance_GetFingerRecord 通过指纹ID获取指纹信息
NETClient.Attendance_RemoveFingerRecord 通过指纹ID删除指纹
NETClient.SetDVRMessCallBack 设置报警回调
NETClient.StartListen 开始监听
NETClient.StopListen 停止监听
NETClient.ControlDevice 采集指纹

【注意事项】
1、编译环境为VS2010，NETSDKCS库最低只支持.NET Framework 4.0,如用户需要支持低于4.0的版本需要更改NetSDK.cs文件中使用到IntPtr.Add的方法,我们不提供修改。
2、此演示程序仅支持普通考勤机产口，不支持红外人脸考勤机。
3、请把General_NetSDK_ChnEng_CSharpXX_IS_VX.XXX.XXXXXXXX.X.R.XXXXXX中libs目录中的所有文件复制到对应该演示程序中bin目录下的生成目录中。
