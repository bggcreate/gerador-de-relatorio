[Introduction]
The demo program introduces SDK initialization,Login, logout, auto reconnection, monitor, opening barrier gate, attaching intelligent traffic event, receiving intelligent traffic event, decoding intelligent traffic event, displaying part information of intelligent traffic event, displaying big image, displaying small image, manual snapshot, and detaching intelligent traffic event.
The demo program demonstrates the following events: vehicle proceeding through red light, traffic junction, speeding, low speed, manual snapshot, parking place available, parking place unavailable, crossing solid white line, wrong-way driving, illegal turning left, illegal turning right, illegal U turn, illegal parking, disobey lane direction sign, illegal changing lane, crossing solid yellow line, heavy vehicle in lane, not giving way to pedestrian, vehicle in lane, driving in bus line, illegal reversing and not wearing seat belt.

[Interfaces]
NETClient.Init  Initialize SDK and set disconnection callback
NETClient.SetAutoReconnect Set reconnection callback
NETClient.Login Login
NETClient.Logout Logout
NETClient.RealPlay Start real time monitor
NETClient.StopRealPlay Stop real time monitor
NETClient.ControlDevice Open barrier gate and manual snapshot
NETClient.RealLoadPicture  Attach event
NETClient.StopLoadPic Detach event
NETClient.Cleanup Release SDK resources

[Notice]
When the compiling environment is VS2010, NETSDKCS library can support the version of .NET Framework 4.0 or newer. If you want to use the version older than .NET Framework 4.0, change the method of using NetSDK.cs in IntPtr.Add. We will not support the modification.
When receiving the images of the intelligent events, the demo program receives 2 M image cache by default. So it might miss some events with large imaged. Call SetNetworkParam function after initializing SDK to modify the size of the image cache, which cannot be larger than 8 M.
The demo program demonstrates the attach function of listening the single channel and single device only, and it does not support the attach function of listening multiple channels and multiple devices. If you need the the attach function of listening multiple channels and multiple devices, modify it by yourself.
The demo program demonstrates parts of general intelligent traffic events functions only. If you need the other intelligent traffic events functions, add it by yourself.
Copy all file in the libs directory of General_NetSDK_ChnEng_CSharpXX_IS_VX.XXX.XXXXXXXX.X.R.XXXXXX to the build directory of bin directory of the corresponding demo programs.


【演示程序功能】
1、演示程序介绍了SDK初始化、登陆设备、登出设备、自动重连、监视、打开道闸、订阅智能交通事件、接收智能交通事件信息、解析智能交通事件信息、显示智能交通事件部分信息、显示大图、显示小图、手动抓拍、取消订阅智能交通事件功能。
2、演示程序演示了闯红灯事件、卡口事件、超速事件、低速事件、手动抓拍事件、无车位事件、有车位事件、压车道线事件、逆行事件、违章左转事件、违章右转事件、违章掉头事件、违章停车事件、不按车道行事件、违章变道事件、压黄线事件、黄牌车占道事件、未礼让行人事件、有车占道事件、占用公交车道事件、违章倒车事件、未系安全带事件。

【接口列表】
NETClient.Init SDK初始化与设置断线回调
NETClient.SetAutoReconnect 设置重连回调
NETClient.Login 登录设备
NETClient.Logout 登出设备
NETClient.RealPlay 实时监视
NETClient.StopRealPlay 停止实时监视
NETClient.ControlDevice 打开道闸与手动抓拍
NETClient.RealLoadPicture 订阅事件
NETClient.StopLoadPic 停止订阅事件
NETClient.Cleanup 释放SDK资源

【注意事项】
1、编译环境为VS2010，NETSDKCS库最低只支持.NET Framework 4.0,如用户需要支持低于4.0的版本需要更改NetSDK.cs文件中使用到IntPtr.Add的方法,我们不提供修改。
2、此演示程序接收智能事件图片时，采用默认的2M接收图片缓存，如部分事件图片过大，可能接收不到事件，请用户在SDK初始化后调用SetNetworkParam函数，修改接收图片缓存的大小，最大可支持到8M。
3、此演示程序只演示监听单设备单通道订阅功能，不支持多设备多通道订阅功能，如用户有需求请自行修改。
4、此演示程序仅演示部分通用的智能交通事件功能，如需另外智能交通事件功能，请自行添加。
5、请把General_NetSDK_ChnEng_CSharpXX_IS_VX.XXX.XXXXXXXX.X.R.XXXXXX中libs目录中的所有文件复制到对应该演示程序中bin目录下的生成目录中。
