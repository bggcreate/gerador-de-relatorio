[Introduction]
The demo program introduces login, logout, starting real time monitor, stopping real time monitor, attaching event, detaching event, displaying uploaded face recognition and face detection event, querying  face library, adding  face library, modifying  face library, deleting face library, querying user, adding user, modifying user, deleting user, and arming in channels according to face library, and deleting face library in armed channels.

[Interfaces]
NETClient.Init Initialize SDK and set disconnection callback
NETClient.Login Login
NETClient.Logout Logout
NETClient.RealLoadPicture  Attach event
NETClient.StopLoadPic Detach event
NETClient.RealPlay Start real time monitor
NETClient.RenderPrivateData Display/Not display headframe
NETClient.StopRealPlay Stop real time monitor
NETClient.FindGroupInfo Query face library
NETClient.OperateFaceRecognitionGroup Add, modify and delete face library
NETClient.StartFindFaceRecognition Start querying user
NETClient.StopFindFaceRecognition Stop querying user
NETClient.DoFindFaceRecognition Query user
NETClient.AttachFaceFindState Attach querying user
NETClient.DetachFaceFindState Detach  querying user
NETClient.OperateFaceRecognitionDB Add, modify and delete user
NETClient.FaceRecognitionPutDisposition Arm according to the face library
NETClient.FaceRecognitionDelDisposition Disarm according to the face library
NETClient.Cleanup Release SDK resources

[Notice]
When the compiling environment is VS2010, NETSDKCS library can support the version of .NET Framework 4.0 or newer. If you want to use the version older than .NET Framework 4.0, change the method of using NetSDK.cs in IntPtr.Add. We will not support the modification.
The demo program does not support arming face library according to channels.
The demo program does not support image search.
Copy all file in the libs directory of General_NetSDK_ChnEng_CSharpXX_IS_VX.XXX.XXXXXXXX.X.R.XXXXXX to the build directory of bin directory of the corresponding demo programs.

【演示程序功能】
1、演示程序介绍设备登录、设备登出、打开实时监视、关闭实时监视、订阅事件、取消订阅事件、人脸识别与人脸检测事件上报信息显示、人脸库的查询、增加人脸库、修改人脸库、删除人脸库、人脸库中人员的查询、增加人员、修改人员、删除人员、按人脸库布控到通道中、人脸库在已布控的通道中删除。

【接口列表】
NETClient.Init SDK初始化与设置断线回调
NETClient.Login 登录设备
NETClient.Logout 登出设备
NETClient.RealLoadPicture 订阅事件
NETClient.StopLoadPic 停止订阅事件
NETClient.RealPlay 实时监视
NETClient.RenderPrivateData 显示与不显示人头框
NETClient.StopRealPlay 停止实时监视
NETClient.FindGroupInfo 查询人脸库
NETClient.OperateFaceRecognitionGroup 增加、修改、删除人脸库
NETClient.StartFindFaceRecognition 开始查询人员
NETClient.StopFindFaceRecognition停止查询人员
NETClient.DoFindFaceRecognition 查询人员
NETClient.AttachFaceFindState 订阅查询人员
NETClient.DetachFaceFindState 取消订阅查询人员
NETClient.OperateFaceRecognitionDB 增加、修改、删除人员
NETClient.FaceRecognitionPutDisposition 按人脸库布控
NETClient.FaceRecognitionDelDisposition 按人脸库撤控
NETClient.Cleanup 释放SDK资源

【注意事项】
1、编译环境为VS2010，NETSDKCS库最低只支持.NET Framework 4.0,如用户需要支持低于4.0的版本需要更改NetSDK.cs文件中使用到IntPtr.Add的方法,我们不提供修改。
2、此演示程序不支持按通道布控人脸库。
3、此演示程序不支持以图搜图。
4、请把General_NetSDK_ChnEng_CSharpXX_IS_VX.XXX.XXXXXXXX.X.R.XXXXXX中libs目录中的所有文件复制到对应该演示程序中bin目录下的生成目录中。